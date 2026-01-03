using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;

    [SerializeField] private List<SpellData> allSpells = new List<SpellData>();
    [SerializeField] private int maxEquippedSpells = 3;

    private List<SpellData> equippedSpells = new List<SpellData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log($"[SpellManager] Destroying duplicate SpellManager. Keeping persistent instance.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadEquippedSpells();
    }

    private void LoadEquippedSpells()
    {
        equippedSpells.Clear();
        foreach (SpellData spell in allSpells)
        {
            if (spell.isEquipped)
            {
                equippedSpells.Add(spell);
            }
        }
    }

    public List<SpellData> GetAllSpells()
    {
        return allSpells;
    }

    public List<SpellData> GetEquippedSpells()
    {
        return equippedSpells;
    }

    public bool CanEquipMoreSpells()
    {
        return equippedSpells.Count < maxEquippedSpells;
    }

    public bool EquipSpell(SpellData spell)
    {
        if (spell.isEquipped || !CanEquipMoreSpells())
        {
            return false;
        }

        spell.isEquipped = true;
        equippedSpells.Add(spell);
        return true;
    }

    public void AddSpell(SpellData spell)
    {
        if (spell == null)
        {
            Debug.LogWarning("[SpellManager] Attempted to add null spell");
            return;
        }

        if (allSpells.Contains(spell))
        {
            Debug.LogWarning($"[SpellManager] Spell '{spell.spellName}' already in allSpells list");
            return;
        }

        allSpells.Add(spell);
        Debug.Log($"[SpellManager] Added spell '{spell.spellName}' to allSpells");
    }

    public void LearnAndEquipSpell(SpellData spell)
    {
        if (spell == null)
        {
            Debug.LogError("[SpellManager] Cannot learn null spell");
            return;
        }

        if (!allSpells.Contains(spell))
        {
            AddSpell(spell);
        }

        bool equipped = EquipSpell(spell);
        if (equipped)
        {
            Debug.Log($"[SpellManager] ✓ Learned and equipped '{spell.spellName}'");
        }
        else
        {
            Debug.LogWarning($"[SpellManager] Learned '{spell.spellName}' but could not equip (already equipped or slots full)");
        }
    }

    public void UnequipSpell(SpellData spell)
    {
        if (!spell.isEquipped)
        {
            return;
        }

        spell.isEquipped = false;
        equippedSpells.Remove(spell);
    }

    public bool IsSpellEquipped(SpellData spell)
    {
        return spell.isEquipped;
    }
}
