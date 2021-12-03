using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static int MAZE_WIDTH;
    public static int MAZE_HEIGHT;

    private const int MIN_SIZE = 10;
    private const int MAX_SIZE = 250;

    private Cell[,] grid;
    private GameObject Container;
    public GameObject CellPrefab;

    public int width;
    public int height;
    private int widthBorder;
    private int heightBorder;
    public int CC;

    private Vector2Int startCellCoord;
    private Vector2Int endCellCoords;
    private const int FRONTIER = 2;

    //private int lastGivenWidth;
    //private int lastGivenHeight;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateMaze(width, height);
    }

    void Start()
    {
        Container = Instantiate(new GameObject("Container"), transform);
        //lastGivenWidth = 252;
        //lastGivenHeight = 252;
        MAZE_WIDTH = 252;
        MAZE_HEIGHT = 252;
        GenerateCells();
    }

    public void GenerateMaze(int pWidth, int pHeight)
    {
        widthBorder = (pWidth < MIN_SIZE || pWidth > MAX_SIZE) ? Mathf.Clamp(pWidth, MIN_SIZE, MAX_SIZE) : pWidth;
        heightBorder = (pHeight < MIN_SIZE || pHeight > MAX_SIZE) ? Mathf.Clamp(pHeight, MIN_SIZE, MAX_SIZE) : pHeight;

        // EVEN AND ODD numbers FIX
        // Adding borders (We do not count borders in the actual size of the maze)
        if (widthBorder % 2 == 0)
            widthBorder += 1;

        if (heightBorder % 2 == 0)
            heightBorder += 1;

        MAZE_WIDTH = widthBorder;
        MAZE_HEIGHT = heightBorder;

        for (int y = 0; y < 252; y++)
            for (int x = 0; x < 252; x++)
            {
                grid[x, y].Disable();
            }

        for (int y = 0; y < heightBorder; y++)
            for (int x = 0; x < widthBorder; x++)
            {
                grid[x, y].Enable();
                //MakeWallOrPassage(x, y);
            }

        List<Cell> frontiers = new List<Cell>();
        frontiers.Add(grid[1, 1]);
        Cell currentCell = frontiers[0];

        currentCell.MakePass();
        Vector2Int index = currentCell.GetIndex();
        frontiers.Remove(currentCell);
        GetFrontierNeighbours(index.x, index.y, frontiers);
        Debug.Log($"For the first Cell there are {frontiers.Count} frontier neighbours");
        if (frontiers.Count > 0)
            currentCell = frontiers[UnityEngine.Random.Range(0, frontiers.Count)];
        else currentCell = null;

        Debug.Log("While Loop Started");
        int counter = 0;
        while (currentCell != null)
        {
            currentCell.MakePass();
            index = currentCell.GetIndex();
            JoinRandomPassNeighbour(index.x, index.y, frontiers);
            frontiers.Remove(currentCell);
            GetFrontierNeighbours(index.x, index.y, frontiers);

            if (frontiers.Count > 0)
                currentCell = frontiers[UnityEngine.Random.Range(0, frontiers.Count)];
            else currentCell = null;
        }

        Debug.Log("While Loop Ended");
        // Start and End points
        grid[1, 0].MakePass();
        grid[widthBorder - 2, heightBorder - 1].MakePass();
    }

    private void MakeWallOrPassage(int x, int y)
    {
        if (UnityEngine.Random.Range(0, 2) != 0)
            grid[x, y].MakeWall();
        else
            grid[x, y].MakePass();
    }

    private void GetFrontierNeighbours(int x, int y, List<Cell> frontierCells)
    {
        if (y > 0 && y < heightBorder - 1)
        {
            // NORTH
            int neighbourY = y + FRONTIER;
            if (neighbourY < heightBorder - 1 && grid[x, neighbourY].IsWall && !grid[x, neighbourY].Processing)
            {
                Debug.Log("NORTH");
                grid[x, neighbourY].Process();
                frontierCells.Add(grid[x, neighbourY]);
            }
            // SOUTH
            neighbourY = y - FRONTIER;
            if (neighbourY > 0 && grid[x, neighbourY].IsWall && !grid[x, neighbourY].Processing)
            {
                Debug.Log("SOUTH");
                grid[x, neighbourY].Process();
                frontierCells.Add(grid[x, neighbourY]);
            }
        }

        if (x > 0 && x < widthBorder - 1)
        {
            // WEST
            int neighbourX = x + FRONTIER;
            if (neighbourX < widthBorder - 1 && grid[neighbourX, y].IsWall && !grid[neighbourX, y].Processing)
            {
                Debug.Log("WEST");
                grid[neighbourX, y].Process();
                frontierCells.Add(grid[neighbourX, y]);
            }
            // EAST
            neighbourX = x - FRONTIER;
            if (neighbourX > 0 && grid[neighbourX, y].IsWall && !grid[neighbourX, y].Processing)
            {
                Debug.Log("EAST");
                grid[neighbourX, y].Process();
                frontierCells.Add(grid[neighbourX, y]);
            }
        }
    }

    private void JoinRandomPassNeighbour(int x, int y, List<Cell> frontierCells)
    {
        List<Vector2Int> availablePassCells = new List<Vector2Int>();

        if (y > 0 && y < heightBorder - 1)
        {
            // NORTH
            int neighbourY = y + FRONTIER;
            if (neighbourY < heightBorder - 1 && !grid[x, neighbourY].IsWall)
                availablePassCells.Add(new Vector2Int(x, neighbourY));
            // SOUTH
            neighbourY = y - FRONTIER;
            if (neighbourY > 0 && !grid[x, neighbourY].IsWall)
                availablePassCells.Add(new Vector2Int(x, neighbourY));
        }

        if (x > 0 && x < widthBorder - 1)
        {
            // WEST
            int neighbourX = x + FRONTIER;
            if (neighbourX < widthBorder - 1 && !grid[neighbourX, y].IsWall)
                availablePassCells.Add(new Vector2Int(neighbourX, y));
            // EAST
            neighbourX = x - FRONTIER;
            if (neighbourX > 0 && !grid[neighbourX, y].IsWall)
                availablePassCells.Add(new Vector2Int(neighbourX, y));
        }

        if (availablePassCells.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, availablePassCells.Count);
            Vector2Int nIndex = availablePassCells[index];
            int inbetweenCellX = x - (x - nIndex.x) / 2;
            int inbetweenCellY = y - (y - nIndex.y) / 2;
            grid[x, y].MakePass();
            grid[inbetweenCellX, inbetweenCellY].MakePass();
            frontierCells.Remove(grid[x, y]);
        }
    }

    public void GenerateCells()
    {
        grid = new Cell[252, 252];

        for (int y = 0; y < 252; y++)
            for (int x = 0; x < 252; x++)
            {
                GameObject cellGO = Instantiate(CellPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity, Container.transform);
                grid[x, y] = cellGO.GetComponent<Cell>();
                grid[x, y].gameObject.name = $"{x + 1}:{y + 1}";
            }
    }

    private void ChangeColors()
    {
        for (int y = 0; y < 252; y++)
            for (int x = 0; x < 252; x++)
            {
                if (UnityEngine.Random.Range(0, 2) > 0)
                    grid[x, y].MakeWall();
                else
                    grid[x, y].MakePass();
            }
    }

    private void UnusedCode()
    {
        //// Check if new given width and height values are smaller than older ones
        //// if smaller, then deactivate the unused ones.
        //if (width < lastGivenWidth && height < lastGivenHeight) {
        //    int fromWidth = lastGivenWidth - width + 2;
        //    int fromHeight = lastGivenHeight - height + 2;
        //    for (int y = 0; y < height; y++)
        //        for (int x = 0; x < width; x++) {
        //            if (x >= fromWidth || y >= fromHeight) grid[x, y].Disable();
        //            else //grid[x, y].MakeWall();
        //                MakeWallOrPassage(x, y);
        //        }
        //}
        //else if (width < lastGivenWidth) { // If only width has changed
        //    int fromWidth = lastGivenWidth - width + 2;
        //    for(int y = 0; y < height; y++)
        //        for (int x = 0; x < width; x++) 
        //            if (x >= fromWidth) grid[x, y].Disable();
        //            else //grid[x, y].MakeWall();
        //                MakeWallOrPassage(x, y);
        //}
        //else if (height < lastGivenHeight)
        //{ // If only height has changed
        //    int fromHeight = lastGivenHeight - height + 2;
        //    for (int y = 0; y < height; y++)
        //        for (int x = 0; x < width; x++)
        //            if (x >= fromHeight) grid[x, y].Disable();
        //            else //grid[x, y].MakeWall();
        //                MakeWallOrPassage(x, y);
        //}
        //else {
        //    // Otherwise enable more based on new given width and height
        //    // and reset each cell state to WALL
        //    for (int y = 0; y < height; y++)
        //        for(int x = 0; x < width; x++) {
        //            grid[x, y].Enable();
        //            //grid[x, y].MakeWall();
        //            MakeWallOrPassage(x, y);
        //        }
        //}
    }
}
