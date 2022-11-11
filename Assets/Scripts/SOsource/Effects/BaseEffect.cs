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
    public int AbilityID;
    public float DurationLength;   
    public float Timer;

    public virtual void ApplySingleEffect(Character character, bool cast = false, int equipId = -1)
    {
        if (!cast)
            return;

        //if (toggle)
        //{
        //    if (character.Risiduals.Find(x => x))
        //}

        if (DurationLength > 0)
            AddRisidualEffect(character, equipId);
            
    }
    public virtual void AddRisidualEffect(Character character, int equipId = -1)
    {
        if (DurationLength > 0) // Timed
        {
            character.Risiduals.Add(GenerateEffect(equipId));
            return;
        }

        if (EquipID > -1) // Toggle
        {

        }
    }
    public virtual void RemoveRisidualEffect(Character character, int equipId)
    {

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

    public virtual BaseEffect GenerateEffect(float potency = 1, bool inject = true, int equipId = -1)
    {
        BaseEffect newEffect = (BaseEffect)CreateInstance("BaseEffect");
        newEffect.CloneEffect(this, equipId);
        return newEffect;
    }
}
