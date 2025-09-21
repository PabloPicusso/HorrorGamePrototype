using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Animator")]
    [Tooltip("Value to put in your Animator int parameter (e.g., 'WeaponID').")]
    public int animatorId = 0;

    [Header("Visual Offset")]
    [Tooltip("Local position offset for this weapon on the rig.")]
    public Vector3 localOffset = new Vector3(0.02f, -0.193f, 0.66f);

    [Header("Optional Prefab (only if you want to spawn instead of using scene objects)")]
    public GameObject prefab; // leave null if you're using existing children in the scene

    [Header("UI / Name (optional)")]
    public string displayName = "Weapon";
}
