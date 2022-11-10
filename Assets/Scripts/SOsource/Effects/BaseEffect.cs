using UnityEngine;

//[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effects")]
public class BaseEffect : ScriptableObject
{
    [Header("Base Properties")]
    public string Name;
    public Sprite Sprite;
    public ParticleSystem Condition;
    public ParticleSystem Impact;

    [Header("Persistant Properties")]
    public int EquipID;           // n < 0 == passive/sustained
    public float DurationLength;
    public float Timer;

    public BaseEffect() // default constructor
    {

    }
    
    public virtual void CloneEffect(BaseEffect source, int equipId = -1, float amp = 1, bool inject = true)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        Condition = source.Condition;
        Impact = source.Impact;

        EquipID = equipId;
        DurationLength = source.DurationLength;
        Timer = DurationLength;
    }
}
