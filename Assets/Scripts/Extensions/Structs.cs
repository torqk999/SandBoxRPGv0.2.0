using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enums
public enum EffectType
{
    ONCE,
    TIMED,
    PASSIVE,
    SPAWN
}
public enum EffectTarget
{
    HEALTH,
    STAMINA,
    MANA,
    SPEED,
    CROWD_CONTROL
}
public enum EffectStatus
{
    STAT_CHANGE, // Damage, Heal
    STAT_ADJUST, // MaxHealth, Resistance, etc.
    DOT
}
public enum CCstatus
{
    IMMOBILE,
    UN_ARMED,
    SILENCED
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

// Structs
[System.Serializable]
public struct Effect
{
    public string Name;
    public Sprite Sprite;
    public EffectType Type;
    public EffectTarget Target;
    public EffectStatus Status;
    public CCstatus CCstatus;
    public ElementPackage ElementPack;
    //public ElementPackage PenPackage;
    public float Duration;
    public float Timer;
    public bool bIsBuff;

    public Effect(Effect source)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        Type = source.Type;
        Target = source.Target;
        Status = source.Status;
        CCstatus = source.CCstatus;
        ElementPack = source.ElementPack;
        Duration = source.Duration;
        Timer = (Type == EffectType.TIMED) ? Duration : 0;
        this.bIsBuff = source.bIsBuff;
    }
}
[System.Serializable]
public struct StatPackage
{
    public float HEALTH;
    public float STAMINA;
    public float MANA;
    public float SPEED;

    public void Amplify(float amp)
    {
        float[] returnValues = PullData();
        for (int i = 0; i < returnValues.Length; i++)
            returnValues[i] *= amp;
        EnterData(returnValues);
    }
    public void EnterData(float[] input)
    {
        if (input.Length < CharacterMath.STATS_RAW_COUNT)
            return;

        HEALTH = input[0];
        STAMINA = input[1];
        MANA = input[2];
        SPEED = input[3];
    }
    public float[] PullData()
    {
        float[] output = new float[CharacterMath.STATS_RAW_COUNT];

        output[0] = HEALTH;
        output[1] = STAMINA;
        output[2] = MANA;
        output[3] = SPEED;

        return output;
    }
    public StatPackage Clone(StatPackage source)
    {
        StatPackage clone = new StatPackage();

        clone.EnterData(source.PullData());

        return clone;
    }
}
[System.Serializable]
public struct ElementPackage
{
    public float PHYSICAL;
    public float FIRE;
    public float WATER;
    public float EARTH;
    public float AIR;
    public float POISON;
    public float HEALING;

    public void Amplify(float amp)
    {
        float[] returnValues = PullData();
        for (int i = 0; i < returnValues.Length; i++)
            returnValues[i] *= amp;
        EnterData(returnValues);
    }
    public void EnterData(float[] input)
    {
        if (input.Length != CharacterMath.STATS_ELEMENT_COUNT)
            return;

        PHYSICAL = input[0];
        FIRE = input[1];
        WATER = input[2];
        EARTH = input[3];
        AIR = input[4];
        POISON = input[5];
        HEALING = input[6];
    }
    public float[] PullData()
    {
        float[] output = new float[CharacterMath.STATS_ELEMENT_COUNT];

        output[0] = PHYSICAL;
        output[1] = FIRE;
        output[2] = WATER;
        output[3] = EARTH;
        output[4] = AIR;
        output[5] = POISON;
        output[6] = HEALING;

        return output;
    }
    public ElementPackage Clone(ElementPackage source)
    {
        ElementPackage clone = new ElementPackage();

        clone.EnterData(source.PullData());

        return clone;
    }
}
[System.Serializable]
public struct EXPpackage
{
    public float LIGHT;
    public float MEDIUM;
    public float HEAVY;
    public float ONE_HAND;
    public float TWO_HAND;
    public float SHIELD;
    public float RANGED;
    public float MAGIC;

    public float LEVEL;

    public void EnterData(float[] input)
    {
    }
}
[System.Serializable]
public struct LVLpackage
{
    public int LIGHT;
    public int MEDIUM;
    public int HEAVY;
    public int ONE_HAND;
    public int TWO_HAND;
    public int SHIELD;
    public int RANGED;
    public int MAGIC;

    public int CHARACTER;

    public void EnterData(int[] input)
    {

    }
    public int[] PullData()
    {
        int[] output = new int[CharacterMath.SKILLS_COUNT];

        output[0] = LIGHT;
        output[1] = MEDIUM;
        output[2] = HEAVY;
        output[3] = ONE_HAND;
        output[4] = TWO_HAND;
        output[5] = SHIELD;
        output[6] = RANGED;
        output[7] = MAGIC;

        return output;
    }
}
[System.Serializable]
public struct InteractData
{
    public TriggerType Type;
    public string Splash;
    public float HealthCurrent;
    public float HealthMax;
}

//Interfaces
public interface Interaction
{
    public void Interact();
    public InteractData GetInteractData();
}