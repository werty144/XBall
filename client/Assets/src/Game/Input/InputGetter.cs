using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.WebSockets;


using UnityEngine;


using static InputProcessor;
using static SettingsManager;


public class InputGetter : MonoBehaviour
{
    Dictionary<string, KeyCode> actionsToKey;    

    void Start()
    {
        actionsToKey = SettingsManager.actionsToKey;
    }

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
                    InputProcessor.selectPlayer(selectedPlayer);
                    break;
                case "Field":
                    InputProcessor.fieldLeftClick(hit.point);
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
                InputProcessor.fieldRightClick(hit.point);
            }
        }

        if (Input.GetKey(actionsToKey["grab"]))
        {
            InputProcessor.grab();
        }

        if (Input.GetKey(actionsToKey["throw"]))
        {
            InputProcessor.throwIntention();
        }

        if (Input.GetKey(actionsToKey["attack"]))
        {
            InputProcessor.attack();
        }

        if (Input.GetKey(actionsToKey["turn"]))
        {
            InputProcessor.turnIntention();
        }

        if (Input.GetKey(actionsToKey["stop"]))
        {
            InputProcessor.stop();
        }

        if (Input.GetKey(actionsToKey["bend"]))
        {
            InputProcessor.bendIntention();
        }

        if (Input.GetKeyUp(actionsToKey["switchPlayer"]))
        {
            InputProcessor.switchPlayer();
        }

        if (Input.GetKey(actionsToKey["choosePlayer1"]))
        {
            InputProcessor.selectPlayer(1);
        }

        if (Input.GetKey(actionsToKey["choosePlayer2"]))
        {
            InputProcessor.selectPlayer(2);
        }

        if (Input.GetKey(actionsToKey["choosePlayer3"]))
        {
            InputProcessor.selectPlayer(3);
        }

        InputProcessor.processMousePosition(Input.mousePosition);
    }
}
