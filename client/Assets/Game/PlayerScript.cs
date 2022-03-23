using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SocketConnection;
using static Utils;

public class PlayerScript : MonoBehaviour
{
    public int id;
    public int userId;
    Renderer m_ObjectRenderer;
    Material highlightedMaterial;
    List<Material> startMaterials = new List<Material>();
    List<Material> highlightedMaterials;

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectRenderer = GetComponent<Renderer>();
        m_ObjectRenderer.GetMaterials(startMaterials);
        highlightedMaterial = Resources.Load("Materials/HighlightGreen", typeof(Material)) as Material;
        highlightedMaterials = new List<Material>(startMaterials);
        highlightedMaterials.Add(highlightedMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        if (SocketConnection.state != null)
        {
            gameObject.transform.position = Utils.serverFieldCoordsToUnityVector3
            (
                SocketConnection.state.players[id].state.x,
                SocketConnection.state.players[id].state.y,
                SocketConnection.state.players[id].state.z
            );
        }
        
    }

    public void SetHighlight()
    {
        m_ObjectRenderer.materials = highlightedMaterials.ToArray();
    }

    public void ResetHighlight()
    {
        m_ObjectRenderer.materials = startMaterials.ToArray();
    }

}
