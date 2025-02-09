using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioClip jellyRemoveSound;
    [SerializeField] private AudioClip jellyDropSound;
    private AudioSource audioSource;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJellyRemoveSound() {
        audioSource.PlayOneShot(jellyRemoveSound);
    }

    public void PlayJellyDropSound() {
        audioSource.PlayOneShot(jellyDropSound);
    }
}
