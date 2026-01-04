using UnityEngine;

public enum SoundType 
{ 
    MOVE,
    FRIGHTENED,
    EATEN
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

    public static void PlaySound(SoundType type, float volume = 1f, float delay = 0f)
    {
        //instance.audioSrc.PlayOneShot(instance.soundList[(int)type], volume);
        instance.audioSrc.clip = instance.soundList[(int)type];
        instance.audioSrc.volume = volume;
        instance.audioSrc.PlayDelayed(delay);
    }
}
