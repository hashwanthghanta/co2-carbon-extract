using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    [Header("Scene to load when LAUNCH is clicked")]
    [SerializeField] private string simulationSceneName = "SampleScene";

    [Header("Scene to load when DEMO SCREEN is clicked")]
    [SerializeField] private string demoSceneName = "DemoScreen";

    [Header("Background image (assign a Sprite asset)")]
    [SerializeField] private Sprite backgroundSprite;

    private VisualElement _root;

    private Button _btnLaunch;
    private Button _btnDemo;
    private Button _btnInfo;
    private Button _btnAbout;
    private Button _btnCredits;
    private Button _btnQuit;

    private VisualElement _overlay;
    private VisualElement _modalInfo;
    private VisualElement _modalAbout;
    private VisualElement _modalCredits;

    private Button _closeInfo;
    private Button _closeAbout;
    private Button _closeCredits;

    private VisualElement _background;
    private VisualElement _openModal;

    private void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        if (doc == null) { Debug.LogError("[MainMenuController] No UIDocument found!"); return; }
        _root = doc.rootVisualElement;
        _root.schedule.Execute(Setup).StartingIn(200);
    }

    private void OnDisable() { Unsubscribe(); }

    private void Setup()
    {
        Debug.Log("[MainMenuController] Setup started...");

        _background = _root.Q<VisualElement>("background");
        if (_background != null && backgroundSprite != null)
            _background.style.backgroundImage = new StyleBackground(backgroundSprite);

        _btnLaunch  = _root.Q<Button>("btn-launch");
        _btnDemo    = _root.Q<Button>("btn-demo");
        _btnInfo    = _root.Q<Button>("btn-info");
        _btnAbout   = _root.Q<Button>("btn-about");
        _btnCredits = _root.Q<Button>("btn-credits");
        _btnQuit    = _root.Q<Button>("btn-quit");

        _overlay      = _root.Q<VisualElement>("modal-overlay");
        _modalInfo    = _root.Q<VisualElement>("modal-info");
        _modalAbout   = _root.Q<VisualElement>("modal-about");
        _modalCredits = _root.Q<VisualElement>("modal-credits");

        _closeInfo    = _root.Q<Button>("close-info");
        _closeAbout   = _root.Q<Button>("close-about");
        _closeCredits = _root.Q<Button>("close-credits");

        if (!CheckReferences()) { Debug.LogError("[MainMenuController] Setup failed."); return; }

        CloseModal();
        Subscribe();

        Debug.Log("[MainMenuController] All buttons wired. Ready to play!");
    }

    private void Subscribe()
    {
        _btnLaunch.clicked  += OnLaunch;
        _btnDemo.clicked    += OnDemo;
        _btnInfo.clicked    += () => OpenModal(_modalInfo);
        _btnAbout.clicked   += () => OpenModal(_modalAbout);
        _btnCredits.clicked += () => OpenModal(_modalCredits);
        _closeInfo.clicked  += CloseModal;
        _closeAbout.clicked += CloseModal;
        _closeCredits.clicked += CloseModal;
        if (_btnQuit != null) _btnQuit.clicked += OnQuit;
        _overlay.RegisterCallback<ClickEvent>(OnOverlayClick);
        _root.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    private void Unsubscribe()
    {
        if (_btnLaunch    != null) _btnLaunch.clicked    -= OnLaunch;
        if (_btnDemo      != null) _btnDemo.clicked      -= OnDemo;
        if (_closeInfo    != null) _closeInfo.clicked    -= CloseModal;
        if (_closeAbout   != null) _closeAbout.clicked   -= CloseModal;
        if (_closeCredits != null) _closeCredits.clicked -= CloseModal;
        if (_btnQuit      != null) _btnQuit.clicked      -= OnQuit;
        _overlay?.UnregisterCallback<ClickEvent>(OnOverlayClick);
        _root?.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    private void OnLaunch()
    {
        Debug.Log($"[MainMenuController] Loading scene: {simulationSceneName}");
        SceneManager.LoadScene(simulationSceneName);
    }

    private void OnDemo()
    {
        Debug.Log($"[MainMenuController] Loading Demo Screen: {demoSceneName}");
        SceneManager.LoadScene(demoSceneName);
    }

    private void OnQuit()
    {
        Debug.Log("[MainMenuController] Quitting application.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OpenModal(VisualElement modal)
    {
        if (modal == null) { Debug.LogWarning("[MainMenuController] OpenModal called with null modal."); return; }
        if (_openModal != null) _openModal.style.display = DisplayStyle.None;
        _overlay.style.display = DisplayStyle.Flex;
        modal.style.display    = DisplayStyle.Flex;
        _openModal = modal;
    }

    private void CloseModal()
    {
        if (_openModal != null) { _openModal.style.display = DisplayStyle.None; _openModal = null; }
        if (_overlay   != null)   _overlay.style.display   = DisplayStyle.None;
    }

    private void OnOverlayClick(ClickEvent evt)
    {
        if (evt.target == _overlay) CloseModal();
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Escape && _openModal != null)
        {
            CloseModal();
            evt.StopPropagation();
        }
    }

    private bool CheckReferences()
    {
        bool ok = true;
        void Warn(string n) { Debug.LogWarning($"[MainMenuController] Could not find '{n}' in UXML."); ok = false; }
        if (_btnLaunch    == null) Warn("btn-launch");
        if (_btnDemo      == null) Warn("btn-demo");
        if (_btnInfo      == null) Warn("btn-info");
        if (_btnAbout     == null) Warn("btn-about");
        if (_btnCredits   == null) Warn("btn-credits");
        if (_overlay      == null) Warn("modal-overlay");
        if (_modalInfo    == null) Warn("modal-info");
        if (_modalAbout   == null) Warn("modal-about");
        if (_modalCredits == null) Warn("modal-credits");
        if (_closeInfo    == null) Warn("close-info");
        if (_closeAbout   == null) Warn("close-about");
        if (_closeCredits == null) Warn("close-credits");
        return ok;
    }
}
