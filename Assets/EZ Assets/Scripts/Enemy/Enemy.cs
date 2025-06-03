using Assets.EZ_Assets.Scripts.GameManager;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Quản lý trạng thái và animation của enemy.
/// </summary>
public class Enemy : MonoBehaviour
{
    private IAnimationManager animationManager;
    private EnemyMovement enemyMovement;
    private Transform player;
    private NavMeshAgent agent;

    private Transform targetPlayer;

    void Start()
    {
        animationManager = GetComponent<IAnimationManager>();
        enemyMovement = GetComponent<EnemyMovement>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (enemyMovement == null || animationManager == null)
            return;
        targetPlayer = FindNearestPlayer();
        enemyMovement.SetDestination(player.position);
        if (enemyMovement.IsMoving)
        {
            animationManager.PlayRun(enemyMovement.IsMoving);
            if (!enemyMovement.IsMoving)
                animationManager.PlayIdle(); 
        }
    }



    Transform FindNearestPlayer()
    {
        float minDist = float.MaxValue;
        Transform nearest = null;
        foreach (var player in GameManager.Instance.players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = player.transform;
            }
        }
        return nearest;
    }
}
