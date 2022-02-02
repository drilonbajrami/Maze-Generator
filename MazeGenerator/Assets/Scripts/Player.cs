using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private MazeGenerator maze;
    private ICell occupiedCell;
    Vector2 moveDirection;
    Vector2 endPoint;
    bool playing = false;

    private void OnEnable()
    {
        playing = true;
        occupiedCell = maze.grid[0 + (int)maze.MODE, 0];
        endPoint = new Vector2(maze.widthInput - 1 - (int)maze.MODE, maze.heightInput - 1);
        UpdatePosition();
    }

    void Update()
    {
        if(Input.anyKeyDown && playing)
        {
            if (Input.GetKeyDown(KeyCode.W)) moveDirection = Vector2.up;
            else if (Input.GetKeyDown(KeyCode.S)) moveDirection = Vector2.down;
            else if (Input.GetKeyDown(KeyCode.D)) moveDirection = Vector2.right;
            else if (Input.GetKeyDown(KeyCode.A)) moveDirection = Vector2.left;

            Move();
        }
    }

    public void Move()
    {
        if (maze.MODE == Mode.SLIM)
        {
            if (moveDirection == Vector2.up && occupiedCell.Index.y < maze.grid.GetLength(1) - 1 && !occupiedCell.HasNorthWall())
            {
                occupiedCell = maze.grid[occupiedCell.Index.x, occupiedCell.Index.y + 1];
            }
            else if (moveDirection == Vector2.down && occupiedCell.Index.y > 0 && !occupiedCell.HasSouthWall())
            {
                occupiedCell = maze.grid[occupiedCell.Index.x, occupiedCell.Index.y - 1];
            }
            else if (moveDirection == Vector2.left && occupiedCell.Index.x > 0 && !occupiedCell.HasWestWall())
            {
                occupiedCell = maze.grid[occupiedCell.Index.x - 1, occupiedCell.Index.y];
            }
            else if (moveDirection == Vector2.right && occupiedCell.Index.x < maze.grid.GetLength(0) - 1 && !occupiedCell.HasEastWall())
            {
                occupiedCell = maze.grid[occupiedCell.Index.x + 1, occupiedCell.Index.y];
            }
        }
        else
        {
            if (moveDirection == Vector2.up && occupiedCell.Index.y < maze.grid.GetLength(1) - 1)
            {
                if(maze.grid[occupiedCell.Index.x, occupiedCell.Index.y + 1].IsPassage())
                    occupiedCell = maze.grid[occupiedCell.Index.x, occupiedCell.Index.y + 1];
            }
            else if (moveDirection == Vector2.down && occupiedCell.Index.y > 0)
            {
                if (maze.grid[occupiedCell.Index.x, occupiedCell.Index.y - 1].IsPassage())
                    occupiedCell = maze.grid[occupiedCell.Index.x, occupiedCell.Index.y - 1];
            }
            else if (moveDirection == Vector2.left && occupiedCell.Index.x > 0)
            {
                if (maze.grid[occupiedCell.Index.x - 1, occupiedCell.Index.y].IsPassage())
                    occupiedCell = maze.grid[occupiedCell.Index.x - 1, occupiedCell.Index.y];
            }
            else if (moveDirection == Vector2.right && occupiedCell.Index.x < maze.grid.GetLength(0) - 1)
            {
                if (maze.grid[occupiedCell.Index.x + 1, occupiedCell.Index.y].IsPassage())
                    occupiedCell = maze.grid[occupiedCell.Index.x + 1, occupiedCell.Index.y];
            }
        }

        UpdatePosition();
        CheckIfDone();
    }

    public void UpdatePosition() => transform.position = occupiedCell.GetPosition();
    public void CheckIfDone()
    {     
        if (occupiedCell.Index == endPoint)
        {
            playing = false;
            maze.EndGame();
        }
    }
}