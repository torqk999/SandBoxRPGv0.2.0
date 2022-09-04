using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using System.Runtime.InteropServices;

using UnityEditor;

[CustomEditor(typeof(TestBOOTON))]
public class TestBOOTONeditor : Editor
{
    public override void OnInspectorGUI()
    {
        TestBOOTON button = (TestBOOTON)target;

        button.oldPosMouse = EditorGUILayout.Vector3Field("oldPosMouse", button.oldPosMouse);
        button.oldPosButton = EditorGUILayout.Vector3Field("oldPosButton", button.oldPosButton);
        button.currentDelta = EditorGUILayout.Vector3Field("currentDelta", button.currentDelta);
        button.Following = EditorGUILayout.Toggle("Following", button.Following);

        // Show default inspector property editor
        DrawDefaultInspector();
    }
}

public class TestBOOTON : Button
{
    [Header("MyHeader")]
    public Vector3 oldPosMouse;
    public Vector3 oldPosButton;
    public Vector3 currentDelta;
    public bool Following;

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
        this.transform.position = oldPosButton;
        Following = false;
    }

    void FollowMouse()
    {
        if (!Following)
            return;

        currentDelta = Input.mousePosition - oldPosMouse;
        this.transform.position = oldPosButton + currentDelta;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
    }
}
