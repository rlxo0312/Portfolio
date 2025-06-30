using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 플레이어의 카메라 움직임을 제어하는 클래스 
/// </summary>
public class PlayerCameraMove : MonoBehaviour
{
    [Header("카메라 제어 변수")]
    [SerializeField] private Transform Cameratarget;
    [SerializeField] private float cameraZDistance;
    [SerializeField] private float cameraYDistance;
    [SerializeField] private float cameraXDistance;
    [SerializeField] private float cameraRotateSpeed;
    [SerializeField] private float cameraLimitAngle;
    [SerializeField] private bool inverseX;             // 마우스 위아래 반전 체크
    [SerializeField] private bool inverseY;             // 마우스 좌우   반전 체크
    

    float rotationX;
    float rotationY;

    public bool canCameraMove = true;
    public Quaternion cameraLookRotation => Quaternion.Euler(0, rotationY, 0); 
    
    // Update is called once per frame
    void Update()
    {
        if (!canCameraMove)
        {
            return;
        }
        float invertXValue = (inverseX) ? -1 : 1;
        float invertYValue = (inverseY) ? -1 : 1;

        rotationX -= Input.GetAxis("Mouse Y") * cameraRotateSpeed * invertYValue;
        rotationX = Mathf.Clamp(rotationX, -cameraLimitAngle, cameraLimitAngle);

        rotationY += Input.GetAxis("Mouse X") * cameraRotateSpeed * invertXValue;
                
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        transform.rotation = targetRotation; 
                
        Vector3 cameraFocusPosition = Cameratarget.position;
        //Debug.Log($"플레이어 위치 {Cameratarget.position}");
        transform.position = cameraFocusPosition - (targetRotation * new Vector3(cameraXDistance, cameraYDistance, cameraZDistance));
        //Debug.Log($"카메라 위치: {transform.position}"); 
        
    }
}
