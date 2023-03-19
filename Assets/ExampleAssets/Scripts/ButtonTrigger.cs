using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
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

    public void ReadyForNextStage() {
        switch (sceneManager.CurrentStage) {
            case SceneManager.Stage.ROOM_MAP:
                sceneManager.UpdateStage(SceneManager.Stage.PLACE_PLANKS);
                break;
            case SceneManager.Stage.PLACE_PLANKS:
                sceneManager.UpdateStage(SceneManager.Stage.GRACE);
                break;
            default:
                Debug.Log("Error transitioning stages: " + sceneManager.CurrentStage);
                break;
        }
    }
}
