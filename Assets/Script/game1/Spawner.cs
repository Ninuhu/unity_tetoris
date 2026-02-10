using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] minos; //みの種類
    public float baseFallTime = 1.0f; //初めの速度
    public float speedMultiplier = 0.9f; //level　ごとに*0.9用の数
    public float curFalLT = 1.0f; //currentFallTime
    public GameObject nextMinoPrefab;   // 次のミノ
    public Transform nextSlot;          // NEXT を表示する場所


    void Start()
    {
        
        curFalLT = baseFallTime;

        // 最初の nextmino を決める
        nextMinoPrefab = minos[Random.Range(0,minos.Length)];
        RefreshNextView();

        SpawnNext();
    }


   

    public void SpawnNext()
    {
        // NEXT に入ってるミノを表示
        GameObject mino = Instantiate(nextMinoPrefab,transform.position,Quaternion.identity);
        // 落下速度を渡す
        Tetorismino t = mino.GetComponent<Tetorismino>();
        
        t.SetFallTime(curFalLT);

        t.prefabOriginal = nextMinoPrefab;
        // ホールド用
        t.canHold = true; // 新しく出たミノはホールド可
        t.isHolding = false;

        // 次の NEXT を決める
        nextMinoPrefab = minos[Random.Range(0,minos.Length)];
        RefreshNextView();
    }

    //落ちる速度更新
    public void UpFalsp()
    {
        float newFallTime = baseFallTime * Mathf.Pow(speedMultiplier,GameManager.level-1);
        curFalLT = newFallTime;
    }
    //nextミノ表示
    void RefreshNextView()
    {
        if(nextSlot == null) return;
        foreach (Transform c in nextSlot) Destroy(c.gameObject);
        GameObject view = Instantiate(nextMinoPrefab,nextSlot.position,Quaternion.identity,nextSlot);
        
        view.transform.localPosition = Vector3.zero;
        view.transform.localRotation = Quaternion.identity;
        /*view.transform.localScale = Vector3.one * 0.5f;
        size大きさ変更*/

        // 操作されないように
        Destroy(view.GetComponent<Tetorismino>());
    }


}