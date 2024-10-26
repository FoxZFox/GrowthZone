using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public void PlayAttackAnimation(Action callback = null)
    {
        animator.SetTrigger("Attack");
        StartCoroutine(CheckAnimationEnd("Attack", callback));
    }
    public void PlayHurtAnimation(Action callback = null)
    {
        animator.SetTrigger("Hurt");
        StartCoroutine(CheckAnimationEnd("Hurt", callback));
    }
    public void PlayDieAnimation(Action callback = null)
    {
        animator.SetTrigger("Die");
        StartCoroutine(CheckAnimationEnd("Die", callback));
    }

    public void ResetAnimation()
    {
        animator.SetTrigger("Idle");
    }
    private IEnumerator CheckAnimationEnd(string name, Action action = null)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        if (name != "Die")
            animator.SetTrigger("Idle");
        action?.Invoke();
    }
}
