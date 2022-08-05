using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;


using UnityEngine.SceneManagement;
using UnityEngine;
using log4net;


using static GameManager;
using static ServerManager;
using static LobbyManager;


public class MainMenu : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(MainMenu));
    public static void prepareGame(GameState state, string side)
    {
        OnLeave();
        GameManager.prepareGame(state, side);
        SceneManager.LoadScene("GameScene");
        // RequestCreator.readyRequest();
    }

    private static void OnLeave()
    {
        LobbyManager.setReadyFalse();
    }

    public static void test()
    {
    }

    public void OnExit()
    {
        Log.Info("Exiting");
        Application.Quit();
    }
}