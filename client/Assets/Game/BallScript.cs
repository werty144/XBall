using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static SocketConnection;
using static Utils;


public class BallScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SocketConnection.state != null) {
            gameObject.transform.position = Utils.serverFieldCoordsToUnityVector3
            (
                SocketConnection.state.ballState.x,
                SocketConnection.state.ballState.y,
                SocketConnection.state.ballState.z
            );
        }
    }
}
