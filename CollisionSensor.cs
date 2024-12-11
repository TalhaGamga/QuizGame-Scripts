using System;
using UnityEngine;

public class CollisionSensor : MonoBehaviour
{
    public Action<Collider> OnCollision { get; set; }

    private void OnTriggerEnter(Collider collision)
    {
        OnCollision?.Invoke(collision);
    }
}
