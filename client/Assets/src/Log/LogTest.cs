using System.Collections;
using System.Collections.Generic;

using log4net;
using UnityEngine;

public class LogTest : MonoBehaviour
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(LogTest));
    void Start()
    {
        Log.Debug("My first debug");
    }
  
}
