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
    //public EffectStatus Status;
    public RawStat TargetStat;
    public CCstatus CCstatus;
    public ElementPackage ElementPack;
    //public ElementPackage PenPackage;
    public float DurationLength;
    public float Timer;
    public bool bIsBuff;

    public void Clone(Effect source, float amp = 1, bool inject = true)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        Duration = source.Duration;
        Value = source.Value;
        Type = source.Type;
        //Status = source.Status;
        CCstatus = source.CCstatus;
        ElementPack = source.ElementPack;
        ElementPack.Reflection.Reflect(ref ElementPack.Elements, inject); // <.<  just wild....
        ElementPack.Amplify(amp);
        DurationLength = source.DurationLength;
        Timer = (Duration == EffectDuration.TIMED) ? DurationLength : 0;
        bIsBuff = source.bIsBuff;
    }
}
