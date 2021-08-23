using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDoor : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] string locationName;

    public string GetSceneName()
    {
        return sceneName;
    }

    public string GetLocationName()
    {
        return locationName;
    }
}
