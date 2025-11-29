using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowToExplodeTap : MonoBehaviour
{
    public enum DifficultyLevel { Easy, Medium, Hard }

    [Header("Input")]
    public string key = "a";  // 玩家按键（P1 用 A，P2 用 L）

    [Header("Balloon Visual")]
    public float maxScale = 3f;  // 达到目标次数时的体积上限

    [Header("Difficulty (Press Counts)")]
    public DifficultyLevel difficulty = DifficultyLevel.Easy;
    public int pressesEasy = 20;
    public int pressesMedium = 30;
    public int pressesHard = 30;

    [Header("Hard Mode Leak Settings")]
    [Tooltip("困难模式每秒漏掉的按键等效次数")]
    public float hardLeakPressesPerSecond = 5.0f; // ⚖️ 调低以便能吹爆
    [Range(0.5f, 3f)]
    public float leakStrength = 1.0f; // 漏气倍率（1 = 基准速度）
    [Range(0.05f, 0.3f)]
    public float leakCapRatio = 0.15f; // 每秒最多漏掉进度的比例

    private int requiredPresses;
    private float pressUnits;
    private Vector3 startScale;
    private bool exploded = false;

    private SpriteRenderer spriteRenderer;
    private ParticleSystem popFX;

    void Start()
    {
        startScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        popFX = transform.Find("PopFX")?.GetComponent<ParticleSystem>();

        ConfigureDifficulty();
        UpdateVisual();
    }

    void Update()
    {
        if (exploded) return;

        // ===== 困难模式漏气（平衡版） =====
        if (difficulty == DifficultyLevel.Hard && pressUnits > 0f)
        {
            float ratio = pressUnits / requiredPresses;

            // 越大越漏，但会随体积衰减，防止吹不爆
            float leakRate = hardLeakPressesPerSecond * ratio * (1f - 0.5f * ratio) * leakStrength;

            // 限制最大漏速
            float maxLeak = pressUnits * leakCapRatio;
            leakRate = Mathf.Min(leakRate, maxLeak);

            // 实际扣除
            pressUnits -= leakRate * Time.deltaTime;
            if (pressUnits < 0f) pressUnits = 0f;
        }

        // ===== 玩家按键充气 =====
        if (Input.GetKeyDown(key))
        {
            pressUnits += 1f;

            if (pressUnits >= requiredPresses)
            {
                exploded = true;

                if (spriteRenderer != null)
                    spriteRenderer.enabled = false;

                if (popFX != null)
                {
                    var main = popFX.main;
                    main.useUnscaledTime = true;
                    popFX.Play();
                }

                FindObjectOfType<GameJudge>()?.OnPlayerExploded(this);
            }
        }

        UpdateVisual();
    }

    /// <summary>
    /// 根据难度设置不同参数
    /// </summary>
    private void ConfigureDifficulty()
    {
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                requiredPresses = Mathf.Max(1, pressesEasy);
                hardLeakPressesPerSecond = 0f;
                break;

            case DifficultyLevel.Medium:
                requiredPresses = Mathf.Max(1, pressesMedium);
                hardLeakPressesPerSecond = 0f;
                break;

            case DifficultyLevel.Hard:
                requiredPresses = Mathf.Max(1, pressesHard);
                if (hardLeakPressesPerSecond <= 0f)
                    hardLeakPressesPerSecond = 5.0f; // ⚙️ 默认漏气
                break;
        }

        pressUnits = Mathf.Clamp(pressUnits, 0f, requiredPresses);
    }

    /// <summary>
    /// 根据进度更新气球大小
    /// </summary>
    private void UpdateVisual()
    {
        float progress = (requiredPresses > 0) ? Mathf.Clamp01(pressUnits / requiredPresses) : 0f;
        float baseScale = startScale.x;
        float target = Mathf.Lerp(baseScale, maxScale, progress);
        transform.localScale = Vector3.one * target;
    }

    /// <summary>
    /// 重置气球状态
    /// </summary>
    public void ResetBalloon()
    {
        exploded = false;
        pressUnits = 0f;
        transform.localScale = startScale;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (popFX != null)
            popFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    // ===========================
    // ===== 对外接口 ======
    // ===========================

    public void SetDifficultyByName(string level)
    {
        if (string.IsNullOrEmpty(level)) return;

        switch (level.ToLower())
        {
            case "easy": difficulty = DifficultyLevel.Easy; break;
            case "medium": difficulty = DifficultyLevel.Medium; break;
            case "hard": difficulty = DifficultyLevel.Hard; break;
            default: difficulty = DifficultyLevel.Medium; break;
        }

        ConfigureDifficulty();
        UpdateVisual();
    }

    public void SetKey(string newKey)
    {
        if (!string.IsNullOrEmpty(newKey))
            key = newKey;
    }

    public bool IsExploded() => exploded;

    public float GetProgress() =>
        (requiredPresses > 0) ? Mathf.Clamp01(pressUnits / requiredPresses) : 0f;

    /// <summary>
    /// 外部脚本设置困难模式漏气速度（保留旧接口，防止红线）
    /// </summary>
    public void SetHardLeak(float pressesPerSecond)
    {
        hardLeakPressesPerSecond = Mathf.Max(0f, pressesPerSecond);
    }
}
