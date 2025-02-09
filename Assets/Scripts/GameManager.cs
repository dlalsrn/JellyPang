using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    private const int NUM_JELLY = 6; // 게임에 등장하는 젤리의 종류
    private const int MOVELIMIT = 10; // 움직일 수 있는 횟수 기본 값
    private const int ADDMOVE = 5; // 각 레벨별 추가로 움직일 수 있는 횟수
    private const int BOARD_WIDTH = 9; // 보드 가로 사이즈
    private const int BOARD_HEIGHT = 9; // 보드 세로 사이즈

    private int currentLevel; // 현재 레벨
    private int currentMove; // 현재 움직인 횟수

    private int[] goalJelly = {10, 15, 20}; // i+1번째 level의 목표 젤리 개수
    private int[] remainGoalJellies; // 각 젤리마다 제거해야하는 개수

    private bool isReady; // 젤리를 움직일 수 있는 상태인지 (젤리가 제거되거나 새로 생성 중이면 마우스 컨트롤 x)
    private bool isGameOver; // 게임이 끝났는지

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        Init();
        InitGameStartPanel();
    }

    private void Init() { // 게임 세팅
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        currentMove = 0;
        isReady = false;
        isGameOver = false;
    }

    private void InitGameStartPanel() {
        HUDManager.instance.ShowGameStartPanel();
        HUDManager.instance.UpdateStageNumImage(currentLevel);
        HUDManager.instance.UpdateGoalMoveText(MOVELIMIT + currentLevel * ADDMOVE);
        HUDManager.instance.UpdateGoalJellyText(goalJelly[currentLevel]);
    }

    private void InitGameInfoPanel() {
        HUDManager.instance.HideGameStartPanel();
        remainGoalJellies = new int[NUM_JELLY];
        HUDManager.instance.UpdateCurrentLevelText(currentLevel + 1); // 현재 레벨
        HUDManager.instance.UpdateRemainTimeText(MOVELIMIT + currentLevel * ADDMOVE); // 남은 횟수
        for (int i = 0; i < NUM_JELLY; i++) {
            remainGoalJellies[i] = goalJelly[currentLevel];
            HUDManager.instance.UpdateRemainJelly(remainGoalJellies[i], i);
        }
        isReady = true;
    }

    public void SettingClear() {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Scenes/GameScene");
    }

    public int GetGameBoardWidth() {
        return BOARD_WIDTH;
    }

    public int GetGameBoardHeight() {
        return BOARD_HEIGHT;
    }

    public int GetRemainMove() { // 현재 남아있는 움직일 수 있는 횟수
        return (MOVELIMIT + currentLevel * ADDMOVE) - currentMove;
    }

    public void DecreaseRemainMove() { // 움직임 - 1
        currentMove++;
        HUDManager.instance.UpdateRemainTimeText(GetRemainMove());
    }

    private bool CheckRemainJellies() { // 젤리가 남아있으면 true, 다 없앴으면 false
        foreach(int num in remainGoalJellies) {
            if (num != 0) {
                return true;
            }
        }
        return false;
    }

    public void DecreaseRemainJelly(int index) { // index번 젤리의 목표 개수 - 1
        if (remainGoalJellies[index] > 0) {
            remainGoalJellies[index]--;
        }
        HUDManager.instance.UpdateRemainJelly(remainGoalJellies[index], index);
    }

    private void GameOver() { // 움직임 횟수 모두 소모
        if (!isGameOver) {
            isGameOver = true;
            isReady = false;
            HUDManager.instance.ShowResultPanel(false);
        }
    }

    private void LevelUp() { // 스테이지 클리어
        if (!isGameOver) {
            isGameOver = true;
            isReady = false;
            PlayerPrefs.SetInt("CurrentLevel", (currentLevel < 2 ? currentLevel + 1 : 2));
            HUDManager.instance.ShowResultPanel(true);
        }
    }

    public void CheckGameOver() {
        if (!CheckRemainJellies()) { // 남아있는 젤리가 없으면
            LevelUp();
        } else if (isGameOver || GetRemainMove() == 0) {
            GameOver();
        }
    }

    public void SetIsReady(bool ready) {
        isReady = ready;
    }

    public bool GetIsReady() {
        return isReady;
    }

    public bool GetIsGameOver() {
        return isGameOver;
    }

    public void StartGame() {
        InitGameInfoPanel();
    }

    public void LoadMenuScene() {
        SceneManager.LoadScene("Scenes/MenuScene");
    }

    public void LoadGameScene() {
        SceneManager.LoadScene("Scenes/GameScene");
    }
}
