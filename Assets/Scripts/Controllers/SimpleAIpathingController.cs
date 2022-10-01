using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathNode
{
    public NavNode Source;
    public int[] Index;

    public PathNode Parent;

    public bool bIsOpen = true;
    public int TravelCost = 0;
    public int HeuristicCost = 0;
    public int FinalCost = 0;

    public PathNode(NavNode source, int[] index, PathNode parent)
    {
        Source = source;
        Index = index;
        Parent = parent;
    }
}

public class SimpleAIpathingController : MonoBehaviour
{
    [Header("Refs")]
    public GameState GameState;
    public SimpleAIcontroller myAI;

    [Header("Defs")]
    public float InclineThreshold;

    [Header("NODEZ!?")]
    public PathNode Beginning;
    public PathNode Current;
    public PathNode Ending;

    public List<PathNode> Generated = new List<PathNode>();
    public List<PathNode> Path = new List<PathNode>();

    [Header("Logic")]
    public int CurrentPathIndex;
    public int[] EndCoord = new int[2];
    public int[] StartCoord = new int[2];

    //public List<int[]> ObstructionMask = new List<int[]>();

    //[Header("TESTING")]
    //public Transform START;
    //public Transform END;
    //public Vector3 TP;

    #region PUBLIC
    public bool GenerateNewPath(Vector3 startLocation, Vector3 destination)
    {
        if (GameState == null ||
            GameState.NavMesh == null)
            return false;

        if (GameState.NavMesh.AxisCounts == null ||
            GameState.NavMesh.AxisCounts[0] <= 0 ||
            GameState.NavMesh.AxisCounts[1] <= 0)
            return false;

        GameState.NavMesh.FindClosestNavIndex(startLocation, ref StartCoord);
        GameState.NavMesh.FindClosestNavIndex(destination, ref EndCoord);

        return AstarPathing(ref StartCoord, ref EndCoord);
    }
    public bool GenerateNewPath(Vector3 startLocation, int[] destinationCoords)
    {
        if (GameState == null ||
            GameState.NavMesh == null)
            return false;

        if (GameState.NavMesh.AxisCounts == null ||
            GameState.NavMesh.AxisCounts[0] <= 0 ||
            GameState.NavMesh.AxisCounts[1] <= 0)
            return false;

        if (destinationCoords == null ||
            destinationCoords.Length != 2)
            return false;

        GameState.NavMesh.FindClosestNavIndex(startLocation, ref StartCoord);
        
        //GenerateStartAndEndCoords(startLocation);
        EndCoord = destinationCoords;

        return AstarPathing(ref StartCoord, ref EndCoord);
    }
    public bool Repath(Vector3 startLocation)
    {
        if (!GenerateObstructionMask(startLocation))
        {
            Debug.Log("ObMaskFailed");
            return false;
        }
        bool result = GenerateNewPath(startLocation, EndCoord);
        GameState.NavMesh.ClearObstructions();
        return result;
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
    bool GenerateObstructionMask(Vector3 startLocation)//, PathNode source)
    {
        { 

        // 5x5 around the obstructed point
        /*
         
         XXX
         XOX
         XXX
         
         */

        /*
        float yBeariing = GenerateYbearing(startLocation, source.Source.Position);

        int iS;
        int iE;
        int jS;
        int jE;
        */
    }
        //if (source == null)
        //    return false;

        int range = 3;
        //int remaining = Path.Count - CurrentPathIndex;
        
        int targetIndex = CurrentPathIndex + range;
        Debug.Log($"PathCount: {Path.Count} | CurrentIndex: {CurrentPathIndex} | TargetIndex {targetIndex}");
        if (targetIndex >= Path.Count)
            return false;

        for (int i = -range; i <= range; i++)
            for (int j = -range; j <= range; j++)
            {
                /*int[] neighbourIndex = new int[] {
                source.Index[0] + i, source.Index[1] + j
                };*/

                int neighbourIndex_X = Path[targetIndex].Index[0] + i;
                int neighbourIndex_Y = Path[targetIndex].Index[1] + j;

                if (neighbourIndex_X < 0 ||
                    neighbourIndex_X >= GameState.NavMesh.AxisCounts[0] ||
                    neighbourIndex_Y < 0 ||
                    neighbourIndex_Y >= GameState.NavMesh.AxisCounts[1])
                    continue; // Can't be anything there

                GameState.NavMesh.NavNodes[neighbourIndex_X, neighbourIndex_Y].Obstructed = true;
            }

        //CurrentPathIndex += range;
        //CurrentPathIndex = CurrentPathIndex >= Path.Count ? Path.Count - 1: CurrentPathIndex;
        //CurrentPathIndex = CurrentPathIndex < 0 ? 0 : CurrentPathIndex;

        return true;
    }
    public bool RequestNextTravelPoint(ref Vector3 travelPoint)
    {
        Debug.Log($"Mob: {myAI.CurrentCharacter.Root.name} | CurrentPathIndex: {CurrentPathIndex}");

        if (Path.Count < 1)
            return false;

        if (CurrentPathIndex < 0)
            return false;

        CurrentPathIndex = CurrentPathIndex >= Path.Count ? Path.Count - 1 : CurrentPathIndex;
        Current = Path[CurrentPathIndex];
        travelPoint = Current.Source.Position;
        CurrentPathIndex--;

        return true;
    }
    #endregion

    #region A*

    bool AstarPathing(ref int[] start, ref int[] end)
    {
        if (!NodeSafetyCheck(GameState.NavMesh.NavNodes[end[0], end[1]]))
        {
            Debug.Log("Bad end point!");
            return false;
        }

        Path.Clear();
        Generated.Clear();

        Beginning = new PathNode(GameState.NavMesh.NavNodes[start[0], start[1]], new int[] { start[0], start[1] }, null);

        NodeHeuristicCost(Beginning);
        NeighbourTravelAndFinalCosts(Beginning);
        Generated.Add(Beginning);

        int safeCount = 0;

        while (safeCount < GlobalConstants.MAX_PATH_CYCLES)
        {
            Current = Generated.Find(x => x.bIsOpen);
            if (Current == null)
            {
                //Debug.Log("SCIENCE BITCH!");
                return false;
            }

            for (int i = 0; i < Generated.Count; i++)
            {
                if (Generated[i].FinalCost < Current.FinalCost && Generated[i].bIsOpen)
                {
                    Current = Generated[i];
                    //Debug.Log($"Better open node found at: {Current.Index[0]}:{Current.Index[1]}");
                }
            }

            Current.bIsOpen = false;

            if (Current.Index[0] == end[0] && Current.Index[1] == end[1])
            {
                Ending = Current;
                break;
            }

            UpdateNeighbours(Current);

            safeCount++;
        }

        if (safeCount == GlobalConstants.MAX_PATH_CYCLES)
        {
            //Debug.Log("Naw mang");
            return false;
        }

        CurrentPathIndex = -1;

        while (Current != null)
        {
            //Debug.Log($"PathNode {count}: {Current.Source.Position}");
            CurrentPathIndex++;

            Path.Add(Current);
            Current = Current.Parent;
        }

        Generated.Clear();

        if (Path.Count == 0)
            return false;

        return true;
    }
    void UpdateNeighbours(PathNode source)
    {
        for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
            {
                int neighbourIndex_X = source.Index[0] + i;
                int neighbourIndex_Y = source.Index[1] + j;

                if (neighbourIndex_X < 0 ||
                    neighbourIndex_X >= GameState.NavMesh.AxisCounts[0] ||
                    neighbourIndex_Y < 0 ||
                    neighbourIndex_Y >= GameState.NavMesh.AxisCounts[1])
                    continue; // Can't be anything there

                if (i == 0 && j == 0)
                    continue; // Skip itself

                float verticalDisplacement = GameState.NavMesh.NavNodes[neighbourIndex_X, neighbourIndex_Y].Position.y - source.Source.Position.y;
                if (verticalDisplacement > InclineThreshold)
                    continue; // Too high to reach

                if (!NodeSafetyCheck(GameState.NavMesh.NavNodes[neighbourIndex_X, neighbourIndex_Y]))
                    continue; // Pathing unfriendly

                PathNode neighbour = Generated.Find(x => x.Index[0] == neighbourIndex_X && x.Index[1] == neighbourIndex_Y);
                if (neighbour == null) // Create a new neighbour node
                {
                    neighbour = new PathNode(GameState.NavMesh.NavNodes[neighbourIndex_X, neighbourIndex_Y], new int[] { neighbourIndex_X, neighbourIndex_Y }, source);
                    NodeHeuristicCost(neighbour);
                    NeighbourTravelAndFinalCosts(neighbour);
                    Generated.Add(neighbour);
                }

                if (!neighbour.bIsOpen)
                    continue; // Neighbour is closed

                int newDistanceCost = NeighbourCostReturn(source, neighbour) + source.TravelCost;

                if (newDistanceCost < neighbour.TravelCost)
                {
                    neighbour.Parent = source;
                    NeighbourTravelAndFinalCosts(neighbour);
                }
            }
    }
    bool NodeSafetyCheck(NavNode rawNode)
    {
        if (rawNode.Type == NavNodeType.EDGE ||
            rawNode.Type == NavNodeType.PIT)
            return false;

        if (rawNode.Obstructed)
            return false;

        return true;
    }
    void NodeHeuristicCost(PathNode node)
    {
        int deltaX = Mathf.Abs(node.Index[0] - EndCoord[0]);
        int deltaY = Mathf.Abs(node.Index[1] - EndCoord[1]);

        int diagonal = (deltaX < deltaY) ? deltaX : deltaY;
        int lateral = Mathf.Abs(deltaX - deltaY);

        node.HeuristicCost = (diagonal * 14) + (lateral * 10);
    }
    void NeighbourTravelAndFinalCosts(PathNode neighbour)
    {
        if (neighbour.Parent != null)
            neighbour.TravelCost = NeighbourCostReturn(neighbour, neighbour.Parent) + neighbour.Parent.TravelCost;

        neighbour.FinalCost = neighbour.TravelCost + neighbour.HeuristicCost;
    }
    int NeighbourCostReturn(PathNode A, PathNode B)
    {
        if (A == B)
            return 0;
        if (A.Index[0] != B.Index[0] && A.Index[1] != B.Index[1])
            return 14;
        return 10;
    }
    #endregion

    void TestFiring()
    {
        if (Path.Count < 2)
            return;

        //Debug.Log("Firing!");

        for (int i = Path.Count - 1; i > -1 ; i--)
            if (Path[i].Parent != null)
                Debug.DrawLine(Path[i].Source.Position, Path[i].Parent.Source.Position, Color.blue);
    }

    // Start is called before the first frame update
    void Start()
    {
        //ReturnTravelPoint(START.position, END.position, out TP);
    }

    // Update is called once per frame
    void Update()
    {
        TestFiring();
    }
}
