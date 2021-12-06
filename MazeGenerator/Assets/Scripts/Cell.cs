using UnityEngine;

public class Cell : MonoBehaviour, ICell
{
    private bool _isBlocked = true;
    public bool IsBlocked { get { return _isBlocked; } }

    private bool _processing;
    public bool Processing => _processing;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private LineRenderer NorthWall;
    [SerializeField] private LineRenderer SouthWall;
    [SerializeField] private LineRenderer EastWall;
    [SerializeField] private LineRenderer WestWall;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer.enabled) { 
            spriteRenderer.sprite = SpriteLibrary.CellSprite;
            spriteRenderer.material = SpriteLibrary.Wall;
        }

        NorthWall.material = SpriteLibrary.Wall;
        SouthWall.material = SpriteLibrary.Wall;
        EastWall.material = SpriteLibrary.Wall;
        WestWall.material = SpriteLibrary.Wall;

        gameObject.SetActive(false);
    }

    public void SetName(string name)
    {
        gameObject.name = name;
    }

    public Vector3 GetPosition() => transform.position;

    public void Reset(bool slim) {
        gameObject.SetActive(true);      
        ShowSlimWalls(slim);
        _isBlocked = true;
        _processing = false;
        if (spriteRenderer != null) spriteRenderer.material = SpriteLibrary.Wall;
    }

    public void Disable() => gameObject.SetActive(false);

    public Vector2Int GetIndex() => new Vector2Int((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f));

    public void Process()
    {
        _processing = true;
        spriteRenderer.enabled = true;
        spriteRenderer.material = SpriteLibrary.Frontier;
    } 

    public void TurnIntoEndPoint()
    {
        spriteRenderer.enabled = true;
        spriteRenderer.material = SpriteLibrary.Frontier;
    }

    public void ShowSlimWalls(bool condition) {
        spriteRenderer.enabled = !condition;
        NorthWall.gameObject.SetActive(condition);
        SouthWall.gameObject.SetActive(condition);
        EastWall.gameObject.SetActive(condition);
        WestWall.gameObject.SetActive(condition);
    }

    public void TurnIntoPassage()
    {
        _isBlocked = false;
        if (spriteRenderer != null) spriteRenderer.material = SpriteLibrary.Passage;
    }

    public void RemoveWallWithNeighbour(int neighbourX, int neighbourY)
    {
        if (transform.position.x < neighbourX + 0.5f)
            EastWall.gameObject.SetActive(false);
        else if (transform.position.x > neighbourX + 0.5f)
            WestWall.gameObject.SetActive(false);
        else if (transform.position.y < neighbourY + 0.5f)
            NorthWall.gameObject.SetActive(false);
        else
            SouthWall.gameObject.SetActive(false);

        _isBlocked = false;
        spriteRenderer.enabled = false;
    }
}