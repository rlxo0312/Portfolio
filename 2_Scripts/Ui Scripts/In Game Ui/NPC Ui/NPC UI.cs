using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

/// <summary>
/// NPC �̸��� UI�� ǥ���ϴ� Ŭ����.
/// TextMeshProUGUI�� ���� NPC�� �̸��� �ؽ�Ʈ�� ����
/// <para>��� ���� : private TextMeshProUGUI text</para>
/// </summary>
public class NPCUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    //[SerializeField] private Image questIcon;

    public void SetName(string name)
    {
        Debug.Log($"SetName ȣ���: {name}");
        if (text != null)
        {
            text.text = name;
        }
        else
        {
            Debug.LogError("text is NULL!! NPCName ������Ʈ�� Text ������Ʈ�� ������� ����");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
