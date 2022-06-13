using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


using static SteamLobby;
using static MainMenu;


public class LobbyManager : MonoBehaviour
{
    private static bool inLobby = false;
    private static bool ready = false;


    public static async void createLobby(LobbyMetaData metaData)
    {
        SteamLobby.createLobby();

        int timeSpent = 0;
        while (!SteamLobby.lobbyCreated)
        {
            await Task.Delay(25);
            timeSpent = timeSpent + 25;

            if (timeSpent > 5000)
            {
                Debug.Log("Can't create lobby");
                return;
            }
        }
        SteamLobby.lobbyCreated = false;
        
        setMetaData(metaData);
    }

    public static void enterLobby()
    {
        inLobby = true;
        SteamLobby.setLobbyReady(false);
        ready = false;
    }

    public static void setMetaData(LobbyMetaData metaData)
    {
        if (inLobby)
        {
            SteamLobby.setMetaData(metaData);
        }
    }

    public static void inviteToLobby()
    {
        SteamLobby.inviteToLobby();
    }

    public static LobbyData getLobbyData()
    {
        if (inLobby)
        {
            return SteamLobby.getLobbyData();
        }

        return null;
    }

    public static void OnLobbyUpdate(LobbyData lobbyData)
    {
        var lobbyView = GameObject.Find("Lobby");
        if (lobbyView != null)
        {
            lobbyView.GetComponent<LobbyViewContoller>().OnLobbyUpdate(lobbyData);
        }

        if (SteamLobby.allInAndReady())
        {
            MainMenu.lobbyReady
            (
                SteamLobby.getID(),
                lobbyData.membersData.Count,
                lobbyData.metaData.speed,
                lobbyData.metaData.playersNumber
            );  
        }
    }

    public static void leaveLobby()
    {
        inLobby = false;
        SteamLobby.leaveLobby();
    }

    public static void lobbyChangeReady()
    {
        if (inLobby) 
        {
            ready = !ready;
            SteamLobby.setLobbyReady(ready);
        }
    }

    public static void setReadyFalse()
    {
        ready = false;
        SteamLobby.setLobbyReady(ready);
    }
}


public class LobbyMemberData
{
    public string name;
    public bool isReady;
}

public class LobbyMetaData
{
    public string speed;
    public int playersNumber;
}

public class LobbyData
{
    public LobbyMetaData metaData;
    public List<LobbyMemberData> membersData;
}
