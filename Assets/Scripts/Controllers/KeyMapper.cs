using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct KeyMap
{
    public KeyAction Action;
    public KeyCode[] Keys;

    public KeyMap(KeyAction action)
    {
        Action = action;
        Keys = new KeyCode[GlobalConstants.ACTION_KEY_COUNT];
        for (int i = 0; i < Keys.Length; i++)
        {
            Keys[i] = KeyCode.None;
        }
    }

    public KeyMap(KeyMap source)//, float value)
    {
        Action = source.Action;
        //Value = value;
        Keys = new KeyCode[GlobalConstants.ACTION_KEY_COUNT];
        for(int i = 0; i < source.Keys.Length; i++)
        {
            try { Keys[i] = source.Keys[i]; }
            catch { Keys[i] = KeyCode.None; }
        }
    }
}

public class KeyMapper : MonoBehaviour
{
    public KeyMap[] Map;
    public KeyMap[] Default;
    public int OpenIndex;
    public int OpenSlot;
    public KeyCode OldKey;
    public bool bMapOpen;

    public void OpenKeyMap(int index)
    {
        if (bMapOpen)
            return;

        OpenSlot = index % 2;
        OpenIndex = (index - OpenSlot) / 2;
        OldKey = Map[OpenIndex].Keys[OpenSlot];

        bMapOpen = true;
        Debug.Log($"KeyMapIndex: {OpenIndex}");
    }

    public void CloseMap(KeyCode keyCode)
    {
        if (!bMapOpen)
            return;

        Map[OpenIndex].Keys[OpenSlot] = keyCode;
        bMapOpen = false;
    }

    void GenerateKeyMap()
    {
        Map = new KeyMap[Default.Length];

        for (int i = 0; i < Default.Length; i++)
            Map[i] = new KeyMap(Default[i]);
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateKeyMap();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
