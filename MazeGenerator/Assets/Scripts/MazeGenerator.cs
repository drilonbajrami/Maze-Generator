using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

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

    [Range(0f, 10f)]
    public float interval = 1f;

    private const int FRONTIER = 2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            //StartCoroutine(GenerateMaze(width, height));
            GenerateMaze(width,height);
        }
    }

    void Start()
    {
        Container = Instantiate(new GameObject("Container"), transform);
        MAZE_WIDTH = 252;
        MAZE_HEIGHT = 252;
        GenerateCells();
    }

    public /*IEnumerator*/void GenerateMaze(int pWidth, int pHeight)
    {
        widthBorder = (pWidth < MIN_SIZE || pWidth > MAX_SIZE) ? Mathf.Clamp(pWidth, MIN_SIZE, MAX_SIZE) : pWidth;
        heightBorder = (pHeight < MIN_SIZE || pHeight > MAX_SIZE) ? Mathf.Clamp(pHeight, MIN_SIZE, MAX_SIZE) : pHeight;

        //EVEN AND ODD numbers FIX
        //Adding borders(We do not count borders in the actual size of the maze)
        //if (widthBorder % 2 == 0)
        //    widthBorder += 1;

        //if (heightBorder % 2 == 0)
        //    heightBorder += 1;

        MAZE_WIDTH = widthBorder;
        MAZE_HEIGHT = heightBorder;

        for (int y = 0; y < 250; y++)
            for (int x = 0; x < 250; x++)
            {
                grid[x, y].Disable();
            }

        for (int y = 0; y < heightBorder; y++)
            for (int x = 0; x < widthBorder; x++)
            {
                grid[x, y].Enable();
                grid[x, y].ApplyWalls();
            }

        List<Cell> frontiers = new List<Cell>();
        frontiers.Add(grid[1, 1]);
        Cell currentCell = frontiers[0];

        currentCell.MakePass();
        Vector2Int index = currentCell.GetIndex();
        frontiers.Remove(currentCell);
        GetFrontierNeighboursX(index.x, index.y, frontiers);
        if (frontiers.Count > 0)
            currentCell = frontiers[UnityEngine.Random.Range(0, frontiers.Count)];
        else currentCell = null;
        //yield return new WaitForSeconds(interval);

        //Debug.Log("While Loop Started");
        int counter = 0;
        while (currentCell != null)
        {
            currentCell.MakePass();
            index = currentCell.GetIndex();
            JoinRandomPassNeighbourX(index.x, index.y, frontiers);
            frontiers.Remove(currentCell);
            GetFrontierNeighboursX(index.x, index.y, frontiers);

            if (frontiers.Count > 0)
                currentCell = frontiers[UnityEngine.Random.Range(0, frontiers.Count)];
            else currentCell = null;
            //yield return new WaitForSeconds(interval);
        }

        //Debug.Log("While Loop Ended");
        // Start and End points
        grid[0, 0].MakeEnterExit();
        grid[widthBorder - 1, heightBorder - 1].MakeEnterExit();
    }

    private void GetFrontierNeighbours(int x, int y, List<Cell> frontierCells)
    {
        if (y > 0 && y < heightBorder - 1)
        {
            // NORTH
            int neighbourY = y + 2;
            if (neighbourY < heightBorder - 1 && grid[x, neighbourY].IsWall && !grid[x, neighbourY].Processing)
            {
                //Debug.Log("NORTH");
                grid[x, neighbourY].Process();
                frontierCells.Add(grid[x, neighbourY]);
            }
            // SOUTH
            neighbourY = y - 2;
            if (neighbourY > 0 && grid[x, neighbourY].IsWall && !grid[x, neighbourY].Processing)
            {
                //Debug.Log("SOUTH");
                grid[x, neighbourY].Process();
                frontierCells.Add(grid[x, neighbourY]);
            }
        }

        if (x > 0 && x < widthBorder - 1)
        {
            // WEST
            int neighbourX = x + 2;
            if (neighbourX < widthBorder - 1 && grid[neighbourX, y].IsWall && !grid[neighbourX, y].Processing)
            {
                //Debug.Log("WEST");
                grid[neighbourX, y].Process();
                frontierCells.Add(grid[neighbourX, y]);
            }
            // EAST
            neighbourX = x - 2;
            if (neighbourX > 0 && grid[neighbourX, y].IsWall && !grid[neighbourX, y].Processing)
            {
                //Debug.Log("EAST");
                grid[neighbourX, y].Process();
                frontierCells.Add(grid[neighbourX, y]);
            }
        }
    }

    private void JoinRandomPassNeighbour(int x, int y, List<Cell> frontierCells)
    {
        List<Vector2Int> availablePassCells = new List<Vector2Int>();

        // NORTH
        int neighbourY = y + FRONTIER;
        if (neighbourY < heightBorder - 1 && !grid[x, neighbourY].IsWall)
            availablePassCells.Add(new Vector2Int(x, neighbourY));

        // SOUTH
        neighbourY = y - FRONTIER;
        if (neighbourY > 0 && !grid[x, neighbourY].IsWall)
            availablePassCells.Add(new Vector2Int(x, neighbourY));

        // WEST
        int neighbourX = x + FRONTIER;
        if (neighbourX < widthBorder - 1 && !grid[neighbourX, y].IsWall)
            availablePassCells.Add(new Vector2Int(neighbourX, y));

        // EAST
        neighbourX = x - FRONTIER;
        if (neighbourX > 0 && !grid[neighbourX, y].IsWall)
            availablePassCells.Add(new Vector2Int(neighbourX, y));

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

    private void GetFrontierNeighboursX(int x, int y, List<Cell> frontierCells)
    {
        // NORTH
        int neighbourY = y + 1;
        if (neighbourY < heightBorder && grid[x, neighbourY].IsWall && !grid[x, neighbourY].Processing) {
            grid[x, neighbourY].Process();
            frontierCells.Add(grid[x, neighbourY]);
        }
        // SOUTH
        neighbourY = y - 1;
        if (neighbourY >= 0 && grid[x, neighbourY].IsWall && !grid[x, neighbourY].Processing) {
            grid[x, neighbourY].Process();
            frontierCells.Add(grid[x, neighbourY]);
        }

        // WEST
        int neighbourX = x + 1;
        if (neighbourX < widthBorder && grid[neighbourX, y].IsWall && !grid[neighbourX, y].Processing) {
            grid[neighbourX, y].Process();
            frontierCells.Add(grid[neighbourX, y]);
        }
        // EAST
        neighbourX = x - 1;
        if (neighbourX >= 0 && grid[neighbourX, y].IsWall && !grid[neighbourX, y].Processing) {
            grid[neighbourX, y].Process();
            frontierCells.Add(grid[neighbourX, y]);
        }
    }

    private void JoinRandomPassNeighbourX(int x, int y, List<Cell> frontierCells)
    {
        List<Vector2Int> availablePassCells = new List<Vector2Int>();

        // NORTH
        int neighbourY = y + 1;
        if (neighbourY < heightBorder && !grid[x, neighbourY].IsWall)
            availablePassCells.Add(new Vector2Int(x, neighbourY));

        // SOUTH
        neighbourY = y - 1;
        if (neighbourY >= 0 && !grid[x, neighbourY].IsWall)
            availablePassCells.Add(new Vector2Int(x, neighbourY));

        // WEST
        int neighbourX = x + 1;
        if (neighbourX < widthBorder && !grid[neighbourX, y].IsWall)
            availablePassCells.Add(new Vector2Int(neighbourX, y));

        // EAST
        neighbourX = x - 1;
        if (neighbourX >= 0 && !grid[neighbourX, y].IsWall)
            availablePassCells.Add(new Vector2Int(neighbourX, y));


        if (availablePassCells.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, availablePassCells.Count);
            Vector2Int neighbourIndex = availablePassCells[index];

            if (x < neighbourIndex.x) {
                grid[x, y].RemoveEastWall();
                grid[neighbourIndex.x, neighbourIndex.y].RemoveWestWall();
            }
            else if (x > neighbourIndex.x) {
                grid[x, y].RemoveWestWall();
                grid[neighbourIndex.x, neighbourIndex.y].RemoveEastWall();
            }
            else if (y < neighbourIndex.y) {
                grid[x, y].RemoveNorthWall();
                grid[neighbourIndex.x, neighbourIndex.y].RemoveSouthWall();
            }
            else {
                grid[x, y].RemoveSouthWall();
                grid[neighbourIndex.x, neighbourIndex.y].RemoveNorthWall();
            }
            
            frontierCells.Remove(grid[x, y]);
        }
    }

    public void GenerateCells()
    {
        grid = new Cell[250, 250];

        for (int y = 0; y < 250; y++)
            for (int x = 0; x < 250; x++)
            {
                GameObject cellGO = Instantiate(CellPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity, Container.transform);
                grid[x, y] = cellGO.GetComponent<Cell>();
                grid[x, y].gameObject.name = $"{x + 1}:{y + 1}";
                grid[x, y].ApplyWalls();
            }
    }
}
