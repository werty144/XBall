using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Threading.Tasks;


using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using log4net;
using Steamworks;


using static GameManager;
using static ServerManager;
using static LobbyManager;


public class MainMenu : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(MainMenu));
    private GameObject blockingOverlay;

    void Awake()
    {
        blockingOverlay = GameObject.Find("Canvas/BlockingOverlay");
        blockingOverlay.SetActive(false);
    }

    public static void prepareGame(GameState state, string side)
    {
        OnLeave();
        GameManager.prepareGame(state, side);
        SceneManager.LoadScene("GameScene");
    }

    private static void OnLeave()
    {
        LobbyManager.setReadyFalse();
    }

    public void test()
    {
        Log.Debug("Huy");
    }

    public void activateOverlayBlock(string message="Blocked")
    {
        blockingOverlay.SetActive(true);
        blockingOverlay.transform.Find("Text").GetComponent<Text>().text = message;
    }

    public void deactivateOverlayBlock()
    {
        blockingOverlay.SetActive(false);
    }

    public static async Task doWithOverlay(string message, Func<Task> taskContainer)
    {
        var menuObject = GameObject.Find("Canvas/MainMenu").GetComponent<MainMenu>();
        menuObject.activateOverlayBlock(message);
        await taskContainer();
        menuObject.deactivateOverlayBlock();
    }

    public void OnExit()
    {
        Log.Info("Exiting");
        Application.Quit();
    }
}