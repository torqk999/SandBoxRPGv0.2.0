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
    INVENTORY,
    CONTAINER,
    LIST_SKILL,
    SLOT_SKILL,
    SLOT_EQUIP,
    SLOT_RING,
    KEY_MAP
}

public class ExtendedButton : Button
{
    [Header ("ExtendedButton")]
    //public ButtonType Type;
    public UIManager UIMan;
    public RectTransform MyRect;
    public Image MyImage;

    public Slider CD_Bar;
    public Text ButtonText;

    //[DllImport("user32.dll")]
    //public static extern bool SetCursorPos(int X, int Y);
    public GameObject GenerateButtonObject(GameObject prefab, Transform folder)
    {
        return Instantiate(prefab, folder);
    }

    public virtual ExtendedButton GenerateButton(GameObject prefab, Transform folder)
    {
        GameObject buttonObject = GenerateButtonObject(prefab, folder);
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
