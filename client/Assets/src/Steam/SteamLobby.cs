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

    private static CSteamID lobbyID;

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
            Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }
    }

    public void OnLobbyCreated(LobbyCreated_t pCallback)
    {
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            lobbyID = new CSteamID(pCallback.m_ulSteamIDLobby);
            print("Lobby created!");
            SteamMatchmaking.SetLobbyJoinable(lobbyID, true);
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
            SteamMatchmaking.SetLobbyData(lobbyID, key, value);
        }
    }

    public static void inviteToLobby()
    {
        if (SteamManager.Initialized)
        {
            SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
        }
    }

    private void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
    {
        List<string> usersInLobby = new List<string>();

        for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(lobbyID); i++)
        {
            usersInLobby.Add(getLobbyUserName(i));
        }

        LobbyManager.OnLobbyUpdate(usersInLobby);
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
    {
        List<string> usersInLobby = new List<string>();

        for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(lobbyID); i++)
        {
            usersInLobby.Add(getLobbyUserName(i));
        }

        LobbyManager.OnLobbyUpdate(usersInLobby);
    }

    private string getLobbyUserName(int index)
    {
        CSteamID userID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, index);
        return SteamFriends.GetFriendPersonaName(userID);
    }

    public static void leaveLobby()
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.LeaveLobby(lobbyID);
        }
    }
}
