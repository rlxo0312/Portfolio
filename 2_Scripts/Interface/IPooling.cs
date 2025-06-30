using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������Ʈ Ǯ�� �ý��ۿ��� �������� ����ϴ� �������̽�
/// Ǯ���� GameObject�� ��������, �ٽ� ��ȯ�ϴ� ����� ����
///
/// <para>��� �޼���</para>
/// <para>GameObject GetFromPool()</para>
/// <para>void ReturnToPool(GameObject obj)</para>
/// </summary>
public interface IPooling 
{
    GameObject GetFromPool();
    void ReturnToPool(GameObject obj);
}
