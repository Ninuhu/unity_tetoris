using UnityEngine;

public class Tetorismino : MonoBehaviour
{
    public float fallTime = 1.0f;
    private float timer = 0f;
    public float softDropMultiplier = 0.3f; //softドロップ用
    private float defaultFallTime;

    public  bool isLocking = false; //着地(動かなくなったかの判定のためのやつ)
    bool isLocked = false; //二つ使う
    public GameObject prefabOriginal;  //ホールド用
    
    public bool canHold = true; //holdできるか

    public bool isHolding = false; // holdがgridに選択されないように

    public GameObject ghostPrefab;  // ゴーストテト
    private GameObject ghostInstance;

    public AudioClip lockSound;  // 固定時に効果音
    private AudioSource audioSource;


    void Update()
    {
        timer += Time.deltaTime;

        // 自動落下
        if(timer >= fallTime)
        {
            Move(Vector2.down);
            timer = 0;
        }
        if (ghostInstance == null) CreateGhost(); //生成（複数出ないように）

        HandleInput();
        UpdateGhost();
    }
    void Start()
    {
        defaultFallTime = fallTime;
        audioSource = GetComponent<AudioSource>(); //参照　おとを
    }


    //　入力
    void HandleInput()
    {
        //  左 ←　
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector2.left);

        // 右  →
        if(Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector2.right);
        
        // 降下  ↓
        if(Input.GetKey(KeyCode.DownArrow))
            fallTime = defaultFallTime * softDropMultiplier;
        else
        {
            fallTime = defaultFallTime; //はなしたら元の速さになる
        }
        
        // ハードドロップ  Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }


        // Hold   LeftShift
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log(" hold 押された");
            FindObjectOfType<HoldManager>().Hold(this);
        }

        //  ホールドを使う（今のミノを破棄して置換） RightShift
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Debug.Log(" 置換押された");
            FindObjectOfType<HoldManager>().UseHold(this);
        }

        //　回転  ↑
        if(Input.GetKeyDown(KeyCode.UpArrow))
            Rotate();
    }
    public void SetFallTime(float newFallTime)
    {
        fallTime = newFallTime;
        defaultFallTime = newFallTime;
    }



    void Move(Vector2 dir)
    {
        if(isLocked || isLocking) return;
        transform.position += (Vector3)dir;
        if(!IsValid())
        {
            transform.position -= (Vector3)dir;
            if(dir == Vector2.down)
            {
                isLocking = true;   // ここで二重予約防止
                Invoke(nameof(LockMino),0.05f);
            }
        }
    }


    void LockMino()
    {
        if(isHolding) return; //hold中は固定しない
        if(isLocked || GameManager.isGameOver) return;
        isLocked = true;

        //固定時に効果音
        if (audioSource != null && lockSound != null)
        {
            audioSource.PlayOneShot(lockSound);
        }

        //grid追加
        AddToGrid();

        if (ghostInstance != null)
        Destroy(ghostInstance);  // ゴースト消す

        if(!GameManager.isGameOver)   //gameoverのときにspawnさせない
        FindObjectOfType<Spawner>().SpawnNext();
        enabled = false;
    }

    void Rotate()
    {
        transform.Rotate(0,0,90);
        if(!IsValid()) transform.Rotate(0,0,-90);
    }



    bool IsValid()
    {
        foreach(Transform block in transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);

            // 範囲がいか
            if(x <0 || x >= GameManager.width || y <0) return false;

            // 壁 or 床に当たってるか
            if(!GameManager.IsInsideGrid(x,y)) return false;

            // 既にブロックがあるか
            if(y < GameManager.height && GameManager.grid[x,y] != null)return false;
 
        }
        return true;
    }

    // Grid(マス目での固定)
    void AddToGrid()
    {
        if(isHolding) return;    // ホールド中は登録しない
        foreach(Transform block in transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);

            if(y >= GameManager.height) //ゲームオーバー
            {
                FindObjectOfType<GameManager>().GameOver();
                return;
            }


            if(x <0 || x >= GameManager.width || y <0 || y >= GameManager.height)
            return; //配列外のアクセスなしに
            // 上に突き抜けてたらGameOver
            
            
        }
        // ここまで来たら安全
        foreach(Transform block in transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            GameManager.grid[x,y] = block;
        }
        GameManager.CheckLines();
    }
    //Ghostミノ作成
    void CreateGhost()
    {
        if (ghostPrefab == null || ghostInstance != null) return;
        ghostInstance = Instantiate(ghostPrefab);  // 親はつけない
    }
    void UpdateGhost()
    {
        if (ghostInstance == null) return;
        ghostInstance.transform.position = transform.position;
        ghostInstance.transform.rotation = transform.rotation;


        // 下に行けるだけ落とす
        while (true)
        {
            ghostInstance.transform.position += Vector3.down;
            if (!IsValidGhost())
            {
                ghostInstance.transform.position -= Vector3.down;
                break;
            }
        }
    }
    bool IsValidGhost()
    {
        foreach (Transform block in ghostInstance.transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            if (x <0 || x >= GameManager.width || y <0) return false;
            if (y < GameManager.height && GameManager.grid[x,y] != null) return false;
        }
        return true;
    }
    public void DestroyGhost()
    {
        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
        }
    }

    //ハードドロップ関数
    void HardDrop()
    {
        if (isLocked || isLocking) return;
        // 下に行けるだけ落とす
        while (true)
        {
            transform.position += Vector3.down;
            if (!IsValid())
            {
                transform.position -= Vector3.down;
                break;
            }
        }
        // 即ロック
        CancelInvoke(nameof(LockMino)); // 予約されてたら解除
        LockMino();
    }


}