using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnPlanks : MonoBehaviour
{
    // Game Objects
    public GameObject PlankPrefab;
    private GameObject SpawnedPlank;
    public GameObject DisplayPlankPrefab; 
    private GameObject DisplayPlank;

    // Private vars
    private ARRaycastManager m_RaycastManager;
    private List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    private Camera arCam;

    // Start is called before the first frame update
    void Start()
    {
        SpawnedPlank = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        // Start Spawning Display Plank
        Vector3 camPoint = arCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        DisplayPlank = Instantiate(DisplayPlankPrefab, camPoint, Quaternion.identity);
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        MoveDisplayPlank();
    }

    private void GetInput() {
        if (Input.touchCount == 0)
            return;
        
        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began && SpawnedPlank == null)
            {
                if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.collider.gameObject.tag == "Spawnable")
                            {
                                SpawnedPlank = hit.collider.gameObject;
                            }
                            else
                            {
                                SpawnPrefab(m_Hits[0].pose.position);
                            }
                        }
        
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved && SpawnedPlank != null)
            {
                SpawnedPlank.transform.position = m_Hits[0].pose.position;
            }
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                SpawnedPlank = null;
             }
        }
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        SpawnedPlank = Instantiate(PlankPrefab, spawnPosition, Quaternion.identity);
    }

    private void MoveDisplayPlank() {
        DisplayPlank.transform.position = arCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
    }
}
