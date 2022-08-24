using System.Threading.Tasks;


using UnityEngine;
using log4net;


public class GameStarter : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(GameStarter));
    private static bool preparingGame = false;
    public static void lobbyReady(LobbyData lobbyData)
    {
        ManagerOfScenes.activateOverlay();
        LobbyManager.setReadyFalse();
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

    public static async void prepareGame(GameState state, string side)
    {
        if (preparingGame) return;

        preparingGame = true;
        GameManager.prepareGame(state, side);
        ManagerOfScenes.loadScene("GameScene");
        while (!ManagerOfScenes.sceneReady())
        {
            await Task.Delay(25);
        }
        RequestCreator.createGameReadyRequest();
        
        await Task.Delay(10000);
        cancelGame();
    }

    public static void startGame()
    {
        if (preparingGame) 
        {
            // await ManagerOfScenes.activateScene();
            ManagerOfScenes.deactivateOverlay();
        }
        preparingGame = false;
    }

    public static async void cancelGame()
    {
        if (preparingGame)
        {
            ManagerOfScenes.loadScene("MenuScene");
            while (!ManagerOfScenes.sceneReady())
            {
                await Task.Delay(25);
            }
            ManagerOfScenes.deactivateOverlay();
        }
        preparingGame = false;
    }
}
