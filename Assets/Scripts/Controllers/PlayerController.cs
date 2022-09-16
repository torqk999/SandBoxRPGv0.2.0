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

    //public Interaction CurrentInteraction;
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

    [Header("Debugging")]
    public int CurrentTargetPartyIndex;
    public int CurrentTargetCharacterIndex;
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
        if (targetPawn == null || !targetPawn.Root.gameObject.activeSelf)
        {
            Debug.Log("PawnMan failed possession");
            return false;
        }

        oldRotation = GameState.GameCamera.transform.parent.rotation;
        oldPosition = GameState.GameCamera.transform.parent.position;
        GameState.pController.CurrentPawn = targetPawn;
        GameState.pController.CurrentCharacter = targetPawn as Character;
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
    bool CheckAction(KeyAction action, KeyState state = KeyState.DOWN, int index = -1)
    {
        KeyMap? targetMap = Array.Find(GameState.KeyMap.Map, x => x.Action == action && x.Index == index);
        if (!targetMap.HasValue)
            return false;

        //Debug.Log($"{targetMap.Value.Action} : {targetMap.Value.Index}");
        //Debug.Log($"{targetMap.Value.Keys == null}");

        for (int i = 0; i < targetMap.Value.Keys.Length; i++)
        {
            if (targetMap.Value.Keys[i] == KeyCode.None)
                continue;

            switch (state)
            {
                case KeyState.DOWN:
                    if (Input.GetKeyDown(targetMap.Value.Keys[i]))
                        return true;
                    break;

                case KeyState.PRESSED:
                    if (Input.GetKey(targetMap.Value.Keys[i]))
                        return true;
                    break;

                case KeyState.UP:
                    if (Input.GetKeyUp(targetMap.Value.Keys[i]))
                        return true;
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

        if (CurrentCharacter != null)
            UpdateCharacterInput();

        if (!bIsInPlay)
            return;

        float x = MouseAxisScale * Input.GetAxisRaw("Mouse X");
        float y = MouseAxisScale * Input.GetAxisRaw("Mouse Y");
        float z = Input.GetAxis("Roll") * RollScale;

        // Looking
        UpdateCamera(ref x, ref y, ref z);

        if (CurrentPawn == null ||
            !CurrentPawn.bControllable)
            return;

        switch (CurrentControlMode)
        {
            case ControlMode.FLIGHT:
                FlightControl(ref x, ref y, ref z);
                break;

            case ControlMode.TACTICAL:
                TacticalControl(ref x, ref y, ref z);
                break;

            case ControlMode.PERSON:
                PersonControl(ref x, ref y, ref z);
                break;
        }
    }
    void UpdateCharacterInput()
    {
        if (CurrentCharacter.bControllable && CheckAction(KeyAction.INTERACT))
        {
            CurrentCharacter.SwapInteractions();
            //CurrentCharacter.RemoveInteraction(CurrentCharacter.CurrentTargetInteraction);
            /*Debug.Log("Interacting");
            if (CurrentCharacter.CurrentTargetInteraction != null &&
                CurrentCharacter.CurrentTargetInteraction is Character)
                CurrentCharacter.CurrentTargetInteraction.Interact();
            else
                ClearTarget();*/
        }

        if (CheckAction(KeyAction.CHARACTER))
            ToggleCharacterPage(CharPage.Character);

        if (CheckAction(KeyAction.SKILLS))
            ToggleCharacterPage(CharPage.Skills);

        if (CheckAction(KeyAction.CYCLE_TARGETS))
            CycleCharacterTargets();
    }
    void FlightControl(ref float x, ref float y, ref float z)
    {
        // Actions
        if (Input.GetButton("Focus") && Focus != null)
            CurrentPawn.Root.LookAt(Focus, Vector3.up);

        if (!bIsInControl)
            return;

        // Looking
        if (!CheckAction(KeyAction.CAM_LOOK, KeyState.PRESSED))
            CurrentPawn.Root.Rotate(-y, x, z, Space.Self);

        if (CurrentPawn.RigidBody == null)
            return;

        // Movement
        CurrentPawn.RigidBody.AddForce(Input.GetAxis("Forward") * CurrentPawn.Root.forward * KeyAxisScale, ForceMode.Impulse);
        CurrentPawn.RigidBody.AddForce(Input.GetAxis("Right") * CurrentPawn.Root.right * KeyAxisScale, ForceMode.Impulse);
        CurrentPawn.RigidBody.AddForce(Input.GetAxis("Up") * CurrentPawn.Root.transform.up * KeyAxisScale, ForceMode.Impulse);

        if (Input.GetButton("Break"))
        {
            CurrentPawn.RigidBody.velocity = new Vector3(0, 0, 0);
            CurrentPawn.RigidBody.angularVelocity = new Vector3(0, 0, 0);
        }
    }
    void TacticalControl(ref float x, ref float y, ref float z)
    {
        // Looking
        if (Input.GetMouseButton(0))
        {

            CurrentPawn.Root.Rotate(-y, 0, 0, Space.Self);
            CurrentPawn.Root.Rotate(0, x, 0, Space.World);
        }

        // Movement
        CurrentPawn.Root.position += Input.GetAxis("Forward") * CurrentPawn.Root.forward * KeyAxisScale;
        CurrentPawn.Root.position += Input.GetAxis("Right") * CurrentPawn.Root.right * KeyAxisScale;
        CurrentPawn.Root.position += Input.GetAxis("Up") * CurrentPawn.Root.up * KeyAxisScale;

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
    void PersonControl(ref float x, ref float y, ref float z)
    {
        // Turning
        if (!(CheckAction(KeyAction.CAM_LOOK, KeyState.PRESSED) && bCameraLookEnabled))
            CurrentPawn.Root.Rotate(0, x, 0, Space.World);

        // Action Bar
        for (int i = 0; i < CharacterMath.ABILITY_SLOTS; i++)
            if (CheckAction(KeyAction.HOTBAR, KeyState.DOWN, i))
                AttemptAction(i);

        /* Click
        if (Input.GetMouseButtonDown(0))
            AttemptAction(10);
        if (Input.GetMouseButtonDown(1))
            AttemptAction(11);*/

        // Movement
        if (CurrentPawn.RigidBody == null
            || !CurrentPawn.bIsGrounded
            || !CurrentPawn.bControllable
            || !CurrentCharacter.bIsAlive/*!GameState.CharacterMan.PullCurrentCharacter().bIsAlive*/
            || CurrentCharacter.CheckCCstatus(CCstatus.IMMOBILE))
            return;

        if (CheckAction(KeyAction.JUMP))
            CurrentPawn.RigidBody.AddForce(JumpForce * CurrentPawn.Root.up, ForceMode.Impulse);

        // Max Velocity
        if (CurrentPawn == CurrentCharacter && CurrentPawn.RigidBody.velocity.magnitude >= CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])
            return;

        float forceScale = (1 - (CurrentPawn.RigidBody.velocity.magnitude / CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED]));

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

        CurrentPawn.RigidBody.AddForce(forward * CurrentPawn.Root.forward * KeyAxisScale * forceScale, ForceMode.Impulse);
        CurrentPawn.RigidBody.AddForce(right * CurrentPawn.Root.right * KeyAxisScale * forceScale, ForceMode.Impulse);

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
        CurrentPawn.Root.position = CurrentPawn.DefPos;
        CurrentPawn.Root.rotation = Quaternion.Euler(CurrentPawn.DefRot);

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
        CurrentPawn.Root.rotation = selection.CameraSnapPoint.rotation;
        CurrentPawn.Root.position = selection.CameraSnapPoint.position;
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

        if (GameState.CharacterMan.CharacterPool.Count == 0)
            return;

        int index = (CurrentCharacter.CurrentTargetCharacter == null) ? 0 : GameState.CharacterMan.CharacterPool.FindIndex(x => x == CurrentCharacter.CurrentTargetCharacter) + 1;
        //index = (index == -1) ? 0 : index;
        index = (index >= GameState.CharacterMan.CharacterPool.Count) ? 0 : index;

        CurrentCharacter.CurrentTargetCharacter = GameState.CharacterMan.CharacterPool[index];
        //Debug.Log(GameState.CharacterMan.CharacterPool.FindIndex(x => x == CurrentCharacter.Target));

        UpdateInteractionTarget(CurrentCharacter.CurrentTargetCharacter);
    }
    void ClearTarget()
    {
        CurrentCharacter.CurrentTargetCharacter = null;
        UpdateInteractionTarget();

        CurrentTargetCharacterIndex = -1;
        CurrentTargetPartyIndex = -1;
    }
    void AttemptAction(int abilityIndex)
    {
        Debug.Log("Attempting");
        GameState.CharacterMan.AttemptAbility(abilityIndex, CurrentCharacter);
    }
    public void UpdateInteractionTarget(Interaction target = null)
    {
        CurrentCharacter.CurrentTargetInteraction = target;
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
    void UpdateBoomClipping()
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
    void UpdateCamera(ref float x, ref float y, ref float z)
    {
        UpdateZoom();

        if (CheckAction(KeyAction.CAM_RESET))
            CameraRealign();

        bool looking = CheckAction(KeyAction.CAM_LOOK, KeyState.PRESSED) && bCameraLookEnabled;

        switch (CurrentCameraMode)
        {
            case CameraMode.CHASE:
                if (!(CurrentControlMode == ControlMode.FLIGHT &&
                    !looking))
                    CurrentPawn.Boom.Rotate(-y, 0, 0, Space.Self);
                if (looking)
                    CurrentPawn.Boom.Rotate(0, x, 0, Space.World);
                break;

            case CameraMode.FIXED:
                if (!looking)
                    break;
                FixedXY.x += x;
                FixedXY.y += y;
                CurrentPawn.Boom.localRotation = Quaternion.Euler(FixedXY.y, FixedXY.x, 0);
                break;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        CurrentTargetPartyIndex = -1;
        CurrentTargetCharacterIndex = -1;
    }
    // Update is called once per frame
    void Update()
    {
        UpdatePawnInput();
        UpdateBoomClipping();
        LerpCam();

        if (CurrentCharacter != null &&
            CurrentCharacter.bControllable)
            UpdateCharacterAnimationState();
    }
}
