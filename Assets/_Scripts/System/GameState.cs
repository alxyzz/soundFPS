using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


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

    public float maxTime = 15; //in minutes 

    [HideInInspector] int maxKills = 30;

    public void GivePeopleDebugWeapons()
    {
        Debug.Log("GameState.GivePeopleDebugWeapons() start.");
        foreach (PlayerBody item in GetPlayerStateList())
        {
            Debug.Log("GameState.GivePeopleDebugWeapons() - gave weapon to " + item);
            //item.GiveDebugWeapon();
        }
    }

    

    public void Update()
    {
        if (!isLocalPlayer)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    public void UpdatePlayerScoreboard()
    {

    }
    [ClientRpc]
    public void RPCUpdatePlayerScoreboard()
    {

    }



    private void InitializeBeat()
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
            RPCDoBeat();
        }
       
    }
    /*
       When running a game as a host with a local client, ClientRpc calls will be invoked on the local client even though it is in the same process as the server. So the behaviours of local and remote clients are the same for ClientRpc calls.
     */
    [ClientRpc]
    private void RPCDoBeat()
    {
        foreach (PlayerBody item in GetPlayerStateList())
        {
            Debug.Log("GameState.GivePeopleDebugWeapons() - gave weapon to " + item);
            item.DoBeat();
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

    public readonly SyncDictionary<uint, string> PlayerList_NameID = new SyncDictionary<uint, string>();
    
    public int ConnectedPlayerNum => PlayerList_NameID.Count;
    [Server]
    public void AddPlayer(string playerName, uint playerID)
    {
        if (!PlayerList_NameID.ContainsKey(playerID))
        {
            Debug.Log($"Server Game State add player {playerID}");
            PlayerList_NameID.Add(playerID, playerName);
            
        }
    }
    [Server]
    public void RemovePlayer(uint ID)
    {
        Debug.Log($"Server game state remove player {ID}");
        if (PlayerList_NameID.ContainsKey(ID)) PlayerList_NameID.Remove(ID);
       
      
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
    public bool TryGetPlayerStateByName(string nickname, out PlayerBody ps)
    {
        ps = null;
        if (PlayerList_NameID.Values.Contains(nickname))
        { //we check if the list of spawned players contains the same ID as the player we need, then we use that to grab that player's NetworkIdentity
            if (NetworkClient.spawned.TryGetValue(PlayerList_NameID.FirstOrDefault(x => x.Value == nickname).Key, out NetworkIdentity identity))
            {
                return identity.TryGetComponent(out ps);
            }
        }
        return false;
    }
    public bool TryGetPlayerStateByNetId(uint netId, out PlayerBody ps)
    {
        ps = null;
        if (PlayerList_NameID.Keys.Contains<uint>(netId))
        {
            if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
            {
                return identity.TryGetComponent(out ps);
            }
        }
        return false;
    }
    public List<PlayerBody> GetPlayerStateList() // usually called on the client
    {
        List<PlayerBody> results = new List<PlayerBody>();
        foreach (var item in PlayerList_NameID.Keys)
        {
            if (NetworkClient.spawned.TryGetValue(item, out NetworkIdentity identity))
            {
                if (identity.TryGetComponent(out PlayerBody ps))
                {
                    results.Add(ps);
                }
            }
        }
        return results;
    }

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
    [ClientRpc]
    //private void RpcCountdown(string str)
    //{
    //    UI_GameHUD.SetCountdown(str);
    //}
    [Server]
    public void PlayerDied(uint ID) // only called on the server
    {
        if (IfGameOver(out string winnerName, out bool isDraw))
        {
            GameOver(winnerName, isDraw);
        }
    }

    #region End Conditions

    private bool IfGameOver(out string winner, out bool isDraw)
    {
        ;
        isDraw = false;
       
        List<PlayerBody> livings = GetPlayerStateList().FindAll(x => x.Kills >= instance.maxKills);

        
        switch (livings.Count)
        {
            case 1:
                winner = livings[0].Nickname;
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
    private void GameOver(string winnerNetId, bool isDraw = false)
    {

        _stage = GameStage.OVER;
        LocalGame.Instance.onServerGameEnded?.Invoke();
        if (isDraw)
        {
            Debug.Log("DRAW! Time has elapsed with no winner.");
        }
        else
        {
            Debug.Log($"MATCH END: WINNER IS {winnerNetId}");
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
