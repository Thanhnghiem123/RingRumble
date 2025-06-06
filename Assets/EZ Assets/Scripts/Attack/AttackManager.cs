﻿using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{

    private static AttackManager instance;
    private Animator animator;

    [Tooltip("Trạng thái bình thường của Player")]
    public static bool PNormalState = true;
    [Tooltip("Trạng thái bình thường của Enemy")]
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


    /// <summary>
    /// Set NormalState thành false trong một khoảng thời gian nhất định.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="targetAnimator"></param>
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
