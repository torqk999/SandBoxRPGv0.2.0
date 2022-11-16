using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(DraggableButton))]
public class DraggableButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        DraggableButton button = (DraggableButton)target;
    }
}

public class DraggableButton : SelectableButton
{
    [Header("DraggableButton")]
    public Vector2 CurrentPosMouse;
    public Vector2 OldPosButton;
    public Vector2 NewPosButton;
    public Vector2 Offset;
    public Vector2 ButtonBounds;
    public bool Following;
    
    public void SnapButton(bool success = false)
    {
        if (success)
            MyRect.position = NewPosButton;
        else
            MyRect.position = OldPosButton;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OldPosButton = (Vector2)MyRect.position;
        Offset = OldPosButton - CurrentPosMouse;
        Following = true;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        Following = false;
    }

    void FollowMouse(Vector2 mousePos)
    {
        if (!Following)
            return;

        this.transform.position = CurrentPosMouse + Offset;
    }

    protected override void Start()
    {
        base.Start();
        //MyRect = gameObject.GetComponent<RectTransform>();
        ButtonBounds.x = MyRect.rect.width / 2;
        ButtonBounds.y = MyRect.rect.height / 2;
    }

    // Update is called once per frame
    public override void Update()
    {
        CurrentPosMouse = Input.mousePosition;
        FollowMouse(CurrentPosMouse);
    }
}
