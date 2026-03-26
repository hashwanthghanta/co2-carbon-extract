// =====================================================
//  OverheatAlertManager.cs
//  Shows a red full-screen alert when stripper
//  reaches 125°C and hides it when temp drops
// =====================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OverheatAlertManager : MonoBehaviour
{
    public static OverheatAlertManager Instance;

    [Header("UI References")]
    [Tooltip("Full screen red overlay panel")]
    public GameObject alertPanel;

    [Tooltip("Main alert title text")]
    public TextMeshProUGUI alertTitle;

    [Tooltip("Alert description text")]
    public TextMeshProUGUI alertMessage;

    [Tooltip("Stripper temperature slider")]
    public UnityEngine.UI.Slider stripperSlider;

    private bool _isAlertActive = false;
    private Coroutine _pulseCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (alertPanel != null)
            alertPanel.SetActive(false);

        if (stripperSlider != null)
            stripperSlider.onValueChanged.AddListener(OnStripperTempChanged);
    }

    private void OnStripperTempChanged(float value)
    {
        if (value >= 125f && !_isAlertActive)
            ShowAlert();
        else if (value < 125f && _isAlertActive)
            HideAlert();
    }

    public void ShowAlert()
    {
        _isAlertActive = true;

        if (alertPanel != null)
            alertPanel.SetActive(true);

        if (alertTitle != null)
            alertTitle.text = "⚠ SYSTEM CRITICAL — OVERHEAT DETECTED";

        if (alertMessage != null)
            alertMessage.text =
                "Stripper temperature has reached 125°C.\n" +
                "The solvent regeneration process has been terminated.\n" +
                "CO2 capture is no longer active.\n\n" +
                "Reduce the Stripper temperature below 125°C to resume operations.";

        if (_pulseCoroutine != null)
            StopCoroutine(_pulseCoroutine);
        _pulseCoroutine = StartCoroutine(PulseAlert());

        // Also notify the notification panel
        if (NotificationManager.Instance != null)
            NotificationManager.Instance.ShowNotification(
                "⚠ CRITICAL — OVERHEAT",
                "Stripper at 125°C! Simulation stopped. Reduce temperature.");
    }

    public void HideAlert()
    {
        _isAlertActive = false;

        if (_pulseCoroutine != null)
        {
            StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = null;
        }

        if (alertPanel != null)
            alertPanel.SetActive(false);

        // Notify recovery
        if (NotificationManager.Instance != null)
            NotificationManager.Instance.ShowNotification(
                "SYSTEM RECOVERED",
                "Temperature normalised. CO2 capture resuming.");
    }

    // Pulsing red effect to grab attention
    private IEnumerator PulseAlert()
    {
        Image bg = alertPanel?.GetComponent<Image>();
        if (bg == null) yield break;

        Color baseColor  = new Color(0.7f, 0f, 0f, 0.85f);
        Color peakColor  = new Color(1f,  0f, 0f, 0.95f);

        while (_isAlertActive)
        {
            float t = Mathf.PingPong(Time.time * 1.5f, 1f);
            bg.color = Color.Lerp(baseColor, peakColor, t);
            yield return null;
        }

        if (bg != null)
            bg.color = baseColor;
    }

    public bool IsOverheated => _isAlertActive;
}
