using Mirror;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameHUD : MonoBehaviour
{
    private static UI_GameHUD instance;


    public static UI_GameHUD Instance => instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {        
        ClearInteractionHint();
    }

    

    [Header("Interaction")]
    [SerializeField] private TextMeshProUGUI _tmpInteraction;

    public static void AddInteractionHint(string content)
    {
        instance._tmpInteraction.SetText(content);
    }
    public static void ClearInteractionHint()
    {
        instance._tmpInteraction.SetText("");
    }

    [Header("Opposite Name")]
    [SerializeField] private TextMeshProUGUI _tmpAimedPlayerName;
    [SerializeField] private LayerMask _aimLayerMask;
    public void SetAimedPlayerName(string nickname)
    {
        instance._tmpAimedPlayerName.SetText(nickname);
    }

    //[Header("Inventory")]
    //[SerializeField] private UI_Panel_Inventory _inventoryScroller;
    //public static void SetNewWeapon(int index, string newName)
    //{
    //    instance._inventoryScroller.SetNewWeapon(index, newName);
    //}
    //public static void ActiveInventorySlot(int index)
    //{
    //    instance._inventoryScroller.ActiveSlot(index);
    //}

    [Header("Ammo")]
    [SerializeField] private GameObject _pnlAmmo;

    public GameObject _crosshair;
    public GameObject _inventoryScroller;

    //public static void SetAmmo(int val)
    //{
    //    instance._tmpAmmo.SetText(val.ToString());
    //}
    //public static void SetBackupAmmo(int val)
    //{
    //    instance._tmpBackupAmmo.SetText(val.ToString());
    //}

    //[Header("Crosshair")]
    //[SerializeField] private UI_Crosshair _crosshair;
    //public static void SetCrosshairActive(bool active)
    //{
    //    instance._crosshair.gameObject.SetActive(active);
    //}
    //public static void SetCrosshairWeaponSpread(float pixel)
    //{
    //    if (instance._crosshair.gameObject.activeSelf)
    //    {
    //        instance._crosshair.WeaponSpread = pixel;
    //    }
    //}
    //public static void SetCrosshairMovementSpread(float pixel)
    //{
    //    if (instance._crosshair.gameObject.activeSelf)
    //    {
    //        instance._crosshair.MovementSpread = pixel;
    //    }
    //}
    //public static void SetCrosshairFireSpread(float pixel, float duration)
    //{
    //    if (instance._crosshair.gameObject.activeSelf)
    //    {
    //        instance._crosshair.SetFireSpread(pixel, duration);
    //    }
    //}

    [Header("Personal")]
    [SerializeField] private GameObject _pnlPersonal;
    [SerializeField] private TextMeshProUGUI _tmpHealth;
    [SerializeField] private Color _hpColor1 = Color.white;
    [SerializeField] private Color _hpColor2 = Color.yellow;
    [SerializeField] private Color _hpColor3 = Color.red;
    [SerializeField] private TextMeshProUGUI _tmpArmor;

    [SerializeField]
    private GameObject _statistics;


    public void SetHealth(int val)
    {
        instance._tmpHealth.SetText(val.ToString());
        instance._tmpHealth.color = val >= 50 ? instance._hpColor1 :
            (val >= 20 ? instance._hpColor2 : instance._hpColor3);
    }
    public static void SetArmor(int val)
    {
        instance._tmpArmor.SetText(val.ToString());
    }

    //[Header("Damaged")]
    //[SerializeField] private UI_Game_Damaged _damaged;
    //public void RegisterPlayerTransform(Transform player)
    //{
    //    _damaged.PlayerTransform = player;
    //}
    //public void SetDamaged(Transform instigator)
    //{
    //    _damaged.SetDamaged(instigator);
    //}

    //[Header("Kill Message")]
    //[SerializeField] private UI_Game_KillMsg _killMsg;
    //public static void AddKillMessage(string killerName, string objectName, Sprite icon, DamageType type)
    //{
    //    instance._killMsg.AddKillMessage(killerName, objectName, icon, type);
    //}
    //[Header("Statistics")]
    //[SerializeField] private UI_Panel_Statistics _statistics;
    //public void SetStatisticsShown(bool shown)
    //{
    //    _statistics.SetShown(shown);
    //}
    //public void AddPlayerToStatistics(uint netId)
    //{
    //    _statistics.AddPlayerSlot(netId);
    //}
    //public void RemovePlayerFromStatistics(uint netId)
    //{
    //    _statistics.RemovePlayerSlot(netId);
    //}
    //private void InitPlayerStatistics()
    //{
    //    foreach (var item in GameState.Instance._playerNetIds)
    //    {
    //        AddPlayerToStatistics(item);
    //    }
    //}

    [Header("Round End")]
    [SerializeField] private GameObject _winnerPanel;
    [SerializeField] private RawImage _imgWinnerIcon;
    [SerializeField] private TextMeshProUGUI _tmpWinnerName;
    [SerializeField] private Button _btnReturnToLobby;

    public static void ShowWinner(PlayerBody ps)
    {

    }

    //public void OnClickReturnToLobby()
    //{
    //    // SteamLobby.SceneToLoad = "Lobby";
    //    MyNetworkManager.singleton.ServerChangeScene("Lobby");
    //}

    public static void SetUIEnabled(bool enabled)
    {
        instance._pnlAmmo.SetActive(enabled);
        instance._pnlPersonal.SetActive(enabled);
        instance._inventoryScroller.gameObject.SetActive(enabled);
        instance._crosshair.gameObject.SetActive(enabled);
    }

    private void FixedUpdate()
    {
        //if (null == GameState.Instance) return;
        //if (GameState.Instance.Stage != GameStage.PLAYING) return;

        //// check aiming player
        //if (Physics.Raycast(Camera.main.transform.position,
        //    Camera.main.transform.forward,
        //    out RaycastHit hit,
        //    150,
        //    _aimLayerMask))
        //{
        //    // Debug.Log(hit.transform.gameObject.name);
        //    if (hit.transform.TryGetComponent(out PlayerBody ps))
        //    {
        //       // SetAimedPlayerName(ps.Nickname);
        //        return;
        //    }            
        //}
        //SetAimedPlayerName("");
    }
}
