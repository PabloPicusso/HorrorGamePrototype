using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // New Input System (optional)
#endif

public class CollectPages : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject collectText;   

    [Header("Audio")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioSource audioSource;  

    [Header("Reach / Destroy")]
    [SerializeField] private float reachRadius = 1.5f;
    [SerializeField] private bool destroyOnCollect = false;

    [Header("Counter")]
    [SerializeField] private GameLogic gameLogic;      
    [SerializeField] private string gameLogicTag = "GameLogic"; 

    private Transform player; 
    private bool inReach;

    void Awake()
    {
        if (collectText) collectText.SetActive(false);

        player = Camera.main ? Camera.main.transform : null;

        
        if (gameLogic == null)
        {
            var go = GameObject.FindGameObjectWithTag(gameLogicTag);
            if (go) gameLogic = go.GetComponent<GameLogic>();
        }
    }

    void Update()
    {
        if (!player) return;

       
        float sqr = (player.position - transform.position).sqrMagnitude;
        bool nowInReach = sqr <= reachRadius * reachRadius;

        if (nowInReach != inReach)
        {
            inReach = nowInReach;
            if (collectText) collectText.SetActive(inReach);
        }

        if (!inReach) return;

        
        bool pressed = false;
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            pressed = true;
#else
        pressed = Input.GetButtonDown("pickup"); 
#endif

        if (pressed) Collect();
    }

    private void Collect()
    {
        
        if (gameLogic != null) gameLogic.AddPage(1);

        if (collectText) collectText.SetActive(false);

        if (collectClip)
        {
            if (audioSource) audioSource.PlayOneShot(collectClip);
            else AudioSource.PlayClipAtPoint(collectClip, transform.position);
        }

        if (destroyOnCollect) Destroy(gameObject);
        else gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, reachRadius);
    }
#endif
}
