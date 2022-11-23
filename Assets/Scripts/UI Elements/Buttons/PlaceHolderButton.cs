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
    public PlaceHolderType PlaceType;
    //public PlaceHolderPanel Panel;
    
    public bool CheckCanOccupy(DraggableButton drag)
    {
        switch (PlaceType)
        {
            case PlaceHolderType.INVENTORY:
                if (!(drag is InventoryButton) ||
                    !(drag is EquipmentButton))
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

            case PlaceHolderType.NONE:
                return false;
        }
        return true;
    }
    public bool Vacate()
    {
        if (Panel.VirtualParent.Occupants.List[SlotIndex] == null)
            return true;

        return ((DraggableButton)Panel.VirtualParent.Occupants.List[SlotIndex]).Relocate();
    }
    public void ResetImage()
    {
        MyImage.sprite = UIMan.PlaceHolderSprite;
    }

    public override void Init(ButtonOptions options)
    {
        //Debug.Log("PlaceHolder Init");
        base.Init(options);
        if (UIMan == null)
            return;

        //Debug.Log("PlaceHolder found UIMan");

        PlaceType = options.PlaceType;

        if (options.ResetImage)
        {
            MyImage.sprite = UIMan.PlaceHolderSprite;

            SpriteState ss = new SpriteState();

            ss.highlightedSprite = MyImage.sprite;
            ss.selectedSprite = MyImage.sprite;
            ss.pressedSprite = MyImage.sprite;
            ss.disabledSprite = MyImage.sprite;

            spriteState = ss;
        }
        

        //Debug.Log("PlaceHolder Init done");
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
