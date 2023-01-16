using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _ifLobbyId;
    [SerializeField] GameObject grid;
    private void Start()
    {
        SteamLobby.Instance.onRecoverUI += () => { SetChildrenEnabled(true); };
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
        SetChildrenEnabled(false);
        SteamLobby.Instance.HostLobby();
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
        if (ulong.TryParse(_ifLobbyId.text, out ulong result))
        {
            SteamLobby.Instance.JoinLobby(result);
        }
        else
        {
            MasterUIManager.AddPopupHint("The LobbyID is not a number...");
            SetChildrenEnabled(true);
        }
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
