using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagable
{
    [Header("Initials")]
    [SerializeField] private Transform initialPoint;
    [SerializeField] private Animator animator;

    [Header("Answering")]
    [SerializeField] private float jumpPower;
    [SerializeField] private int jumpCount;
    [SerializeField] private float jumpDuration;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Ease jumpEase;

    [Header("Firing")]
    [SerializeField] private Transform bulletPref;
    [SerializeField] private float bulletPower;
    [SerializeField] private int bulletJumpCount;
    [SerializeField] private float bulletDuration;
    [SerializeField] private Ease bulletEase;
    [SerializeField] private Transform firePoint;

    [Header("ParticleSystem")]
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private ParticleSystem waterBlowEffect;

    [SerializeField] private Transform initialLookAt;
    public Action<Transform> OnPlayerShoot;

    [SerializeField] private CollisionSensor collisionSensor;

    [SerializeField] private string playerTag;

    [SerializeField] PostProcessingEffector effector;
    private void OnEnable()
    {
        collisionSensor.OnCollision += TakeCollision;
    }

    private void OnDisable()
    {
        collisionSensor.OnCollision -= TakeCollision;
    }


    public void JumpInto(Transform targetPoint)
    {
        modelTransform.DOJump(
        targetPoint.position,   // Target position to jump to
        jumpPower,         // Jump height
        jumpCount,          // Number of jumps
        jumpDuration           // Duration of the tween
        ).SetEase(jumpEase);

        animator.SetTrigger("Jump");
        AudioManager.Instance.Play("Jump");
    }

    public void Shoot(Transform targetPoint)
    {
        StartCoroutine(ShootCoroutine(targetPoint));
    }

    private IEnumerator ShootCoroutine(Transform targetPoint)
    {
        firePoint.gameObject.SetActive(true);

        yield return new WaitForSeconds(jumpDuration);
        AudioManager.Instance.Play("Shoot_" + playerTag);

        // Instantiate and position the bullet
        Transform bullet = Instantiate(bulletPref);
        fireEffect?.Play();
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.LookAt(targetPoint.position);

        // Calculate the direction and target position
        Vector3 direction = (targetPoint.position - bullet.transform.position).normalized;
        Vector3 offset = direction * 3f + Vector3.up;
        Vector3 fireTo = targetPoint.position + offset;

        // Move the bullet with DOTween
        bullet.DOMove(
            fireTo,               // Target position
            bulletDuration        // Duration of the tween
        ).SetEase(bulletEase).OnComplete(() => Destroy(bullet.gameObject));

        // Trigger the shoot animation
        animator.SetTrigger("Shoot");

        OnPlayerShoot?.Invoke(targetPoint);

        firePoint.gameObject.SetActive(false);
    }

    public void LookAt(Transform point)
    {
        modelTransform.LookAt(point.position);
    }

    public void Reset()
    {
        modelTransform.DOMove(initialPoint.position, 0.5f).OnComplete(() => modelTransform.LookAt(initialLookAt.position));
        animator.SetTrigger("Idle");
    }

    public void TakeCollision(Collider collider)
    {
        if (collider.CompareTag("Bullet"))
        {
            Vector3 hitDirection = (collider.gameObject.transform.position - modelTransform.position).normalized;
            modelTransform.DOMove(modelTransform.position - hitDirection * 10, 0.25f);
            effector.TriggerHitEffect();
            animator.SetTrigger("Die");
            AudioManager.Instance.Play("Die");
        }

        if (collider.CompareTag("Water"))
        {
            waterBlowEffect.transform.position = modelTransform.position;
            waterBlowEffect.Play();
            AudioManager.Instance.Play("Waterblow");
        }
    }
}