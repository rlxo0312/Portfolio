using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 디버프 아이콘의 런타임 정보를 저장하는 클래스.
/// 이미지 컴포넌트와 깜빡임을 위한 코루틴 참조를 관리함
/// <para>사용 변수</para>
/// <para>public Image image, Coroutine blinkCoroutine </para>
/// </summary>
public class DebuffRuntimeInfo 
{
    public Image image;
    public Coroutine blinkCoroutine;
}
