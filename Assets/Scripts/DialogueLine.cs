using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;
    public string speakerName;
    public Sprite speakerPortrait;
    public float typingSpeed = 0.05f;
}
