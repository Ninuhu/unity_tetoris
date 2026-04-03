using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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

    public GameObject retry_BackTitleUI; //gameover時のretry&タイトルに戻る
    public GameObject pauselUI;
    public TextMeshProUGUI countDownText; //最初のカウントダウン
    public AudioClip countSE; //カウントダウンSE
    public AudioClip startSE; //count 語のstartSE
    public AudioSource bgmSource;
    public AudioSource seSource;


    [Header("BGM")]
    public AudioClip gameBGM;public AudioClip gameOverBGM;
    void Start()
    {
        //スコアをシーン別ランキングにするために各シーンごとに分ける
        ScoreManager.mode = SceneManager.GetActiveScene().name; 

        isGameOver = false;
        
        // gridリセット
        grid = new Transform[width,height];
        
        //非表示
        gameOverText.gameObject.SetActive(false);
        retry_BackTitleUI.SetActive(false);
        pauselUI.SetActive(false);
        countDownText.gameObject.SetActive(false);
        //=====

        score = 0;
        scoreText.text = "Score: 0";
        level =1;
        levelText.text = "Level: 1";
        StartCoroutine(StartCountDown()); //カウントダウン
    }

    //gameover関数
    public void GameOver()
    {
        if(isGameOver) return;
        ScoreManager.AddScore(score); //Rankingに
        //表示
        retry_BackTitleUI.SetActive(true);

        
        isGameOver = true;
        //表示
        gameOverText.gameObject.SetActive(true);

        //playBGMからgameover BGMに移行
        bgmSource.Stop();
        bgmSource.clip = gameOverBGM;
        bgmSource.loop = true;
        bgmSource.Play();


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
            case 5: return 1600;
            default: return 0;
        }
    }
    private bool isPaused = false; //pauseの有無

    void Update()
    {
        // pause機能
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(isPaused) Resume();
            else Pause();
        }
    }
    // pouse関数
    public void Pause()
    {
        pauselUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    //reasume関数
    public void Resume()
    {
        pauselUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // 最初のカウントダウン
    System.Collections.IEnumerator StartCountDown()
    {
        Time.timeScale = 0f;
        countDownText.gameObject.SetActive(true);
        countDownText.text = "3";
        seSource.PlayOneShot(countSE);
        yield return new WaitForSecondsRealtime(1f);
        countDownText.text = "2";
        seSource.PlayOneShot(countSE);
        yield return new WaitForSecondsRealtime(1f);
        countDownText.text = "1";
        seSource.PlayOneShot(countSE);
        yield return new WaitForSecondsRealtime(1f);

        // STARTの拡大アニメ演出+SE
        yield return StartCoroutine(StartTextEffect());
        countDownText.gameObject.SetActive(false);
        Time.timeScale = 1f;

        //playBGM再生
        bgmSource.clip = gameBGM;
        bgmSource.loop = true;
        bgmSource.Play();


    }
    // STARTの拡大アニメ演出
    IEnumerator StartTextEffect()
    {
        seSource.PlayOneShot(startSE);
        countDownText.gameObject.SetActive(true);
        countDownText.text = "START!";
        // 元のサイズを保存
        float baseSize = countDownText.fontSize,t = 0;
        float bigSize = baseSize * 2f; // 2倍に拡大
        float duration = 0.3f; // 拡大時間
        // 拡大アニメ
        while(t < duration)
        {
            t += Time.unscaledDeltaTime;
            countDownText.fontSize = Mathf.Lerp(baseSize,bigSize,t / duration);
            yield return null;
        }
        // 少し待つ
        yield return new WaitForSecondsRealtime(0.5f);
        // 元に戻す
        countDownText.fontSize = baseSize;
    }
    
    
}
// pause　でのsetting ホールドon/offのためのやつ
public static class GameSettings
{
    public static bool holdEnabled = true;
}