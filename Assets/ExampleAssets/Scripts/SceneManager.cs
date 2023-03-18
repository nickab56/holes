using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    public enum Stage { ROOM_MAP, PLACE_PLANKS, GAME_START, GRACE, GAME_OVER }

    public Stage CurrentStage;

    // Start is called before the first frame update
    void Start()
    {
        CurrentStage = Stage.ROOM_MAP;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStage();
    }

    private void UpdateStage() {
        switch (CurrentStage) {
            case Stage.ROOM_MAP:
                // Instruct user to map room
                break;
            case Stage.PLACE_PLANKS:
                // Prepare game for player to place planks
                break;
            case Stage.GAME_START:
                // Implement begin game
                break;
            case Stage.GRACE:
                // Implement grace period between levels
                // Call countdown to switch stages?
                break;
            case Stage.GAME_OVER:
                // Transition to game over scene
                break;
            default:
                Debug.Log("There was an error updating stages.");
                break;
        }
    }
}
