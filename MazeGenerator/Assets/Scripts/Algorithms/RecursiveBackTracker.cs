using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveBackTracker : IMazeAlgorithm
{
    public string Name { get => "Recursive Back Tracker"; }

    public IEnumerator Run(ICell[,] grid, int width, int height, int maxSize, Mode pMode, float waitSeconds)
    {
        bool slimWalls = pMode == Mode.SLIM;
        int mode = (int)pMode;
        for (int y = 0; y < maxSize; y++)
            for (int x = 0; x < maxSize; x++)
            {
                if (y < height && x < width)
                    grid[x, y].Reset(slimWalls);
                else
                    grid[x, y].Disable();
            }

        Stack<ICell> stack = new Stack<ICell>();
        stack.Push(grid[mode, mode]);
        stack.Peek().Visit();
        stack.Peek().Mark();

        while (stack.Count != 0) {       
            Vector2Int topCellIndex = stack.Peek().Index;
            ICell nCell = GetRandomNonVisitedNeighbour(grid, topCellIndex.x, topCellIndex.y, mode, width, height);

            if (nCell != null) {
                stack.Peek().Unmark(slimWalls);
                JoinWithNeighbour(stack.Peek(), nCell, grid, mode, width, height);
                stack.Push(nCell);
                nCell.Visit();
                stack.Peek().Mark();
            } else {
                if(!slimWalls) stack.Peek().Unmark(slimWalls);
                stack.Pop();
                if(stack.Count != 0) stack.Peek().Mark();
            }

            if (waitSeconds != 0) yield return new WaitForSeconds(waitSeconds);
        }

        // Used for animated version, unmark all marked cells (from red to white)
        if (slimWalls) {
            for (int y = 0; y < maxSize; y++)
                for (int x = 0; x < maxSize; x++){
                    if (y < height && x < width)
                        grid[x, y].Unmark(slimWalls);
                }
        }

        // Start and End points
        grid[0 + mode, 0].Mark();
        grid[width - 1 - mode, height - 1].Mark();

        yield return null;
    }

    private ICell GetRandomNonVisitedNeighbour(ICell[,] grid, int x, int y, int mode, int width, int height)
    {
        List<ICell> availableNeighbours = new List<ICell>();

        //NORTH
        int neighbourY = y + 1 + mode; // mode = 0 (SLIM) || mode = 1 (THICK)
        if (neighbourY < height - mode && !grid[x, neighbourY].HasBeenVisited)
            availableNeighbours.Add(grid[x, neighbourY]);

        // SOUTH
        neighbourY = y - 1 - mode;
        if (neighbourY > -1 + mode && !grid[x, neighbourY].HasBeenVisited)
            availableNeighbours.Add(grid[x, neighbourY]);

        // WEST
        int neighbourX = x + 1 + mode;
        if (neighbourX < width - mode && !grid[neighbourX, y].HasBeenVisited)
            availableNeighbours.Add(grid[neighbourX, y]);

        // EAST
        neighbourX = x - 1 - mode;
        if (neighbourX > -1 + mode && !grid[neighbourX, y].HasBeenVisited)
            availableNeighbours.Add(grid[neighbourX, y]);

        if (availableNeighbours.Count > 0)
            return availableNeighbours[Random.Range(0, availableNeighbours.Count)];
        else
            return null;
    }

    private void JoinWithNeighbour(ICell currentCell, ICell neighbourCell, ICell[,] grid, int mode, int width, int height)
    {
        Vector2Int cIndex = currentCell.Index;
        Vector2Int nIndex = neighbourCell.Index;

        if (mode == 0) { // If slim walls ( mode == 0 )
            currentCell.RemoveWallWithNeighbour(nIndex.x, nIndex.y);
            neighbourCell.RemoveWallWithNeighbour(cIndex.x, cIndex.y);
        }
        else { // The cell inbetween two neighbour cells
            int inbetweenCellX = cIndex.x - (cIndex.x - nIndex.x) / 2;
            int inbetweenCellY = cIndex.y - (cIndex.y - nIndex.y) / 2;
            currentCell.TurnIntoPassage();
            neighbourCell.TurnIntoPassage();
            grid[inbetweenCellX, inbetweenCellY].TurnIntoPassage();
        }
    }
}