using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public class SwitchEnemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    //[SerializeField] private float speed = 3f;
    [SerializeField] private int startDirection = 1;

    public Color colorOn = Color.green;
    public Color colorOff = Color.red;
 
    Vector2 lastVelocity;
    public Vector2 startForce;
    private bool turnItOn;
    

    private bool hurtAble;
    

    
    private bool left;
    private float leftCheck = 0f;
    public Vector2 startForceBack;
    private int currentDirection;
    private NewPlayerMovement player;

    void Start()
    {
        player = FindFirstObjectByType<NewPlayerMovement>();
        
        InvokeRepeating(nameof(HitAble), 3.0f, 2f);
          
        InvokeRepeating(nameof(HitUnAble), 6f, 2f);

        


         
         
          
        // halfwidth = sr.bounds.extents.x;
         currentDirection = startDirection;

    }
    private IEnumerator Hitty()
    {
        

        yield return new WaitForSeconds(2f);
        turnItOn = true;
        
    }



    void Update()
    {
        
        StartCoroutine(Hitty());
        // InvokeRepeating(nameof(HitUnAble), 0.5f, 0.3f);
        Debug.Log (hurtAble);
        
        if(left != true)
        {
            rb.AddForce(startForce , ForceMode2D.Force);
        }
        if(left == true)
        {
            leftCheck = leftCheck + 1;
            rb.AddForce(startForceBack , ForceMode2D.Force);
        }
        lastVelocity = rb.linearVelocity;
        
        
    }
    void HitAble()
    {
        hurtAble = true;
        sr.color = colorOn;
    }
    void HitUnAble()
    {
        hurtAble = false;
        sr.color = colorOff;
    }
     private void FixedUpdate()
     {

        
      
     }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && NewPlayerMovement.speed >= player.boost1Threshold && hurtAble == true)
        {
            Destroy(gameObject);
        }
        var speed = lastVelocity.magnitude;
        var direction = Vector2.Reflect(lastVelocity.normalized , col.contacts[0].normal);
        rb.linearVelocity = direction * Mathf.Max(speed , 0f);
          if (col.gameObject.CompareTag("Obstacle") && leftCheck <= 0f)
          {
            left = true;
           
            
          }
        else if (col.gameObject.CompareTag("Obstacle") && leftCheck >= 0f)
          {
            left = false;
            leftCheck = 0;
           
            
          }
    }
}