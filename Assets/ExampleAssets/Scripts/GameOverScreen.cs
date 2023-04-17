using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{

    public float delayAmount;

    public TMP_Text recentScoreTxt;
    public TMP_Text highScoreTxt;

    // Start is called before the first frame update
    void Start()
    {
        recentScoreTxt.text = "Score: " + PlayerPrefs.GetInt("recent_score");
        highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("high_score");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadSplash());
    }

    IEnumerator LoadSplash()
    {
        yield return new WaitForSeconds(delayAmount);
        UnityEngine.SceneManagement.SceneManager.LoadScene("SplashScene");
    }
}
