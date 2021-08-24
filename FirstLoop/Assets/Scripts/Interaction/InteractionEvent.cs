using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] bool isAutoEvent = false;
    [SerializeField] DialogueEvent dialogueEvent;


    private void Start()
    {
        bool t_Flag = CheckEvent();

        gameObject.SetActive(t_Flag);
    }

    bool CheckEvent()
    {
        bool t_Flag = true;
        //등장 조건과 일치하지 않을 경우, 등장시키지 않음
        for (int i = 0; i < dialogueEvent.eventTiming.eventConditions.Length; i++)
        {
            if (DatabaseManager.instance.eventFlags[dialogueEvent.eventTiming.eventConditions[i]] != dialogueEvent.eventTiming.conditionFlag)
            {
                t_Flag = false;
                break;
            }
        }
        //등장 조건과 관계 없이, 퇴장 조건과 일치할 겨우, 무조건 등장시키지 않음
        if (DatabaseManager.instance.eventFlags[dialogueEvent.eventTiming.eventEndNum])
        {
            t_Flag = false;
        }

        return t_Flag;
    }

    public Dialogue[] GetDialogue()
    {
        DatabaseManager.instance.eventFlags[dialogueEvent.eventTiming.eventNum] = true;
        DialogueEvent t_DialogueEvent = new DialogueEvent();
        t_DialogueEvent.dialogues = DatabaseManager.instance.GetDialogue((int)dialogueEvent.line.x, (int)dialogueEvent.line.y);

        for (int i = 0; i < dialogueEvent.dialogues.Length; i++)
        {
            t_DialogueEvent.dialogues[i].tf_Target = dialogueEvent.dialogues[i].tf_Target;
            t_DialogueEvent.dialogues[i].cameraType = dialogueEvent.dialogues[i].cameraType;
        }
        dialogueEvent.dialogues = t_DialogueEvent.dialogues;

        return dialogueEvent.dialogues;
    }

    public AppearType GetAppearType()
    {
        return dialogueEvent.appearType;
    }

    public GameObject[] GetTargets()
    {
        return dialogueEvent.go_Target;
    }

    public GameObject GetNextEvent()
    {
        return dialogueEvent.go_NextEvent;
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
