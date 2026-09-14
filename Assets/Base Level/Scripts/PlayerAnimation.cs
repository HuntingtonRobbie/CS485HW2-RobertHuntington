using UnityEngine;
using UnityEngine.InputSystem;

// Attach to: the Player root, next to PlayerMove. Drag the Zombie child (the object with the Animator) into 'animator'.
// Function: drive the pack's FreeZombieController from the same "Move" action PlayerMove reads. PlayerMove is untouched.
//   Controller parameters used:
//     MoveSpeed (float)   -1 = walk backwards, 0 = idle, 1 = walk forward  (the controller's blend tree)
//     Attack    (trigger) bite animation — fired by CollectableManager on a HIT via Bite(), optional
public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;                 // the Zombie child's Animator
    public float walkAnimationSpeed = 2f;     // playback multiplier while moving. The pack tuned its walk for ~2 m/s;
    // PlayerMove runs 4, so 2x keeps the feet from sliding. 1 = untouched.

    private InputAction moveAction;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        if (animator == null) Debug.LogError("Drag the Zombie's Animator into PlayerAnimation on the Player");
    }

    void Update()
    {
        if (animator == null) return;
        Vector2 move = moveAction.ReadValue<Vector2>();

        // W/S give the signed value the blend tree expects; A/D alone still shows the forward walk
        float moveSpeed = Mathf.Abs(move.y) > 0.01f ? move.y : Mathf.Abs(move.x);

        animator.SetFloat("MoveSpeed", moveSpeed, 0.1f, Time.deltaTime);   // 0.1 s damping = no snap between idle and walk
        animator.speed = (moveSpeed == 0f) ? 1f : walkAnimationSpeed;
    }

    // Optional: called by CollectableManager when a burger is hit
    public void Bite()
    {
        if (animator != null) animator.SetTrigger("Attack");
    }
}