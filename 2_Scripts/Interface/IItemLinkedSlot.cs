using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���԰� ����� UI ��Ҹ� �����ϱ� ���� �������̽�.
/// �κ��丮�� ���� ���� UI�� ���� Ư�� �����۰� ����� ���Կ��� ����ϰ�
/// �������� ���� ����, ���� ��� �Բ� UI ������ ���� ȣ���. 
/// <para>��� �ż���</para>
/// </summary>
public interface IItemLinkedSlot 
{
    /// <summary>
    /// ����� �������� UI�� �����մϴ�.
    /// ����, ������, ���� ���� �ݿ��Ͽ� �ð������� ������Ʈ�� �� ����
    /// </summary>
    void RefreshPlayerUseItemUI();
    /// <summary>
    /// ������ ����, ����� ������ ������ UI ���¸� �ʱ�ȭ  
    /// </summary>
    void ClearSlot();
}
