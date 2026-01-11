using UnityEngine;

public enum SoundType 
{ 
    MOVE,
    FRIGHTENED,
    DEAD,
    WIN,
    START
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static SoundManager instance;
    private AudioSource audioSrc;
     
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType type, float volume = 1f, float delay = 0f, bool loop = true)
    {
        //instance.audioSrc.PlayOneShot(instance.soundList[(int)type], volume);
        instance.audioSrc.clip = instance.soundList[(int)type];
        instance.audioSrc.loop = loop;
        instance.audioSrc.volume = volume;
        instance.audioSrc.PlayDelayed(delay);
    }

    public static void PauseSound()
    {
        instance.audioSrc.Pause();
    }
}
