using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); //리소스 폴더의 csv파일을 유니티로 가져옴

        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Dialogue dialogue = new Dialogue();

            dialogue.name = row[1];

            //Debug.Log(row[1]);
            List<string> contextList = new List<string>();
            List<string> spriteList = new List<string>();

            do
            {
                contextList.Add(row[2]); //대사 갯수만큼 반복문
                //Debug.Log(row[2]);
                spriteList.Add(row[3]);

                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            }
            while (row[0].ToString() == "");

            dialogue.contexts = contextList.ToArray(); //리스트를 배열로 바꿈
            dialogue.spriteName = spriteList.ToArray();
            dialogueList.Add(dialogue);
        }

        return dialogueList.ToArray();
    }
}
