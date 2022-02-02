using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prims : IMazeAlgorithm
{
    public string Name { get => "Prim's"; }

    public IEnumerator Run(ICell[,] grid, int width, int height, int maxSize, Mode pMode, float waitSeconds)
    {
        int mode = (int)pMode;
        bool slimWalls = pMode == Mode.SLIM;

        // Disable out of range cells and enable those in range
        for (int y = 0; y < maxSize; y++)
            for (int x = 0; x < maxSize; x++)
            {
                if (y < height && x < width)
                    grid[x, y].Reset(slimWalls);
                else
                    grid[x, y].Disable();
            }

        // List of frontier cells to be processed
        List<ICell> frontiers = new List<ICell>();

        // Pick the starting cell based on wall mode and turn it into passage
        ICell currentCell = grid[mode, mode];
        currentCell.TurnIntoPassage();

        // Get the index of the current cell and its neighbours
        Vector2Int index = currentCell.Index;
        GetNeighbours(grid, index.x, index.y, frontiers, mode, width, height);

        while (frontiers.Count > 0)
        {
            // Pick random frontier cell
            currentCell = frontiers[Random.Range(0, frontiers.Count)];
            // Turn current cell into passage  and 
            currentCell.TurnIntoPassage();
            // Get its index
            index = currentCell.Index;
            // Join a random available neighbour
            JoinRandomPassageNeighbour(grid, index.x, index.y, mode, width, height);
            // Remove current cell from frontier list
            frontiers.Remove(currentCell);
            // Find its neighbours
            GetNeighbours(grid, index.x, index.y, frontiers, mode, width, height);

            if (waitSeconds > 0.0f) yield return new WaitForSeconds(waitSeconds);
        }

        // Start and End points
        grid[0 + mode, 0].Mark();
        grid[width - 1 - mode, height - 1].Mark();
        yield return null;
    }

    private void GetNeighbours(ICell[,] grid, int x, int y, List<ICell> frontierCells, int mode, int width, int height)
    {
        // NORTH
        int neighbourY = y + 1 + mode; // mode = 0 (SLIM) || mode = 1 (THICK)
        if (neighbourY < height - mode && grid[x, neighbourY].IsBlocked && !grid[x, neighbourY].IsBeingProcessed)
        {
            grid[x, neighbourY].Process();
            frontierCells.Add(grid[x, neighbourY]);
        }
        // SOUTH
        neighbourY = y - 1 - mode;
        if (neighbourY > -1 + mode && grid[x, neighbourY].IsBlocked && !grid[x, neighbourY].IsBeingProcessed)
        {
            grid[x, neighbourY].Process();
            frontierCells.Add(grid[x, neighbourY]);
        }

        // WEST
        int neighbourX = x + 1 + mode;
        if (neighbourX < width - mode && grid[neighbourX, y].IsBlocked && !grid[neighbourX, y].IsBeingProcessed)
        {
            grid[neighbourX, y].Process();
            frontierCells.Add(grid[neighbourX, y]);
        }
        // EAST
        neighbourX = x - 1 - mode;
        if (neighbourX > -1 + mode && grid[neighbourX, y].IsBlocked && !grid[neighbourX, y].IsBeingProcessed)
        {
            grid[neighbourX, y].Process();
            frontierCells.Add(grid[neighbourX, y]);
        }
    }

    private void JoinRandomPassageNeighbour(ICell[,] grid, int x, int y, int mode, int width, int height)
    {
        List<Vector2Int> availablePassCells = new List<Vector2Int>();

        // NORTH Neighbour
        int neighbourY = y + 1 + mode; // mode = 0 (SLIM) || mode = 1 (THICK)
        if (neighbourY < height - mode && !grid[x, neighbourY].IsBlocked)
            availablePassCells.Add(new Vector2Int(x, neighbourY));
        // SOUTH Neighbour
        neighbourY = y - 1 - mode;
        if (neighbourY > -1 + mode && !grid[x, neighbourY].IsBlocked)
            availablePassCells.Add(new Vector2Int(x, neighbourY));
        // WEST Neighbour
        int neighbourX = x + 1 + mode;
        if (neighbourX < width - mode && !grid[neighbourX, y].IsBlocked)
            availablePassCells.Add(new Vector2Int(neighbourX, y));
        // EAST Neighbour
        neighbourX = x - 1 - mode;
        if (neighbourX > -1 + mode && !grid[neighbourX, y].IsBlocked)
            availablePassCells.Add(new Vector2Int(neighbourX, y));

        if (availablePassCells.Count > 0)
        {
            int index = Random.Range(0, availablePassCells.Count);
            Vector2Int nIndex = availablePassCells[index];

            if (mode == 0)
            { // If slim walls ( mode == 0 )
                grid[x, y].RemoveWallWithNeighbour(nIndex.x, nIndex.y);
                grid[nIndex.x, nIndex.y].RemoveWallWithNeighbour(x, y);
            }
            else
            { // The cell inbetween two neighbour cells
                int midX = x - (x - nIndex.x) / 2;
                int midY = y - (y - nIndex.y) / 2;
                grid[x, y].TurnIntoPassage();
                grid[midX, midY].TurnIntoPassage();
            }
        }
    }
}