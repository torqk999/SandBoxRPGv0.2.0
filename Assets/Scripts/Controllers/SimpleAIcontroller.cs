using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public enum AIstate
{
    PASSIVE,
    FOLLOW,
    AGGRO,
    AFRAID
}
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
    //public bool bTestMotion; // ????????????
    public bool bStrategyActive;
    public bool bDebuggingDisable;

    [Header("don't touch dees bools...")]
    public bool IsAIawake;
    public bool bOperationComplete;
    //public bool bSequenceComplete;
    public bool bLerping;
    public bool bMoving;
    public bool bTargetArrival;
    public AIstate State;
    public AIstate oldState;
    //public bool bIsAggro;
    public bool bNavPointReached;

    [Header("\"You sure you know what you're doing son?\"")]
    public float TotalOperationTime;
    public float TargetTimeOutThreshold;
    public float TargetArrivalDistanceThreshold;
    public float AIwalkForce;
    public float AIturnRate;
    public float CollisionAvoidanceRange;
    //public float CollisionAvoidanceForceScalar;
    public float AggroRange;
    public float DisengageRange;
    public float FollowerBoxRadius;

    [Header("Just Stahp :-| ")]
    public int[] RandomBuffer = new int[2];
    public int CurrentOperationIndex;
    public float CurrentOperationTime;
    public float CurrentTimeOutRemaining;
    public float oldRot;
    public float newRot;
    public float delta;
    public Vector3 TravelPoint;
    //public Vector3 BufferVector;

    ////////////////////
    /// TEST SECTION ///
    ////////////////////



    ////////////////////

    void CheckAwake()
    {
        IsAIawake = !(GameState == null
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
                x => x.Sheet.Faction != CurrentCharacter.Sheet.Faction &&
                Vector3.Distance(x.Root.position, CurrentCharacter.Root.position) <= AggroRange);
            //if (TargetCharacter != null)
                //Debug.Log("Found you!");
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

        if (State == AIstate.AGGRO && !CheckAggroRange())
        {
            Debug.Log("De-Aggro");
            State = oldState;
            //bIsAggro = false;
            TargetCharacter = null;
            ResetPassiveSequence();
        }
        else if (State != AIstate.AGGRO && CheckAggroRange())
        {
            //Debug.Log("Aggro");
            oldState = State;
            State = AIstate.AGGRO;
        } //bIsAggro = true;
    }
    bool CheckAggroRange()
    {
        foreach (Character enemy in CurrentCharacter.CurrentParty.Foes)
        {
            if (Vector3.Distance(enemy.Root.position, CurrentCharacter.Root.position) < AggroRange)
                return true;

            if (State == AIstate.AGGRO &&
                Vector3.Distance(enemy.Root.position, CurrentCharacter.Root.position) < DisengageRange)
                return true;
        }
        return false;
    }
    bool CheckAbilityRange()
    {
        if (CurrentCharacter.CurrentAction == null)
            return false;

        if (!(CurrentCharacter.CurrentAction is EffectAbility))
            return true;

        EffectAbility currentTargettedAction = (EffectAbility)CurrentCharacter.CurrentAction;

        foreach (BaseEffect effect in currentTargettedAction.Effects)
            if (effect.HasEligableTarget())
                return true;

        return false;

        //return (/*State == AIstate.AGGRO
    //&& CurrentCharacter.CurrentTargetCharacter != null
    //&& Vector3.Distance(CurrentCharacter.Root.position, CurrentCharacter.CurrentTargetCharacter.Root.position) <= (currentTargettedAction.AOE_Range)); //* AbilityRangeScalar));
    }
    /*void MoveAggro()
    {
        Rigidbody rigidBody = CurrentCharacter.Root.gameObject.GetComponent<Rigidbody>();
        if (rigidBody == null)
            return;

        Vector3 newVector = rigidBody.transform.rotation.eulerAngles;
        newVector.y = GenerateYbearing(CurrentCharacter.Root.position, TargetCharacter.Root.position);
        rigidBody.transform.rotation = Quaternion.Euler(newVector);

        if (rigidBody.velocity.magnitude <= CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED] && Vector3.Distance(CurrentCharacter.Root.position, TargetCharacter.Root.position) > TargetMaintainRange)
            rigidBody.AddForce(CurrentCharacter.Root.forward * AIwalkForce, ForceMode.Impulse);
    }*/

    /*void GenerateNewRandomRawTravelPoint()
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
    }*/
    bool GenerateNewRandomPath()
    {
        if (Pathing == null ||
            !bIsUsingNavMesh ||
            GameState.NavMesh.NavNodes.Length == 0)
            return false;

        if (CheckAbilityRange())
            return false;

        switch(State)
        {
            case AIstate.PASSIVE:
                RandomBuffer[0] = (int)(UnityEngine.Random.value * GameState.NavMesh.AxisCounts[0]);
                RandomBuffer[1] = (int)(UnityEngine.Random.value * GameState.NavMesh.AxisCounts[1]);

                if (!Pathing.GenerateNewPath(CurrentCharacter.Root.position, RandomBuffer))
                {
                    return false;
                }
                break;

            case AIstate.FOLLOW:
                if (TargetCharacter == null)
                    return false;

                TravelPoint.x = UnityEngine.Random.Range(FollowerBoxRadius, -FollowerBoxRadius);
                TravelPoint.y = 0;
                TravelPoint.z = UnityEngine.Random.Range(FollowerBoxRadius, -FollowerBoxRadius);

                if (!Pathing.GenerateNewPath(CurrentCharacter.Root.position, TravelPoint))
                {
                    return false;
                }
                break;

            case AIstate.AGGRO:
                if (TargetCharacter == null)
                    return false;

                Debug.Log("Pathing to you!");
                TravelPoint = TargetCharacter.Root.position;
                if (!Pathing.GenerateNewPath(CurrentCharacter.Root.position, TravelPoint))
                {
                    return false;
                }
                break;
        }

        if (!Pathing.RequestNextTravelPoint(ref TravelPoint))
        {
            Debug.Log("Failed to request first point!");
            return false;
        }

        return true;
    }
    void UpdateOperation()
    {
        if (!bOperationComplete)// || bSequenceComplete)
            return;

        if (myStaticSequence.Operations.Length < 1)
            return;

        //Debug.Log($"{CurrentCharacter.Root.name} || OPind: {CurrentOperationIndex}");

        AIoperation operation = myStaticSequence.Operations[CurrentOperationIndex];

        switch (operation.Type)
        {
            case AIoperationType.WAIT:
                if (!GenerateNewRandomPath())
                    return;
                TotalOperationTime = operation.duration;
                CurrentOperationTime = TotalOperationTime;
                bMoving = false;
                bLerping = true;
                break;

            case AIoperationType.ROTATE:
                TotalOperationTime = operation.duration;
                CurrentOperationTime = TotalOperationTime;
                oldRot = CurrentCharacter.Root.rotation.eulerAngles.y;
                bLerping = true;
                break;

            case AIoperationType.WALK:
                CurrentTimeOutRemaining = TargetTimeOutThreshold;
                bMoving = true;
                bLerping = false;
                break;
        }

        CurrentOperationIndex++;
        CurrentOperationIndex = (CurrentOperationIndex >= myStaticSequence.Operations.Length) ? 0 : CurrentOperationIndex;
        bOperationComplete = false;
    }
    float GenerateYbearing(Vector3 source, Vector3 target) // Source ------------> Target
    {
        float output = 0;
        float magnitude = Vector3.Distance(source, target);

        if (magnitude == 0)
            return output;

        float deltaX = target.x - source.x;
        float deltaZ = target.z - source.z;

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
                //Debug.Log($"{CurrentCharacter.Root.name} is rotating!");// : {CurrentCharacter.Root.position}");
                Quaternion start = Quaternion.Euler(0, oldRot, 0);
                Quaternion end = Quaternion.Euler(0, GenerateYbearing(CurrentCharacter.Root.position, target), 0);
                float lerp = 1 - (CurrentOperationTime / TotalOperationTime);
                CurrentCharacter.Root.rotation = Quaternion.Lerp(start, end, lerp);
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

        bMoving = Vector3.Distance(CurrentCharacter.Root.position, target) > TargetArrivalDistanceThreshold;
        bOperationComplete = !bMoving;
        IntentVector.z = bMoving ? 1 : 0;
    }
    void NavMoving()
    {
        if (!bMoving || !bIsUsingNavMesh)
            return;

        //Debug.Log($"{CurrentCharacter.Root.name} is moving");

        //Rigidbody rigidBody = CurrentCharacter.Root.gameObject.GetComponent<Rigidbody>();
        if (CurrentCharacter.RigidBody == null)
            return;

        Vector3 newVector = CurrentCharacter.Root.transform.rotation.eulerAngles;

        //if (bTestMotion)
        {
            float newBearing = GenerateYbearing(CurrentCharacter.Root.position, TravelPoint);

            //Debug.Log($"{CurrentCharacter.Root.name} || newBearing: {newBearing} || TravelPoint: {TravelPoint}");
            float currentBearing = CurrentCharacter.Root.transform.rotation.eulerAngles.y;

            newVector.y = newBearing;

            CurrentCharacter.Root.transform.rotation = Quaternion.Lerp(CurrentCharacter.Root.transform.rotation, Quaternion.Euler(newVector), AIturnRate);

            float magnitude = GenerateBearingTurnMagnitude(currentBearing, newBearing);

            float turningFactor = (180 - magnitude) / 180;
            Vector3 newDirection = TravelPoint - CurrentCharacter.Root.position;

            // Hope //
            CollisionVectorAdjust(ref newDirection, GameState.CharacterMan.CharacterPool);
            //////////

            if (CurrentCharacter.RigidBody.velocity.magnitude <= CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])
                CurrentCharacter.RigidBody.AddForce(newDirection * (turningFactor * (1 - (CurrentCharacter.RigidBody.velocity.magnitude / CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED]))), ForceMode.Impulse);
        }
        /*else
        {
            newVector.y = GenerateYbearing(CurrentCharacter.Root.position, TravelPoint);
            rigidBody.transform.rotation = Quaternion.Euler(newVector);

            if (rigidBody.velocity.magnitude <= CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])
                rigidBody.AddForce(CurrentCharacter.Root.forward * AIwalkForce * (1 - (rigidBody.velocity.magnitude / CurrentCharacter.MaximumStatValues.Stats[(int)RawStat.SPEED])), ForceMode.Impulse);
        }*/

        bNavPointReached = Vector3.Distance(CurrentCharacter.Root.position, TravelPoint) < TargetArrivalDistanceThreshold;

        if (bNavPointReached)
        {
            CurrentTimeOutRemaining = TargetTimeOutThreshold;
            //CurrentCharacter.RigidBody.velocity = Vector3.zero;
            //Debug.Log($"{CurrentCharacter.Root.name} Reached the navPoint!");
            bMoving = Pathing.RequestNextTravelPoint(ref TravelPoint);
            //bOperationComplete = !bMoving;
        }

        if (CheckAbilityRange())
        { 
            bMoving = false;
            //bOperationComplete = true;
        }

        CurrentTimeOutRemaining -= GlobalConstants.TIME_SCALE;
        if (CurrentTimeOutRemaining <= 0)
        {
            if (!Pathing.Repath(CurrentCharacter.Root.position))
            {
                //bSequenceComplete = true;
                //bOperationComplete = true;
                bMoving = false;
            }
            else
            {
                bMoving = Pathing.RequestNextTravelPoint(ref TravelPoint);
                //bOperationComplete = !bMoving;
                CurrentTimeOutRemaining = TargetTimeOutThreshold;
            }  
        }

        bOperationComplete = !bMoving;
        IntentVector.z = bMoving ? 1 : 0;
    }
    void CollisionVectorAdjust(ref Vector3 init, List<Character> pool)
    {
        Vector3 bufferVector = Vector3.zero;
        Vector3 initNorm = Vector3.Normalize(init);
        float initMag = Vector3.Distance(init, Vector3.zero);

        foreach(Character character in pool)
        {
            float colMag = Vector3.Distance(CurrentCharacter.Root.position , character.Root.position);
            if (colMag > CollisionAvoidanceRange)
                continue;

            Vector3 colNorm = Vector3.Normalize(CurrentCharacter.Root.position - character.Root.position);
            Vector3 colFinal = AIwalkForce * (1 - (colMag / CollisionAvoidanceRange)) * colNorm;
            bufferVector += colFinal;
        }

        float colFinalMag = Vector3.Distance(bufferVector, Vector3.zero);
        float blah = 2;
        init = (((1 - (colFinalMag / (colFinalMag + blah))) * initMag) * initNorm) + bufferVector;
        //Debug.Log($"{CurrentCharacter.Root.name} || finalVector: {init} || bufferVector: {bufferVector}");
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
                magnitude = -newBearing + currentBearing;

            if (newBearing > currentBearing + 180)
                magnitude = (360 - newBearing) + currentBearing;
        }

        return magnitude;
    }
    void testDelta()
    {
        delta = Vector3.Distance(TravelPoint, CurrentCharacter.Root.position);
    }
    void ResetPassiveSequence()
    {
        CurrentOperationIndex = 0;
        //bSequenceComplete = true;
        bOperationComplete = true;
    }
    void RunPassiveSequence()
    {
        UpdateOperation();
        Lerping();
    }

    // Start is called before the first frame update
    void Start()
    {
        bDebuggingDisable = false;
        State = bIsFollowing ? AIstate.FOLLOW : AIstate.PASSIVE;
        ResetPassiveSequence();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCharacterAnimationState();
        CheckAwake();

        if (bDebuggingDisable
            || !IsAIawake
            || CurrentCharacter.CheckCCstatus(CCstatus.DEAD)
            || CurrentCharacter.CheckCCstatus(CCstatus.IMMOBILE))
            return;

        FindTarget();
        CheckAggro();
        //GenerateNewRandomPath();

        switch(State)
        {
            case AIstate.PASSIVE:
            case AIstate.FOLLOW:
                RunPassiveSequence();
                break;

            case AIstate.AGGRO:
                break;
        }
        //if (State != AIstate.AGGRO)
            
        //else
        //    RunCombatTactics();

        NavMoving();
        testDelta();
    }
}
