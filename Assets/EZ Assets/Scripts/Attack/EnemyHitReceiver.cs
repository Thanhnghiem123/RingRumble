using UnityEngine;
using System.Collections;

public class EnemyHitReceiver : HitReceiver
{
    protected override void Awake()
    {
        base.Awake();
    }

  

    protected override void ReceiveHit(HitType hitType, Transform attacker = null)
    {
        if (attacker != null && knockbackConfig != null)
        {
            // Lấy animationManager và Rigidbody từ attacker
            AnimationManager attackerAnim = attacker.GetComponent<AnimationManager>();
            Rigidbody attackerRb = attacker.GetComponent<Rigidbody>();

            // Phát animation trên attacker
            attackerAnim?.PlayHit(hitType);

            // Áp dụng knockback lên attacker
            if (attackerRb != null)
            {
                Debug.Log($"EnemyHitReceiver: Received hit of type {hitType} from {attacker.name}");
                Debug.Log($"EnemyHitReceiver: Transform position: {transform.position}, Attacker position: {attacker.position}");
                Vector3 knockbackDir = (attacker.position - transform.position ).normalized;
                attackerRb.AddForce(knockbackDir * knockbackConfig.GetKnockbackForce(hitType), ForceMode.Impulse);
            }
        }
    }
}