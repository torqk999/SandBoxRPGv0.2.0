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

    public float IntendedTranslationMagnitude;
    public float IntendedYawMagnitude;

    void UpdateCharacterAnimationState()
    {
        if (CurrentCharacter == null)
            return;

        CurrentCharacter.UpdateAnimationState(IntendedTranslationMagnitude, IntendedYawMagnitude);
        IntendedTranslationMagnitude = 0;
        IntendedYawMagnitude = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCharacterAnimationState();
    }
}
