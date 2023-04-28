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
    public GameObject m_SpawnManager;

    public TMP_Text ObjectiveTxt;
    public TMP_Text lvlText;
    public TMP_Text countdownTxt;
    public TMP_Text countdownSubTxt;
    public TMP_Text continueTxt;

    public int currentLevel = 1;
    public int GracePeriod;
    public int numOfActiveHoles = 1;
    public int numOfUsablePlanks = 1;

    public AudioSource countdownSFX;
    public AudioSource userHelpSFX;

    public GameObject rmTip;
    public GameObject ppTip;

    public bool viewingTip;

    // High score implementation
    private int HighScore = 0;
    private const string HighScoreKey = "high_score";
    private const string RecentScoreKey = "recent_score";

    // Timer
    public float Timer; 
    private bool isTiming;

    private TouchToContinue touchToContinue;

    // Start is called before the first frame update
    void Start()
    {
        Timer = 30f;
        isTiming = false;
        CurrentStage = Stage.None;
        touchToContinue = this.GetComponent<TouchToContinue>();
        viewingTip = false;
        
        // Begin setup
        UpdateStage(Stage.ROOM_MAP);

        // Check if high score already exists
        if (PlayerPrefs.HasKey(HighScoreKey))
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey);
        }
        else
        {
            PlayerPrefs.SetInt(HighScoreKey, currentLevel);
        }
        PlayerPrefs.SetInt(RecentScoreKey, currentLevel);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTiming) {
            Timer -= Time.deltaTime;
            if (Timer <= 0) {
                if (currentLevel > HighScore)
                {
                    PlayerPrefs.SetInt(HighScoreKey, currentLevel);
                }
                PlayerPrefs.SetInt(RecentScoreKey, currentLevel);
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
                InitiateTip();
                ObjectiveTxt.text = "Map Room";
                break;
            case Stage.PLACE_PLANKS:
                // Prepare game for player to place planks
                InitiateTip();
                ObjectiveTxt.text = "Set location for planks";
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
                continueTxt.enabled = false;
                lvlText.enabled = true;
                lvlText.text = "Level: " + currentLevel;
                break;
            case Stage.GRACE:
                // Check to ensure that this isn't the beginning of the game
                if (m_SessionManager.GetComponent<SpawnHoles>().isActiveAndEnabled) {
                    // Player has survived the level. Reset planks, destroy holes, and prepare
                    // for the next level
                    isTiming = false;
                    Timer = 30;
                    NextLevel();
                } else {
                    touchToContinue.enabled = false;
                }
                // Set number of active holes and usable holes
                numOfActiveHoles = m_SessionManager.GetComponent<SpawnHoles>().maxHoles;
                numOfUsablePlanks = m_SessionManager.GetComponent<SpawnPlanks>().maxPlanks;
                Debug.Log("Number of active holes set to: " + numOfActiveHoles);
                // Initiate grace period
                BeginGracePeriod(GracePeriod, Stage.LEVEL_START);
                ObjectiveTxt.text = "";
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
        m_SessionManager.GetComponent<SpawnHoles>().maxHoles = Mathf.CeilToInt(m_SessionManager.GetComponent<SpawnHoles>().maxHoles * 1.25f);
        m_SessionManager.GetComponent<SpawnPlanks>().maxPlanks = m_SessionManager.GetComponent<SpawnHoles>().maxHoles;
        
        m_SessionManager.GetComponent<SpawnPlanks>().ResetPlanks();
        m_SessionManager.GetComponent<SpawnHoles>().RemoveAllHoles();
        m_SpawnManager.GetComponent<PickupPlank2>().enabled = false;
    }

    public void BeginGracePeriod(int time, Stage NewStage) {
        countdownTxt.enabled = true;
        countdownSubTxt.enabled = true;
        countdownSubTxt.text = "Level " + currentLevel + "<br>will begin in";
        StartCoroutine(CountDown(time, NewStage));
    }

    IEnumerator CountDown(int time, Stage NewStage) {
        if (time == 0) {
            countdownSubTxt.enabled = false;
            countdownTxt.text = "GO";
        } else {
            countdownTxt.text = time.ToString();
            if (!countdownSFX.isPlaying) {
                countdownSFX.PlayOneShot(countdownSFX.clip, 1f);
            }
        }
        yield return new WaitForSeconds(1);
        if (time > 0) {
            StartCoroutine(CountDown(time-1, NewStage));
        } else {
            countdownTxt.enabled = false;
            UpdateStage(NewStage);
        }
    }

    public void InitiateTip() {
        viewingTip = true;
        if (CurrentStage == Stage.ROOM_MAP) {
            rmTip.SetActive(true);
            if (!userHelpSFX.isPlaying) {
                userHelpSFX.PlayOneShot(userHelpSFX.clip, 1f);
            }
        } else if (CurrentStage == Stage.PLACE_PLANKS) {
            ppTip.SetActive(true);
            if (!userHelpSFX.isPlaying) {
                userHelpSFX.PlayOneShot(userHelpSFX.clip, 1f);
            }
        } else {
            Debug.Log("There was an error initializing the tip.");
        }
    }

    public void DisableTip() {
        viewingTip = false;
        if (CurrentStage == Stage.ROOM_MAP) {
            rmTip.SetActive(false);
        } else if (CurrentStage == Stage.PLACE_PLANKS) {
            ppTip.SetActive(false);
            m_SessionManager.GetComponent<SpawnPlanks>().enabled = true;
        } else {
            Debug.Log("There was an error disabling the tip.");
        }
    }
}
