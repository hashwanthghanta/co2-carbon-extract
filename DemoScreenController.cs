using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class DemoScreenController : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument uiDocument;

    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("Video Display")]
    public RawImage videoDisplay;

    [Header("Timeline Slider")]
    public UnityEngine.UI.Slider videoTimeline;

    private UnityEngine.UIElements.Button btnPlayPause;
    private UnityEngine.UIElements.Button btnReplay;
    private UnityEngine.UIElements.Button btnSimulation;
    private UnityEngine.UIElements.Button btnMainMenu;
    private UnityEngine.UIElements.Button btnQuit;

    private VisualElement _root;
    private bool isPlaying = false;
    private bool isScrubbing = false;

    void OnEnable()
    {
        _root = uiDocument.rootVisualElement;

        btnPlayPause  = _root.Q<UnityEngine.UIElements.Button>("btn-playpause");
        btnReplay     = _root.Q<UnityEngine.UIElements.Button>("btn-replay");
        btnSimulation = _root.Q<UnityEngine.UIElements.Button>("btn-simulation");
        btnMainMenu   = _root.Q<UnityEngine.UIElements.Button>("btn-mainmenu");
        btnQuit       = _root.Q<UnityEngine.UIElements.Button>("btn-quit");

        btnPlayPause.clicked  += OnPlayPause;
        btnReplay.clicked     += OnReplay;
        btnSimulation.clicked += OnGoToSimulation;
        btnMainMenu.clicked   += OnGoToMainMenu;
        if (btnQuit != null) btnQuit.clicked += OnQuit;

        _root.RegisterCallback<KeyDownEvent>(OnKeyDown);
        _root.focusable = true;
        _root.Focus();
    }

    void OnDisable()
    {
        btnPlayPause.clicked  -= OnPlayPause;
        btnReplay.clicked     -= OnReplay;
        btnSimulation.clicked -= OnGoToSimulation;
        btnMainMenu.clicked   -= OnGoToMainMenu;
        if (btnQuit != null) btnQuit.clicked -= OnQuit;
        _root?.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    void Start()
    {
        if (videoPlayer != null && videoDisplay != null)
            videoPlayer.targetTexture = (RenderTexture)videoDisplay.texture;

        if (videoTimeline != null)
            videoTimeline.onValueChanged.AddListener(OnTimelineScrub);

        if (videoPlayer != null)
        {
            videoPlayer.Play();
            isPlaying = true;
            if (btnPlayPause != null)
                btnPlayPause.text = "II  PLAY / PAUSE";
        }
    }

    void Update()
    {
        if (videoPlayer != null && videoPlayer.isPlaying && !isScrubbing)
        {
            if (videoPlayer.length > 0)
            {
                float progress = (float)(videoPlayer.time / videoPlayer.length);
                if (videoTimeline != null)
                    videoTimeline.SetValueWithoutNotify(progress);
            }
        }
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Escape)
        {
            evt.StopPropagation();
            OnGoToMainMenu();
        }
    }

    void OnTimelineScrub(float value)
    {
        if (videoPlayer != null && videoPlayer.length > 0)
        {
            isScrubbing = true;
            videoPlayer.time = value * videoPlayer.length;
            isScrubbing = false;
        }
    }

    void OnPlayPause()
    {
        if (videoPlayer == null) return;
        if (isPlaying)
        {
            videoPlayer.Pause();
            btnPlayPause.text = "PLAY / PAUSE";
            isPlaying = false;
        }
        else
        {
            videoPlayer.Play();
            btnPlayPause.text = "II  PLAY / PAUSE";
            isPlaying = true;
        }
    }

    void OnReplay()
    {
        if (videoPlayer == null) return;
        videoPlayer.Stop();
        videoPlayer.Play();
        btnPlayPause.text = "II  PLAY / PAUSE";
        isPlaying = true;
        if (videoTimeline != null)
            videoTimeline.SetValueWithoutNotify(0f);
    }

    void OnGoToSimulation() { SceneManager.LoadScene("SampleScene"); }
    void OnGoToMainMenu()   { SceneManager.LoadScene("SampleScene 1"); }

    void OnQuit()
    {
        Debug.Log("[DemoScreenController] Quitting application.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
