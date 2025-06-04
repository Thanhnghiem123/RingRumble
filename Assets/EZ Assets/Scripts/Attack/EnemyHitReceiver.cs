using UnityEngine;
using System.Collections;
using Ilumisoft.HealthSystem;
using Ilumisoft.HealthSystem.UI;

public class EnemyHitReceiver : HitReceiver
{

    protected override void Awake()
    {
        base.Awake();
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
                    //attackerrAnim?.PlayVictory();

                    // Tắt toàn bộ script (MonoBehaviour) trên GameObject của attacker ngoai tru scri
                    //if (attackerrAnim != null)
                    //{
                    //    MonoBehaviour[] scripts = attackerrAnim.gameObject.GetComponents<MonoBehaviour>();
                    //    foreach (var script in scripts)
                    //    {
                    //        if (script != null && script != this) // Không tắt chính EnemyHitReceiver nếu cần
                    //            script.enabled = false;
                    //    }
                    //}
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
                            // Khi player win hoặc defeat
                            GameManager.Instance.LoadSceneAfterDelay("SampleScene", 5f);

                        }

                    }


                    Debug.Log("ISALIVEssssss: " + IsAlive());
                    return;
                }
                else
                {
                    receiverAnim?.PlayHit(hitType);
                }
            }
        }
    }


}




