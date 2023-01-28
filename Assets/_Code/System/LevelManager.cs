using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private List<GameObject> playerSpawns;

    /// <summary>
    /// Server Only
    /// </summary>
    /// <param name="killerName"></param>
    /// <param name="objectName"></param>
    /// <param name="dbIndex"></param>
    /// <param name="type"></param>
    public void BroadcastKillMessage(string killerName, string objectName, int dbIndex, DamageType type)
    {
        RpcKillMessage(killerName, objectName, dbIndex, type);
    }

    [ClientRpc]
    private void RpcKillMessage(string killerName, string objectName, int dbIndex, DamageType type)
    {
      //  UI_GameHUD.AddKillMessage(killerName, objectName, GameManager.GetWeaponData(dbIndex).KillIcon, type);
    }



    public GameObject GetRandomSpawner()
    {
        return playerSpawns[Random.Range(0, playerSpawns.Count - 1)];
    }

}   
