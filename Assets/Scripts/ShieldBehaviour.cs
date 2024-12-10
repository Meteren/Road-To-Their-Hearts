using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<FireBall>(out FireBall fireBall))
        {
            Vector2 oppositeDirection = new Vector2(-1 * fireBall.Direction.x,fireBall.Direction.y);
            Vector2 closestPoint = collision.ClosestPoint(fireBall.transform.position);
            ObjectPooling.EnqueuePool<FireBall>("FireBall", fireBall);
            HitParticle hitParticle = ObjectPooling.DequeuePool<HitParticle>("HitParticle");
            hitParticle.Init(oppositeDirection,closestPoint);
        }
    }
}
