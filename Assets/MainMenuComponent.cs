using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuComponent : MonoBehaviour
{
    // Start is called before the first frame update
    //async void Start()
    //{
    //    await RequestLogin.LoginAsync();
    //    await RequestProfile.GetProfileAsync();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}


    [Header("UI")]
    public GameObject loadingPanel;
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;
    public Button retryButton;

    [Tooltip("How fast the slider fills (units per second)")]
    public float fillSpeed = 0.5f;

    private float targetProgress = 0f;
    private bool isLoading = false;

    private void Awake()
    {
        if (loadingText != null)
        {
            var rt = loadingText.rectTransform;
            rt.anchorMin = new Vector2(0.1f, 0.5f);
            rt.anchorMax = new Vector2(0.9f, 0.7f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            loadingText.alignment = TextAlignmentOptions.Center;
            loadingText.enableWordWrapping = true;
        }

        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(false);
            retryButton.onClick.AddListener(OnRetryClicked);

            var btnRT = retryButton.GetComponent<RectTransform>();
            btnRT.anchorMin = new Vector2(0.4f, 0.3f);
            btnRT.anchorMax = new Vector2(0.6f, 0.4f);
            btnRT.pivot = new Vector2(0.5f, 0.5f);
            btnRT.anchoredPosition = Vector2.zero;
        }
    }

    private async void Start()
    {
        await BeginLoadSequence();
    }

    private async Task BeginLoadSequence()
    {
        loadingText.text = "Loading...";
        retryButton?.gameObject.SetActive(false);
        loadingPanel?.SetActive(true);

        if (loadingSlider != null)
        {
            loadingSlider.minValue = 0f;
            loadingSlider.maxValue = 1f;
            loadingSlider.value = 0f;
        }

        isLoading = true;
        targetProgress = 0f;

        // 1) Login
        try
        {
            await RequestLogin.LoginAsync();
        }
        catch
        {
            ShowError();
            return;
        }
        targetProgress = 0.5f;

        // 2) Profile
        try
        {
            await RequestProfile.GetProfileAsync();
        }
        catch
        {
            ShowError();
            return;
        }
        targetProgress = 1f;
    }

    private void Update()
    {
        if (!isLoading || loadingSlider == null) return;

        loadingSlider.value = Mathf.MoveTowards(
            loadingSlider.value,
            targetProgress,
            fillSpeed * Time.deltaTime
        );

        if (loadingSlider.value >= 1f && targetProgress >= 1f)
        {
            isLoading = false;
            loadingPanel?.SetActive(false);
        }
    }

    private void ShowError()
    {
        isLoading = false;
        loadingText.text = "Failed to load.\nPlease check your connection.";
        loadingText.color = Color.red;

        retryButton?.gameObject.SetActive(true);
    }

    private async void OnRetryClicked()
    {
        retryButton?.gameObject.SetActive(false);
        await BeginLoadSequence();
    }
}
