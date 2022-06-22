using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.UI;


public class LoggerViewController : MonoBehaviour
{
    [SerializeField] private GameObject logBarPrefab;

    public void addLog(string log)
    {
        var logBar = Instantiate(logBarPrefab);
        logBar.GetComponent<Text>().text = log;
        logBar.transform.SetParent(this.transform);
    }
}
