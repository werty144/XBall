using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static GameManager;
using static Utils;


public class BallController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.state != null) {
            gameObject.transform.position = Utils.serverFieldCoordsToUnityVector3
            (
                GameManager.state.ballState.x,
                GameManager.state.ballState.y,
                GameManager.state.ballState.z
            );
        }
    }
}
