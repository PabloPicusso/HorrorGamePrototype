using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NightVisionScript : MonoBehaviour
{
    public Camera cam;
    public Image zoomBar;
    public Image batteryChunks;

    [Header("Zoom")]
    public float minFOV = 10f;
    public float maxFOV = 60f;
    public float zoomStep = 5f;
    public float scrollPositiveThreshold = 0.01f;
    public float scrollNegativeThreshold = -0.01f;
    public float zoomFillDivisor = 100f;

    [Header("Battery")]
    [Range(0f, 1f)] public float batteryPower = 1.0f;
    public float drainTime = 2f;
    public float drainAmountPerTick = 0.25f;
    public float batteryMinValue = 0f;
    public float batteryMaxValue = 1f;

    [Header("UI on Enable")]
    public bool setBarToDefaultOnEnable = false;
    [Range(0f, 1f)] public float defaultFillOnEnable = 0.6f;

    void Start()
    {
        if (!zoomBar)
            zoomBar = GameObject.Find("ZoomBar")?.GetComponent<Image>();
        if (!batteryChunks)
            batteryChunks = GameObject.Find("BatteryChunks")?.GetComponent<Image>();
        if (!cam)
        {
            var go = GameObject.Find("FirstPersonCharacter");
            cam = go ? go.GetComponent<Camera>() : Camera.main;
        }

        UpdateZoomBarImmediate();
        if (batteryChunks) batteryChunks.fillAmount = Mathf.Clamp(batteryPower, batteryMinValue, batteryMaxValue);

        InvokeRepeating(nameof(BatteryDrain), drainTime, drainTime);
    }

    void OnEnable()
    {
        if (setBarToDefaultOnEnable && zoomBar)
            zoomBar.fillAmount = Mathf.Clamp(defaultFillOnEnable, batteryMinValue, batteryMaxValue);
        else
            UpdateZoomBarImmediate();
    }

    void Update()
    {
        if (!cam) return;

        float scrollY = Mouse.current != null
            ? Mouse.current.scroll.ReadValue().y
            : Input.GetAxis("Mouse ScrollWheel");

        if (scrollY > scrollPositiveThreshold && cam.fieldOfView > minFOV)
            cam.fieldOfView = Mathf.Max(minFOV, cam.fieldOfView - zoomStep);
        else if (scrollY < scrollNegativeThreshold && cam.fieldOfView < maxFOV)
            cam.fieldOfView = Mathf.Min(maxFOV, cam.fieldOfView + zoomStep);

        UpdateZoomBarImmediate();
        if (batteryChunks) batteryChunks.fillAmount = Mathf.Clamp(batteryPower, batteryMinValue, batteryMaxValue);
    }

    void BatteryDrain()
    {
        if (batteryPower > batteryMinValue)
            batteryPower = Mathf.Max(batteryMinValue, batteryPower - drainAmountPerTick);
    }

    void UpdateZoomBarImmediate()
    {
        if (!zoomBar || !cam) return;
        zoomBar.fillAmount = Mathf.Clamp(cam.fieldOfView / zoomFillDivisor, batteryMinValue, batteryMaxValue);
    }
}
