using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool _isWall = true;
    public bool IsWall => _isWall;

    private bool _processing;
    public bool Processing => _processing;

    private SpriteRenderer spriteRenderer;

    public LineRenderer NorthWall;
    public LineRenderer SouthWall;
    public LineRenderer EastWall;
    public LineRenderer WestWall;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(spriteRenderer != null) { 
            spriteRenderer.sprite = SpriteLibrary.CellSprite;
            spriteRenderer.material = SpriteLibrary.Wall;
        }
        NorthWall.material = SpriteLibrary.ThinWall;
        SouthWall.material = SpriteLibrary.ThinWall;
        EastWall.material = SpriteLibrary.ThinWall;
        WestWall.material = SpriteLibrary.ThinWall;
        gameObject.SetActive(false);
    }

    private void OnDisable() => MakeWall();
    public void Enable() => gameObject.SetActive(true);
    public void Disable() => gameObject.SetActive(false);
    public Vector2Int GetIndex() => new Vector2Int((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f));

    public void Process()
    {
        _processing = true;
        if (spriteRenderer != null) spriteRenderer.material = SpriteLibrary.Frontier;
    }


    public void MakeWall()
    {
        _isWall = true;
        _processing = false;
        if (spriteRenderer != null) spriteRenderer.material = SpriteLibrary.Wall;
    }

    public void MakePass()
    {
        _isWall = false;
        _processing = false;
        if (spriteRenderer != null) spriteRenderer.material = SpriteLibrary.Passage;
    }

    public void MakeEnterExit()
    {
        if (spriteRenderer != null) spriteRenderer.material = SpriteLibrary.Frontier;
    }

    public void ApplyWalls() {
        NorthWall.gameObject.SetActive(true);
        SouthWall.gameObject.SetActive(true);
        EastWall.gameObject.SetActive(true);
        WestWall.gameObject.SetActive(true);
    }

    public void RemoveNorthWall() {
        NorthWall.gameObject.SetActive(false);
        _isWall = false;
        _processing = false;
    }
    public void RemoveSouthWall() { 
        SouthWall.gameObject.SetActive(false);
        _isWall = false;
        _processing = false;
    }
    public void RemoveEastWall() {
        EastWall.gameObject.SetActive(false);
        _isWall = false;
        _processing = false;
    }
    public void RemoveWestWall() { 
        WestWall.gameObject.SetActive(false);
        _isWall = false;
        _processing = false;
    }
}