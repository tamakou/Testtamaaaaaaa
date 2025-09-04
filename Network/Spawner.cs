using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkObject playerPrefab;

    // Mapping between Token ID and Re-created Worker
    public Dictionary<int, NetworkObject> mapTokenIDWithNetworkPlayer;
    // Mappting between a Player Ref and Player Characters
    Dictionary<PlayerRef, NetworkObject> _playerCharacterMap;
    //Other compoents
    WorkSessionListUI sessionListUIHandler;
    // Pending Players to Join
    List<int> _pendingTokens;
    void Awake()
    {
        //Create a new Dictionary
        mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkObject>();
        _playerCharacterMap = new Dictionary<PlayerRef, NetworkObject>();
        sessionListUIHandler = FindObjectOfType<WorkSessionListUI>(true);
        _pendingTokens = new List<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    int GetPlayerToken(NetworkRunner runner, PlayerRef player)
    {
        if (runner.LocalPlayer == player)
        {
            // Just use the local Player Connection Token
            return ConnectionTokenUtils.HashToken(GameManager.instance.GetConnectionToken());
        }
        else
        {
            // Get the Connection Token stored when the Client connects to this Host
            var token = runner.GetPlayerConnectionToken(player);

            if (token != null)
                return ConnectionTokenUtils.HashToken(token);

            Debug.LogError($"GetPlayerToken returned invalid token");

            return 0; // invalid
        }
    }

    public void SetConnectionTokenMapping(int token, NetworkObject networkPlayer)
    {
        mapTokenIDWithNetworkPlayer[token]= networkPlayer;
    }
    public void RemoveAllConnectionTokenMapping()
    {
        mapTokenIDWithNetworkPlayer.Clear();
    }
    public void RemoveConnectionTokenMapping(int token)
    {
        Log.Debug($"Recovered Token: {token}");

        if (mapTokenIDWithNetworkPlayer.ContainsKey(token))
        {
            mapTokenIDWithNetworkPlayer.Remove(token);
        }
       
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
       
        if (runner.IsServer)
        {
            //Get the token for the player
            int playerToken = GetPlayerToken(runner, player);

            Debug.Log($"OnPlayerJoined we are server. Connection token {playerToken}");

            //Check if the token is already recorded by the server. 
            if (mapTokenIDWithNetworkPlayer.TryGetValue(playerToken, out NetworkObject networkPlayer))
            {  
                Debug.Log($"Found old connection token for token {playerToken}. Assigning controlls to that player");
                //if(networkPlayer==null)
                //{
                //    NetworkObject spawnedNetworkPlayer = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
                //    spawnedNetworkPlayer.GetComponent<NetworkWorker>().token = playerToken;
                //}
                print(networkPlayer.HasInputAuthority + "/" + networkPlayer.HasStateAuthority);
                if (networkPlayer.HasStateAuthority)
                {
                    networkPlayer.AssignInputAuthority(player);
                    networkPlayer.GetComponent<NetworkWorker>().Spawned();
                }
                else
                {
                    Debug.Log($"Spawning new player for connection token {playerToken}");
                    NetworkObject spawnedNetworkPlayer = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

                    //Store the token for the player
                    spawnedNetworkPlayer.GetComponent<NetworkWorker>().token = playerToken;

                    spawnedNetworkPlayer.AssignInputAuthority(player);
                    spawnedNetworkPlayer.GetComponent<NetworkWorker>().Spawned();
                    //Store the mapping between playerToken and the spawned network player
                    mapTokenIDWithNetworkPlayer[playerToken] = spawnedNetworkPlayer;
                    runner.SetPlayerObject(player, spawnedNetworkPlayer);
                }
                print(networkPlayer.HasInputAuthority + "/" + networkPlayer.HasStateAuthority);
                
                _playerCharacterMap[player] = networkPlayer;
               
                lock (_pendingTokens)
                {
                    if (_pendingTokens.Contains(playerToken))
                    {
                        _pendingTokens.Remove(playerToken);
                    }
                }
            }
            else
            {
                Debug.Log($"Spawning new player for connection token {playerToken}");
                NetworkObject spawnedNetworkPlayer = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

                //Store the token for the player
                spawnedNetworkPlayer.GetComponent<NetworkWorker>().token = playerToken;

                //Store the mapping between playerToken and the spawned network player
                mapTokenIDWithNetworkPlayer[playerToken] = spawnedNetworkPlayer;
                runner.SetPlayerObject(player, spawnedNetworkPlayer);
            }
        }
        else Debug.Log("OnPlayerJoined");
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }


    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnectedToServer"); }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {

    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log("OnShutdown"); mapTokenIDWithNetworkPlayer.Clear();
    }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("OnDisconnectedFromServer"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { Debug.Log("OnConnectRequest"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("OnConnectFailed"); }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        //Only update the list of sessions when the session list UI handler is active
        if (sessionListUIHandler == null)
            return;

        if (sessionList.Count == 0)
        {
            Debug.Log("Joined lobby no sessions found");

            sessionListUIHandler.OnNoSessionsFound();
        }
        else
        {
            sessionListUIHandler.ClearList();
            int idObject = 0;
            foreach (SessionInfo sessionInfo in sessionList)
            {
                if (sessionInfo.Properties.TryGetValue("idObject", out var id))
                {
                   idObject = (int)id.PropertyValue;
                }
                sessionListUIHandler.AddToList(sessionInfo, idObject);

                Debug.Log($"Found session {sessionInfo.Name} playerCount {sessionInfo.PlayerCount}");
            }
        }

    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) 
    {
        Debug.Log("OnHostMigration");

        // Shut down the current runner
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        //Find the network runner handler and start the host migration
        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);

    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnHostMigrationCleanUp()
    {
        Debug.Log("Spawner OnHostMigrationCleanUp started");

        foreach (KeyValuePair<int, NetworkObject> entry in mapTokenIDWithNetworkPlayer)
        {
            NetworkObject networkObjectInDictionary = entry.Value;
            if (networkObjectInDictionary == null) return;
            if (networkObjectInDictionary.InputAuthority.IsNone)
            {
                Debug.Log($"{Time.time} Found player that has not reconnected. Despawning {entry.Value.GetComponent<NetworkWorker>().nickName}");

                networkObjectInDictionary.Runner.Despawn(networkObjectInDictionary);
            }
        }
    
        Debug.Log("Spawner OnHostMigrationCleanUp completed");
    }

}
