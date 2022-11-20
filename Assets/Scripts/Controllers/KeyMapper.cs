using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct KeyMap
{
    public KeyAction Action;
    public int Index;
    public KeyCode[] Keys;

    public KeyMap(KeyAction action, int index = -1)
    {
        Action = action;
        Index = index;
        Keys = new KeyCode[GlobalConstants.ACTION_KEY_COUNT];
        for (int i = 0; i < Keys.Length; i++)
        {
            Keys[i] = KeyCode.None;
        }
    }

    public KeyMap(KeyMap source, bool defaultIndex = true)//, float value)
    {
        Action = source.Action;
        Index = defaultIndex? -1 : source.Index;
        Keys = new KeyCode[GlobalConstants.ACTION_KEY_COUNT];
        for(int i = 0; i < source.Keys.Length; i++)
        {
            try { Keys[i] = source.Keys[i]; }
            catch { Keys[i] = KeyCode.None; }
        }
    }
}
public enum KeyState
{
    DOWN,
    PRESSED,
    UP
}
public enum KeyAction
{
    LEFT,
    RIGHT,
    FORWARD,
    BACKWARD,
    SPRINT,
    JUMP,
    HOTBAR,

    PAUSE,
    HOME_TP,
    TELEPORT,
    TOG_PARTY,
    TOG_CHAR,
    TOG_PAWN,
    TOG_CAM_MODE,

    INTERACT,
    CYCLE_TARGETS,
    CHARACTER,
    SKILLS,
    CAM_LOOK,
    CAM_RESET
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

    public void GenerateKeyMap()
    {
        Map = new KeyMap[Default.Length + CharacterMath.HOT_BAR_SLOTS];

        for (int i = 0; i < Default.Length; i++)
            Map[i] = new KeyMap(Default[i]);

        for (int i = 0; i < CharacterMath.HOT_BAR_SLOTS; i++)
        {
            KeyMap hotMap = new KeyMap(KeyAction.HOTBAR, i);
            if (i < 10)
            {
                hotMap.Keys[0] = KeyCode.Alpha0 + i;
                hotMap.Keys[1] = KeyCode.Keypad0 + i;
            }
            Map[i + Default.Length] = hotMap;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
