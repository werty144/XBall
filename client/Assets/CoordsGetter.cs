using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.WebSockets;
using Newtonsoft.Json;


public class CoordsGetter : MonoBehaviour
{

    private float time = 0;
    public float radius = 2.0f;
    public Queue<string> messages = new Queue<string>();
    SocketConnection socketConnection;
    public static GameObject controlledUnit = null;         // instead of GameObject, could use custom type like ControllableUnit
    // Start is called before the first frame update
    void Start()
    {
        socketConnection = GameObject.FindWithTag("connection").GetComponent<SocketConnection>();

    }

    // Update is called once per frame
    void Update()
    {
        time += 1;
        if (Input.GetKey("i")) {
            messages.Enqueue("{\"path\": \"invite\", \"body\": {\"invitedId\": 0, \"speed\": \"FAST\", \"playersNumber\": 1}}");
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) // you can also only accept hits to some layer and put your selectable units in this layer
            {
                if(hit.collider.tag == "Player"){
                    if (controlledUnit != null) {
                        controlledUnit.GetComponent<PlayerScript>().ResetHighlight();
                    }
                    controlledUnit = hit.transform.gameObject; // if using custom type, cast the result to type here
                    print(controlledUnit.name);
                    controlledUnit.GetComponent<PlayerScript>().SetHighlight();
                } else {
                    if (controlledUnit != null) {
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
                // Debug.Log("Hit point: " + hit.point);
                MoveRequest request = new MoveRequest();
                request.path = "makeMove";
                MoveBody body = new MoveBody();
                Move m = new Move();
                m.playerId = controlledUnit.GetComponent<PlayerScript>().id;
                m.action = "movement";
                Point p = new Point();
                p.x = hit.point.x * 10;
                p.y = hit.point.z * 10;
                m.actionData = p;
                body.move = m;
                request.body = body;
                print(JsonConvert.SerializeObject(request));
                messages.Enqueue(JsonConvert.SerializeObject(request));
            }
        }
    }

    public bool hasState() {
        return socketConnection.state != null;
    }

    public Vector3 getPosition(int id) {
        // if (id == 1) {
        //     return new Vector3(radius * (float) Math.Sin(time / 1000) + , 0.0f, radius * (float) Math.Cos(time / 1000));
        // }
        // if (id == 2) {
        //     return new Vector3(radius * (float) Math.Sin(time / 1000 + 2), (float) 0, radius * (float) Math.Cos(time / 1000 + 2));
        // }
        return new Vector3(socketConnection.state.players[id].state.x / 10, socketConnection.state.players[id].state.z / 10, socketConnection.state.players[id].state.y / 10);
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
