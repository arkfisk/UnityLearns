using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CameraType
{
    ObjectFront,
    Reset,
    FadeOut,
    FadeIn,
    FlashOut,
    FlashIn,
    ShowCutScene,
    HideCutScene,
    AppearSlideCG,
    DisappearSlideCG,
    ChangeSlideCG,
}

public enum AppearType
{
    None,
    Appear,
    Disappear,
}

[System.Serializable]   //�ν����� â���� Ŀ������ ������ ���
public class Dialogue
{
    [Header("ī�޶� Ÿ������ ���")]
    public CameraType cameraType;
    public Transform tf_Target;

    [HideInInspector]
    public string name;

    [HideInInspector]
    public string[] contexts;

    [HideInInspector]
    public string[] spriteName;

    [HideInInspector]
    public string[] VoiceName;
}

[System.Serializable]
public class DialogueEvent
{
    public string name;

    public Vector2 line;
    public Dialogue[] dialogues;

    [Space]
    public AppearType appearType;
    public GameObject[] go_Target;

}
