using UnityEngine;

public class LighterScript : MonoBehaviour
{
    [SerializeField] private GameObject lighterObj;
    [SerializeField] private bool startOn = false;

    void Awake()
    {
        // If you put this script on the exact object to show/hide,
        // you don’t have to assign it in the Inspector.
        if (!lighterObj) lighterObj = gameObject;
        if (lighterObj) lighterObj.SetActive(startOn);
    }

    void OnEnable() => GameEvents.LighterToggled += OnToggle;
    void OnDisable() => GameEvents.LighterToggled -= OnToggle;

    private void OnToggle(bool on)
    {
        if (lighterObj) lighterObj.SetActive(on);
    }
}
