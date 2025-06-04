using UnityEngine;
using System.Collections;
using Ilumisoft.HealthSystem;

[RequireComponent(typeof(Rigidbody))]
public abstract class HitReceiver : MonoBehaviour
{
    protected IAnimationManager animationManager;
    protected Rigidbody rb;
    protected KnockbackConfig knockbackConfig;
    protected float gizmoCapsuleHeight;
    protected float gizmoCapsuleRadius;
    protected Color gizmoColor = new Color(1f, 0f, 0f, 0.5f);
    protected Health health;
    protected HealthEnemy healthEnemy;

    protected virtual void Awake()
    {
        animationManager = GetComponent<IAnimationManager>();
        rb = GetComponent<Rigidbody>();
        knockbackConfig = GetComponent<KnockbackConfig>();
        
    }

    public virtual void ReceiveHoldPunch()
    {
        animationManager?.PlayHoldPunch();
    }

    public virtual void ReceiveKnockout()
    {
        animationManager?.PlayKnockedOut();
    }

    public virtual void ReceiveHit(HitType hitType, float delay, float capsuleHeight, float capsuleRadius, float dame, LayerMask enemyLayer)
    {
        StartCoroutine(DelayedHitDetection(hitType, delay, capsuleHeight, capsuleRadius, dame, enemyLayer));
        gizmoCapsuleHeight = capsuleHeight;
        gizmoCapsuleRadius = capsuleRadius;
    }

    public IEnumerator DelayedHitDetection(HitType hitType, float delay, float capsuleHeight, float capsuleRadius,float dame, LayerMask enemyLayer)
    {
        yield return new WaitForSeconds(delay);

        // Kiểm tra xem enemyLayer có chứa layer "Player" hay không
        int playerLayer = LayerMask.NameToLayer("Player");
        Collider[] hits = DetectColliders(capsuleHeight, capsuleRadius, enemyLayer);
        foreach (var hit in hits)
        {
            Debug.Log($"PlayerHitReceiver: Detected hit on {hit.name} with type {hitType}");
            if (((1 << playerLayer) & enemyLayer) != 0)
            {
                var hitReceiver = hit.GetComponent<PlayerHitReceiver>();
                if (hitReceiver != null)
                {
                Debug.Log($"PlayerHitReceivername; {hitReceiver.gameObject.name}");

                    health = hitReceiver.gameObject.GetComponent<Health>();
                    ReceiveHit(hitType, dame, hit.transform);
                }
            }
            else
            {
                var hitReceiver = hit.GetComponent<EnemyHitReceiver>();
                if (hitReceiver != null)
                {
                    healthEnemy = hitReceiver.gameObject.GetComponent<HealthEnemy>();
                Debug.Log($"EnemyHitReceivername; {hitReceiver.gameObject.name}");

                    ReceiveHit(hitType, dame, hit.transform);

                }

            }

        }
    }

    protected virtual void ReceiveHit(HitType hitType, float dame, Transform attacker = null)
    {
        
    }


    public bool IsAlive()
    {
        if (health != null)
        {
            Debug.Log($"IsAlive called on {gameObject.name}. Health: {health.CurrentHealth} IsAlive: {health.IsAlive}");
            return health.IsAlive;
        }
        else if (healthEnemy != null)
        {
            Debug.Log($"IsAlive called on {gameObject.name}. HealthEnemy: {healthEnemy.CurrentHealth} IsAlive: {healthEnemy.IsAlive}");
            return healthEnemy.IsAlive;
        }
        else
        {
            Debug.Log($"IsAlive called on {gameObject.name}. No health or healthEnemy component found.");
            return true;
        }
    }


    

    public virtual Collider[] DetectColliders(float capsuleHeight, float capsuleRadius, LayerMask enemyLayer)
    {
        Vector3 center = transform.position + transform.forward * capsuleRadius;
        Vector3 point1 = center + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 point2 = center - Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        return Physics.OverlapCapsule(point1, point2, capsuleRadius, enemyLayer);
    }


    ///
    ///
    ///
    ///
    ///
    ///
    ///
    ///



    protected virtual void OnDrawGizmosSelected()
    {
        DrawCapsuleGizmo();
    }

    protected virtual void DrawCapsuleGizmo()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = gizmoColor;
        Vector3 center = transform.position + transform.forward * gizmoCapsuleRadius;
        Vector3 point1 = center + Vector3.up * (gizmoCapsuleHeight * 0.5f - gizmoCapsuleRadius);
        Vector3 point2 = center - Vector3.up * (gizmoCapsuleHeight * 0.5f - gizmoCapsuleRadius);
        DrawWireCapsule(point1, point2, gizmoCapsuleRadius);
        Gizmos.color = oldColor;
    }

    protected virtual void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
    {
        Gizmos.DrawWireSphere(p1, radius);
        Gizmos.DrawWireSphere(p2, radius);
        Vector3 up = (p2 - p1).normalized;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized;
        Gizmos.DrawLine(p1 + right * radius, p2 + right * radius);
        Gizmos.DrawLine(p1 - right * radius, p2 - right * radius);
        forward = Vector3.Cross(up, right).normalized;
        Gizmos.DrawLine(p1 + forward * radius, p2 + forward * radius);
        Gizmos.DrawLine(p1 - forward * radius, p2 - forward * radius);
        Gizmos.DrawMesh(CreateCapsuleMesh(p1, p2, radius), 0, Vector3.zero, Quaternion.identity);
    }

    protected virtual Mesh CreateCapsuleMesh(Vector3 p1, Vector3 p2, float radius)
    {
        Mesh mesh = new Mesh();
        int segments = 12;
        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[segments * 6];
        float angle = 0;
        float angleStep = 2 * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);
            vertices[i] = p1 + new Vector3(x, 0, z) * radius;
            vertices[i + segments] = p2 + new Vector3(x, 0, z) * radius;
            angle += angleStep;
        }
        for (int i = 0; i < segments; i++)
        {
            int nextI = (i + 1) % segments;
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = nextI;
            triangles[i * 6 + 2] = i + segments;
            triangles[i * 6 + 3] = nextI;
            triangles[i * 6 + 4] = nextI + segments;
            triangles[i * 6 + 5] = i + segments;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
