
using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.MultipleAdditiveScenes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyNicknameTextbox;

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
            GameState.Instance.AddHostToPlayer(_lobbyNicknameTextbox.text);
            return;
        }
            SetChildrenEnabled(false);
          NetworkManager_ArenaFPS.singleton.StartHost();
    }
    [Header("Popup Hint")]
    [SerializeField] private RectTransform _popupHintList;
    [SerializeField] private GameObject _pfbPopupHint;




    public static void AddPopupHint(string content)
    {
        //UI_Cmn_PopupHint popup = Instantiate(instance._pfbPopupHint, instance._popupHintList).GetComponent<UI_Cmn_PopupHint>();
        //popup.Appear(content);

        Debug.Log("Popup message simulated with content: " + content);
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
        
            //MasterUIManager.AddPopupHint("The LobbyID is not a number...");
        
    }

    [SerializeField] GameObject EpilepsyMenu;
    [SerializeField] AnimateGridMaterialWithSound menuScript;
    public void onClickEpilepticYes()
    {
        menuScript.started = true;
        menuScript.epilepsy = true;
        EpilepsyMenu.SetActive(false);
        //grid.GetComponent<Animator>().enabled = false;

    }

    public void onClickEpilepticNo()
    {
        menuScript.started = true;
        menuScript.epilepsy = false;
        EpilepsyMenu.SetActive(false);
    }

}
