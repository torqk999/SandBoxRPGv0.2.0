using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("===CHAR CONTROL===")]
    public GameState GameState;
    public Transform Focus;
    public Pawn CurrentPawn;
    public Character CurrentCharacter;
    public Character TargetCharacter;
    //public CharacterRender Render;

    public Vector3 IntentVector;
    public Vector3 IntentRotations;

    public void UpdateCharacterAnimationState()
    {
        //Debug.Log($"IntentVector: {IntentVector}");

        if (CurrentCharacter == null)
            return;

        float trueForward = Vector3.Dot(IntentVector, CurrentCharacter.RigidBody.velocity) / (IntentVector.magnitude * CurrentCharacter.RigidBody.velocity.magnitude);

        //CurrentCharacter.UpdateAnimationIntents(trueForward, IntentVector.x);
        CurrentCharacter.UpdateAnimationIntents(IntentVector.z, IntentVector.x);
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateCharacterAnimationState();
    }
}
