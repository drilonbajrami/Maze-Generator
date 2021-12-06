using UnityEngine;
using UnityEngine.UI;

public enum Mode { SLIM = 0, THICK = 1 }

public class MazeGenerator : MonoBehaviour
{
    [Header("UI Elements")]
    public InputField widthInputField;
    public InputField heightInputField;
    public Button generateButton;
    public Toggle slimModeToggle;
    public Slider animationTimeSlider;

    public static int MAZE_WIDTH;
    public static int MAZE_HEIGHT;

    private const int MIN_SIZE = 10;
    private const int MAX_SIZE = 250;
    
    [Space(20)]
    [Header("Input")]
    public GameObject CellPrefab;
    private GameObject Container;
    private ICell[,] grid;

    public int widthInput;
    public int heightInput;

    public Mode MODE;

    [Range(0f, 1f)]
    public float seconds;

    PrimsAlgorithm primsAlgorithm = new PrimsAlgorithm();

    void Start()
    {
        // Create an empty game object to store in the generated cells
        Container = Instantiate(new GameObject("Container"), transform);
        grid = GenerateCells(MAX_SIZE, CellPrefab, Container.transform);

        // Add Listeners to UI elements
        generateButton.onClick.AddListener(GenerateMaze);
        widthInputField.onEndEdit.AddListener(OnWidthValueChanged);
        heightInputField.onEndEdit.AddListener(OnHeightValueChanged);
        slimModeToggle.onValueChanged.AddListener(OnWallModeValueChanged);
        animationTimeSlider.onValueChanged.AddListener(OnAnimationTimeSliderValueChanged);
    }

    /// <summary> 
    /// Listener method for 'Generate' button onClick event
    /// </summary>
    private void GenerateMaze()
    {
        StopAllCoroutines();
        StartCoroutine(primsAlgorithm.Run(grid, widthInput, heightInput, MAX_SIZE, MODE, seconds));
        
    }

    /// <summary>
    /// Returns a grid of cells with max given dimensions.
    /// </summary>
    private ICell[,] GenerateCells(int maxSize, GameObject cellPrefab, Transform parent)
    {
        ICell[,] grid = new ICell[maxSize, maxSize];

        for (int y = 0; y < maxSize; y++)
            for (int x = 0; x < maxSize; x++)
            {
                GameObject cellGO = Instantiate(cellPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity, parent);
                grid[x, y] = cellGO.GetComponent<ICell>();
                grid[x, y].SetName($"{x + 1}:{y + 1}");
            }

        return grid;
    }

    #region UI Event Listener Methods
    private void OnWidthValueChanged(string width)
    {
        if (width == string.Empty) width = MIN_SIZE.ToString();
        
        int w = int.Parse(width);
        if (w < MIN_SIZE || w > MAX_SIZE) {
            widthInput = Mathf.Clamp(w, MIN_SIZE, MAX_SIZE);
            widthInputField.text = widthInput.ToString();
        }
        else widthInput = w;  

        if (MODE == Mode.THICK) // if mode is thick walls
        {
            //'even' number of dimensions FIX for thick wall mode
            if (widthInput % 2 == 0 && widthInput < MAX_SIZE) widthInput += 1;
            else if (widthInput == MAX_SIZE) widthInput -= 1;
        }

        MAZE_WIDTH = widthInput;
    }

    private void OnHeightValueChanged(string height)
    {
        if (height == string.Empty) height = MIN_SIZE.ToString();

        int h = int.Parse(height);
        if (h < MIN_SIZE || h > MAX_SIZE) {
            heightInput = Mathf.Clamp(h, MIN_SIZE, MAX_SIZE);
            heightInputField.text = heightInput.ToString();     
        }
        else heightInput = h;

        if (MODE == Mode.THICK) // if mode is thick walls
        {
            //'even' number of dimensions FIX for thick wall mode
            if (heightInput % 2 == 0 && heightInput < MAX_SIZE) heightInput += 1;
            else if (heightInput == MAX_SIZE) heightInput -= 1;
        }

        MAZE_HEIGHT = heightInput;
    }

    private void OnWallModeValueChanged(bool condition)
    {
        if (condition) MODE = Mode.SLIM;
        else MODE = Mode.THICK;

        widthInputField.onEndEdit.Invoke(widthInputField.text);
        heightInputField.onEndEdit.Invoke(heightInputField.text);
    }

    private void OnAnimationTimeSliderValueChanged(float pSeconds) => seconds = pSeconds;

    public void Quit() => Application.Quit();
    #endregion
}
