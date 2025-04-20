using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder
{
    public List<GridSpace> GetPath(GridSpace startSpace, GridSpace endSpace, int moveRange, Dictionary<Vector3Int, GridSpace> grid) {
        if (!startSpace && !endSpace)
        {
            return new List<GridSpace>();
        }
        
        List<GridSpace> openList = new List<GridSpace>();
        List<GridSpace> closedList = new List<GridSpace>();

        openList.Add(startSpace);

        while(openList.Any())
        {
            GridSpace currentSpace = openList.OrderBy(x => x.F).First();

            // Remove from open list and add to closed list
            openList.Remove(currentSpace);
            closedList.Add(currentSpace);

            // Check if currentSpace space is destination
            if (currentSpace == endSpace)
            {
                return GetPath(startSpace, endSpace);
            }

            List<GridSpace> neighbors = GetNeighbors(currentSpace, grid);
            foreach(GridSpace space in neighbors)
            {
                space.G = GetManhattanDistance(startSpace, space);
                space.H = GetManhattanDistance(endSpace, space);

                // TODO: Implement jumping
                if (space.IsBlocked || closedList.Contains(space) || space.G > moveRange)
                {
                    continue;
                }

                space.previous = currentSpace;

                if(!openList.Contains(space))
                {
                    openList.Add(space);
                }
            }
        }

        // Return best possible path
        GridSpace bestDestination = closedList.OrderBy(x => x.H).First();
        return GetPath(startSpace, bestDestination);
    }

    public List<GridSpace> GetPossibleSpaces(GridSpace startSpace, int moveRange, Dictionary<Vector3Int, GridSpace> grid)
    {
        List<GridSpace> openList = new List<GridSpace>();
        List<GridSpace> closedList = new List<GridSpace>();

        openList.Add(startSpace);

        while(openList.Any())
        {
            GridSpace currentSpace = openList.First();

            // Remove from open list and add to closed list
            openList.Remove(currentSpace);
            closedList.Add(currentSpace);

            List<GridSpace> neighbors = GetNeighbors(currentSpace, grid);
            foreach(GridSpace space in neighbors)
            {
                float dist = GetManhattanDistance(startSpace, space);
                // TODO: Implement jumping
                if (space.IsBlocked || closedList.Contains(space) || dist > moveRange)
                {
                    continue;
                }
                else if(!openList.Contains(space))
                {
                    openList.Add(space);
                }
            }
        }

        // No path
        return closedList;
    }

    private int GetManhattanDistance(GridSpace current, GridSpace destination)
    {
        return Mathf.Abs(destination.X - current.X) + Mathf.Abs(destination.Y - current.Y);
    }

    private List<GridSpace> GetNeighbors(GridSpace space, Dictionary<Vector3Int, GridSpace> gridDict)
    {
        List<GridSpace> neighbors = new List<GridSpace>();
        GridSpace neighbor;

        // Get North
        if(gridDict.TryGetValue(new Vector3Int(space.X + 1, space.Y, 0), out neighbor))
        {
            neighbors.Add(neighbor);
        }

        // Get South
        if(gridDict.TryGetValue(new Vector3Int(space.X - 1, space.Y, 0), out neighbor))
        {
            neighbors.Add(neighbor);
        }

        // Get East
        if(gridDict.TryGetValue(new Vector3Int(space.X, space.Y + 1, 0), out neighbor))
        {
            neighbors.Add(neighbor);
        }

        // Get West
        if(gridDict.TryGetValue(new Vector3Int(space.X, space.Y - 1, 0), out neighbor))
        {
            neighbors.Add(neighbor);
        }

        return neighbors;
    }

    List<GridSpace> GetPath(GridSpace start, GridSpace end)
    {
        List<GridSpace> path = new List<GridSpace>();
        GridSpace currentTile = end;
        path.Add(end);

        while(currentTile != start)
        {
            path.Add(currentTile.previous);
            currentTile = currentTile.previous;
        }

        path.Reverse();

        return path;
    }
}
