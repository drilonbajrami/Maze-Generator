using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ellers : IMazeAlgorithm
{
    public string Name { get => "Ellers's"; }

    public IEnumerator Run(ICell[,] grid, int width, int height, int maxSize, Mode pMode, float waitSeconds)
    {
        // Check maze selected mode
        bool slimWalls = pMode == Mode.SLIM;
        int mode = (int)pMode;

        // Reset all cells based on given width and height
        for (int y = 0; y < maxSize; y++)
            for (int x = 0; x < maxSize; x++) {
                if (y < height && x < width) {
                    grid[x, y].Reset(slimWalls);
                    grid[x, y].Set = -1;
                }
                else grid[x, y].Disable();
            }

        // Algorithm 
        Dictionary<int, List<ICell>> sets = new Dictionary<int, List<ICell>>();
        int setIndex = 0;
        int row = height - 1 - mode;

        while (row > -1 + mode) {
            // Set set index for each cell if there is no index (set == -1 || set < 0)
            for (int x = 0; x < width - mode; x++) {
                if (grid[x, row].Set < 0) {
                    grid[x, row].Set = setIndex;
                    setIndex++;
                }
            }

            for (int x = 0 + mode; x < width - 1 - mode; x += 1 + mode) {
                ICell a = grid[x, row];
                ICell b = grid[x + 1 + mode, row];

                a.Mark();
                if (waitSeconds > 0.0f) yield return new WaitForSeconds(waitSeconds);

                if (row <= mode) {
                    if (mode == 0) JoinCells(a, b);
                    else JoinCells(a, b, grid[a.Index.x + 1, row]);
                } else {
                    if (sets.Count == 0) {
                        if (a.Set == b.Set) sets.Add(a.Set, new List<ICell> { a, b });
                        else if (UnityEngine.Random.Range(0, 2) == 0) {
                            sets.Add(a.Set, new List<ICell> { a, b });
                            b.Mark();
                            if (waitSeconds > 0.0f) yield return new WaitForSeconds(waitSeconds);
                            if (mode == 0) JoinCells(a, b);                 
                            else JoinCells(a, b, grid[a.Index.x + 1, row]);
                            b.Set = a.Set;
                        } else {
                            sets.Add(a.Set, new List<ICell> { a });
                            sets.Add(b.Set, new List<ICell> { b });
                        }
                    }
                    else if (a.Set != b.Set) {
                        if (UnityEngine.Random.Range(0, 2) == 0) {
                            b.Mark();
                            if (waitSeconds > 0.0f) yield return new WaitForSeconds(waitSeconds);
                            if (mode == 0) JoinCells(a, b);
                            else JoinCells(a, b, grid[a.Index.x + 1, row]);
                            if (sets.ContainsKey(a.Set)) sets[a.Set].Add(b);
                            else sets.Add(a.Set, new List<ICell>() { a, b });
                            if (sets.ContainsKey(b.Set)) sets[b.Set].Remove(b);
                            b.Set = a.Set;
                        } else {
                            if (!sets.ContainsKey(a.Set)) sets.Add(a.Set, new List<ICell> { a });
                            else if (!sets[a.Set].Contains(a)) sets[a.Set].Add(a);

                            if (!sets.ContainsKey(b.Set)) sets.Add(b.Set, new List<ICell> { b });
                            else if (!sets[b.Set].Contains(b)) sets[b.Set].Add(b);
                        }
                    }
                }

                a.Unmark(slimWalls);
            }

            // Vertical Connections
            foreach (KeyValuePair<int, List<ICell>> cellSet in sets) {
                if (cellSet.Value.Count > 0) {
                    int index = UnityEngine.Random.Range(0, cellSet.Value.Count);
                    ICell a = cellSet.Value[index];
                    ICell b = grid[a.Index.x, row - 1 - mode];
                    b.Set = a.Set;
                    sets[a.Set].Remove(a);
                    a.Mark();
                    if (waitSeconds > 0.0f) yield return new WaitForSeconds(waitSeconds);
                    b.Mark();
                    if (waitSeconds > 0.0f) yield return new WaitForSeconds(waitSeconds);
                    if (mode == 0) JoinCells(a, b);
                    else JoinCells(a, b, grid[a.Index.x, row - 1]);
                    a.Unmark(slimWalls);
                }
            }

            sets.Clear();
            row -= 1 + mode;
        }

        // Start and End points
        grid[0 + mode, 0].Mark();
        grid[width - 1 - mode, height - 1].Mark();

        yield return null;
    }

    private void JoinCells(ICell cellA, ICell cellB)
    {
        cellA.RemoveWallWithNeighbour(cellB);
        cellB.RemoveWallWithNeighbour(cellA);
    }

    private void JoinCells(ICell cellA, ICell cellB, ICell inbetweenCell)
    {
        cellA.TurnIntoPassage();
        cellB.TurnIntoPassage();
        inbetweenCell.TurnIntoPassage();
    }
}
