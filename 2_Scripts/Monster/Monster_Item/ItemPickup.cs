using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ ��� Ŭ���� - �����ۿ� ���� Ŭ������ �����Ͽ� tag�� player�� gameobject�� ������ ������ ���
/// <para>��뺯��</para>
/// <para>private: Item_Data item_Data, itemKey, isPickup(bool), returnTime</para>
/// <para>�ż���</para>
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
    /// ������ ��ӽ� �ʱ⼳�� �ż���, Item_Data�ʿ�
    /// </summary>    
    /// <param name="data">������ ItemData</param>
    public void SetUp(Item_Data data)
    {
        if (data == null)
        {
            Debug.Log("[ItemPickup] SetUp�� ���޵� item_Data�� null�Դϴ�!");
            return;
        }
        item_Data = data;
        itemKey = data.itemKey;
        isPickup = false;
        //Debug.Log("[ItemPickup] item_Data ������: " + item_Data?.itemName);
        if(returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
        returnCoroutine = StartCoroutine(TimeToReturnPool(returnTime));
    }
    /// <summary>
    /// ���� �ð��� ������ �������� pool�� �ǵ���
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
    /// �÷��̾ �浹���� �� ������ ȹ�� �õ�
    /// </summary>
    /// <param name="other">�浹�� �ݶ��̴�</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryPickup();
        }
    }
    /// <summary>
    /// �÷��̾ �ݶ��̴��� �ӹ��� ���� ������ ȹ�� �õ�
    /// </summary>
    /// <param name="other">�浹�� �ݶ��̴�</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryPickup();
        }
    }
    /// <summary>
    /// �κ��丮�� �������� �߰��ϰ�, ���� �� ������Ʈ Ǯ�� ��ȯ
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
