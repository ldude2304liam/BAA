using UnityEngine;
using UnityEngine.UI;

public class SpeedBar : MonoBehaviour
{
    private Slider slider;
    private NewPlayerMovement player;
    private Image fillImage;
    private Color originalColor;

    //only in the last stage
    public float blinkSpeed = 6f;

    public Color warningColor = Color.yellow;

    public Color dangerColor = Color.white;
    public Color holdingColor = Color.red;


    void Start()
    {
        slider = GetComponent<Slider>();
        player = FindFirstObjectByType<NewPlayerMovement>();

        slider.minValue = 0f;
        slider.maxValue = player.maxSpeed;

        fillImage = slider.fillRect.GetComponent<Image>();
        originalColor = fillImage.color; 
    }

    void Update()
    { //// 1 is MaxChargeHoldTime 
        slider.value = NewPlayerMovement.speed;

        if (player.isCharging)
        {
            float held = player.chargeHoldTimer;
            float limit = player.maxChargeHoldTime;

            if (held < limit * 0.33f)
            {
                //safe, normal color, no blink
                fillImage.color = holdingColor;
            }
            else if (held < limit * 0.66f)
            {
                // warning, yellow, no blink
                fillImage.color = warningColor;
            }
            else
            {
                // danger, grey, blinking
                float alpha = Mathf.Sin(Time.time * blinkSpeed * Mathf.PI) * 0.5f + 0.5f; //math sin moves the number from -1 and 1
                fillImage.color = new Color(dangerColor.r, dangerColor.g, dangerColor.b, alpha);
            }
        }
        else
        {
            //restore original color and full opacity
            fillImage.color = originalColor;
        }
    }
}