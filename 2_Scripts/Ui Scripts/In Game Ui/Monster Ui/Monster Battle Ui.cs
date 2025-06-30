using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DebuffType
{
    attack,
    defense
}
/// <summary>
/// 몬스터 전투 UI를 제어하는 클래스.
/// 체력 바와 이름 텍스트를 표시하고 갱신하는 역할을 함
/// <para>사용 변수</para>
/// <para>private UnityEngine.UI.Slider slider, TextMeshProUGUI text, Transform debuffIconRoot,  List&lt;DebuffIconData&gt; debuffList
/// Dictionary&lt;DebuffType, DebuffIconData&gt; debuffDictionary, List&lt;Transform&gt; debuffIconSlots,  HashSet&lt;GameObject&gt; activeDebuffIcons
/// ,Dictionary&lt;GameObject, DebuffRuntimeInfo&gt; debuffRunTImeDictionary</para>
/// <para>public Vector2 iconsize </para>
/// <para>public float blinkStartTIme, blinkInterval</para>
/// <para>public enum attack, defense</para>
/// <para>사용 매서드</para>
/// <para>public void SetName(string name), public void ShowDebuff(DebuffType type, float duration), public void ClearAllDebuffs()</para>
/// </summary>
public class MonsterBattleUi : MonoBehaviour
{
    [Header("Ui 컴포넌트")]
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    [Header("디버프 아이콘 위치 및 슬롯")]
    [SerializeField] private Transform debuffIconRoot;
    [SerializeField] private List<DebuffIconData> debuffList;

    private Dictionary<DebuffType, DebuffIconData> debuffDictionary;
    [SerializeField] private List<Transform> debuffIconSlots = new List<Transform>();
    private HashSet<GameObject> activeDebuffIcons = new HashSet<GameObject>();
    private Dictionary<GameObject, DebuffRuntimeInfo> debuffRunTImeDictionary = new Dictionary<GameObject, DebuffRuntimeInfo>();

    public Vector2 iconsize;
    public float blinkStartTIme;
    public float blinkInterval;
    
    /// <summary>
    /// 체력 바의 최소값을 설정하거나 가져옴
    /// </summary>
    public float MinValue
    {
        get => slider.minValue;
        set => slider.minValue = value;
    }
    /// <summary>
    /// 체력 바의 최대값을 설정하거나 가져옴
    /// </summary>
    public float MaxValue
    {
        get => slider.maxValue;
        set => slider.maxValue = value;
    }
    /// <summary>
    /// 현재 체력 값을 설정하거나 가져옴
    /// </summary>
    public float Value
    {
        get => slider.value;
        set => slider.value = value;
    }
    private void Awake()
    {
        debuffDictionary = new Dictionary<DebuffType, DebuffIconData>();
        foreach (var data in debuffList)
        {
            if (!debuffDictionary.ContainsKey(data.debuffType))
            {
                debuffDictionary.Add(data.debuffType, data);
            }
            else
            {
                Debug.LogWarning($"[MonsterBattleUi]중복 디버프 유형 감지: {data.debuffType}");
            }
        }
    }


