using UnityEngine;
using System.Collections;
using Ilumisoft.HealthSystem;
using Ilumisoft.HealthSystem.UI;

public class PlayerHitReceiver : HitReceiver
{

    protected override void Awake()
    {
        base.Awake();
       
    }

    

    protected override void ReceiveHit(HitType hitType, float dame,Transform receiver = null)
    {
        Debug.Log($"PlayerHitReceiver: ReceiveHit called with hitType: {hitType}, dame: {dame}, receiver: {receiver?.name}");
        Debug.Log($"PlayerHitReceiver: knockbackConfig : {knockbackConfig}");
        if (receiver != null && knockbackConfig != null)
        {
            // Lấy animationManager và Rigidbody từ attacker
            AnimationManager receiverAnim = receiver.GetComponent<AnimationManager>();
            AnimationManager attackerrAnim = GetComponent<AnimationManager>();
            Rigidbody receiverRb = receiver.GetComponent<Rigidbody>();
            Debug.Log($"PlayerHitReceiver: receiverAnim: {receiverAnim.gameObject.name}, attackerrAnim: {attackerrAnim.gameObject.name}, receiverRb: {receiverRb.gameObject.name}");




            // attacker = player
            // receiver = enemy
            receiverAnim?.PlayHit(hitType);
            Debug.Log($"PlayerHitReceiver: Playing hit animation for");
            Debug.Log("ISALIVE: " + IsAlive());
            
            // Áp dụng knockback lên attacker
            if (receiverRb != null)
            {
                Debug.Log($"EnemyHitReceiver: Received hit of type {hitType} from {receiver.name}");
                Debug.Log($"EnemyHitReceiver: Transform position: {transform.position}, Attacker position: {receiver.position}");
                Vector3 knockbackDir = (receiver.position - transform.position).normalized;
                receiverRb.AddForce(knockbackDir * knockbackConfig.GetKnockbackForce(hitType), ForceMode.Impulse);
                Debug.Log($"PlayerHitReceiver: Applying damage {dame} to healthEnemy: {healthEnemy.name} :  {dame}");
                healthEnemy.ApplyDamage(dame);

                if (IsAlive() == false)
                {
                    receiverAnim?.PlayDefeat();
                    //attackerrAnim?.PlayVictory();
                    if (receiverAnim != null)
                    {
                        MonoBehaviour[] scripts = receiverAnim.gameObject.GetComponents<MonoBehaviour>();
                        foreach (var script in scripts)
                        {
                            if (script != null && script != this && !(script is PlayerHealthbar))
                            {
                                script.enabled = false;
                            }
                        }

                        var Capsule = receiverAnim.GetComponent<CapsuleCollider>();
                        if (Capsule != null)
                            Capsule.enabled = false;

                        var rb = receiverAnim.GetComponent<Rigidbody>();
                        if (rb != null)
                            rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                    }
                    Debug.Log("ISALIVEssssss: " + IsAlive());

                    return;
                }
            }
        }
    }

   
}