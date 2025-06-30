using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �浹 üũ�� ���� Ŭ����
/// ������ Box ���� ���� �ִ� �ݶ��̴��� �����Ͽ� ��ȯ�ϰ�, �����Ϳ��� ������ �ð�ȭ��
///
/// <para>��� ����</para>
/// <para>public Vector3 boxSize</para>
/// <para>public LayerMask layermask</para>
///
/// <para>�޼���</para>
/// <para>public Collider[] GetColliderObject()</para>
/// </summary>
public class ManualCollider : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(2, 2, 2);
    public Vector3 boxCentor = Vector3.zero;
    public LayerMask layermask;

    /// <summary>
    /// ������ �ڽ� ���� �ȿ� �ִ� �ݶ��̴��� ��ȯ��
    /// </summary>
    public Collider[] GetColliderObject()
    {
        Vector3 colliderCentor = transform.TransformPoint(boxCentor);
        //Debug.Log($"[ManualCollider] OverlapBox at {boxCentor}, size: {boxSize}");
        Collider[] colliders = Physics.OverlapBox(colliderCentor, boxSize / 2, transform.rotation, layermask); //transform.position
        //Debug.Log($"[ManualCollider] ������ �ݶ��̴� ��: {colliders.Length}");
        return colliders;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCentor, boxSize);
    }
}