    // Start is called before the first frame update
    private void Start()
    {

    }
    /// <summary>
    /// 몬스터 이름 텍스트를 설정
    /// </summary>
    /// <param name="name">표시할 이름 문자열</param>
    public void SetName(string name)
    {
        Debug.Log($"SetName 호출됨: {name}");
        if (text != null)
        {
            text.text = name;
        }
        else
        {
            Debug.LogError("text is NULL!! MonsterName 오브젝트에 Text 컴포넌트가 연결되지 않음");
        }
    }
    /// <summary>
    /// 지정된 디버프 아이콘을 보여줌
    /// </summary>
    /// <param name="type">디버프 타입</param>
    /// <param name="duration">디버프 지속 시간</param>
    public void ShowDebuff(DebuffType type, float duration)
    {
        Debug.Log("[MonsterBattleUi] ShowDebuff 호출됨");
        if (!debuffDictionary.TryGetValue(type, out var data))
        {
            Debug.Log($"[MonsterBattleUi]디버프 타입을 찾지 못했음 - {type}");
            return;
        }

        if (activeDebuffIcons.Count >= debuffIconSlots.Count)
        {
            Debug.LogWarning("[MonsterBattleUi] 디버프 슬롯이 가득 찼음.");
            return;
        }

        int slotIndex = activeDebuffIcons.Count;
        Transform targetSlot = debuffIconSlots[slotIndex];

        GameObject icon = ObjectPoolingManager.Instance.SpawnFromPool
            (data.poolKey, Vector3.zero, Quaternion.identity); //targetSlot.transform
        Debug.Log($"[ShowDebuff] 생성된 디버프 오브젝트 이름: {icon.name}");
        //icon.transform.SetParent(debuffIconRoot, false);
        targetSlot.gameObject.SetActive(true);
        if (icon.TryGetComponent(out RectTransform rect))
        {
            rect.SetParent(targetSlot, false); // 부모 지정
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.sizeDelta = iconsize; // 필요 시 조절
        }

        /*Image image = icon.GetComponent<Image>();
        if (image == null)
        {
            image = icon.GetComponentInChildren<Image>(); 
        }*/
        DebuffIconHolder holder = icon.GetComponent<DebuffIconHolder>();
        if (holder == null || holder.image == null)
        {
            Debug.LogWarning($"[ShowDebuff] DebuffIconHolder 또는 image가 없습니다: {icon.name}");
            return;
        }

        Image image = holder.image;

        if (image != null)
        {
            image.sprite = data.iconSprite;
            image.enabled = true;

            debuffRunTImeDictionary[icon] = new DebuffRuntimeInfo
            {
                image = image
            };
        }
        else
        {
            Debug.LogWarning($"[MonsterBattleUi] Image 컴포넌트를 찾지 못했습니다: {icon.name}");
            return; 
        }

        if (activeDebuffIcons.Contains(icon))
        {
            Debug.LogWarning($"[ShowDebuff] 이미 등록된 디버프 아이콘: {icon.name}");
            return;
        }
        activeDebuffIcons.Add(icon);
        Debug.Log($"[ShowDebuff] icon transform: {icon.transform}, parent: {icon.transform.parent.name}");

        Coroutine blink = StartCoroutine(DebuffTick(icon, duration));
        //blinkCoroutine.Add(icon, blink);
        debuffRunTImeDictionary[icon].blinkCoroutine = blink;
    }
    /// <summary>
    /// 모든 디버프 아이콘을 제거하고 반환함
    /// </summary>
    public void ClearAllDebuffs()
    {
        KeyValuePair<DebuffType, DebuffIconData>[] keyValueArray = debuffDictionary.ToArray();
        var iconList = activeDebuffIcons.ToList();
        for (int i = 0; i < activeDebuffIcons.Count; i++)        
        {
            GameObject icon = iconList[i];

            

            if (icon == null)
            {
                continue;
            }
            Debug.Log($"[ClearAllDebuffs] 반환 대상 아이콘 이름: {icon.name}");
            if (debuffRunTImeDictionary.TryGetValue(icon, out var runtime))
            {
                if (runtime.blinkCoroutine != null)
                {
                    StopCoroutine(runtime.blinkCoroutine);
                }

                if (runtime.image != null)
                {
                    runtime.image.enabled = true;
                }
            }

            Transform slot = icon.transform.parent;
            if (slot != null && debuffIconSlots.Contains(slot))
            {
                slot.gameObject.SetActive(false);
            }

            for (int j = 0; j < keyValueArray.Length; j++)
            {
                Debug.Log($"[ClearAllDebuffs] 비교 대상 prefab 이름: {keyValueArray[j].Value.prefab.name}");
                if (icon.name.Contains(keyValueArray[j].Value.prefab.name))
                {
                    Debug.Log($"[ClearAllDebuffs] 풀로 반환됨: {icon.name}");
                    ObjectPoolingManager.Instance.ReturnToPool(keyValueArray[j].Value.poolKey, icon);
                    break;
                }
            }
        }

        activeDebuffIcons.Clear();
        debuffRunTImeDictionary.Clear();
    }
    /// <summary>
    /// 디버프 지속 시간 후 깜빡임 효과를 부여하는 코루틴
    /// </summary>
    /// <param name="iconObject">디버프 아이콘 GameObject</param>
    /// <param name="duration">전체 디버프 지속 시간</param>
    private IEnumerator DebuffTick(GameObject iconObject, float duration)
    {
        
        if (!debuffRunTImeDictionary.TryGetValue(iconObject, out var runtime))
        {
            yield break;
        }
        Image image = runtime.image;
       
        float waitBlink = duration - blinkStartTIme;
        if (waitBlink > 0f)
        {
            yield return WaitForSecondsCache.Get(waitBlink);
        }

        Coroutine blink = StartCoroutine(BlinkIcon(image, blinkInterval));
        runtime.blinkCoroutine = blink;

        yield return WaitForSecondsCache.Get(blinkStartTIme);

        if (image != null)
        {
            image.enabled = true;
        }
    }
    /// <summary>
    /// 이미지를 깜빡이는 코루틴
    /// </summary>
    /// <param name="img">Image 컴포넌트</param>
    /// <param name="interval">깜빡임 간격</param>
    private IEnumerator BlinkIcon(Image img, float interval)
    {
        while (true)
        {
            if (img != null)
            {
                img.enabled = !img.enabled;
            }
            yield return WaitForSecondsCache.Get(interval);
        }
    }
}
