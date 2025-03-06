using UnityEngine;
public class GoalkeeperX : MonoBehaviour
{
    public float speed = 15;
    private Rigidbody goalkeeperRb;
    private GameObject currentTarget;
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
        // Finding nearest enemy every frame
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = enemy;
            }
        }

        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            goalkeeperRb.AddForce(direction * speed);
        }

        // Checking boundaries and correct position if needed
        Vector3 currentPos = transform.position;
        if (currentPos.x > maxX)
        {
            currentPos.x = maxX;
        }
        if (currentPos.x < minX)
        {
            currentPos.x = minX;
        }
        if (currentPos.z > maxZ)
        {
            currentPos.z = maxZ;
        }
        if (currentPos.z < minZ)
        {
            currentPos.z = minZ;
        }

        transform.position = currentPos;
    }
}