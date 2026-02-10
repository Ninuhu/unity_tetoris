using UnityEngine;

public class TetrisSpawner : MonoBehaviour
{
    public GameObject[] pieces;  // テトリス種類のPrefabを登録する
    public Transform spawnPoint; // 画面上部・中央の位置

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        int r = Random.Range(0, pieces.Length);
        GameObject obj = Instantiate(pieces[r], spawnPoint.position, Quaternion.identity);
        TetrisPiece piece = obj.GetComponent<TetrisPiece>();
        piece.spawner = this;
    }

}
