using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(ExtendedButton))]
public class ExtendedButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        ExtendedButton button = (ExtendedButton)target;
    }
}

public enum ButtonType
{
    DEFAULT,
    ITEM,
    PLACE,  // implement later...
    //EQUIP,
    //INVENTORY,
    SKILL,
    //HOT_BAR,
    //CONTAINER,
    //SLOT_RING,
    //KEY_MAP
}

[Serializable]
public struct ButtonOptions
{
    public ButtonType ButtonType;
    public PlaceHolderType PlaceType;
    public int Index_Size;
    public bool ResetImage;
    public RootScriptObject Root;
    public ListPanel Page;

    /// <summary>
    /// For building a new root directly into the target folder
    /// </summary>
    /// <param name="root"> The root object that is tied to this button </param>
    /// <param name="page"> The page component that currently houses this button </param>
    /// <param name="index"> The index of this button and it's placeHolder </param>
    public ButtonOptions(RootScriptObject root , ListPanel page, int index = 0)
    {
        Index_Size = index;
        Root = root;
        Page = page;

        ResetImage = false;
        PlaceType = default;
        ButtonType = default;
    }

    /// <summary>
    /// For build a new placeHolder directly into the target folder
    /// </summary>
    /// <param name="panel"> The panel component that currently houses this button </param>
    /// <param name="size"> The index of this placHolder and it's button </param>
    public ButtonOptions(ListPanel panel, bool resetImage = false, int size = 0)
    {
        Index_Size = size;
        Root = null;
        Page = panel;

        ResetImage = resetImage;
        PlaceType = default;
        ButtonType = default;
    }
}

public class ExtendedButton : Button
{
    [Header ("ExtendedButton")]
    public UIManager UIMan;
    public RectTransform MyRect;
    public Image MyImage;

    public Slider CD_Bar;
    public Text ButtonText;

    public virtual void Assign(RootScriptObject root)
    {
        if (root != null &&
            root.sprite != null)

        MyImage.sprite = root.sprite;
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

    public virtual void Init(ButtonOptions options, RootScriptObject root = null)
    {
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
