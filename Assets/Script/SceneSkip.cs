using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSkip : MonoBehaviour
{
    //Title→select
    public void se()
    {
        SceneManager.LoadScene(1);

    }
    //select→game1
    public void goga1()
    {
        SceneManager.LoadScene(2);

    }
    //select→game2
    public void goga2()
    {
        SceneManager.LoadScene(3);

    }
    //select→game3
    public void goga3()
    {
        SceneManager.LoadScene(3);

    }

    //gameover後のstart画面移行
    public void BackSTscene()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(0);
    }
    //retryボタン
    public void Retry()
    {
        Time.timeScale = 1f; //timescaleを元に戻す
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    
}
