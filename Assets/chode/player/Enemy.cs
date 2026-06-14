using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<NewPlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")&& NewPlayerMovement.speed >= NewPlayerMovement.boost1Threshold)
        {
            Destroy(gameObject);
        }
    }
    
}
