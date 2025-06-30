using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 슬롯과 연결된 UI 요소를 관리하기 위한 인터페이스.
/// 인벤토리나 단축 슬롯 UI와 같이 특정 아이템과 연결된 슬롯에서 사용하고
/// 아이템의 수량 변경, 제거 등과 함께 UI 갱신을 위해 호출됨. 
/// <para>사용 매서드</para>
/// </summary>
public interface IItemLinkedSlot 
{
    /// <summary>
    /// 연결된 아이템의 UI를 갱신합니다.
    /// 수량, 아이콘, 상태 등을 반영하여 시각적으로 업데이트할 때 사용됨
    /// </summary>
    void RefreshPlayerUseItemUI();
    /// <summary>
    /// 슬롯을 비우고, 연결된 아이템 참조와 UI 상태를 초기화  
    /// </summary>
    void ClearSlot();
}
