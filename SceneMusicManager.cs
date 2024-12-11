using UnityEngine;

public class SceneMusicManager : MonoBehaviour
{
    [SerializeField] string[] stopMusic;
    [SerializeField] string startMusic;

    void Start()
    {
        foreach (var stopMusic in stopMusic)
        {
            AudioManager.Instance.Stop(stopMusic);
        }

        AudioManager.Instance.Play(startMusic);
    }
}
