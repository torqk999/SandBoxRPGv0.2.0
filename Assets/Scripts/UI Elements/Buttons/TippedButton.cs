using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(TippedButton))]
public class TippedButtonButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        TippedButton button = (TippedButton)target;
    }
}

public class TippedButton : ExtendedButton
{
    [Header("TippedButton")]
    public StringBuilder Title;
    public StringBuilder Stats;
    public StringBuilder Flavour;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        UIMan.ToolTip.UpdateText(Title, Stats, Flavour);
        //Stats.Clear();
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        UIMan.ToolTip.DisableTip();
        base.OnPointerExit(eventData);
    }
    public override void Assign(RootScriptObject root)
    {
        base.Assign(root);

        Title.Clear();
        Stats.Clear();
        Flavour.Clear();
    }
    public override void Init(ButtonOptions options)
    {
        //Debug.Log("Tipped Init");
        base.Init(options);
        Title = new StringBuilder();
        Stats = new StringBuilder();
        Flavour = new StringBuilder();
        //Debug.Log("Tipped done");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); 
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
