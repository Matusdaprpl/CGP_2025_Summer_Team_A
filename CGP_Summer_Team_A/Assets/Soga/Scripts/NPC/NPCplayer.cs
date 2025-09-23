using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
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

    public List<Tile> hand = new List<Tile>();

    private Rigidbody2D rb;
    private float currentSpeed;
    private float timeSinceLastChange;
    private float remainingCountdownTime;
    private bool isCountdownActive = true;
    private Transform targetItem;
    private float timeSinceLastItemSearch;
    public float laneChangeCooldown = 2.0f;
    private Yakuman targetYakuman;

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
        Debug.Log($"NPCの目標役満: {targetYakuman}");

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
        if (!isCountdownActive && rb != null)
        {
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
        Collider2D[] itemsInRange = Physics2D.OverlapCircleAll(transform.position, itemDetectionRadius, LayerMask.GetMask("Default")); // "Item"レイヤーなど、アイテムのレイヤーに合わせて調整

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

                bool isNeeded = YakumanEvaluator.IsTileNeededFor(itemTile, targetYakuman, this.hand);

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            if (hand.Count >= 15)
            {
                return;
            }

            ItemController itemController = other.GetComponent<ItemController>();
            if (itemController != null)
            {
                Tile pickedTile = itemController.GetTile();
                if (pickedTile != null)
                {
                    hand.Add(pickedTile);
                    Debug.Log($"NPCが牌を拾った: {pickedTile.GetDisplayName()}");

                    if (other.transform == targetItem)
                    {
                        targetItem = null;
                    }

                    Destroy(other.gameObject);
                }
            }
        }
    }
}

