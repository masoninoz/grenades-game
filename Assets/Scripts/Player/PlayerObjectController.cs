using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIDNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;

    public int health;

    private CustomNetworkManager manager;

    public GameObject playButton;

    public GameplayManager gameplayManager;

    private CustomNetworkManager Manager {
        get {
            if (manager != null) {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    void Start(){
        DontDestroyOnLoad(this.gameObject);
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        manager = gameplayManager.manager;
        if(SteamMatchmaking.GetNumLobbyMembers(new CSteamID(manager.gameObject.GetComponent<SteamLobby>().CurrentLobbyID)) >= 1){
            Debug.Log(manager.gameObject.GetComponent<SteamLobby>().playButton);
            CmdEnableGameobject("playButton");
        }
    }

    public override void OnStartAuthority()
    {
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        gameObject.transform.Find("Main Camera").name = "LocalCamera";
        manager = gameplayManager.manager;
        manager.clearCameras();
        this.gameObject.GetComponent<GameLogicController>().OnStartAuth();
    }

    [Command]
    public void CmdEnableGameobject(string gmName){
        Debug.Log(gameplayManager);
        gameplayManager.RPCEnableGameobject(gmName);
    }


    [Command]
    private void CmdSetPlayerName(string playerName) {
        this.PlayerNameUpdate(this.playerName, playerName);
    }

    public void startGame(string SceneName){
        if(hasAuthority){
            CmdStartGame(SceneName);
        }
    }

    [Command]
    public void CmdStartGame(string SceneName){
        manager.startGame(SceneName);
        foreach (GameObject player in manager.Players){
            player.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            player.GetComponent<Transform>().localPosition = new Vector3(0f, 1.5f, 0f);
        }
    }

    public void PlayerNameUpdate(string oldValue, string newValue) {
        if (isServer)
        {
            this.playerName = newValue;
        }
    }
}
