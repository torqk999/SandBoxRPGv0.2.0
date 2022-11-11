using System;
using System.Collections;
using System.Collections.Generic;

// Enums
public enum ClassType
{
    WARRIOR,
    MAGE,
    ROGUE
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
    DEAD,
    IMMOBILE,
    UN_ARMED,
    SILENCED,
    FEARED,
    CHARMED,
    DRUNK,
    STUNNED
}
[Serializable]
public struct CCstateReflection
{
    public bool DEAD;
    public bool IMMOBILE;
    public bool UN_ARMED;
    public bool SILENCED;
    public bool FEARED;
    public bool CHARMED;
    public bool DRUNK;
    public bool STUNNED;

    public bool Reflect(ref bool[] ccStates, bool inject = true)
    {
        try
        {
            if (inject)
            {
                ccStates[(int)CCstatus.DEAD] = DEAD;
                ccStates[(int)CCstatus.IMMOBILE] = IMMOBILE;
                ccStates[(int)CCstatus.UN_ARMED] = UN_ARMED;
                ccStates[(int)CCstatus.SILENCED] = SILENCED;
                ccStates[(int)CCstatus.FEARED] = FEARED;
                ccStates[(int)CCstatus.CHARMED] = CHARMED;
                ccStates[(int)CCstatus.DRUNK] = DRUNK;
                ccStates[(int)CCstatus.STUNNED] = STUNNED;
            }
            else
            {
                DEAD = ccStates[(int)CCstatus.DEAD];
                IMMOBILE = ccStates[(int)CCstatus.IMMOBILE];
                UN_ARMED = ccStates[(int)CCstatus.UN_ARMED];
                SILENCED = ccStates[(int)CCstatus.SILENCED];
                FEARED = ccStates[(int)CCstatus.FEARED];
                CHARMED = ccStates[(int)CCstatus.CHARMED];
                DRUNK = ccStates[(int)CCstatus.DRUNK];
                STUNNED = ccStates[(int)CCstatus.STUNNED];
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
public enum SkillType
{
    UN_ARMED,
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
[Serializable]
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
[Serializable]
public struct StatPackage
{
    public StatReflection Reflection;
    public float[] Stats;

    public StatPackage(CharacterSheet sheet)
    {
        Stats = new float[CharacterMath.STATS_RAW_COUNT];
        for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
        {
            float stat = CharacterMath.RAW_BASE[i] +
                (CharacterMath.RAW_GROWTH[i] *
                CharacterMath.RAW_MUL_RACE[(int)sheet.Race, i] *
                sheet.Level);

            Stats[i] = stat;
        }
        Reflection = new StatReflection();
        Reflection.Reflect(ref Stats, false);
    }
    public StatPackage(StatPackage source)
    {
        Reflection = source.Reflection;
        Stats = new float[CharacterMath.STATS_RAW_COUNT];
        Clone(source);
    }

    public void Clone(StatPackage source)
    {
        if (source.Stats != null && source.Stats.Length == CharacterMath.STATS_RAW_COUNT)
            for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
                Stats[i] = source.Stats[i];
    }
    public void Amplify(float amp)
    {
        for (int i = 0; i < Stats.Length; i++)
            Stats[i] *= amp;
    }
    public void Reflect(bool inject = true)
    {
        Reflection.Reflect(ref Stats, inject);
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
[Serializable]
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
[Serializable]
public struct ElementPackage
{
    public ElementReflection Reflection;
    public float[] Elements;

    public ElementPackage(float healMagnitude)
    {
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
        Elements[(int)Element.HEALING] = healMagnitude;
        Reflection = new ElementReflection();
        Reflection.Reflect(ref Elements, false);
    }
    public ElementPackage(CharacterSheet sheet)
    {
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            Elements[i] = CharacterMath.RES_MUL_RACE[(int)sheet.Race, i] * (CharacterMath.RES_BASE[i] +
                (CharacterMath.RES_GROWTH[i] * sheet.Level));
        }
        Reflection = new ElementReflection();
        Reflection.Reflect(ref Elements, false);
    }
    public ElementPackage(ElementPackage source)
    {
        Reflection = source.Reflection;
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
        Clone(source);
    }

    public void Clone(ElementPackage source)
    {
        if (source.Elements != null && source.Elements.Length == CharacterMath.STATS_ELEMENT_COUNT)
            for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
                Elements[i] = source.Elements[i];
    }
    public void Amplify(float amp)
    {
        for (int i = 0; i < Elements.Length; i++)
            Elements[i] *= amp;
    }
    public void Reflect(bool inject = true)
    {
        Reflection.Reflect(ref Elements, inject);
    }
}
[Serializable]
public struct EXPpackage
{
    public float[] Experience;
    public EXPpackage(int count)
    {
        Experience = new float[count];
    }
}
[Serializable]
public struct LVLpackage
{
    public int[] Levels;
    public LVLpackage(int count)
    {
        Levels = new int[count];
    }
}
[Serializable]
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