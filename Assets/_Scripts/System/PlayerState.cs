using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class RaycastHitComparer : IComparer<RaycastHit>
{
    public int Compare(RaycastHit x, RaycastHit y)
    {
        return x.distance.CompareTo(y.distance);
    }
}

public enum DebugEquipWeapon
{
    Pistol,
    Shotgun,
    SMG

}


/* A Player State is the state of a participant in the game.
 * Some examples of player information included:
 *   Name
 *   Current level
 *   Health
 *   Score
 * Player States for all players exist on all machines and can replicate data from the server to the client to keep things in sync.
*/
public class PlayerState : NetworkBehaviour, IDamageable
{
    [Command]
    private void CmdStartLocalPlayer(ulong steamIdUlong)
    {
        _steamIdUlong = steamIdUlong;
        SteamId = new CSteamID(steamIdUlong);
        _nickname = SteamFriends.GetFriendPersonaName(SteamId);
        GameState.Instance.AddPlayer(steamIdUlong, netId);
    }
    //public override void OnStartLocalPlayer()
    //{
    //    Debug.Log("On player state start local player");
    //    // CmdSetSteamPersonaInfo(SteamUser.GetSteamID().m_SteamID, SteamFriends.GetPersonaName());
    //}
    public override void OnStopServer()
    {
        GameState.Instance.RemovePlayer(_steamIdUlong);
    }

    private void Awake()
    {
        _charaAnimHandler = GetComponent<CharacterAnimHandler>();      
    }
    private void Start()
    {
        if (isLocalPlayer)
        {
            Debug.Log("On player state start.");
            CmdStartLocalPlayer(SteamUser.GetSteamID().m_SteamID);
            onHealthChanged += (val) => UI_GameHUD.Instance.SetHealth(val);
            _cUpdatePing = StartCoroutine(UpdatePing());
        }
    }

    [Header("Components")]
    [SerializeField] private Transform _tpSocketWeaponLeft;
    [SerializeField] private Transform _tpSocketWeaponRight;
    [SerializeField] private Transform _fpSocketWeaponLeft;
    [SerializeField] private Transform _fpSocketWeaponRight;
    [SerializeField] private AudioSource _weaponAudioSource;
    [SerializeField] public BeatHUD _beatHUDComponent;
    [SerializeField] private GameObject _scoreUI;


    // [SerializeField] private Animator _firstPersonAnimator;
    // [SerializeField] private Animator _thirdPersonAnimator;
    private List<IObserver> _observers = new List<IObserver>();
    private CharacterAnimHandler _charaAnimHandler;
    private readonly int _aFire = Animator.StringToHash("Fire");
    private readonly int _aReload = Animator.StringToHash("Reload");
    private readonly int _aUnholster = Animator.StringToHash("Unholster");
    private readonly int _aInspect = Animator.StringToHash("Inspect");
    private readonly int _aUninspect = Animator.StringToHash("Uninspect");
    public bool isFiring;

   

    [HideInInspector]public DebugEquipWeapon startWeapon = DebugEquipWeapon.Pistol;

    private List<WeaponInHand> weaponInventory = new();
    
    [Header("General Settings")]
    [SerializeField] private LayerMask _shootingLayer;

    // ----------------------------
    //  Steam status
    // ----------------------------
    [SyncVar(hook = nameof(OnSteamIdUlongChanged))] private ulong _steamIdUlong;
    public CSteamID SteamId { get; private set; }
    public Action<string> onNicknameChanged;
    private void OnSteamIdUlongChanged(ulong oldVal, ulong newVal)
    {
        SteamId = new CSteamID(newVal);
    }
    [SyncVar(hook = nameof(OnNicknameChanged))] private string _nickname;
    public string Nickname => _nickname;
    private void OnNicknameChanged(string oldStr, string newStr)
    {
        onNicknameChanged?.Invoke(newStr);
    }

    [SyncVar(hook = nameof(OnBodyColourChanged))][HideInInspector] public Color bodyColour;


    // WeaponRangeType.SHORT = 0 ; WeaponRangeType.MEDIUM = 1 ; WeaponRangeType.LONG = 2
    [SyncVar][HideInInspector] private int _curWpnIndex = -1;
    // [SyncVar(hook = nameof(OnCurWpnDbIndexChanged))][HideInInspector] public int curWpnDbIndex = -1;

