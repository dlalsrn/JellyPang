using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance = null;

    // Game Info Panel
    [SerializeField] private GameObject gameInfoPanel; // 진행 상황 Panel
    [SerializeField] private TextMeshProUGUI remainMoveText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI[] remainJellyTexts;

    // Game Start Panel
    [SerializeField] private GameObject gameStartPanel; // 게임의 목표를 보여주는 Panel
    [SerializeField] private Image stageNumIamge;
    [SerializeField] private Sprite[] stageNumImages;
    [SerializeField] private TextMeshProUGUI goalMoveText;
    [SerializeField] private TextMeshProUGUI[] goalJellyTexts;

    // Game Over panel
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button progressButton;
    [SerializeField] private Sprite restartImage;
    [SerializeField] private Sprite okImage;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void UpdateRemainTimeText(int remainMove) {
        remainMoveText.text = $"{remainMove}";
    }

    public void UpdateCurrentLevelText(int level) {
        currentLevelText.text = $"Level {level}";
    }

    public void UpdateRemainJelly(int remain, int index) {
        remainJellyTexts[index].text = $"{remain}";
    }

    public void UpdateStageNumImage(int currentLevel) {
        stageNumIamge.sprite = stageNumImages[currentLevel];
    }

    public void UpdateGoalMoveText(int move) {
        goalMoveText.text = $"{move}";
    }

    public void UpdateGoalJellyText(int jellyNum) {
        foreach (TextMeshProUGUI goalText in goalJellyTexts) {
            goalText.text = $"{jellyNum}";
        }
    }

    public void ShowGameStartPanel() {
        gameStartPanel.SetActive(true);
        gameInfoPanel.SetActive(true);
    }

    public void HideGameStartPanel() {
        gameStartPanel.SetActive(false);
    }

    public void ShowResultPanel(bool result) {
        gameInfoPanel.SetActive(false);
        resultPanel.SetActive(true);
        if (result) { // 스테이지를 클리어 했으면
            SetOKButton();
            SetResultText("Mission Complete");
        } else {
            SetRestartButton();
            SetResultText("Mission Fail");
        }
    }

    public void SetRestartButton() {
        progressButton.GetComponent<Image>().sprite = restartImage;
    }

    public void SetOKButton() {
        progressButton.GetComponent<Image>().sprite = okImage;
    }

    public void SetResultText(string text) {
        resultText.text = text;
    } 
}
