using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Newtonsoft.Json;


using static LobbyManager;
using static MainMenu;


public class SteamLobby : MonoBehaviour
{
    public static bool lobbyReady = false;
    public static ulong lobbyID;

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            // Callback<LobbyMatchList_t>.Create(OnLobbyList);
        }
    }

    public void OnLobbyCreated(LobbyCreated_t pCallback)
    {
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            lobbyID = pCallback.m_ulSteamIDLobby;
            print("Lobby created!");
            SteamMatchmaking.SetLobbyJoinable(new CSteamID(lobbyID), true);
            lobbyReady = true;
        }
    }
    
    public static void createLobby()
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
        }
    }

    public static void setLobbyData(string key, string value)
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyID), key, value);
        }
    }

    public static void autoInvite()
    {
        if (SteamManager.Initialized)
        {
            
            // bool result = SteamMatchmaking.InviteUserToLobby(new CSteamID(lobbyID), new CSteamID(MainMenu.myID));
            // GameObject.Find("Test Text").GetComponent<Text>().text = string.Format("Reult: {0}", result);
            SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(lobbyID));
        }
    }

    // public static void listLobbies()
    // {
    //     if (SteamManager.Initialized)
    //     {
    //         print(SteamMatchmaking.GetLobbyData(new CSteamID(lobbyID), "name"));
    //         // SteamMatchmaking.RequestLobbyList();
    //     }
    // }

    // public void OnLobbyList(LobbyMatchList_t pCallback)
    // {
    //     for (int i = 0; i < pCallback.m_nLobbiesMatching; i++)
    //     {
    //         CSteamID lobbyID_ = SteamMatchmaking.GetLobbyByIndex(i);
    //         print(SteamMatchmaking.GetLobbyData(lobbyID_, "name"));
    //     }
    // }
}
