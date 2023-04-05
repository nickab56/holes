using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnPlanks : MonoBehaviour
{
    // Game Vars
    public int maxPlanks = 3;

    // Game Objects
    public GameObject plankPrefab;
    private GameObject spawnedPlank;
    public GameObject displayPlankPrefab; 
    private GameObject displayPlank;

    public SceneManager sceneManager;

    // Private vars
    private ARRaycastManager m_RaycastManager;
    private ARPlaneManager m_PlaneManager;
    private List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    private List<ARAnchor> m_AnchorPoints;
    private ARAnchorManager m_AnchorManager;
    
    private Camera arCam;
    private Vector3 camPoint;
    private ARPlane currentPlane;

    private List<Vector3> SpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        spawnedPlank = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();

        // Start Spawning Display Plank
        camPoint = arCam.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, arCam.nearClipPlane));
        displayPlank = Instantiate(displayPlankPrefab, camPoint, Quaternion.identity);

        // Get Components
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_AnchorPoints = new List<ARAnchor>();
        SpawnPos = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (displayPlank != null) {
            MoveDisplayPlank();
            GetInput();
        }
    }

    private void GetInput() {
        if (Input.touchCount == 0)
            return;

        // Check if player is touching
        if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedPlank == null) {
            // Spawn planks
            SpawnAllPlanks();
        }
    }

    private void SpawnAllPlanks()
    {
        for (int i = 0; i < maxPlanks; i++) {
            // Spawn planks using display plank's position
            Vector3 PlankPos = new(displayPlank.transform.position.x, displayPlank.transform.position.y+(i*.1f), displayPlank.transform.position.z);
            AddPlank(currentPlane, PlankPos);
            Debug.Log("Plank #" + i + ": " + spawnedPlank.transform.position);   
        }
        // Display plank is no longer needed
        Destroy(displayPlank);
    }

    private void MoveDisplayPlank() {
        // Cast ray from screen to world 
        camPoint = new Vector3(Screen.width/2, Screen.height/2, arCam.nearClipPlane);
        Ray ray = arCam.ScreenPointToRay(camPoint);

        // Spawn planks if trackable plane found
        if (m_RaycastManager.Raycast(camPoint, m_Hits, TrackableType.PlaneWithinPolygon) && m_PlaneManager.GetPlane(m_Hits[0].trackableId)) {
            // Update display plank's position
            currentPlane = m_PlaneManager.GetPlane(m_Hits[0].trackableId);
            displayPlank.transform.position = m_Hits[0].pose.position;
        }
    }

    private void AddPlank(ARPlane PlankPlane, Vector3 AnchorPos) {  
        if (PlankPlane == null) {
            Debug.Log("Error creating plank: Not a valid plane");
            return;
        }

        // Valid plane found. Instantiate plank and attach it to an anchor          
        ARAnchor anchor = m_AnchorManager.AttachAnchor(PlankPlane, new Pose(AnchorPos, Quaternion.identity));
        spawnedPlank = Instantiate(plankPrefab, anchor.transform);
        spawnedPlank.GetComponent<BlockHole>().sceneManager = sceneManager;

        // Store plank's position
        SpawnPos.Add(spawnedPlank.transform.position);

        // Check if anchor is null before storing anchor
        if (anchor == null)
        {
            Debug.Log("Error creating anchor.");
        }
        else
        {
            // Stores the anchor so that it may be removed later.
            m_AnchorPoints.Add(anchor);
        }
    }

    private void RemoveAllAnchors()
    {
        foreach (var anchor in m_AnchorPoints)
        {
            Destroy(anchor);
        }
        m_AnchorPoints.Clear();
    }

    public void ResetPlanks() {
        GameObject[] planks = GameObject.FindGameObjectsWithTag("Unusable");
        for (int i = 0; i < planks.Length; i++) {
            planks[i].transform.position = SpawnPos[i];
            planks[i].tag = "Plank";
        }
    }
}
