using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(PlaceHolderButton))]
public class PlaceHolderButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        PlaceHolderButton button = (PlaceHolderButton)target;
    }
}

public enum PlaceHolderType
{
    NONE,
    INVENTORY,
    EQUIP,
    SKILL
}

public class PlaceHolderButton : SelectableButton
{
    [Header("PlaceHolder")]
    public DraggableButton Occupant;

    public override bool Vacate(DraggableButton drag)
    {
        if (Occupant == null)
            return true;

        switch(PlaceType)
        {
            case PlaceHolderType.INVENTORY:
                if (!(drag is InventoryButton))
                    return false;
                break;

            case PlaceHolderType.EQUIP:
                if (!(drag is EquipmentButton))
                    return false;
                break;

            case PlaceHolderType.SKILL:
                if (!(drag is SkillButton))
                    return false;
                break;
        }

        return Occupant.Vacate(drag);
    }
    public void ResetImage()
    {
        if (Root == null || Root.sprite == null)
            MyImage.sprite = UIMan.PlaceHolderSprite;
        else
            MyImage.sprite = Root.sprite;
    }

    public override void Init()
    {
        base.Init();
        if (UIMan == null)
            return;
        MyImage.sprite = UIMan.PlaceHolderSprite;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
