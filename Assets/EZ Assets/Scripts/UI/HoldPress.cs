using UnityEngine;
using UnityEngine.EventSystems;

public class HoldPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum AttackType { Punch, Kick, Jump, Block }
    public AttackType attackType;
    public float holdTime = 1.0f;
    private float pointerDownTimer = 0f;
    private bool isPointerDown = false;
    public bool hasTriggeredAttack = false; // Đã tung chiêu đặc biệt chưa

    public Player playerAttack; // Kéo thả vào Inspector

    void Update()
    {
        if (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (!hasTriggeredAttack && pointerDownTimer >= holdTime)
            {
                TriggerAttack(true); // true = hold
                hasTriggeredAttack = true;
                Reset();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0f;
        hasTriggeredAttack = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TriggerAttack(false);
        // Nếu chưa từng hold thành công thì mới cho phép đánh thường (tap)
        if (!hasTriggeredAttack)
        {
            TriggerAttack(false); // false = tap
        }
        Reset();
    }

    private void Reset()
    {
        isPointerDown = false;
        pointerDownTimer = 0f;
    }

    // isHold: true nếu là hold, false nếu là tap
    private void TriggerAttack(bool isHold)
    {
        if (playerAttack == null) return;
        if (!AttackManager.PNormalState) return;
        switch (attackType)
        {
            case AttackType.Punch:
                if (isHold)
                    playerAttack.HoldPunch();
                else
                    playerAttack.Punch();
                break;


            case AttackType.Kick:
                if (isHold)
                    playerAttack.HoldKick(); 
                else
                    playerAttack.Kick();
                break;


            case AttackType.Jump:
                if (isHold)
                    playerAttack.JumpOverIntro(); 
                else
                    playerAttack.Jump();
                break;



            case AttackType.Block:
                if (isHold)
                    playerAttack.Block();
                else if (isHold == false)
                    playerAttack.UnBlock(); 
                break;
        }
    }
}
