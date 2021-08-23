using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Location
{
    public string name;
    public Transform tf_Spawn;
}

public class TransferSpawnManager : MonoBehaviour
{
    [SerializeField] Location[] locations;
    Dictionary<string, Transform> locationDic = new Dictionary<string, Transform>();

    public static bool spawnTiming = false;


    void Start()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locationDic.Add(locations[i].name, locations[i].tf_Spawn);
        }

        if (spawnTiming)
        {
            TransferManager theTM = FindObjectOfType<TransferManager>();
            string t_LocationName = theTM.GetLocationName();
            Transform t_Spawn = locationDic[t_LocationName];
            PlayerController2.instance.transform.position = t_Spawn.position;
            PlayerController2.instance.transform.rotation = t_Spawn.rotation;
            Camera.main.transform.localPosition = new Vector3(0, 1, 0);
            Camera.main.transform.localEulerAngles = Vector3.zero;
            PlayerController2.instance.Reset();
            spawnTiming = false;

            StartCoroutine(theTM.Done());
        }
    }
}
