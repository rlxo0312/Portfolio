using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �÷��̾��� ī�޶� �������� �����ϴ� Ŭ���� 
/// </summary>
public class PlayerCameraMove : MonoBehaviour
{
    [Header("ī�޶� ���� ����")]
    [SerializeField] private Transform Cameratarget;
    [SerializeField] private float cameraZDistance;
    [SerializeField] private float cameraYDistance;
    [SerializeField] private float cameraXDistance;
    [SerializeField] private float cameraRotateSpeed;
    [SerializeField] private float cameraLimitAngle;
    [SerializeField] private bool inverseX;             // ���콺 ���Ʒ� ���� üũ
    [SerializeField] private bool inverseY;             // ���콺 �¿�   ���� üũ
    

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
        //Debug.Log($"�÷��̾� ��ġ {Cameratarget.position}");
        transform.position = cameraFocusPosition - (targetRotation * new Vector3(cameraXDistance, cameraYDistance, cameraZDistance));
        //Debug.Log($"ī�޶� ��ġ: {transform.position}"); 
        
    }
}
