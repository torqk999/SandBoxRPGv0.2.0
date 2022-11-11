using UnityEngine;

public enum EffectType
{
    PASSIVE,
    PROC,
    TOGGLE
}

//[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effects")]
public class BaseEffect : ScriptableObject
{
    [Header("Base Properties")]
    public string Name;
    public Sprite Sprite;
    public ParticleSystem Condition;
    public ParticleSystem Impact;

    [Header("Persistant Properties - Do Not Touch")]
    public CharacterSheet SourceSheet;
    public CharacterAbility SourceAbility;
    public Equipment SourceEquip;

    public EffectType EffectType;
    //public int EquipID; 
    //public int AbilityID;
    public float DurationLength;
    public float Timer;

    public virtual void ApplySingleEffect(Character target, CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool cast = false, bool toggle = true)
    {
        if (!cast)
            return;
        switch(EffectType)
        {
            case EffectType.PASSIVE:
                AddRisidualEffect(target, sheet, ability, equip);
                break;

            case EffectType.TOGGLE:
                if (toggle)
                    AddRisidualEffect(target, sheet, ability, equip);
                else
                    RemoveRisidualEffect(target, equip.EquipID);
                break;

            case EffectType.PROC:
                if (DurationLength > 0)
                    AddRisidualEffect(target, sheet, ability, equip);
                break;
        }  
    }
    public virtual void AddRisidualEffect(Character target, CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = false)
    {
        switch(EffectType)
        {
            case EffectType.PASSIVE:
            case EffectType.TOGGLE:
                target.Risiduals.Add(GenerateEffect(sheet, ability, equip, inject));
                break;

            case EffectType.PROC:
                if (DurationLength > 0) // Timed
                    target.Risiduals.Add(GenerateEffect(sheet, ability, equip, inject));
                break;
        }
    }
    public virtual void RemoveRisidualEffect(Character character, int equipId)
    {

    }
    public virtual void CloneEffect(BaseEffect source, CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = false)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        Condition = source.Condition;
        Impact = source.Impact;

        SourceSheet = sheet;
        SourceAbility = ability;
        SourceEquip = equip;

        if (ability != null)
        {
            //AbilityID = ability.AbilityID;
            switch(ability)
            {
                case ToggleAbility:
                    EffectType = EffectType.TOGGLE;
                    break;

                case ProcAbility:
                    EffectType = EffectType.PROC;
                    break;

                default:
                    EffectType = EffectType.PASSIVE;
                    break;
            }
        }
        
        DurationLength = source.DurationLength;
        Timer = DurationLength;
    }
    public virtual BaseEffect GenerateEffect(CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = true)
    {
        BaseEffect newEffect = (BaseEffect)CreateInstance("BaseEffect");
        newEffect.CloneEffect(this, sheet, ability, equip);
        return newEffect;
    }
}
