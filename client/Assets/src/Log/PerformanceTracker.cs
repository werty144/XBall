using System.Collections;
using System.Collections.Generic;
using System.Linq;


using UnityEngine;
using log4net;


public class PerformanceTracker : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(PerformanceTracker));

    public static int MessagesFromServer = 0;
    public static int FPS = 0;
    public static int P2PSent = 0;
    public static List<int> P2PReceived = new List<int>();

    static int seconds = 0;
    static string msg = "[Seconds:{0},MessagesFromServer:{1},FPS:{2},P2PReceived:{3},AvgP2PMessageBucket:{4}]";

    void Start()
    {
        // InvokeRepeating("logStats", 2.0f, 1.0f);
    }

    void logStats()
    {
        seconds += 1;
        Log.Info(string.Format(
            msg,
            seconds,
            MessagesFromServer,
            FPS,
            P2PReceived.Sum(),
            AvgOrZero()
        ));
        zeroAll();
    }

    void zeroAll()
    {
        MessagesFromServer = 0;
        FPS = 0;
        P2PReceived.Clear();
    }

    double AvgOrZero()
    {
        if (P2PReceived.Count == 0) return 0.0d;
        return P2PReceived.Average();
    }
}
