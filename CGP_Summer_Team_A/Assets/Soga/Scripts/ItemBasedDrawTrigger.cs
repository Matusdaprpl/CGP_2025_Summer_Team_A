using UnityEngine;

/// <summary>
/// 特定のタグが付いたオブジェクトとの接触をきっかけに牌を引くトリガー
/// </summary>
public class ItemBasedDrawTrigger : DrawTrigger
{
    [Header("アイテム設定")]
    [Tooltip("このタグが付いたオブジェクトに当たると牌を引きます")]
    [SerializeField] private string itemTag = "Item"; // インスペクターで変更可能

    // 2Dの当たり判定（Is Triggerがオンの場合）
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 衝突した相手のタグが設定したものであれば
        if (other.CompareTag(itemTag))
        {
            Debug.Log(other.name + " に接触しました。牌を引きます。");
            
            // DrawTriggerの基本機能を使って、牌を引くリクエストを発行する
            RequestDraw();

            // 必要に応じて、取得したアイテムを消す処理などを後からここに追加します
            //例: Destroy(other.gameObject);
        }
    }

    /*
    // --- 3Dゲームの場合はこちらを使用します ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(itemTag))
        {
            Debug.Log(other.name + " に接触しました。牌を引きます。");
            RequestDraw();
            //例: Destroy(other.gameObject);
        }
    }
    */
}