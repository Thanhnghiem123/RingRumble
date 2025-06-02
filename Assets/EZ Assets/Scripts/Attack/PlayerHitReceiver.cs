using UnityEngine;
using System.Collections;

public class PlayerHitReceiver : HitReceiver
{
    private GameObject gameobjPlayer;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        gameobjPlayer = GameObject.FindGameObjectWithTag("Player");
        player = gameobjPlayer?.GetComponent<Player>();
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
                Debug.Log($"PlayerHitReceiver: Received hit of type {hitType} from {attacker.name}");
                Debug.Log($"PlayerHitReceiver: Transform position: {transform.position}, Attacker position: {attacker.position}");
                Vector3 knockbackDir = (attacker.position - transform.position).normalized;
                attackerRb.AddForce(knockbackDir * knockbackConfig.GetKnockbackForce(hitType), ForceMode.Impulse);

                // Thêm logic giảm máu cho Player
            }
        }
    }
}