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
        //transform.position += transform.up * (10 + 2) * Time.deltaTime;
        transform.Rotate (30f, 360f, 30f);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")&& NewPlayerMovement.speed >= NewPlayerMovement.boost1Threshold)
        {
            Destroy(gameObject);
        }
        if (col.gameObject.CompareTag("Obstacle"))
        {
            transform.eulerAngles = new Vector3(100f, 50f, -200f);
        }
    }
    
}
