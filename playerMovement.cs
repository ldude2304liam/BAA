using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Unity.VisualScripting;
//using UnityEditor.Tilemaps;

public class PlayerMovement : MonoBehaviour
{


    Vector2 StartPosition;

    
    public float speed = 8f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.12f;
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    public LayerMask groundLayer;


    private bool SoundPlayed;

    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;

    public float fallSpeedMultiplier = 2f;

    private Rigidbody2D rb;
    private float horizInput;
    public static bool isGrounded;
    private SpriteRenderer spriteRenderer;
    AudioManager111 audioManager;
    
    private float jumpTimeCounter;

    public float jumpTime;

    private bool isJumping;

    private bool inBlade2;
    [SerializeField] private Animator _animator;
    [SerializeField] ParticleSystem particlesystem;
    Vector3 currentEulerAngles;



    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;


    void Awake()
    {
         audioManager = GameObject.FindGameObjectWithTag("audiomanager").GetComponent<AudioManager111>();
         StartPosition = transform.position;
    }

    void Start()
    {
        GetComponent<Barrel>();
        GetComponent<Barrel2>();
        SoundPlayed = false;
        

        if(GetComponent<Rigidbody2D>() != null)
        {
          rb = GetComponent<Rigidbody2D>();
          spriteRenderer = GetComponent<SpriteRenderer>();
          

        }

    }

    private void Gravity()
    {



        if(isJumping == true && maxFallSpeed < 2  )
        {
            cameraShake.Instance.StartShake(65f);
        }
        
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; //makes you fall faster
            rb.linearVelocity = new Vector2(rb.linearVelocity.x , Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else 
        {
            rb.gravityScale = baseGravity;
        }
        
    }

    void Update()
    {
       
        
        horizInput = Input.GetAxisRaw("Horizontal");
        
        if(Barrel.showAnim == false)
        {
            _animator.SetBool("isJumping", true);
        }
       
        
           
        

      
      
            
        

        
       
    
        Vector2 rayOrigin = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        if(Barrel.inBarrel == true)
        {
            SoundPlayed = false;
        }
        if(Barrel2.inBarrel2 == true)
        {
            SoundPlayed = false;
        }
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            currentEulerAngles += new Vector3(0, 0, 0);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
            Barrel.points = 100f;
            Barrel.LaunchForce = 100f;
            Barrel.SCORE = Barrel.SCORE - 1;
                    
            Barrel2.LaunchForce2 = 100f;
            

        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; //=- means subtract
        }

        if (isGrounded == true && Barrel.tummpoints > 3  && SoundPlayed == false )
        {
            audioManager.PlaySFX(audioManager.hithard);
            audioManager.PlaySFX(audioManager.boo);
            particlesystem.Play();

            StartCoroutine(ShakeFloor());
            StartCoroutine(tummy());
            SoundPlayed = true;

        }
        
        //if (isGrounded == true && Barrel.tummpoints > 3 && SoundPlayed == false)
       // {
       //     audioManager.PlaySFX(audioManager.boo);




       //     SoundPlayed = true;
            
  

      //  }

        if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0f )
        {
            _animator.SetBool("isJumping", true);
            


            
            isJumping = true;
            
            jumpTimeCounter = jumpTime;
          rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);



