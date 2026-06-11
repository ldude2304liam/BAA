using UnityEngine;

public class NewPlayerMovement : MonoBehaviour
{
    [Header("Speed Attributes")]
    public static float speed = 1f;
    public static float speedBoost = 1f;
    private bool isCharging ;
    [Header("direction Attributes")]
    private bool left ;
    private bool right ;
    private float angle = 1f;
    [SerializeField] private GameObject waytogo;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
            angle = angle + 0.4f;
            transform.position = Vector2.MoveTowards(transform.position ,waytogo.transform.position, speed * Time.deltaTime);
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
            angle = angle - 0.4f;
            transform.position = Vector2.MoveTowards(transform.position ,waytogo.transform.position, speed * Time.deltaTime);
        }

        

        
        
    }
    void FixedUpdate()
    {
        if (isCharging == true && speed < 5f)
            {
                speed = speed +0.1f;

            }
            
        Debug.Log (speed);

    }
}
