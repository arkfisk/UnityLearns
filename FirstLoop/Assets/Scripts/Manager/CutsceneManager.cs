using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static bool isFinished = false;

    SplashManager theSplashManager;
    CameraController theCam;

    [SerializeField] Image img_CutScene;


    private void Start()
    {
        theSplashManager = FindObjectOfType<SplashManager>();
        theCam = FindObjectOfType<CameraController>();
    }

    public bool CheckCutScene()
    {
        return img_CutScene.gameObject.activeSelf;  //현재 객체가 활성화 상태인지 비활성화 상태인지 알려줌
    }

    public IEnumerator CutSceneCoroutine(string p_CutSceneName, bool p_isSlow)
    {
        SplashManager.isfinished = false;
        StartCoroutine(theSplashManager.FadeOut(true, false));
        yield return new WaitUntil(() => SplashManager.isfinished);

        if (p_isSlow)
        {
            Sprite t_Sprite = Resources.Load<Sprite>("CutScenes/" + p_CutSceneName);
            if (t_Sprite != null)
            {
                img_CutScene.gameObject.SetActive(true);
                img_CutScene.sprite = t_Sprite;
                theCam.CameraTargetting(null, 0.1f, true, false);
            }
            else
            {
                Debug.LogError("잘못된 컷신 CG파일 이름입니다.");
            }
        }
        else
        {
            img_CutScene.gameObject.SetActive(false);
        }

        SplashManager.isfinished = false;
        StartCoroutine(theSplashManager.FadeIn(true, false));
        yield return new WaitUntil(() => SplashManager.isfinished);

        yield return new WaitForSeconds(0.5f);

        isFinished = true;
    }
}
