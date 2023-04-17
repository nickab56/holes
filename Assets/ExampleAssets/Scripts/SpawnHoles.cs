using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnHoles : MonoBehaviour
{
    public int maxHoles = 3;

    public GameObject holePrefab;

    List<ARAnchor> m_AnchorPoints;
    ARAnchorManager m_AnchorManager;
    ARPlaneManager m_PlaneManager;

    private List<TrackableId> trackableIDList;

    // Start is called before the first frame update
    void Start()
    {
        m_AnchorManager = GetComponent<ARAnchorManager>(); // Process all anchors and update their position and rotation
        m_PlaneManager = GetComponent<ARPlaneManager>(); // Surface detection
        m_AnchorPoints = new List<ARAnchor>();

        // Get all trackable planes, add their id to the list
        trackableIDList = new List<TrackableId>();
        GetTrackablePlanes();
        GenerateHoles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateHoles() {
        // For each hole
        Debug.Log("Beginning to Generate Holes...");
        for (int j = 0; j < maxHoles; j++) {
            // Select random plane from list of trackables
            Debug.Log("Getting plane id");
            TrackableId planeId = trackableIDList[GetRandomPlane()];
            Debug.Log("Attempting to get plane");
            ARPlane holePlane = m_PlaneManager.GetPlane(planeId);

            // Get random positions on plane (TODO)
            // For development purposes, the holes currently spawn in the center of the plane
            // (I believe) the planes are polygons. Therefore, we will need to determine how
            // we can check to ensure that the holes will spawn within the plane via the list
            // if boundaries found.
            Vector3 pos = holePlane.center;
            //Vector2[] boundaries = holePlane.boundary.ToArray();

            if (trackableIDList.Count >= maxHoles) {
                Collider[] hitColliders = Physics.OverlapSphere(center, radius);
                foreach (var hitCollider in hitColliders)
                {
                    hitCollider.SendMessage("AddDamage");
                }
                while (Physics.CheckSphere(pos, 3)) {
                    Collider[] hitColliders = Physics.OverlapSphere(center, radius);
                    foreach (var hitCollider in hitColliders)
                    {
                        hitCollider.SendMessage("AddDamage");
                    }
                }
            }

            // Instantiate hole
            ARAnchor anchor = m_AnchorManager.AttachAnchor(holePlane, new Pose(pos, Quaternion.identity));
            GameObject newHole = Instantiate(holePrefab, anchor.transform);
            if (holePlane.alignment == PlaneAlignment.Vertical) {
                newHole.transform.Rotate(90, 0, 0);
            }

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
    }

    private int GetRandomPlane() {
        Debug.Log("LIST COUNT: " + trackableIDList.Count);
        int index = Random.Range(0, trackableIDList.Count);
        return index;
    }

    private void GetTrackablePlanes() {
        Debug.Log("Finding all trackable planes...");
        foreach (ARPlane plane in m_PlaneManager.trackables) {
            trackableIDList.Add(plane.trackableId);
        }
        Debug.Log("Finished looking for planes. " + trackableIDList.Count + " found.");
    }

    // Removes all the anchors that have been created.
    private void RemoveAllAnchors()
    {
        foreach (var anchor in m_AnchorPoints)
        {
            Destroy(anchor);
        }
        m_AnchorPoints.Clear();
    }

    // Remove all holes
    public void RemoveAllHoles() {
        GameObject[] holes = GameObject.FindGameObjectsWithTag("Hole");
        foreach (GameObject hole in holes) {
            Destroy(hole);
        }
        RemoveAllAnchors();
    }
}
