using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnHoles2 : MonoBehaviour
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
            Vector3 pos;
            ARPlane holePlane;

            int max_requests = 100;
            // Check to ensure that each hole gets a random spot
            do
            {
                // Select random plane from list of trackables
                TrackableId planeId = trackableIDList[GetRandomPlane()];
                holePlane = m_PlaneManager.GetPlane(planeId);

                // Get random positions on plane (TODO)
                // For development purposes, the holes currently spawn in the center of the plane
                // (I believe) the planes are polygons. Therefore, we will need to determine how
                // we can check to ensure that the holes will spawn within the plane via the list
                // if boundaries found.
                pos = holePlane.center;
                //Vector2[] boundaries = holePlane.boundary.ToArray();

                max_requests--;
            } while (IsDuplicate(pos) || max_requests > 0);

            Debug.Log("Hole #" + j + ": TrackableId: " + holePlane.trackableId);
            Debug.Log("Hole #" + j + ": Position: " + pos);
            Debug.Log("Normal: " + holePlane.normal);

            // Instantiate hole
            ARAnchor anchor = m_AnchorManager.AttachAnchor(holePlane, new Pose(pos, Quaternion.identity));
            GameObject newHole = Instantiate(holePrefab, anchor.transform);
            if (holePlane.alignment == PlaneAlignment.Vertical) {
                newHole.transform.Rotate(-90, 0, 0);
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

    private bool IsDuplicate(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, 1f);
        foreach (var hit in hits)
        {
            if (hit.gameObject.CompareTag("Hole"))
            {
                return true;
            }
        }
        return false;
    }
}
