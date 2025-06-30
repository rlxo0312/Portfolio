using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

/// <summary>
/// NPC 이름을 UI에 표시하는 클래스.
/// TextMeshProUGUI를 통해 NPC의 이름을 텍스트로 설정
/// <para>사용 변수 : private TextMeshProUGUI text</para>
/// </summary>
public class NPCUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    //[SerializeField] private Image questIcon;

    public void SetName(string name)
    {
        Debug.Log($"SetName 호출됨: {name}");
        if (text != null)
        {
            text.text = name;
        }
        else
        {
            Debug.LogError("text is NULL!! NPCName 오브젝트에 Text 컴포넌트가 연결되지 않음");
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
