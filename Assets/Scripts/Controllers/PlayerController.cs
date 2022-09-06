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
    CHARACTER,
    EQUIPMENT,
    SKILLS,
    STRATEGY,
}

public class PlayerController : CharacterController
{
    #region VARS
    [Header("Input Preferances")]
    [Header("===PLAYER CONTROL===")]
    public float KeyAxisScale;
    public float MouseAxisScale;
    public float RollScale;
    public float JumpForce;

    [Header("Player Logic")]
    public bool bIsInPlay = true;
    public bool bIsInControl = true;
    public int PawnIndex;
    public GenericContainer targetContainer;

    public ControlMode CurrentControlMode;
    public TacticalMode CurrentTacticalMode = TacticalMode.SELECT;

    [Header("Camera Logic")]
    public CameraMode CurrentCameraMode = CameraMode.CHASE;
    public bool bIsZooming = true;
    public bool bCameraLookEnabled;
    public float Zoom;
    public Vector2 FixedXY;

    [Header("Lerp Logic")]
    public float LerpTimer = 0;
    public bool bIsCamLerping = false;
    public Quaternion oldRotation;
    public Vector3 oldPosition;
    #endregion

    #region PUBLIC

    public void ToggleCameraMode()
    {
        if (CurrentCameraMode == CameraMode.FIXED)
            CurrentCameraMode = 0;
        else
            CurrentCameraMode++;

        CameraRealign();
    }
    public bool PossessPawn(Pawn targetPawn)
    {
        if (targetPawn == null || !targetPawn.Source.gameObject.activeSelf)
        {
            Debug.Log("PawnMan failed possession");
            return false;
        }

        oldRotation = GameState.GameCamera.transform.parent.rotation;
        oldPosition = GameState.GameCamera.transform.parent.position;
        GameState.Controller.CurrentPawn = targetPawn;
        GameState.Controller.CurrentCharacter = targetPawn as Character;
        GameState.GameCamera.transform.parent = targetPawn.Socket;
        LerpTimer = 0;
        bIsCamLerping = true;

        return true;
    }
    public bool ReturnCameraRay(out Ray ray)
    {
        ray = new Ray();
        if (GameState.GameCamera == null)
            return false;

        ray = GameState.GameCamera.ScreenPointToRay(Input.mousePosition);
        return true;
    }
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
        /*
        switch (page)
        {
            case CharPage.Character: // Character
                GameState.bInventoryOpen = !GameState.UIman.Inventory.activeSelf;
                GameState.UIman.UpdateInventory();
                break;

            case CharPage.Equipment: // Equip
                GameState.bEquipmentOpen = !GameState.UIman.Equipment.activeSelf;
                GameState.UIman.UpdateEquipment();
                break;

            case CharPage.Looting: // Container
                GameState.bLootingOpen = !GameState.bLootingOpen;
                GameState.UIman.UpdateContainer();
                break;

            case CharPage.Skills: // Skills
                GameState.bSkillsOpen = !GameState.bSkillsOpen;
                GameState.UIman.UpdateSkills();
                break;

            case CharPage.Strategy: // Strategy
                GameState.bStrategyOpen = !GameState.UIman.Strategy.activeSelf;
                GameState.UIman.UpdateStrategy();
                break;
        }
        */

