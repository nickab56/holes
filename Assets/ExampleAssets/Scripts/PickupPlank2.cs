using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PickupPlank2 : MonoBehaviour
{
    public ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    public GameObject PlanksPrefab;
    public RawImage plankUI;

    private Camera arCam;
    private bool isHoldingPlank;

    // Start is called before the first frame update
    void Start()
    {
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        isHoldingPlank = false;
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

        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits, TrackableType.PlaneWithinPolygon))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit) && !isHoldingPlank)
                {
                    if (hit.collider.gameObject.tag == "Plank")
                    {
                        Destroy(hit.collider.gameObject);
                        isHoldingPlank = true;
                        plankUI.enabled = true;
                    }
                } 
                else
                {
                    Instantiate(PlanksPrefab, m_Hits[0].pose.position, Quaternion.identity);
                    isHoldingPlank = false;
                    plankUI.enabled = false;
                }
            }
        }
    }
}
