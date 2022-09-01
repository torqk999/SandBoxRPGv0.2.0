using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuilderMode
{
    SCROLL,
    DISPLACE,
    X_ROTATION,
    Y_ROTATION,
    Z_ROTATION
}

public class ObjectBuilder : MonoBehaviour
{
    [Header ("References")]
    public List<GameObject> Prefabs;
    public Transform Caster;
    public Transform PrefabFolder;
    public Transform BuiltObjectFolder;

    [Header("UserSettings")]
    public string BuilderTag;
    public float MinCastRange;
    public float MaxCastRange;
    public float RotationScalar;
    public float CastScalar;

    [Header("Logic")]
    public BuilderMode CurrentBuilderMode = BuilderMode.SCROLL;
    public int PrefabSelectionIndex;

    [Header("Casting")]
    public bool bIsCastingPrefab;
    public float CastRange;
    public Vector3 CastOrientation;

    [Header("Test")]
    public float lastDelta;

    // Public Methods
    public void ToggleCast(bool castOn)
    {
        bIsCastingPrefab = castOn;
        TogglePrefabs();
    }
    public void PickSelection(int index)
    {
        if (index < 0 || index >= Prefabs.Count)
            return;
        PrefabSelectionIndex = index;

        TogglePrefabs();
    }
    public void BuilderScrollWheel(float input)
    {
        switch(CurrentBuilderMode)
        {
            case BuilderMode.SCROLL:
                ScrollSelection(input);
                break;

            case BuilderMode.DISPLACE:
                DisplaceCast(input);
                break;

            case BuilderMode.X_ROTATION:
                ChangeEuler(0, input);
                break;

            case BuilderMode.Y_ROTATION:
                ChangeEuler(1, input);
                break;

            case BuilderMode.Z_ROTATION:
                ChangeEuler(2, input);
                break;
        }
    }
    public void BuildModeChange()
    {
        if (CurrentBuilderMode == BuilderMode.Z_ROTATION)
            CurrentBuilderMode = 0;
        else
            CurrentBuilderMode++;
    }
    public void BuildModeClick()
    {
        if (BuiltObjectFolder == null)
            return;

        GameObject newObject = Instantiate(
            Prefabs[PrefabSelectionIndex],
            Prefabs[PrefabSelectionIndex].transform.position,
            Quaternion.Euler(CastOrientation),
            BuiltObjectFolder);

        newObject.tag = (BuilderTag == string.Empty)? "BUILT" : BuilderTag;

        Collider collider = (Collider)newObject.GetComponent("Collider");
        if (collider != null)
            collider.enabled = true;
    }

    // Scroll Methods
    void ScrollSelection(float up)
    {
        PrefabSelectionIndex += (up > 0) ? 1 : -1;
        PrefabSelectionIndex = (PrefabSelectionIndex >= Prefabs.Count) ? 0 : PrefabSelectionIndex;
        PrefabSelectionIndex = (PrefabSelectionIndex < 0) ? Prefabs.Count - 1 : PrefabSelectionIndex;

        TogglePrefabs();
    }
    void DisplaceCast(float delta)
    {
        lastDelta = delta;
        CastRange += delta * CastScalar;
        CastRange = (CastRange < MinCastRange) ? MinCastRange : CastRange;
        CastRange = (CastRange > MaxCastRange) ? MaxCastRange : CastRange;
    }
    void ChangeEuler(int axis, float angle)
    {
        switch(axis)
        {
            case 0:
                CastOrientation.x += angle * RotationScalar;
                break;
            case 1:
                CastOrientation.y += angle * RotationScalar;
                break;
            case 2:
                CastOrientation.z += angle * RotationScalar;
                break;
        }
    }

    // Logic Methods
    void PopulatePrefabs()
    {
        for (int i = 0; i < PrefabFolder.childCount; i++)
        {
            GameObject nextPrefab = PrefabFolder.GetChild(i).gameObject;
            Collider collderComp = (Collider)nextPrefab.GetComponent("Collider");
            if (collderComp != null)
                collderComp.enabled = false;

            Prefabs.Add(PrefabFolder.GetChild(i).gameObject);
        }
    }
    void TogglePrefabs()
    {
        for (int i = 0; i < Prefabs.Count; i++)
        {
            bool set = (i == PrefabSelectionIndex) ? true : false;
            set = (bIsCastingPrefab) ? set : false;
            Prefabs[i].SetActive(set);
        }
    }
    void CastPrefab()
    {
        if (Prefabs.Count < 1 || Caster == null)
            return;

        Vector3 castPosition = Caster.position + (Caster.forward * CastRange);

        Prefabs[PrefabSelectionIndex].transform.position = castPosition;
        Prefabs[PrefabSelectionIndex].transform.rotation = Quaternion.Euler(CastOrientation);
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulatePrefabs();
        TogglePrefabs();
    }

    // Update is called once per frame
    void Update()
    {
        CastPrefab();
    }
}
