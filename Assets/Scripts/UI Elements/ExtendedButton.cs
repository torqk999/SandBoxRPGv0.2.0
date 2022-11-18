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
    EQUIP,
    INVENTORY,
    LIST_SKILL,
    SLOT_SKILL,
    //CONTAINER,
    //SLOT_RING,
    //KEY_MAP
}

[Serializable]
public struct ButtonOptions
{
    //public string ClassID;
    public ButtonType Type;
    public bool PlaceHolder;
    public int Count;
    public GameObject Body;
    public Transform Home;
    public ExtendedButton[] Folder;
    
    public ButtonOptions(GameObject body, ExtendedButton[] folder, Transform home = null, bool placeHolder = false, int count = 1)
    {
        Count = count;
        Type = default;
        Body = body;
        Home = home;
        Folder = folder;
        PlaceHolder = placeHolder;
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

    //[DllImport("user32.dll")]
    //public static extern bool SetCursorPos(int X, int Y);
    public virtual void Assign(RootScriptObject root)
    {
        if (root != null &&
            root.Sprite != null)

        MyImage.sprite = root.Sprite;
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

    protected override void Start()
    {
        base.Start();
        gameObject.tag = GlobalConstants.TAG_BUTTON;
        MyRect = gameObject.GetComponent<RectTransform>();
        MyImage = gameObject.GetComponent<Image>();
        try { ButtonText = MyRect.transform.GetChild(0).GetComponent<Text>(); }
        catch { Debug.Log($"Button Text failed to be found!"); }
        try { CD_Bar = MyRect.transform.GetChild(1).GetComponent<Slider>(); }
        catch { Debug.Log($"Cooldown Slider failed to be found!"); }

    }
    // Update is called once per frame
    public virtual void Update()
    {
        
    }
}