    public WeaponIdentityData[] inventoryWeapons = new WeaponIdentityData[3]; //4 weapons
    public WeaponIdentityData CurrentWeaponIdentity
    {
        get
        {
            if (_curWpnIndex >= 0) return inventoryWeapons[_curWpnIndex];
            else return null;
        }
    }
    private GameObject _curWpnObj;
    public WeaponInHand CurrentWeaponInHand
    {
        get
        {
            if (_curWpnObj != null) return _curWpnObj.GetComponent<WeaponInHand>();
            else return null;
        }
    }
    public int CurrentWeaponDatabaseIndex => GameManager.GetWeaponDataIndex(CurrentWeaponIdentity.Data);

    [Command]
    private void CmdEquipWeapon(int index, int dbIndex, int ammo, int backupAmmo)
    {
        _curWpnIndex = index;
        RpcEquipWeapon(dbIndex, ammo, backupAmmo);
    }
    [ClientRpc]
    private void RpcEquipWeapon(int dbIndex, int ammo, int backupAmmo)
    {
        if (_curWpnObj != null) Destroy(_curWpnObj);
        WeaponData data = GameManager.GetWeaponData(dbIndex);
        string path = Path.Combine("Weapons", "InHand", data.WeaponName);

        if (isLocalPlayer)
        {
            _curWpnObj = Instantiate(Resources.Load<GameObject>(path), _fpSocketWeaponRight);
            foreach (var item in _curWpnObj.GetComponentsInChildren<Renderer>())
            {
                item.shadowCastingMode = ShadowCastingMode.Off;
            }
            _curWpnObj.GetComponent<WeaponInHand>().Init(CurrentWeaponIdentity, GetComponent<LocalPlayerController>());

            
            UI_GameHUD.SetAmmo(ammo);
            UI_GameHUD.SetBackupAmmo(backupAmmo);
            UI_GameHUD.SetCrosshairWeaponSpread(data.CrosshairSpread);
        }
        else
        {
            _curWpnObj = Instantiate(Resources.Load<GameObject>(path), _tpSocketWeaponRight);
            _curWpnObj.GetComponent<WeaponInHand>().Init(new WeaponIdentityData(data, 0, 0), null);
        }

        Debug.Log("PlayerState.RpcEquipWeapon() done.");
    }

    public void PickUpWeapon(WeaponData data, int currentAmmo, int backupAmmo) // only called on the client in order to replenish the equivalent weapon's ammo when entering a weapon pickup's collider
    {
        UI_GameHUD.SetNewWeapon(data.WeaponID ,data.WeaponName);

        if (inventoryWeapons[data.WeaponID] != null)
        {
            inventoryWeapons[data.WeaponID].AddAmmo(currentAmmo + backupAmmo);
        }
        inventoryWeapons[data.WeaponID] = new WeaponIdentityData(data, currentAmmo, backupAmmo);

        if (!isHoldingWeapon())
        {
            GetComponent<LocalPlayerController>().SetFirstPersonVisible(true);
            EquipWeaponByIndex((int)data.WeaponID);
        }
        return;
    }



    public void GiveDebugWeapon()
    {
        RpcEquipWeapon(0, 30, 30);
    }

    private bool isHoldingWeapon()
    {

        if (_curWpnIndex < 0)
        {
            return false;
        } return true;
    }

    [Command]
    public void CmdSetBodyColour(Color colour) { bodyColour = colour; }
    private void OnBodyColourChanged(Color oldColour, Color newColour)
    {
        foreach (var item in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            item.material.color = newColour;
        }
    }

   
    public void ToggleScoreboard(bool b)
    {
        _scoreUI.SetActive(b); //this shows or hides the list with frags and deaths and assists
    }



    public void RelayBeat()
    {
        if (!isLocalPlayer)
        {
            Debug.LogWarning("For some reason, RelayBEatTick@LocalPlayerController ran on a player which is not local.");
            return;
        }
        _beatHUDComponent.DoBeatTick();
    }

    public void Shoot()
    {
        if (CurrentWeaponInHand != null && CurrentWeaponInHand._CanFire)
        {
            //PlayWeaponFireSound(CurrentWeaponDatabaseIndex);
            //_charaAnimHandler.FpSetTrigger(_aFire);
            //_charaAnimHandler.CmdTpSetTrigger(_aFire);
            //CurrentWeaponInHand.FireContinuously(out List<Vector3> directions);
            //UI_GameHUD.SetAmmo(CurrentWeaponIdentity.CurrentAmmo);
            //CmdFire(CurrentWeaponDatabaseIndex, Camera.main.transform.position, directions);
        }
    }
    public void FireStop()
    {
        CurrentWeaponInHand?.FireStop();
    }

