using UnityEngine;

[CreateAssetMenu(fileName="SwordText", menuName="SwordTexts")]
public class SwordText : ScriptableObject
{
    [TextArea]
    public string text;
}
