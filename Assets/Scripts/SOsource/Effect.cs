using UnityEngine;
// Structs
[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effect")]
public class Effect : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public EffectDuration Duration;
    public EffectValue Value;
    public EffectType Type;
    public EffectStatus Status;
    public RawStat TargetStat;
    public CCstatus CCstatus;
    public ElementPackage ElementPack;
    //public ElementPackage PenPackage;
    public float DurationLength;
    public float Timer;
    public bool bIsBuff;

    public Effect()
    {
        ElementPack = new ElementPackage(CharacterMath.STATS_ELEMENT_COUNT);
    }

    public Effect(Effect source)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        Duration = source.Duration;
        Value = source.Value;
        Type = source.Type;
        Status = source.Status;
        CCstatus = source.CCstatus;
        ElementPack = source.ElementPack;
        DurationLength = source.DurationLength;
        Timer = (Duration == EffectDuration.TIMED) ? DurationLength : 0;
        this.bIsBuff = source.bIsBuff;
    }
}
