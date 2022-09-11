using System.Collections;
using System.Collections.Generic;

// Enums
public enum EffectDuration
{
    ONCE,
    TIMED,
    PASSIVE,
    SPAWN
}
public enum EffectType
{
    RESISTANCE,
    STAT,
    CROWD_CONTROL
}
public enum RawStat
{
    HEALTH,
    STAMINA,
    MANA,
    SPEED,
}
public enum EffectValue
{
    NONE,
    PERC_CURR,
    PERC_MAX,
    PERC_MISS,
    FLAT
}
/*public enum EffectStatus
{
    NONE,
    STAT_CURRENT, // Damage, Heal
    STAT_MAX, // MaxHealth, Resistance, etc.
    DOT
}*/
public enum CCstatus
{
    NONE,
    IMMOBILE,
    UN_ARMED,
    SILENCED
}

public enum SkillType
{
    LIGHT,
    MEDIUM,
    HEAVY,
    HAND_ONE,
    HAND_OFF,
    HAND_TWO,
    SHIELD,
    RANGED,
    MAGIC,
}
/*[System.Serializable]
public struct StatSingle
{
    public RawStat Stat;
    public float Value;
}*/
[System.Serializable]
public struct StatPackage
{
    public float[] Stats;
    public StatPackage(int count)
    {
        Stats = new float[count];
    }
    public void Amplify(float amp)
    {
        for (int i = 0; i < Stats.Length; i++)
            Stats[i] *= amp;
    }
}
[System.Serializable]
public struct ElementSingle
{
    public Element Element;
    public float Value;
}

public enum Element
{
    PHYSICAL,
    FIRE,
    WATER,
    EARTH,
    AIR,
    POSION,
    HEALING
}

[System.Serializable]
public struct ElementPackage
{
    public ElementSingle[] Elements;

    public ElementPackage(ElementPackage source)
    {
        Elements = new ElementSingle[CharacterMath.STATS_ELEMENT_COUNT];
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
            Elements[i].Element = (Element)i;

        if (source.Elements != null)
            for (int i = 0; i < source.Elements.Length; i++)
                Elements[(int)source.Elements[i].Element].Value = source.Elements[i].Value;
    }

    public void Init()
    {
        Elements = new ElementSingle[CharacterMath.STATS_ELEMENT_COUNT];
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
            Elements[i].Element = (Element)i;
    }

    public void Amplify(float amp)
    {
        for (int i = 0; i < Elements.Length; i++)
            Elements[i].Value *= amp;
    }

}
[System.Serializable]
public struct EXPpackage
{
    public float[] Experience;
    public EXPpackage(int count)
    {
        Experience = new float[count];
    }
}
[System.Serializable]
public struct LVLpackage
{
    public int[] Levels;
    public LVLpackage(int count)
    {
        Levels = new int[count];
    }
}
[System.Serializable]
public class InteractData
{
    public TriggerType Type;
    public string Name;
}

public class CharacterData : InteractData
{
    public Character myCharacter;

    public float HealthCurrent;
    public float HealthMax;

    public CharacterData(Character character)
    {
        Name = character.Sheet.Name;
        myCharacter = character;

        HealthCurrent = character.CurrentStats.Stats[(int)RawStat.HEALTH];
        HealthMax = character.MaximumStatValues.Stats[(int)RawStat.HEALTH];
    }
}

public class LootData : InteractData
{
    public float Weight;
    public Quality Quality;
}

//Interfaces
public interface Interaction
{
    public void Interact();
    public InteractData GetInteractData();
}