using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.WebSockets;


using static SocketConnection;
using static InputProcessor;
using static SettingsManager;


public class InputGetter : MonoBehaviour
{

    InputProcessor inputProcessor;
    Dictionary<string, KeyCode> actionsToKey;    

    // Start is called before the first frame update
    void Start()
    {
        inputProcessor = new InputProcessor();
        actionsToKey = SettingsManager.actionsToKey;
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
                switch (hit.collider.tag)
                {
                case "Player":
                    var selectedPlayer = hit.transform.gameObject; // if using custom type, cast the result to type here
                    inputProcessor.selectPlayer(selectedPlayer);
                    break;
                case "Field":
                    inputProcessor.fieldLeftClick(hit.point);
                    break;
                default:
                    break;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit) && hit.collider.tag == "Field")
            {
                inputProcessor.fieldRightClick(hit.point);
            }
        }

        if (Input.GetKey(actionsToKey["grab"]))
        {
            inputProcessor.grab();
        }

        if (Input.GetKey(actionsToKey["throw"]))
        {
            inputProcessor.throwIntention();
        }

        if (Input.GetKey(actionsToKey["attack"]))
        {
            inputProcessor.attack();
        }

        if (Input.GetKey(actionsToKey["turn"]))
        {
            inputProcessor.turnIntention();
        }

        if (Input.GetKey(actionsToKey["stop"]))
        {
            inputProcessor.stop();
        }

        if (Input.GetKey(actionsToKey["bend"]))
        {
            inputProcessor.bendIntention();
        }

        if (Input.GetKeyUp(actionsToKey["switchPlayer"]))
        {
            inputProcessor.switchPlayer();
        }

        inputProcessor.processMousePosition(Input.mousePosition);
    }
}
