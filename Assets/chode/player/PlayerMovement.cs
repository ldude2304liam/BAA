using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private bool isTurn;
    private bool slowDown;
    private float angle = 1f;
    public static float speed = 5f;
    public static float speedMult = 1f;
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
            
           


            
            //float newX = Mathf.MoveTowards(rb.linearVelocity.x, speed, accel * Time.fixedDeltaTime);
             isTurn = false;
             speed = 5f;
             slowDown = false;
             //rb.linearVelocity = new Vector3(0, 5, 0);
         }
         if(isTurn == true)
        {
            transform.position = Vector2.MoveTowards(transform.position , waytogo.transform.position , speed * Time.deltaTime);

            //rb.linearVelocity = new Vector3(0, speed, 0);
            
            angle = angle +0.4f;
            transform.eulerAngles = new Vector3 (0, 0, angle);
        }
        if(speed == 1f)
        {
            isTurn = true;
        }
        //Debug.Log (speed);
       // Debug.Log (speedMult);
  


    }
    void FixedUpdate()
    {
        if(isTurn == true)
        {  
            if(speedMult < 10f)
            {
            speedMult = speedMult +0.1f;
            }
           
        }
        if(isTurn == false)
        {
            if(speedMult > 1f)
            {
                 speedMult = speedMult -0.15f;
            }
           
           
        }
        if(slowDown == true )
        {
            if(speed > 1f)
            {
                speed = speed - 0.5f;
            }
           
            

           
        }
        
        
    }
    void StartBoost(float Mult)
        {
            StartCoroutine(SpeedBoost(Mult));
        }
        private IEnumerator SpeedBoost(float Mult)
        {
            speedMult = Mult;
            
            yield return new WaitForSeconds(0.4f);
            speedMult = 1f;

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

