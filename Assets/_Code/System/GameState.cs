using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Telepathy;
using Unity.VisualScripting;
using UnityEngine;



//public class PlayerData
//{
//    public uint Kills;
//    public uint Deaths;
//    public uint Assists;


//    public string nickname;
//    public string Ip;
//    public uint Id;
//    public uint Ping;
//    public float SecondsConnected;
//    public bool Dead;


//    public PlayerData()
//    {
//    }

//    public void GetObjectData(SerializationInfo info, StreamingContext context)
//    {
//        if (info == null)
//            throw new ArgumentNullException("info");

//        info.AddValue("Kills", this.Kills);
//        info.AddValue("Deaths", this.Deaths);
//        info.AddValue("Assists", this.Assists);


//        info.AddValue("nickname", this.nickname);
//        info.AddValue("Ip", this.Ip);
//        info.AddValue("Id", this.Id);
//        info.AddValue("Ping", this.Ping);
//        info.AddValue("Dead", this.Dead);

//        throw new NotImplementedException();
//    }
//}





public enum GameStage
{
    //WAITING, // waiting for other players
    //READY, // all players connected. Countdown
    PLAYING, // playing
    OVER // winner
}



/* When rule-related events in the game happen and need to be tracked and shared with all players,
 * that information is stored and synced through the Game State. This information can include:
 *   How long the game has been running (including running time before the local player joined).
 *   When each individual player joined the game, and the current state of that player.
 *   The list of connected players.
 *   The current Game Mode.
 *   Whether or not the game has begun.
 * The Game State is responsible for enabling the clients to monitor the state of the game.
 * Conceptually, the Game State should manage information that is meant to be known to all connected clients.
 */
public class GameState : NetworkBehaviour
{
    public bool beat_toggle;
    [HideInInspector] public float timeElapsed;
    
   
    public readonly List<GameObject> players = new();


    public bool inactive = false;
    [SyncVar] public float maxTime = 15; //in minutes 

    [SyncVar] [SerializeField] int maxKills = 30;


