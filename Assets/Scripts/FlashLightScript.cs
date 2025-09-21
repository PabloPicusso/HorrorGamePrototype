using UnityEngine;
using UnityEngine.UI;

public class FlashLightScript : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Image whose fillAmount shows battery (0..1).")]
    [SerializeField] private Image batteryChunks;
    [Tooltip("If not assigned, we'll try to find this object name in the scene.")]
    [SerializeField] private string batteryObjectName = "FLBatteryChunks";

    [Header("Battery")]
    [Range(0f, 1f)] public float batteryPower = 1f;   // 1 = full, 0 = empty
    [Tooltip("Seconds between drain ticks.")]
    public float drainTime = 2f;
    [Tooltip("Amount removed each tick (0..1).")]
    public float drainAmountPerTick = 0.25f;

    bool draining;

    void Awake()
    {
        if (!batteryChunks && !string.IsNullOrEmpty(batteryObjectName))
        {
            var go = GameObject.Find(batteryObjectName);
            if (go) batteryChunks = go.GetComponent<Image>();
        }
        UpdateUI();
    }

    void OnEnable()
    {
        StartDrain();
        UpdateUI();
    }

    void OnDisable()
    {
        StopDrain();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (batteryChunks)
            batteryChunks.fillAmount = Mathf.Clamp01(batteryPower);
    }

    void FLBatteryDrain()
    {
        if (batteryPower <= 0f)
        {
            batteryPower = 0f;
            StopDrain();
            return;
        }

        batteryPower = Mathf.Max(0f, batteryPower - drainAmountPerTick);
    }

    public void StartDrain()
    {
        if (draining) return;
        InvokeRepeating(nameof(FLBatteryDrain), drainTime, drainTime);
        draining = true;
    }

    public void StopDrain()
    {
        if (!draining) return;
        CancelInvoke(nameof(FLBatteryDrain));
        draining = false;
    }
}
