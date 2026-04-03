using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    public void Retry()
    {
        Time.timeScale = 1f; //timescaleを元に戻す
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}