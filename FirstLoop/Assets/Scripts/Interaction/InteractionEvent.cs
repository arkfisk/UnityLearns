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

            //���� ���ǰ� ��ġ���� ���� ���, �����Ű�� ����
            for (int i = 0; i < dialogueEvent[x].eventTiming.eventConditions.Length; i++)
            {
                if (DatabaseManager.instance.eventFlags[dialogueEvent[x].eventTiming.eventConditions[i]] != dialogueEvent[x].eventTiming.conditionFlag)
                {
                    t_Flag = false;
                    break;
                }
            }
            //���� ���ǰ� ���� ����, ���� ���ǰ� ��ġ�� �ܿ�, ������ �����Ű�� ����
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

        //��ȣ�ۿ� �� ��ȭ
        if (!DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum] || dialogueEvent[currentCount].isSame)
        {
            DatabaseManager.instance.eventFlags[dialogueEvent[currentCount].eventTiming.eventNum] = true;
            dialogueEvent[currentCount].dialogues = SettingDialogue(dialogueEvent[currentCount].dialogues, (int)dialogueEvent[currentCount].line.x, (int)dialogueEvent[currentCount].line.y);

            return dialogueEvent[currentCount].dialogues;
        }
        //��ȣ�ۿ� �� ��ȭ
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
