using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    TITLE,
    PLAY,
    COMBAT,
    MENU,
    CINEMATIC
}

public class GameState : MonoBehaviour
{
    [Header("Essentials")]
    public Camera GameCamera;

    [Header("Controllers")]
    public PlayerController Controller;
    public SimpleAIcontroller testAI;
    public SimpleAIpathingController testPath;
    public NavMesh NavMesh;
    public KeyMapper KeyMap;

    [Header("Managers")]
    public SceneManager SceneMan;
    public PawnManager PawnMan;
    public CharacterManager CharacterMan;
    public UIManager UIman;

    [Header("Test References")]
    public SimpleWorldBuilder testBuilder;
    public GravityController Gravity;
    public ObjectBuilder Builder;

    [Header("Game Logic")]
    //public GameMode GameMode;
    public bool bDebugEffects;
    public bool bGravity;
    public bool bPause;
    public bool bPartyChanged = true;

    [Header("UI State Logic")]
    public bool bPauseMenuOpen;
    public bool bHUDactive;
    public bool bGameMenuOpen;
    //public bool bInventoryOpen;
    //public bool bEquipmentOpen;
    //public bool bLootingOpen;
    //public bool bSkillsOpen;
    //public bool bStrategyOpen;
    
    [Header("Generated Indices")]
    public int EQUIPMENT_INDEX;

    [Header("Dynamic References")]
    public List<Pawn> RigidBodyPawns;

    public void GamePause(bool toggle)
    {
        bPause = toggle;
        if (bPause)
        {
            UIman.OldPage = UIman.CurrentPage;
            UIman.CurrentPage = UIman.OldPage;
        }
        else
            UIman.CurrentPage = UIman.OldPage;
            
        UpdateHUDstate();
        UIman.UpdateGameMenuCanvasState(UIman.CurrentPage);
        UIman.PauseMenuCanvasNavigation((toggle) ? 0 : -1);
        UpdateRigidBodyPawns();
    }
    public void InteractWithContainer(GenericContainer container)
    {
        Controller.targetContainer = container;
        Controller.ToggleCharacterPage(CharPage.Looting);
    }
    void UpdateHUDstate()
    {
        bHUDactive = !(bPause);// || bCharacterMenuOpen);
        UIman.HUDcanvas.gameObject.SetActive(bHUDactive);
    }
    void UpdateRigidBodyPawns()
    {
        foreach (Pawn pawn in RigidBodyPawns)
        {
            pawn.CurrentVelocity = (bPause) ? pawn.RigidBody.velocity : pawn.CurrentVelocity;
            pawn.RigidBody.velocity = (bPause) ? Vector3.zero : pawn.CurrentVelocity;
            pawn.RigidBody.useGravity = (bPause) ? false : (pawn.bUsesGravity) ? true : false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (bPause)
            return;

        UpdateHUDstate();

        if (bGravity)
            Gravity.UpdateGravity();
    }
}
