using UnityEngine;

public class detectenemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.tag == "Player") 
        {
            PlayerMovement.speedMult = PlayerMovement.speedMult + 2f;
            Debug.Log("yeh");
        }
    }

}
    

