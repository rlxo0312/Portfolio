using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 아이템 드랍 클래스 - 아이템에 현재 클래스를 부착하여 tag가 player인 gameobject를 만나면 아이템 드롭
/// <para>사용변수</para>
/// <para>private: Item_Data item_Data, itemKey, isPickup(bool), returnTime</para>
/// <para>매서드</para>
/// <para>SetUp(void), OnTriggerEnter(void)</para>
/// </summary>
public class ItemPickup : MonoBehaviour
{
    private Item_Data item_Data;
    [HideInInspector] public string itemKey; 
    private bool isPickup;
    [SerializeField] private float returnTime;   
    private Coroutine returnCoroutine;
    /// <summary>
    /// 아이템 드롭시 초기설정 매서드, Item_Data필요
    /// </summary>    
    /// <param name="data">적용할 ItemData</param>
    public void SetUp(Item_Data data)
    {
        if (data == null)
        {
            Debug.Log("[ItemPickup] SetUp에 전달된 item_Data가 null입니다!");
            return;
        }
        item_Data = data;
        itemKey = data.itemKey;
        isPickup = false;
        //Debug.Log("[ItemPickup] item_Data 설정됨: " + item_Data?.itemName);
        if(returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
        returnCoroutine = StartCoroutine(TimeToReturnPool(returnTime));
    }
    /// <summary>
    /// 일정 시간이 지나면 아이템을 pool로 되돌림
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    private IEnumerator TimeToReturnPool(float timer)
    {
        //yield return new WaitForSeconds(timer);
        yield return WaitForSecondsCache.Get(timer);

        if (!isPickup)
        {
            ObjectPoolingManager.Instance.ReturnToPool(itemKey, gameObject);
        }
    }
    /// <summary>
    /// 플레이어가 충돌했을 때 아이템 획득 시도
    /// </summary>
    /// <param name="other">충돌한 콜라이더</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryPickup();
        }
    }
    /// <summary>
    /// 플레이어가 콜라이더에 머무를 때도 아이템 획득 시도
    /// </summary>
    /// <param name="other">충돌한 콜라이더</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryPickup();
        }
    }
    /// <summary>
    /// 인벤토리에 아이템을 추가하고, 성공 시 오브젝트 풀로 반환
    /// </summary>
    private void TryPickup()
    {
        if(isPickup || item_Data ==null || Inventory.instance == null)
        {
            return;
        }        

        bool result = Inventory.instance.AddItem(item_Data, this.gameObject);
        

        if (result)
        {
            isPickup = true;
            ObjectPoolingManager.Instance.ReturnToPool(itemKey, gameObject);
        }
    }
}
