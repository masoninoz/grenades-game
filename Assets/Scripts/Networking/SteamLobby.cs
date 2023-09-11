using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;
using UnityEngine.UI;


public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    //Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;

    public GameplayManager gameplayManager;

    public GameObject playButton;
    public GameObject hostButton;
    public Text LobbyNameText;


    private void Start()
    {
        if (!SteamManager.Initialized) { return; }

        manager = GetComponent<CustomNetworkManager>();

        Debug.Log("Hello");
        

       gameplayManager.gameObject.SetActive(true);

        if (Instance == null) { Instance = this; }

        LobbyCreated = Callback<LobbyCreated_t>.Create(onLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(onJoinRequested);
        LobbyEntered = Callback<LobbyEnter_t>.Create(onLobbyEntered);

    }

    public void HostLobby() {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);

    }

    private void onLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK) 
        {
            return;
        }

        Debug.Log("Lobby Created successfully");
        
       

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s very important lobby");
    }

    private void onJoinRequested(GameLobbyJoinRequested_t callback) 
    {
        Debug.Log("Request to join lobby");

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }



    private void onLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        LobbyNameText.gameObject.SetActive(true);
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
        
        //Disable HostGame button
        hostButton.SetActive(false);

        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }
}
