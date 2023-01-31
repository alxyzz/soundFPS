using System;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerBody : NetworkBehaviour
{

    private TextMeshProUGUI healthDisplay;
    private TextMeshProUGUI killsDisplay;




    [HideInInspector] [SyncVar] public uint ID;
    [HideInInspector] [SyncVar] public uint kills;
    [HideInInspector] [SyncVar] public uint deaths;
    [HideInInspector] [SyncVar] public uint assists;
    [HideInInspector] [SyncVar] public int Health = 100;

    public SC_FPSController Controller;
    public GameObject Camera;
    public AudioSource PlayerAudioSource;

    public FPS_UI_Component HUD;
    //public PlayerMind Mind;

    public WeaponData pistolWep;
    public WeaponData smgWep;
    public WeaponData sniperWep;

    public Tuple<WeaponData, bool> pistol;
    public Tuple<WeaponData, bool> smg;
    public Tuple<WeaponData, bool> sniper;

    public WeaponData equippedWep;
    private int currWepIndex = 0;

    public Animator CurrWepAnim => equippedWep.gameObject.GetComponent<Animator>();

    [HideInInspector] public PlayerBody lastAttacker;
    [HideInInspector] public PlayerBody penultimateAttacker;


    public bool beat;


    void Update()
    {


    }


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
        else
        {
            Camera.SetActive(false);
        }
        pistol = new Tuple<WeaponData, bool> (pistolWep, true); ;
        smg = new Tuple<WeaponData, bool> (smgWep, true); ;
   sniper = new Tuple<WeaponData, bool>(sniperWep, true); ;
   equippedWep = pistol.Item1;
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
        if (Health <= 0)
        {
            deaths++;
            NetworkClient.Send(new DeathMessage { who = ID });
        }



    }

    [ClientRpc]
    public void Respawn()
    {
        

        //drops weapons
        sniper = new Tuple<WeaponData, bool>(sniper.Item1, false);
        smg = new Tuple<WeaponData, bool>(smg.Item1, false);

    }


[Command]
    public void DieCommand()
    {
        Debug.Log("Playerbody @  DieCommand() - player just died.");
        DieRpc();
    }
    [ClientRpc]
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
                    pistol = new Tuple<WeaponData, bool>(pistol.Item1, true);
                }
                break;

            case 2:
                if (!smg.Item2)
                {
                    smg = new Tuple<WeaponData, bool>(smg.Item1, true);
                }
                break;

            case 3:
                if (!sniper.Item2)
                {
                    sniper = new Tuple<WeaponData, bool>(sniper.Item1, true );
                }
                break;
        }


    }

    

    public void EquipNextWep()
    {
        currWepIndex = Mathf.Clamp(currWepIndex + 1, 0, 3);
        ChangeWeaponBasedOnIndex();
    }

    public void EquipPreviousWep()
    {
        currWepIndex = Mathf.Clamp(currWepIndex - 1, 0, 3);
        ChangeWeaponBasedOnIndex();
    }

    public void ChangeWeaponBasedOnIndex( int i = -1)
    {
        if (i!= -1)
        {
            switch (i)
            {
                case 1:
                    if (pistol.Item2)
                        ChangeWep(pistolWep); Debug.Log("switched to pistolWep");
                    break;

                case 2:
                    if (smg.Item2)
                        ChangeWep(smgWep); Debug.Log("switched to smgWep");
                    break;
                case 3:
                    if (sniper.Item2)
                        ChangeWep(sniperWep); Debug.Log("switched to sniperWep");
                    break;
            }

        }
        else switch(currWepIndex)
        {
            case 1:
            if (pistol.Item2)
                ChangeWep(pistolWep);

            break;

            case 2:
            if (smg.Item2)
                ChangeWep(smgWep);
            break;

            case 3:
            if (sniper.Item2)
                ChangeWep(sniperWep);
            break;
        }



    }

    private void ChangeWep(WeaponData d)
    {
        pistolWep.gameObject.SetActive(false);
        smgWep.gameObject.SetActive(false);
        sniperWep.gameObject.SetActive(false);
        equippedWep = d;

        equippedWep.gameObject.SetActive(true);
    }


    public void DoBeat()
    {

    }

}
