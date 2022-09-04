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

    [Header("Defs")]
    public float InclineThreshold;

    [Header("NODEZ!?")]
    public PathNode Beginning;
    public PathNode Current;
    public PathNode Ending;

    public List<PathNode> Generated = new List<PathNode>();
    public List<PathNode> Path = new List<PathNode>();

    [Header("Logic")]
    public int[] EndCoord = new int[2];

    //[Header("TESTING")]
    //public Transform START;
    //public Transform END;
    //public Vector3 TP;

    #region PUBLIC
    public bool GenerateNewPath(Vector3 location, Vector3 destination)
    {
        if (GameState == null ||
            GameState.NavMesh == null)
            return false;

        if (GameState.NavMesh.AxisCounts == null ||
            GameState.NavMesh.AxisCounts[0] <= 0 ||
            GameState.NavMesh.AxisCounts[1] <= 0)
            return false;

        int[] startNode = new int[2] { -1, -1 };
        int[] endNode = new int[2] { -1, -1 };

        for (int i = 0; i < GameState.NavMesh.AxisCounts[0]; i++)
        {
            for (int j = 0; j < GameState.NavMesh.AxisCounts[0]; j++)
            {
                if ((startNode[0] == -1 && startNode[1] == -1) ||
                    Vector3.Distance(location, GameState.NavMesh.NavNodes[i, j].Position) <
                    Vector3.Distance(location, GameState.NavMesh.NavNodes[startNode[0], startNode[1]].Position))
                {
                    startNode[0] = i;
                    startNode[1] = j;
                }

                if ((endNode[0] == -1 && endNode[1] == -1) ||
                    Vector3.Distance(destination, GameState.NavMesh.NavNodes[i, j].Position) <
                    Vector3.Distance(destination, GameState.NavMesh.NavNodes[endNode[0], endNode[1]].Position))
                {
                    endNode[0] = i;
                    endNode[1] = j;
                }
            }
        }

        EndCoord[0] = endNode[0];
        EndCoord[1] = endNode[1];

        return AstarPathing(startNode, EndCoord);
    }
    public bool GenerateNewPath(Vector3 location, int[] index)
    {
        if (GameState == null ||
            GameState.NavMesh == null)
            return false;

        if (GameState.NavMesh.AxisCounts == null ||
            GameState.NavMesh.AxisCounts[0] <= 0 ||
            GameState.NavMesh.AxisCounts[1] <= 0)
            return false;

        int[] startNode = new int[2] { -1, -1 };

        for (int i = 0; i < GameState.NavMesh.AxisCounts[0]; i++)
        {
            for (int j = 0; j < GameState.NavMesh.AxisCounts[0]; j++)
            {
                if ((startNode[0] == -1 && startNode[1] == -1) ||
                    Vector3.Distance(location, GameState.NavMesh.NavNodes[i, j].Position) <
                    Vector3.Distance(location, GameState.NavMesh.NavNodes[startNode[0], startNode[1]].Position))
                {
                    startNode[0] = i;
                    startNode[1] = j;
                }
            }
        }

        EndCoord[0] = index[0];
        EndCoord[1] = index[1];

        return AstarPathing(startNode, EndCoord);
    }
    public bool RequestNextTravelPoint(out Vector3 travelPoint)
    {
        if (Path.Count == 0)
        {
            travelPoint = Vector3.zero;
            return false;
        }

        travelPoint = Path[Path.Count - 1].Source.Position;

        if (Path.Count > 0)
            Path.RemoveAt(Path.Count - 1);

        return true;
    }
    #endregion

    #region A*

    bool AstarPathing(int[] start, int[] end)
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
        NodeTravelAndFinalCosts(Beginning);
        Generated.Add(Beginning);

        int safeCount = 0;

        while (safeCount < 1000)
        {
            Current = Generated.Find(x => x.bIsOpen);

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

        if (safeCount == 1000)
        {
            //Debug.Log("Naw mang");
            return false;
        }

        int count = 1;

        while (Current != null)
        {
            //Debug.Log($"PathNode {count}: {Current.Source.Position}");
            count++;

            Path.Add(Current);
            Current = Current.Parent;
        }

        Generated.Clear();

        if (Path.Count == 0)
            return false;

        return true;
    }
    void UpdateNeighbours(PathNode target)
    {
        //Debug.Log($"Checking Node: {target.Index[0]}:{target.Index[1]}");

        for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
            {
                int[] neighbourIndex = new int[] {
                target.Index[0] + i, target.Index[1] + j
                };

                if (neighbourIndex[0] < 0 ||
                    neighbourIndex[0] > GameState.NavMesh.AxisCounts[0] ||
                    neighbourIndex[1] < 0 ||
                    neighbourIndex[1] > GameState.NavMesh.AxisCounts[1])
                    continue; // Can't be anything there

                if (i == 0 && j == 0)
                    continue; // Skip itself

                float verticalDisplacement = GameState.NavMesh.NavNodes[neighbourIndex[0], neighbourIndex[1]].Position.y - target.Source.Position.y;
                if (verticalDisplacement > InclineThreshold)
                    continue; // Too high to reach

                if (!NodeSafetyCheck(GameState.NavMesh.NavNodes[neighbourIndex[0], neighbourIndex[1]]))
                    continue; // Pathing unfriendly

                PathNode neighbour = Generated.Find(x => x.Index[0] == neighbourIndex[0] && x.Index[1] == neighbourIndex[1]);
                if (neighbour == null) // Create a new neighbour node
                {
                    neighbour = new PathNode(GameState.NavMesh.NavNodes[neighbourIndex[0], neighbourIndex[1]], neighbourIndex, target);
                    NodeHeuristicCost(neighbour);
                    NodeTravelAndFinalCosts(neighbour);
                    Generated.Add(neighbour);

                    //Debug.Log($"Generated new node at: {neighbour.Index[0]}:{neighbour.Index[1]}");
                }

                if (!neighbour.bIsOpen)
                    continue; // Neighbour is closed

                float newDistanceCost = (int)Vector3.Distance(neighbour.Source.Position, target.Source.Position) + target.TravelCost;

                if (newDistanceCost < neighbour.TravelCost)
                {
                    //Debug.Log($"Improving path to neighbour: {neighbour.Index[0]}:{neighbour.Index[1]}");
                    neighbour.Parent = target;
                    NodeTravelAndFinalCosts(neighbour);
                }
            }
    }
    bool NodeSafetyCheck(NavNode rawNode)
    {
        if (rawNode.Type == NavNodeType.EDGE ||
            rawNode.Type == NavNodeType.PIT)
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
