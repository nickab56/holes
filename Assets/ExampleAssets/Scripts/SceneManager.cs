using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{

    public enum Stage { None, ROOM_MAP, PLACE_PLANKS, LEVEL_START, GRACE, GAME_OVER }

    public Stage CurrentStage;
    public TMP_Text ObjectiveTxt;
    public GameObject m_SessionManager;
    public GameObject NextBtn;
    public float GracePeriod;

    // Timer
    public float Timer; 
    private bool isTiming;

    // Start is called before the first frame update
    void Start()
    {
        isTiming = false;
        CurrentStage = Stage.None;
        // Begin setup
        UpdateStage(Stage.ROOM_MAP);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTiming) {
            Timer -= Time.deltaTime;
            if (Timer <= 0) {
                UpdateStage(Stage.GAME_OVER);
            }    
        }
    }

    public void UpdateStage(Stage NewStage) {
        // UpdateStage will be called to disable and reenable the appropriate scripts
        CurrentStage = NewStage;
        switch (CurrentStage) {
            case Stage.ROOM_MAP:
                // Instruct user to map room
                ObjectiveTxt.text = "Map Room";
                break;
            case Stage.PLACE_PLANKS:
                // Prepare game for player to place planks
                if (!m_SessionManager.GetComponent<SpawnPlanks>().isActiveAndEnabled) {
                    m_SessionManager.GetComponent<SpawnPlanks>().enabled = true;
                }
                ObjectiveTxt.text = "Set Location for planks";
                break;
            case Stage.LEVEL_START:
                // Begin game
                // Check if SpawnHoles needs enabled (need to get trackable planes)
                if (!m_SessionManager.GetComponent<SpawnHoles>().isActiveAndEnabled) {
                    m_SessionManager.GetComponent<SpawnHoles>().enabled = true;
                } else {
                    // Otherwise, generate holes
                    m_SessionManager.GetComponent<SpawnHoles>().GenerateHoles();
                }
                ObjectiveTxt.text = "Block holes";
                NextBtn.SetActive(false);
                break;
            case Stage.GRACE:
                // Check to ensure that this isn't the beginning of the game
                if (m_SessionManager.GetComponent<SpawnHoles>().isActiveAndEnabled) {
                    // Player has survived the level. Reset planks, destroy holes, and prepare
                    // for the next level
                    isTiming = false;
                    m_SessionManager.GetComponent<SpawnPlanks>().ResetPlanks();
                    m_SessionManager.GetComponent<SpawnHoles>().RemoveAllHoles();
                }
                // Initiate grace period
                StartCoroutine(BeginGracePeriod(GracePeriod, Stage.LEVEL_START));
                ObjectiveTxt.text = "Grace period";
                break;
            case Stage.GAME_OVER:
                // Transition to game over scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
                break;
            default:
                Debug.Log("There was an error updating stages.");
                break;
        }
    }

    IEnumerator BeginGracePeriod(float time, Stage NewStage) {
        yield return new WaitForSeconds(time);
        UpdateStage(NewStage);
    }
}
