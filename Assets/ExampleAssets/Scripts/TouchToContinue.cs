using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchToContinue : MonoBehaviour
{
    private SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        sceneManager = this.GetComponent<SceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (sceneManager.viewingTip) {
                sceneManager.DisableTip();
                return;
            }
            
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
}
