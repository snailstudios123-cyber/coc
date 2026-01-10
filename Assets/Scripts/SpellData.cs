using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell System/Spell Data")]
public class SpellData : ScriptableObject
{
    public string spellName;
    [TextArea(3, 6)]
    public string description;
    public Sprite icon;
    public float manaCost;
    public bool isEquipped;
    
    [Header("Cast Type")]
    public bool isInstantCast = true;
}
