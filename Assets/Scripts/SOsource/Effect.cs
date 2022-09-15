using UnityEngine;
// Structs
[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effect")]
public class Effect : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public EffectDuration Duration;
    public ValueType Value;
    public EffectAction Action;
    public RawStat TargetStat;
    public Element TargetElement;
    public CCstatus TargetCCstatus;
    public ElementPackage ElementPack;
    public StatPackage StatAdjustPack;
    //public ElementPackage PenPackage;
    public float DurationLength;
    public float Timer;
    public bool bIsImmune;
    public bool bAllImmune;

    public void Clone(Effect source, float amp = 1, bool inject = true)
    {
        Name = source.Name;
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
