using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Race
{
    HUMAN,
    ORC,
    DWARF,
    TROLL,
    GNOME,
    ///
    HIGHELF,
    DARKELF,
    BLOODELF,
    ///
    LIKIN,
    REPTILIAN,
    AVIAN,
    CEPHILOPOD,
    ///
    ELEMENTAL,
    GOLEM,
    SPRITE
}
public enum Faction
{
    GOODIES,
    BADDIES
}

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "ScriptableObjects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    // Random Gen
    public Sprite Portrait;
    public string Name;

    // Persistent
    public int Level;
    public float CurrentEXP;
    public float NextLevelEXP;

    public Race Race;
    public Faction Faction;

    public EXPpackage CurrentSkillEXP;
    public EXPpackage NextLevelSkillEXP;
    public LVLpackage SkillsLevels;

    public CharacterAbility[] InnateAbilities;
    //public BaseEffect[] InnatePassives;

    public void Clone(CharacterSheet source)
    {
        Name = source.Name;
        Level = source.Level;
        CurrentEXP = source.CurrentEXP;
        NextLevelEXP = source.NextLevelEXP;

        Portrait = source.Portrait;
        Race = source.Race;
        Faction = source.Faction;

        CurrentSkillEXP.Clone(source.CurrentSkillEXP);
        NextLevelSkillEXP.Clone(source.NextLevelSkillEXP);
        SkillsLevels.Clone(source.SkillsLevels);

        if (source.InnateAbilities != null)
        {
            InnateAbilities = new CharacterAbility[source.InnateAbilities.Length];
            for (int i = 0; i < source.InnateAbilities.Length; i++)
                InnateAbilities[i] = source.InnateAbilities[i];
        }
    }

    public void Initialize(bool fresh = true)
    {
        Debug.Log("Sheet init");
        if (fresh)
        {
            Level = 0;
            CurrentEXP = 0;
            NextLevelEXP = 0;
        }
        CurrentSkillEXP.Initialize(!fresh);
        NextLevelSkillEXP.Initialize(!fresh);
        SkillsLevels.Initialize(!fresh);// = new LVLpackage(CharacterMath.STATS_SKILLS_COUNT);

        Debug.Log($"Innates: {InnateAbilities != null}");
    }

    /*== GOOD ==
     * =MAINSTATS
     * health
     * stamina
     * mana
     * speed
     * 
     * =SIDESTATS
     * evade
     * block
     * parry
     * resistance
     * CoolDownBar
     * level scaling
     * race modifiers
     * 
     * == MAYBE ==
     * phase? ghost?
     */
}