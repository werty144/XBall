using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMeasure : MonoBehaviour
{
    void Update()
    {
        PerformanceTracker.FPS += 1;
    }
}
