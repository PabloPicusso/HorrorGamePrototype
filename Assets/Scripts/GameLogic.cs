using UnityEngine;
using TMPro;

public class GameLogic : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text counter;   // drag your TMP Text here (e.g., PageCounter)
    [SerializeField] int totalPages = 8;

    public int PageCount { get; private set; }

    void Awake()
    {
        // fallback: try to auto-find a TMP_Text on an object named "PageCounter"
        if (!counter)
        {
            var go = GameObject.Find("PageCounter");
            if (go) counter = go.GetComponent<TMP_Text>();
        }
    }

    void Start()
    {
        PageCount = 0;
        UpdateLabel();
    }

    // call this when a page is collected
    public void AddPage(int amount = 1)
    {
        PageCount = Mathf.Clamp(PageCount + amount, 0, totalPages);
        UpdateLabel();
    }

    // call this if you ever want to reset
    public void ResetPages()
    {
        PageCount = 0;
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (!counter) return;                 // silent if not assigned
        counter.text = $"{PageCount}/{totalPages}";
    }
}
