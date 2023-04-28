using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PickupPlank2 : MonoBehaviour
{
    public ARRaycastManager m_RaycastManager;
    public ARPlaneManager m_PlaneManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    public GameObject PlanksPrefab;
    public RawImage plankUI;

    public SceneManager sceneManager;

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
                    GameObject spawnedPlank = Instantiate(PlanksPrefab, m_Hits[0].pose.position, Quaternion.identity);
                    ARPlane hitPlane = m_PlaneManager.GetPlane(m_Hits[0].trackableId);
                    if (hitPlane.alignment == PlaneAlignment.Vertical) {
                        spawnedPlank.transform.Rotate(90, 0, 0);
                        if (hitPlane.normal.x == 1.0f || hitPlane.normal.x == -1.0f) {
                            spawnedPlank.transform.Rotate(0, 0, 90);
                        }
                    }
                    spawnedPlank.GetComponent<BlockHole>().sceneManager = sceneManager;
                    isHoldingPlank = false;
                    plankUI.enabled = false;
                }
            }
        }
    }
}
