using UnityEngine;

public class SlenderManAI : MonoBehaviour
{
    public Transform player;

    [Header("Teleport Settings")]
    public float teleportDistance = 10f;
    public float teleportCooldown = 5f;
    public float returnCooldown = 10f;
    [Range(0f, 1f)] public float chaseProbability = 0.65f;

    [Header("Rotation")]
    public float rotationSpeed = 5f;
    public float minRotationMagnitude = 0.0001f;

    [Header("Audio")]
    public AudioClip teleportSound;
    AudioSource audioSource;

    [Header("Static Effect")]
    public GameObject staticObject;
    public float staticActivationRange = 5f;

    [Header("Grounding")]
    public LayerMask groundMask = ~0;
    public float groundRayHeight = 60f;
    public float footRadius = 0.28f;
    public float extraFootClearance = 0.12f;
    public float sphereCastDistanceMultiplier = 2f;
    public float fallbackFootOffset = 0.9f;

    Vector3 baseTeleportSpot;
    float timer;

    CharacterController cc;
    Rigidbody rb;
    Collider col;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Start()
    {
        baseTeleportSpot = transform.position;
        timer = teleportCooldown;

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (staticObject)
            staticObject.SetActive(false);
    }

    void Update()
    {
        if (!player) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) DecideTeleportAction();

        RotateTowardsPlayer();

        float sqr = (transform.position - player.position).sqrMagnitude;
        bool shouldStatic = sqr <= staticActivationRange * staticActivationRange;
        if (staticObject && staticObject.activeSelf != shouldStatic)
            staticObject.SetActive(shouldStatic);
    }

    void DecideTeleportAction()
    {
        if (UnityEngine.Random.value <= chaseProbability) TeleportNearPlayer();
        else TeleportToBaseSpot();
    }

    void TeleportNearPlayer()
    {
        Vector2 dir2 = UnityEngine.Random.insideUnitCircle.normalized * teleportDistance;
        Vector3 candidate = player.position + new Vector3(dir2.x, 0f, dir2.y);
        TeleportSafely(candidate);
        timer = teleportCooldown;
    }

    void TeleportToBaseSpot()
    {
        TeleportSafely(baseTeleportSpot);
        timer = returnCooldown;
    }

    void TeleportSafely(Vector3 target)
    {
        Vector3 snapped = SnapToGround(target, groundRayHeight, groundMask);
        float foot = GetFootOffset();
        snapped.y += foot + extraFootClearance;

        if (cc)
        {
            bool was = cc.enabled;
            cc.enabled = false;
            transform.position = snapped;
            cc.enabled = was;
        }
        else if (rb)
        {
            if (col) col.enabled = false;
            rb.position = snapped;
            rb.linearVelocity = Vector3.zero;
            if (col) col.enabled = true;
        }
        else
        {
            transform.position = snapped;
        }

        if (teleportSound)
            audioSource.PlayOneShot(teleportSound);
    }

    float GetFootOffset()
    {
        if (cc) return Mathf.Max(0f, cc.height * 0.5f + cc.center.y) * transform.lossyScale.y;
        if (col) return col.bounds.extents.y;
        return fallbackFootOffset;
    }

    Vector3 SnapToGround(Vector3 point, float rayHeight, LayerMask mask)
    {
        Vector3 from = point + Vector3.up * rayHeight;
        float castDist = rayHeight * sphereCastDistanceMultiplier;

        if (Physics.SphereCast(from, footRadius, Vector3.down,
                               out RaycastHit hit, castDist, mask,
                               QueryTriggerInteraction.Ignore))
        {
            point.y = hit.point.y;
        }
        return point;
    }

    void RotateTowardsPlayer()
    {
        Vector3 to = player.position - transform.position;
        to.y = 0f;
        if (to.sqrMagnitude > minRotationMagnitude)
        {
            var target = Quaternion.LookRotation(to);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }
}
