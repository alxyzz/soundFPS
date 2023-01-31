using Mirror;
using System;
using UnityEngine;
public struct ChatMessage : NetworkMessage
{
    public string content;
}
public class Chat : MonoBehaviour
{
    public 
    // Start is called before the first frame update
    void Start()
    {
        NetworkServer.RegisterHandler<ChatMessage>(OnChatMessage);
    }

    private void OnChatMessage(NetworkConnectionToClient arg1, ChatMessage arg2)
    {
        throw new NotImplementedException();
    }

   
  
}
