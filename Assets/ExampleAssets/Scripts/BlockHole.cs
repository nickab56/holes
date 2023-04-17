using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHole : MonoBehaviour
{

    public SceneManager sceneManager;

    private AudioSource plankSFX;

    // Start is called before the first frame update
    void Start()
    {
        plankSFX = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Hole")) {
            if (!plankSFX.isPlaying)
            {
                plankSFX.PlayOneShot(plankSFX.clip, 1f);
            }
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
