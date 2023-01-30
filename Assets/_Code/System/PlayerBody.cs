using System;
using JetBrains.Annotations;
using Mirror;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public class PlayerBody : NetworkBehaviour
{



    [HideInInspector] [SyncVar]public uint ID;
    [HideInInspector] [SyncVar] public uint kills;
    [HideInInspector] [SyncVar] public uint deaths;
    [HideInInspector] [SyncVar] public uint assists;
    [HideInInspector] [SyncVar] public int Health = 100;

    public SC_FPSController Controller;

    //public PlayerMind Mind;

    public WeaponData pistolWep;
    public WeaponData smgWep;
    public WeaponData sniperWep;

    public Tuple<WeaponData, bool, int> pistol;
    public Tuple<WeaponData, bool, int> smg;
    public Tuple<WeaponData, bool, int> sniper;

    public WeaponData equippedWep;
    private int currWepIndex = 0;


    [HideInInspector]public PlayerBody lastAttacker;
    [HideInInspector] public PlayerBody penultimateAttacker;


    public bool beat;


    void Start()
    {
        Controller = GetComponent<SC_FPSController>();
        
        if (LocalGame.Instance.localPlayer != null)
        {
            throw new Exception("LocalGame localPlayer was not null.");
        }

        if (isLocalPlayer)
        {
            LocalGame.Instance.localPlayer = this.gameObject;
          Debug.Log("just set up the local player.");

            
        }
        GameState.Instance.lastconnectedplayerID++;
        ID = GameState.Instance.lastconnectedplayerID;
    }


    public void DieAcknowledgement()
    {

       
        


        
    }

    
    public void DieMessage()
    {

        deaths++;
        NetworkClient.Send(new DeathMessage { who = ID });


        
    }
    public void HitAcknowledgement()
    {

        deaths++;
        NetworkClient.Send(new DeathMessage { who = ID });



    }

    public void HitMessage()
    {

        deaths++;
        NetworkClient.Send(new DeathMessage { who = ID });



    }

    public void Respawn()
    {

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

    

    public void EquipNextWep()
    {
        currWepIndex = Mathf.Clamp(currWepIndex + 1, 0, 3);
    }

    public void EquipPreviousWep()
    {
        currWepIndex = Mathf.Clamp(currWepIndex - 1, 0, 3);
    }

    private void ChangeWeaponBasedOnIndex(int i)
    {
        switch (currWepIndex)
        {
            case 1:
                equippedWep = pistol.Item1;
                break;

            case 2:
                equippedWep = smg.Item1;
                break;

            case 3:
                equippedWep = sniper.Item1;
                break;
        }

        ChangeWeaponVisibility();
    }

    private void ChangeWeaponVisibility()
    {

    }


    public void DoBeat()
    {

    }

}
