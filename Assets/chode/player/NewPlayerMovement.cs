using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewPlayerMovement : MonoBehaviour
{
    [Header("Speed Attributes")]
    public static float speed = 2.5f;
    public static float speedBoost = 1f;
    public static bool slowDown;
    public static float chargeSpeed = 1f;
    private bool isCharging ;
    [Header("direction Attributes")]
    private bool left ;
    private bool right ;
    private float angle = 1f;
    [SerializeField] private GameObject waytogo;
    [Header("Art Attributes")]
    [SerializeField] ParticleSystem particlesystem;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position ,waytogo.transform.position, speed * Time.deltaTime);
        isCharging = true;
        if (isCharging == true)
        {
            transform.position += transform.up*speed*speedBoost*Time.deltaTime;

        }

        if (Input.GetKeyDown(KeyCode.A))
        {
           
          
            left = true;
 
        }
        if (Input.GetKeyUp(KeyCode.A))
        {

            left = false;
  
        }
        if (left == true)
        {
            transform.eulerAngles = new Vector3 (0, 0, angle);
            angle = angle + 0.6f;
            //angle = Mathf.MoveTowards(angle, 0.8f, 20f * Time.fixedDeltaTime);
            //transform.position = Vector2.MoveTowards(transform.position ,waytogo.transform.position, speed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {    
            right = true;     
        }
        if (Input.GetKeyUp(KeyCode.D))
        { 
            right = false;       
        }
        if (right == true)
        {
            transform.eulerAngles = new Vector3 (0, 0, angle);
            angle = angle - 0.6f;
            //angle = Mathf.MoveTowards(angle, -0.8f, 20f * Time.fixedDeltaTime);
            //transform.position = Vector2.MoveTowards(transform.position ,waytogo.transform.position, speed * Time.deltaTime);
        }
        if(speed >= 7.5f)
        {
            StartCoroutine(Boost());
        }
         if (Input.GetKeyDown(KeyCode.Space))
        {
            slowDown = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
           
            slowDown = false;
            speed = speed + chargeSpeed;
           
        }
        if(slowDown == true)
        {
            if(chargeSpeed <15f)
            {
              chargeSpeed = chargeSpeed + 1f;  
            }
            
            if(speed >= 2)
            {
                speed = speed -0.05f;
            }
            
            
        }
        if(slowDown == false)
        {
            speed = Mathf.MoveTowards(speed, 7.5f, 0.5f * Time.fixedDeltaTime);
       
           
            
            
            
        }

            


        

        
        
    }
    void FixedUpdate()
    {
        if (isCharging == true && speed < 7.5f && slowDown == false )
            {
                speed = Mathf.MoveTowards(speed, 7.5f, 100f * Time.fixedDeltaTime);

            }
            
        Debug.Log (speed);
        

    }
    IEnumerator Boost()
    {
        
        yield return new WaitForSeconds(2f);
        speedBoost = 2.3f;
        particlesystem.Play();
        yield break;
    }
}
