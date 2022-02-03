using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    int a = 0;
    public int id;
    float speed = 0.05f;
    CoordsGetter coordsGetter;
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
        coordsGetter = GameObject.FindWithTag("coords").GetComponent<CoordsGetter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w")) {
            gameObject.transform.position += Vector3.forward * speed;
        }
        if (Input.GetKey("a")) {
            gameObject.transform.position += Vector3.left * speed;
        }
        if (Input.GetKey("s")) {
            gameObject.transform.position += Vector3.back * speed;
        }
        if (Input.GetKey("d")) {
            gameObject.transform.position += Vector3.right * speed;
        }
        gameObject.transform.position = coordsGetter.getPosition(id);
        
    }
    void OnGUI()
    {
        Event e = Event.current;
        if (e.control)
        {
            Debug.Log("Control was pressed.");
        }
    }

    public void SetHighlight() {
        m_ObjectRenderer.materials = highlightedMaterials.ToArray();
    }

    public void ResetHighlight() {
        m_ObjectRenderer.materials = startMaterials.ToArray();
    }

}
