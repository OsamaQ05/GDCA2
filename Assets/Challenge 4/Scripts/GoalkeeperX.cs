using UnityEngine;

public class GoalkeeperX : MonoBehaviour
{
    public float speed = 15f;
    private Rigidbody goalkeeperRb;
    private GameObject currentTarget;
    public Vector3 centerPosition = new Vector3(0f, 30f, 40f); // Set your desired center position
    private float maxX = 20f;
    private float minX = -20f;
    private float maxZ = -33f;
    private float minZ = -48f;

    void Start()
    {
        goalkeeperRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        FindEnemyInRange();
        
        if (currentTarget != null)
        {
            MoveTowards(currentTarget.transform.position);
        }
        else
        {
            MoveTowards(centerPosition);
        }

        ClampPosition();
    }

    void FindEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        currentTarget = null;

        foreach (GameObject enemy in enemies)
        {
            if (IsWithinRange(enemy.transform.position))
            {
                currentTarget = enemy;
                break; // Stops at the first enemy found in range
            }
        }
    }

    bool IsWithinRange(Vector3 position)
    {
        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }

    void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        goalkeeperRb.AddForce(direction * speed);
    }

    void ClampPosition()
    {
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.z = Mathf.Clamp(currentPos.z, minZ, maxZ);
        transform.position = currentPos;
    }
}
