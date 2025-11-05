using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class NPCplayer : MonoBehaviour
{
    [Header("速度設定")]
    [Tooltip("NPCの最低速度")]
    public float minSpeed = 2f;

    [Tooltip("NPCの最高速度")]
    public float maxSpeed = 5f;

    [Header("挙動判定")]
    [Tooltip("速度を変更する間隔（秒）")]
    public float speedChangeInterval = 3f;

    [Header("レーンの移動設定")]
    [Tooltip("レーンの移動速度")]
    public float laneMoveSpeed = 5f;

    [Tooltip("レーンのY座標（4レーン）")]
    public float[] laneYs = {-5.0f,-3.5f,-2.0f,-0.5f};

    [Tooltip("レーンのY軸範囲")]
    public float minLaneY = -5f;

    [Tooltip("レーンのY軸範囲")]
    public float maxLaneY = -0.5f;

    [Tooltip("レーンの変更間隔（秒）")]
    public float laneChangeCooldown = 2.0f;

    [Header("アイテム追跡設定")]
    [Tooltip("アイテムを探す範囲")]
    public float itemDetectionRadius = 20f;

    [Tooltip("アイテムを探す間隔（秒）")]
    public float itemSearchInterval = 1f;

    private float timeSinceLastLaneChange;

    [Header("カウントダウン設定")]
    [Tooltip("カウントダウンの時間（秒）")]
    public float countdownTime = 3f;

    [Tooltip("カウントダウンのUIテキスト")]
    public TMP_Text countdownText;

    [Header("牌の管理")]
    [SerializeField]
    private NPCmahjong npcMahjong;

    [Header("点棒設定")]
    [Tooltip("点棒による停止時間（秒）")]
    public float stopTime = 2f;

    private Rigidbody2D rb;
    private float currentSpeed;
    private float timeSinceLastChange;
    private float remainingCountdownTime;
    private bool isCountdownActive = true;
    private Transform targetItem;
    private float timeSinceLastItemSearch;
    private Yakuman targetYakuman;
    public Yakuman TargetYakuman => targetYakuman;

    // ★★★ 停止フラグを追加 ★★★
    private bool isStopped = false; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2Dコンポーネントが見つかりません。");
        }
    }

    void Start()
    {
        var yakumanValues = System.Enum.GetValues(typeof(Yakuman));

        targetYakuman = (Yakuman)yakumanValues.GetValue(Random.Range(1, yakumanValues.Length));
        Debug.Log($"{gameObject.name}の目標役満: {targetYakuman}");

        UpdateSpeed();
        timeSinceLastChange = 0f;
        timeSinceLastLaneChange = 0f;
        remainingCountdownTime = countdownTime;
        if (countdownText != null)
        {
            countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();

        }
    }

    void Update()
    {
        // ★★★ 停止フラグのチェックを追加 ★★★
        if (isStopped) return; 

        if (isCountdownActive)
        {
            remainingCountdownTime -= Time.deltaTime;

            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();
            }

            if (remainingCountdownTime <= 0)
            {
                isCountdownActive = false;
                if (countdownText != null)
                {
                    countdownText.text = "Go!";
                    Invoke("HideCountdownText", 1f);
                }
            }
            return;
        }

        timeSinceLastLaneChange += Time.deltaTime;
        HandleItemSeekingAndLaneChange();
    }

    void FixedUpdate()
    {
        // ★★★ 停止フラグのチェックを追加 ★★★
        if (isStopped) return; 

        if (!isCountdownActive && rb != null)
        {
            // ここで速度を設定している
            rb.linearVelocity = new Vector2(currentSpeed, 0);
        }

        timeSinceLastChange += Time.fixedDeltaTime;

        if (timeSinceLastChange >= speedChangeInterval)
        {
            UpdateSpeed();
            timeSinceLastChange = 0f;
        }
    }

    private void HandleItemSeekingAndLaneChange()
    {
        timeSinceLastItemSearch += Time.deltaTime;

        if (targetItem == null || timeSinceLastItemSearch >= itemSearchInterval)
        {
            FindClosestItem();
            timeSinceLastItemSearch = 0f;
        }
        if (targetItem != null && timeSinceLastLaneChange >= laneChangeCooldown)
        {
            float targetY = targetItem.position.y - 0.2f;
            targetY = Mathf.Clamp(targetY, minLaneY, maxLaneY);
            
            // laneYsの最も近い値にスナップ
            float closestLaneY = laneYs.OrderBy(y => Mathf.Abs(y - targetY)).First();
            Vector3 currentPosition = transform.position;

            float newY = Mathf.MoveTowards(currentPosition.y, closestLaneY, laneMoveSpeed * Time.deltaTime);
            transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);

            // レーンの変更クールダウンをリセット
            timeSinceLastLaneChange = 0f;
        }
    }

    private void FindClosestItem()
    {
        Collider2D[] itemsInRange = Physics2D.OverlapCircleAll(transform.position, itemDetectionRadius, LayerMask.GetMask("Default")); 

        float bestTargetDistance = float.MaxValue;
        Transform bestTarget = null;
        bool foundNeededItem = false;

        foreach (var itemCollider in itemsInRange)
        {
            if (itemCollider.CompareTag("Item"))
            {
                ItemController itemController = itemCollider.GetComponent<ItemController>();
                if (itemController == null) continue;

                Tile itemTile = itemController.GetTile();
                if (itemTile == null) continue;

                float distance = Vector2.Distance(transform.position, itemCollider.transform.position);

                bool isNeeded = YakumanEvaluator.IsTileNeededFor(itemTile, targetYakuman, npcMahjong.hand);

                if (isNeeded)
                {
                    if (!foundNeededItem)
                    {
                        foundNeededItem = true;
                        bestTarget = itemCollider.transform;
                        bestTargetDistance = distance;
                    }
                    else if (distance < bestTargetDistance)
                    {
                        bestTarget = itemCollider.transform;
                        bestTargetDistance = distance;
                    }
                }
                else if (!foundNeededItem && distance < bestTargetDistance)
                {
                    bestTargetDistance = distance;
                    bestTarget = itemCollider.transform;
                    bestTarget = itemCollider.transform;
                }
            }
        }

        targetItem = bestTarget;
    }

    void UpdateSpeed()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void HideCountdownText()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    private bool isProcessingTile = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"{gameObject.name} OnTriggerEnter2D 呼び出し: tag={other.tag}, isStopped={isStopped}, isProcessingTile={isProcessingTile}, handCount={npcMahjong.hand.Count}");

        if (isStopped)
        {
            Debug.Log($"{gameObject.name} は停止中です。アイテム取得をスキップします。");
            return;
        }

        if (other.CompareTag("Bullet"))
        {
            Debug.Log($"{gameObject.name}が点棒に当たりました。");
            StartCoroutine(HandleTenbouHit());
            Destroy(other.gameObject);
            return;
        }
        if (other.CompareTag("Item") && !isProcessingTile && npcMahjong.hand.Count <= 14)
        {
            ItemController itemController = other.GetComponent<ItemController>();
            if (itemController != null)
            {
                Tile pickedTile = itemController.GetTile();
                if (pickedTile != null)
                {
                    Destroy(other.gameObject);
                    if (other.transform == targetItem)
                    {
                        targetItem = null;
                    }

                    Debug.Log($"{gameObject.name} がアイテムを拾いました: {pickedTile.GetDisplayName()}");
                    StartCoroutine(ProcessTileExchange(pickedTile));
                }
                else
                {
                    Debug.Log($"{gameObject.name} ItemController.GetTile() が null です。");
                }
            }
            else
            {
                Debug.Log($"{gameObject.name} ItemController が付いていません。");
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} アイテム取得条件を満たしていません: isProcessingTile={isProcessingTile}, handCount={npcMahjong.hand.Count}");
        }
    }
    
    private IEnumerator HandleTenbouHit()
    {
        isStopped = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        int dropCount = Mathf.Min(5, npcMahjong.hand.Count);
        List<Tile> tilesToDrop = npcMahjong.hand.OrderBy(x => Random.value).Take(dropCount).ToList();

        foreach(Tile tile in tilesToDrop)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-3f, -1f), 0.2f, 0);
            Vector3 dropPosition = transform.position + randomOffset;

            ItemManager.instance.DropDiscardedTile(tile, dropPosition);
            npcMahjong.hand.Remove(tile);
        }

        yield return new WaitForSeconds(stopTime);

        isStopped = false;
    }

    private IEnumerator ProcessTileExchange(Tile pickedTile)
    {
        isProcessingTile = true;
        //Debug.Log($"{gameObject.name} ProcessTileExchange 開始、isProcessingTile を true に設定");

        try
        {
            npcMahjong.AddTileToHand(pickedTile);
            npcMahjong.PrintHandToConsole("牌を拾った直後");

            string handDescription = string.Join(",", npcMahjong.hand.Select(t => t.GetDisplayName()));
            Debug.Log($"{gameObject.name}の拾った牌: {pickedTile.GetDisplayName()}");
            Debug.Log($"{gameObject.name}の手牌: {handDescription}");

            if (YakumanEvaluator.IsYakumanComplete(npcMahjong.hand, TargetYakuman))
            {
                Debug.Log($"{gameObject.name}は役満 {TargetYakuman} を完成させました！");
                MahjongManager.instance?.OnNpcWin(gameObject.name, TargetYakuman, npcMahjong.hand);
                isProcessingTile = false;
                //Debug.Log($"{gameObject.name} ProcessTileExchange 終了（役満完成）、isProcessingTile を false に設定");
                yield break;
            }

            yield return new WaitForSeconds(1.0f);

            // 捨てる前にY座標をlaneYsの最も近い値に即座にスナップ
            float currentY = transform.position.y;
            float closestLaneY = laneYs.OrderBy(y => Mathf.Abs(y - currentY)).First();
            transform.position = new Vector3(transform.position.x, closestLaneY, transform.position.z);
            Debug.Log($"{gameObject.name} Y座標をスナップ: {currentY} -> {closestLaneY}");

            bool isOnLane = laneYs.Any(laneY => Mathf.Approximately(transform.position.y, laneY));
            Debug.Log($"{gameObject.name} isOnLane: {isOnLane}, currentY: {transform.position.y}, laneYs: {string.Join(",", laneYs)}");

            if (isOnLane)
            {
                npcMahjong.DiscardTile();
                npcMahjong.PrintHandToConsole("捨て牌後");
                Debug.Log($"{gameObject.name} 牌を捨てました");
            }
            else
            {
                Debug.Log($"{gameObject.name} はレーン上にいないため、捨てられません");
            }
        }
        finally
        {
            isProcessingTile = false;
            Debug.Log($"{gameObject.name} ProcessTileExchange 終了、isProcessingTile を false に設定");
        }
    }

    public void StopMovement()
    {
        isStopped = true; 
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Rigidbodyの速度を強制的にゼロにする (重要!)
            rb.angularVelocity = 0f;
            //Debug.Log($"{gameObject.name} のRigidbody速度をリセットしました。");
        }
        
        this.enabled = false; // スクリプト自体も無効化 (二重の停止措置)
        //Debug.Log($"{gameObject.name} の動きを完全に停止しました。");
    }
}