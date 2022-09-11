using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using System.Runtime.InteropServices;

using UnityEditor;

[CustomEditor(typeof(ExtendedButton))]
public class TestBOOTONeditor : Editor
{
    public override void OnInspectorGUI()
    {
        ExtendedButton button = (ExtendedButton)target;

        button.Type = (ButtonType)EditorGUILayout.EnumPopup("Type", button.Type);
        button.Index = EditorGUILayout.IntField("Index", button.Index);
        button.UIMan = (UIManager)EditorGUILayout.ObjectField("UIMan", button.UIMan, typeof(UIManager), true);
        button.oldPosMouse = EditorGUILayout.Vector2Field("oldPosMouse", button.oldPosMouse);
        button.oldPosButton = EditorGUILayout.Vector2Field("oldPosButton", button.oldPosButton);
        button.currentDelta = EditorGUILayout.Vector2Field("currentDelta", button.currentDelta);
        button.ButtonBounds = EditorGUILayout.Vector2Field("ButtonBounds", button.ButtonBounds);
        button.Following = EditorGUILayout.Toggle("Following", button.Following);

        // Show default inspector property editor
        DrawDefaultInspector();
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
    public ButtonType Type;
    public int Index;
    public UIManager UIMan;
    public Vector2 oldPosMouse;
    public Vector2 oldPosButton;
    public Vector2 currentDelta;
    public Vector2 ButtonBounds;
    public bool Following;
    public RectTransform MyRect;

    //[DllImport("user32.dll")]
    //public static extern bool SetCursorPos(int X, int Y);
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        oldPosMouse = Input.mousePosition;
        oldPosButton = this.transform.position;
        currentDelta = Vector3.zero;
        Following = true;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        Following = false;

        if (MyRect == null ||
            UIMan == null ||
            (Math.Abs(currentDelta.x) < ButtonBounds.x && Math.Abs(currentDelta.y) < ButtonBounds.y))
        {
            this.transform.position = oldPosButton;
            return;
        }


    }
    void FollowMouse()
    {
        if (!Following)
            return;

        currentDelta = (Vector2)Input.mousePosition - oldPosMouse;
        this.transform.position = oldPosButton + currentDelta;
    }
    protected override void Start()
    {
        MyRect = this.gameObject.GetComponent<RectTransform>();
        ButtonBounds.x = MyRect.rect.width / 2;
        ButtonBounds.y = MyRect.rect.height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
    }
}
