using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (null == SteamLobby.Instance)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
        StartCoroutine(artificialLoading());
    }


    IEnumerator artificialLoading()
    {
        yield return new WaitForSecondsRealtime(4f);
        SceneManager.LoadScene("MainMap");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
