using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DEPRICATED. IMPROVED UPON BY THE HEXSTAR Script. Kept for reference. -- Brandon 

/// <summary>
/// Uses A* to allow for pathfinding in a changing hex grid of variable size
/// </summary>
public class HexPathfinder{

    class AStarSolver
    {
        enum TileStates { Unchecked, Open, Closed }; // enum to title and track the 3 states a tile can be in.

        private static Vector2[][] directions = 
        { // int order; SE, NE, N, NW, SW, S 
            new Vector2[] 
            {
                new Vector2(+1, +1), new Vector2(+1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(-1, +1), new Vector2(0, +1)
            },
            new Vector2[]
            {
                new Vector2(+1, 0), new Vector2(+1, -1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(0, +1)
            }
        };

        private static int MapSize; // the total NxN size of the map being worked with.

        private Tile[,] map; // the map A* will traverse and the tiles therin
        private List<Tile> openTiles; // the queue of tiles being evaluated to find the desired path.

        public AStarSolver(int mS) // default constructor where mS is the number of rows/columns in the map
        {
            map = new Tile[mS, mS];
            MapSize = mS;
            InitMap();
        }

        // "Prime" the map so all tiles are in their default state and their locations set to the proper cordinates in the map
        private void InitMap()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    map[i, j] = new Tile(new Vector2(i, j));
                }
            }
        }

        // Sets the a tile at location l to the value of p
        public void SetPassalbe(Vector2 l, bool p)
        {
            map[(int)l.x, (int)l.y].Passable = p;
        }

        // Sets the scalar cost of a tile at Vector2 l to the value of c
        public void SetMovementCost(Vector2 l, int c)
        {
            map[(int)l.x, (int)l.y].CostScalar = c;
        }

        // Starting at the goal, traced back through the parents of each tile until it reachest the start tile, and returns a 
        // list of the Vector2s on the path in the order of start -> finish
        public List<Vector2> GetPath(Vector2 s, Vector2 g)
        {
            List<Vector2> p = new List<Vector2>();

            Tile.Start = map[(int)s.x, (int)s.y];
            Tile.Goal = map[(int)g.x, (int)g.y];

            if (Solve())
            {

                Tile cursor = Tile.Goal;
                while (cursor != null)
                {
                    p.Add(cursor.Location);
                    cursor = cursor.Parent;
                }
                p.Reverse();

            }

            return p;
        }

        /// <summary>
        /// The logic for A* traversal
        /// </summary>
        /// <returns>returns true if a valid path was found</returns>
        private bool Solve()
        {
            SortTileByF sF = new SortTileByF(); // sorting logic for the tiles
            openTiles = new List<Tile>() { Tile.Start }; // the tile to be checked is the initial location

            while (openTiles.Count != 0) // Continues until either there are no more tiles to evaluate, or a solution has been found
            {
                openTiles.Sort(sF); // sor the list so the tiles with the lowest cost are in the front.
                Tile.Current = openTiles[0]; // the current tile is now the most optimal tile
                Tile.Current.State = TileStates.Closed; // close the tile so it is not evaluated again

                openTiles.RemoveAt(0); // remove from the list of open tiles.

                if (Tile.Current == Tile.Goal) // if we have reached our goal exit the loop
                {
                    return true;
                }

                List<Tile> neighbors = TracePath(); // evaluate the adjacent tiles, opening them as needed, and building our path based on the least cost to traverse to each one.

                foreach (Tile neighbor in neighbors)
                {
                    if (neighbor.State == TileStates.Unchecked) // if it hasnt been checked before, we need only add it to the list of open tiles 
                    {
                        neighbor.State = TileStates.Open;
                        neighbor.Parent = map[(int)Tile.Current.Location.x, (int)Tile.Current.Location.y]; // go set its parent to be the first tile to disover it by default
                        openTiles.Add(neighbor);
                    }
                    else // if the neighbor is already open, we now need to see if its an ideal node to travel to
                    {
                        double gNew; // equal to the sum of the g(x) of it's parent (aka the direct g cost of the parent) and the cost it would take to get to it from the parent tile.
                        gNew = neighbor.Parent.Distance2Start + Vector2.Distance(neighbor.Location, neighbor.Parent.Location) * neighbor.CostScalar;

                        if (gNew < neighbor.Distance2Start) // if the gNew cost is a better deal the cost to get to neighbor from start, put this node on the path
                        {
                            neighbor.Parent = map[(int)Tile.Current.Location.x, (int)Tile.Current.Location.y];
                        }
                    }
                }
            }

            return false;
        }

