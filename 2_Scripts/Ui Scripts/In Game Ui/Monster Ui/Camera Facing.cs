using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트가 카메라를 바라보도록 회전 방향을 설정하는 클래스입니다.
/// 지정된 축 기준으로 LookAt 처리하며, UI 요소나 월드 공간 텍스트 등에 사용됩니다.
/// <para>사용 변수: public Camera refCamera, public bool reverseFace, public Axis axis</para>
/// </summary>
public class CameraFacing : MonoBehaviour
{
    public enum Axis { up, down, right, left, forward, back }

    public Camera refCamera; 
    public bool reverseFace = false;     
    public Axis axis;

    public void Awake()
    {
        if(!refCamera)
        {
            refCamera = Camera.main;
        }
    }
    /// <summary>
    /// 지정된 Axis 열거형 값을 Vector3로 변환합니다.
    /// </summary>
    /// <param name="axis">변환할 축</param>
    /// <returns>축에 대응되는 Vector3</returns>
    public Vector3 GetAxis(Axis axis)
    {
        switch (axis) 
        {
            case Axis.down:
                return Vector3.down;
            case Axis.right:
                return Vector3.right;
            case Axis.left:
                return Vector3.left;
            case Axis.forward:
                return Vector3.forward;
            case Axis.back:
                return Vector3.back;
            default:
                return Vector3.up;
        }
    }
    /// <summary>
    /// 오브젝트가 지정된 카메라를 향해 회전하도록 처리합니다.
    /// </summary>
    private void LateUpdate()
    {
        if (refCamera == null)
        {
            refCamera = Camera.main;
            if (refCamera == null) return;
        }
        Vector3 targetPos = transform.position + 
                            refCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);

        Vector3 targetOrientation = refCamera.transform.rotation *GetAxis(axis);
        transform.LookAt(targetPos, targetOrientation);

    }
}
