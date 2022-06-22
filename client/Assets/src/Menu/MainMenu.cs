using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;


using UnityEngine.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;


using static GameManager;
using static ServerManager;
using static LobbyManager;


public class MainMenu : MonoBehaviour
{
    public static void prepareGame(GameState state, Side side)
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

    public static async void lobbyReady(LobbyData lobbyData)
    {
        ServerManager.startServer();

        var timePassed = 0;
        while (!ServerManager.serverReady)
        {
            await System.Threading.Tasks.Task.Delay(25);
            timePassed += 25;   

            if (timePassed > 5000)
            {
                log("Can't start server");
                return;
            }
        }
        log("Server started");

        RequestCreator.lobbyReady(lobbyData);
    }

    public static void test()
    {
        // LobbyData lobbyData = new LobbyData();

        // lobbyData.ID = 0;

        // LobbyMemberData memberData = new LobbyMemberData();
        // memberData.ID = 0;
        // memberData.isReady = true;
        // memberData.name = "Antoha";
        // lobbyData.membersData = new List<LobbyMemberData> {memberData};

        // LobbyMetaData metaData = new LobbyMetaData();
        // metaData.playersNumber = 3;
        // metaData.speed = "FAST";
        // lobbyData.metaData = metaData;

        // ServerManager.lobbyReady(lobbyData);
    }

    public static void log(string log)
    {
        var logger = GameObject.Find("LoggerContent");
        if (logger != null)
        {   
            logger.GetComponent<LoggerViewController>().addLog(log);
        }
    }

    public void OnExit()
    {
        log("Exiting");
        Application.Quit();
    }
}