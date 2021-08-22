using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static bool isWaiting = false;

    [SerializeField] GameObject go_DialogueBar;
    [SerializeField] GameObject go_DialogueNameBar;

    [SerializeField] Text txt_Dialogue;
    [SerializeField] Text txt_Name;

    Dialogue[] dialogues;

    bool isDialogue = false;
    bool isNext = false;

    [Header("텍스트 출력 딜레이")]
    [SerializeField] float textDelay;

    int lineCount = 0;
    int contextCount = 0;

    //이벤트 끝나면 등장시킬(퇴장시킬) 오브젝트들
    GameObject[] go_Objects;
    byte appearTypeNumber;
    const byte NONE = 0, APPEAR = 1, DISAPPEAR = 2;

    public void SetAppearObjects(GameObject[] p_Targets)
    {
        go_Objects = p_Targets;
        appearTypeNumber = APPEAR;
    }

    public void SetDisappearObjects(GameObject[] p_Targets)
    {
        go_Objects = p_Targets;
        appearTypeNumber = DISAPPEAR;
    }


    InteractionController theIC;
    CameraController theCam;

    SpriteManager theSpriteManager;
    SplashManager theSplashManager;
    CutsceneManager theCutSceneManager;
    SlideManager theSlideManager;
    

    private void Start()
    {
        theIC = FindObjectOfType<InteractionController>();
        theCam = FindObjectOfType<CameraController>();
        theSpriteManager = FindObjectOfType<SpriteManager>();
        theSplashManager = FindObjectOfType<SplashManager>();
        theCutSceneManager = FindObjectOfType<CutsceneManager>();
        theSlideManager = FindObjectOfType<SlideManager>();
    }

    private void Update()
    {
        if (isDialogue)
        {
            if (isNext)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isNext = false;
                    txt_Dialogue.text = "";
                    if (++contextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(TypeWriter());
                    }
                    else
                    {
                        contextCount = 0;
                        if (++lineCount < dialogues.Length)
                        {
                            StartCoroutine(CameraTargettingType());
                        }
                        else
                        {
                            StartCoroutine(EndDialogue());
                        }
                    }
                }
            }
        }
    }

    public void ShowDialogue(Dialogue[] p_dialogues)
    {
        isDialogue = true;
        txt_Dialogue.text = "";
        txt_Name.text = "";
        theIC.SettingUI(false);
        dialogues = p_dialogues;

        StartCoroutine(StartDialogue());

    }

    IEnumerator StartDialogue()
    {
        if (isWaiting)
            yield return new WaitForSeconds(0.5f);

        isWaiting = false;
        theCam.CamOrignSetting();
        StartCoroutine(CameraTargettingType());
    }

    IEnumerator CameraTargettingType()
    {
        switch (dialogues[lineCount].cameraType)
        {
            case CameraType.FadeIn:
                SettingUI(false);  SplashManager.isfinished = false; StartCoroutine(theSplashManager.FadeIn(false, true));
                yield return new WaitUntil(() => SplashManager.isfinished);
                break;
            case CameraType.FadeOut:
                SettingUI(false); SplashManager.isfinished = false; StartCoroutine(theSplashManager.FadeOut(false, true));
                yield return new WaitUntil(() => SplashManager.isfinished);
                break;
            case CameraType.FlashIn:
                SettingUI(false); SplashManager.isfinished = false; StartCoroutine(theSplashManager.FadeIn(true, true));
                yield return new WaitUntil(() => SplashManager.isfinished);
                break;
            case CameraType.FlashOut:
                SettingUI(false); SplashManager.isfinished = false; StartCoroutine(theSplashManager.FadeOut(true, true));
                yield return new WaitUntil(() => SplashManager.isfinished);
                break;
            case CameraType.ObjectFront: theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
            case CameraType.Reset: theCam.CameraTargetting(null, 0.05f, true, false);
                break;
            case CameraType.ShowCutScene:
                SettingUI(false); CutsceneManager.isFinished = false;
                StartCoroutine(theCutSceneManager.CutSceneCoroutine(dialogues[lineCount].spriteName[contextCount], true));
                yield return new WaitUntil(() => CutsceneManager.isFinished);
                break;
            case CameraType.HideCutScene:
                SettingUI(false); CutsceneManager.isFinished = false;
                StartCoroutine(theCutSceneManager.CutSceneCoroutine(null, false));
                yield return new WaitUntil(() => CutsceneManager.isFinished);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
            case CameraType.AppearSlideCG:
                SlideManager.isFinished = false;
                StartCoroutine(theSlideManager.AppearSlide(SplitSlideCGName()));
                yield return new WaitUntil(() => SlideManager.isFinished);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
            case CameraType.DisappearSlideCG:
                SlideManager.isFinished = false;
                StartCoroutine(theSlideManager.DisappearSlide());
                yield return new WaitUntil(() => SlideManager.isFinished);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
            case CameraType.ChangeSlideCG:
                SlideManager.isFinished = false;
                StartCoroutine(theSlideManager.ChangeSlide(SplitSlideCGName()));
                yield return new WaitUntil(() => SlideManager.isFinished);
                theCam.CameraTargetting(dialogues[lineCount].tf_Target);
                break;
        }
        StartCoroutine(TypeWriter());
    }

    string SplitSlideCGName()
    {
        string t_Text = dialogues[lineCount].spriteName[contextCount];
        string[] t_Array = t_Text.Split(new char[] { '/' });
        if (t_Array.Length <= 1)
            return t_Array[0];
        else
            return t_Array[1];
    }

    IEnumerator EndDialogue()
    {
        if (theCutSceneManager.CheckCutScene())
        {
            CutsceneManager.isFinished = false;
            StartCoroutine(theCutSceneManager.CutSceneCoroutine(null, false));
            yield return new WaitUntil(() => CutsceneManager.isFinished);
        }

        AppearOrDisappearObjects();

        yield return new WaitUntil(() => SpinSprite.isFinished);

        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        theCam.CameraTargetting(null, 0.05f, true, true);
        SettingUI(false);
    }

    void AppearOrDisappearObjects()
    {
        if (go_Objects != null)
        {
            SpinSprite.isFinished = false;
            for (int i = 0; i < go_Objects.Length; i++)
            {
                if (appearTypeNumber == APPEAR)
                {
                    go_Objects[i].SetActive(true);
                    StartCoroutine(go_Objects[i].GetComponent<SpinSprite>().SetAppearOrDiasppear(true));
                }

                else if (appearTypeNumber == DISAPPEAR)
                    StartCoroutine(go_Objects[i].GetComponent<SpinSprite>().SetAppearOrDiasppear(false));
            }
        }
        go_Objects = null;
        appearTypeNumber = NONE;
    }

    void ChangeSprite()
    {
        if (dialogues[lineCount].tf_Target!=null)
        {
            if (dialogues[lineCount].spriteName[contextCount] != "")
            {
                StartCoroutine(theSpriteManager.SpriteChangeCoroutine
                    (dialogues[lineCount].tf_Target, dialogues[lineCount].spriteName[contextCount].Split(new char[] { '/' })[0]));
            }
        }
    }

    void PlaySound()
    {
        if(dialogues[lineCount].VoiceName[contextCount] != "")
        {
            SoundManager.instance.PlaySound(dialogues[lineCount].VoiceName[contextCount], 2);
        }
    }

    IEnumerator TypeWriter()
    {
        SettingUI(true);
        ChangeSprite();
        PlaySound();

        string t_ReplaceText=dialogues[lineCount].contexts[contextCount];
        t_ReplaceText = t_ReplaceText.Replace("'", ",");
        t_ReplaceText = t_ReplaceText.Replace("\\n", "\n");

        bool t_white = false;
        bool t_yellow = false;
        bool t_cyan = false;
        bool t_ignore = false;

        for (int i = 0; i < t_ReplaceText.Length; i++)
        {
            switch (t_ReplaceText[i])
            {
                case 'ⓦ': t_white = true; t_yellow = false; t_cyan = false; t_ignore = true;
                    break;
                case 'ⓨ':
                    t_white = false; t_yellow = true; t_cyan = false; t_ignore = true;
                    break;
                case 'ⓒ':
                    t_white = false; t_yellow = false; t_cyan = true; t_ignore = true;
                    break;
                case '①': StartCoroutine(theSplashManager.Splash());
                    SoundManager.instance.PlaySound("Emotion1", 1);  t_ignore = true;
                    break;
                case '②': StartCoroutine(theSplashManager.Splash());
                    SoundManager.instance.PlaySound("Emotion2", 1); t_ignore = true;
                    break;
            }

            string t_letter = t_ReplaceText[i].ToString();

            if (!t_ignore)
            {
                if (t_white)
                {
                    t_letter = "<color=#FFFFFF>" + t_letter + "</color>";
                }
                else if(t_yellow)
                {
                    t_letter = "<color=#FFFF00>" + t_letter + "</color>";
                }else if (t_cyan)
                {
                    t_letter = "<color=#42DEE3>" + t_letter + "</color>";
                }

                txt_Dialogue.text += t_letter;
            }
            t_ignore = false;

            yield return new WaitForSeconds(textDelay);
        }
        
        isNext = true;
    }

    void SettingUI(bool p_flag)
    {
        go_DialogueBar.SetActive(p_flag);

        if (p_flag)
        {
            if (dialogues[lineCount].name == "")
            {
                go_DialogueNameBar.SetActive(false);
            }
            else
            {
                go_DialogueNameBar.SetActive(true);
                txt_Name.text = dialogues[lineCount].name;
            }
        }
        else
        {
            go_DialogueNameBar.SetActive(false);
        }
    }
}
