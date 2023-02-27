using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{

    public GameObject ARorigin;
    private SpawnHoles spawnHoles;

    // Start is called before the first frame update
    void Start()
    {
        spawnHoles = ARorigin.GetComponent<SpawnHoles>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HoleSpawn() {
        Debug.Log("EXECUTED HOLE SPAWN");
        spawnHoles.enabled = true;
    }
}
