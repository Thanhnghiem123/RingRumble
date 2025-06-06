using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private static AttackManager instance;
    private Animator animator;

    public static bool PNormalState = true;
    public static bool ENormalState = true;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        animator.SetBool("NormalState", true);

    }


    public static void SetNormalStateFalse(float duration, Animator targetAnimator)
    {
        Debug.Log($"SetNormalStateFalse called with duration: {duration}, targetAnimator: {targetAnimator?.name}");
        if (instance != null)
            instance.StartCoroutine(NormalStateCoroutine(duration, targetAnimator));
    }

    private static IEnumerator NormalStateCoroutine(float duration, Animator targetAnimator)
    {
        if (targetAnimator.gameObject.CompareTag("Player"))
        {
            PNormalState = false;
            if (targetAnimator != null)
                targetAnimator.SetBool("NormalState", false);
            yield return new WaitForSeconds(duration);

            PNormalState = true;
            if (targetAnimator != null)
                targetAnimator.SetBool("NormalState", true);
        }

        else
        {
            ENormalState = false;
            if (targetAnimator != null)
                targetAnimator.SetBool("NormalState", false);
            yield return new WaitForSeconds(duration);
            ENormalState = true;
            if (targetAnimator != null)
                targetAnimator.SetBool("NormalState", true);
        }
        

        
    }

}