    public void ToggleScope()
    {
        if (CurrentWeaponInHand != null && CurrentWeaponInHand.CanToggleScope())
        {
            CurrentWeaponInHand.ToggleScope();
        }        
    }

    public void EquipWeaponByIndex(int index) // only called on the client
    {
        if (inventoryWeapons[index] != null)
        {
            _curWpnIndex = index;

            _charaAnimHandler.FpSetTrigger(_aUnholster);
            _charaAnimHandler.CmdTpSetTrigger(_aUnholster);

            CmdEquipWeapon(index,
                GameManager.GetWeaponDataIndex(inventoryWeapons[index].Data),
                inventoryWeapons[index].CurrentAmmo,
                inventoryWeapons[index].BackupAmmo);
        }
    }
    [Command]
    public void CmdFire(int dbIndex, Vector3 origin, List<Vector3> directions)
    {
        RpcFireSound(dbIndex);
        WeaponData wpn = GameManager.GetWeaponData(dbIndex);
        RaycastHit[] hits = new RaycastHit[5];
        foreach (Vector3 dir in directions)
        {
            // Debug.DrawRay(origin, dir, Color.red, 20);
            int hitInfoLen = Physics.RaycastNonAlloc(origin, dir, hits, wpn.MaxRange, _shootingLayer);
            Array.Sort(hits, 0, hitInfoLen, new RaycastHitComparer());
            float attenuation = 1.0f;
            float dmg = 1.0f;
            for (int i = 0; i < hitInfoLen; i++)
            {
                // will not apply damage on self
                PlayerState ps = hits[i].transform.GetComponentInParent<PlayerState>();
                if (this == ps)
                    continue;

                // Apply Damage
                IDamageable d = hits[i].transform.GetComponentInParent<IDamageable>();
                if (null != d)
                {
                    Debug.Log("Get Damageable");
                    if (hits[i].transform.TryGetComponent(out CharacterBodyParts b))
                    {
                        switch (b.Part)
                        {
                            case BodyPart.HEAD: dmg = wpn.DamageHead; break;
                            case BodyPart.BODY: dmg = wpn.DamageBody; break;
                            case BodyPart.ARM: dmg = wpn.DamageArm; break;
                            case BodyPart.THIGH: dmg = wpn.DamageThigh; break;
                            case BodyPart.CALF: dmg = wpn.DamageCalf; break;
                        }
                    }
                    else dmg = wpn.BaseDamage;
                   
                    d.ApplyDamage(Mathf.Max(0, Mathf.RoundToInt(dmg * attenuation)), this, wpn, DamageType.SHOOT);
                }

                // Temperory: spawn decal               
                GameObject obj = Instantiate(Resources.Load<GameObject>("test"), hits[i].point, Quaternion.identity);
                NetworkServer.Spawn(obj);
                
            }
        }
    }

    [ClientRpc(includeOwner = false)]
    private void RpcFireSound(int dbIndex)
    {
        PlayWeaponFireSound(dbIndex);
    }
    private void PlayWeaponFireSound(int dbIndex)
    {
        //_weaponAudioSource.clip = GameManager.GetWeaponData(dbIndex).FireSound;
        //_weaponAudioSource.Play();
        _weaponAudioSource.PlayOneShot(GameManager.GetWeaponData(dbIndex).FireSound);
    }
    public void EquipScroll(int val)
    {
        int k;
        for (int i = 1; i < inventoryWeapons.Length; i++)
        {
            k = (_curWpnIndex + inventoryWeapons.Length + val * i) % inventoryWeapons.Length;
            if (inventoryWeapons[k] != null)
            {
                EquipWeaponByIndex(k);
                break;
            }
        }
    }
    public void OnUnholstered()
    {
        if (!IsAlive) return;
        if (null != CurrentWeaponInHand)
        {
            CurrentWeaponInHand.IsHolstered = false;
        }
    }

