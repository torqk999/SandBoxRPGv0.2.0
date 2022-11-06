using UnityEngine;
// Structs
[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effect")]
public class Effect : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public ParticleSystem PSystem;

    public EffectDuration Duration;
    public ValueType Value;
    public EffectAction Action;
    public RawStat TargetStat;
    public Element TargetElement;
    public CCstatus TargetCCstatus;

    public ElementPackage ElementPack;
    public StatPackage StatAdjustPack;

    public int EquipID; // for resolving passives/sustained lost on equip removal
    public float DurationLength;
    public float Timer;
    public bool bIsImmune;
    public bool bAllImmune;

    public Effect(string name, CCstatus status, Sprite sprite = null) // Hard indefinite CC creation (ez death)
    {
        Name = name;
        Sprite = sprite;
        Duration = EffectDuration.PASSIVE;
        Action = EffectAction.CROWD_CONTROL;
        TargetCCstatus = status;
    }

    public void CloneEffect(Effect source, int equipId = -1, float amp = 1, bool inject = true)
    {
        Name = source.Name;
        EquipID = equipId;
        Sprite = source.Sprite;
        Duration = source.Duration;
        Value = source.Value;
        Action = source.Action;
        TargetCCstatus = source.TargetCCstatus;

        if (source.Action == EffectAction.DMG_HEAL ||
            source.Action == EffectAction.RES_ADJ)
        {
            ElementPack = new ElementPackage(source.ElementPack);
            ElementPack.Reflection.Reflect(ref ElementPack.Elements, inject);
            ElementPack.Amplify(amp);
        }
        
        if (source.Action == EffectAction.STAT_ADJ)
        {
            StatAdjustPack = new StatPackage(source.StatAdjustPack);
            StatAdjustPack.Reflection.Reflect(ref StatAdjustPack.Stats, inject);
            StatAdjustPack.Amplify(amp);
        }
        
        DurationLength = source.DurationLength;
        Timer = (Duration == EffectDuration.TIMED) ? DurationLength : 0;
        bIsImmune = source.bIsImmune;
    }
}
