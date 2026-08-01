using UnityEngine;

public class ColorBlink : MonoBehaviour
{
    [Header("Animation Timing Settings")]
    public float blinkClock;
    public float clockSpeed = 1;
    public float maxClock = 1;
    public AnimationCurve blinkCurve;

    [Header("Color Settings")]
    public Color currentColor;
    public Color startColor;
    public Color endColor;

    [Header("System Settings(usually dont touch)")]
    public float lerpValue;
    public Material myMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myMaterial = GetComponent<MeshRenderer>().material;
        currentColor = myMaterial.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (blinkClock > maxClock)
            blinkClock = 0;

        lerpValue = blinkCurve.Evaluate(blinkClock);
        currentColor = Color.Lerp(startColor, endColor, lerpValue);

        myMaterial.SetColor("_EmmissionColor", currentColor);
        myMaterial.color = currentColor;

        blinkClock += Time.deltaTime * clockSpeed;
    }

    public void SetSpeed(float newspeed)
    {
        blinkClock = newspeed;
    }
}
