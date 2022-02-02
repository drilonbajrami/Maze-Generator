using System.Collections;

public interface IMazeAlgorithm 
{
    public string Name { get; }
    IEnumerator Run(ICell[,] grid, int width, int height, int maxSize, Mode pMode, float waitSeconds);
}