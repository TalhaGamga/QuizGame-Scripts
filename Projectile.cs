using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ParticleSystem collisionEffect;
    private void OnTriggerEnter(Collider collision)
    {
        collisionEffect?.Play();
    }
}