        // Tile Def and Methods
        /// <summary>
        /// Tile class for use only by the AStarSolver
        /// Static values hold on to the start, goal, and current places of the path
        /// Each tile knows the cost to get to and from it, and it's total cost.
        /// A tile knows its location on the map and its parent in the path.
        /// </summary>
        private class Tile
        {
            public static Tile Start; // the starting location the path is comming from
            public static Tile Goal; // the exit location the path must reach
            public static Tile Current; // the current location on the path while tring to get from start to goal

            public Tile Parent; // the adjacent tile preceeding this one in the calculated path

            private int costScalar; // not really used right now, but can be used to indicate any extra cost a it takes to get to a tile. Like difficult terrain costing twice as much movement.
            private bool passable; // if this tile on the map cannot be move to or through, this is false
            private Vector2 location; // the tiles location in the 2d map array
            private TileStates state; // instance of the enum TileStates to indicate if it's been closed, open, or unchecked by the algorithim

            //Default constructor, most of the time, only the minimum Vector2 should be provided, and all other parameters left to their default value
            public Tile(Vector2 l, bool p = true, TileStates t = TileStates.Unchecked, int cS = 1)
            {
                location = l;
                passable = p;
                state = t;
                costScalar = cS;
            }

            //Accessor Methods
            public bool Passable
            { get { return passable; } set { passable = value; } }
            public Vector2 Location
            { get { return location; } }
            public TileStates State
            { get { return state; } set { state = value; } }
            public int CostScalar
            { get { return costScalar; } set { costScalar = value; } }

            //Tile Heuristics 
            public double Distance2Goal // the "h(x)" value of the heuristics. Or "cost to get there"
            { get { return Vector2.Distance(location, Tile.Goal.location); } }
            public double Distance2Start // the "g(x)" value of the huristics. Or "cost to get here"
            { get { return Vector2.Distance(location, Tile.Start.location); } }
            public double Cost // the "f(x)" value of the heuristic. Or "cost of path"
            { get { return Distance2Start * costScalar + Distance2Goal; } }

        }

        private class SortTileByF : IComparer<Tile>
        {
            public int Compare(Tile t1, Tile t2) // if you want to have some fun, swap these values so that A* will find the most direct but least optimal solution.
            {
                if (t1.Distance2Goal > t2.Distance2Goal)
                {
                    return 1;
                }
                else if (t1.Distance2Goal < t2.Distance2Goal)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }  // Simple class to be used by the Sort method of the List class. 

        public bool ValidTile(int x, int y)
        {
            return (x >= 0 && x < AStarSolver.MapSize) && (y >= 0 && y < AStarSolver.MapSize);
        } // returns true if the tile at the given location is within the bounds of the map

        public bool ValidTile(Vector2 p)
        {
            return (p.x >= 0 && p.x < AStarSolver.MapSize) && (p.y >= 0 && p.y < AStarSolver.MapSize);
        } // overload of ValidTile to use a passed Vector2 parameter

        //Builds the traversal path bassed on the most ideal canidate of the current tile
        private List<Tile> TracePath()
        {
            List<Tile> result = new List<Tile>();
            int nx = 0;
            int ny = 0;

            int parity = (int)Tile.Current.Location.y & 1;

            foreach (Vector2 p in directions[parity]) // looks at a possible cordinate for every tile neighboring the current tile
            {
                nx = (int)(Tile.Current.Location.x + p.x);
                ny = (int)(Tile.Current.Location.y + p.y);

                if (ValidTile(nx, ny)) // continue to evaluate only if the tile is on the map
                {
                    Tile neighbor = map[nx, ny]; // the tile being evaluated
                    if (neighbor.Passable) // continue to evaluate only if it's a tile that can be mvoed to / on
                    {
                        if (neighbor.State != TileStates.Closed) // continue to evaluate if the tile has not already been closed 
                        {
                            result.Add(neighbor);
                        }
                    }
                }
            }
            return result;
        }
    }
}
