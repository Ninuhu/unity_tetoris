using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static int width = 10; //よこ
    public static int height = 20;  //たて
    public static bool isGameOver = false; //ゲームオーバー処理
    public TextMeshProUGUI gameOverText;  //gameover

    public static int score = 0; //スコア
    public TextMeshProUGUI scoreText;
    public static int totaltt = 0;
    public static int level = 1; //levelアップの
    public float baseFallTime = 1.0f;  //base速度

    public TextMeshProUGUI levelText; //level


    // ブロックが埋まったかどうかの配列
    public static Transform[,] grid = new Transform[width,height];

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        score = 0;
        scoreText.text = "Score: 0";
        level =1;
        levelText.text = "Level: 1";
    }

    //gameover関数
    public void GameOver()
    {
        if(isGameOver) return;
        isGameOver = true;
        
        gameOverText.gameObject.SetActive(true);
        Invoke(nameof(StopGame),0.1f);
    
    }

    //念のため
    void StopGame()

    {
        Time.timeScale = 0f;
    }
    public void UpScore()
    {
        scoreText.text = "Score: " + score;
    }
    public void UpdateLevel()
    {
        levelText.text = "Level: " + level;
    }




    // 指定座標が埋まっているか返す
    public static bool IsInsideGrid(int x,int y)
    {
        return x >= 0 && x <width && y >= 0;
    }


    public static void CheckLines()
    {
        int tt=0;//スコア回数
        for(int y = 0;y <height;y++)
        {
            if(IsLineFull(y))
            {
                DeleteLine(y);
                MoveDownLines(y);
                y--;
                tt++;
            }
        }

        if(tt >0)
        {
            totaltt += tt;
            score += CalcScore(tt);
            int newLevel = totaltt / 10 +1;
            if(newLevel >level)
            {
                level = newLevel;
                FindObjectOfType<Spawner>().UpFalsp();
                FindObjectOfType<GameManager>().UpdateLevel();
            }

            FindObjectOfType<GameManager>().UpScore();

        }
        
    }
    static bool IsLineFull(int y)
    {
        for(int x = 0;x <width;x++)
        if(grid[x,y] == null) return false;
        return true;
    }
    //ラインけし
    static void DeleteLine(int y)
    {
        for(int x = 0;x <width;x++)
        {
            GameObject.Destroy(grid[x,y].gameObject);
            grid[x,y] = null;
        }
    }

    //ライン消した後に下におろす
    static void MoveDownLines(int y)
    {
        for(int i = y +1;i <height;i++)
        {
            for(int x = 0;x <width;x++)
            {
                if(grid[x,i] != null)
                {
                    grid[x,i-1] = grid[x,i];
                    grid[x,i] = null;
                    grid[x,i-1].position += Vector3.down;
                }
            }
        }
    }
    
    //スコア
    static int CalcScore(int lines)
    {
        switch (lines)
        {
            case 1: return 100;
            case 2: return 200;
            case 3: return 400;
            case 4: return 800;
            default: return 0;
        }
    }


    

}