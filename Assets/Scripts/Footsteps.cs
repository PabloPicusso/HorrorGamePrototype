using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class Footsteps : MonoBehaviour
{
    public FPController controller;

    [Header("Step Sounds")]
    public AudioClip[] stepClips;
    public float walkStepDistance = 2.1f;
    public float sprintDistanceMul = 0.8f;
    public float crouchDistanceMul = 1.4f;

    [Header("Jump / Land")]
    public AudioClip[] jumpClips;
    public AudioClip[] landClips;
    public float minLandVelocity = 3f;

    [Header("Tuning")]
    public float minMoveSpeed = 0.2f;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("Internal Constants")]
    public float zeroFloat = 0f;
    public int randomMinIndex = 0;
    public bool playOnAwakeValue = false;
    public bool loopValue = false;
    public float spatialBlendValue = 0f;

    CharacterController cc;
    AudioSource src;
    Vector3 lastPos;
    float distAccum;
    bool wasGrounded;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        src = GetComponent<AudioSource>();
        if (!controller) controller = GetComponent<FPController>();

        src.playOnAwake = playOnAwakeValue;
        src.loop = loopValue;
        src.spatialBlend = spatialBlendValue;

        lastPos = transform.position;
    }

    void Update()
    {
        if (controller == null) return;

        if (!wasGrounded && cc.isGrounded)
        {
            if (landClips != null && landClips.Length > randomMinIndex && Mathf.Abs(cc.velocity.y) > minLandVelocity)
                PlayOne(landClips);
            distAccum = zeroFloat;
            lastPos = transform.position;
        }
        wasGrounded = cc.isGrounded;

        if (!cc.isGrounded || stepClips == null || stepClips.Length == randomMinIndex) { lastPos = transform.position; return; }

        Vector3 pos = transform.position;
        Vector3 delta = pos - lastPos; delta.y = zeroFloat;
        lastPos = pos;

        Vector3 horizVel = cc.velocity; horizVel.y = zeroFloat;
        float speed = horizVel.magnitude;
        if (speed < minMoveSpeed) { distAccum = zeroFloat; return; }

        distAccum += delta.magnitude;

        float stepDist = walkStepDistance;
        bool sprinting = controller.sprintAction && controller.sprintAction.action.IsPressed();
        bool crouching = controller.crouchAction && controller.crouchAction.action.IsPressed();
        if (sprinting) stepDist *= sprintDistanceMul;
        if (crouching) stepDist *= crouchDistanceMul;

        if (distAccum >= stepDist)
        {
            distAccum -= stepDist;
            PlayOne(stepClips);
        }
    }

    public void PlayJump()
    {
        if (jumpClips != null && jumpClips.Length > randomMinIndex)
            PlayOne(jumpClips);
    }

    void PlayOne(AudioClip[] set)
    {
        if (set == null || set.Length == randomMinIndex) return;
        int i = UnityEngine.Random.Range(randomMinIndex, set.Length);
        src.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);
        src.PlayOneShot(set[i]);
    }
}
