using System.Collections;
using UnityEngine;
using DG.Tweening;

public class GameBoard : MonoBehaviour
{
    [SerializeField] GameObject jellyTile;
    [SerializeField] private Jelly[] jellyPrefabs;
    [SerializeField] private Jelly specialJellyPrefabs;
    [SerializeField] Jelly[,] jellyMap; // 젤리 맵의 정보

    private int width; // 게임보드의 가로 사이즈
    private int height; // 게임보드의 세로 사이즈
    private Vector2 centerPos; // Map의 중앙 좌표

    private MatchChecker matchChecker;

    private float dropDuration = 0.5f; // Jelly가 떨어지는데 걸리는 시간

    void Start() {
        SetSize();
        CreateBoard();
        matchChecker = FindObjectOfType<MatchChecker>();
    }

    private void SetSize() {
        width = GameManager.instance.GetGameBoardWidth();
        height = GameManager.instance.GetGameBoardHeight();
    }

    private void CreateBoard() {
        jellyMap = new Jelly[width, height];
        centerPos = GetCenterPos(); // N x M 크기의 보드판의 중앙값 찾기
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector2 pos = new Vector2(x, y) - centerPos;
                Instantiate<GameObject>(jellyTile, pos, Quaternion.identity);
                jellyMap[x, y] = CreateRandomJelly(x, y, pos, true);
            }
        }
    }

    private Vector2 GetCenterPos() {
        return new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
    }
    
    private Jelly CreateRandomJelly(int x, int y, Vector2 pos, bool init) {
        int index = Random.Range(0, jellyPrefabs.Length);

        if (init) { // 만약 처음 보드를 만드는 상황이면 매치되는 젤리들이 존재하면 안 됨
            while (CheckHorizontal(x, y, index) || CheckVertical(x, y, index)) {
                index = Random.Range(0, jellyPrefabs.Length);
            }
        }

        Jelly jelly = Instantiate<Jelly>(jellyPrefabs[index], pos, Quaternion.identity);
        jelly.SetPos(x, y);
        return jelly;
    }

    private Jelly CreateSpecialJelly(int x, int y, Vector2 pos) {
        Jelly jelly = Instantiate<Jelly>(specialJellyPrefabs, pos, Quaternion.identity);
        jelly.SetPos(x, y);
        return jelly;
    }

    private bool CheckHorizontal(int x, int y, int index) { // 동일한 젤리 3개가 가로로 나열되는지 check
        if (x >= 2) {
            if (jellyMap[x - 1, y].GetJellyType() == jellyPrefabs[index].GetJellyType() // 왼쪽 2개의 Jelly Type이 같으면 다른 젤리를 생성
            && jellyMap[x - 2, y].GetJellyType() == jellyPrefabs[index].GetJellyType()) {
                return true;
            }
        }
        return false;
    }

    private bool CheckVertical(int x, int y, int index) { // 동일한 젤리 3개가 세로로 나열되는지 check
        if (y >= 2) {
            if (jellyMap[x, y - 1].GetJellyType() == jellyPrefabs[index].GetJellyType() // 아래 2개의 Jelly Type이 같으면 다른 젤리를 생성
            && jellyMap[x, y - 2].GetJellyType() == jellyPrefabs[index].GetJellyType()) {
                return true;
            }
        }
        return false;
    }

    public void StartRemoveJelliesRoutine() {
        StartCoroutine(RemoveJelliesRoutine());
    }

    IEnumerator RemoveJelliesRoutine() {
        bool addSpecialJelly = matchChecker.GetJellyLists().Count >= 4 ? true : false;

        RemoveMatchedJellies();
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlayJellyRemoveSound();
        DropJellies();
        SoundManager.instance.PlayJellyDropSound();
        yield return new WaitForSeconds(1f);
        FillJellies((!matchChecker.GetUseSpecialJelly() && addSpecialJelly)); // 스페셜 젤리를 사용하지 않았고, 한 번에 4개 이상이 제거되면 스페셜 젤리 생성
        SoundManager.instance.PlayJellyDropSound();
        yield return new WaitForSeconds(1f);
        
        matchChecker.SetUseSpecialJelly(true); // 처음 Swap 이후로는 스페셜 젤리를 사용했던 안 했던 4개 이상이어도 스페셜 젤리 생성 x
        matchChecker.CheckAllMatches();
        if (!GameManager.instance.GetIsGameOver() && matchChecker.GetJellyLists().Count > 0) { // 새로 생성된 젤리로 인해 매치가 또 존재하면
            StartRemoveJelliesRoutine(); // 처음 Swap 당시에 4개 이상일 때만 스페셜 젤리 생성, 이후는 x
        } else {
            matchChecker.SetUseSpecialJelly(false);
            GameManager.instance.SetIsReady(true);
            GameManager.instance.CheckGameOver();
        }
    }

    public void RemoveMatchedJellies() {
        foreach (Jelly jelly in matchChecker.GetJellyLists()) {
            int x = jelly.GetPosX();
            int y = jelly.GetPosY();
            SetJellyMap(x, y, null); // (x, y)에 있는 Jelly의 정보를 null로
            jelly.Remove(); // Object 삭제
        }
    }

    private void DropJellies() {
        for (int x = 0; x < width; x++) {
            int cnt = 0; // 빈 공간의 개수
            for (int y = 0; y < height; y++) {
                if (jellyMap[x, y] == null) {
                    cnt++;
                    continue;
                } else if (cnt == 0) {
                    continue;
                }

                Vector2 pos = new Vector2(x, y - cnt) - centerPos; // cnt 값만큼 빈 공간이 있다는 뜻이므로 cnt만큼 내려가야함
                jellyMap[x, y - cnt] = jellyMap[x, y]; // Jelly 정보 이동
                jellyMap[x, y].transform.DOMove(pos, dropDuration).SetEase(Ease.OutBounce); // 도착했을 때 바운스 효과
                jellyMap[x, y].SetPos(x, y - cnt);
                jellyMap[x, y] = null;
            }
        }
    }

    private void FillJellies(bool addSpecialJelly) {
        int emptySpaceCnt = GetEmptySpaceCount(); // 빈 공간 카운트
        int specialJellyIndex = Random.Range(0, emptySpaceCnt);

        int jellyIndex = 0;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (jellyMap[x, y] == null) {
                    Vector2 pos = new Vector2(x, y) - centerPos; 
                    if (addSpecialJelly && jellyIndex == specialJellyIndex) {
                        jellyMap[x, y] = CreateSpecialJelly(x, y, pos + Vector2.up * height); // 스페셜 젤리 생성
                    } else {
                        jellyMap[x, y] = CreateRandomJelly(x, y, pos + Vector2.up * height, false); // 일반 젤리 생성
                    }
                    jellyMap[x, y].transform.DOMove(pos, dropDuration * 0.5f).SetEase(Ease.OutBounce);
                    jellyIndex++;
                }
            }
        }
    }

    private int GetEmptySpaceCount() {
        int cnt = 0;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (jellyMap[x, y] == null) {
                    cnt++;
                }
            }
        }
        return cnt;
    }

    public Jelly[,] GetJellyMap() {
        return jellyMap;
    }

    public void SetJellyMap(int x, int y, Jelly jelly) {
        jellyMap[x, y] = jelly;
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
}