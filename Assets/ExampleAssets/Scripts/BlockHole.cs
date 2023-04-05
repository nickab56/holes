using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHole : MonoBehaviour
{

    public SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Hole")) {
            Vector3 holePos = collider.gameObject.transform.position;
            Destroy(collider.gameObject);
            this.transform.position = holePos;
            this.tag = "Unusable";
            // Decrease number of active holes
            sceneManager.numOfActiveHoles--;
            sceneManager.numOfUsablePlanks--;
        }
    }
}
