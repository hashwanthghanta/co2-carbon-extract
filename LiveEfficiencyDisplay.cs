// =====================================================
//  LiveEfficiencyDisplay.cs
//  Auto-builds the entire Live Efficiency panel UI
//  No manual text objects needed
// =====================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class LiveEfficiencyDisplay : MonoBehaviour
{
    [Header("Preview Settings")]
    [Tooltip("Turn ON to see panel in Edit mode for positioning. Turn OFF when done.")]
    public bool showInEditMode = false;

    [Header("Position & Size — adjust these")]
    public Vector2 panelSize     = new Vector2(320f, 180f);
    public Vector2 panelPosition = new Vector2(-10f, -10f);

    [Header("Slider References — assign in Inspector")]
    public UnityEngine.UI.Slider absorberSlider;
    public UnityEngine.UI.Slider stripperSlider;

    // ─────────────────────────────────────────────────
    // Auto-created text references
    private TextMeshProUGUI _absorberTempText;
    private TextMeshProUGUI _absorberEffText;
    private TextMeshProUGUI _stripperTempText;
    private TextMeshProUGUI _stripperEffText;
    private TextMeshProUGUI _combinedEffText;
    private TextMeshProUGUI _statusText;

    // Colors
    private readonly Color cyan    = new Color(0f,   0.82f, 1f,   1f);
    private readonly Color green   = new Color(0.2f, 0.9f,  0.3f, 1f);
    private readonly Color orange  = new Color(1f,   0.6f,  0.1f, 1f);
    private readonly Color white   = new Color(1f,   1f,    1f,   1f);
    private readonly Color dimWhite= new Color(0.7f, 0.8f,  0.9f, 1f);
    private readonly Color red     = new Color(1f,   0.2f,  0.2f, 1f);

    // Reference data
    float[] absorberTemps      = { 25f, 30f, 35f, 40f, 45f, 50f, 55f, 60f, 65f };
    float[] absorberEfficiency = { 96f, 93f, 92f, 91f, 83f, 70f, 69f, 68f, 67f };
    float[] stripperTemps      = { 85f,  90f,  95f, 100f, 105f, 110f, 115f, 120f, 125f };
    float[] stripperEfficiency = { 78f,  80f,  80f,  88f,  91f,  93f,  95f,  94f,  92f };

    // ─────────────────────────────────────────────────

    void Awake()
    {
        if (!Application.isPlaying)
        {
            UpdateEditModeVisibility();
        }
    }

    void OnValidate()
    {
        // Called whenever Inspector values change in Edit mode
        UpdateEditModeVisibility();
        UpdatePanelTransform();
    }

    void UpdateEditModeVisibility()
    {
        Image bg = GetComponent<Image>();
        if (bg == null) return;
        if (showInEditMode)
            bg.color = new Color(0.04f, 0.08f, 0.16f, 0.95f);
        else
            bg.color = new Color(0f, 0f, 0f, 0f);

        // Show/hide children
        foreach (Transform child in transform)
        {
            if (child.GetComponent<UnityEngine.UI.Image>() != null ||
                child.GetComponent<TMPro.TextMeshProUGUI>() != null)
                child.gameObject.SetActive(showInEditMode);
        }
    }

    void UpdatePanelTransform()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin        = new Vector2(1f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(1f, 1f);
        rt.sizeDelta        = panelSize;
        rt.anchoredPosition = panelPosition;
    }

    void Start()
    {
        // Force exact position and size
        RectTransform rt = GetComponent<RectTransform>();
        // ── ADJUST THESE VALUES TO MOVE/RESIZE THE PANEL ──
        // anchoredPosition: X = left/right (-value moves left), Y = up/down
        // sizeDelta: X = width, Y = height
        // ── ADJUST THESE VALUES TO MOVE/RESIZE THE PANEL ──
        rt.anchorMin        = new Vector2(1f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(1f, 1f);
        rt.sizeDelta        = new Vector2(500f, 230f);  // ← WIDTH / HEIGHT
        rt.anchoredPosition = new Vector2(-70f, -300f); // ← POS X / POS Y

        BuildUI();
    }

    void OnDisable()
    {
        if (Application.isPlaying)
        {
            // Exiting play mode — destroy built children
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        // Always hide background when disabled
        Image bg = GetComponent<Image>();
        if (bg != null) bg.color = new Color(0f, 0f, 0f, 0f);
    }

    // ─────────────────────────────────────────────────
    //  BUILD UI AUTOMATICALLY
    // ─────────────────────────────────────────────────

    void BuildUI()
    {
        RectTransform rt = GetComponent<RectTransform>();
        // Size is set in Start() — do not override here

        // Background
        Image bg = GetComponent<Image>();
        if (bg == null) bg = gameObject.AddComponent<Image>();
        bg.color = new Color(0.04f, 0.08f, 0.16f, 0.95f);

        // Top cyan border line
        CreateBorderLine();

        // ── Title ─────────────────────────────────────
        CreateLabel("LIVE CAPTURE EFFICIENCY", 14f,
            new Vector2(0f, 90f), new Vector2(480f, 24f),
            cyan, FontStyles.Bold);

        // ── Divider ───────────────────────────────────
        CreateDivider(new Vector2(0f, 74f));

        // ── Absorber row ──────────────────────────────
        CreateLabel("ABSORBER", 13f,
            new Vector2(-160f, 52f), new Vector2(120f, 22f),
            cyan, FontStyles.Normal);

        _absorberTempText = CreateLabel("25°C", 18f,
            new Vector2(30f, 52f), new Vector2(120f, 26f),
            white, FontStyles.Bold);

        _absorberEffText = CreateLabel("96.0%", 18f,
            new Vector2(160f, 52f), new Vector2(120f, 26f),
            green, FontStyles.Bold);

        // ── Stripper row ──────────────────────────────
        CreateLabel("STRIPPER", 13f,
            new Vector2(-160f, 22f), new Vector2(120f, 22f),
            cyan, FontStyles.Normal);

        _stripperTempText = CreateLabel("110°C", 18f,
            new Vector2(30f, 22f), new Vector2(120f, 26f),
            white, FontStyles.Bold);

        _stripperEffText = CreateLabel("93.0%", 18f,
            new Vector2(160f, 22f), new Vector2(120f, 26f),
            orange, FontStyles.Bold);

        // ── Divider ───────────────────────────────────
        CreateDivider(new Vector2(0f, 5f));

        // ── Combined efficiency ───────────────────────
        CreateLabel("COMBINED", 13f,
            new Vector2(-160f, -15f), new Vector2(120f, 22f),
            dimWhite, FontStyles.Normal);

        _combinedEffText = CreateLabel("94.5%", 22f,
            new Vector2(50f, -15f), new Vector2(120f, 26f),
            white, FontStyles.Bold);

        // ── Divider ───────────────────────────────────
        CreateDivider(new Vector2(0f, -35f));

        // ── Status ────────────────────────────────────
        _statusText = CreateLabel("● OPTIMAL", 14f,
            new Vector2(0f, -55f), new Vector2(480f, 24f),
            green, FontStyles.Bold);
        _statusText.alignment = TextAlignmentOptions.Center;

        // ── Bottom border ─────────────────────────────
        CreateBorderLine(bottom: true);
    }

    void CreateBorderLine(bool bottom = false)
    {
        GameObject line = new GameObject("BorderLine", typeof(RectTransform));
        line.transform.SetParent(transform, false);
        Image img = line.AddComponent<Image>();
        img.color = cyan;
        RectTransform lrt = line.GetComponent<RectTransform>();
        lrt.anchorMin = new Vector2(0f, bottom ? 0f : 1f);
        lrt.anchorMax = new Vector2(1f, bottom ? 0f : 1f);
        lrt.pivot     = new Vector2(0.5f, bottom ? 0f : 1f);
        lrt.sizeDelta = new Vector2(0f, 2f);
        lrt.anchoredPosition = Vector2.zero;
    }

    void CreateDivider(Vector2 pos)
    {
        GameObject div = new GameObject("Divider", typeof(RectTransform));
        div.transform.SetParent(transform, false);
        Image img = div.AddComponent<Image>();
        img.color = new Color(0f, 0.82f, 1f, 0.2f);
        RectTransform drt = div.GetComponent<RectTransform>();
        drt.anchoredPosition = pos;
        drt.sizeDelta = new Vector2(480f, 1f);
    }

    TextMeshProUGUI CreateLabel(string text, float fontSize,
        Vector2 pos, Vector2 size, Color color, FontStyles style)
    {
        GameObject obj = new GameObject(text.Replace(" ", "_"),
            typeof(RectTransform));
        obj.transform.SetParent(transform, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.color     = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.enableWordWrapping = false;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        return tmp;
    }

    // ─────────────────────────────────────────────────
    //  UPDATE — live values every frame
    // ─────────────────────────────────────────────────

    void Update()
    {
        if (absorberSlider == null || stripperSlider == null) return;

        float absTemp = absorberSlider.value;
        float strTemp = stripperSlider.value;
        bool  overheated = strTemp >= 125f;

        float absEff = Interpolate(absorberTemps, absorberEfficiency, absTemp);
        float strEff = overheated ? 0f :
                       Interpolate(stripperTemps, stripperEfficiency, strTemp);
        float combined = overheated ? 0f : (absEff + strEff) / 2f;

        // Update values
        if (_absorberTempText != null) _absorberTempText.text = $"{absTemp:F0}°C";
        if (_absorberEffText  != null) _absorberEffText.text  = $"{absEff:F1}%";
        if (_stripperTempText != null) _stripperTempText.text = $"{strTemp:F0}°C";

        if (_stripperEffText  != null)
        {
            _stripperEffText.text  = overheated ? "STOP" : $"{strEff:F1}%";
            _stripperEffText.color = overheated ? red : orange;
        }

        if (_combinedEffText  != null)
        {
            _combinedEffText.text  = overheated ? "0.0%" : $"{combined:F1}%";
            _combinedEffText.color = combined >= 90f ? green :
                                     combined >= 75f ? orange : red;
        }

        if (_statusText != null)
        {
            if (overheated)
            { _statusText.text = "⚠ OVERHEATED — STOPPED"; _statusText.color = red; }
            else if (combined >= 90f)
            { _statusText.text = "● OPTIMAL";              _statusText.color = green; }
            else if (combined >= 75f)
            { _statusText.text = "● GOOD";                 _statusText.color = orange; }
            else
            { _statusText.text = "● LOW EFFICIENCY";       _statusText.color = red; }
        }
    }

    // ─────────────────────────────────────────────────
    //  INTERPOLATE efficiency from reference data
    // ─────────────────────────────────────────────────

    float Interpolate(float[] temps, float[] effs, float target)
    {
        if (target <= temps[0])                return effs[0];
        if (target >= temps[temps.Length - 1]) return effs[temps.Length - 1];
        for (int i = 0; i < temps.Length - 1; i++)
        {
            if (target >= temps[i] && target <= temps[i + 1])
            {
                float t = (target - temps[i]) / (temps[i + 1] - temps[i]);
                return Mathf.Lerp(effs[i], effs[i + 1], t);
            }
        }
        return effs[0];
    }
}