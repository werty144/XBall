using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using log4net;


public class PerformanceTracker : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(PerformanceTracker));

    public static int MessagesFromServer = 0;
    public static int FPS = 0;

    static int seconds = 0;
    static string msg = "[Seconds:{0},MessagesFromServer:{1},FPS:{2}]";

    void Start()
    {
        InvokeRepeating("logStats", 2.0f, 1.0f);
    }

    void logStats()
    {
        seconds += 1;
        Log.Info(string.Format(msg, seconds, MessagesFromServer, FPS));
        zeroAll();
    }

    void zeroAll()
    {
        MessagesFromServer = 0;
        FPS = 0;
    }
}