    public uint lastconnectedplayerID = 0;



    
    [Server]
    public void Update()
    {
        if (inactive)
        {
            return;
        }
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= maxTime)
            {
                if (_stage == GameStage.PLAYING)
                {
                    GameOver("no one", true);
                }
            }
    }

    //public void UpdatePlayerScoreboard()
    //{

    //}
    //[ClientRpc]
    //public void RPCUpdatePlayerScoreboard()
    //{

    //}
   // [Server]
    //public void AddHostToPlayer(string nick)
    //{
    //    PlayerData host = new PlayerData();
    //    host.nickname = nick;
    //    host.Ip = NetworkManager_ArenaFPS.singleton.networkAddress;
    //    lastconnectedplayerID++;
    //    host.Id = lastconnectedplayerID;
    //    players.Add(host);
    //    Debug.Log("host nickname is " + host.nickname);
    //    Debug.Log("host ip is " + host.Ip);
    //}

    private bool beatrunning = false;
    [Server]
    public void InitializeBeat()
    {
        if (beatrunning)
        {
            return;
        }

        beatrunning = true;
        Debug.Log(("InitializeBeat @ GameState just ran."));
        if (!isClient) //wont happen anyways but oh well
        {
            Debug.LogError("GameState@ InitializeBeat - invoked the repetition");
            InvokeRepeating("PeriodicBeat", 4, 4);
        }
    }

    int beatNr = 0;
    IEnumerator PeriodicBeat()
    {
        Debug.LogError("Periodic Beat #" + beatNr);
        beatNr++;
        yield return new WaitForSecondsRealtime(2f);
        if (beat_toggle)
        {
            NetworkServer.SendToAll(new Beat { beat = true });

            yield return new WaitForSecondsRealtime(2f);


            NetworkServer.SendToAll(new Beat {beat = false});
        }
       
    }
    

    /*
       When running a game as a host with a local client, ClientRpc calls will be invoked on the local client even though it is in the same process as the server. So the behaviours of local and remote clients are the same for ClientRpc calls.
     */

    public override void OnStartServer()
    {
        Debug.Log("Game state OnStartServer.AAAAAAAAAAAAAAAAAAAAAAAA");
        instance = this;
    }

    [Command]
   
  

    public override void OnStartClient()
    {
        Debug.Log("Game state OnStartClient. Binding callback method.");
        instance = this;



        // _playerNetIds
        //foreach (var item in PlayerList_NameID)
        //{
        //    UI_GameHUD.Instance.AddPlayerToStatistics(item.Key);

        //}
        //PlayerList_NameID.Callback += PlayerListNameIdCallback;
    }

    private void PlayerListNameIdCallback(SyncIDictionary<ulong, uint>.Operation op, ulong key, uint item)
    {
        Debug.Log($"On client game state _playerNetIds changed: {op}.");
        switch (op)
        {
            case SyncIDictionary<ulong, uint>.Operation.OP_ADD:
                //UI_GameHUD.Instance.AddPlayerToStatistics(item);
                UpdateConnectedPlayerNum();
                break;
            case SyncIDictionary<ulong, uint>.Operation.OP_CLEAR:
                break;
            case SyncIDictionary<ulong, uint>.Operation.OP_REMOVE:
                //UI_GameHUD.Instance.RemovePlayerFromStatistics(item);
                break;
            case SyncIDictionary<ulong, uint>.Operation.OP_SET:
                break;
            default:
                break;
        }
    }

    [ClientRpc]
    private void RpcUpdateConnectedPlayerNum()
    {
        UpdateConnectedPlayerNum();
    }
    [Client]
    private void UpdateConnectedPlayerNum()
    {
        //UI_GameHUD.Instance.UpdateConnectedPlayerNum(PlayerList_NameID.Count, SteamMatchmaking.GetNumLobbyMembers(SteamLobby.Instance.CurrentLobbyId));
    }

    private static GameState instance;
    public static GameState Instance => instance;
    [SyncVar(hook = nameof(OnGameStageChanged))] private GameStage _stage = GameStage.PLAYING;
    public GameStage Stage => instance._stage;
    private void OnGameStageChanged(GameStage oldVal, GameStage newVal)
    {
        switch (newVal)
        {
            case GameStage.PLAYING:
                if (LocalGame.Instance.onClientGameStarted != null)
                {
                    LocalGame.Instance.onClientGameStarted.Invoke();
                }
                break;
            case GameStage.OVER:
                LocalGame.Instance.onClientGameEnded?.Invoke();
                break;
            default:
                break;
        }
    }





    [ClientRpc]
    public void RefreshClientStatistics()
    {

    }

    #region End Conditions

    private bool IfGameOver(out string winner, out bool isDraw, string winnerID)
    {
                winner = winnerID;
                isDraw = false;
                //DISPLAY WINNER //todo add the popup display here
                return true;
    }
    //[Server]
    //private void GameReady()
    //{
    //    if (_stage == GameStage.WAITING)
    //    {
    //        _stage = GameStage.READY;
    //        _cReadyCountdown = StartCoroutine(ReadyCountdown());
    //    }
    //}
    [Server]
    private void GameOver(string winnerId, bool isDraw = false)
    {

        _stage = GameStage.OVER;
        LocalGame.Instance.onServerGameEnded?.Invoke();
        if (isDraw)
        {
            Debug.Log("DRAW! Time has elapsed with no winner.");
        }
        else
        {
            Debug.Log($"MATCH END: WINNER IS {winnerId}");
        }
    }
    [ClientRpc]
    private void RpcDecalreWinner(uint netId)
    {
        //if (TryGetPlayerStateByNetId(netId, out PlayerBody ps))
        //{
        //    UI_GameHUD.ShowWinner(ps);
        //}
    }
    #endregion
}
