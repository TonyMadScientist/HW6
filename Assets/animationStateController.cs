using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    private bool hasWon = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Do not return to walking after winning.
        if (hasWon)
        {
            return;
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        bool isMoving =
            horizontalInput != 0 ||
            verticalInput != 0;

        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isIdle", !isMoving);
    }

    public void PlayWinAnimation()
    {
        if (animator == null || hasWon)
        {
            return;
        }

        hasWon = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", false);
        animator.SetBool("Win", true);
    }
}