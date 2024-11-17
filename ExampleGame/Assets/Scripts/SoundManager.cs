using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    //enum for sound actions
    public enum SoundAction
    {
        Flip,
        Backflip,
        Match,
        Mismatch,
        Bonus,
        Penalty,
        GameStart,
        Win
    }

    [SerializeField] private AudioClip flipSound;
    [SerializeField] private AudioClip backflipSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip mismatchSound;
    [SerializeField] private AudioClip bonusSound;
    [SerializeField] private AudioClip penaltySound;
    [SerializeField] private AudioClip gameStartSound;
    [SerializeField] private AudioClip winSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(SoundAction action)
    {
        switch (action)
        {
            case SoundAction.Flip:
                audioSource.PlayOneShot(flipSound);
                break;
            case SoundAction.Backflip:
                audioSource.PlayOneShot(backflipSound);
                break;
            case SoundAction.Match:
                audioSource.PlayOneShot(matchSound);
                break;
            case SoundAction.Bonus:
                audioSource.PlayOneShot(bonusSound);
                break;
            case SoundAction.Penalty:
                audioSource.PlayOneShot(penaltySound);
                break;
            case SoundAction.GameStart:
                audioSource.PlayOneShot(gameStartSound);
                break;
            case SoundAction.Win:
                audioSource.PlayOneShot(winSound);
                break;
        }
    }
}
