using UnityEngine;

public class CameraPan : MonoBehaviour
{
    // Cache the camera
    Camera cam;

    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;

    // Constraints
    Vector2 xAxisConstraints;
    Vector2 yAxisConstraints;

    // Cache the current screen width in case we switch resolutions
    int currentResWidth;
    float screenWidthHeightRatio;
    int lastGeneratedWidth;
    int lastGeneratedHeight;

    // Cache the current camera orthographic size in case we change it
    float camOrthoSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        //currentResWidth = Screen.width;
        //camOrthoSize = cam.orthographicSize;
        screenWidthHeightRatio = (float)Screen.width / Screen.height;
        //lastGeneratedWidth = MazeGenerator.MAZE_WIDTH;
        //lastGeneratedHeight = MazeGenerator.MAZE_HEIGHT;
        //xAxisConstraints = GetXAxisConstraints((float)MazeGenerator.MAZE_WIDTH);
        //yAxisConstraints = GetYAxisConstraints((float)MazeGenerator.MAZE_HEIGHT);
        //UpdateCameraTransform();
    }

    void Update()
    {
        //if (currentResWidth != Screen.width || camOrthoSize != cam.orthographicSize)
        //{
        //    currentResWidth = Screen.width;
        //    camOrthoSize = cam.orthographicSize;
        //    screenWidthHeightRatio = (float)Screen.width / Screen.height;
        //    UpdateCameraTransform();
        //}

        //if(lastGeneratedWidth != MazeGenerator.MAZE_WIDTH || lastGeneratedHeight != MazeGenerator.MAZE_HEIGHT)
        //{
        //    lastGeneratedWidth = MazeGenerator.MAZE_WIDTH;
        //    lastGeneratedHeight = MazeGenerator.MAZE_HEIGHT;
        //    UpdateCameraTransform();
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    hit_position = Input.mousePosition;
        //    camera_position = transform.position;
        //}
        //if (Input.GetMouseButton(0))
        //{
        //    current_position = Input.mousePosition;
        //    UpdateCameraTransform();
        //}
    }

    void UpdateCameraTransform()
    {
        Vector3 direction = cam.ScreenToWorldPoint(current_position) - cam.ScreenToWorldPoint(hit_position);

        // Invert direction to that terrain appears to move with the mouse.
        direction = direction * -1;

        Vector3 position = camera_position + direction;

        // Get constraints
        xAxisConstraints = GetXAxisConstraints((float)MazeGenerator.MAZE_WIDTH);
        yAxisConstraints = GetYAxisConstraints((float)MazeGenerator.MAZE_HEIGHT);

        // Clamp position based on axis constraints
        position.x = Mathf.Clamp(position.x, xAxisConstraints.x, xAxisConstraints.y);
        position.y = Mathf.Clamp(position.y, yAxisConstraints.x, yAxisConstraints.y);
        position.z = transform.position.z;

        transform.position = position;
    }

    private Vector2 GetXAxisConstraints(float width)
    {
        float xMinimum = screenWidthHeightRatio * camOrthoSize;
        float xMaximum = width <= camOrthoSize * 2 * screenWidthHeightRatio ? xMinimum : width - xMinimum;
        return new Vector2(xMinimum, xMaximum);
    }

    private Vector2 GetYAxisConstraints(float height)
    {
        float yMinimum = camOrthoSize;
        float yMaximum = height <= camOrthoSize * 2 ? yMinimum : height - yMinimum;
        return new Vector2(yMinimum, yMaximum);
    }

    public void UpdateCameraSize()
    {
        if (MazeGenerator.MAZE_WIDTH / 2 + 3 > MazeGenerator.MAZE_HEIGHT)
            cam.orthographicSize = MazeGenerator.MAZE_WIDTH / screenWidthHeightRatio / 2;  
        else
            cam.orthographicSize = (float)MazeGenerator.MAZE_HEIGHT / 2;

        cam.gameObject.transform.position = new Vector3((float)MazeGenerator.MAZE_WIDTH / 2, (float)MazeGenerator.MAZE_HEIGHT / 2, -10);
    }
}