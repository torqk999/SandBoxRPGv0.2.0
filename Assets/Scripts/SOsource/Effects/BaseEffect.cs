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
    public int EquipID; // id == -1 timed proc, == 0 innate ability, > 0 equip ability
    public float DurationLength;   
    public float Timer;
    public virtual void ApplySingleEffect(Character character, bool cast = false, int equipId = -1)
    {
        if (cast)
            AddRisidualEffect(character, equipId);
    }
    public virtual void AddRisidualEffect(Character character, int equipId = -1)
    {
        if (EquipID == -1 && DurationLength > 0) // timed proc
            character.Risiduals.Add(GenerateEffect(equipId));

        if (EquipID > -1) // Sustain/Passive
        {

        }
    }
    public virtual void CloneEffect(BaseEffect source, int equipId = -1, float potency = 1, bool inject = true)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        Condition = source.Condition;
        Impact = source.Impact;

        EquipID = equipId;
        DurationLength = source.DurationLength;
        Timer = DurationLength;
    }

    public virtual BaseEffect GenerateEffect(int equipId = -1, float potency = 1, bool inject = true)
    {
        BaseEffect newEffect = (BaseEffect)CreateInstance("BaseEffect");
        newEffect.CloneEffect(this, equipId);
        return newEffect;
    }
}
