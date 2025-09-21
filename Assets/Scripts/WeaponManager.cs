using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public enum weaponSelect { knife, cleaver, bat, axe, pistol, shotgun, sprayCan, bottle }

    [Header("Setup")]
    public weaponSelect chosenWeapon = weaponSelect.knife;
    public GameObject[] weapons;

    [Header("Animator")]
    public Animator anim;
    public string weaponIdParam = "WeaponID";
    public string weaponChangedParam = "WeaponChanged";
    public string attackTriggerParam = "Attack";
    public float changedResetDelay = 0.5f;

    [Header("Audio")]
    private AudioSource audioPlayer;
    public AudioClip[] weaponSounds;

    [Header("Input Keys")]
    public Key nextKey = Key.X;
    public Key prevKey = Key.Z;

    [Header("Positions")]
    public Vector3 defaultWeaponPosition = new Vector3(0.02f, -0.193f, 0.66f);
    public Vector3 shotgunPosition = new Vector3(0.02f, -0.193f, 0.46f);

    int weaponID = 0;

    void Start()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        weaponID = Mathf.Clamp((int)chosenWeapon, 0, (weapons?.Length ?? 1) - 1);
        ChangeWeapons();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb[nextKey].wasPressedThisFrame)
        {
            if (weaponID < weapons.Length - 1)
                weaponID++;
            ChangeWeapons();
        }

        if (kb[prevKey].wasPressedThisFrame)
        {
            if (weaponID > 0)
                weaponID--;
            ChangeWeapons();
        }

        bool attackPressed = Mouse.current != null
            ? Mouse.current.leftButton.wasPressedThisFrame
            : Input.GetMouseButtonDown(0);

        if (attackPressed)
        {
            if (anim) anim.SetTrigger(attackTriggerParam);
            if (audioPlayer &&
                weaponSounds != null &&
                weaponID >= 0 && weaponID < weaponSounds.Length &&
                weaponSounds[weaponID] != null)
            {
                audioPlayer.clip = weaponSounds[weaponID];
                audioPlayer.Play();
            }
        }
    }

    void ChangeWeapons()
    {
        if (weapons == null || weapons.Length == 0) return;

        foreach (var w in weapons) if (w) w.SetActive(false);
        if (weaponID >= 0 && weaponID < weapons.Length && weapons[weaponID])
            weapons[weaponID].SetActive(true);

        chosenWeapon = (weaponSelect)weaponID;

        if (anim)
        {
            anim.SetInteger(weaponIdParam, weaponID);
            anim.SetBool(weaponChangedParam, true);
            StopAllCoroutines();
            StartCoroutine(WeaponReset());
        }

        Move();
    }

    IEnumerator WeaponReset()
    {
        yield return new WaitForSeconds(changedResetDelay);
        if (anim) anim.SetBool(weaponChangedParam, false);
    }

    void Move()
    {
        switch (chosenWeapon)
        {
            case weaponSelect.shotgun:
                transform.localPosition = shotgunPosition;
                break;
            default:
                transform.localPosition = defaultWeaponPosition;
                break;
        }
    }
}
