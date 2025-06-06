using UnityEngine;

public class PlayerAttack : MonoBehaviour, IPlayerAttack
{
    [Header("Attack Settings")]
    public float capsuleHeight = 2f;
    public float capsuleRadius = 0.35f;

    [Header("Attack Delay Settings")]
    public float punchDelay = 0.2f;
    public float kickDelay = 0.25f;
    public float holdPunchDelay = 0.9f;
    public float holdKickDelay = 0.35f;

    [Header("Attack Damage Settings")]
    public float damePunch = 10f;
    public float dameKick = 15f;
    public float dameHoldPunch = 20f;
    public float dameHoldKick = 25f;
    public float kickRange = 0.65f;

    [SerializeField]
    private bool isBlocking = false;


    public LayerMask enemyLayer;
    private PlayerHitReceiver hitReceiver;

    public float DamePunch { get => damePunch; set => damePunch = value; }
    public float DameHoldPunch { get => dameHoldPunch; set => dameHoldPunch = value; }
    public float DameKick { get => dameKick; set => dameKick = value; }
    public float DameHoldKick { get => dameHoldKick; set => dameHoldKick = value; }
    public float AttackCooldown { get; set; } = 0.5f; 

    void Start()
    {
        hitReceiver = GetComponent<PlayerHitReceiver>();
        if (hitReceiver == null)
        {
            Debug.LogError("HitReceiver component not found on " + gameObject.name);
        }
    }


    public bool IsBlocking() => isBlocking;

    
    public bool Block()
    {
        isBlocking = true;
        return isBlocking;
    }

    public bool UnBlock()
    {
        return isBlocking = false;
    }

    public void Punch(HitType hitType)
    {
        hitReceiver.ReceiveHit(hitType, punchDelay, capsuleHeight, capsuleRadius, damePunch, enemyLayer);

    }


    public void HoldPunch(HitType hitType)
    {
        hitReceiver.ReceiveHit(hitType, holdPunchDelay, capsuleHeight, capsuleRadius, dameHoldPunch, enemyLayer);
    }

    public void Kick(HitType hitType)
    {
        hitReceiver.ReceiveHit(hitType, kickDelay, capsuleHeight, capsuleRadius, dameKick, enemyLayer);
    }

    public void HoldKick(HitType hitType)
    {
        hitReceiver.ReceiveHit(hitType, holdKickDelay, capsuleHeight, capsuleRadius * kickRange, dameHoldKick, enemyLayer);
    }






}