// =====================================================
//  VideoHighlightManager.cs — FINAL v5
//  5 Component Highlights + Control Panel
//  Absorber → Stripper → Reboiler → Storage → FilterPanel
// =====================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoHighlightManager : MonoBehaviour
{
    [Header("── Components to Highlight ──────────────")]
    public GameObject absorberObject;       // Absorber_draft_13_AT_0502_WithoutAnimationPipes
    public GameObject stripperObject;       // MEA_Shower_Stripper
    public GameObject reboilerObject;       // Reboiler Shell
    public GameObject storageObject;        // CO2Storage.001
    public GameObject controlPanelObject;   // FilterPanel

    [Header("── Dark Overlay ────────────────────────")]
    public Image darkOverlay;

    [Header("── Settings ─────────────────────────────")]
    [Range(0f, 0.90f)]
    public float overlayDarkness = 0.75f;
    public float fadeDuration = 0.8f;

    [Header("── Tint Colors ──────────────────────────")]
    public Color highlightTint = new Color(0.5f, 1.0f, 1.0f, 1.0f);
    public Color darkenTint    = new Color(0.15f, 0.15f, 0.15f, 1.0f);
    public bool darkenOtherObjects = false;

    [Header("── Auto Run ─────────────────────────────")]
    public bool autoRunOnStart = true;

    // ─────────────────────────────────────────────────
    // Timestamps in seconds
    private const float SCENE1_DUR = 10f;  // Introduction
    private const float SCENE2_DUR = 20f;  // Absorber
    private const float SCENE3_DUR = 20f;  // Stripper
    private const float SCENE4_DUR = 25f;  // Reboiler
    private const float SCENE5_DUR = 20f;  // Storage
    private const float SCENE6_DUR = 10f;  // Control Panel

    private Dictionary<Material, Color> _originalColors = new Dictionary<Material, Color>();
    private Renderer[] _allRenderers;

    // ─────────────────────────────────────────────────

    void Start()
    {
        if (darkOverlay != null)
            darkOverlay.color = new Color(0f, 0f, 0f, 0f);

        // Cache colors of all highlight objects
        CacheColors(absorberObject);
        CacheColors(stripperObject);
        CacheColors(reboilerObject);
        CacheColors(storageObject);

        _allRenderers = FindObjectsOfType<Renderer>();

        if (autoRunOnStart)
            StartCoroutine(RunSequence());
    }

    private void CacheColors(GameObject obj)
    {
        if (obj == null) return;
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            foreach (Material mat in r.materials)
                if (!_originalColors.ContainsKey(mat))
                    _originalColors[mat] = mat.HasProperty("_Color") ? mat.color : Color.white;
    }

    // ─────────────────────────────────────────────────
    //  MAIN SEQUENCE
    // ─────────────────────────────────────────────────

    private IEnumerator RunSequence()
    {
        Debug.Log("[VHM] Sequence started.");

        // ── Scene 1: Introduction — no highlight ──────
        yield return new WaitForSeconds(SCENE1_DUR);

        // ── Scene 2: Absorber ─────────────────────────
        yield return StartCoroutine(FadeOverlay(0f, overlayDarkness));
        Highlight(absorberObject, true);
        Debug.Log("[VHM] Absorber highlighted.");
        yield return new WaitForSeconds(SCENE2_DUR);
        Highlight(absorberObject, false);
        RestoreColors(absorberObject);
        yield return new WaitForSeconds(0.3f);

        // ── Scene 3: Stripper ─────────────────────────
        Highlight(stripperObject, true);
        Debug.Log("[VHM] Stripper highlighted.");
        yield return new WaitForSeconds(SCENE3_DUR);
        Highlight(stripperObject, false);
        RestoreColors(stripperObject);
        yield return new WaitForSeconds(0.3f);

        // ── Scene 4: Reboiler ─────────────────────────
        Highlight(reboilerObject, true);
        Debug.Log("[VHM] Reboiler highlighted.");
        yield return new WaitForSeconds(SCENE4_DUR);
        Highlight(reboilerObject, false);
        RestoreColors(reboilerObject);
        yield return new WaitForSeconds(0.3f);

        // ── Scene 5: Storage ──────────────────────────
        Highlight(storageObject, true);
        Debug.Log("[VHM] Storage highlighted.");
        yield return new WaitForSeconds(SCENE5_DUR);
        Highlight(storageObject, false);
        RestoreColors(storageObject);
        yield return new WaitForSeconds(0.3f);

        // ── Scene 6: Control Panel ────────────────────
        Highlight(controlPanelObject, true);
        Debug.Log("[VHM] Control Panel highlighted.");
        yield return new WaitForSeconds(SCENE6_DUR);
        Highlight(controlPanelObject, false);

        // ── Done ──────────────────────────────────────
        yield return StartCoroutine(FadeOverlay(overlayDarkness, 0f));
        Debug.Log("[VHM] Sequence complete.");
    }

    // ─────────────────────────────────────────────────
    //  HIGHLIGHT — raises render queue above overlay
    //  and applies cyan tint
    // ─────────────────────────────────────────────────

    private void Highlight(GameObject obj, bool on)
    {
        if (obj == null) return;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        HashSet<Renderer> targetSet = new HashSet<Renderer>(renderers);

        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                mat.renderQueue = on ? 4000 : 2000;
                if (mat.HasProperty("_Color"))
                    mat.color = on ? highlightTint : (
                        _originalColors.ContainsKey(mat) ? _originalColors[mat] : Color.white);
            }
        }

        if (darkenOtherObjects)
        {
            foreach (Renderer r in _allRenderers)
            {
                if (targetSet.Contains(r)) continue;
                foreach (Material mat in r.materials)
                    if (mat.HasProperty("_Color"))
                        mat.color = on ? darkenTint : (
                            _originalColors.ContainsKey(mat) ? _originalColors[mat] : Color.white);
            }
        }
    }

    private void RestoreColors(GameObject obj)
    {
        if (obj == null) return;
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            foreach (Material mat in r.materials)
                if (_originalColors.ContainsKey(mat)) mat.color = _originalColors[mat];
    }

    // ─────────────────────────────────────────────────
    //  FADE OVERLAY
    // ─────────────────────────────────────────────────

    private IEnumerator FadeOverlay(float from, float to)
    {
        if (darkOverlay == null) yield break;
        darkOverlay.canvas.sortingOrder = 10;
        float elapsed = 0f;
        Color col = darkOverlay.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            col.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            darkOverlay.color = col;
            yield return null;
        }
        col.a = to;
        darkOverlay.color = col;
    }
}
