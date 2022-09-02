using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlMode
{
    FLIGHT,
    TACTICAL,
    PERSON
}
public enum TacticalMode
{
    SELECT,
    BUILD,
    DELETE
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

    HotBar0,
    HotBar1,
    HotBar2,
    HotBar3,
    HotBar4,
    HotBar5,
    HotBar6,
    HotBar7,
    HotBar8,
    HotBar9,

    PAUSE,
    HOME_TP,
    TELEPORT,
    TOG_PARTY,
    TOG_CHAR,
    TOG_PAWN,
    TOG_CAM_MODE,

    INTERACT,
    CYCLE_TARGETS,
    INVENTORY,
    EQUIPMENT,
    SKILLS,
    STRATEGY,
}

public class PlayerController : MonoBehaviour
{
    //[Header("Player Stats")]
    //public float CollisionVelocityTolerance;
    //public float CollisionDamageScalar;

    #region VARS
    [Header("Object References")]
    public GameState GameState;
    public Transform Focus;

    [Header("Input Preferances")]
    public float KeyAxisScale;
    public float MouseAxisScale;
    public float RollScale;

    [Header("Player Logic")]
    public bool bIsInPlay = true;
    public bool bIsInControl = true;
    public int PawnIndex;

    public Rigidbody PhysicsObject;
    public Character CurrentCharacter;
    public GenericContainer targetContainer;
    public Vector3 PausedFlightVector;

    public ControlMode CurrentControlMode;
    public TacticalMode CurrentTacticalMode = TacticalMode.SELECT;

    #endregion

    #region PUBLIC

