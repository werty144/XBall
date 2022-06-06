using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


using static SteamLobby;
using static MainMenu;


public class LobbyManager : MonoBehaviour
{
    static int playersNumber = 3;
    static string speed = "FAST";

    public void processPlayerNumberSelection(int newOption)
    {
        if (newOption == 0)
        {
            playersNumber = 3;
        } else 
        {
            playersNumber = newOption;
        }
        setInfo();
    }

    public void processSpeedSelection(int newOption)
    {
        switch (newOption)
        {
            case 0:
                speed = "FAST";
                break;
            case 1:
                speed = "NORMAL";
                break;
            case 2:
                speed = "SLOW";
                break;
            default:
                break;
        }
        setInfo();
    }

    public async void createLobby()
    {
        SteamLobby.createLobby();

        int timeSpent = 0;
        while (!SteamLobby.lobbyReady)
        {
            await Task.Delay(25);
            timeSpent = timeSpent + 25;

            if (timeSpent > 5000)
            {
                Debug.Log("Can't create lobby");
                return;
            }
        }
        
        setInfo();
        SteamLobby.setLobbyReady(false);
    }

    private void setInfo()
    {
        SteamLobby.setInfo(speed, playersNumber);
    }

    public static void inviteToLobby()
    {
        SteamLobby.inviteToLobby();
    }

    public static void OnLobbyUpdate(LobbyData lobbyData)
    {
        MainMenu.OnLobbyUpdate(lobbyData);
        if (SteamLobby.allInAndReady())
        {
            MainMenu.lobbyReady(SteamLobby.getID(), speed, playersNumber);
        }
    }

    public static void leaveLobby()
    {
        SteamLobby.leaveLobby();
    }

    public static void setLobbyReady(bool ready)
    {
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
