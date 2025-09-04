using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using System.Linq;

public class NetworkWorker : NetworkBehaviour, IPlayerLeft
{
    public static NetworkWorker Local { get; set; }
    public Transform playerModel;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }

    // Remote Client Token Hash
    [Networked] public int token { get; set; }

    bool isPublicJoinMessageSent = false;

    //Other components
    NetworkInGameMessages networkInGameMessages;

    void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Runner.IsClient&& Object.HasInputAuthority)
        networkInGameMessages.SendShareRPCMessage();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            RPC_SetNickName(GameManager.instance.playerNickName);
            Debug.Log("Spawned local player");
        }
        else
        {
            Debug.Log($"{Time.time} Spawned remote player");
        }
        Runner.SetPlayerObject(Object.InputAuthority, Object);
        //Make it easier to tell which player is which.
        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
            {
                if (playerLeftNetworkObject == Object)
                {
                    Local.GetComponent<NetworkInGameMessages>().SendInGameRPCMessage(playerLeftNetworkObject.GetComponent<NetworkWorker>().nickName.ToString(), " "+ LanguageManager.Instance.GetLocalizedString("txt_leftcurrentnumberofpeopleis")+" " + Runner.ActivePlayers.Count() + "/" + GameManager.instance.maxPlayer);
                }
            }

        }

        Runner.gameObject.GetComponent<Spawner>().RemoveConnectionTokenMapping(token);
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);

    }
    static void OnNickNameChanged(Changed<NetworkWorker> changed)
    {
        Debug.Log($"{Time.time} OnNickNameChanged value {changed.Behaviour.nickName}");

        changed.Behaviour.OnNickNameChanged();
    }

    private void OnNickNameChanged()
    {
        Debug.Log($"Nickname changed for player to {nickName} for player {gameObject.name}");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        Debug.Log($"[RPC] SetNickName {nickName}");
        this.nickName = nickName;

        if (!isPublicJoinMessageSent)
        {
            networkInGameMessages.SendInGameRPCMessage(nickName, " " + LanguageManager.Instance.GetLocalizedString("txt_joincurrentnumberofpeopleis") + " " + Runner.ActivePlayers.Count()+ "/" + GameManager.instance.maxPlayer);

            isPublicJoinMessageSent = true;
        }
    }
}
