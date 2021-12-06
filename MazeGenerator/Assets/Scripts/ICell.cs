using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICell
{
    // Used for setting the cell's gameObject name
    public void SetName(string name);
    public Vector3 GetPosition();

    public bool IsBlocked { get; }
    public bool Processing { get; }

    /// <summary>
    /// Reset cell data
    /// </summary>
    /// <param name="mode"></param>
    public void Reset(bool mode);

    /// <summary>
    /// Disable cell gameObject
    /// </summary>
    public void Disable();

    public Vector2Int GetIndex();

    /// <summary>
    /// Process the cell, so there are no duplicates of it in the frontier list of cells
    /// </summary>
    public void Process();

    /// <summary>
    /// Used for turning a cell into an Start or End point
    /// </summary>
    public void TurnIntoEndPoint();

    /// <summary>
    /// Show walls based on given Wall Mode ( true for slim walls && false for thick walls)
    /// </summary>
    /// <param name="condition"></param>
    public void ShowSlimWalls(bool condition);

    /// <summary>
    /// Turn cell into a passage
    /// </summary>
    public void TurnIntoPassage();

    /// <summary>
    /// Remove wall with neighbour based on this cell's index and neighbour cell's index
    /// </summary>
    /// <param name="neighbourX"></param>
    /// <param name="neighbourY"></param>
    public void RemoveWallWithNeighbour(int neighbourX, int neighbourY);
}