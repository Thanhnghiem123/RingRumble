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


    /// <summary>
    /// Xử lý nhận hit từ một đối tượng khác và xử lý các sự kiện liên quan
    /// </summary>
    /// <param name="hitType"></param>
    /// <param name="dame"></param>
    /// <param name="receiver"></param>
    protected override void ReceiveHit(HitType hitType, float dame, Transform receiver = null)
    {
        if (receiver != null && knockbackConfig != null)
        {
            AnimationManager receiverAnim = receiver.GetComponent<AnimationManager>();
            AnimationManager attackerrAnim = GetComponent<AnimationManager>();
            Rigidbody receiverRb = receiver.GetComponent<Rigidbody>();

            

            Debug.Log("ISALIVE: " + IsAlive());
            

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
                        }

                        if (receiver.CompareTag("Player"))
                        {
                            attackerrAnim?.PlayVictory();

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
                    Debug.Log($"EnemyHitReceiver: Hit time for animation: {hitTime}, Hit type: {hitType}");
                    AttackManager.SetNormalStateFalse(hitTime, receiver.GetComponent<Animator>());

                }
            }
        }
    }


}




