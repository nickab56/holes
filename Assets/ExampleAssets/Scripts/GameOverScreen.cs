using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{

    public float delayAmount;

    // Start is called before the first frame update
    void Start()
    {
        
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
