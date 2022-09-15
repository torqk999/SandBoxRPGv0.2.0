using System.Collections;
using System.Collections.Generic;

// Enums
public enum EffectDuration
{
    ONCE,
    TIMED,
    PASSIVE,
    SUSTAINED
}
public enum EffectAction
{
    RES,
    STAT,
    CROWD_CONTROL,
    IMMUNE_RES,
    IMMUNE_STAT,
    IMMUNE_CC,
    SPAWN
}
public enum ValueType
{
    NONE,
    PERC_CURR,
    PERC_MAX,
    PERC_MISS,
    FLAT
}
public enum CCstatus
{
    NONE,
    IMMOBILE,
    UN_ARMED,
    SILENCED,
    FEARED,
    CHARMED,
    DRUNK
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
public enum RawStat
{
    HEALTH,
    STAMINA,
    MANA,
    SPEED,
}
[System.Serializable]
public struct StatReflection
{
    public float HEALTH;
    public float STAMINA;
    public float MANA;
    public float SPEED;

    public bool Reflect(ref float[] rawStats, bool inject = true)
    {
        try
        {
            if (inject)
            {
                rawStats[(int)RawStat.HEALTH] = HEALTH;
                rawStats[(int)RawStat.STAMINA] = STAMINA;
                rawStats[(int)RawStat.MANA] = MANA;
                rawStats[(int)RawStat.SPEED] = SPEED;
            }
            else
            {
                HEALTH = rawStats[(int)RawStat.HEALTH];
                STAMINA = rawStats[(int)RawStat.STAMINA];
                MANA = rawStats[(int)RawStat.MANA];
                SPEED = rawStats[(int)RawStat.SPEED];
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
[System.Serializable]
public struct StatPackage
{
    public float[] Stats;
    public StatReflection Reflection;
    public StatPackage(StatPackage source)
    {
        Reflection = source.Reflection;
        Stats = new float[CharacterMath.STATS_RAW_COUNT];
        if (source.Stats != null && source.Stats.Length == CharacterMath.STATS_RAW_COUNT)
            for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
                Stats[i] = source.Stats[i];
    }
    public void Init()
    {
        Stats = new float[CharacterMath.STATS_RAW_COUNT];
    }
    public void Amplify(float amp)
    {
        for (int i = 0; i < Stats.Length; i++)
            Stats[i] *= amp;
    }
}
public enum Element
{
    PHYSICAL,
    FIRE,
    WATER,
    EARTH,
    AIR,
    POISON,
    HEALING
}
[System.Serializable]
public struct ElementReflection
{
    public float PHYSICAL;
    public float FIRE;
    public float WATER;
    public float EARTH;
    public float AIR;
    public float POISON;
    public float HEALING;

    public bool Reflect(ref float[] elements, bool inject = true)
    {
        try
        {
            if (inject)
            {
                elements[(int)Element.PHYSICAL] = PHYSICAL;
                elements[(int)Element.FIRE] = FIRE;
                elements[(int)Element.WATER] = WATER;
                elements[(int)Element.EARTH] = EARTH;
                elements[(int)Element.AIR] = AIR;
                elements[(int)Element.POISON] = POISON;
                elements[(int)Element.HEALING] = HEALING;
            }
            else
            {
                PHYSICAL = elements[(int)Element.PHYSICAL];
                FIRE = elements[(int)Element.FIRE];
                WATER = elements[(int)Element.WATER];
                EARTH = elements[(int)Element.EARTH];
                AIR = elements[(int)Element.AIR];
                POISON = elements[(int)Element.POISON];
                HEALING = elements[(int)Element.HEALING];
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
[System.Serializable]
public struct ElementPackage
{
    public ElementReflection Reflection;
    public float[] Elements;

    public ElementPackage(ElementPackage source)
    {
        Reflection = source.Reflection; // maybe?
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
        if (source.Elements != null && source.Elements.Length == CharacterMath.STATS_ELEMENT_COUNT)
            for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
                Elements[i] = source.Elements[i];
    }

    public void Init()
    {
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
    }

    public void Amplify(float amp)
    {
        for (int i = 0; i < Elements.Length; i++)
            Elements[i] *= amp;
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