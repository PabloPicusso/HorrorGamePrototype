using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera))]
public class LookMode : MonoBehaviour
{
    public PostProcessVolume volume;
    public PostProcessProfile standard;
    public PostProcessProfile nightVision;

    public GameObject nightVisionOverlay;
    public GameObject flashlightOverlay;

    public Light flashLight;
    public bool disableFlashlightWhenNVOn = false;

    public float defaultFOV = 60f;

    public AudioClip nvOnClip, nvOffClip, flOnClip, flOffClip;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [SerializeField] float toggleCooldown = 0.25f;

    public AudioClip nvBootClip;
    public float nvBootDelaySeconds = 4f;

    bool nightVisionOn = false;
    bool flashLightOn = false;
    float nextNVToggleAllowed = 0f;
    float nextFLToggleAllowed = 0f;

    bool nvBooting = false;
    bool nvBootDone = false;

    AudioSource audioSrc;
    Camera cam;
    NightVisionScript nvUI;
    FlashLightScript flUI;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (!volume) volume = GetComponent<PostProcessVolume>();
        if (volume && standard) volume.profile = standard;

        if (nightVisionOverlay)
        {
            nightVisionOverlay.SetActive(false);
            nvUI = nightVisionOverlay.GetComponent<NightVisionScript>();
        }

        if (flashlightOverlay)
        {
            flashlightOverlay.SetActive(false);
            flUI = flashlightOverlay.GetComponent<FlashLightScript>();
        }

        if (!flashLight)
        {
            var go = GameObject.Find("FlashLight");
            if (go) flashLight = go.GetComponent<Light>();
        }
        if (flashLight) flashLight.enabled = false;

        audioSrc = GetComponent<AudioSource>();
        if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.spatialBlend = 0f;

        ForceNightVision(false, false);
        ForceFlashlight(false, false);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.nKey.wasPressedThisFrame && Time.unscaledTime >= nextNVToggleAllowed)
        {
            if (!nvBooting)
            {
                if (nightVisionOn)
                {
                    ForceNightVision(false, true);
                }
                else
                {
                    if (!nvBootDone)
                    {
                        StartCoroutine(NightVisionFirstBootRoutine());
                    }
                    else
                    {
                        ForceNightVision(true, false);
                    }
                }
            }
            nextNVToggleAllowed = Time.unscaledTime + toggleCooldown;
        }

        if (kb.fKey.wasPressedThisFrame && Time.unscaledTime >= nextFLToggleAllowed)
        {
            if (flashLightOn) ForceFlashlight(false, true);
            else ForceFlashlight(true, true);

            nextFLToggleAllowed = Time.unscaledTime + toggleCooldown;
        }

        if (nightVisionOn && nvUI != null && nvUI.batteryPower <= 0f)
            ForceNightVision(false, true);

        if (flashLightOn && flUI != null && flUI.batteryPower <= 0f)
            ForceFlashlight(false, true);
    }

    System.Collections.IEnumerator NightVisionFirstBootRoutine()
    {
        nvBooting = true;

        if (disableFlashlightWhenNVOn && flashLightOn)
            ForceFlashlight(false, false);

        float wait = nvBootClip ? nvBootClip.length : nvBootDelaySeconds;
        if (nvBootClip) { audioSrc.Stop(); audioSrc.PlayOneShot(nvBootClip, sfxVolume); }
        if (wait > 0f) yield return new WaitForSecondsRealtime(wait);

        if (nvUI != null && nvUI.batteryPower <= 0f)
        {
            nvBooting = false;
            nvBootDone = true;
            yield break;
        }

        ForceNightVision(true, false);

        nvBooting = false;
        nvBootDone = true;
    }

    void ForceNightVision(bool on, bool playSfx)
    {
        if (on && disableFlashlightWhenNVOn && flashLightOn)
            ForceFlashlight(false, false);

        nightVisionOn = on;

        if (volume) volume.profile = on ? nightVision : standard;
        if (nightVisionOverlay) nightVisionOverlay.SetActive(on);
        if (!on && cam) cam.fieldOfView = defaultFOV;

        if (playSfx)
        {
            if (on && nvOnClip) { audioSrc.Stop(); audioSrc.PlayOneShot(nvOnClip, sfxVolume); }
            if (!on && nvOffClip) { audioSrc.Stop(); audioSrc.PlayOneShot(nvOffClip, sfxVolume); }
        }
    }

    void ForceFlashlight(bool on, bool playSfx)
    {
        flashLightOn = on;

        if (flashlightOverlay) flashlightOverlay.SetActive(on);
        if (flashLight) flashLight.enabled = on;

        if (flUI != null)
        {
            if (on) flUI.StartDrain();
            else flUI.StopDrain();
        }

        if (playSfx)
        {
            if (on && flOnClip) { audioSrc.Stop(); audioSrc.PlayOneShot(flOnClip, sfxVolume); }
            if (!on && flOffClip) { audioSrc.Stop(); audioSrc.PlayOneShot(flOffClip, sfxVolume); }
        }
    }
}