        GameState.UIman.UpdateGameMenuCanvasState(page);
        TogglePlayStatus();
    }
    public void InitialPawnControl()
    {
        PawnIndex = -1;
        Debug.Log($"Initial Pawn Index: {PawnIndex}");
        Debug.Log($"Initial PawnControl Success: {PossessPawn()}");

        if (CurrentPawn == null)
            return;

        SnapPawnOptions();
        Teleport();
    }

    #endregion

    #region INPUT
    void TogglePlayStatus()
    {
        bIsInPlay = GameState.UIman.CurrentPage == CharPage.None;
        CursorToggle(!bIsInPlay);
    }
    bool CheckAction(KeyAction action, KeyState state = KeyState.DOWN)
    {
        KeyMap? targetMap = Array.Find(GameState.KeyMap.Map, x => x.Action == action);
        if (!targetMap.HasValue)
            return false;

        for (int i = 0; i < targetMap.Value.Keys.Length; i++)
        {
            switch (state)
            {
                case KeyState.DOWN:
                    if (targetMap.Value.Keys[i] != KeyCode.None &&
                        Input.GetKeyDown(targetMap.Value.Keys[i]))
                    {
                        //Debug.Log("Down");
                        return true;
                    }

                    break;

                case KeyState.PRESSED:
                    if (targetMap.Value.Keys[i] != KeyCode.None &&
                        Input.GetKey(targetMap.Value.Keys[i]))
                    {
                        //Debug.Log("Pressed");
                        return true;
                    }
                    break;

                case KeyState.UP:
                    if (targetMap.Value.Keys[i] != KeyCode.None &&
                        Input.GetKeyUp(targetMap.Value.Keys[i]))
                    {
                        //Debug.Log("Up");
                        return true;
                    }

                    break;
            }
        }

        return false;
    }
    void UpdatePawnInput()
    {
        if (CheckAction(KeyAction.PAUSE) &&
            GameState.UIman.CurrentMenu != GameMenu.KEY_MAP)
            TogglePause(!GameState.bPause);

        if (GameState.bPause)
            return;

        if (CheckAction(KeyAction.TOG_PAWN))
            ChangePawns();

        if (CheckAction(KeyAction.TOG_CAM_MODE))
            ToggleCameraMode();

        if (CheckAction(KeyAction.TOG_CHAR))
            ChangeCharacters();

        if (CheckAction(KeyAction.TOG_PARTY))
            ChangeParties();

        if (CheckAction(KeyAction.HOME_TP))
            JumpHome();

        if (CheckAction(KeyAction.TELEPORT))
            Teleport();

        if (CheckAction(KeyAction.INTERACT))
        {
            if (CurrentPawn.CurrentInteraction != null)
                CurrentPawn.CurrentInteraction.Interact();
            else
                ClearTarget();
        }

        if (CurrentCharacter != null)
            UpdateCharacterInput();

        if (!bIsInPlay || CurrentPawn == null)
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
    void UpdateCharacterInput()
    {
        if (CheckAction(KeyAction.CHARACTER))
            ToggleCharacterPage(CharPage.Character);

        //if (CheckAction(KeyAction.EQUIPMENT))
        //    ToggleCharacterPage(CharPage.Equipment);

        if (CheckAction(KeyAction.SKILLS))
            ToggleCharacterPage(CharPage.Skills);

        //if (CheckAction(KeyAction.STRATEGY))
        //    ToggleCharacterPage(CharPage.Strategy);

        if (CheckAction(KeyAction.CYCLE_TARGETS))
            CycleCharacterTargets();
    }
    void FlightControl()
    {
        // Actions
        if (Input.GetButton("Focus") && Focus != null)
            CurrentPawn.Source.LookAt(Focus, Vector3.up);

        if (!bIsInControl)
            return;

        // Looking
        //if (Input.GetMouseButton(0))
        {
            float x = MouseAxisScale * Input.GetAxisRaw("Mouse X");
            float y = MouseAxisScale * Input.GetAxisRaw("Mouse Y");
            float z = Input.GetAxis("Roll") * RollScale;

            CurrentPawn.Source.Rotate(-y, x, z, Space.Self);
        }

        if (CurrentPawn.RigidBody == null)
            return;

        // Movement
        CurrentPawn.RigidBody.AddForce(Input.GetAxis("Forward") * CurrentPawn.Source.forward * KeyAxisScale, ForceMode.Impulse);
        CurrentPawn.RigidBody.AddForce(Input.GetAxis("Right") * CurrentPawn.Source.right * KeyAxisScale, ForceMode.Impulse);
        CurrentPawn.RigidBody.AddForce(Input.GetAxis("Up") * CurrentPawn.Source.transform.up * KeyAxisScale, ForceMode.Impulse);

        if (Input.GetButton("Break"))
        {
            CurrentPawn.RigidBody.velocity = new Vector3(0, 0, 0);
            CurrentPawn.RigidBody.angularVelocity = new Vector3(0, 0, 0);
        }
    }
    void TacticalControl()
    {
        // Looking
        if (Input.GetMouseButton(0))
        {
            float x = MouseAxisScale * Input.GetAxisRaw("Mouse X");
            float y = MouseAxisScale * Input.GetAxisRaw("Mouse Y");

            CurrentPawn.Source.Rotate(-y, 0, 0, Space.Self);
            CurrentPawn.Source.Rotate(0, x, 0, Space.World);
        }

        // Movement
        CurrentPawn.Source.position += Input.GetAxis("Forward") * CurrentPawn.Source.forward * KeyAxisScale;
        CurrentPawn.Source.position += Input.GetAxis("Right") * CurrentPawn.Source.right * KeyAxisScale;
        CurrentPawn.Source.position += Input.GetAxis("Up") * CurrentPawn.Source.up * KeyAxisScale;

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
                    if (ReturnCameraRay(out ray))
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
                    if (ReturnCameraRay(out ray))
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
            float x = MouseAxisScale * Input.GetAxisRaw("Mouse X");
            float y = MouseAxisScale * Input.GetAxisRaw("Mouse Y");
            float z = Input.GetAxis("Roll") * RollScale;

            if (CurrentCameraMode != CameraMode.FIXED)
                CurrentPawn.Boom.Rotate(-y, 0, 0, Space.Self);
            CurrentPawn.Source.Rotate(0, x, 0, Space.World);
        }

        // Action Bar
        for (int i = (int)KeyAction.HotBar0; i < 10 + (int)KeyAction.HotBar0; i++)
            if (CheckAction((KeyAction)i))
                AttemptAction(i);

        // Click
        if (Input.GetMouseButtonDown(0))
            AttemptAction(10);
        if (Input.GetMouseButtonDown(1))
            AttemptAction(11);

        // Movement
        if (CurrentPawn.RigidBody == null
            || !CurrentPawn.bIsGrounded
            || !CurrentPawn.bControllable
            || !GameState.CharacterMan.PullCurrentCharacter().bIsAlive)
            return;

        if (CheckAction(KeyAction.JUMP))
            CurrentPawn.RigidBody.AddForce(JumpForce * CurrentPawn.Source.up, ForceMode.Impulse);

        if (CurrentPawn == CurrentCharacter && CurrentPawn.RigidBody.velocity.magnitude >= CurrentCharacter.MaximumStatValues.SPEED)
            return;
        float forceScale = (1 - (CurrentPawn.RigidBody.velocity.magnitude / CurrentCharacter.MaximumStatValues.SPEED));

        float forward = 0;
        float right = 0;
        if (CheckAction(KeyAction.FORWARD, KeyState.PRESSED))
        {
            forward += 1;
        }
        if (CheckAction(KeyAction.BACKWARD, KeyState.PRESSED))
        {
            forward -= 1;
        }
        if (CheckAction(KeyAction.RIGHT, KeyState.PRESSED))
        {
            right += 1;
        }
        if (CheckAction(KeyAction.LEFT, KeyState.PRESSED))
        {
            right -= 1;
        }

        CurrentPawn.RigidBody.AddForce(forward * CurrentPawn.Source.forward * KeyAxisScale * forceScale, ForceMode.Impulse);
        CurrentPawn.RigidBody.AddForce(right * CurrentPawn.Source.right * KeyAxisScale * forceScale, ForceMode.Impulse);

        IntentVector.x = right;
        IntentVector.z = forward;
    }
    #endregion

    #region PAWN ACTIONS
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
            if (!PossessPawn(GameState.PawnMan.PlayerPawns[PawnIndex]))
            {
                Debug.Log("Controller failed to possess pure pawn");
                return false;
            }
        }

        else if (!PossessPawn(GameState.CharacterMan.PullCurrentCharacter()))
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
        CurrentControlMode = CurrentPawn.ControlMode;
        CursorToggle((CurrentControlMode == ControlMode.TACTICAL) ? true : false);
        //PhysicsObject = CurrentPawn.RigidBody;
    }
    void CursorToggle(bool toggle)
    {
        if (toggle)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = toggle;
    }
    void JumpHome()
    {
        CurrentPawn.Source.position = CurrentPawn.DefPos;
        CurrentPawn.Source.rotation = Quaternion.Euler(CurrentPawn.DefRot);

        if (CurrentPawn.RigidBody != null)
            CurrentPawn.RigidBody.velocity = Vector3.zero;
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
        bIsZooming = !(CurrentTacticalMode == TacticalMode.BUILD);
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
        CurrentPawn.Source.rotation = selection.CameraSnapPoint.rotation;
        CurrentPawn.Source.position = selection.CameraSnapPoint.position;
    }
    void DeleteSelect(RaycastHit hit)
    {
        if (hit.transform.tag == GameState.Builder.BuilderTag)
            Destroy(hit.transform.gameObject);
    }
    #endregion

    #region CHARACTER ACTIONS
    void CycleCharacterTargets()
    {
        //Debug.Log("cycling");
        //Debug.Log(GameState.CharacterMan.CharacterPool.FindIndex(x => x == CurrentCharacter.Target));
        if (GameState.CharacterMan.CharacterPool.Count == 0)
            return;
        int index = (CurrentCharacter.Target == null) ? 0 : GameState.CharacterMan.CharacterPool.FindIndex(x => x == CurrentCharacter.Target) + 1;
        //index = (index == -1) ? 0 : index;
        index = (index >= GameState.CharacterMan.CharacterPool.Count) ? 0 : index;

        CurrentCharacter.Target = GameState.CharacterMan.CharacterPool[index];
        //GameState.UIman.UpdateInteractionHUD()
    }
    void ClearTarget()
    {
        CurrentCharacter.Target = null;
    }
    void AttemptAction(int abilityIndex)
    {
        GameState.CharacterMan.AttemptAbility(abilityIndex, CurrentCharacter);
    }
    #endregion

    #region CAMERA
    void LerpCam()
    {
        if (!bIsCamLerping)
            return;
        if (GameState.GameCamera.transform.parent == null)
        { bIsCamLerping = false; return; }

        GameState.GameCamera.transform.rotation = Quaternion.Lerp(oldRotation, GameState.GameCamera.transform.parent.rotation, LerpTimer);
        GameState.GameCamera.transform.position = Vector3.Lerp(oldPosition, GameState.GameCamera.transform.parent.position, LerpTimer);
        LerpTimer += GlobalConstants.TIME_SCALE;

        if (LerpTimer >= 1)
            bIsCamLerping = false;
    }
    void UpdateZoom()
    {
        if (!bIsZooming || CurrentPawn == null)
            return;

        Zoom += CurrentPawn.ZoomScale * -Input.mouseScrollDelta.y;
        Zoom = (Zoom < CurrentPawn.ZoomMin) ? CurrentPawn.ZoomMin : Zoom;
        Zoom = (Zoom > CurrentPawn.ZoomMax) ? CurrentPawn.ZoomMax : Zoom;

    }
    void UpdateBoom()
    {
        if (CurrentPawn == null)
            return;

        Vector3 newLocal = CurrentPawn.Socket.localPosition;
        RaycastHit hit;


        if (Physics.Raycast(CurrentPawn.Boom.position, -CurrentPawn.Boom.forward, out hit, Zoom))
        {
            //Debug.Log($"HitName: {hit.collider.name}");
            newLocal.z = -Vector3.Distance(CurrentPawn.Boom.position, hit.point);
        }
        else
            newLocal.z = -Zoom;

        CurrentPawn.Socket.localPosition = newLocal;
    }
    void CameraRealign()
    {
        switch (CurrentCameraMode)
        {
            case CameraMode.CHASE:
                CurrentPawn.Boom.localRotation = Quaternion.Euler(CurrentPawn.DefRot);
                break;

            case CameraMode.FIXED:
                FixedXY = new Vector2(CurrentPawn.DefRot.y, CurrentPawn.DefRot.x);
                break;
        }
    }
    void UpdateCamera()
    {
        if (Input.GetButtonDown("CamReset"))
            CameraRealign();

        // Camera Rotate
        if (!Input.GetButton("CameraLook") || !bCameraLookEnabled)
        {
            GameState.Controller.bIsInControl = true;
            return;
        }

        GameState.Controller.bIsInControl = false;

        float x = MouseAxisScale * Input.GetAxisRaw("Mouse X");
        float y = MouseAxisScale * Input.GetAxisRaw("Mouse Y");
        float z = Input.GetAxis("Roll") * RollScale;

        switch (CurrentCameraMode)
        {
            case CameraMode.CHASE:
                CurrentPawn.Boom.Rotate(-y, x, z, Space.World);
                break;

            case CameraMode.FIXED:
                //CurrentPawn.Source.Rotate(-y, 0, 0, Space.Self);
                //CurrentPawn.Boom.Rotate(0, x, 0, Space.World);
                FixedXY.x += x;
                FixedXY.y += y;
                break;
        }
    }

    void FixedBoom()
    {
        if (CurrentCameraMode != CameraMode.FIXED)
            return;

        CurrentPawn.Boom.rotation = Quaternion.Euler(CurrentPawn.Source.rotation.eulerAngles.x + FixedXY.y, CurrentPawn.Source.rotation.eulerAngles.y + FixedXY.x, 0);
    }
    #endregion

    void UpdateCurrentTarget()
    {
        if (CurrentCharacter != null &&
            CurrentCharacter.Target != null)
            GameState.UIman.UpdateInteraction();
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        UpdateZoom();
        UpdateBoom();
        UpdatePawnInput();
        UpdateCamera();
        UpdateCurrentTarget();
        LerpCam();
        FixedBoom();

        UpdateCharacterAnimationState();
    }
}
