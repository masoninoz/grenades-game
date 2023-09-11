/*
===================================================
GameplayManager.cs is a file containing gamelogic for the main gameplay loop
Author: Mason B
===================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class GameplayManager : NetworkBehaviour
{
    public CustomNetworkManager manager;

    public GameLogicController player;

    public GameObject playButton;

    void Awake(){
        this.gameObject.SetActive(true);
    }

    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void setPlayer(GameLogicController Player){
        player = Player;
    }

    public void startGame(string SceneName) {
        player.startGame(SceneName);
    }

    [ClientRpc]
    public void RPCEnableGameobject(string gmName){
        Debug.Log("Now something is wrong with the gameobject");
        //A stupid solution to an issue caused by passing gameobjects to the server and back
        if(gmName == "playButton"){
            manager.gameObject.GetComponent<SteamLobby>().playButton.SetActive(true);
        }
    }
    }