    public void UpdateCollide(float velocity)
    {

    }
    public void TogglePause(bool toggle)
    {
        GameState.GamePause(toggle);
        bIsInPlay = !toggle;
        CursorToggle((toggle) ? true : (CurrentControlMode != ControlMode.TACTICAL) ? false : true);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
    public void ToggleCharacterPage(CharPage page)
    {
        switch (page)
        {
            case CharPage.Inventory: // Inventory
                GameState.bInventoryOpen = !GameState.UIman.Inventory.activeSelf;
                GameState.UIman.UpdateInventory();
                break;
            case CharPage.Equipment: // Equip
                GameState.bEquipmentOpen = !GameState.UIman.Equipment.activeSelf;
                GameState.UIman.UpdateEquipment();
                break;
            case CharPage.Container: // Container
                GameState.bContainerOpen = !GameState.UIman.Container.activeSelf;
                GameState.UIman.UpdateContainer();
                break;
            case CharPage.Skills: // Skills
                GameState.bSkillsOpen = !GameState.UIman.Skills.activeSelf;
                GameState.UIman.UpdateSkills();
                break;
            case CharPage.Strategy: // Strategy
                GameState.bStrategyOpen = !GameState.UIman.Strategy.activeSelf;
                GameState.UIman.UpdateStrategy();
                break;
        }

        TogglePlayStatus();
    }
    public void InitialPawnControl()
    {
        PawnIndex = -1;
        Debug.Log($"Initial Pawn Index: {PawnIndex}");
        Debug.Log($"Initial PawnControl Success: {PossessPawn()}");

        if (GameState.PawnMan.CurrentPawn == null)
            return;

        SnapPawnOptions();
        Teleport();
    }

    #endregion

    #region INPUT
    void TogglePlayStatus()
    {
        bIsInPlay = (
        !GameState.bInventoryOpen &&
        !GameState.bEquipmentOpen &&
        //!GameState.bContainerOpen &&
        !GameState.bSkillsOpen);
        CursorToggle(!bIsInPlay);
    }
    bool CheckAction(KeyAction action, KeyState state = KeyState.DOWN)
    {
        for (int i = 0; i < GameState.KeyMap.Map[(int)action].Keys.Length; i++)
        {
            switch(state)
            {
                case KeyState.DOWN:
                    if (Input.GetKeyDown(GameState.KeyMap.Map[(int)action].Keys[i]))
                        return true;
                    break;

                case KeyState.PRESSED:
                    if (Input.GetKey(GameState.KeyMap.Map[(int)action].Keys[i]))
                        return true;
                    break;

                case KeyState.UP:
                    if (Input.GetKeyUp(GameState.KeyMap.Map[(int)action].Keys[i]))
                        return true;
                    break;
            }
        }

        return false;
    }
    void UpdateInput()
    {
        if (CheckAction(KeyAction.PAUSE) &&
            GameState.UIman.CurrentMenu != GameMenu.KEY_MAP)
            TogglePause(!GameState.bPause);

        if (GameState.bPause)
            return;

        if (CheckAction(KeyAction.TOG_PAWN))
            ChangePawns();

        if (CheckAction(KeyAction.TOG_CAM_MODE))
            GameState.PawnMan.ToggleCameraMode();

        if (CheckAction(KeyAction.TOG_CHAR))
            ChangeCharacters();

        if (CheckAction(KeyAction.TOG_PARTY))
            ChangeParties();

        if (CheckAction(KeyAction.HOME_TP))
            JumpHome();

        if (CheckAction(KeyAction.TELEPORT))
            Teleport();

        if (CurrentCharacter != null)
        {
            if (CheckAction(KeyAction.INVENTORY))
                ToggleCharacterPage(CharPage.Inventory);

            if (CheckAction(KeyAction.EQUIPMENT))
                ToggleCharacterPage(CharPage.Equipment);

            if (CheckAction(KeyAction.SKILLS))
                ToggleCharacterPage(CharPage.Skills);

            if (CheckAction(KeyAction.STRATEGY))
                ToggleCharacterPage(CharPage.Strategy);
        }

        //if (Input.GetButtonDown("Interact"))
        if (CheckAction(KeyAction.INTERACT))
        {
            if (GameState.PawnMan.CurrentPawn.CurrentInteraction != null)
                GameState.PawnMan.CurrentPawn.CurrentInteraction.Interact();
            else
                ClearTarget();
        }

        //if (Input.GetButtonDown("CycleTargets"))
        if (CheckAction(KeyAction.CYCLE_TARGETS))
            CycleCharacterTargets();

        if (!bIsInPlay || GameState.PawnMan.CurrentPawn == null)
            return;

        switch (CurrentControlMode)
        {
            case ControlMode.FLIGHT:
                FlightControl();
                break;

            case ControlMode.TACTICAL:
                TacticalControl();
                break;

            case ControlMode.PERSON:
                PersonControl();
                break;
        }
    }
    void FlightControl()
    {
        // Actions
        if (Input.GetButton("Focus") && Focus != null)
            GameState.PawnMan.CurrentPawn.Source.LookAt(Focus, Vector3.up);

        if (!bIsInControl)
            return;

        // Looking
        //if (Input.GetMouseButton(0))
        {
            float x = GameState.PawnMan.CurrentPawn.MouseAxisScale * Input.GetAxisRaw("Mouse X");
            float y = GameState.PawnMan.CurrentPawn.MouseAxisScale * Input.GetAxisRaw("Mouse Y");
            float z = Input.GetAxis("Roll") * GameState.PawnMan.CurrentPawn.RollScale;

            GameState.PawnMan.CurrentPawn.Source.Rotate(-y, x, z, Space.Self);
        }

        if (PhysicsObject == null)
            return;

        // Movement
        PhysicsObject.AddForce(Input.GetAxis("Forward") * GameState.PawnMan.CurrentPawn.Source.forward * GameState.PawnMan.CurrentPawn.KeyAxisScale, ForceMode.Impulse);
        PhysicsObject.AddForce(Input.GetAxis("Right") * GameState.PawnMan.CurrentPawn.Source.right * GameState.PawnMan.CurrentPawn.KeyAxisScale, ForceMode.Impulse);
        PhysicsObject.AddForce(Input.GetAxis("Up") * GameState.PawnMan.CurrentPawn.Source.transform.up * GameState.PawnMan.CurrentPawn.KeyAxisScale, ForceMode.Impulse);

        if (Input.GetButton("Break"))
        {
            PhysicsObject.velocity = new Vector3(0, 0, 0);
            PhysicsObject.angularVelocity = new Vector3(0, 0, 0);
        }
    }
    void TacticalControl()
    {
        // Looking
        if (Input.GetMouseButton(0))
        {
            float x = GameState.PawnMan.CurrentPawn.MouseAxisScale * Input.GetAxisRaw("Mouse X");
            float y = GameState.PawnMan.CurrentPawn.MouseAxisScale * Input.GetAxisRaw("Mouse Y");

            GameState.PawnMan.CurrentPawn.Source.Rotate(-y, 0, 0, Space.Self);
            GameState.PawnMan.CurrentPawn.Source.Rotate(0, x, 0, Space.World);
        }

        // Movement
        GameState.PawnMan.CurrentPawn.Source.position += Input.GetAxis("Forward") * GameState.PawnMan.CurrentPawn.Source.forward * GameState.PawnMan.CurrentPawn.KeyAxisScale;
        GameState.PawnMan.CurrentPawn.Source.position += Input.GetAxis("Right") * GameState.PawnMan.CurrentPawn.Source.right * GameState.PawnMan.CurrentPawn.KeyAxisScale;
        GameState.PawnMan.CurrentPawn.Source.position += Input.GetAxis("Up") * GameState.PawnMan.CurrentPawn.Source.up * GameState.PawnMan.CurrentPawn.KeyAxisScale;

        if (Input.GetButtonDown("TacticalMode"))
            TacticalModeChange();

        if (Input.GetButtonDown("BuildMode"))
            GameState.Builder.BuildModeChange();

        // Mode
        switch (CurrentTacticalMode)
        {
            case TacticalMode.SELECT:

                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray;
                    if (GameState.PawnMan.ReturnCameraRay(out ray))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                            FocusSelect(hit);
                    }
                }

                break;

            case TacticalMode.DELETE:

                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray;
                    if (GameState.PawnMan.ReturnCameraRay(out ray))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                            DeleteSelect(hit);
                    }
                }

                break;

            case TacticalMode.BUILD:

                float scroll = Input.GetAxis("MouseScrollWheel");

                if (scroll != 0)
                    GameState.Builder.BuilderScrollWheel(scroll);

                if (Input.GetMouseButtonDown(0))
                    GameState.Builder.BuildModeClick();

                break;
        }
    }
    void PersonControl()
    {
        if (!bIsInControl)
            return;

        // Looking
        //if (Input.GetMouseButton(0))
        {
            float x = GameState.PawnMan.CurrentPawn.MouseAxisScale * Input.GetAxisRaw("Mouse X");
            float y = GameState.PawnMan.CurrentPawn.MouseAxisScale * Input.GetAxisRaw("Mouse Y");
            float z = Input.GetAxis("Roll") * GameState.PawnMan.CurrentPawn.RollScale;

            if (GameState.PawnMan.CurrentCameraMode != CameraMode.FIXED)
                GameState.PawnMan.CurrentPawn.Boom.Rotate(-y, 0, 0, Space.Self);
            GameState.PawnMan.CurrentPawn.Source.Rotate(0, x, 0, Space.World);
        }

        // Action Bar
        for (int i = (int)KeyAction.HotBar0; i < 10 + (int)KeyAction.HotBar0; i++)
            if (CheckAction((KeyAction)i))
            //if (Input.GetKeyDown((KeyCode)(i + 48)))
                AttemptAction(i);

        // Click
        if (Input.GetMouseButtonDown(0))
            AttemptAction(10);
        if (Input.GetMouseButtonDown(1))
            AttemptAction(11);

        // Movement
        if (PhysicsObject == null 
            || !GameState.PawnMan.CurrentPawn.bIsGrounded 
            || !GameState.PawnMan.CurrentPawn.bControllable 
            || !GameState.CharacterMan.PullCurrentCharacter().bIsAlive)
            return;

        //if (Input.GetButtonDown("Jump"))
        if (CheckAction(KeyAction.JUMP))
            PhysicsObject.AddForce(GameState.PawnMan.CurrentPawn.JumpForce * GameState.PawnMan.CurrentPawn.Source.up, ForceMode.Impulse);

        if (GameState.PawnMan.CurrentPawn == GameState.Controller.CurrentCharacter && PhysicsObject.velocity.magnitude >= GameState.Controller.CurrentCharacter.MaximumStatValues.SPEED)
            return;
        float forceScale = (1 - (PhysicsObject.velocity.magnitude/ GameState.Controller.CurrentCharacter.MaximumStatValues.SPEED));

        float forward = 0;
        if (CheckAction(KeyAction.FORWARD))
            forward += 1;
        if (CheckAction(KeyAction.BACKWARD))

        PhysicsObject.AddForce(Input.GetAxis("Forward") * GameState.PawnMan.CurrentPawn.Source.forward * GameState.PawnMan.CurrentPawn.KeyAxisScale * forceScale, ForceMode.Impulse);
        PhysicsObject.AddForce(Input.GetAxis("Right") * GameState.PawnMan.CurrentPawn.Source.right * GameState.PawnMan.CurrentPawn.KeyAxisScale * forceScale, ForceMode.Impulse);
    }
    #endregion

    #region ACTIONS
    void ChangePawns()
    {
        PawnIndex++;
        PawnIndex = (PawnIndex >= GameState.PawnMan.PlayerPawns.Length) ? -1 : PawnIndex;
        PossessPawn();
        SnapPawnOptions();
    }
    void ChangeCharacters()
    {
        GameState.CharacterMan.ToggleCharacter();
        PossessPawn();
        SnapPawnOptions();
    }
    void ChangeParties()
    {
        GameState.CharacterMan.ToggleParty();
        PossessPawn();
        SnapPawnOptions();
    }
    bool PossessPawn()
    {
        if (PawnIndex > -1) // Other Pawns such as tactical or flight
        {
            if (!GameState.PawnMan.PossessPawn(GameState.PawnMan.PlayerPawns[PawnIndex]))
            {
                Debug.Log("Controller failed to possess pure pawn");
                return false;
            }
        }

        else if (!GameState.PawnMan.PossessPawn(GameState.CharacterMan.PullCurrentCharacter()))
        {
            Debug.Log("Controller failed to possess character pawn");
            return false;
        }

        //Debug.Log("successful possession");
        SnapPawnOptions();
        return true;
    }
    void SnapPawnOptions()
    {
        CurrentControlMode = GameState.PawnMan.CurrentPawn.ControlMode;
        CursorToggle((CurrentControlMode == ControlMode.TACTICAL) ? true : false);
        PhysicsObject = GameState.PawnMan.CurrentPawn.RigidBody;
    }
    void CursorToggle(bool toggle)
    {
        if (toggle)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = toggle;
    }
    void CycleCharacterTargets()
    {
        //Debug.Log("cycling");
        //Debug.Log(GameState.CharacterMan.CharacterPool.FindIndex(x => x == CurrentCharacter.Target));

        int index = (CurrentCharacter.Target == null) ? 0 : GameState.CharacterMan.CharacterPool.FindIndex(x => x == CurrentCharacter.Target) + 1;
        //index = (index == -1) ? 0 : index;
        index = (index >= GameState.CharacterMan.CharacterPool.Count) ? 0 : index;
        if (GameState.CharacterMan.CharacterPool.Count == 0)
            return;
        CurrentCharacter.Target = GameState.CharacterMan.CharacterPool[index];
        //GameState.UIman.UpdateInteractionHUD()
    }
    void ClearTarget()
    {
        GameState.Controller.CurrentCharacter.Target = null;
    }
    void AttemptAction(int abilityIndex)
    {
        GameState.CharacterMan.AttemptAbility(abilityIndex, CurrentCharacter);
    }
    void JumpHome()
    {
        GameState.PawnMan.CurrentPawn.Source.position = GameState.PawnMan.CurrentPawn.DefPos;
        GameState.PawnMan.CurrentPawn.Source.rotation = Quaternion.Euler(GameState.PawnMan.CurrentPawn.DefRot);

        if (PhysicsObject != null)
            PhysicsObject.velocity = Vector3.zero;
    }
    void Teleport(Transform location = null)
    {
        if (GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex] == null ||
            GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].Formation == null)
            return;

        GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].Formation.PositionParty(location);
    }
    void TacticalModeChange()
    {
        if (CurrentTacticalMode == TacticalMode.DELETE)
            CurrentTacticalMode = 0;
        else
            CurrentTacticalMode++;

        GameState.Builder.ToggleCast(CurrentTacticalMode == TacticalMode.BUILD);
        GameState.PawnMan.bIsZooming = !(CurrentTacticalMode == TacticalMode.BUILD);
    }
    void FocusSelect(RaycastHit hit)
    {
        if (GameState.Gravity == null)
            return;

        PointMass selection = GameState.Gravity.PointMasses.Find(x => x.MassTransform == hit.transform);
        if (selection == null)
            return;
        Debug.Log(selection.Name);
        if (selection.CameraSnapPoint == null)
            return;

        Focus = selection.MassTransform;
        GameState.PawnMan.CurrentPawn.Source.rotation = selection.CameraSnapPoint.rotation;
        GameState.PawnMan.CurrentPawn.Source.position = selection.CameraSnapPoint.position;
    }
    void DeleteSelect(RaycastHit hit)
    {
        if (hit.transform.tag == GameState.Builder.BuilderTag)
            Destroy(hit.transform.gameObject);
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }
}
