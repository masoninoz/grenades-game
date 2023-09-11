using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    [SerializeField] private GameplayManager gameplayManagerPrefab;
    public List<GameObject> Players {get;} = new List<GameObject>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {

        PlayerObjectController PlayerInstance = Instantiate(GamePlayerPrefab);
        PlayerInstance.ConnectionID = conn.connectionId;

        // PlayerInstance.PlayerSteamID = Steamworks.GetLobbyMemberByIndex();

        NetworkServer.AddPlayerForConnection(conn, PlayerInstance.gameObject);

        Players.Add(PlayerInstance.gameObject);

        clearCameras();


    }

    

    public void clearCameras(){
        
        foreach (GameObject player in Players){
            if(player.name == "LocalGamePlayer"){
                player.transform.Find("LocalCamera").gameObject.GetComponent<Camera>().enabled = true;
            } else {
                player.transform.Find("Main Camera").gameObject.GetComponent<Camera>().enabled = false;
            }
        }
    }


    public void startGame(string SceneName) {
        ServerChangeScene(SceneName);
        clearCameras();
    }


    
}
