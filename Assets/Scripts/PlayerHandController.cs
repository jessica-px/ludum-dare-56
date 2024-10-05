using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = transform.GetComponentInChildren<Animator>();
    }

    public void PlayGrabAnimation(bool isSuccess)
    {
        animator.SetBool("GrabSuccess", isSuccess);
        animator.Play("HandAnim_Strike");
    }

}
