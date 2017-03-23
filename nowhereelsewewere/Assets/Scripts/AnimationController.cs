using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CharacterType characterType;
    // Used to determine which animations to pick from.
    private enum CharacterType
    {
        Skye,
        Wrather,
        Barbarian,
        Knight,
        Archer,
        Mage,
    }

    [SerializeField]
    private AnimationClip walkingAnim;

    [SerializeField]
    private AnimationClip attackAnim;
	[SerializeField]
	private float attackAnimSpeed = 1.0f;

	[SerializeField]
    private AnimationClip dieAnim;
	[SerializeField]
	private float dieAnimSpeed = 1.0f;

	[SerializeField]
    private AnimationClip specialAttackAnim;
	[SerializeField]
	private float specialAttackAnimSpeed = 1.0f;

	[SerializeField]
    private AnimationClip dodgeAnim;
	[SerializeField]
	private float dodgeAnimSpeed = 1.0f;

	[SerializeField]
    private AnimationClip blockAnim;
	[SerializeField]
	private float blockAnimSpeed = 1.0f;

	[SerializeField]
    private AnimationClip idlingAnim;
	[SerializeField]
	private float idlingAnimSpeed = 1.0f;

	[SerializeField]
    private AnimationClip heavyAttackAnim;
	[SerializeField]
	private float heavyAttackAnimSpeed = 1.0f;

	private volatile bool actionAnimating = false;

	private AnimState animState;
    public enum AnimState
    {
        Walking,
        Attack,
        Die,
        SpecialAttack,
        Dodge,
        Block,
        Idling,
        HeavyAttack
    }

    void Start()
    {
        // Choose a type of character which changes animation types.
        animator.SetInteger("CharacterType", (int)characterType);
        PlayAnimation(AnimState.Idling);
    }

	void Update()
	{
		if(!actionAnimating)
		{
			animator.speed = idlingAnimSpeed;
		}
	}

    public float GetAnimationLength(AnimState state)
    {
        float time = -1;
        if (state == AnimState.Idling && idlingAnim != null)
        {
            time = idlingAnim.length / idlingAnimSpeed;
        }
        else if (state == AnimState.Walking && walkingAnim != null)
        {
			time = walkingAnim.length;
        }
        else if (state == AnimState.Attack && attackAnim != null)
        {
			time = attackAnim.length / attackAnimSpeed;
        }
        else if (state == AnimState.Die && dieAnim != null)
        {
            time = dieAnim.length / dieAnimSpeed;
        }
        else if (state == AnimState.HeavyAttack && heavyAttackAnim != null)
        {
            time = heavyAttackAnim.length / heavyAttackAnimSpeed;
        }
        else if (state == AnimState.SpecialAttack && specialAttackAnim != null)
        {
            time = specialAttackAnim.length / specialAttackAnimSpeed;
        }
        else if (state == AnimState.Block && blockAnim != null)
        {
            time = blockAnim.length / blockAnimSpeed;
        }
        else if (state == AnimState.Dodge && dodgeAnim != null)
        {
            time = dodgeAnim.length / dodgeAnimSpeed;
        }
        return time;
    }
    
    public void PlayAnimation(AnimState state)
    {
        animState = state;
        if (animator != null)
        {
            if(animState == AnimState.Idling)
            {
                PlayIdleAnimation();
            }
            else if(animState == AnimState.Walking)
            {
                PlayWalkingAnimation();
            }
            else if (animState == AnimState.Attack)
            {
                PlayAttackAnimation();
				StartCoroutine(ResetAnimatorSpeed(GetAnimationLength(animState)));
			}
            else if (animState == AnimState.Die)
            {
                PlayDeathAnimation();
				StartCoroutine(ResetAnimatorSpeed(GetAnimationLength(animState)));
			}
            else if (animState == AnimState.HeavyAttack)
            {
                PlayHeavyAttackAnimation();
				StartCoroutine(ResetAnimatorSpeed(GetAnimationLength(animState)));
			}
            else if (animState == AnimState.SpecialAttack)
            {
                PlaySpecialAttackAnimation();
				StartCoroutine(ResetAnimatorSpeed(GetAnimationLength(animState)));
			}
            else if (animState == AnimState.Block)
            {
                PlayBlockAnimation();
				StartCoroutine(ResetAnimatorSpeed(GetAnimationLength(animState)));
			}
            else if (animState == AnimState.Dodge)
            {
                PlayDodgeAnimation();
				StartCoroutine(ResetAnimatorSpeed(GetAnimationLength(animState)));
			}
        }
        else
        {
            Debug.Log("No animator attached to public field");
        }
    }

    private void PlayIdleAnimation()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdling", true);
    }

    private void PlayWalkingAnimation()
    {
        animator.SetBool("isIdling", false);
        animator.SetBool("isWalking", true);
    }

    private void PlayBlockAnimation()
    {
        animator.SetTrigger("Block");
		SetAnimatorSpeed(blockAnimSpeed);
    }

    private void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
		SetAnimatorSpeed(attackAnimSpeed);
    }

    private void PlayHeavyAttackAnimation()
    {
        animator.SetTrigger("HeavyAttack");
		SetAnimatorSpeed(heavyAttackAnimSpeed);
    }

    private void PlaySpecialAttackAnimation()
    {
        animator.SetTrigger("SpecialAttack");
		SetAnimatorSpeed(specialAttackAnimSpeed);
    }

    private void PlayDeathAnimation()
    {
        animator.SetTrigger("Die");
		SetAnimatorSpeed(dieAnimSpeed);
    }

    private void PlayDodgeAnimation()
    {
        animator.SetTrigger("Dodge");
		SetAnimatorSpeed(dodgeAnimSpeed);
    }

	private void SetAnimatorSpeed(float newSpeed)
	{
		actionAnimating = true;
		animator.speed = newSpeed;
	}

	private IEnumerator ResetAnimatorSpeed(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		actionAnimating = false;
	}
}
