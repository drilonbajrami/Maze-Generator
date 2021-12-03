using UnityEngine;

public class SpriteLibrary : MonoBehaviour
{
    public static Sprite CellSprite;
    public static Material Wall;
    public static Material Passage;
    public static Material Frontier;

    private void Awake()
    {
        CellSprite = Resources.Load<Sprite>("Cell");
        Wall = Resources.Load<Material>("Wall");
        Passage = Resources.Load<Material>("Passage");
        Frontier = Resources.Load<Material>("Frontier");
        //if (CellSprite != null) Debug.Log("Sprite loaded");
        //if (Wall != null) Debug.Log("Wall loaded");
        //if (Passage != null) Debug.Log("Passage loaded");
    }
}
