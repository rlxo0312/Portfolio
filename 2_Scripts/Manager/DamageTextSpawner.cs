using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// 데미지 텍스트를 생성하고, 월드 공간에 표시하며, 일정 시간 후 오브젝트 풀로 반환하는 클래스
/// 
/// <para>사용 변수</para>
/// <para>public static DamageTextSpawner Instance</para>
/// <para>public string poolKey</para>
/// <para>public float damageTextDuration, (x, y, z)</para>
/// <para>private Camera playerCamera</para>
/// 
/// <para>사용 함수</para>
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
    /// 지정된 위치에 데미지 텍스트를 표시하고 애니메이션을 재생하며 일정 시간 후 풀에 반환
    /// </summary>
    /// <param name="pos">표시 위치</param>
    /// <param name="damage">데미지 수치</param>
    /// <param name="isGuardMode">가드 여부</param>
    public void ShowDamage(Vector3 pos, float damage, bool isGuardMode = false, bool isMiss = false)
    {
        if(ObjectPoolingManager.Instance == null)
        {
            Debug.Log("[DamageTextSpawner] ObjectPoolingManager 인스턴스를 찾을 수 없습니다.");
            return;
        }
        Vector3 spawnPos = pos + new Vector3(x, y, z);
        GameObject instance = ObjectPoolingManager.Instance.SpawnFromPool(poolKey, spawnPos, Quaternion.identity);
        //ObjectPoolingManager.Instance.GetPool("DamageText");
        /*if (poolKey == null)
        {
            Debug.Log($"[DamageTextSpawner] 현재 등록된{poolKey}가 없습니다.");
        }
        if(ObjectPoolingManager.Instance.GetPool(poolKey) == null)
        {
            Debug.Log($"[DamageTextSpawner] 현재 등록된{poolKey}가 없습니다.");
        }*/
        if (instance == null)
        {
            Debug.Log("[DamageTextSpawner] 오브젝트 풀에서 DamageText를 가져오지 못했습니다.");
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
        //lookDirection.y = 0; // 수직 회전 제거해서 텍스트가 위아래로 휘지 않게
        //Debug.Log($"[ShowDamage] 카메라 위치: {playerCamera.transform.position}");
        //Debug.Log($"[ShowDamage] 데미지 텍스트 위치: {pos + new Vector3(x, y, z)}");
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
                Debug.LogWarning($"[DamageTextSpawner] 'Damaged' 상태를 현재 Animator Controller에서 찾을 수 없습니다: {animator.runtimeAnimatorController.name}");
            }
        }
        else
        {
            Debug.LogWarning("[DamageTextSpawner] Animator가 없거나 Controller가 할당되지 않았습니다.");
        }
        StartCoroutine(ReturnAfterDelay(instance, damageTextDuration));
        //ReturnAfterDelay(instance, damageTextDuration);
        //ObjectPoolingManager.Instance.ReturnToPool(poolKey, instance);
    }
    /// <summary>
    /// 일정 시간 후 데미지 텍스트 오브젝트를 오브젝트 풀로 반환
    /// </summary>
    /// <param name="instance">반환할 오브젝트</param>
    /// <param name="delay">대기 시간</param>
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
            Debug.Log("[DamageTextSpawner] 현재 ObjectPoolingManager 인스턴스가 존재하지 않음");
        }
    }
}
