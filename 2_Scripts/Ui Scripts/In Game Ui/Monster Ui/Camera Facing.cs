using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ʈ�� ī�޶� �ٶ󺸵��� ȸ�� ������ �����ϴ� Ŭ�����Դϴ�.
/// ������ �� �������� LookAt ó���ϸ�, UI ��ҳ� ���� ���� �ؽ�Ʈ � ���˴ϴ�.
/// <para>��� ����: public Camera refCamera, public bool reverseFace, public Axis axis</para>
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
    /// ������ Axis ������ ���� Vector3�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="axis">��ȯ�� ��</param>
    /// <returns>�࿡ �����Ǵ� Vector3</returns>
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
    /// ������Ʈ�� ������ ī�޶� ���� ȸ���ϵ��� ó���մϴ�.
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
