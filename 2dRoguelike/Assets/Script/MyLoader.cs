using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLoader : MonoBehaviour
{
    public GameObject gameManager;

    private void Awake()
    {
        if (MyGameManager.instance == null)
        {
            Instantiate(gameManager);
        }
    }
}
