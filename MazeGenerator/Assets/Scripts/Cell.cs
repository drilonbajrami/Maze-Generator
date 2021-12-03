using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool _isWall = true;
    public bool IsWall => _isWall;

    private bool _processing;
    public bool Processing => _processing;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteLibrary.CellSprite;
        spriteRenderer.material = SpriteLibrary.Wall;
        gameObject.SetActive(false);
    }

    private void OnDisable() => MakeWall();

    public void Process()
    {
        _processing = true;
        spriteRenderer.material = SpriteLibrary.Frontier;
    }


    public void MakeWall()
    {
        _isWall = true;
        _processing = false;
        spriteRenderer.material = SpriteLibrary.Wall;
    }

    public void MakePass()
    {
        _isWall = false;
        _processing = false;
        spriteRenderer.material = SpriteLibrary.Passage;
    }

    public void Enable() => gameObject.SetActive(true);
    public void Disable() => gameObject.SetActive(false);
    public Vector2Int GetIndex() => new Vector2Int((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f));
}
