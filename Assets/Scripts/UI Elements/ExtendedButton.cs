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

    //[DllImport("user32.dll")]
    //public static extern bool SetCursorPos(int X, int Y);

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
        MyRect = gameObject.GetComponent<RectTransform>();
        MyImage = gameObject.GetComponent<Image>();
    }
    // Update is called once per frame
    public virtual void Update()
    {
        
    }
}
