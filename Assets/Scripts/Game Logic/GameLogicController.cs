using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class GameLogicController : NetworkBehaviour
{
    public GameplayManager gameplayManager;

    public CustomNetworkManager manager;

    public void Start(){
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        manager = gameplayManager.manager;
        if(hasAuthority){
            gameplayManager.setPlayer(this);
        }
        if(SteamMatchmaking.GetNumLobbyMembers(new CSteamID(manager.gameObject.GetComponent<SteamLobby>().CurrentLobbyID)) >= 1){
            Debug.Log(manager.gameObject.GetComponent<SteamLobby>().playButton);
            CmdEnableGameobject("playButton");
        }
    }

    public void OnStartAuth(){
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        manager = gameplayManager.manager;
    }

    [Command]
    public void CmdEnableGameobject(string gmName){
        gameplayManager.RPCEnableGameobject(gmName);
    }

        public void startGame(string SceneName){
        if(hasAuthority){
            CmdStartGame(SceneName);
            this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            this.gameObject.GetComponent<Transform>().localPosition = new Vector3(0f, 1.5f, 0f);
        }
    }

    [Command]
    public void CmdStartGame(string SceneName){
        RPCStartGame();
        manager.startGame(SceneName);
        manager.clearCameras();
    }

    [ClientRpc]
    public void RPCStartGame(){
        foreach(GameObject player in manager.Players){
            player.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            player.GetComponent<Transform>().localPosition = new Vector3(0f, 1.5f, 0f);
    }
        manager.clearCameras();
    }

    public void takeDamage(int damageAmount){
        if(!isServer) {return;}
        this.gameObject.GetComponent<PlayerObjectController>().health -= damageAmount;
    }

   
}
