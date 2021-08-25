using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] bool isAutoEvent = false;
    [SerializeField] DialogueEvent[] dialogueEvent;
    int currentCount;


    private void Start()
    {
        bool t_Flag = CheckEvent();

        gameObject.SetActive(t_Flag);
    }

    bool CheckEvent()
    {
        bool t_Flag = true;

        for (int x = 0; x < dialogueEvent.Length; x++)
        {
            t_Flag = true;

            //등장 조건과 일치하지 않을 경우, 등장시키지 않음
            for (int i = 0; i < dialogueEvent[x].eventTiming.eventConditions.Length; i++)
            {
                if (DatabaseManager.instance.eventFlags[dialogueEvent[x].eventTiming.eventConditions[i]] != dialogueEvent[x].eventTiming.conditionFlag)
                {
                    t_Flag = false;
                    break;
                }
            }
            //등장 조건과 관계 없이, 퇴장 조건과 일치할 겨우, 무조건 등장시키지 않음
            if (DatabaseManager.instance.eventFlags[dialogueEvent[x].eventTiming.eventEndNum])
            {
                t_Flag = false;
            }

            if (t_Flag)
            {
                currentCount = x;
                break;
            }
        }

        return t_Flag;
    }

    public Dialogue[] GetDialogue()
    {
        if (DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventEndNum])
        {
            return null;
        }

        //상호작용 전 대화
        if (!DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum] || dialogueEvent[currentCount].isSame)
        {
            DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum] = true;
            dialogueEvent[currentCount].dialogues = SettingDialogue(dialogueEvent[currentCount].dialogues, (int)dialogueEvent[currentCount].line.x, (int)dialogueEvent[currentCount].line.y);

            return dialogueEvent[currentCount].dialogues;
        }
        //상호작용 후 대화
        else
        {
            dialogueEvent[currentCount].dialoguesB = SettingDialogue(dialogueEvent[currentCount].dialoguesB, (int)dialogueEvent[currentCount].lineB.x, (int)dialogueEvent[currentCount].lineB.y);

            return dialogueEvent[currentCount].dialoguesB;
        }
    }

    Dialogue[] SettingDialogue(Dialogue[] p_Dialogue, int p_lineX, int p_lineY)
    {
        Dialogue[] t_Dialogues = DatabaseManager.instance.GetDialogue(p_lineX, p_lineY);

        for (int i = 0; i < dialogueEvent[currentCount].dialogues.Length; i++)
        {
            t_Dialogues[i].tf_Target = p_Dialogue[i].tf_Target;
            t_Dialogues[i].cameraType = p_Dialogue[i].cameraType;
        }
        return t_Dialogues;
    }

    public AppearType GetAppearType()
    {
        return dialogueEvent[currentCount].appearType;
    }

    public GameObject[] GetTargets()
    {
        return dialogueEvent[currentCount].go_Target;
    }

    public GameObject GetNextEvent()
    {
        return dialogueEvent[currentCount].go_NextEvent;
    }

    public int GetEventNumber()
    {
        CheckEvent();
        return dialogueEvent[currentCount].eventTiming.eventNum;
    }

    private void Update()
    {
        if(isAutoEvent && DatabaseManager.isFinish)
        {
            if (isAutoEvent && DatabaseManager.isFinish && TransferManager.isFinished)
            {
                DialogueManager theDM = FindObjectOfType<DialogueManager>();
                DialogueManager.isWaiting = true;

                if (GetAppearType() == AppearType.Appear) theDM.SetAppearObjects(GetTargets());
                else if (GetAppearType() == AppearType.Disappear) theDM.SetDisappearObjects(GetTargets());
                theDM.SetNextEvent(GetNextEvent());
                theDM.ShowDialogue(GetDialogue());

                gameObject.SetActive(false);
            }
        }
    }
}