            coyoteTimeCounter = 0f;
        }
        else
        {
              _animator.SetBool("isJumping", false);
        }

        if (spriteRenderer != null)
        {
            if (horizInput > 0.1f) spriteRenderer.flipX = false;
            else if (horizInput < -0.1f) spriteRenderer.flipX = true;
        }
        if(horizInput != 0)
        {

            

           
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }


        if(Input.GetKey(KeyCode.Space))
        
        {
            
            if(jumpTimeCounter > 0  && isJumping == true )
            {
                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                 jumpTimeCounter -= Time.deltaTime;
                
            }
            else
            {
                isJumping = false;
            }
            
           
        }
        if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
            }
            

        Gravity();
    }

    IEnumerator RespawnTime()
    {
         rb.constraints = RigidbodyConstraints2D.FreezePosition;
        
        yield return new WaitForSeconds(0.3f);
        _animator.SetBool("isRespawn", false);
        transform.position = StartPosition;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        yield break;

    }


    public void Respawn()
    {
        _animator.SetBool("isRespawn", true);
        audioManager.PlaySFX(audioManager.hithard);
        audioManager.PlaySFX(audioManager.boo);
        //rb.constraints = RigidbodyConstraints2D.FreezePosition;
        StartCoroutine(RespawnTime());
        particlesystem.Play();
        
    }
    IEnumerator ShakeFloor()
    {
        cameraShake.Instance.StartShake(35f);
        yield return new WaitForSeconds(0.2f);
        cameraShake.Instance.StopShake();
        yield break;
    }


    IEnumerator tummy()
    {
        
        yield return new WaitForSeconds(2f);
        Barrel.tummpoints = 0f;
        yield break;
    }
   // private void OnTriggerEnter2D (Collider2D collision)
   // {

      // if( GetComponent<Rigidbody2D>() != null && collision.CompareTag("blade") )
      //  {
      //      GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            
            
           
  
            
                 
      //  }
        
       // if( GetComponent<Rigidbody2D>() != null && collision.CompareTag("blade")  )
       // {
       //     GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
           
  
            
                 
       // }
   // }


    //private void OnTriggerExit2D (Collider2D collision)
    //{

      //  if( GetComponent<Rigidbody2D>() != null && collision.CompareTag("blade") && GetComponent<Rigidbody2D>())
       // {
       //     GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;   
       // }
   // }
    void FixedUpdate()
    {
        float accel = 100f; // tune this


        float targetSpeed = horizInput * speed;
        if(isGrounded == true)
        {
           accel = 100f; // tune this

        }
        else
        {
            accel = 55f;
        }
      

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y); 
    }


    public void EnterBarrel(GameObject barrel)
    {
        

        rb.linearVelocity = new Vector2 (0,0);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY ;
        

        
        

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        transform.parent = barrel.transform;
       
        
    }
       public void EnterBarrel2(GameObject barrel2)
    {

        
        

        rb.linearVelocity = new Vector2 (0,0);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY ;
        

        
        

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        transform.parent = barrel2.transform;
       
        
    }
    public void LaunchFrom2(Vector2 launchDir2 , float launchForce2 )
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
   
        
        
        transform.parent = null;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(launchDir2 * launchForce2 );
        ;
        
        
        

    }
     public void LaunchFrom(Vector2 launchDir , float launchForce )
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
   
        
        
        transform.parent = null;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(launchDir * launchForce );
        ;
        
        
        

    }
    public void EnterBarrel3(GameObject blade3)
    {

        
        

        rb.linearVelocity = new Vector2 (0,0);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY ;
        

        
        

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        transform.parent = blade3.transform;
       
        
    }

        public void LaunchFrom3(Vector2 launchDir3 , float launchForce3 )
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
   
        
        
        transform.parent = null;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(launchDir3 * launchForce3 );
        ;
        
        
        

    }
    public void EnterBarrel4(GameObject blade4)
    {

        
        

        rb.linearVelocity = new Vector2 (0,0);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY ;
        

        
        

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        transform.parent = blade4.transform;
       
        
    }
    public void LaunchFrom4(Vector2 launchDir4 , float launchForce4 )
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
   
        
        
        transform.parent = null;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(launchDir4 * launchForce4 );
        ;
        
        
        

    }
    public void EnterBarrel5(GameObject blade5)
    {

        
        

        rb.linearVelocity = new Vector2 (0,0);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY ;
        

        
        

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        transform.parent = blade5.transform;
       
        
    }
    public void LaunchFrom5(Vector2 launchDir5 , float launchForce5 )
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
   
        
        
        transform.parent = null;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(launchDir5 * launchForce5 );
        ;
        
        
        

    }
    public void EnterBarrel6(GameObject blade6)
    {

        
        

        rb.linearVelocity = new Vector2 (0,0);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY ;
        

        
        

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        transform.parent = blade6.transform;
       
        
    }
        public void LaunchFrom6(Vector2 launchDir6 , float launchForce6 )
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
   
        
        
        transform.parent = null;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(launchDir6 * launchForce6 );
        ;
        
        
        

    }




        
        
        
        


    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + (Vector3)groundCheckOffset, transform.position + (Vector3)groundCheckOffset + Vector3.down * groundCheckDistance);
        }
    }
}