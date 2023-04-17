using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SplashScreen : MonoBehaviour
{

    public float delayAmount = 0.2f;
    public TMP_Text titleTxt;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        titleTxt.fontSize = (Mathf.PingPong(Time.time * 0.65f, 1) * 18) + 84;
    }

    public void Play()
    {
        StartCoroutine(LoadSplash());
    }

    IEnumerator LoadSplash()
    {
        yield return new WaitForSeconds(delayAmount);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
