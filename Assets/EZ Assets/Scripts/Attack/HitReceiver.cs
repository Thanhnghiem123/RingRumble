using UnityEngine;
using System.Collections;

public class HitReceiver : MonoBehaviour
{
    private IAnimationManager animationManager;
    private Rigidbody rb;
    private GameObject player;
    private KnockbackConfig knockbackConfig;


    private void Awake()
    {
        animationManager = GetComponent<IAnimationManager>();
        if (animationManager == null)
            Debug.LogError("IAnimationManager not found on " + gameObject.name);
        rb = GetComponent<Rigidbody>();
        if (animationManager == null)
            Debug.LogError("IAnimationManager not found on " + gameObject.name);
        if (rb == null)
            Debug.LogError("Rigidbody not found on " + gameObject.name);
        player = GameObject.FindGameObjectWithTag("Player");
        knockbackConfig = GetComponent<KnockbackConfig>();

    }


    public void ReceiveHit(HitType hitType, Transform attacker = null)
    {
        animationManager?.PlayHit(hitType);

        if (rb != null && attacker != null)
        {
            Debug.Log("Hit received: " + hitType + " from " + attacker.name);
            Vector3 knockbackDir = (attacker.position - player.transform.position).normalized;

            Debug.Log("transform.position: " + player.transform.position + ", attacker.position: " + attacker.position + ", knockbackDir: " + knockbackDir);
            rb.AddForce(knockbackDir * knockbackConfig.GetKnockbackForce(hitType), ForceMode.Impulse);
        }
    }


    public void ReceiveHoldPunch()
    {
        animationManager?.PlayHoldPunch();
    }

    public void ReceiveKnockout()
    {
        animationManager?.PlayKnockedOut();
    }


    public void DelayedHit(HitType hitType, float delay, float attackRange, LayerMask enemyLayer)
    {
        StartCoroutine(DelayedHitDetection(hitType, delay, attackRange, enemyLayer));
    }
    public IEnumerator DelayedHitDetection(HitType hitType, float delay, float attackRange, LayerMask enemyLayer)
    {
        yield return new WaitForSeconds(delay);

        
        Vector3 attackPos = transform.position + transform.forward * attackRange * 0.5f;
        Collider[] hits = Physics.OverlapSphere(attackPos, attackRange, enemyLayer);

        foreach (var hit in hits)
        {
            Debug.Log("Hit detected: " + hit.name + " with hitType: " + hitType);
            var hitReceiver = hit.GetComponent<HitReceiver>();
            if (hitReceiver != null)
            {
                hitReceiver.ReceiveHit(hitType, hit.transform);
            }
        }
    }
}
