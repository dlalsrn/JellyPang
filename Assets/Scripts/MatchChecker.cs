using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    private List<Jelly> jellyList = new List<Jelly>();
    GameBoard gameBoard;
    Jelly[,] jellyMap;
    private int width;
    private int height;
    private bool useSpecialJelly;

    private void Start() {
        useSpecialJelly = false;
        gameBoard = FindObjectOfType<GameBoard>();
        width = gameBoard.GetWidth();
        height = gameBoard.GetHeight();
    }
    public void CheckAllMatches() {
        jellyList.Clear();

        jellyMap = gameBoard.GetJellyMap();

        for (int x = 0; x < width - 2; x++) { // 가로 확인
            for (int y = 0; y < height; y++) {
                if (jellyMap[x, y].GetJellyType() == jellyMap[x + 1, y].GetJellyType() // 가로로 3개의 젤리가 match될 경우
                && jellyMap[x + 1, y].GetJellyType() == jellyMap[x + 2, y].GetJellyType()) {
                    jellyList.Add(jellyMap[x, y]);
                    jellyList.Add(jellyMap[x + 1, y]);
                    jellyList.Add(jellyMap[x + 2, y]);
                }
            }
        }

        for (int x = 0; x < width; x++) { // 세로 확인
            for (int y = 0; y < height - 2; y++) {
                if (jellyMap[x, y].GetJellyType() == jellyMap[x, y + 1].GetJellyType() // 세로로 3개의 젤리가 match될 경우
                && jellyMap[x, y + 1].GetJellyType() == jellyMap[x, y + 2].GetJellyType()) {
                    jellyList.Add(jellyMap[x, y]);
                    jellyList.Add(jellyMap[x, y + 1]);
                    jellyList.Add(jellyMap[x, y + 2]);
                }
            }
        }

        if (jellyList.Count > 0) {
            jellyList = jellyList.Distinct().ToList();
        }
    }

    public void CheckSpecialMatches(Jelly firstJelly, Jelly secondJelly) { // 만약 내가 움직인 젤리가 스페셜 젤리라면
        jellyMap = gameBoard.GetJellyMap();

        if (firstJelly.GetJellyType() == JellyType.Special && secondJelly.GetJellyType() == JellyType.Special) {
            useSpecialJelly = true;
            for (int x = 0; x < width; x++) { // 두 젤리 모두 스페셜 젤리면 모두 제거
                for (int y = 0; y < height; y++) {
                    jellyList.Add(jellyMap[x, y]);
                }
            }
            if (jellyList.Count > 0) {
                jellyList = jellyList.Distinct().ToList();
            }
        } else if (firstJelly.GetJellyType() == JellyType.Special) {
            useSpecialJelly = true;
            jellyList.Add(firstJelly);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (jellyMap[x, y].GetJellyType() == secondJelly.GetJellyType()) { // secondJelly와 같은 젤리 타입이면 모두 제거
                        jellyList.Add(jellyMap[x, y]);
                    }
                }
            }
            if (jellyList.Count > 0) {
                jellyList = jellyList.Distinct().ToList();
            }
        }
    }

    public List<Jelly> GetJellyLists() {
        return jellyList;
    }

    public void SetUseSpecialJelly(bool use) {
        useSpecialJelly = use;
    }

    public bool GetUseSpecialJelly() {
        return useSpecialJelly;
    }
}
