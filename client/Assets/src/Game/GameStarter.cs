using log4net;


public class GameStarter
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(GameStarter));
    public static void lobbyReady(LobbyData lobbyData)
    {
        P2PReceiver.startReceivingMessages();

        if (LobbyManager.IAmLobbyOwner())
        {
            prepareToBeHost(lobbyData);
        } else
        {
            prepareToBeClient();
        }
    }

    static void prepareToBeHost(LobbyData lobbyData)
    {
        P2PReceiver.isHost = true;
        RequestCreator.isHost = true;
        ServerMessageProcessor.isHost = true;
        RequestCreator.lobbyReady(lobbyData);
    }

    static void prepareToBeClient()
    {
        P2PReceiver.isHost = false;
        RequestCreator.isHost = false;
        ServerMessageProcessor.isHost = false;

        SteamP2P.sendInitialMessage(LobbyManager.getLobbyOwner());
    }
}
