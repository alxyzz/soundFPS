using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponOverworld : NetworkBehaviour
{
    enum GunType
    {
        Pistol,
        SMG,
        Sniper
    }

    private GunType _type;

    private void Awake()
    {
        //_pfbInfoWidget = Resources.Load<GameObject>("UI/Game/InfoWidget");
    }

    public void BeInteracted(PlayerBody pBody)
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerBody b = other.GetComponentInChildren<PlayerBody>();
        if (b != null)
        {
            int i = 1;
            switch (_type)
            {
                case GunType.Pistol:
                    i = 1;
                    break;

                case GunType.SMG:
                    i = 2;
                    break;

                case GunType.Sniper:
                    i = 3;
                    break;
            }
            b.PickUpWeapon(i);
            CmdDestroy();
        }
    }

    //public void EndBeingSeen()
    //{
    //    // Debug.Log(name + " was unseen.");
    //    UI_GameHUD.ClearInteractionHint();
    //    Destroy(_widget);
    //}

    //public void StartBeingSeen()
    //{
    //    Debug.Log(name + " was seen.");
    //    UI_GameHUD.AddInteractionHint("E: Pick up");
    //    _widget = Instantiate(_pfbInfoWidget);
    //    _widget.GetComponent<WID_Info>().Initialise(transform, _data.WeaponName, CurrentAmmo, BackupAmmo, _data.RangeType);
    //}

    [Command(requiresAuthority = false)]
    public void CmdDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }
}
