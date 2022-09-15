using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AIoperationType
{
    WAIT,
    ROTATE,
    WALK
}
[System.Serializable]
public struct AIoperation
{
    public AIoperationType Type;
    public float duration;
}
[System.Serializable]
public struct AIstaticSequence
{
    public AIoperation[] Operations;
}
public class SimpleAIcontroller : CharacterController
{
    [Header("Links and stuff")]
    [Header("===AI CONTROL===")]
    public SimpleAIpathingController Pathing;
    public AIstaticSequence myStaticSequence;
    public Transform[] BoundaryTransforms;
    public Strategy Strategy;

    [Header("TOUCH DA BOOLS!")]
    public bool bIsAgressive;
    public bool bIsFollowing;
    public bool bIsUsingNavMesh;
    public bool bTestMotion; // ????????????
    public bool bStrategyActive;
    public bool bDebuggingDisable;

    [Header("don't touch dees bools...")]
    public bool bIsAwake;
    public bool bOperationComplete;
    public bool bSequenceComplete;
    public bool bLerping;
    public bool bMoving;
    public bool bTargetArrival;
    public bool bIsAggro;
    public bool bNavPointReached;

    [Header("\"You sure you know what you're doing son?\"")]
    public float TotalOperationTime;
    public float TargetTimeOutThreshold;
    public float TargetArrivalThreshold;
    public float AIwalkForce;
    public float AIturnRate;
    public float TargetMaintainRange;
    public float AggroRange;
    public float DisengageRange;
    public float FollowerBoxRadius;

    [Header("Just Stahp :-| ")]
    public int CurrentOperationIndex;
    public float CurrentOperationTime;
    public float CurrentTimeOutRemaining;
    public float oldRot;
    public float newRot;
    public float delta;
    public Vector3 TravelPoint;

    ////////////////////
    /// TEST SECTION ///
    ////////////////////

    

    ////////////////////

