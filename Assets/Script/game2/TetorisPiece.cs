using UnityEngine;

public class TetrisPiece : MonoBehaviour
{
    public float fallSpeed = 2.0f;    // 1秒に落ちるマス数（後で上げる）
    public float cellSize = 3.0f;     // 1マスの物理サイズ（3×3）

    private float worldFallSpeed;     // 実際の落下速度（ワールド座標用）
    public LayerMask groundLayer;
    public bool isLanded = false;
    public TetrisSpawner spawner;//field

    void Start()
    {
        // 「1秒に2マス」→「1秒に 2 * cellSize の距離を落ちる」
        worldFallSpeed = fallSpeed * cellSize;
    }

    void Update()
    {
        if (!isLanded)
        {
            Fall();
            CheckGround();
        }
    }
    void CheckGround()
    {
        foreach (Transform block in transform) // 子ブロックごと
        {
            RaycastHit2D hit = Physics2D.Raycast(
                block.position,               // 中心じゃなく子
                Vector2.down,0.05f,              // 半マスちょi
                groundLayer
            );
            if (hit.collider != null)
            {
                Land(); // 着地処理をまとめる
                return;
            }
        }
    
    }

void Land()
{
    if (isLanded) return;
    isLanded = true;

    SnapToGrid();        // ★先
    AddBlocksToGrid();   // ★後

    spawner.Spawn();
    Destroy(gameObject);
}




    //落下
    void Fall()
    {
        // 下にスムーズに落ちる
        transform.position += Vector3.down * worldFallSpeed * Time.deltaTime;
    }
    void SnapToGrid()
{
    // ★ グリッド座標に変換
    int ix = Mathf.RoundToInt((transform.position.x - TetrisGrid.leftX) / cellSize);
    int iy = Mathf.RoundToInt((transform.position.y - TetrisGrid.bottomY) / cellSize);

    // ★ ワールド座標に戻す
    float x = TetrisGrid.leftX + ix * cellSize;
    float y = TetrisGrid.bottomY + iy * cellSize;

    transform.position = new Vector3(x, y, transform.position.z);
}


    //grid固定
    void AddBlocksToGrid()
{
    foreach (Transform block in transform)
    {
        // ★ 位置はもう親で揃ってるので触らない
        TetrisGrid.AddToGrid(block);

        block.SetParent(null);
    }
}


} 