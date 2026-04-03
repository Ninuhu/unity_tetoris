using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public static int width = 30; //fieldの横（10ブロック分）
    public static int height = 36; //fieldの縦（1ブロック分）
    public static Transform[,] grid = new Transform[width, height];
    public static float cellSize = 3f; // 1マスの物理サイズ（3×3）
    // フィールド左端のワールドX
    public static float leftX = -15f; // ← 実際の値に合わせる

    public static float bottomY = -16f; // フィールド一番下のワールドY



    public static Vector2 RoundVector2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    //グリッドが範囲内かチェック
    public static bool IsInsideGrid(Vector2 pos)
    {
        return 
            (int)pos.x >= 0 &&
            (int)pos.x < width &&
            (int)pos.y >= 0 &&
            (int)pos.y < height;
    }

    //ブロックをグリッドに登録
   public static void AddToGrid(Transform block)
{
    int x = Mathf.RoundToInt((block.position.x - leftX) / cellSize);
    int y = Mathf.RoundToInt((block.position.y - bottomY) / cellSize);


    if (x >= 0 && x < width && y >= 0 && y < height)
    {
        grid[x, y] = block;
    
    }
    Debug.Log(block.position.y);

}




}
