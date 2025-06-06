using UnityEngine;
using System.Collections;
using Ilumisoft.HealthSystem;
using Ilumisoft.HealthSystem.UI;

public class EnemyHitReceiver : HitReceiver
{

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }



    protected override void ReceiveHit(HitType hitType, float dame, Transform receiver = null)
    {
        if (receiver != null && knockbackConfig != null)
        {
            // Lấy animationManager và Rigidbody từ attacker
            AnimationManager receiverAnim = receiver.GetComponent<AnimationManager>();
            AnimationManager attackerrAnim = GetComponent<AnimationManager>();
            Rigidbody receiverRb = receiver.GetComponent<Rigidbody>();

            

            Debug.Log("ISALIVE: " + IsAlive());
            

            // Áp dụng knockback lên attacker
            if (receiverRb != null)
            {
                Debug.Log($"EnemyHitReceiver: Received hit of type {hitType} from {receiver.name}");
                Debug.Log($"EnemyHitReceiver: Transform position: {transform.position}, Attacker position: {receiver.position}");
                Vector3 knockbackDir = (receiver.position - transform.position).normalized;
                receiverRb.AddForce(knockbackDir * knockbackConfig.GetKnockbackForce(hitType), ForceMode.Impulse);
                Debug.Log($"EnemyHitReceiver: Applying damage {dame} to health: {health.name} :  {dame}");
                health.ApplyDamage(dame);

                if (IsAlive() == false)
                {
                    // receiver = player
                    // attacker = enemy
                    receiverAnim?.PlayDefeat();

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
                        if (!IsAlive())
                        {
                            GameManager.Instance.RemovePlayer(receiverAnim.gameObject);
                             //... các logic chết khác
                        }

                        if (receiver.CompareTag("Player"))
                        {
                            attackerrAnim?.PlayVictory();

                            // Disable all GameObjects in the scene with tag "Canvas"
                            GameObject[] allCanvasObjects = GameObject.FindGameObjectsWithTag("Canvas");

                            foreach (GameObject go in allCanvasObjects)
                            {
                                Debug.Log("Disabling Canvas: " + go.name);
                                go.SetActive(false);
                                for (int i = 0; i < go.transform.childCount; i++)
                                {
                                    go.transform.GetChild(i).gameObject.SetActive(false);
                                }
                            }
                            PopupEnd.Instance.ShowDefeatPopup();

                        }

                    }


                    Debug.Log("ISALIVEssssss: " + IsAlive());
                    return;
                }
                else
                {
                    float hitTime = (float)(receiverAnim?.PlayHit(hitType));
                    // Debug hittime va hit type
                    Debug.Log($"EnemyHitReceiver: Hit time for animation: {hitTime}, Hit type: {hitType}");
                    AttackManager.SetNormalStateFalse(hitTime, receiver.GetComponent<Animator>());

                }
            }
        }
    }


}




