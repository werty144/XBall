using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using log4net;

public class Test : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(Test));
    // Start is called before the first frame update
    void Start()
    {
        Log.Debug("huy");   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
