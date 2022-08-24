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
        RequestCreator.lobbyReady(lobbyData);
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
