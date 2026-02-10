using UnityEngine;

public class HoldManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner;

    void Start()
    {
        
        if(spawner == null)
        Debug.LogError("HoldManager: Spawner が未設定（Inspectorで設定）");
        if(holdSlot == null)
        Debug.LogError("HoldManager: holdSlot が未設定");
        
    }
    public Transform holdSlot; // 表示位置（空オブジェクト）
    private GameObject currentHold;   // ホールド中のプレハブ

    private bool hasHold = false;   // ホールド中か判断
    
    //hold中のミノ表示
    void RefreshHoldView()
    {
        foreach (Transform c in holdSlot) Destroy(c.gameObject);
        if(currentHold != null)
        {
            GameObject view = Instantiate(currentHold,holdSlot); // ★ position指定しない
            view.transform.localPosition = Vector3.zero;
            view.transform.localRotation = Quaternion.identity;

            /*view.transform.localScale = Vector3.one * 0.5f;
            size大きさ変更*/

            Destroy(view.GetComponent<Tetorismino>()); // 表示専用
        }
    }



    public void Hold(Tetorismino t)
    {
        if(hasHold) return; // すでにホールド中なら不可
        // ゴースト消す
        t.DestroyGhost();

        if(t.prefabOriginal == null) return; //念のため

        t.CancelInvoke(); // 予約されてる LockMino をキャンセル
        t.isLocking = false; // ロック予約状態を解除

        currentHold = t.prefabOriginal;
        hasHold = true; //hold中へ

        RefreshHoldView();  //hold中のミノ表示
        Destroy(t.gameObject); //holdしたから消す
        spawner.SpawnNext(); //次のミノへ
    }


    // ホールドを使う
    public void UseHold(Tetorismino current)
    {
        // ゴースト消す
        current.DestroyGhost();
        
        //holdしてるorできないとき
        if(!hasHold || currentHold == null) return;

        Destroy(current.gameObject);
        
        GameObject obj = Instantiate(currentHold,spawner.transform.position,Quaternion.identity);
        Tetorismino nt = obj.GetComponent<Tetorismino>();

        nt.prefabOriginal = currentHold;
        nt.fallTime = spawner.curFalLT;
        nt.canHold = true;      // 次のホールドを可能に
        nt.isHolding = false;  // 念のため
        
        currentHold = null;
        hasHold = false; //holdなしへ
        RefreshHoldView();
    }


}