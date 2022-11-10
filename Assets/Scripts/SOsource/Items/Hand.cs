using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : Equipment
{
    [Header("Hand Properties")]
    public MaterialType Base;
    public MaterialType Handle;
    public List<MaterialType> Details;
}
