 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTree : IMazeAlgorithm
{
    public string Name { get => "Binary Tree"; }

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

        for (int y = mode; y < height; y += 1 + mode)
        {
            for (int x = mode; x < width; x += 1 + mode)
            {
                grid[x, y].Mark();
                if ((x == mode || x == 0) && (y == mode || y == 0)) grid[x, y].Unmark(slimWalls);
                if (waitSeconds > 0.0f) yield return new WaitForSeconds(waitSeconds);
                JoinRandomNeighbour(grid, x, y, mode, width, height);
            }
        }

        grid[0 + mode, 0].Mark();
        grid[width - 1 - mode, height - 1].Mark();
        yield return null;
    }

    private void JoinRandomNeighbour(ICell[,] grid, int x, int y, int mode, int width, int height)
    {
        List<ICell> neighbours = new List<ICell>();

        // SOUTH
        int neighbourY = y - 1 - mode;
        if (neighbourY > -1 + mode)
            neighbours.Add(grid[x, neighbourY]);

        int neighbourX = x - 1 - mode;
        if (neighbourX > -1 + mode)
            neighbours.Add(grid[neighbourX, y]);

        if (neighbours.Count > 0)
        {
            int index = Random.Range(0, neighbours.Count);
            Vector2Int nIndex = neighbours[index].Index;

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
