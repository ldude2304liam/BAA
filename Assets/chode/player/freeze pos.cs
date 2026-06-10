using System.Collections;
using UnityEngine;

public class freezepos : MonoBehaviour
{

    private bool isTurn;
    private bool slowDown;
    private float angle = 1f;
    private float speed = 5f;
    private float speedMult = 1f;
    [SerializeField] private GameObject waytogo;
    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
         if(isTurn != true)
         {
            transform.position += transform.up*speed*speedMult*Time.deltaTime;
            transform.position = waytogo.transform.position;
            
         }
         if (Input.GetKeyDown(KeyCode.Space))
         {
            
            
            //transform.eulerAngles = new Vector3 (0, 0, angle);
            slowDown = true;
            

             //isTurn = true;
             //rb.linearVelocity = new Vector3(0, 0, 0);
         }
        if (Input.GetKeyUp(KeyCode.Space))
         {
            //float accel = 100f; // tune this
           //StartCoroutine(SpeedBoost(speedMult));
             transform.position = waytogo.transform.position;
            
           


            
            //float newX = Mathf.MoveTowards(rb.linearVelocity.x, speed, accel * Time.fixedDeltaTime);
             isTurn = false;
             speed = 5f;
             slowDown = false;
             //rb.linearVelocity = new Vector3(0, 5, 0);
         }
         if(isTurn == true)
        {
           
            angle = angle +0.4f;
            transform.eulerAngles = new Vector3 (0, 0, angle);
        }
        // if(isTurn == false)
        // {
           
            
        // }
        if(speed == 1f)
        {
            isTurn = true;
        }
        //Debug.Log (speed);
  


    }
    void FixedUpdate()
    {

        if(slowDown == true )
        {
            if(speed > 1f)
            {
                speed = speed - 0.5f;
            }
           
            

           
        }
        
        
    }

        // }

        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     Debug.Log("jump");
        //    rb.linearVelocity = new Vector3(0, 10, 0);
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     Debug.Log("jump");
        //    rb.linearVelocity = new Vector3(10, 0, 0);
        // }
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     Debug.Log("jump");
        //    rb.linearVelocity = new Vector3(-10, 0, 0);
        // }
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     Debug.Log("jump");
        //    rb.linearVelocity = new Vector3(0, -10, 0);
        // }
}

