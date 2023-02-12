using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using log4net;

public class LoadingManager : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(LoadingManager));
    GameObject mainMenu;
    GameObject loadingScreen;

    void Awake() {
        mainMenu = GameObject.Find("Canvas/MainMenu");
        mainMenu.SetActive(false);
        loadingScreen = GameObject.Find("Canvas/LoadingScreen");
        Log.Debug(loadingScreen);
    }

    public void ButtonPressed() {
        loadingScreen.SetActive(false);
        mainMenu.SetActive(true);
    }
}
