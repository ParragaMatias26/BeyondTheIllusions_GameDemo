using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPulsation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Light2D _auraLight;

    [Header("Area Values")]
    [SerializeField] float _radius = 1f;
    [SerializeField] float _pulseSpeed = 2f;

    [Header("Intensity Values")]
    [SerializeField] float _minIntensity = 0.5f;
    [SerializeField] float _maxIntensity = 1.5f;
    

    [Header("Size Pulsation Values")]
    [SerializeField] float minRadiusMultiplier = 1.9f;
    [SerializeField] float maxRadiusMultiplier = 2.0f;

    private void Awake()
    {
        _auraLight = gameObject.GetComponent<Light2D>();
    }
    private void Update()
    {
        PulseLightEffect();
    }

    void PulseLightEffect()
    {
        float t = (Mathf.Sin(Time.time * _pulseSpeed) + 1f) / 2f;
        _auraLight.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, t);

        var radiusLerp = Mathf.Lerp(_radius * minRadiusMultiplier, _radius * maxRadiusMultiplier, t);
        _auraLight.transform.localScale = new Vector3(radiusLerp, radiusLerp);
    }
}
