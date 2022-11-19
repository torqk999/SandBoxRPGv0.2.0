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
    DRAG,
    PLACE,  // implement later...
    //EQUIP,
    //INVENTORY,
    //LIST_SKILL,
    HOT_BAR,
    //CONTAINER,
    //SLOT_RING,
    //KEY_MAP
}

[Serializable]
public struct ButtonOptions
{
    //public string ClassID;
    public ButtonType ButtonType;
    public PlaceHolderType PlaceType;
    public int Index;
    public RootScriptObject Root;
    public RectTransform Home;
    public ExtendedButton[] OccupantFolder;
    public ExtendedButton[] PlaceFolder;
    public PlaceHolderButton PlaceHolder;

    /// <summary>
    /// For building a new root directly into the target folder
    /// </summary>
    /// <param name="occupantFolder"> Where the root buttons go </param>
    /// <param name="root"> The root object that is tied to this button </param>
    /// <param name="placeHolder"> The current placeHolder button tied to this button  </param>
    /// <param name="home"> The parent transform of the button </param>
    /// <param name="index"> The index of this button and it's placeHolder </param>
    public ButtonOptions(ExtendedButton[] occupantFolder, RootScriptObject root = null, PlaceHolderButton placeHolder = null, RectTransform home = null, int index = 0)
    {
        Index = index;
        PlaceType = default;
        ButtonType = default;
        Root = root;
        Home = home;
        OccupantFolder = occupantFolder;
        PlaceFolder = null;
        PlaceHolder = placeHolder;
    }

    /// <summary>
    /// For build a new placeHolder directly into the target folder
    /// </summary>
    /// <param name="placeFolder"> Where the placeHolder buttons go </param>
    /// <param name="occupantFolder"> Where the root buttons go </param>
    /// <param name="home"> The parent transform of the button </param>
    /// <param name="index"> The index of this button and it's occupant </param>
    public ButtonOptions(ExtendedButton[] placeFolder , ExtendedButton[] occupantFolder , RectTransform home = null, int index = 0) // Place Holder
    {
        Index = index;
        PlaceType = default;
        ButtonType = default;
        Root = null;
        Home = home;
        OccupantFolder = occupantFolder;
        PlaceFolder = placeFolder;
        PlaceHolder = null;
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

    //public ButtonOptions Options;
    //[DllImport("user32.dll")]
    //public static extern bool SetCursorPos(int X, int Y);
    public virtual void Assign(RootScriptObject root)
    {
        if (root != null &&
            root.sprite != null)

        MyImage.sprite = root.sprite;
    }
    

    public virtual ExtendedButton GenerateButton(ButtonOptions options)
    {
        GameObject buttonObject = UIMan.GenerateButtonObject(options);
        ExtendedButton newButton = buttonObject.AddComponent<ExtendedButton>();
        return newButton;
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
