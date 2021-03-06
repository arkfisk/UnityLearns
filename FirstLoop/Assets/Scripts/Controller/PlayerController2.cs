using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public static PlayerController2 instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float applySpeed;

    [SerializeField] float fieldSensitivity;
    [SerializeField] float fieldLookLimitX;

    [SerializeField] Transform tf_Crosshair;
    [SerializeField] Transform tf_Cam;
    [SerializeField] Vector2 camBoundary;
    [SerializeField] float sightMoveSpeed;
    [SerializeField] float sightSensivitity;
    [SerializeField] float lookLimitX;
    [SerializeField] float lookLimitY;

    float currentAngleX;
    float currentAngleY;

    [SerializeField] GameObject go_NotCamDown;
    [SerializeField] GameObject go_NotCamUp;
    [SerializeField] GameObject go_NotCamLeft;
    [SerializeField] GameObject go_NotCamRight;

    float originPosY;


    public void Reset()
    {
        tf_Crosshair.localPosition = Vector3.zero;
        currentAngleX = 0;
        currentAngleY = 0;
    }

    void Start()
    {
        originPosY = tf_Cam.localPosition.y;
    }

    void Update()
    {
        if (!InteractionController.isInteract)
        {
            if (CameraController.onlyView)
            {
                CrosshairMoving();
                ViewMoving();
                KeyViewMoving();
                CameraLimit();
                NotCamUI();
            }
            else
            {
                FieldMoving();
                FieldLooking();
            }
        }

    }

    void FieldMoving()
    {
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            Vector3 t_Dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                applySpeed = runSpeed;
            }
            else
            {
                applySpeed = walkSpeed;
            }

            transform.Translate(t_Dir * applySpeed * Time.deltaTime, Space.Self);
        }
    }

    void FieldLooking()
    {
        if(Input.GetAxisRaw("Mouse X") != 0)
        {
            float t_AngleY = Input.GetAxisRaw("Mouse X");
            Vector3 t_Rot = new Vector3(0, t_AngleY * fieldSensitivity, 0);
            transform.rotation = Quaternion.Euler(transform.localEulerAngles + t_Rot);
        }
        if(Input.GetAxisRaw("Mouse Y") != 0)
        {
            float t_AngleX = Input.GetAxisRaw("Mouse Y");
            currentAngleX -= t_AngleX;  //y?? ???? ?????? ????
            currentAngleX = Mathf.Clamp(currentAngleX, -fieldLookLimitX, fieldLookLimitX);
            tf_Cam.localEulerAngles = new Vector3(currentAngleX, 0, 0);
        }
    }

    void NotCamUI()
    {
        go_NotCamDown.SetActive(false);
        go_NotCamUp.SetActive(false);
        go_NotCamLeft.SetActive(false);
        go_NotCamRight.SetActive(false);

        if (currentAngleY >= lookLimitX)
            go_NotCamRight.SetActive(true);
        else if (currentAngleY <= -lookLimitX)
            go_NotCamLeft.SetActive(true);

        if (currentAngleX <= -lookLimitY)
            go_NotCamUp.SetActive(true);
        else if (currentAngleX >= lookLimitY)
            go_NotCamDown.SetActive(true);
    }

    void CameraLimit()
    {
        if (tf_Cam.localPosition.x >= camBoundary.x)
            tf_Cam.localPosition = new Vector3(camBoundary.x, tf_Cam.localPosition.y, tf_Cam.localPosition.z);
        else if (tf_Cam.localPosition.x <= -camBoundary.x)
            tf_Cam.localPosition = new Vector3(-camBoundary.x, tf_Cam.localPosition.y, tf_Cam.localPosition.z);

        if (tf_Cam.localPosition.y >= 1+camBoundary.y)
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, originPosY + camBoundary.y, tf_Cam.localPosition.z);
        else if (tf_Cam.localPosition.y <= 1 - camBoundary.y)
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, originPosY - camBoundary.y, tf_Cam.localPosition.z);
    }

    void KeyViewMoving()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            currentAngleY += sightSensivitity * Input.GetAxisRaw("Horizontal");
            currentAngleY = Mathf.Clamp(currentAngleY, -lookLimitX, lookLimitX);

            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + sightMoveSpeed * Input.GetAxisRaw("Horizontal"),tf_Cam.localPosition.y,tf_Cam.localPosition.z);
        }
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            currentAngleX += sightSensivitity * -Input.GetAxisRaw("Vertical");
            currentAngleX = Mathf.Clamp(currentAngleX, -lookLimitY, lookLimitY);

            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, tf_Cam.localPosition.y + sightMoveSpeed * Input.GetAxisRaw("Vertical"), tf_Cam.localPosition.z);
        }

        tf_Cam.localEulerAngles = new Vector3(currentAngleX, currentAngleY, tf_Cam.localEulerAngles.z);
    }

    void CrosshairMoving()
    {
        tf_Crosshair.localPosition = new Vector2(Input.mousePosition.x - (Screen.width / 2),
                                                 Input.mousePosition.y-(Screen.height/2));
        float t_cursorPosX = tf_Crosshair.localPosition.x;
        float t_cursorPosY = tf_Crosshair.localPosition.y;

        t_cursorPosX = Mathf.Clamp(t_cursorPosX, (-Screen.width / 2 + 50), (Screen.width / 2 - 50));
        t_cursorPosY = Mathf.Clamp(t_cursorPosY, (-Screen.height / 2 + 50), (Screen.height / 2 - 50));

        tf_Crosshair.localPosition = new Vector2(t_cursorPosX, t_cursorPosY);
    }

    void ViewMoving()
    {
        if(tf_Crosshair.localPosition.x > (Screen.width / 2-100) || tf_Crosshair.localPosition.x < (-Screen.width / 2 + 100))
        {
            currentAngleY += (tf_Crosshair.localPosition.x > 0) ? sightSensivitity : -sightSensivitity;
            currentAngleY = Mathf.Clamp(currentAngleY, -lookLimitX, lookLimitX);

            float t_applySpeed = (tf_Crosshair.localPosition.x > 0) ? sightMoveSpeed : -sightMoveSpeed;
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + t_applySpeed, tf_Cam.localPosition.y, tf_Cam.localPosition.z);
        }

        if (tf_Crosshair.localPosition.y > (Screen.height / 2 - 100) || tf_Crosshair.localPosition.y < (-Screen.height / 2 + 100))
        {
            currentAngleX += (tf_Crosshair.localPosition.y > 0) ? -sightSensivitity : sightSensivitity;
            currentAngleX = Mathf.Clamp(currentAngleX, -lookLimitY, lookLimitY);

            float t_applySpeed = (tf_Crosshair.localPosition.y > 0) ? sightMoveSpeed : -sightMoveSpeed;
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, tf_Cam.localPosition.y + t_applySpeed, tf_Cam.localPosition.z);
        }

        tf_Cam.localEulerAngles = new Vector3(currentAngleX, currentAngleY, tf_Cam.localEulerAngles.z);
    }
}
