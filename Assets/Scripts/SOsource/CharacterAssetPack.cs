using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebugState
{
    DEFAULT,
    DEAD,
    LOSS_H,
    LOSS_S,
    LOSS_M,
    GAIN_H,
    GAIN_S,
    GAIN_M
}

[CreateAssetMenu(fileName = "CharacterAssetPack", menuName = "ScriptableObjects/CharacterAssetPack")]
public class CharacterAssetPack : ScriptableObject
{
    [Header ("DebugMaterials")]
    public Material Default;
    public Material Dead;

    public Material HealthLoss, StamLoss, ManaLoss;
    public Material HealthGain, StaminaGain, ManaGain;

    [Header ("Resources")]
    public Animator Animator;
}
