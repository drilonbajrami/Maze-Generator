using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICell
{
    #region Properties
    public Vector2Int Index { get; }
    public bool IsBlocked { get; }
    public bool IsBeingProcessed { get; }
    public bool HasBeenVisited { get; }
    public int Set { get; set; }
    #endregion

    public Vector3 GetPosition();
    public void SetName(string name);
    public void Reset(bool mazeMode);
    public void Disable();
    public void Visit();
    public void Process();
    public void Mark();
    public void Unmark(bool slimMode);
    public void TurnIntoPassage();
    public void RemoveWallWithNeighbour(int neighbourX, int neighbourY);
    public void RemoveWallWithNeighbour(ICell cell);
    public void MakePath();

    public bool HasNorthWall();
    public bool HasSouthWall();
    public bool HasWestWall();
    public bool HasEastWall();
    public bool IsPassage();
}