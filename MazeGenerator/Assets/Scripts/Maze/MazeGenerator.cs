using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum Mode { SLIM = 0, THICK = 1 }

public class MazeGenerator : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject UI;
    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;
    public Button generateButton;
    public Toggle slimModeToggle;
    public Slider animationTimeSlider;
    public TMP_Dropdown algorithmSelectDropdown;
    public Button playButton;
    public Button stopButton;
    public GameObject endGamePanel;

    public CameraPan cameraPan;

    public static int MAZE_WIDTH;
    public static int MAZE_HEIGHT;

    private const int MIN_SIZE = 10;
    private const int MAX_SIZE = 50;

    [Space(20)]
    [Header("Input")]
    public GameObject CellPrefab;
    private GameObject Container;
    public ICell[,] grid;

    public int widthInput;
    public int heightInput;

    public Mode MODE;

    private bool generated = false;

    [Range(0f, 1f)]
    public float seconds;

    public List<IMazeAlgorithm> algorithms;
    private IMazeAlgorithm selectedAlgorithm;

    public Player player;
    private bool isPlaying = false;
    private float timer = 0.0f;

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
        algorithmSelectDropdown.onValueChanged.AddListener(delegate { OnAlgorithmSelected(); });
        playButton.onClick.AddListener(Play);
        stopButton.onClick.AddListener(Stop);

        OnWidthValueChanged("10");
        OnHeightValueChanged("10");

        algorithms = new List<IMazeAlgorithm>
        {
            (IMazeAlgorithm)new Prims(),
            (IMazeAlgorithm)new RecursiveBackTracker(),
            (IMazeAlgorithm)new BinaryTree(),
            (IMazeAlgorithm)new Ellers(),
        };

        algorithmSelectDropdown.options.Clear();

        foreach (IMazeAlgorithm algorithm in algorithms)
            algorithmSelectDropdown.options.Add(new TMP_Dropdown.OptionData(algorithm.Name));

        selectedAlgorithm = (IMazeAlgorithm)algorithms[0];
        algorithmSelectDropdown.RefreshShownValue();
    }

    private void Update()
    {
        if (isPlaying) timer += Time.deltaTime;
    }

    /// <summary> 
    /// Listener method for 'Generate' button onClick event
    /// </summary>
    private void GenerateMaze()
    {
        StopAllCoroutines();
        StartCoroutine(selectedAlgorithm.Run(grid, widthInput, heightInput, MAX_SIZE, MODE, seconds));
        if (!playButton.gameObject.activeSelf) playButton.gameObject.SetActive(true);
        cameraPan.UpdateCameraSize();
    }

    private void Play()
    {
        UI.SetActive(false);
        player.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(true);
        isPlaying = true;
        timer = 0.0f;
    }

    public void Stop()
    {
        UI.SetActive(true);
        stopButton.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        isPlaying = false;
    }

    public void EndGame()
    {
        endGamePanel.SetActive(true);
        int min = Mathf.FloorToInt(timer / 60);
        int sec = Mathf.FloorToInt(timer % 60);
        endGamePanel.transform.GetChild(1).GetComponent<TMP_Text>().text = min.ToString("00") + ":" + sec.ToString("00");
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
        if (w < MIN_SIZE || w > MAX_SIZE)
        {
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
        if (h < MIN_SIZE || h > MAX_SIZE)
        {
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

    void OnAlgorithmSelected()
    {
        selectedAlgorithm = algorithms[algorithmSelectDropdown.value];
    }

    public void Quit() => Application.Quit();
    #endregion
}
