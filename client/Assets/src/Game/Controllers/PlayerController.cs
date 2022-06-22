using System.Collections;
using System.Collections.Generic;


using UnityEngine;


using static GameManager;

public class PlayerController : MonoBehaviour
{
    public int id;
    public GameObject highlightCircle;
    public GameObject marker;
    Outline highlight;
    Renderer markerRenderer;

    void Start()
    {
        highlight = highlightCircle.GetComponent<Outline>();
        markerRenderer = marker.GetComponent<Renderer>();
        ResetHighlight();
    }

    void Update()
    {
        if (GameManager.state != null)
        {
            transform.position = new Vector3
            (
                GameManager.state.players[id].state.x,
                GameManager.state.players[id].state.z,
                GameManager.state.players[id].state.y
            );
            transform.eulerAngles = new Vector3
            (
                0,
                -GameManager.state.players[id].state.rotationAngle * Mathf.Rad2Deg,
                0
            );
        }
        
    }

    public void SetHighlight()
    {
        highlight.OutlineWidth = 5;
        markerRenderer.enabled = true;
    }

    public void ResetHighlight()
    {
        highlight.OutlineWidth = 0;
        markerRenderer.enabled = false;
    }
}