    public void StartReload()
    {
        if (!IsAlive) return;
        if (CurrentWeaponInHand != null && CurrentWeaponInHand.CanReload())
        {
            _charaAnimHandler?.FpSetTrigger(_aReload);
            _charaAnimHandler?.CmdTpSetTrigger(_aReload);
            CurrentWeaponInHand?.StartReload();
        }
    }
    public void ReloadAttachToHand(int attach)
    {
        if (!IsAlive) return;

        if (attach > 0)
            CurrentWeaponInHand?.RemoveMagazine(isLocalPlayer ? _fpSocketWeaponLeft : _tpSocketWeaponLeft);
        else
            CurrentWeaponInHand?.LoadMagazine();
    }
    public void Reload()
    {
        if (!IsAlive) return;
        CurrentWeaponInHand?.Reload();
    }
    public void EndReload()
    {
        if (!IsAlive) return;
        CurrentWeaponInHand?.EndReload();
    }


    #region Damage
    [SyncVar(hook = nameof(OnHealthChanged))] private int _health = 100;
    public Action<int> onHealthChanged;
    public int Health => _health;
    public void ApplyDamage(int amount, PlayerState instigator, object causer, DamageType type)
    {
        if (!IsAlive) return;
        if (GameState.Instance.Stage != GameStage.PLAYING) return;

        Debug.Log($"Current Health : {_health} ;;;;;; Applied damage : {amount}");
        _health = Mathf.Max(0, _health - amount);

        if (instigator != null) TargetGetDamage(instigator.netId, true);
        // TargetRefreshHealth(health);
        if (_health == 0)
        {
            if (instigator != null) instigator.AddKill();
            // dead
            gameObject.layer = LayerMask.NameToLayer("Dead");
            RpcDie();
            GameState.Instance.PlayerDied(netId);

            switch (type)
            {
                case DamageType.DEFAULT:
                    break;
                case DamageType.SHOOT:
                    LevelManager.Instance.BroadcastKillMessage(
                        instigator.Nickname,
                        Nickname,
                        GameManager.GetWeaponDataIndex(causer as WeaponData),
                        type);
                    break;
                case DamageType.EXPLOSION:
                    break;
                case DamageType.FALL:
                    LevelManager.Instance.BroadcastKillMessage("", Nickname, 0, type);
                    break;
                case DamageType.POISON:
                    LevelManager.Instance.BroadcastKillMessage("", Nickname, 0, type);
                    break;
                default:
                    break;
            }
        }
    }
    [TargetRpc]
    private void TargetGetDamage(uint netId, bool isByNetObj)
    {
        if (isByNetObj && NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            UI_GameHUD.Instance.SetDamaged(identity.transform);
        }
        else
        {
            UI_GameHUD.Instance.SetDamaged(null);
        }
    }
    private void OnHealthChanged(int oldVal, int newVal)
    {
        // if (isLocalPlayer) UI_GameHUD.Instance.SetHealth(newVal);
        onHealthChanged?.Invoke(newVal);
    }
    [Command]
    public void CmdSetSelfDamage(int amount, DamageType type)
    {
        ApplyDamage(amount, this, null, type);
    }
    #endregion

    #region Statistics
    [SyncVar(hook = nameof(OnPingChanged))] private int _ping = 0;
    public int Ping => _ping;
    public Action<int> onPingChanged;
    [Command]
    private void CmdSetPing(int val) { _ping = val; }
    private void OnPingChanged(int oldPing, int newPing) { onPingChanged?.Invoke(newPing); }
    private Coroutine _cUpdatePing;
    IEnumerator UpdatePing()
    {
        while (true)
        {
            CmdSetPing(Mathf.RoundToInt((float)(NetworkTime.rtt * 1000.0)));
            yield return new WaitForSeconds(1.0f);
        }
    }

    [SyncVar(hook = nameof(OnKillsChanged))] private int _kills = 0;
    public int Kills => _kills;
    public Action<int> onKillsChanged;
    [Command]
    private void CmdSetKills(int val) { _kills = val; }
    /// <summary>
    /// Server Onlet
    /// </summary>
    private void AddKill() { _kills++; }
    private void OnKillsChanged(int oldKill, int newKill) { onKillsChanged?.Invoke(newKill); }
    #endregion

    public bool IsAlive => _health > 0;
    // public Action onDied;
    [ClientRpc]
    public void RpcDie()
    {
        Debug.Log("Rpc Die");
        gameObject.layer = LayerMask.NameToLayer("Dead");
        if (_curWpnObj) Destroy(_curWpnObj);
        if (isLocalPlayer)
        {
            _charaAnimHandler.CmdTpSetLayerWeight(1, 0);
            _charaAnimHandler.CmdTpSetTrigger(Animator.StringToHash("Dead"));
            GetComponent<LocalPlayerController>().Die();
        }
    }





}
