using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PickUpPlank : MonoBehaviour
{

    public ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    public GameObject PlanksPrefab;

    Camera arCam;
    GameObject spawnedPlanks;

    private Mesh mesh;
    private Vector3[] newVerts;

    // Start is called before the first frame update
    void Start()
    {
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        spawnedPlanks = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if(m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began && spawnedPlanks == null)
            {
                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.collider.gameObject.tag == "Plank")
                    {
                        spawnedPlanks = hit.collider.gameObject;
                        ScaleMesh(1.5f);
                    }
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedPlanks != null)
            {
                spawnedPlanks.transform.position = m_Hits[0].pose.position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                ScaleMesh(1/1.5f);
                spawnedPlanks = null;
            }
        }
    }

    private void ScaleMesh(float scalar) {
        mesh = spawnedPlanks.GetComponent<MeshFilter>().mesh;
        newVerts = mesh.vertices;
        for(int i = 0; i < newVerts.Length; i++)
        {
            newVerts[i] *= scalar;
        }
        mesh.vertices = newVerts;
        mesh.RecalculateBounds();
    }
}
