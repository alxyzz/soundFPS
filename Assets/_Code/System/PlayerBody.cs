using System;
using Mirror;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public class PlayerBody : NetworkBehaviour
{

    public LocalPlayerController Controller;
    public PlayerMind Mind;

    public Tuple<WeaponData, bool, int> pistol;
    public Tuple<WeaponData, bool, int> smg;
    public Tuple<WeaponData, bool, int> sniper;

    public WeaponData equippedWep;


    public PlayerMind lastAttacker;
    public PlayerMind penultimateAttacker;





    void Start()
    {
        Controller = GetComponent<LocalPlayerController>();
        Mind = GetComponent<PlayerMind>();
    }


    

    public void Die()
    {




        //DieCommand();
    }



    public void DieCommand()
    {

    }

    public void DieRpc()
    {

    }

    public void PickUpWeapon(int which)
    {
        switch (which)
        {
            case 1:
                if (!pistol.Item2)
                {
                    pistol = new Tuple<WeaponData, bool, int>(pistol.Item1, true, pistol.Item3);
                }
                
             
                break;

            case 2:
                if (!smg.Item2)
                {
                    smg = new Tuple<WeaponData, bool, int>(smg.Item1, true, smg.Item3);
                }
                break;

            case 3:
                if (!sniper.Item2)
                {
                    sniper = new Tuple<WeaponData, bool, int>(sniper.Item1, true, sniper.Item3);
                }
                break;
        }


    }
    public void Beat(bool toggle)
    {

    }
    public void Unbeat(bool toggle)
    {

    }

    public void EquipNextWep()
    {
        
    }

    public void EquipPreviousWep()
    {

    }




    [Command]
    private void CommandEquipWeapon()
    {

    }

    public void DoBeat()
    {

    }

}
