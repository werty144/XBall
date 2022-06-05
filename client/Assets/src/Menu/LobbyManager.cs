using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



using static SteamLobby;

public class LobbyManager : MonoBehaviour
{
    int playersNumber = 3;
    Speed speed = Speed.FAST;

    public void processPlayerNumberSelection(int newOption)
    {
        if (newOption == 0)
        {
            playersNumber = 3;
        } else 
        {
            playersNumber = newOption;
        }
    }

    public void processSpeedSelection(int newOption)
    {
        switch (newOption)
        {
            case 0:
                speed = Speed.FAST;
                break;
            case 1:
                speed = Speed.SLOW;
                break;
            case 2:
                speed = Speed.NORMAL;
                break;
            default:
                break;
        }
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
        
        SteamLobby.setLobbyData("name", "my best lobby");
    }

    public static void autoInvite()
    {
        SteamLobby.autoInvite();
    }
}


public enum Speed
{
    SLOW,
    NORMAL,
    FAST
}


public class LobbyMetaData
{
    public Speed speed;
    public int playersNumber;


    public LobbyMetaData(Speed speed, int playersNumber)
    {
        this.speed = speed;
        this.playersNumber = playersNumber;
    }
}
