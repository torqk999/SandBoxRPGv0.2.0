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

    public PathNode(NavNode source, int indX, int indY, PathNode parent)
    {
        Source = source;
        Index = new int[] {indX, indY};
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
    //public int[] NeighbourBuffer = new int[2];

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

        GenerateStartAndEndCoords(startLocation, destination, true);

        return AstarPathing( StartCoord,  EndCoord);
    }
    public bool GenerateNewPath(Vector3 startLocation, ref int[] destinationCoords)
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

        GenerateStartAndEndCoords(startLocation);
        EndCoord = destinationCoords;

        return AstarPathing( StartCoord,  EndCoord);
    }
    public bool Repath(Vector3 startLocation)
    {
        GenerateObstructionMask(startLocation);
        bool result = GenerateNewPath(startLocation, ref EndCoord);
        GameState.NavMesh.ClearObstructions();
        return result;
    }
    void GenerateStartAndEndCoords( Vector3 startLocation, Vector3 endLocation = new Vector3(), bool bGenEnd = false)
    {
        StartCoord[0] = -1;
        StartCoord[1] = -1;

        if (bGenEnd)
        {
            EndCoord[0] = -1;
            EndCoord[1] = -1;
        }

        //startNode = new int[2] { -1, -1 };
        //endNode = new int[2] { -1, -1 };

        for (int i = 0; i < GameState.NavMesh.AxisCounts[0]; i++)
        {
            for (int j = 0; j < GameState.NavMesh.AxisCounts[0]; j++)
            {
                if ( !GameState.NavMesh.NavNodes[i, j].Obstructed &&
                    ( (StartCoord[0] == -1 && StartCoord[1] == -1) ||
                    Vector3.Distance(startLocation, GameState.NavMesh.NavNodes[i, j].Position) <
                    Vector3.Distance(startLocation, GameState.NavMesh.NavNodes[StartCoord[0], StartCoord[1]].Position)))
                {
                    //Debug.Log("SetStartNode");
                    StartCoord[0] = i;
                    StartCoord[1] = j;
                }

                if (!bGenEnd)
                    continue;

                if ( !GameState.NavMesh.NavNodes[i, j].Obstructed &&
                    ( (EndCoord[0] == -1 && EndCoord[1] == -1) ||
                    Vector3.Distance(endLocation, GameState.NavMesh.NavNodes[i, j].Position) <
                    Vector3.Distance(endLocation, GameState.NavMesh.NavNodes[EndCoord[0], EndCoord[1]].Position) ) )
                {
                    EndCoord[0] = i;
                    EndCoord[1] = j;
                }
            }
        }
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
    void GenerateObstructionMask(Vector3 startLocation)
    {
        //ObstructionMask.Clear();

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

        int range = 3;
        int targetIndex = CurrentPathIndex + range - 1;
        targetIndex = targetIndex >= Path.Count ? Path.Count - 1 : targetIndex;
        PathNode source = Path[targetIndex];

        for (int i = -range; i <= range; i++)
            for (int j = -range; j <= range; j++)
            {
                int NeighbourIndex_X = source.Index[0] + i;
                int NeighbourIndex_Y = source.Index[1] + j;

                if (NeighbourIndex_X < 0 ||
                    NeighbourIndex_X >= GameState.NavMesh.AxisCounts[0] ||
                    NeighbourIndex_Y < 0 ||
                    NeighbourIndex_Y >= GameState.NavMesh.AxisCounts[1])
                    continue; // Can't be anything there

                GameState.NavMesh.NavNodes[NeighbourIndex_X, NeighbourIndex_Y].Obstructed = true;
            }

        //CurrentPathIndex += range;
        //CurrentPathIndex = CurrentPathIndex >= Path.Count ? Path.Count - 1: CurrentPathIndex;
        //CurrentPathIndex = CurrentPathIndex < 0 ? 0 : CurrentPathIndex;
        //Debug.Log($"PathCount: {Path.Count}");
        //Debug.Log($"PathIndex: {CurrentPathIndex}");

        //Current = Path[CurrentPathIndex];
    }
    public bool RequestNextTravelPoint(out Vector3 travelPoint)
    {
        if (CurrentPathIndex < 0)
        {
            travelPoint = Vector3.zero;
            return false;
        }

        if (CurrentPathIndex >= Path.Count)
            CurrentPathIndex = Path.Count - 1;

        Current = Path[CurrentPathIndex];
        travelPoint = Current.Source.Position;
        CurrentPathIndex--;

        return true;
    }
    #endregion

    #region A*
    //bool AstarPathing(ref int[] start,ref int[] end)
    bool AstarPathing(int[] start, int[] end)
    {
        if (!NodeSafetyCheck(GameState.NavMesh.NavNodes[end[0], end[1]]))
        {
            Debug.Log("Bad end point!");
            return false;
        }

        Path.Clear();
        Generated.Clear();

        Beginning = new PathNode(GameState.NavMesh.NavNodes[start[0], start[1]], start[0], start[1], null);

        NodeHeuristicCost(Beginning);
        NodeTravelAndFinalCosts(Beginning);
        Generated.Add(Beginning);

        int safeCount = 0;

        while (safeCount < 1000)
        {
            Current = Generated.Find(x => x.bIsOpen);

            for (int i = 0; i < Generated.Count; i++)
            {
                Debug.Log($"My Subject: {myAI.CurrentCharacter.gameObject.name}");
                Debug.Log($"GenerateCount: {Generated.Count}");
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

            /*if (!UpdateNeighbours(Current))
            {
                // for now terminate the path
                Ending = Current;
                break;
            }*/

            safeCount++;
        }

        if (safeCount == 1000)
        {
            Debug.Log("Naw mang");
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
    bool UpdateNeighbours(PathNode source)
    {
        //Debug.Log($"Checking Node: {target.Index[0]}:{target.Index[1]}");
        bool bValidNeighbours = false;

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
                    neighbour = new PathNode(GameState.NavMesh.NavNodes[neighbourIndex_X, neighbourIndex_Y], neighbourIndex_X, neighbourIndex_Y, source);
                    NodeHeuristicCost(neighbour);
                    NodeTravelAndFinalCosts(neighbour);
                    Generated.Add(neighbour);

                    //Debug.Log($"Generated new node at: {neighbour.Index[0]}:{neighbour.Index[1]}");
                }

                if (!neighbour.bIsOpen)
                    continue; // Neighbour is closed

                bValidNeighbours = true;

                int newDistanceCost = NodeCostReturn(source, neighbour) + source.TravelCost;
                //float newDistanceCost = (int)Vector3.Distance(neighbour.Source.Position, source.Source.Position) + source.TravelCost;

                if (newDistanceCost < neighbour.TravelCost)
                {
                    //Debug.Log($"Improving path to neighbour: {neighbour.Index[0]}:{neighbour.Index[1]}");
                    neighbour.Parent = source;
                    NodeTravelAndFinalCosts(neighbour);
                }
            }
        return bValidNeighbours;
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
        //node.HeuristicCost = (int)Vector3.Distance(node.Source.Position, EndNavNode.Position);
    }
    void NodeTravelAndFinalCosts(PathNode node)
    {
        if (node.Parent != null)
            node.TravelCost = NodeCostReturn(node, node.Parent) + node.Parent.TravelCost;
            //node.TravelCost = (int)Vector3.Distance(node.Source.Position, node.Parent.Source.Position) + node.Parent.TravelCost;

        node.FinalCost = node.TravelCost + node.HeuristicCost;
    }
    int NodeCostReturn(PathNode A, PathNode B)
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
