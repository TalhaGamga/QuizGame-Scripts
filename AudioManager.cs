using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] Sound[] sounds;

    private void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    public void Play(string name)
    {
        if (Instance == null)
        {
            return;
        }

        Sound sound = Array.Find(sounds, sound => sound.name == name);

        sound.source.volume = UnityEngine.Random.Range(sound.volumeLowest, sound.volumeHighest);
        sound.source.pitch = UnityEngine.Random.Range(sound.pitchLowest, sound.pitchHighest);

        sound.source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        sound.source.Stop();
    }
}
