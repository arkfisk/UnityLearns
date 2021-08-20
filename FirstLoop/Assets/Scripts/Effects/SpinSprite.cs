using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSprite : MonoBehaviour
{
    [SerializeField] Transform tf_Target;


    private void Update()
    {
        if (tf_Target != null)
        {
            Quaternion t_Rotation = Quaternion.LookRotation(tf_Target.position);
            Vector3 t_Euler = new Vector3(0, t_Rotation.eulerAngles.y, 0);
            transform.eulerAngles = t_Euler;
        }
    }
}
