using UnityEngine;
using System.Collections;

public class NewPlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    //Rigidbody2D rb; 

    [Header("UI")]


    [Header("Speed Settings")]
    [Tooltip("Acceleration while in Stage 0")]
    public float accelerationStage0 = 3.0f;
    [Tooltip("Acceleration while in Stage 1")]
    public float accelerationStage1 = 0.8f;
    [Tooltip("Acceleration while in Stage 2")]
    public float accelerationStage2 = 0.6f;

    [Tooltip("Triggers Boost 1")]


   //[Tooltip("triggers Boost 1")]
   public float boost1Threshold = 8.5f;

    [Tooltip("Triggers Boost 2")]
    public float boost2Threshold = 25f;

    [Tooltip("Speed cap (Stage 2 ceiling)")]
    public float maxSpeed = 40f;

    [Tooltip("Flat speed added when a boost fires")]
    public float boostSpeedBonus = 5.5f;


   /// <summary>
     [Header("bouncing")]

   /// </summary>
   [Tooltip("How many seconds after pressing Control the bounce is still valid")]
    public float bounceInputWindow = 0.15f;
    private float lastControlPressTime = -999f;
    //public float bounceSpeedBonus = 4f;
    private bool justBounced = false;

/// <summary>
/// ///////////////////////////////////////
/// </summary>
    [Header("Charge Settings")]
    public float maxChargeSpeed = 15f;

    [Tooltip("How fast the charge builds while Space is held")]
    public float chargeRate = 1f;

    [Tooltip("How much speed is drained per frame while charging")]
    public float chargeDrainRate = 0.05f;

    public float chargeMultiplierStage1 = 1.5f;
    public float chargeMultiplierStage2 = 2.5f;

    [Tooltip("How fast the charge burst fades after release")]
    public float burstDecay = 8f;

    [Header("breaking")]
    public float maxChargeHoldTime = 3f;
    public float chargeHoldTimer = 0f;
    public float overheatBrakeRate = 12f; // slow decrease

    



    [Header("Turning Settings")]
    public float turnRate = 50f;
    public float turnMultCharge = 10f;
    public float turnMult = 6f;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem boostParticles;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Default colour (Stage 0)")]
    public Color colorStage0 = Color.white;
    [Tooltip("Colour after first boost (Stage 1)")]
    public Color colorStage1 = Color.yellow;
    [Tooltip("Colour after second boost (Stage 2)")]
    public Color colorStage2 = Color.red;
    [Tooltip("Colour flash on hitting an enemy")]
    public Color colorEnemy = Color.blue;
    public float enemyFlashDuration = 0.2f;

    // currentStage: 0 = normal, 1 = after boost 1, 2 = after boost 2
    private int currentStage = 0;

    public static float speed = 1f;
    public float driftFactor = 0.95f;

    private float chargeSpeed = 0f;
    private float chargeBurst = 0f; // temporary overlay, does NOT affect base speed
    public bool isCharging = false;
    private bool boostPending = false; // coroutine only fires once per threshold

    private float angle = 0f;
    private float steeringInput = 0f;
    private Vector3 MoveForce; 

    // Speed floor for each stage (so demotion lands here, not at 0)
    private float[] stageFloor = { 1f, 25f, 30f };

    // ─────────────────────────────────────────
    //  TARGET SPEED per stage (what the auto-
    //  climb moves toward before the boost fires)
    // ─────────────────────────────────────────
    private float TargetSpeedForStage()
    {
        switch (currentStage)
        {
            case 0: return boost1Threshold;
            case 1: return boost2Threshold;
            default: return maxSpeed;
        }
    }

    private float AccelerationForStage()
    {
        switch (currentStage)
        {
            case 0: return accelerationStage0;
            case 1: return accelerationStage1;
            default: return accelerationStage2;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
       
        ApplyStageColor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        lastControlPressTime = Time.time;

        HandleChargeInput();
        CheckBoostTrigger();

        Debug.Log($"Stage: {currentStage} Speed: {speed:F2} charge: {chargeSpeed}");
    }

    void FixedUpdate()
    {
       
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            lastControlPressTime = Time.time;

        controls();
        MoveForward();
        killSideForce();

        if (!isCharging)
        {
            float target = TargetSpeedForStage();
            float accel = AccelerationForStage();
            speed = Mathf.MoveTowards(speed, target, accel * Time.fixedDeltaTime);
        }

        if (chargeBurst > 0f)
            chargeBurst = Mathf.MoveTowards(chargeBurst, 0f, burstDecay * Time.fixedDeltaTime);
    }



    void MoveForward()
    {
        //transform.position += transform.up * (speed + chargeBurst) * Time.deltaTime;
        rb.linearVelocity = (Vector2)transform.up * (speed + chargeBurst);
        
    }

    void controls()
    {
        steeringInput = Input.GetAxis("Horizontal");

        float steerInput = Input.GetAxis("Horizontal");
        Vector2 inputVector = Vector2.zero;

        
        


        // if (left)
        // {
        //     angle += turnRate;
        //     transform.eulerAngles = new Vector3(0f, 0f, angle);
        // }
        // if (right)      
        // {
        //     angle -= turnRate;
        //     transform.eulerAngles = new Vector3(0f, 0f, angle);
        // }
        if(isCharging == true)
        {
            float speedFactor = Mathf.Lerp(1f, 0.35f, speed / maxSpeed); // reduces turning as speed increases during charging
            angle -= steeringInput * turnRate * turnMultCharge * speedFactor;

        }
       
        else
        {
            angle -= steeringInput * turnRate * turnMult;

        }

        rb.MoveRotation(angle);
    }

    void killSideForce()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);
        rb.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
    }

    void HandleChargeInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            chargeSpeed = 0f;
        }

        if (isCharging)
        {
            chargeHoldTimer += Time.deltaTime;

            // Build up the charge
            if (chargeSpeed < maxChargeSpeed)
                chargeSpeed += chargeRate * Time.deltaTime * 60f;

            // Drain current speed while charging
            if (speed >= stageFloor[currentStage] + 0.1f)
                speed -= chargeDrainRate;

            if (speed >= stageFloor[1] + 0.1f)
                speed -= chargeDrainRate * 2f;


             if (chargeHoldTimer >= maxChargeHoldTime)
            {
                speed = Mathf.MoveTowards(speed, 0f, overheatBrakeRate * Time.deltaTime);
                chargeSpeed = 0f;  // loose charge 
                chargeBurst = 0f;  // and if any burst it cancels (not sure if i want to)
            }       

            // checks stage if speed drops below the threshold 
            if (currentStage == 2 && speed < boost2Threshold)
            {
                currentStage = 1;
                boostPending = false;
                ApplyStageColor();
            }
            if (currentStage == 1 && speed < boost1Threshold)
            {
                currentStage = 0;
                boostPending = false;
                ApplyStageColor();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;
            chargeHoldTimer = 0f;

            // Scale the charge bonus based on current stage
            float stageMultiplier = 1f;
            if (currentStage == 1) stageMultiplier = chargeMultiplierStage1;
            if (currentStage == 2) stageMultiplier = chargeMultiplierStage2;

            chargeBurst = chargeSpeed * stageMultiplier; // separate from speed

            if (chargeSpeed >= 15)
                speed = speed + 2;

            chargeSpeed = 0f;

            if (currentStage == 0)
                speed = speed + 5f;

            speed = Mathf.Min(speed + (chargeSpeed * stageMultiplier), maxSpeed);
            chargeSpeed = 0f;
        }
    }

    void CheckBoostTrigger()
    {
        if (boostPending) return;

        if (currentStage == 0 && speed >= boost1Threshold)
        {
            boostPending = true;
            StartCoroutine(TriggerBoost(1));
        }
        else if (currentStage == 1 && speed >= boost2Threshold)
        {
            boostPending = true;
            StartCoroutine(TriggerBoost(2));
        }
    }

    IEnumerator TriggerBoost(int newStage)
    {
        yield return new WaitForSeconds(0.5f); // dramatic pause

        currentStage = newStage;
        speed += boostSpeedBonus;
        boostPending = false;

        ApplyStageColor();

        if (boostParticles != null)
            boostParticles.Play();
    }

    public void TakeHit()
    {
      /*   if (currentStage == 0) return; // already at base, nothing to lose */

/*         currentStage = 0;
        speed = stageFloor[currentStage];
        boostPending = false;
        StopAllCoroutines(); */
        StopAllCoroutines();
        boostPending = false;

        currentStage = 0;
        speed = stageFloor[0];
        chargeBurst = 0f;   // kill any active burst too
        chargeSpeed = 0f;   // kill any building charge


        ApplyStageColor();

        if (boostParticles != null)
            boostParticles.Stop();

        Debug.Log($"hit {currentStage}");
    }

    public void HitGuy()
    {
        if (spriteRenderer == null) return;

        boostPending = true;

        StartCoroutine(TriggerEnemyBoost());
        StartCoroutine(EnemyColorFlash());
        StartCoroutine(GameFeelPause());
    }

    IEnumerator TriggerEnemyBoost()
    {
        yield return new WaitForSeconds(0.1f);
        speed += boostSpeedBonus;
        boostPending = false;
        if (boostParticles != null)
            boostParticles.Play();
    }

    IEnumerator GameFeelPause()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.02f);
        Time.timeScale = 1f;
    }

    IEnumerator EnemyColorFlash()
    {
        spriteRenderer.color = colorEnemy;
        yield return new WaitForSeconds(enemyFlashDuration);
        ApplyStageColor();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            bool withinWindow = (Time.time - lastControlPressTime) <= bounceInputWindow;

            if (withinWindow )
            {
                lastControlPressTime = -999f;
                Bounce(col);
            }
            else
                TakeHit();
        }
        if (col.gameObject.CompareTag("Enemy") && speed >= boost1Threshold)
            HitGuy();
    }

    // fires every  frame while still touching the wall
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Obstacle") && !justBounced )
        {
            speed = stageFloor[0];
            chargeBurst = 0f;
            chargeSpeed = 0f;

            if (currentStage != 0)
            {
                currentStage = 0;
                boostPending = false;
                StopAllCoroutines();
                ApplyStageColor();
            }
        }
    }

    void Bounce(Collision2D col)
    {
        //the direction pointing away from the wall
        Vector2 wallNormal = col.contacts[0].normal;

        //direction off that normal of the wall
        Vector2 currentDirection = transform.up;
        Vector2 reflectedDirection = Vector2.Reflect(transform.up, wallNormal);

        // convert the reflected direction back into an angle
        angle = Mathf.Atan2(reflectedDirection.x, reflectedDirection.y) * Mathf.Rad2Deg * -1f;
        rb.MoveRotation(angle);
        rb.linearVelocity = Vector2.zero;
        justBounced = true; 

            // Small speed bonus for pulling it off
         ///speed = Mathf.Min(speed + bounceSpeedBonus, maxSpeed);
        ///rb.linearVelocity = Vector2.zero;
        //transform.position += (Vector3)(wallNormal * 0.3f);



    }
    void OnCollisionExit2D(Collision2D col)
{
    if (col.gameObject.CompareTag("Obstacle"))
        justBounced = false; // clear flag once fully separated from wall
}

    void ApplyStageColor()
    {
        if (spriteRenderer == null) return;
        switch (currentStage)
        {
            case 0: spriteRenderer.color = colorStage0; break;
            case 1: spriteRenderer.color = colorStage1; break;
            case 2: spriteRenderer.color = colorStage2; break;
        }
    }
}