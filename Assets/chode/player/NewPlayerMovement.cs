using UnityEngine;
using System.Collections;

public class NewPlayerMovement : MonoBehaviour
{
    //  SPEED SETTINGS
    //
    [Header("Speed Settings")]
    [Tooltip("How fast speed naturally climbs toward the next threshold")]
    public float acceleration = 0.6f;

    [Tooltip("triggers Boost 1")]
    public static float boost1Threshold = 8.5f;

    [Tooltip("triggers Boost 2")]
    public float boost2Threshold = 25f;

    [Tooltip("speed cap (Stage 2 ceiling)")]
    public float maxSpeed = 40f;

    [Tooltip("flat speed added when a boost fires")]
    public float boostSpeedBonus = 5.5f;
    /// <summary>
    /// ////// charge
    /// </summary>
    [Header("charge Settings")]
    public float maxChargeSpeed = 15f;

    [Tooltip("How fast the charge builds while Space is held")]
    public float chargeRate = 1f;

    [Tooltip("how much speed is taken per frame while charging")]
    public float chargeDrainRate = 0.05f;

    public float chargeMultiplierStage1 = 1.5f;
    public float chargeMultiplierStage2 = 2.5f;


    //  TURNING a
    [Header("Turning Settings")]
    public float turnRate = 0.6f;

/// <summary>
/// /feedback
/// </summary>
    [Header("feedback")]
    [SerializeField] private ParticleSystem boostParticles;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Default colour (Stage 0)")]
    public Color colorStage0 = Color.white;

    [Tooltip("Colour first boost (Stage 1)")]
    public Color colorStage1 = Color.yellow;

    [Tooltip("Colour after second boost (Stage 2)")]
    public Color colorStage2 = Color.red;

    [Tooltip("Colour after you hit enemy")]
    public Color colorEnemy = Color.blue;

    public float enemyFlashDuration = 0.2f;

   
    // currentStage: 0 = normal, 1 = after boost 1, 2 = after boost 2
    private int currentStage = 0;

    public static float speed = 1f;
    private float chargeSpeed = 0f;
    //private float chargeSpeed2 = 100f;
    private bool isCharging = false;
    private bool boostPending = false; //coroutine only fires once per threshold ( does not repeat every frame)

    private float angle = 0f;
    private bool left = false;
    private bool right = false;

    // Speed floor for each stage (so demotion lands here, not at 0)
    private float[] stageFloor = { 1f, 25f, 30f }; // array to store the minimum speed of each level

    // ─────────────────────────────────────────
    //  TARGET SPEED per stage (what the auto-
    //  climb moves toward before the boost fires)
    // ─────────────────────────────────────────
    private float TargetSpeedForStage()
    {
        switch (currentStage) //switches when it gets hit the threshold
        {
            case 0: return boost1Threshold;
            case 1: return boost2Threshold;
            default: return maxSpeed;
        }
    }

    void Start()
    {
        ApplyStageColor();
    }

    void Update()
    {
        controls();
        HandleChargeInput();
        MoveForward();
        CheckBoostTrigger();

        // Debug
        Debug.Log($"Stage: {currentStage} Speed: {speed:F2} charge: {chargeSpeed}");
    }

    void FixedUpdate()
    {
        // Auto-climb speed toward the current stage's ceiling (when not charging)
        if (!isCharging)
        {
            float target = TargetSpeedForStage();
            speed = Mathf.MoveTowards(speed, target, acceleration * Time.fixedDeltaTime);
        }
    }

    void MoveForward()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }
    /// <summary>
    /// /////////////////
    /// </summary>
    void controls()
    {
        if (Input.GetKeyDown(KeyCode.A)) left = true;
        if (Input.GetKeyUp(KeyCode.A))  left = false;
        if (Input.GetKeyDown(KeyCode.D)) right = true;
        if (Input.GetKeyUp(KeyCode.D))  right = false;

        if (left)
        {
            angle += turnRate;
            transform.eulerAngles = new Vector3(0f, 0f, angle);
        }
        if (right)
        {
            angle -= turnRate;
            transform.eulerAngles = new Vector3(0f, 0f, angle);
        }
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
            
            // Build up the charge
            if (chargeSpeed < maxChargeSpeed)
                chargeSpeed += chargeRate * Time.deltaTime * 60f; 

  /*           // Drain current speed while charging 
            if (speed >= stageFloor[currentStage] + 0.1f)
                speed -= chargeDrainRate;
            //if (speed >= stageFloor[0] + 0.1f && speed <= stageFloor[1] +0.1f )
               // speed -= chargeDrainRate * 2 ;
            if (speed >= stageFloor[1] + 0.1f )
                speed -= chargeDrainRate * 2f ; */
                            // Drain speed while charging — exclusive per stage so they never stack
            float drain = chargeDrainRate;          
            if (currentStage == 1) drain *= 3f;     
            if (currentStage == 2) drain *= 3f;   
 
            if (speed >= stageFloor[currentStage] + 0.2f)
                speed -= drain;
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;
            // Scale based on current stage
            float stageMultiplier = 1f;
            if (currentStage == 1) stageMultiplier = chargeMultiplierStage1;
            if (currentStage == 2) stageMultiplier = chargeMultiplierStage2;

            speed = Mathf.Min(speed + (chargeSpeed * stageMultiplier), maxSpeed);
            chargeSpeed = 0f;
        }
    }

    /// <summary>
    ///  BOOST TRIGGER
    /// </summary>
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
        yield return new WaitForSeconds(0.5f); //dramatic pause

        currentStage = newStage;
     
        
        
        speed += boostSpeedBonus;    
        
        //speed += boostSpeedBonus;              // jump forward
        boostPending = false;

        ApplyStageColor();

        if (boostParticles != null)
            boostParticles.Play();
    }

///I need a cooldown or something that makes you feel more the deceleration
    public void TakeHit()
    {
        if (currentStage == 0) return; // already at base, nothing to lose
 
        currentStage = 0;
        speed = stageFloor[currentStage]; // drop to floor of previous stage
        boostPending = false;             // reset boost guard
        StopAllCoroutines();
 
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
        yield return new WaitForSeconds(0.1f); //dramatic pause
        speed += boostSpeedBonus;              // jump forward
        boostPending = false;
        if (boostParticles != null)
        {
           boostParticles.Play(); 
        }          
    }
    IEnumerator GameFeelPause()
    {
        Time.timeScale = 0.1f;
        
        yield return new WaitForSeconds(0.02f);
        Time.timeScale = 1f;
    }
 
    IEnumerator EnemyColorFlash()
    {
        spriteRenderer.color = colorEnemy;         // flash whatever
        yield return new WaitForSeconds(enemyFlashDuration); // wait
        ApplyStageColor();                         // back tot he stage color
    }

    void OnCollisionEnter2D(Collision2D col)
     {
        if (col.gameObject.CompareTag("Obstacle"))
             TakeHit();
        if (col.gameObject.CompareTag("Enemy")&& speed >= boost1Threshold )
            HitGuy();
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