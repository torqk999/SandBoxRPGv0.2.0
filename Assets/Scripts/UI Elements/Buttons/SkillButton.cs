using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(SkillButton))]
public class SkillButtonButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        SkillButton button = (SkillButton)target;
    }
}
public class SkillButton : DraggableButton
{
    [Header("Skill")]
    public CharacterAbility Skill;

    public override void Assign(RootScriptObject root)
    {
        if (!(root is CharacterAbility))
            return;
        CharacterAbility skill = (CharacterAbility)root;

        Skill = skill;
        try { MyImage.sprite = skill.sprite; }
        catch { Debug.Log("missing button sprite"); }
        Title.Append(skill.Name);
        BuildSkillString();
        Flavour.Append(skill.Flavour);
        // Logic for when in hot bar or in strat panel...
    }
    void BuildSkillString()
    {

    }

    public override bool Drop()
    {
        return base.Drop();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        UIMan.CharacterPageSelection();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
