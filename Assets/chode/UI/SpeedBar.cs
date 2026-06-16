using UnityEngine;
using UnityEngine.UI;
public class SpeedBar : MonoBehaviour
{

    private Slider slider;
    private float targetProgress = 0;
    public float fillSpeed = 20f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
    }
    
    void Start()
    {
        GetComponent<NewPlayerMovement>();
        Progress(0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = NewPlayerMovement.speed ;
    }

    public void Progress(float newProgress)
    {
        targetProgress = slider.value += newProgress;
    }
}
