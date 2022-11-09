using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "CharacterAbility", menuName = "ScriptableObjects/CharacterAbility")]
public class CharacterAbility : ScriptableObject
{
    public int EquipID;
    public string Name;
    public Sprite Sprite;
    public void CloneAbility(CharacterAbility ability, int equipId = -1)//, float potency = 1, bool inject = false)
    {
        EquipID = equipId;
        Name = ability.Name;
        Sprite = ability.Sprite;
    }

    public float GeneratePotency(Character currentCharacter, Equipment equip)
    {
        /*float potency =*/ return 1 +

    (((currentCharacter.Sheet.Skills.Levels[(int)equip.EquipSkill] * CharacterMath.CHAR_LEVEL_FACTOR) +                      // Level

    (equip.EquipLevel * CharacterMath.WEP_LEVEL_FACTOR) +                                                                    // Weapon

    (currentCharacter.Sheet.Skills.Levels[(int)equip.EquipSkill] * CharacterMath.SKILL_MUL_LEVEL[(int)equip.EquipSkill])) *  // Skill

    CharacterMath.SKILL_MUL_RACE[(int)currentCharacter.Sheet.Race, (int)equip.EquipSkill]);                                  // Race
    }
}
