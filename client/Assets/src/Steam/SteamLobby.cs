using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Newtonsoft.Json;


using static LobbyManager;
using static MainMenu;


public class SteamLobby : MonoBehaviour
{
    public static bool lobbyCreated = false;

    private static CSteamID lobbyID;
    private const string trueString = "true";
    private const string falseString = "false";


    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            Callback<LobbyEnter_t>.Create(OnLobbyEnter);
            Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
            Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
            Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        }
    }

    public void OnLobbyCreated(LobbyCreated_t pCallback)
    {
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            lobbyID = new CSteamID(pCallback.m_ulSteamIDLobby);
            SteamMatchmaking.SetLobbyJoinable(lobbyID, true);
            lobbyCreated = true;
        }
    }

    public void OnLobbyEnter(LobbyEnter_t pCallback)
    {
        lobbyID = new CSteamID(pCallback.m_ulSteamIDLobby);
        MainMenu.log(string.Format("Lobby ID: {0}", pCallback.m_ulSteamIDLobby));
    }
    
    public static void createLobby()
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 1);
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
        lobbyID = new CSteamID(pCallback.m_ulSteamIDLobby);
        OnLobbyUpdate();
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
    {
        OnLobbyUpdate();
    }

    private void OnLobbyUpdate()
    {
        LobbyManager.OnLobbyUpdate(getLobbyData());
    }

    public static LobbyData getLobbyData()
    {
        LobbyData lobbyData = new LobbyData();
        lobbyData.metaData = getLobbyMetaData();
        lobbyData.membersData = getLobbyMembersData();
        return lobbyData;
    }

    private static List<LobbyMemberData> getLobbyMembersData()
    {
        List<LobbyMemberData> membersData = new List<LobbyMemberData>();

        for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(lobbyID); i++)
        {
            LobbyMemberData memberData = new LobbyMemberData();

            CSteamID userID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
            
            memberData.name = SteamFriends.GetFriendPersonaName(userID);
            memberData.isReady = SteamMatchmaking.GetLobbyMemberData(lobbyID, userID, "ready") == trueString;

            membersData.Add(memberData);
        }
        
        return membersData;
    }

    private static LobbyMetaData getLobbyMetaData()
    {
        LobbyMetaData metaData = new LobbyMetaData();
        metaData.speed = SteamMatchmaking.GetLobbyData(lobbyID, "speed");
        var playersNumber = SteamMatchmaking.GetLobbyData(lobbyID, "playersNumber");
        if (playersNumber == "")
        {
            metaData.playersNumber = -1;
        } else
        {
            metaData.playersNumber = Int32.Parse(playersNumber);
        }
        
        return metaData;
    }

    public static void leaveLobby()
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.LeaveLobby(lobbyID);
        }
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t pCallback)
    {
        SteamMatchmaking.JoinLobby(pCallback.m_steamIDLobby);
    }

    public static void setMetaData(LobbyMetaData metaData)
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.SetLobbyData(lobbyID, "speed", metaData.speed);
            SteamMatchmaking.SetLobbyData(lobbyID, "playersNumber", metaData.playersNumber.ToString());
        }
    }

    public static void setLobbyReady(bool ready)
    {
        if (SteamManager.Initialized)
        {
            if (ready)
            {
                SteamMatchmaking.SetLobbyMemberData(lobbyID, "ready", trueString);
            } else 
            {
                SteamMatchmaking.SetLobbyMemberData(lobbyID, "ready", falseString);     
            }
        }
    }

    public static bool allInAndReady()
    {
        if (SteamManager.Initialized)
        {
            List<LobbyMemberData> membersData = getLobbyMembersData();
            bool everyBodyReady = true;
            foreach (var member in membersData)
            {
                if (!member.isReady)
                {
                    everyBodyReady = false;
                }
            }

            bool allIn = membersData.Count == SteamMatchmaking.GetLobbyMemberLimit(lobbyID);

            MainMenu.log(string.Format("Starting game: {0}", (everyBodyReady && allIn)));

            return everyBodyReady && allIn;
        }

        return false;
    }

    public static ulong getID()
    {
        return lobbyID.m_SteamID;
    }
}
