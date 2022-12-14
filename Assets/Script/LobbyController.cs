using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Mirror.Examples.Pong;

public class LobbyController : MonoBehaviour
{
    //public static LobbyController instance;
    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //}


    //private CSteamID _lobbyId;
    //private bool _isOwner;
    //private Dictionary<CSteamID, UI_Lobby_PlayerItem> _players = new Dictionary<CSteamID, UI_Lobby_PlayerItem>();
    //private bool _isReady = false;

    //private GameObject _playerItem;

    private void Start()
    {
        if (null == SteamLobby.Instance)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
        //_playerItem = Resources.Load<GameObject>("UI/Lobby/LobbyPlayerItem");

        //_lobbyId = SteamLobby.Instance.CurrentLobbyId;
        //_isOwner = SteamMatchmaking.GetLobbyOwner(_lobbyId) == SteamUser.GetSteamID();
        //_isReady = _isOwner;
        //if (_isOwner)
        //{
        //    SteamMatchmaking.SetLobbyData(_lobbyId, SteamLobby.keyGameStarted, "0");
        //    SteamMatchmaking.SetLobbyJoinable(_lobbyId, true);
        //}

        //SteamMatchmaking.SetLobbyMemberData(_lobbyId, SteamLobby.keyReady, _isReady ? "1" : "0");



        //InitPlayerList();


        ////// UpdateButton();
        //SteamLobby.Instance.onLobbyChatUpdate += OnLobbyChatUpdate;
        //SteamLobby.Instance.onLobbyDataUpdate += OnLobbyDataUpdate;

    }

    //private void OnDisable()
    //{
    //    Debug.Log("on lobby controller disable");
    //    if (SteamLobby.Instance)
    //    {
    //        SteamLobby.Instance.onLobbyChatUpdate -= OnLobbyChatUpdate;
    //        SteamLobby.Instance.onLobbyDataUpdate -= OnLobbyDataUpdate;
    //    }
    //}

    //private void OnLobbyDataUpdate(LobbyDataUpdate_t callback)
    //{
    //    Debug.Log("On lobby data update");


    //        CSteamID playerId = new CSteamID(callback.m_ulSteamIDMember);
    //        _players[playerId].Refresh();

    //}
    //private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    //{
    //    Debug.Log("On lobby chat update.");
    //    switch ((EChatMemberStateChange)callback.m_rgfChatMemberStateChange)
    //    {
    //        case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
    //            AddPlayerListItem(new CSteamID(callback.m_ulSteamIDUserChanged));
    //            break;
    //        case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
    //            RemovePlayerListItem(new CSteamID(callback.m_ulSteamIDUserChanged));
    //            break;
    //        case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
    //            RemovePlayerListItem(new CSteamID(callback.m_ulSteamIDUserChanged));
    //            break;
    //        case EChatMemberStateChange.k_EChatMemberStateChangeKicked:
    //            RemovePlayerListItem(new CSteamID(callback.m_ulSteamIDUserChanged));
    //            break;
    //        case EChatMemberStateChange.k_EChatMemberStateChangeBanned:
    //            RemovePlayerListItem(new CSteamID(callback.m_ulSteamIDUserChanged));
    //            break;
    //        default:
    //            break;
    //    }
    //}



    //private void InitPlayerList()
    //{
    //    for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(_lobbyId); i++)
    //    {
    //        AddPlayerListItem(SteamMatchmaking.GetLobbyMemberByIndex(_lobbyId, i));
    //    }
    //}

    //private void AddPlayerListItem(CSteamID playerId)
    //{
    //    UI_Lobby_PlayerItem item = Instantiate(_playerItem, _pnlPlayerList).GetComponent<UI_Lobby_PlayerItem>();
    //    _players.Add(playerId, item);
    //    item.Initialise(playerId);
    //}
    //private void RemovePlayerListItem(CSteamID playerId)
    //{
    //    if (_players.ContainsKey(playerId))
    //    {
    //        if (null != _players[playerId].gameObject)
    //        {
    //            Destroy(_players[playerId].gameObject);
    //        }
    //        _players.Remove(playerId);            
    //    }        
    //}

    private bool CanStartGame()
    {
        // if (_players.Count < 2) return false;
        foreach (var item in _players)
        {
            string ready = SteamMatchmaking.GetLobbyMemberData(
                _lobbyId,
                item.Key,
                SteamLobby.keyReady
                );
            if (ready != "1")
            {
                MasterUIManager.AddPopupHint("There are players unready...");
                return false;
            }
        }
        
        return true;
    }



<<<<<<< Updated upstream
        if (_isOwner)
        {
            if (CanStartGame())
            {
                Debug.Log("Can start game!");
                SteamMatchmaking.SetLobbyJoinable(_lobbyId, false);
                SteamMatchmaking.SetLobbyData(_lobbyId, SteamLobby.keyGameStarted, "1");

                // SteamLobby.SceneToLoad = "MainMap";
                MyNetworkManager.singleton.StartGame();
            }
        }
        else
        {
            _isReady = !_isReady;
            SteamMatchmaking.SetLobbyMemberData(
                _lobbyId,
                SteamLobby.keyReady,
                _isReady ? "1" : "0");
            _btnStartReady.GetComponentInChildren<TextMeshProUGUI>().SetText(_isReady ? "Unready" : "Ready");
        }
    }
=======
    //public void StartOrReady()
    //{
    //    if (SteamMatchmaking.GetLobbyData(_lobbyId, SteamLobby.keyGameStarted) != "0")
    //    {
    //        MasterUIManager.AddPopupHint("The game has already begun.");
    //        return;
    //    }

    //    if (_isOwner)
    //    {
    //        if (CanStartGame())
    //        {
    //            Debug.Log("Can start game!");
    //            SteamMatchmaking.SetLobbyJoinable(_lobbyId, true);
    //            SteamMatchmaking.SetLobbyData(_lobbyId, SteamLobby.keyGameStarted, "1");


    //            MyNetworkManager.singleton.StartGame();
    //        }
    //    }
    //    else
    //    {
    //        _isReady = !_isReady;
    //        SteamMatchmaking.SetLobbyMemberData(
    //            _lobbyId,
    //            SteamLobby.keyReady,
    //            _isReady ? "1" : "0");

    //    }
    //}
>>>>>>> Stashed changes
}
