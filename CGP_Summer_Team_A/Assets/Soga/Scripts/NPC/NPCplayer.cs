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

    [Tooltip("レーンのY軸範囲")]
    public float minLaneY = -2f;

    [Tooltip("レーンのY軸範囲")]
    public float maxLaneY = 2f;

    [Tooltip("レーンの変更間隔（秒）")]
    public float laneChangeInterval = 2f;

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

    private Rigidbody2D rb;
    private float currentSpeed;
    private float timeSinceLastChange;
    private float remainingCountdownTime;
    private bool isCountdownActive = true;
    private Transform targetItem;
    private float timeSinceLastItemSearch;
    public float laneChangeCooldown = 2.0f;
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
            float targetY = targetItem.position.y;
            Vector3 currentPosition = transform.position;

            float newY = Mathf.MoveTowards(currentPosition.y, targetY, laneMoveSpeed * Time.deltaTime);
            transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);
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
        if (isStopped) return; // ★★★ 停止中はアイテム取得もスキップ ★★★

        if (other.CompareTag("Item") && !isProcessingTile && npcMahjong.hand.Count < 15)
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

                    StartCoroutine(ProcessTileExchange(pickedTile));
                }
            }
        }
    }
    
    private IEnumerator ProcessTileExchange(Tile pickedTile)
    {
        isProcessingTile = true;
        
        npcMahjong.AddTileToHand(pickedTile);
        
        Debug.Log($"{gameObject.name}の拾った牌: {pickedTile.GetDisplayName()}");

        if (YakumanEvaluator.IsYakumanComplete(npcMahjong.hand, TargetYakuman))
        {
            Debug.Log($"{gameObject.name}は役満 {TargetYakuman} を完成させました！");
            MahjongManager.instance?.OnNpcWin(gameObject.name, TargetYakuman, npcMahjong.hand);
            isProcessingTile = false;
            yield break; // ← 捨て処理には進まない

        }

        yield return new WaitForSeconds(1.0f);
        
        npcMahjong.DiscardTile();
        
        isProcessingTile = false;
    }

    // ★★★ GameManager2から呼ばれる停止メソッド ★★★
    public void StopMovement()
    {
        isStopped = true; // フラグを立ててUpdate/FixedUpdateを停止
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Rigidbodyの速度を強制的にゼロにする (重要!)
            rb.angularVelocity = 0f;
            Debug.Log($"{gameObject.name} のRigidbody速度をリセットしました。");
        }
        
        this.enabled = false; // スクリプト自体も無効化 (二重の停止措置)
        Debug.Log($"{gameObject.name} の動きを完全に停止しました。");
    }
}