using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 수동 충돌 체크를 위한 클래스
/// 지정된 Box 영역 내에 있는 콜라이더를 감지하여 반환하고, 에디터에서 영역을 시각화함
///
/// <para>사용 변수</para>
/// <para>public Vector3 boxSize</para>
/// <para>public LayerMask layermask</para>
///
/// <para>메서드</para>
/// <para>public Collider[] GetColliderObject()</para>
/// </summary>
public class ManualCollider : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(2, 2, 2);
    public Vector3 boxCentor = Vector3.zero;
    public LayerMask layermask;

    /// <summary>
    /// 지정된 박스 영역 안에 있는 콜라이더를 반환함
    /// </summary>
    public Collider[] GetColliderObject()
    {
        Vector3 colliderCentor = transform.TransformPoint(boxCentor);
        //Debug.Log($"[ManualCollider] OverlapBox at {boxCentor}, size: {boxSize}");
        Collider[] colliders = Physics.OverlapBox(colliderCentor, boxSize / 2, transform.rotation, layermask); //transform.position
        //Debug.Log($"[ManualCollider] 감지된 콜라이더 수: {colliders.Length}");
        return colliders;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCentor, boxSize);
    }
}
