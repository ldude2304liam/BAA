using UnityEngine;
using System.Collections;

public class NewPlayerMovement : MonoBehaviour
{
    // ─────────────────────────────────────────
    //  SPEED SETTINGS
    // ─────────────────────────────────────────
    [Header("Speed Settings")]
    [Tooltip("How fast speed naturally climbs toward the next threshold")]
    public float acceleration = 0.4f;

    [Tooltip("triggers Boost 1")]
    public float boost1Threshold = 8.5f;

    [Tooltip("triggers Boost 2")]
    public float boost2Threshold = 25f;

    [Tooltip("speed cap (Stage 2 ceiling)")]
    public float maxSpeed = 40f;

    [Tooltip("Flat speed added when a boost fires")]
    public float boostSpeedBonus = 5.5f;
    /// <summary>
    /// ////// charge
    /// </summary>
    [Header("Charge Settings")]
    [Tooltip("Max bonus speed the charge can store")]
    public float maxChargeSpeed = 15f;

    [Tooltip("How fast the charge builds while Space is held")]
    public float chargeRate = 1f;

    [Tooltip("How much speed is bled off per frame while charging")]
    public float chargeDrainRate = 0.05f;

    //  TURNING 
    [Header("Turnng Settings")]
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

   
    // currentStage: 0 = normal, 1 = after boost 1, 2 = after boost 2
    private int currentStage = 0;

    private float speed = 1f;
    private float chargeSpeed = 0f;
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

    // ════════════════════════════════════════
    void Start()
    {
        ApplyStageColor();
    }

    // ════════════════════════════════════════
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

            // Drain current speed while charging 
            if (speed >= stageFloor[currentStage] + 0.1f)
                speed -= chargeDrainRate;
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;
            speed = Mathf.Min(speed + chargeSpeed, maxSpeed);
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
        speed += boostSpeedBonus;              // jump forward
        boostPending = false;

        ApplyStageColor();

        if (boostParticles != null)
            boostParticles.Play();
    }
    void ApplyStageColor()
    {
      
    }
}