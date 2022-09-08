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
public enum EffectStatus
{
    NONE,
    STAT_CURRENT, // Damage, Heal
    STAT_MAX, // MaxHealth, Resistance, etc.
    DOT
}
public enum CCstatus
{
    NONE,
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
        //float[] returnValues = PullData();
        for (int i = 0; i < Stats.Length; i++)
            Stats[i] *= amp;
        //EnterData(returnValues);
    }
    /*public void EnterData(float[] input)
    {
        if (input.Length != CharacterMath.STATS_RAW_COUNT)
            return;

        input.CopyTo(Stats, 0);

        //for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
            //Stats[i] = input[i]

        //HEALTH = input[0];
        //STAMINA = input[1];
        //MANA = input[2];
        //SPEED = input[3];
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
    }*/
}
[System.Serializable]
public struct ElementPackage
{
    public float[] Elements;

    public ElementPackage(int count)
    {
        Elements = new float[count];
    }

    public void Amplify(float amp)
    {
        //float[] returnValues = PullData();
        for (int i = 0; i < Elements.Length; i++)
            Elements[i] *= amp;
        //EnterData(returnValues);
    }
    /*public void EnterData(float[] input)
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
    }*/
}
[System.Serializable]
public struct EXPpackage
{
    public float[] Experience;
    public EXPpackage(int count)
    {
        Experience = new float[count];
    }

    /*public float LIGHT;
    public float MEDIUM;
    public float HEAVY;
    public float ONE_HAND;
    public float TWO_HAND;
    public float SHIELD;
    public float RANGED;
    public float MAGIC;

    public float LEVEL;*/

    /*public void EnterData(float[] input)
    {
    }*/
}
[System.Serializable]
public struct LVLpackage
{
    public int[] Levels;
    public LVLpackage(int count)
    {
        Levels = new int[count];
    }
    /*public int LIGHT;
    public int MEDIUM;
    public int HEAVY;
    public int ONE_HAND;
    public int OFF_HAND;
    public int TWO_HAND;
    public int SHIELD;
    public int RANGED;
    public int MAGIC;

    public int CHARACTER;*/

    /*public void EnterData(int[] input)
    {

    }
    public int[] PullData()
    {
        int[] output = new int[CharacterMath.SKILLS_COUNT];

        output[0] = LIGHT;
        output[1] = MEDIUM;
        output[2] = HEAVY;
        output[3] = ONE_HAND;
        output[4] = OFF_HAND;
        output[5] = TWO_HAND;
        output[6] = SHIELD;
        output[7] = RANGED;
        output[8] = MAGIC;

        return output;
    }*/
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