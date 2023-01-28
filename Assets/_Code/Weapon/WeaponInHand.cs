using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class WeaponIdentityData
{
    public WeaponIdentityData(WeaponData data, int currentAmmo, int backupAmmo)
    {
        Data = data;
        CurrentAmmo = currentAmmo;
        BackupAmmo = backupAmmo;
    }

    public WeaponData Data { get; set; }
    public int CurrentAmmo { get; set; }
    public int BackupAmmo { get; set; }

    public void AddAmmo(int q)
    {
        BackupAmmo += Mathf.Clamp(q, 0, Data.BackupAmmo); //adds the ammo but not over the max capacity.
    }
}
public class WeaponInHand : MonoBehaviour
{
    //[SerializeField] private Transform _magazine;
    //private Transform _magazineOriginalParent;
    //private Vector3 _magazinOriginalPosition;
    //private Quaternion _magazineOriginalRotation;
    //public void LoadMagazine()
    //{
    //    _magazine.SetParent(_magazineOriginalParent);
    //    _magazine.SetLocalPositionAndRotation(_magazinOriginalPosition, _magazineOriginalRotation);
    //}
    //public void RemoveMagazine(Transform hand)
    //{
    //    _magazine.SetParent(hand);
    //}

    //[SerializeField] private Transform _muzzle;
    //public Transform Muzzle => _muzzle;

    //protected WeaponIdentityData _identity;
    //public WeaponIdentityData Identity => _identity;
    //protected LocalPlayerController _playerCtrl;
    //public void Init(WeaponIdentityData identity, LocalPlayerController controller)
    //{
    //    _identity = identity;
    //    _playerCtrl = controller;
    //}
    //private void Awake()
    //{
    //    _magazineOriginalParent = _magazine.parent;
    //    _magazinOriginalPosition = _magazine.localPosition;
    //    _magazineOriginalRotation = _magazine.localRotation;
    //}
    ////protected virtual void Start()
    ////{
    ////    if (_identity == null || _playerCtrl == null)
    ////        return;
    ////    SetThingsByScopeLevel(0);
    ////}
    //public bool IsHolstered { get; set; } = true;
    //protected bool _isFiring = false;
    //protected bool _isReloading = false;
    //protected bool _isInspected = false;
    //protected float _recoilValue;

    //public bool _CanFire
    //{

    //    get
    //    {
    //        if (Identity.Data.Ammo > 0)
    //        {
    //            return true;
    //        } return false;
    //    }

    //}
    //protected float RecoilValue
    //{
    //    get { return _recoilValue; }
    //    set
    //    {
    //        _recoilValue = Mathf.Clamp(value, 1, _identity.Data.Ammo);
    //    }
    //}
    
    //protected virtual float FireSpreadRadius => GetFireSpreadRadius(_identity.Data.FireSpread);
    //protected float GetFireSpreadRadius(float baseSpread)
    //{
    //    float gain = 0.0f;
    //    float multiplier = 1.0f;
    //    if (!_playerCtrl.CharaMovementComp.IsOnGround)
    //    {
    //        gain = _identity.Data.InAirSpreadGain;
    //        multiplier = _identity.Data.InAirSpreadMultiplier;
    //    }
    //    else if (_playerCtrl.CharaMovementComp.LastMovementInput.sqrMagnitude != 0)
    //    {
    //        if (_playerCtrl.CharaMovementComp.IsCrouching)
    //        {
    //            gain = _identity.Data.CrouchingSpreadGain;
    //            multiplier = _identity.Data.CrouchingSpreadMultiplier;
    //        }
    //        else if (_playerCtrl.CharaMovementComp.IsWalking)
    //        {
    //            gain = _identity.Data.WalkingSpreadGain;
    //            multiplier = _identity.Data.WalkingSpreadMultiplier;
    //        }
    //        else
    //        {
    //            gain = _identity.Data.JoggingSpreadGain;
    //            multiplier = _identity.Data.JoggingSpreadMultiplier;
    //        }
    //    }
    //    return (baseSpread + gain) * multiplier;
    //}
    //public virtual bool CanFireBurst()
    //{
    //    if (_identity.CurrentAmmo <= 0)
    //    {
    //        // do something : cannot fire burst because current ammo is not enough
    //        FireStop();
    //        return false;
    //    }
    //    if (IsHolstered)
    //    {
    //        return false;
    //    }
    //    if (_isReloading)
    //    {
    //        // do something : cannot fire burst because of _isReloading
    //        return false;
    //    }
    //    if (_isFiring)
    //    {
    //        //  do something : cannot fire burst because of _isFiring;
    //        return false;
    //    }
    //    return true;
    //}
    ////protected virtual void HitScan(out List<Vector3> directions)
    ////{
    ////    //Vector3 dir = _playerCtrl.FirstPersonForward;
    ////    //Vector3 center = Camera.main.transform.position + dir;
    ////    //float r = Random.Range(0f, FireSpreadRadius);
    ////    //float angle = Random.Range(0, Mathf.PI * 2);
    ////    //center.x += Mathf.Cos(angle) * r;
    ////    //center.y += Mathf.Sin(angle) * r;
    ////    //directions = new List<Vector3>();
    ////    //directions.Add(center - Camera.main.transform.position);
    ////}
    ////public virtual void FireContinuously(out List<Vector3> directions)
    ////{
    ////    //HitScan(out directions);
    ////}
    //public virtual void FireStop()
    //{
    //    if (!_isFiring) return;
    //    _isFiring = false;

    //}
    //private IEnumerator ContinuousFiringDelay()
    //{
    //    yield return new WaitForSeconds(_identity.Data.FireDelay);
    //    _isFiring = false;
    //}
    //public virtual bool CanToggleScope()
    //{
    //    return false;
    //}
    //public virtual void ToggleScope()
    //{
    //    Debug.Log("Toggle Scope!");
    //}
    //public bool CanReload()
    //{
    //    return !IsHolstered && !_isReloading && _identity.CurrentAmmo < _identity.Data.Ammo && _identity.BackupAmmo > 0;
    //}
    //public bool CanInspect()
    //{
    //    return !_isInspected && !_isFiring && !_isReloading;
    //}
    //public virtual void SetInspect(bool inspect)
    //{
    //    _isInspected = inspect;
    //}
    //public virtual void StartReload()
    //{
    //    FireStop();
    //    SetInspect(false);
    //    _isReloading = true;
    //}
    //public virtual void Reload()
    //{
    //    int val = Mathf.Min(_identity.BackupAmmo, _identity.Data.Ammo - _identity.CurrentAmmo);
    //    _identity.CurrentAmmo += val;
    //    _identity.BackupAmmo -= val;
    //    //UI_GameHUD.SetAmmo(_identity.CurrentAmmo);
    //    //UI_GameHUD.SetBackupAmmo(_identity.BackupAmmo);
    //}
    //public virtual void EndReload()
    //{
    //    _isReloading = false;
    //}
    //public virtual void SetVisible(bool visible)
    //{
    //    foreach (var item in GetComponentsInChildren<Renderer>())
    //    {
    //        item.gameObject.layer = visible ? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Disable Rendering");
    //    }
    //}
}
