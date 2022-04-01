using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameManager;

public class PlayerController : MonoBehaviour
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
        // m_ObjectRenderer = GetComponent<Renderer>();
        // m_ObjectRenderer.GetMaterials(startMaterials);
        // highlightedMaterial = Resources.Load("Materials/HighlightGreen", typeof(Material)) as Material;
        // highlightedMaterials = new List<Material>(startMaterials);
        // highlightedMaterials.Add(highlightedMaterial);
    }

    // Update is called once per frame
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
        // m_ObjectRenderer.materials = highlightedMaterials.ToArray();
    }

    public void ResetHighlight()
    {
        // m_ObjectRenderer.materials = startMaterials.ToArray();
    }

}
