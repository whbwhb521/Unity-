using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 跟随人物的相机
 */
public class PlayerCameraFollow : MonoBehaviour
{
    Camera mainCamera;                      //获取相机
    private Vector3 Player_LocalPos;        //玩家localposition
    private Vector3 Player_Pos;             //玩家position
    private Vector3 Camera_LocalPos;        //相机localposition
    private Vector3 Camera_Pos;             //相机position
    private float RotateLevel = 3.0f;       //旋转的力度
    private Vector3 CameraRotation;         //相机角度
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        CameraRotation = new Vector3(10,0,0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void LateUpdate()
    {
        CalculateCameraRotation();
        mainCamera.transform.eulerAngles = CameraRotation;
        CalculateCameraPos();
        mainCamera.transform.position = Camera_Pos;
    }
    private void FixedUpdate()
    {
    }

    void CalculateCameraPos() 
    {
        Player_Pos = this.transform.position;
        Player_LocalPos = this.transform.InverseTransformPoint(Player_Pos);
        Camera_LocalPos.x = Player_LocalPos.x + 0.2f;
        Camera_LocalPos.y = Player_LocalPos.y + 2.5f + 2.5f * Mathf.Sin(Mathf.Deg2Rad * CameraRotation.x);
        Camera_LocalPos.z = Player_LocalPos.z - 2.5f * Mathf.Cos(Mathf.Deg2Rad * CameraRotation.x);
        Camera_Pos = this.transform.TransformPoint(Camera_LocalPos);
    }

    void CalculateCameraRotation() 
    {
        float KeyMouseY = Input.GetAxis("Mouse Y");
        if (CameraRotation.x- (RotateLevel / 2.0f) * KeyMouseY <= 35 && CameraRotation.x-(RotateLevel / 2.0f) * KeyMouseY >= -35)
            CameraRotation.x -= (RotateLevel / 2.0f) * KeyMouseY;
        else if (CameraRotation.x - (RotateLevel / 2.0f) * KeyMouseY > 35)
            CameraRotation.x = 35;
        else if (CameraRotation.x - (RotateLevel / 2.0f) * KeyMouseY < -35)
            CameraRotation.x = -35;
        CameraRotation.y = this.transform.eulerAngles.y;
        CameraRotation.z = 0;
    }
}
