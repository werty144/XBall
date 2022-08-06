using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


using UnityEngine;
using log4net;


using static SteamLobby;
using static MainMenu;


public class LobbyManager : MonoBehaviour
{
    private static bool inLobby = false;
    private static bool ready = false;
    private static LobbyViewController lobbyViewController;
    public static readonly ILog Log = LogManager.GetLogger(typeof(LobbyManager));


    void Start()
    {
        lobbyViewController = GameObject.Find("Lobby").GetComponent<LobbyViewController>();
    }

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
                Log.Error("Can't create lobby");
                return;
            }
        }
        SteamLobby.lobbyCreated = false;
        
        setMetaData(metaData);
    }

    public static void enterLobby()
    {
        inLobby = true;
        setReadyFalse();
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
        if (inLobby)
        {
            SteamLobby.inviteToLobby();
        } else
        {
            Log.Info("You are not in lobby");
        }
    }

    public static void inviteToLobby(ulong userID)
    {
        if (inLobby)
        {
            SteamLobby.inviteToLobby(userID);
        } else
        {
            Log.Info("You are not in lobby");
        }
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
        if (lobbyViewController != null) 
        {
            lobbyViewController.OnLobbyUpdate(lobbyData);
        }

        if (SteamLobby.allInAndReady())
        {
            GameStarter.lobbyReady(lobbyData);  
        }
    }

    public static void leaveLobby()
    {
        if (inLobby)
        {
            inLobby = false;
            SteamLobby.leaveLobby();
        }
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
        if (inLobby)
        {
            ready = false;
            SteamLobby.setLobbyReady(ready);
        }
    }

    public static bool IAmLobbyOwner()
    {
        return inLobby && (SteamLobby.IAmLobbyOwner());
    }

    public static ulong getLobbyOwner()
    {
        return SteamLobby.getLobbyOwner();
    }

    public static bool isMember(ulong ID)
    {
        return inLobby && (SteamLobby.isMember(ID));
    }

    void OnApplicationQuit()
    {
        leaveLobby();
    }
}


public class LobbyMemberData
{
    public ulong ID;
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
    public ulong ID;
    public LobbyMetaData metaData;
    public List<LobbyMemberData> membersData;
}
