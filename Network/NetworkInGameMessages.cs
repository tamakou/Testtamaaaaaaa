using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class NetworkInGameMessages : NetworkBehaviour
{
    InGameMessagesUIHander inGameMessagesUIHander;

    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void SendShareRPCMessage()
    {
        if (inGameMessagesUIHander == null)
            inGameMessagesUIHander = MainUIHandler._Instance.inGameMessagesUIHander;
        if (inGameMessagesUIHander != null)
            inGameMessagesUIHander.OnShareMessageReceived();
    }

    public void SendInGameRPCMessage(string userNickName, string message)
    {
        RPC_InGameMessage($"{userNickName}{message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        Debug.Log($"[RPC] InGameMessage {message}");

        if (inGameMessagesUIHander == null)
            inGameMessagesUIHander =MainUIHandler._Instance.inGameMessagesUIHander;

        if (inGameMessagesUIHander != null)
            inGameMessagesUIHander.OnGameMessageReceived(message);
    }
}
