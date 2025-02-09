using System.Collections;
using UnityEngine;
using DG.Tweening;

public class JellyController : MonoBehaviour
{
    private GameBoard gameBoard;

    private Vector2 mouseDownPosition; // 마우스를 클릭했을 때의 pos
    private Vector2 mouseUpPosition; // 마우스 클릭을 놓았을 때의 pos
    private Vector2 mouseScalingPos; // camera 기준의 좌표

    private Jelly firstJelly; // 마우스를 클릭했을 때 선택한 Jelly;
    private Vector2 firstJellyPos;

    private Jelly secondJelly; // 마우스 클릭을 놓았을 때 선택한 Jelly;
    private Vector2 secondJellyPos;

    private float moveDuration = 0.2f; // Jelly가 서로 Swap하는데 걸리는 시간

    private MatchChecker matchChecker;

    void Start() {
        gameBoard = FindObjectOfType<GameBoard>();
        matchChecker = FindObjectOfType<MatchChecker>();
    }

    void Update() {
        if (!GameManager.instance.GetIsReady()) { // 아직 준비가 안 됨
            return;
        } else if (GameManager.instance.GetIsGameOver()) { // 움직임을 모두 소모했거나, 목표를 달성했거나
            return;
        }

        if (Input.GetMouseButtonDown(0)) { // 마우스 클릭
            mouseDownPosition = Input.mousePosition;
            mouseScalingPos = Camera.main.ScreenToWorldPoint(mouseDownPosition); // 카메라 기준으로 좌표 값 스케일링
            RaycastHit2D hit = Physics2D.Raycast(mouseScalingPos, Vector2.zero); // 마우스 위치에 있는 오브젝트의 정보를 가져옴
            if (hit.collider != null) {
                firstJelly = hit.collider.GetComponent<Jelly>();
            }
        }

        if (Input.GetMouseButtonUp(0)) { // 마우스 떼기
            mouseUpPosition = Input.mousePosition;

            // 마우스를 눌렀을 때와 놓았을 때의 각도
            float angle = Mathf.Atan2(mouseUpPosition.y - mouseDownPosition.y, mouseUpPosition.x - mouseDownPosition.x) * Mathf.Rad2Deg; 
            // -45 ~ 45 : Right
            // 45 ~ 135 : Up
            // 135 ~ , ~ -135 : Left
            // -135 ~ -45 : Dwon
            secondJelly = GetSecondJelly(angle);

            if (firstJelly != null && secondJelly != null) {
                GameManager.instance.SetIsReady(false);
                SwapJellies();
                StartCoroutine(CheckMatchRoutine());
                GameManager.instance.DecreaseRemainMove(); // 움직임 횟수 1 감소, 매치 유무 상관 x
            } else {
                ResetJellies(); // 리셋을 안해주면 마지막에 선택한 Jelly를 기준으로 허공에 드래그를 해도 이동이 됨
            }
        }
    }

    private Jelly GetSecondJelly(float angle) {
        if (firstJelly == null) {
            return null;
        }
        
        int dx = ((angle < 45f && angle >= -45f) ? 1 : ((angle < -135f || angle >= 135f) ? -1 : 0)); // 오른쪽이면 1, 왼쪽이면 -1, 아니면 0
        int dy = ((angle < 135f && angle >= 45f) ? 1 : ((angle < -45f && angle >= -135f) ? -1 : 0)); // 위쪽이면 1, 아래쪽이면 -1, 아니면 0

        int x = firstJelly.GetPosX();
        int y = firstJelly.GetPosY();

        if (x + dx < 0 || x + dx >= gameBoard.GetWidth() || y + dy < 0 || y + dy >= gameBoard.GetHeight()) { // 게임 보드 범위 밖이면
            return null;
        }

        return gameBoard.GetJellyMap()[firstJelly.GetPosX() + dx, firstJelly.GetPosY() + dy];
    }

    IEnumerator CheckMatchRoutine() {
        yield return new WaitForSeconds(moveDuration * 2);
        matchChecker.CheckAllMatches();
        matchChecker.CheckSpecialMatches(firstJelly, secondJelly);
        
        if (matchChecker.GetJellyLists().Count > 0) { // 매치되는 젤리가 존재하면
            gameBoard.StartRemoveJelliesRoutine();
        } else {
            SwapJellies();
            yield return new WaitForSeconds(moveDuration);
            GameManager.instance.SetIsReady(true);
            GameManager.instance.CheckGameOver();
        }

        ResetJellies(); // 리셋을 안해주면 마지막에 선택한 Jelly를 기준으로 허공에 드래그를 해도 이동이 됨
    }

    private void SwapJellies() {
        firstJellyPos = firstJelly.transform.position;
        secondJellyPos = secondJelly.transform.position;

        firstJelly.transform.DOMove(secondJellyPos, moveDuration); // DOTween Package
        secondJelly.transform.DOMove(firstJellyPos, moveDuration);
        
        int tempX = firstJelly.GetPosX();
        int tempY = firstJelly.GetPosY();

        firstJelly.SetPos(secondJelly.GetPosX(), secondJelly.GetPosY()); // Object가 갖고 있는 x, y 변수 값 Swap
        secondJelly.SetPos(tempX, tempY);

        gameBoard.SetJellyMap(firstJelly.GetPosX(), firstJelly.GetPosY(), firstJelly); // jellyMap에서의 Jelly 값 Swap
        gameBoard.SetJellyMap(secondJelly.GetPosX(), secondJelly.GetPosY(), secondJelly);
    }

    private void ResetJellies() {
        firstJelly = null;
        secondJelly = null;
    }
}
