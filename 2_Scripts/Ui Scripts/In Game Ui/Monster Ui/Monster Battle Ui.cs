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
/// ���� ���� UI�� �����ϴ� Ŭ����.
/// ü�� �ٿ� �̸� �ؽ�Ʈ�� ǥ���ϰ� �����ϴ� ������ ��
/// <para>��� ����</para>
/// <para>private UnityEngine.UI.Slider slider, TextMeshProUGUI text, Transform debuffIconRoot,  List&lt;DebuffIconData&gt; debuffList
/// Dictionary&lt;DebuffType, DebuffIconData&gt; debuffDictionary, List&lt;Transform&gt; debuffIconSlots,  HashSet&lt;GameObject&gt; activeDebuffIcons
/// ,Dictionary&lt;GameObject, DebuffRuntimeInfo&gt; debuffRunTImeDictionary</para>
/// <para>public Vector2 iconsize </para>
/// <para>public float blinkStartTIme, blinkInterval</para>
/// <para>public enum attack, defense</para>
/// <para>��� �ż���</para>
/// <para>public void SetName(string name), public void ShowDebuff(DebuffType type, float duration), public void ClearAllDebuffs()</para>
/// </summary>
public class MonsterBattleUi : MonoBehaviour
{
    [Header("Ui ������Ʈ")]
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    [Header("����� ������ ��ġ �� ����")]
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
    /// ü�� ���� �ּҰ��� �����ϰų� ������
    /// </summary>
    public float MinValue
    {
        get => slider.minValue;
        set => slider.minValue = value;
    }
    /// <summary>
    /// ü�� ���� �ִ밪�� �����ϰų� ������
    /// </summary>
    public float MaxValue
    {
        get => slider.maxValue;
        set => slider.maxValue = value;
    }
    /// <summary>
    /// ���� ü�� ���� �����ϰų� ������
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
                Debug.LogWarning($"[MonsterBattleUi]�ߺ� ����� ���� ����: {data.debuffType}");
            }
        }
    }


    // Start is called before the first frame update
    private void Start()
    {

    }
    /// <summary>
    /// ���� �̸� �ؽ�Ʈ�� ����
    /// </summary>
    /// <param name="name">ǥ���� �̸� ���ڿ�</param>
    public void SetName(string name)
    {
        Debug.Log($"SetName ȣ���: {name}");
        if (text != null)
        {
            text.text = name;
        }
        else
        {
            Debug.LogError("text is NULL!! MonsterName ������Ʈ�� Text ������Ʈ�� ������� ����");
        }
    }
    /// <summary>
    /// ������ ����� �������� ������
    /// </summary>
    /// <param name="type">����� Ÿ��</param>
    /// <param name="duration">����� ���� �ð�</param>
    public void ShowDebuff(DebuffType type, float duration)
    {
        Debug.Log("[MonsterBattleUi] ShowDebuff ȣ���");
        if (!debuffDictionary.TryGetValue(type, out var data))
        {
            Debug.Log($"[MonsterBattleUi]����� Ÿ���� ã�� ������ - {type}");
            return;
        }

        if (activeDebuffIcons.Count >= debuffIconSlots.Count)
        {
            Debug.LogWarning("[MonsterBattleUi] ����� ������ ���� á��.");
            return;
        }

        int slotIndex = activeDebuffIcons.Count;
        Transform targetSlot = debuffIconSlots[slotIndex];

        GameObject icon = ObjectPoolingManager.Instance.SpawnFromPool
            (data.poolKey, Vector3.zero, Quaternion.identity); //targetSlot.transform
        Debug.Log($"[ShowDebuff] ������ ����� ������Ʈ �̸�: {icon.name}");
        //icon.transform.SetParent(debuffIconRoot, false);
        targetSlot.gameObject.SetActive(true);
        if (icon.TryGetComponent(out RectTransform rect))
        {
            rect.SetParent(targetSlot, false); // �θ� ����
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.sizeDelta = iconsize; // �ʿ� �� ����
        }

        /*Image image = icon.GetComponent<Image>();
        if (image == null)
        {
            image = icon.GetComponentInChildren<Image>(); 
        }*/
        DebuffIconHolder holder = icon.GetComponent<DebuffIconHolder>();
        if (holder == null || holder.image == null)
        {
            Debug.LogWarning($"[ShowDebuff] DebuffIconHolder �Ǵ� image�� �����ϴ�: {icon.name}");
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
            Debug.LogWarning($"[MonsterBattleUi] Image ������Ʈ�� ã�� ���߽��ϴ�: {icon.name}");
            return; 
        }

        if (activeDebuffIcons.Contains(icon))
        {
            Debug.LogWarning($"[ShowDebuff] �̹� ��ϵ� ����� ������: {icon.name}");
            return;
        }
        activeDebuffIcons.Add(icon);
        Debug.Log($"[ShowDebuff] icon transform: {icon.transform}, parent: {icon.transform.parent.name}");

        Coroutine blink = StartCoroutine(DebuffTick(icon, duration));
        //blinkCoroutine.Add(icon, blink);
        debuffRunTImeDictionary[icon].blinkCoroutine = blink;
    }
    /// <summary>
    /// ��� ����� �������� �����ϰ� ��ȯ��
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
            Debug.Log($"[ClearAllDebuffs] ��ȯ ��� ������ �̸�: {icon.name}");
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
                Debug.Log($"[ClearAllDebuffs] �� ��� prefab �̸�: {keyValueArray[j].Value.prefab.name}");
                if (icon.name.Contains(keyValueArray[j].Value.prefab.name))
                {
                    Debug.Log($"[ClearAllDebuffs] Ǯ�� ��ȯ��: {icon.name}");
                    ObjectPoolingManager.Instance.ReturnToPool(keyValueArray[j].Value.poolKey, icon);
                    break;
                }
            }
        }

        activeDebuffIcons.Clear();
        debuffRunTImeDictionary.Clear();
    }
    /// <summary>
    /// ����� ���� �ð� �� ������ ȿ���� �ο��ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="iconObject">����� ������ GameObject</param>
    /// <param name="duration">��ü ����� ���� �ð�</param>
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
    /// �̹����� �����̴� �ڷ�ƾ
    /// </summary>
    /// <param name="img">Image ������Ʈ</param>
    /// <param name="interval">������ ����</param>
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
