
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyNicknameTextbox;
    [SerializeField] GameObject grid;
    private void Start()
    {
        
        EpilepsyMenu.SetActive(true);
    }
    private void SetChildrenEnabled(bool enabled)
    {
        foreach (var item in GetComponentsInChildren<Selectable>())
        {
            item.interactable = enabled;
        }       
    }
    public void OnClickHost()
    {
        if (NicknameCheckInvalid())
        {

            return;
        }
            SetChildrenEnabled(false);
            MyNetworkManager.singleton.StartHost();
    }

    private bool NicknameCheckInvalid()
    {
        if (string.IsNullOrEmpty(_lobbyNicknameTextbox.text) || _lobbyNicknameTextbox.text.Length <= 3 || _lobbyNicknameTextbox.text.Length >= 25)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnClickDebugJoin()
    {
       // SetChildrenEnabled(false);
       // SteamLobby.Instance.HostLobby();
       //// SteamLobby.Instance.JoinLobby(result);
    }

    public void OnClickJoin()
    {
        SetChildrenEnabled(false);
        
            MasterUIManager.AddPopupHint("The LobbyID is not a number...");
        
    }

    [SerializeField] GameObject EpilepsyMenu;
    [SerializeField] AnimateGridMaterialWithSound menuScript;
    public void onClickEpilepticYes()
    {
        menuScript.started = true;
        menuScript.epilepsy = true;
        EpilepsyMenu.SetActive(false);
        grid.GetComponent<Animator>().enabled = false;

    }

    public void onClickEpilepticNo()
    {
        menuScript.started = true;
        menuScript.epilepsy = false;
        EpilepsyMenu.SetActive(false);
    }

}
