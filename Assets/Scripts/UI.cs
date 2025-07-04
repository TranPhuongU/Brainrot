using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Panel Setting")]
    public GameObject settingUI;
    public TextMeshProUGUI score;

    private float cachedTimeScale = 1f;

    private int displayedScore = 0;
    private int targetScore = 0;
    [SerializeField] private float scoreChangeSpeed = 500f; // điểm/giây

    private void Update()
    {
        targetScore = GameManager.Instance.currentScore;

        if (displayedScore != targetScore)
        {
            // Tăng hoặc giảm dần về targetScore
            displayedScore = (int)Mathf.MoveTowards(displayedScore, targetScore, scoreChangeSpeed * Time.unscaledDeltaTime);
            score.text = displayedScore.ToString();
        }
    }

    public void OnSetting()
    {
        settingUI.SetActive(true);

        cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        AudioListener.pause = true;
    }

    public void OffSetting()
    {
        settingUI.SetActive(false);

        Time.timeScale = cachedTimeScale;

        AudioListener.pause = false;
    }
}
