using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<T1, T2> // Basic Key-Value pair of two C# objects. Used to practice implementing the concepts.
{
    public readonly T1 Object;
    public readonly T2 Value;

    public Pair(T1 o, T2 v)
    {
        Object = o;
        Value = v;
    }
}

// C# doesn't have a native Priority Queue that I know of, so we've made our own.
// This should give our A* a O(log n) time complexity
public class P_Queue<T>
{
    private List<Pair<T, int>> heap; // the data of the heal is stored as key value pairs in an object-priority relation

    // Constructor
    public P_Queue()
    {
        heap = new List<Pair<T, int>>();
    }

    // Add to the heap and heapify
    public void Enqueue(T item, int priority)
    {
        heap.Add(new Pair<T, int>(item, priority));
        HeapifyUp();
        Print();
    }

    //Remove from top of the heap
    public T Dequeue()
    {
        T top = heap[0].Object;

        heap[0] = heap[Count - 1];
        heap.RemoveAt(Count - 1);

        HeapifyDown();
        Print();
        return top;
    }

    //pring the heap
    public void Print()
    {
        string s = "";
        foreach (Pair<T, int> p in heap)
        {
            s += (" " + p.Value);
        }
    }

    // Restores heap structure when removing
    private void HeapifyDown()
    {
        int currentIndex = 0;
        int lchild = LeftChild(currentIndex);
        int rchild = RightChild(currentIndex);

        int winner = lchild;

        while (lchild < Count)
        {
            if (rchild < Count && heap[rchild].Value < heap[lchild].Value)
            {
                winner = rchild;
            }

            if (heap[winner].Value < heap[currentIndex].Value)
            {
                Pair<T, int> temp = heap[winner];
                heap[winner] = heap[currentIndex];
                heap[currentIndex] = temp;

                currentIndex = winner;
                lchild = LeftChild(currentIndex);
                rchild = RightChild(currentIndex);
                winner = lchild;
            }
            else
            {
                break;
            }
        }
    }

    // Restores heap strucure when inserting 
    private void HeapifyUp()
    {
        int currentIndex = Count - 1;
        int parentIndex = Parent(currentIndex);

        while (currentIndex != parentIndex)
        {
            if (heap[currentIndex].Value < heap[parentIndex].Value)
            {
                Pair<T, int> temp = heap[parentIndex];
                heap[parentIndex] = heap[currentIndex];
                heap[currentIndex] = temp;
            }
            else
            {
                break;
            }

            currentIndex = parentIndex;
            parentIndex = Parent(currentIndex);
        }
    }

    // Left child of a given node
    private static int LeftChild(int current) 
    {
        return 2 * current + 1;
    }

    // Right child of a given node
    private static int RightChild(int current)
    {
        return 2 * current + 2;
    }

    // Parent of a given node
    private static int Parent(int current)
    {
        if (current == 0)
        {
            return 0;
        }
        else
        {
            return ((current - 1) / 2);
        }
    }

    //
    // Accsesors
    //

    public int Count
    { get { return heap.Count; } }
}


/// <summary>
/// Uses A* to allow for pathfinding in a changing hex grid of variable size,
/// refrences the exsisting hexcell map from map master.
/// </summary>
public static class HexStar
{
    private static OddQPoint[][] directions = { // int order; SE, NE, N, NW, SW, S 
        new OddQPoint[]{
            new OddQPoint(+1, +1), new OddQPoint(+1, 0), new OddQPoint(0, -1), new OddQPoint(-1, 0), new OddQPoint(-1, +1), new OddQPoint(0, +1)
        },
        new OddQPoint[]{
            new OddQPoint(+1, 0), new OddQPoint(+1, -1), new OddQPoint(0, -1), new OddQPoint(-1, -1), new OddQPoint(-1, 0), new OddQPoint(0, +1)
        }
    };

    private static Dictionary<OddQPoint, OddQPoint> cameFrom; // the tiles that have been explored (path builds)
    private static Dictionary<OddQPoint, int> costSoFar; // the currnent cost of the paths(s)

    private struct OddQPoint // a point in odd offset cordinate space
    {
        public readonly int q;
        public readonly int r;

        public OddQPoint(int _q = 0, int _r = 0)
        {
            q = _q;
            r = _r;
        }

        public OddQPoint(HexCell h)
        {
            q = h.Q;
            r = h.R;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(OddQPoint other)
        {
            return (other.q == this.q) && (other.r == this.r);
        }
    }

    // returns the optimal path of a start and end cell, returns empty if path is longer than move speed, or there is no path
    public static List<HexCell> GetPath(HexCell begin, HexCell end, int moveSpeed)
    {
        List<HexCell> path = new List<HexCell>();

        OddQPoint goal = new OddQPoint(end);
        OddQPoint start = new OddQPoint(begin);

        if (SolvePath(MapMaster.Map, start, goal, moveSpeed))
        {
            OddQPoint step = goal;
            while (!step.Equals(start))
            {
                path.Add(MapMaster.Map[step.r, step.q]);
                step = cameFrom[step];
            }
            path.Reverse();

            /*string s = "";
            foreach (HexCell h in path)
            {
                s += h.ToString() + " ";
            }
            Debug.Log(s);*/
        }

        return path;
    }

    //Uses A* to solve for the optimal path, bounded by a cost limit
    private static bool SolvePath(HexCell[,] map, OddQPoint start, OddQPoint goal, int maxCost)
    {
        if (!map[goal.r, goal.q].Passable)
        {
            return false;
        }

        cameFrom = new Dictionary<OddQPoint, OddQPoint>();
        costSoFar = new Dictionary<OddQPoint, int>();

        P_Queue<OddQPoint> openNodes = new P_Queue<OddQPoint>();
        openNodes.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        OddQPoint current;

        while (openNodes.Count != 0) // buid paths until a goal is reached or there is no where else to explore
        {
            current = openNodes.Dequeue();

            if (current.Equals(goal))
            {
                if (costSoFar[current] > maxCost)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            foreach (OddQPoint next in Neighbors(current))
            {
                HexCell h = map[next.r, next.q];
                 if (!h.passable)
                 {
                    if (h.unit == null)
                    {
                        continue;
                    }
                    else if ((h.Unit.Player != PlayerMaster.CurrentTurn))
                    {
                        continue;
                    }
                 }

                int nCost = costSoFar[current] + HexCell.CostToHex(map[current.r, current.q], map[next.r, next.q]);

                if (!costSoFar.ContainsKey(next) || nCost < costSoFar[next])
                {
                    costSoFar[next] = nCost;
                    int prio = nCost + Heuristic(map[next.r, next.q], map[goal.r, goal.q]);
                    openNodes.Enqueue(next, prio);
                    cameFrom[next] = current;
                }
            }
        }
        return false;
    }

    //Finds and returns a list of valid neighbors around a given point
    private static List<OddQPoint> Neighbors(OddQPoint center)
    {
        List<OddQPoint> items = new List<OddQPoint>();

        int parity = center.q & 1;

        OddQPoint n;

        foreach (OddQPoint dir in directions[parity])
        {
            n = new OddQPoint(center.q + dir.q, center.r + dir.r);

            if (MapMaster.IsCellOnMap(n.q, n.r))
            {
                items.Add(n);
            }
        }

        return items;
    }

    // the cost heuristic, in this case, euclidian distance
    static public int Heuristic(HexCell a, HexCell b)
    {
        return HexCell.CubeDistance(a, b); //HexCell.LineDistance(a.transform.position, b.transform.position);
    }
}
