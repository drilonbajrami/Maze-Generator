using UnityEngine;

public class SpriteLibrary : MonoBehaviour
{
    public static Sprite CellSprite;
    public static Material Wall;
    public static Material Passage;
    public static Material Frontier;
    public static Material ThinWall;

    private void Awake()
    {
        CellSprite = Resources.Load<Sprite>("Cell");
        Wall = Resources.Load<Material>("Wall");
        Passage = Resources.Load<Material>("Passage");
        Frontier = Resources.Load<Material>("Frontier");
        ThinWall = Resources.Load<Material>("ThinWall");
    }
}
