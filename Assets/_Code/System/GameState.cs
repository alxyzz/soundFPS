using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;



public class PlayerData
{
    public uint Kills;
    public uint Deaths;
    public uint Assists;


    public string nickname;
    public string Ip;
    public uint Id;
    public uint Ping;
    public float SecondsConnected;
    public bool Dead;


    public PlayerData()
    {
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException("info");

        info.AddValue("Kills", this.Kills);
        info.AddValue("Deaths", this.Deaths);
        info.AddValue("Assists", this.Assists);


        info.AddValue("nickname", this.nickname);
        info.AddValue("Ip", this.Ip);
        info.AddValue("Id", this.Id);
        info.AddValue("Ping", this.Ping);
        info.AddValue("Dead", this.Dead);

        throw new NotImplementedException();
    }
}





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
    public readonly List<PlayerData> players = new();
    public Dictionary<uint, PlayerMind> playerMinds;
    public readonly List<PlayerData> deadPlayers = new();


    public bool inactive = false;
    [SyncVar] public float maxTime = 15; //in minutes 

    [SyncVar] [SerializeField] int maxKills = 30;


    private uint lastconnectedplayerID = 0;

    public void GivePeopleDebugWeapons()
    {
        Debug.Log("GameState.GivePeopleDebugWeapons() start.");
        foreach (PlayerData item in GameState.instance.players)
        {
            Debug.Log("GameState.GivePeopleDebugWeapons() - gave weapon to " + item);
            //item.GiveDebugWeapon();
        }
    }

    
    [Server]
    public void Update()
    {
        if (inactive)
        {
            return;
        }
        if (!isLocalPlayer)
        {
            timeElapsed += Time.deltaTime;
            foreach (PlayerData v in players)
            {
                v.SecondsConnected += Time.deltaTime;
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
    public void AddHostToPlayer(string nick)
    {
        PlayerData host = new PlayerData();
        host.nickname = nick;
        host.Ip = NetworkManager_ArenaFPS.singleton.networkAddress;
        players.Add(host);
        Debug.Log("host nickname is " + host.nickname);
        Debug.Log("host ip is " + host.Ip);
    }

    public void AddPlayer()
    {
        
    }

    public void InitializeBeat()
    {
        if (!isClient)
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
            RpcBeat();

            yield return new WaitForSecondsRealtime(2f);


            RPCUnbeat();
        }
       
    }
    /*
       When running a game as a host with a local client, ClientRpc calls will be invoked on the local client even though it is in the same process as the server. So the behaviours of local and remote clients are the same for ClientRpc calls.
     */
    [ClientRpc]
    private void RpcBeat()
    {
        foreach (KeyValuePair<uint, PlayerMind> item in playerMinds)
        {
            Debug.Log("Started beat for player with ID" + item.Value.ID);
            item.Value.Body.Beat(true);
        }

    }

    private void RPCUnbeat()
    {
        foreach (KeyValuePair<uint, PlayerMind> item in playerMinds)
        {
            Debug.Log("Started beat for player with ID" + item.Value.ID);
            item.Value.Body.Beat(false);
        }

    }


    public override void OnStartServer()
    {
        Debug.Log("Game state OnStartServer.AAAAAAAAAAAAAAAAAAAAAAAA");
        instance = this;
        //SteamLobby.Instance.onLobbyChatUpdate += OnLobbyChatUpdate;
        //GivePeopleDebugWeapons();
        //InitializeBeat();
    }

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

    public void AddPlayer(string playerName, string playerIP)
    {
        int indexIP = players.FindIndex(item => item.Ip == playerIP);
        if (indexIP == -1)
        {
            int indexname = players.FindIndex(item => item.nickname == playerName);
            if (indexname == -1)
            {
                try
                {
                    Debug.Log($"Server Game State add player {playerIP}");
                    lastconnectedplayerID++;
                    PlayerData b = new PlayerData();
                    b.Id = lastconnectedplayerID;
                    b.nickname = playerName;
                    b.Ip = playerIP;
//todo make sure this works because serialization is tricky

                    players.Add(b);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
            else
            {
                Debug.LogWarning("Player tried to join as name " + playerName  + " from IP " + playerIP + " but name was already used."); //TODO HANDLE THIS
            }
            
            
        }
    }
    [Server]
    public void RemovePlayer(uint ID)
    {
        int indexIP = players.FindIndex(item => item.Id == ID);
        if (indexIP != -1)
        {
            
            
                players.RemoveAt(indexIP);
            
            
            
                Debug.LogWarning("Player with ID " + ID + " was removed from the game."); //TODO HANDLE THIS
            


        }


    }

    //public bool TryGetPlayerStateAt(int index, out PlayerBody ps) // usually called on the client
    //{
    //    ps = null;
    //    if (index < 0 || index >= ConnectedPlayerNum) return false;

    //    if (NetworkClient.spawned.TryGetValue(_playerNetIds[index], out NetworkIdentity identity))
    //    {
    //        return identity.TryGetComponent(out ps);
    //    }
    //    return false;
    //}
    //public bool TryGetPlayerStateByName(string nickname, out PlayerBody ps)
    //{
    //    ps = null;
    //    if (PlayerList_NameID.Values.Contains(nickname))
    //    { //we check if the list of spawned players contains the same ID as the player we need, then we use that to grab that player's NetworkIdentity
    //        if (NetworkClient.spawned.TryGetValue(PlayerList_NameID.FirstOrDefault(x => x.Value == nickname).Key, out NetworkIdentity identity))
    //        {
    //            return identity.TryGetComponent(out ps);
    //        }
    //    }
    //    return false;
    //}
    //public bool TryGetPlayerStateByNetId(uint netId, out PlayerBody ps)
    //{
    //    ps = null;
    //    if (PlayerList_NameID.Keys.Contains<uint>(netId))
    //    {
    //        if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
    //        {
    //            return identity.TryGetComponent(out ps);
    //        }
    //    }
    //    return false;
    //}
  
    //Coroutine _cReadyCountdown;
    //private IEnumerator ReadyCountdown()
    //{
    //    yield return new WaitForSecondsRealtime(1.0f);
    //    RpcCountdown("3");
    //    yield return new WaitForSecondsRealtime(1.0f);
    //    RpcCountdown("2");
    //    yield return new WaitForSecondsRealtime(1.0f);
    //    RpcCountdown("1");
    //    yield return new WaitForSecondsRealtime(1.0f);
    //    RpcCountdown("");
    //    _stage = GameStage.PLAYING;
    //    LocalGame.Instance.onServerGameStarted?.Invoke();
    //}
    //[ClientRpc]
    //private void RpcCountdown(string str)
    //{
    //    UI_GameHUD.SetCountdown(str);
    //}


    public void ServersideRegisterKill(uint attacker, uint victim)
    {
        PlayerDied(attacker, victim);
    }

   
    private void PlayerDied(uint attacker, uint victim) // only called on the server
    {
        PlayerMind att;
        PlayerMind vic;

        

        playerMinds.TryGetValue(attacker, out att);
        playerMinds.TryGetValue(victim, out vic);

        PlayerData attM = players.Find(i => i.Id == attacker);
        PlayerData vicM = players.Find(i => i.Id == victim);

       
        try
        {
            int assistant = vic.GetPenultimateAttacker();
            PlayerData assist = players.Find(i => i.Id == assistant);
            assist.Assists += 1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
        
        attM.Deaths += 1;
        vicM.Kills += 1;

        if (_stage != GameStage.OVER)
        {
            if (IfGameOver(out string winnerName, out bool isDraw))
            {
                GameOver(winnerName, isDraw);
            }
        }
       
    }





    [ClientRpc]
    public void RefreshClientStatistics()
    {

    }

    #region End Conditions

    private bool IfGameOver(out string winner, out bool isDraw)
    {
        ;
        isDraw = false;
       
       List<PlayerData> playersScore =  players.FindAll(x => x.Kills >= instance.maxKills);

        //List<PlayerMind> livings =
        switch (playersScore.Count)
        {
            case 1:
                winner = playersScore[0].nickname;
                //DISPLAY WINNER //todo add the popup display here
                return true;
            default:
                if (timeElapsed >= (maxTime * 60))
                {
                    isDraw = true;
                    winner = null;
                    return true;
                }
                isDraw = false;
                winner = null;
                return false;
                break;
        }
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
