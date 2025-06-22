using UnityEngine;
using CustomInspector;
public class InspectorTutorial : MonoBehaviour
{
    [Title("인스펙터 꾸미기", underlined: true, fontSize = 15, alignment = TextAlignment.Center)]
    [HorizontalLine("세부속성"), HideField] public bool _l0;
    [Range(0, 5)] public int TestNum1;
    [Range(0.0f, 10.0f)] public float TestNum2;
    [RichText(true)] public string Teststring;
    [Space(15), ReadOnly(DisableStyle.OnlyText)] public string testReadOnly = "ReadyOnly 테스트";
    [Multiline(lines: 10)] public string teststring2;
    [TextArea(minLines: 2, maxLines: 10)] public string teststring3;
    [Preview(Size.big)] public Sprite sprite;
    [HorizontalLine(color: FixedColor.Red), HideField] public bool _l1;
    [Space(20), Button("Method", size = Size.big), HideField] public bool _b0;
    [Space(20), HorizontalLine(color: FixedColor.Red), HideField] public bool _s0;
    void Method()
    {
        Debug.Log($"Method");
    }
    [Space(20), Button("Method2", size = Size.big), HideField] public bool _b2;
    void Method2()
    {
        Debug.Log($"Method2");
    }
    [Space(15), Button("Method3", true)] public int intNum;
    void Method3(int n)
    {
        Debug.Log($"입력한 숫자{n}");
    }
}