    void CheckAwake()
    {
        bIsAwake = !(GameState == null
            || CurrentCharacter == null
            || CurrentCharacter.bIsPaused
            || (CurrentCharacter.bControllable &&
                GameState.pController != null &&
                GameState.pController.CurrentCharacter == CurrentCharacter));
    }
    void FindTarget()
    {
        if (TargetCharacter != null)
            return;

        if (bIsAgressive)
        {
            TargetCharacter = GameState.CharacterMan.CharacterPool.Find(
                x => x.Sheet.Faction != base.CurrentCharacter.Sheet.Faction &&
                Vector3.Distance(x.Root.position, base.CurrentCharacter.Root.position) <= AggroRange);
            return;
        }

        if (bIsFollowing)
        {
            TargetCharacter = GameState.pController.CurrentCharacter;
            return;
        }
    }
    void CheckAggro()
    {
        if (TargetCharacter == null || !bIsAgressive)
            return;

        if (bIsAggro && Vector3.Distance(TargetCharacter.Root.position, base.CurrentCharacter.Root.position) > DisengageRange)
        {
            bIsAggro = false;
            TargetCharacter = null;
            ResetStaticSequence();
        }
        else if (!bIsAggro && Vector3.Distance(TargetCharacter.Root.position, base.CurrentCharacter.Root.position) < AggroRange)
            bIsAggro = true;
    }
    void MoveAggro()
    {
        Rigidbody rigidBody = base.CurrentCharacter.Root.gameObject.GetComponent<Rigidbody>();
        if (rigidBody == null)
            return;

        Vector3 newVector = rigidBody.transform.rotation.eulerAngles;
        newVector.y = GenerateYbearing(base.CurrentCharacter.Root.position, TargetCharacter.Root.position);
        rigidBody.transform.rotation = Quaternion.Euler(newVector);

        if (rigidBody.velocity.magnitude <= base.CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED] && Vector3.Distance(base.CurrentCharacter.Root.position, TargetCharacter.Root.position) > TargetMaintainRange)
            rigidBody.AddForce(base.CurrentCharacter.Root.forward * AIwalkForce, ForceMode.Impulse);
    }
    void UpdateSequenceIndex()
    {
        if (!bOperationComplete)
            return;

        //Debug.Log("Updating");

        CurrentOperationIndex++;
        CurrentOperationIndex = (CurrentOperationIndex >= myStaticSequence.Operations.Length) ? 0 : CurrentOperationIndex;
        bSequenceComplete = CurrentOperationIndex == 0;
        bOperationComplete = CurrentOperationIndex != 0;
    }
    void GenerateNewRandomRawTravelPoint()
    {
        if (!bSequenceComplete || bIsUsingNavMesh)
            return;

        if (!bIsFollowing &&
            (BoundaryTransforms.Length < 2 ||
            BoundaryTransforms[0] == null ||
            BoundaryTransforms[1] == null))
            return;

        //TravelPoint = Vector3.zero;

        if (!bIsFollowing)
        {
            TravelPoint.x = UnityEngine.Random.Range(BoundaryTransforms[0].position.x, BoundaryTransforms[1].position.x);
            TravelPoint.y = UnityEngine.Random.Range(BoundaryTransforms[0].position.y, BoundaryTransforms[1].position.y);
            TravelPoint.z = UnityEngine.Random.Range(BoundaryTransforms[0].position.z, BoundaryTransforms[1].position.z);
        }
        else
        {
            if (TargetCharacter == null)
                return;

            TravelPoint.x = UnityEngine.Random.Range(FollowerBoxRadius, -FollowerBoxRadius);
            TravelPoint.y = 0;
            TravelPoint.z = UnityEngine.Random.Range(FollowerBoxRadius, -FollowerBoxRadius);
        }


        // Target Magic Here!

        bSequenceComplete = false;
        bOperationComplete = true;
    }
    void GenerateNewRandomPath()
    {
        if (Pathing == null ||
            GameState.NavMesh.NavNodes.Length == 0)
            return;

        if (!bSequenceComplete || !bIsUsingNavMesh)
            return;

        int[] randCoords = new int[2];

        if (!bIsFollowing)
        {
            randCoords[0] = (int)(UnityEngine.Random.value * GameState.NavMesh.AxisCounts[0]);
            randCoords[1] = (int)(UnityEngine.Random.value * GameState.NavMesh.AxisCounts[1]);

            //Debug.Log($"{bOperationComplete} : {bSequenceComplete}");

            //Debug.Log($"{randCoords[0]} : {randCoords[1]}");

            if (!Pathing.GenerateNewPath(CurrentCharacter.Root.position, randCoords))
            {
                //Debug.Log("Shits not workin dawg!");
                return;
            }
            //Debug.Log("Shits workin dawg!");
        }
        else
        {
            if (TargetCharacter == null)
                return;

            TravelPoint.x = UnityEngine.Random.Range(FollowerBoxRadius, -FollowerBoxRadius);
            TravelPoint.y = 0;
            TravelPoint.z = UnityEngine.Random.Range(FollowerBoxRadius, -FollowerBoxRadius);

            if (!Pathing.GenerateNewPath(CurrentCharacter.Root.position, TravelPoint))
            {
                //Debug.Log("Shits not workin dawg!");
                return;
            }
            //Debug.Log("Shits workin dawg!");
        }

        if (!Pathing.RequestNextTravelPoint(out TravelPoint))
        {
            Debug.Log("Failed to request first point!");
            return;
        }

        // Target Magic Here!

        CurrentTimeOutRemaining = TargetTimeOutThreshold;
        bSequenceComplete = false;
        bOperationComplete = true;
    }
    void CommenceOperation()
    {
        if (!bOperationComplete || bSequenceComplete)
            return;

        if (myStaticSequence.Operations.Length < 1)
            return;

        //Debug.Log("Commencing");

        AIoperation operation = myStaticSequence.Operations[CurrentOperationIndex];

        switch (operation.Type)
        {
            case AIoperationType.WAIT:
                TotalOperationTime = operation.duration;
                CurrentOperationTime = TotalOperationTime;
                bLerping = true;
                break;

            case AIoperationType.ROTATE:
                TotalOperationTime = operation.duration;
                CurrentOperationTime = TotalOperationTime;
                oldRot = CurrentCharacter.Root.rotation.eulerAngles.y;
                bLerping = true;
                break;

            case AIoperationType.WALK:
                bMoving = true;
                break;
        }

        bOperationComplete = false;
    }
    float GenerateYbearing(Vector3 source, Vector3 target) // Source ------------> Target
    {
        float output = 0;

        float deltaX = target.x - source.x;
        float deltaZ = target.z - source.z;

        float magnitude = Vector3.Distance(source, target);

        output = (180 / Mathf.PI) * Mathf.Asin(deltaX / magnitude);
        output = (Mathf.Sign(deltaZ) > 0) ? output : 180 - output;

        return output;
    }
    void Lerping()
    {
        if (!bLerping)
            return;

        AIoperation operation = myStaticSequence.Operations[CurrentOperationIndex];

        CurrentOperationTime -= GlobalConstants.TIME_SCALE;
        CurrentOperationTime = (CurrentOperationTime < 0) ? 0 : CurrentOperationTime;

        Vector3 target = (bIsFollowing) ? TargetCharacter.Root.position + TravelPoint : TravelPoint;

        switch (operation.Type)
        {
            case AIoperationType.ROTATE:
                CurrentCharacter.Root.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(0, oldRot, 0)),
                                                        Quaternion.Euler(new Vector3(0, GenerateYbearing(CurrentCharacter.Root.position, target), 0)),
                                                        1 - (CurrentOperationTime / TotalOperationTime));
                break;
        }

        
        bLerping = CurrentOperationTime != 0;
        bOperationComplete = !bLerping;
    }
    void RawMoving()
    {
        if (!bMoving || bIsUsingNavMesh)
            return;

        Rigidbody rigidBody = CurrentCharacter.Root.gameObject.GetComponent<Rigidbody>();
        if (rigidBody == null)
            return;

        // When following, use the TravelPoint as an offset of the followed character's position instead
        Vector3 target = (bIsFollowing) ? TargetCharacter.Root.position + TravelPoint : TravelPoint;

        Vector3 newVector = rigidBody.transform.rotation.eulerAngles;
        newVector.y = GenerateYbearing(CurrentCharacter.Root.position, target);
        rigidBody.transform.rotation = Quaternion.Euler(newVector);

        if (rigidBody.velocity.magnitude <= CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])
            rigidBody.AddForce(CurrentCharacter.Root.forward * AIwalkForce * (1 - (rigidBody.velocity.magnitude / CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])), ForceMode.Impulse);

        bMoving = Vector3.Distance(CurrentCharacter.Root.position, target) > TargetArrivalThreshold;
        bOperationComplete = !bMoving;
        IntentVector.z = bMoving ? 1 : 0;
    }
    void NavMoving()
    {
        if (!bMoving || !bIsUsingNavMesh)
            return;

        Rigidbody rigidBody = CurrentCharacter.Root.gameObject.GetComponent<Rigidbody>();
        if (rigidBody == null)
            return;

        Vector3 newVector = rigidBody.transform.rotation.eulerAngles;

        if (bTestMotion)
        {
            float newBearing = GenerateYbearing(CurrentCharacter.Root.position, TravelPoint);
            float currentBearing = rigidBody.transform.rotation.eulerAngles.y;

            newVector.y = newBearing;

            rigidBody.transform.rotation = Quaternion.Lerp(rigidBody.transform.rotation, Quaternion.Euler(newVector), AIturnRate);

            float magnitude = GenerateBearingTurnMagnitude(currentBearing, newBearing);

            float turningFactor = (180 - magnitude) / 180;
            Vector3 newDirection = TravelPoint - rigidBody.position;
            //rigidBody.velocity = newDirection * (turningFactor * Character.MaximumStatValues.SPEED); // Maybe? : |
            if (rigidBody.velocity.magnitude <= CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])
                rigidBody.AddForce(newDirection * (turningFactor * (1 - (rigidBody.velocity.magnitude / CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED]))), ForceMode.Impulse);
        }
        else
        {
            newVector.y = GenerateYbearing(CurrentCharacter.Root.position, TravelPoint);
            rigidBody.transform.rotation = Quaternion.Euler(newVector);

            if (rigidBody.velocity.magnitude <= CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])
                rigidBody.AddForce(CurrentCharacter.Root.forward * AIwalkForce * (1 - (rigidBody.velocity.magnitude / CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])), ForceMode.Impulse);
        }

        bNavPointReached = Vector3.Distance(CurrentCharacter.Root.position, TravelPoint) < TargetArrivalThreshold;

        if (bNavPointReached)
        {
            CurrentTimeOutRemaining = TargetTimeOutThreshold;
            rigidBody.velocity = Vector3.zero;
            //Debug.Log("Reached the navPoint!");
            bMoving = Pathing.RequestNextTravelPoint(out TravelPoint);
        }

        CurrentTimeOutRemaining -= GlobalConstants.TIME_SCALE;
        if (CurrentTimeOutRemaining <= 0)
        {
            bMoving = Pathing.Repath(CurrentCharacter.Root.position) &&
                Pathing.RequestNextTravelPoint(out TravelPoint);

            //bDebuggingDisable = true;
            CurrentTimeOutRemaining = TargetTimeOutThreshold;
        }

        bOperationComplete = !bMoving;
        IntentVector.z = bMoving ? 1 : 0;
    }
    float GenerateBearingTurnMagnitude(float currentBearing, float newBearing)
    {
        float magnitude = 0;

        if (currentBearing > 180)
        {
            if (newBearing < currentBearing && newBearing > currentBearing - 180)
                magnitude = currentBearing - newBearing;

            if (newBearing > currentBearing)
                magnitude = newBearing - currentBearing;

            if (newBearing < currentBearing - 180)
                magnitude = (360 - currentBearing) + newBearing;
        }
        if (currentBearing < 180)
        {
            if (newBearing > currentBearing && newBearing < currentBearing + 180)
                magnitude = newBearing - currentBearing;

            if (currentBearing > newBearing)
                magnitude =  - newBearing + currentBearing;

            if (newBearing > currentBearing + 180)
                magnitude = (360 - newBearing) + currentBearing;
        }

        return magnitude;
    }
    void testDelta()
    {
        delta = Vector3.Distance(TravelPoint, CurrentCharacter.Root.position);
    }
    void ResetStaticSequence()
    {
        CurrentOperationIndex = 0;
        bSequenceComplete = true;
        bOperationComplete = false;
    }
    void RunSequence()
    {
        if (bStrategyActive)
        {
            Strategy.UpdateCurrentTactic();
        }
        else
        {
            GenerateNewRandomRawTravelPoint();    // x
            GenerateNewRandomPath();
        }
        CommenceOperation();
        Lerping();

        RawMoving();                    // x
        NavMoving();

        UpdateSequenceIndex();
    }

    // Start is called before the first frame update
    void Start()
    {
        bDebuggingDisable = false;
        ResetStaticSequence();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCharacterAnimationState();
        CheckAwake();

        if (bDebuggingDisable
            || !bIsAwake
            || !CurrentCharacter.bIsAlive)
            return;

        FindTarget();
        CheckAggro();

        if (!bIsAggro)
            RunSequence();
        else
            MoveAggro();

        testDelta();
    }
}
