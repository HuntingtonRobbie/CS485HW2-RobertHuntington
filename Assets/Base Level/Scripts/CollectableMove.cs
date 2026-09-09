using UnityEngine;
using System.Collections;
 public class CollectableMove : MonoBehaviour
{
    public float moveSpeed = 2.0f; // m/s along X
    public float leftLimitX = -1.05f;
    public float rightLimitX = 1.05f;
    public int startDirection = 1; //pos1 for right neg1 for left

    public float rotationSpeed = 60f; //degrees per second
    public Vector3 rotationAxis = Vector3.up;

    private int direction;

    void Start()
    {
        direction = (startDirection >= 0) ? 1 : -1;

        Vector3 startPos = transform.position;
        startPos.x = Mathf.Clamp(startPos.x, leftLimitX, rightLimitX);
        transform.position = startPos;
    }

    void Update()
    {
        // Copy PosX out and modify then assign back 
        Vector3 pos = transform.position;
        pos.x += direction * moveSpeed * Time.deltaTime;

        if (pos.x >= rightLimitX) { pos.x = rightLimitX; direction = -1; }
        else if (pos.x <= leftLimitX) { pos.x = leftLimitX; direction = 1; }

        transform.position = pos;

        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }

    // Shows the travel range in Scene view when selected.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 a = new Vector3(leftLimitX, transform.position.y, transform.position.z);
        Vector3 b = new Vector3(rightLimitX, transform.position.y, transform.position.z);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawWireSphere(a, 0.1f);
        Gizmos.DrawWireSphere(b, 0.1f);
    }
}
