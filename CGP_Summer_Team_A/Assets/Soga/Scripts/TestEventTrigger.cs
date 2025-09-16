using UnityEngine;
using UnityEngine.Events; // UnityEventを使う場合に必要

public class TestEventTrigger : MonoBehaviour
{
    // InspectorからMahjongManagerがアタッチされているオブジェクトを設定する
    [SerializeField]
    private MahjongManager mahjongManager;

    // もしMahjongManagerのイベントがUnityEventなら、こちらも用意すると便利
    public UnityEvent testAction;

    void Update()
    {
        // スペースキーが押されたら
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("テストトリガー実行：スペースキーが押されました。");

            // ここで、MahjongManagerの「牌を引く」イベントやメソッドを直接呼び出す
            // 例として、MahjongManagerに `DrawTileByItem()` という公開メソッドがあると仮定
            if (mahjongManager != null)
            {
                mahjongManager.PlayerHitItem(); // このメソッド名は、あなたが実装したメソッド名に書き換えてほしい
            }

            // もしUnityEventを使っているなら、以下のようにInvoke()を呼ぶ
            testAction.Invoke();
        }
    }
}