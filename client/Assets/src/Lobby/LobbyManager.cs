using System.Collections.Generic;
using System.Threading.Tasks;


using UnityEngine;
using log4net;


public class LobbyManager : MonoBehaviour
{
    private static bool inLobby = false;
    private static bool iAmOwner = false;
    private static bool ready = false;
    private static ulong? inviteDemand = null;
    private static LobbyViewController lobbyViewController;
    public static readonly ILog Log = LogManager.GetLogger(typeof(LobbyManager));


    void Start()
    {
        lobbyViewController = this.gameObject.GetComponent<LobbyViewController>();
        if (!inLobby)
        {
            lobbyViewController.disable();
        }
    }

    public static void createLobby(int membersN)
    {
        if (inLobby)
        {
            return;
        }
        
        SteamLobby.createLobby(membersN);
    }

    public static void OnLobbyCreated()
    {
        iAmOwner = true;
    }

    public static void enterLobby()
    {
        inLobby = true;

        if (inviteDemand != null)
        {
            inviteToLobby((ulong)inviteDemand);
            inviteDemand = null;
        }

        setReadyFalse();

        if (IAmLobbyOwner())
        {
            LobbyMetaData metaData = new LobbyMetaData("FAST", 3);
            setMetaData(metaData);
            prepareToBeHost();
        } else
        {
            prepareToBeClient();
        }
    }

    public static void prepareToBeHost()
    {
        P2PReceiver.isHost = true;
        RequestCreator.isHost = true;
        ServerMessageProcessor.isHost = true;
    }

    public static void prepareToBeClient()
    {
        P2PReceiver.isHost = false;
        RequestCreator.isHost = false;
        ServerMessageProcessor.isHost = false;

        SteamP2P.sendInitialMessage(getLobbyOwner());
    }

    public static void setMetaData(LobbyMetaData metaData)
    {
        if (inLobby)
        {
            SteamLobby.setMetaData(metaData);
        }
    }

    public static void inviteToLobby(ulong userID)
    {
        if (inLobby)
        {
            SteamLobby.inviteToLobby(userID);
        } else
        {
            inviteDemand = userID;
            createLobby(2);
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
            lobbyViewController.enable();
        }

        if (SteamLobby.allInAndReady())
        {
            GameStarter.lobbyReady(lobbyData);  
        }
    }

    public static void leaveLobby()
    {
        inLobby = false;
        iAmOwner = false;
        SteamLobby.leaveLobby();
        lobbyViewController.disable();
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
        return inLobby && iAmOwner;
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

    public LobbyMetaData(string speed, int playersNumber)
    {
        this.speed = speed;
        this.playersNumber = playersNumber;
    }
}

public class LobbyData
{
    public ulong ID;
    public LobbyMetaData metaData;
    public List<LobbyMemberData> membersData;
}
