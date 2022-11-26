using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(ExtendedUI))]
public class ExtendedButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        ExtendedUI button = (ExtendedUI)target;
    }
}

/*public enum ButtonType
{
    DEFAULT,
    ITEM,
    CHARACTER,
    //EQUIP,
    //INVENTORY,
    SKILL,
    //HOT_BAR,
    //CONTAINER,
    //SLOT_RING,
    //KEY_MAP
}*/

[Serializable]
public struct UI_Options
{
    public PlaceHolderType PlaceType;
    public int Index;
    public bool ResetImage;
    public RootScriptObject Root;
    public Page Page;

    public UI_Options(Page page = null, PlaceHolderType type = default, bool resetImage = false, int size = 0)
    {
        Index = size;
        Root = null;
        Page = page;

        ResetImage = resetImage;
        PlaceType = type;
        //ButtonType = default;
    }
}

public class ExtendedUI : Selectable
{
    [Header ("ExtendedButton")]
    public UIManager UIMan;
    public RectTransform MyRect;
    public Image MyImage;

    public Slider CD_Bar;
    public Text ButtonText;

    public virtual void Assign(RootScriptObject root)
    {
        //if (root != null &&
        //    root.sprite != null)

        //MyImage.sprite = root.sprite;
    }
    
    public virtual void CreateCallBackIdentity() { }

    public virtual void UpdateContent()
    {

    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    bool GetUIMan()
    {
        Transform parent = transform.parent;
        UIManager uiMan = null;
        int seatBelt = 0; //  >: |  //
        do
        {
            if (parent != null)
                uiMan = parent.GetComponent<UIManager>();

            if (uiMan != null)
            {
                UIMan = uiMan;
                return true;
            }

            parent = parent.parent;
            seatBelt++;
        }
        while (parent != null && seatBelt < 50);
        return false;
    }

    public virtual void Init(UI_Options options)
    {
        Debug.Log("Init extend");
        GetUIMan();
        gameObject.tag = GlobalConstants.TAG_BUTTON;
        MyRect = gameObject.GetComponent<RectTransform>();
        MyImage = gameObject.GetComponent<Image>();
        try { ButtonText = MyRect.transform.GetChild(0).GetComponent<Text>(); }
        catch { Debug.Log($"Button Text failed to be found!"); }
        try { CD_Bar = MyRect.transform.GetChild(1).GetComponent<Slider>(); }
        catch
        {
            //Debug.Log($"Cooldown Slider failed to be found!");
        }
        Debug.Log("Extended Init done");
    }

    protected override void Start()
    {
        base.Start();
    }
    // Update is called once per frame
    public virtual void Update()
    {
        
    }
}
