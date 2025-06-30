using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// ������ �ؽ�Ʈ�� �����ϰ�, ���� ������ ǥ���ϸ�, ���� �ð� �� ������Ʈ Ǯ�� ��ȯ�ϴ� Ŭ����
/// 
/// <para>��� ����</para>
/// <para>public static DamageTextSpawner Instance</para>
/// <para>public string poolKey</para>
/// <para>public float damageTextDuration, (x, y, z)</para>
/// <para>private Camera playerCamera</para>
/// 
/// <para>��� �Լ�</para>
/// <para>public void ShowDamage(Vector3 pos, float damage, bool isGuardMode = false)</para>
/// <para>private IEnumerator ReturnAfterDelay(GameObject instance, float delay)</para>
/// </summary>
public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance { get; private set; }
    
    //public GameObject damageTextPrefabs;
    public string poolKey = "DamageText";
    //public Transform damageTextPos;
    public float damageTextDuration = 1f;
    public float x;
    public float y;
    public float z;
    [SerializeField] private Camera playerCamera;

    private StringBuilder sb = new StringBuilder();
   
    private void OnEnable()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
    }
    private void Awake()
    {
        /*if(Instance != null || Instance != this)
        {
            Destroy(gameObject);
            return;
        }*/
        Instance = this;        
    }
    /// <summary>
    /// ������ ��ġ�� ������ �ؽ�Ʈ�� ǥ���ϰ� �ִϸ��̼��� ����ϸ� ���� �ð� �� Ǯ�� ��ȯ
    /// </summary>
    /// <param name="pos">ǥ�� ��ġ</param>
    /// <param name="damage">������ ��ġ</param>
    /// <param name="isGuardMode">���� ����</param>
    public void ShowDamage(Vector3 pos, float damage, bool isGuardMode = false, bool isMiss = false)
    {
        if(ObjectPoolingManager.Instance == null)
        {
            Debug.Log("[DamageTextSpawner] ObjectPoolingManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            return;
        }
        Vector3 spawnPos = pos + new Vector3(x, y, z);
        GameObject instance = ObjectPoolingManager.Instance.SpawnFromPool(poolKey, spawnPos, Quaternion.identity);
        //ObjectPoolingManager.Instance.GetPool("DamageText");
        /*if (poolKey == null)
        {
            Debug.Log($"[DamageTextSpawner] ���� ��ϵ�{poolKey}�� �����ϴ�.");
        }
        if(ObjectPoolingManager.Instance.GetPool(poolKey) == null)
        {
            Debug.Log($"[DamageTextSpawner] ���� ��ϵ�{poolKey}�� �����ϴ�.");
        }*/
        if (instance == null)
        {
            Debug.Log("[DamageTextSpawner] ������Ʈ Ǯ���� DamageText�� �������� ���߽��ϴ�.");
            return;
        }
        Canvas canvas = instance.GetComponentInChildren<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null && playerCamera != null)
        {
            canvas.worldCamera = playerCamera;
            canvas.GetComponent<Canvas>().worldCamera = playerCamera;
        }
        instance.transform.position = spawnPos;
        instance.transform.forward = playerCamera.transform.forward;
        //instance.transform.LookAt(Camera.main.transform);
        //instance.transform.rotation = Quaternion.LookRotation(instance.transform.position - playerCamera.transform.position); //Camera.main
        //Vector3 lookDirection = instance.transform.position - playerCamera.transform.position;
        //lookDirection.y = 0; // ���� ȸ�� �����ؼ� �ؽ�Ʈ�� ���Ʒ��� ���� �ʰ�
        //Debug.Log($"[ShowDamage] ī�޶� ��ġ: {playerCamera.transform.position}");
        //Debug.Log($"[ShowDamage] ������ �ؽ�Ʈ ��ġ: {pos + new Vector3(x, y, z)}");
        //Debug.Log($"[ShowDamage] Camera.main: {Camera.main?.name}");
        //Debug.Log($"[ShowDamage] playerCamera: {playerCamera?.name}");
        //instance.transform.rotation = Quaternion.LookRotation(lookDirection);
        //instance.transform.forward = Camera.main.transform.forward;
        TextMeshProUGUI _text = instance.GetComponentInChildren<TextMeshProUGUI>();
        if (_text != null)
        {
            _text.rectTransform.localPosition = Vector3.zero;
            //_text.text = damage.ToString();
            //_text.text = Mathf.RoundToInt(damage).ToString();
            /*string displayText = isGuardMode ? $"(Guard) {Mathf.RoundToInt(damage)}" 
                : Mathf.RoundToInt(damage).ToString(); */

            sb.Clear();
            if (isMiss)
            {
                sb.Append("Miss");
            }
            else if (isGuardMode)
            {
                sb.Append("(Guard)" + Mathf.RoundToInt(damage));
            }
            else
            {
                sb.Append(Mathf.RoundToInt(damage));
            }
            _text.text = sb.ToString();
        }

        Animator animator = instance.GetComponent<Animator>();
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            if (animator.HasState(0, Animator.StringToHash("Damaged")))
            {
                animator.Play("Damaged", 0, 0);
            }
            else
            {
                Debug.LogWarning($"[DamageTextSpawner] 'Damaged' ���¸� ���� Animator Controller���� ã�� �� �����ϴ�: {animator.runtimeAnimatorController.name}");
            }
        }
        else
        {
            Debug.LogWarning("[DamageTextSpawner] Animator�� ���ų� Controller�� �Ҵ���� �ʾҽ��ϴ�.");
        }
        StartCoroutine(ReturnAfterDelay(instance, damageTextDuration));
        //ReturnAfterDelay(instance, damageTextDuration);
        //ObjectPoolingManager.Instance.ReturnToPool(poolKey, instance);
    }
    /// <summary>
    /// ���� �ð� �� ������ �ؽ�Ʈ ������Ʈ�� ������Ʈ Ǯ�� ��ȯ
    /// </summary>
    /// <param name="instance">��ȯ�� ������Ʈ</param>
    /// <param name="delay">��� �ð�</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator ReturnAfterDelay(GameObject instance, float delay)
    {
        //yield return new WaitForSeconds(delay);
        yield return WaitForSecondsCache.Get(delay);
        if (!instance.activeSelf || !instance.transform.parent) 
        {
            yield break;
        }
        if (ObjectPoolingManager.Instance != null)
        {
            ObjectPoolingManager.Instance.ReturnToPool(poolKey, instance);
        }
        else
        {
            Destroy(instance);
            Debug.Log("[DamageTextSpawner] ���� ObjectPoolingManager �ν��Ͻ��� �������� ����");
        }
    }
}
