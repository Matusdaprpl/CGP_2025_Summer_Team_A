using System;
using UnityEngine;

// 様々な「牌を引くきっかけ」の設計図となる抽象クラス
public abstract class DrawTrigger : MonoBehaviour
{
    // 牌を引いてほしい時に発行されるイベント
    // abstractを付けず、ここで実装を完了させる
    public event Action OnDrawRequested;

    // 子クラス（ItemBasedDrawTriggerなど）から呼び出すためのメソッド
    protected void RequestDraw()
    {
        // OnDrawRequested イベントに誰か（NPCmahjong）が登録していれば、イベントを発行する
        if (OnDrawRequested != null)
        {
            OnDrawRequested.Invoke();
        }
    }
}