using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public void menuButtonRec()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void surrenderButtonRec()
    {
        Debug.Log("I surrender!");
    }
}
