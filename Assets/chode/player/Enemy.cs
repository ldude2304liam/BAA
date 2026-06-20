using UnityEngine;

public class Enemy : MonoBehaviour
{
    private NewPlayerMovement player;

    void Start()
    {
        player = FindFirstObjectByType<NewPlayerMovement>();
    }

    void Update()
    {
        transform.Rotate(30f, 360f, 30f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && NewPlayerMovement.speed >= player.boost1Threshold)
        {
            Destroy(gameObject);
        }
        if (col.gameObject.CompareTag("Obstacle"))
        {
            transform.eulerAngles = new Vector3(100f, 50f, -200f);
        }
    }
}