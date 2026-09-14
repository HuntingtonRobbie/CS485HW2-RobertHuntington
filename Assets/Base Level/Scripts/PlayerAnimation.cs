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
    
    public float turnSpeed = 720f; //degrees per second the model swings toward the input direction. 720 = half a turn in 0.25s.
    
    private PlayerMove playerMove; // read so the lean angle matches actual velocity, not raw input
    private InputAction moveAction;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        if (animator == null) Debug.LogError("Drag the Zombie's Animator into PlayerAnimation on the Player");
        playerMove = GetComponent<PlayerMove>();   
    }

    void Update()
    {
        if (animator == null) return;
        Vector2 move = moveAction.ReadValue<Vector2>();


        float moveSpeed = Mathf.Clamp01(move.magnitude);

        animator.SetFloat("MoveSpeed", moveSpeed, 0.1f, Time.deltaTime);   // UNCHANGED
        animator.speed = (moveSpeed < 0.01f) ? 1f : walkAnimationSpeed;    // EDITED: was (moveSpeed == 0f)

        // yaw the model toward where it is actually going.
        // Weighting by the two speeds means a W+D diagonal leans by the true motion angle,
        // not a flat 45 degrees.
        Vector2 vel = (playerMove != null)
            ? new Vector2(move.x * playerMove.sidewaySpeed, move.y * playerMove.forwardSpeed)
            : move;

        float targetYaw = (moveSpeed > 0.01f) ? Mathf.Atan2(vel.x, vel.y) * Mathf.Rad2Deg : 0f;

        animator.transform.localRotation = Quaternion.RotateTowards(
            animator.transform.localRotation,
            Quaternion.Euler(0f, targetYaw, 0f),
            turnSpeed * Time.deltaTime);
    }

    // Optional: called by CollectableManager when a burger is hit
    public void Bite()
    {
        if (animator != null) animator.SetTrigger("Attack");
    }
}