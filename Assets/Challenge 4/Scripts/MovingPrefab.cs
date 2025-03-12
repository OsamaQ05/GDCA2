using UnityEngine;

public class MovingPrefab : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 savedVelocity;
    private bool wasKinematic;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Pause()
    {
        if (rb != null)
        {
            savedVelocity = rb.linearVelocity;
            wasKinematic = rb.isKinematic;
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

    public void Resume()
    {
        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
            rb.linearVelocity = savedVelocity;
        }
    }
}
