using UnityEngine;
using System.Collections.Generic;
using System;
public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    //[SerializeField] private float speed = 3f;
    [SerializeField] private int startDirection = 1;
    private float halfwidth;
    Vector2 lastVelocity;
    public Vector2 startForce;
    private bool right = true;
    private bool left;
    private float leftCheck = 0f;
    public Vector2 startForceBack;
    private int currentDirection;
    private NewPlayerMovement player;

    void Start()
    {
        player = FindFirstObjectByType<NewPlayerMovement>();
        // halfwidth = sr.bounds.extents.x;
         currentDirection = startDirection;
         
    }

    void Update()
    {
        Debug.Log(left);
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
     private void FixedUpdate()
     {
    //     rb.linearVelocity = Vector2.right * speed * currentDirection;
    //     rb.linearVelocity = Vector2.down * speed * currentDirection;
         SetDirection();
     }
     private void SetDirection()
     {
        //  if (Physics2D.Raycast(transform.position , Vector2.right , halfwidth + 0.1f , LayerMask.GetMask("Obstacle")))
        //  {
        //     right = false ;
        //     left = true;
        //  }
        //  else if (Physics2D.Raycast(transform.position , Vector2.left , halfwidth + 0.1f , LayerMask.GetMask("Obstacle"))&& startForceBack.x < 0)
        //  {
        //     right = false ;
        //     left = true;
        //  }

     }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && NewPlayerMovement.speed >= player.boost1Threshold)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/PIGDEAD");
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