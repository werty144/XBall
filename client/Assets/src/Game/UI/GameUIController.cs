using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    [SerializeField] public GameObject menuButton;
    public void menuButtonRec()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void surrenderButtonRec()
    {
        Debug.Log("I surrender!");
    }

    public void onGameEnd()
    {
        menuButton.SetActive(true);
    }
}
