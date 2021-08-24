using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferManager : MonoBehaviour
{
    string locationName;

    SplashManager theSplashManager;
    InteractionController theIC;

    public static bool isFinished = true;


    private void Start()
    {
        theSplashManager = FindObjectOfType<SplashManager>();
        theIC = FindObjectOfType<InteractionController>();
    }

    public IEnumerator Transfer(string p_SceneName, string p_LocationName)
    {
        isFinished = false;
        theIC.SettingUI(false);
        SplashManager.isfinished = false;
        StartCoroutine(theSplashManager.FadeOut(false, true));
        yield return new WaitUntil(() => SplashManager.isfinished);

        locationName = p_LocationName;
        TransferSpawnManager.spawnTiming = true;
        SceneManager.LoadScene(p_SceneName);
    }

    public IEnumerator Done()
    {
        SplashManager.isfinished = false;
        StartCoroutine(theSplashManager.FadeIn(false, true));
        yield return new WaitUntil(() => SplashManager.isfinished);
        isFinished = true;

        yield return new WaitForSeconds(0.3f);
        if(!DialogueManager.isWaiting)
            theIC.SettingUI(true);
    }

    public string GetLocationName()
    {
        return locationName;
    }
}
