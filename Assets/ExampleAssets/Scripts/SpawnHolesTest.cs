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
            Vector3 pos;
            ARPlane holePlane;

            int maxAttempts = 100;
            // Check to ensure that each hole gets a random spot
            do
            {
                // Select random plane from list of trackables
                TrackableId planeId = trackableIDList[GetRandomPlane()];
                holePlane = m_PlaneManager.GetPlane(planeId);

                // Get random positions on plane
                pos = holePlane.center;

                // Get minimum and maxmium ranges of the plane
                float xRange = holePlane.size.x/2;
                float yRange = holePlane.size.y/2;

                // Update the position based on the Plane's normal
                if (holePlane.normal.x == 1.0f || holePlane.normal.x == -1.0f) {
                    pos = new(pos.x, pos.y + Random.Range(-yRange, yRange), pos.z + Random.Range(-xRange, xRange));
                } else if (holePlane.normal.y == 1.0f || holePlane.normal.y == -1.0f) {
                    float bound = Mathf.Min(Mathf.Abs(xRange), Mathf.Abs(yRange)); 
                    pos = new(pos.x + Random.Range(-bound, bound), pos.y, pos.z + Random.Range(-bound, bound));
                } else {
                    pos = new(pos.x + Random.Range(-xRange, xRange), pos.y + Random.Range(-yRange, yRange), pos.z);
                }

                maxAttempts--;
            } while (IsDuplicate(pos) && maxAttempts > 0);

            Debug.Log("Hole #" + j + ": TrackableId: " + holePlane.trackableId);
            Debug.Log("Hole #" + j + ": Position: " + pos);
            Debug.Log("Normal: " + holePlane.normal);

            // Instantiate hole
            ARAnchor anchor = m_AnchorManager.AttachAnchor(holePlane, new Pose(pos, Quaternion.identity));
            GameObject newHole = Instantiate(holePrefab, anchor.transform);
            
            Quaternion planeOrientation = Quaternion.LookRotation(holePlane.normal, Vector3.up);
            newHole.transform.rotation = planeOrientation;
            // Flip rotation
            newHole.transform.rotation *= Quaternion.Euler(0, 180f, 0);
            // Rotate X so that pipe isn't upside down
            newHole.transform.Rotate(180, 0, 0);

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
        Collider[] hits = Physics.OverlapSphere(pos, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.gameObject.CompareTag("Hole") || hit.gameObject.CompareTag("Plank"))
            {
                return true;
            }
        }
        return false;
    }
}