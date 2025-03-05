using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed=1;
private Rigidbody enemyRb;
private GameObject player;

void Start() {
enemyRb = GetComponent<Rigidbody>();
player = GameObject. Find("Player"); }

void Update() {
enemyRb. AddForce((player. transform.position- transform.position).normalized * speed); }
}
