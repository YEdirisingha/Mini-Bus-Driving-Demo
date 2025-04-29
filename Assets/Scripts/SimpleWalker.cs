using UnityEngine;

public class SimpleWalker : MonoBehaviour
{
    private Vector3 target;
    private bool walking = false;
    public float speed = 4f;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Idle"); // Play Idle at start
        }
    }
    public void WalkTo(Vector3 destination)
    {
        target = destination;
        walking = true;

        if (animator != null)
        {
            animator.Play("Run"); // Switch to Run animation when start moving
        }
    }

    private void Update()
    {
        if (walking)
        {
            Vector3 moveDirection = (target - transform.position).normalized;
            transform.position += new Vector3(moveDirection.x, 0f, moveDirection.z) * speed * Time.deltaTime;

            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.x, 0, target.z)) < 0.1f)
            {
                walking = false;
                Destroy(gameObject); // Destroy after reaching the bus or exit the bus
            }
        }
    }
}
