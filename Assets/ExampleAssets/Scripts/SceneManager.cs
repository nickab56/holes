using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public enum Stage { None, ROOM_MAP, PLACE_PLANKS, LEVEL_START, GRACE, GAME_OVER }
    public Stage CurrentStage;

    public GameObject m_SessionManager;
    public GameObject NextBtn;
    public GameObject m_SpawnManager;

    public TMP_Text ObjectiveTxt;
    public TMP_Text lvlText;

    public int currentLevel = 1;
    public float GracePeriod;
    public int numOfActiveHoles = 1;
    public int numOfUsablePlanks = 1;

    // Timer
    public float Timer; 
    private bool isTiming;

    // Start is called before the first frame update
    void Start()
    {
        Timer = 30f;
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
        if (numOfActiveHoles == 0) {
            UpdateStage(Stage.GRACE);
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
                m_SpawnManager.GetComponent<PickupPlank2>().enabled = true;
                // Begin Timer
                isTiming = true;
                // Update UI
                ObjectiveTxt.text = "Block holes";
                NextBtn.SetActive(false);
                break;
            case Stage.GRACE:
                // Check to ensure that this isn't the beginning of the game
                if (m_SessionManager.GetComponent<SpawnHoles>().isActiveAndEnabled) {
                    // Player has survived the level. Reset planks, destroy holes, and prepare
                    // for the next level
                    isTiming = false;
                    Timer = 30;
                    NextLevel();
                }
                // Set number of active holes and usable holes
                numOfActiveHoles = m_SessionManager.GetComponent<SpawnHoles>().maxHoles;
                numOfUsablePlanks = m_SessionManager.GetComponent<SpawnPlanks>().maxPlanks;
                Debug.Log("Number of active holes set to: " + numOfActiveHoles);
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

    public void NextLevel()
    {
        currentLevel++;
        lvlText.text = "Level: " + currentLevel;
        m_SessionManager.GetComponent<SpawnHoles>().maxHoles = (int) (m_SessionManager.GetComponent<SpawnHoles>().maxHoles * 1.25);
        m_SessionManager.GetComponent<SpawnPlanks>().maxPlanks = m_SessionManager.GetComponent<SpawnHoles>().maxHoles;
        
        m_SessionManager.GetComponent<SpawnPlanks>().ResetPlanks();
        m_SessionManager.GetComponent<SpawnHoles>().RemoveAllHoles();
        m_SpawnManager.GetComponent<PickupPlank2>().enabled = false;
    }

    IEnumerator BeginGracePeriod(float time, Stage NewStage) {
        yield return new WaitForSeconds(time);
        UpdateStage(NewStage);
    }
}
