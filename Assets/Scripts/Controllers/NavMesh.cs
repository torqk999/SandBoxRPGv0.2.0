using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavNodeType
{
    CLEAR,
    PIT,
    HAZARD,
    EDGE
}

public class NavNode
{
    public Vector3 Position;
    public NavNodeType Type;
    public bool Obstructed;

    public NavNode(Vector3 position, NavNodeType type)
    {
        Position = position;
        Type = type;
        Obstructed = false;
    }
}

public class NavMesh : MonoBehaviour
{
    public bool ShowMesh;

    public Transform[] bounds = new Transform[2];

    public float Resolution;
    public float MaxDetectionRange;
    public float InclineThreshold;
    public Vector3 Offset = new Vector3(0,1,0);

    public int[] AxisCounts = new int[2];

    public NavNode[,] NavNodes;

    public void FindClosestNavIndex(Vector3 location, ref int[] index)
    {
        float deltaX = Mathf.Abs(location.x - bounds[0].position.x);
        float deltaZ = Mathf.Abs(location.z - bounds[0].position.z);

        index[0] = (int)(deltaX / Resolution);
        index[0] = index[0] < 0 ? 0 : index[0];
        index[0] = index[0] >= AxisCounts[0] ? AxisCounts[0] - 1 : index[0];

        index[1] = (int)(deltaZ / Resolution);
        index[1] = index[1] < 0 ? 0 : index[1];
        index[1] = index[1] >= AxisCounts[1] ? AxisCounts[1] - 1 : index[1];
    }
    public void GenerateMesh()
    {
        if (bounds[0] == null || bounds[1] == null || Resolution <= 0)
            return;

        AxisCounts[0] = (int)(Mathf.Abs(bounds[0].position.x - bounds[1].position.x) / Resolution);
        AxisCounts[1] = (int)(Mathf.Abs(bounds[0].position.z - bounds[1].position.z) / Resolution);

        NavNodes = new NavNode[AxisCounts[0], AxisCounts[1]];

        for (int i = 0; i < AxisCounts[0]; i++)
            for (int j = 0; j < AxisCounts[1]; j++)
            {
                Vector3 newSkyPoint = new Vector3(
                    bounds[0].position.x + (i * Resolution * Mathf.Sign(bounds[1].position.x - bounds[0].position.x)),
                    bounds[0].position.y,
                    bounds[0].position.z + (j * Resolution * Mathf.Sign(bounds[1].position.z - bounds[0].position.z)));

                RaycastHit hit;
                if (Physics.Raycast(newSkyPoint, Vector3.down * MaxDetectionRange, out hit))
                    NavNodes[i, j] = new NavNode(hit.point + Offset, NavNodeType.CLEAR);
                else
                {
                    Vector3 location = new Vector3(newSkyPoint.x, newSkyPoint.y - MaxDetectionRange, newSkyPoint.z);
                    NavNodes[i, j] = new NavNode(location, NavNodeType.PIT);
                }
            }
    }
    void GenerateSlopes()
    {
        if (!ShowMesh)
            return;

        for (int i = 0; i < AxisCounts[0]; i++)
            for (int j = 0; j < AxisCounts[1]; j++)
            {
                if (j + 1 <= AxisCounts[1] - 1)
                {
                    // Down Left
                    if (i - 1 >= 0)
                        DrawSlope(NavNodes[i, j].Position, NavNodes[i - 1, j + 1].Position);

                    // Down Middle
                    DrawSlope(NavNodes[i, j].Position, NavNodes[i, j + 1].Position);

                    // Down Right
                    if (i + 1 <= AxisCounts[0] - 1)
                        DrawSlope(NavNodes[i, j].Position, NavNodes[i + 1, j + 1].Position);
                }

                // Right Middle
                if (i + 1 <= AxisCounts[0] - 1)
                    DrawSlope(NavNodes[i, j].Position, NavNodes[i + 1, j].Position);
            }
    }
    void DrawSlope(Vector3 origin, Vector3 target)
    {
        if (InclineThreshold <= 0)
            return;
        float slope = Mathf.Abs(origin.y - target.y);
        Color newColor = new Color(slope / InclineThreshold, 1 - (slope / InclineThreshold), 0);
        Debug.DrawLine(origin, target, newColor);
    }
    public void ClearObstructions()
    {
        foreach (NavNode node in NavNodes)
            node.Obstructed = false;
    }

    /*
    void TestFire()
    {
        foreach(Vector3 skyPoint in SkyPoints)
        {
            Debug.DrawRay(skyPoint, Vector3.down * 10, Color.black);
        }
    }

*/

    // Start is called before the first frame update
    void Start()
    {
        //GenerateSkyPoints();
        //GenerateGroundPoints();
    }

    // Update is called once per frame
    void Update()
    {
        GenerateSlopes();
        //TestFire();
    }
}
