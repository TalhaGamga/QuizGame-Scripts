using UnityEngine;

[CreateAssetMenu(fileName = "SoundSO", menuName = "Scriptable Objects/SoundData")]
public class Sound : ScriptableObject
{
    public AudioClip clip;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float volumeLowest;
    [Range(0, 1f)]
    public float volumeHighest;

    [Header("Pitch")]
    [Range(0.1f, 3f)]
    public float pitchLowest;
    [Range(0.1f, 3f)]
    public float pitchHighest;

    [HideInInspector]
    public AudioSource source;
}