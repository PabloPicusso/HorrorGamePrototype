using UnityEngine;
using UnityEngine.InputSystem;

public class LighterInput : MonoBehaviour
{
    [SerializeField] private Key toggleKey = Key.L; // change to any key
    private bool isOn;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb[toggleKey].wasPressedThisFrame)
        {
            isOn = !isOn;
            GameEvents.ToggleLighter(isOn);
        }
    }
}
