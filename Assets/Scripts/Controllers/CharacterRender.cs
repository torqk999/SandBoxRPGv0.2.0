using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;

public enum GearSlot
{
    NECK,
    CHEST,
    BELT,
    LEGS,
    BOOTS,
    ARMS,
    PAULDRONS,
    GLOVES
}

public enum GearType
{
    NAKED,
    CLOTH,
    LEATHER,
    RING_MAIL,
    PLATE
}

public enum HandSource
{
    AXE,
    SWORD,
    WAND,
    BOW,
    SPEAR,
    STAFF,
    DAGGER,
    SHIELD
}

public enum MaterialType
{
    FLESH,
    HAIR,
    WOOD_BRIGHT,
    WOOD_DARK,
    WOOD_ROTTEN,
    METAL_BRIGHT,
    METAL_RUST,
    METAL_ROUGH,
    TRIM_GOLD,
    TRIM_AURA,
    LEATHER
}

[Serializable]
public class WardrobeProfile
{
    public string Name;
    public int ID;
}
[Serializable]
public class GearProfile : WardrobeProfile
{
    public GearSlot Slot;
    public GearType GearType;
    public MaterialType MatType;
    //public Mesh thisMesh;
}
[Serializable]
public class HandProfile : WardrobeProfile
{
    public HandSource Type;
    public MaterialType Base;
    public MaterialType Handle;
    public List<MaterialType> Details;
    public GameObject thisSource;
}

public enum CharAnimationState
{
    Idle,
    Walking,
    Running,
    Jumping,
    Falling,
    Attacking,
    Casting
}
public enum CharAnimation
{
  Chop,
  Slice,
  Poke,
  Self
}
public enum AnimationTarget
{
    ElementalBall,
    ElementalArrow
}

public class CharacterRender : MonoBehaviour
{
    public CharacterManager CharManager;
    public Character MyCharacter;
    public Animator MyAnimator;
    public List<HandProfile> Hands;
    public List<GearProfile> Gear;
    public GameObject MyPrefab;
    public CharAnimationState MyAnimationState;

    public bool bRunning;
    public int AnimationLayer;
    public float AniCombatTimer;

    public void CombatTimer()
    {
        AniCombatTimer -= GlobalConstants.TIME_SCALE;
    }

    public void walkingAnimation()
    {
        int WalkingLayer = MyAnimator.GetLayerIndex("MovementLayer");

        if (MyAnimator.GetFloat("horizontalMove") + MyAnimator.GetFloat("verticalMove") != 0f)
        {
            MyAnimator.SetLayerWeight(WalkingLayer, 1);
        }

        if (MyAnimator.GetFloat("horizontalMove") + MyAnimator.GetFloat("verticalMove") == 0)
        {
            MyAnimator.SetLayerWeight(WalkingLayer, 0);
        }
    }
    public void AnimationAttempt(CharacterAbility AttemptedAbility = null)

    {
        if (AttemptedAbility == null)
        {
            return;
        }
        int CombatLayer = MyAnimator.GetLayerIndex("CombatLayer");

        if (MyAnimator.GetLayerWeight(CombatLayer) < 0)

        {
            MyAnimator.SetTrigger("inCombat");

            if (MyAnimator.GetBool("Attacking") == false)
            {
                Quaternion CurrentQuat = MyAnimator.rootRotation;
                MyAnimator.SetBoneLocalRotation(HumanBodyBones.UpperChest, CurrentQuat);
            }

            if (MyAnimator.GetBool("Attacking") == true)
            {

                // AttemptedAbility.

                //MyAnimator.SetBoneLocalRotation(HumanBodyBones.UpperChest,tempQuat);
            }

            if (MyAnimator.GetLayerWeight(CombatLayer) == 0)
            {
                MyAnimator.ResetTrigger("inCombat");
            }

            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!bRunning)
            return;
       
        if (Input.GetKey(KeyCode.W))
            MyAnimator.SetBool("Walking", true);
        else
            MyAnimator.SetBool("Walking", false);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyAnimator.SetTrigger("Swing");
            //myAnimator.ResetTrigger("Swing");
        }
        walkingAnimation();
    }
}
