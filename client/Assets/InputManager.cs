using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.WebSockets;
using Newtonsoft.Json;

using static SocketConnection;
using static Utils;


public class InputManager : MonoBehaviour
{

    public static GameObject controlledUnit = null;         // instead of GameObject, could use custom type like ControllableUnit

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) // you can also only accept hits to some layer and put your selectable units in this layer
            {
                if(hit.collider.tag == "Player")
                {
                    if (controlledUnit != null)
                    {
                        controlledUnit.GetComponent<PlayerScript>().ResetHighlight();
                    }
                    controlledUnit = hit.transform.gameObject; // if using custom type, cast the result to type here
                    controlledUnit.GetComponent<PlayerScript>().SetHighlight();
                } else 
                {
                    if (controlledUnit != null)
                    {
                        controlledUnit.GetComponent<PlayerScript>().ResetHighlight();
                    }
                    controlledUnit = null;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit) && hit.collider.tag == "Field" && controlledUnit != null) {
                MoveRequest request = new MoveRequest();
                request.path = "makeMove";
                MoveBody body = new MoveBody();
                Move m = new Move();
                m.playerId = controlledUnit.GetComponent<PlayerScript>().id;
                m.action = "movement";
                Point p = Utils.unityFieldPointToServerPoint(hit.point);
                m.actionData = p;
                body.move = m;
                request.body = body;
                SocketConnection.messages.Enqueue(JsonConvert.SerializeObject(request));
            }
        }
    }
}


public class MoveRequest {
    public string path;
    public MoveBody body;
}

public class MoveBody {
    public Move move;
}

public class Move {
    public int playerId;
    public string action;
    public Point actionData;
}
