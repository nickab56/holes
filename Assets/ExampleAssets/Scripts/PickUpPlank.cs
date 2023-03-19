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
    GameObject SpawnedPlanks;
    // Start is called before the first frame update
    void Start()
    {
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        SpawnedPlanks = null;
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
            if(Input.GetTouch(0).phase == TouchPhase.Began && SpawnedPlanks == null)
            {
                if(Physics.Raycast(ray, out hit))
                {   
                    if(hit.collider.gameObject.tag == "Spawnable")
                    {
                        SpawnedPlanks = hit.collider.gameObject;
                    }
                    else
                    {
                        SpawnPrefab(m_Hits[0].pose.position);
                    }
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && SpawnedPlanks != null)
            {
                SpawnedPlanks.transform.position = m_Hits[0].pose.position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                SpawnedPlanks = null;
            }
        }
    }

    private void SpawnPrefab(Vector3 SpawnPosition)
    {
        SpawnedPlanks = Instantiate(PlanksPrefab, SpawnPosition, Quaternion.identity);
    }
}
