using System.Collections;
using UnityEngine;

public enum JellyType {
        Yellow = 0,
        Green = 1,
        Red = 2,
        Blue = 3,
        Pink = 4,
        Black = 5,
        Special = 6
};

public class Jelly : MonoBehaviour
{
    [SerializeField] private JellyType type;
    [SerializeField] private int x;
    [SerializeField] private int y;

    private int blickCount = 2;
    private float blinkDuration = 0.2f;

    public void SetPos(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public int GetPosX() {
        return x;
    }

    public int GetPosY() {
        return y;
    }

    public JellyType GetJellyType() {
        return type;
    }

    public void Remove() {
        StartCoroutine(BlinkRoutine());
        if (type != JellyType.Special) {
            GameManager.instance.DecreaseRemainJelly((int)type);
        }
        Destroy(gameObject, blickCount * 2 * blinkDuration);
    }

    IEnumerator BlinkRoutine() {
        Renderer renderer = GetComponent<Renderer>();
        for (int i = 0; i < blickCount; i++) {
            renderer.enabled = false;
            yield return new WaitForSeconds(blinkDuration);
            renderer.enabled = true;
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}
