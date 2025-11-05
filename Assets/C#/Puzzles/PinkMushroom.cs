using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class PinkMushroom : MonoBehaviour
{
    [SerializeField] float _maxSpores = 100f;
    [SerializeField] float _mushRespawnTime = 15f;

    [Header("AreaValue")]
    [SerializeField] float _radius = 1f;

    [Header("LightValues")]
    [SerializeField] Light2D _auraLight;
    [SerializeField] Light2D _spotLight;
    [SerializeField] ParticleSystem _sporesParticles;
    [SerializeField] float _minIntensity = 0.5f;
    [SerializeField] float _maxIntensity = 1.5f;
    [SerializeField] float _pulseSpeed = 2f;

    float _originalSporesAmmount;
    float _originalSpotIntensity;

    [Header("Spores Values")]
    [SerializeField] float _decreaseValue = 5f;
    [SerializeField] float _increaseValue = 1f;
    [SerializeField] float _sporesAmmount = 50f;

    [Header("View Values")]
    [SerializeField] float _liveMushDuration = 2f;
    [SerializeField] float _deadMushDuration = 10f;
    [SerializeField] Animator _animator;
    [SerializeField] string _animIsAliveBoolName;

    Transform _targetTransform;

    bool _haveAnySpores = true;
    public bool HaveAnySpores { get { return _haveAnySpores; } }

    private void Start()
    {
        GameManager.Instance.Player._hallucinationManager._allMushrooms.Add(this);
        _targetTransform = GameManager.Instance.Player.transform;

        _sporesAmmount = _maxSpores;
        _originalSporesAmmount = _sporesAmmount;
        _originalSpotIntensity = _spotLight.intensity;
    }
    private void Update()
    {
        if (IsTargetOnRadius(_targetTransform.position)) DecreaseSpores();
        else IncreaseSpores();

        AnimatorSetValues();
        PulseLightEffect();
    }
    public bool IsTargetOnRadius(Vector3 playerPos)
    {
        float distance = (Vector3.Distance(transform.position, playerPos));
        return distance < _radius;
    }

    void IncreaseSpores()
    {
        if (_sporesAmmount >= _maxSpores || !_haveAnySpores) 
        {
            _sporesAmmount = _maxSpores;
            return;
        }

        _sporesAmmount += _increaseValue * Time.deltaTime;
    }

    void DecreaseSpores()
    {
        if (!_haveAnySpores) return;

        if (_sporesAmmount <= 0)
        {
            _sporesAmmount = 0;
            _haveAnySpores = false;

            StartCoroutine(SetDeadMush());
            return;
        }
        _sporesAmmount -= _decreaseValue * Time.deltaTime;
    }
    IEnumerator SetDeadMush()
    {
        StartCoroutine(MushRespawnRoutine());

        _sporesParticles.Stop();

        float elapsedTime = 0f;

        while (elapsedTime < _deadMushDuration)
        {
            _spotLight.intensity = Mathf.Lerp(_spotLight.intensity, 0f, elapsedTime / _deadMushDuration);
            _auraLight.intensity = Mathf.Lerp(_auraLight.intensity, 0f, elapsedTime / _deadMushDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator SetLiveMush()
    {
        _sporesParticles.Play();
        _haveAnySpores = true;

        float elapsedTime = 0f;

        while (elapsedTime < _liveMushDuration)
        {
            _spotLight.intensity = Mathf.Lerp(0, _originalSpotIntensity, elapsedTime / _liveMushDuration);
            _auraLight.intensity = Mathf.Lerp(0, _minIntensity, elapsedTime / _liveMushDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _sporesAmmount = _originalSporesAmmount;
        
    }
    IEnumerator MushRespawnRoutine()
    {
        yield return new WaitForSeconds(_mushRespawnTime);
        StartCoroutine(SetLiveMush());
    }

    void PulseLightEffect()
    {
        if (!_haveAnySpores) return;

        float t = (Mathf.Sin(Time.time * _pulseSpeed) + 1f) / 2f;
        _auraLight.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, t);

        var radiusLerp = Mathf.Lerp(_radius * 1.9f, _radius * 2f, t);
        _auraLight.transform.localScale = new Vector3(radiusLerp, radiusLerp);
    }

    void AnimatorSetValues() 
    {
        _animator.SetBool(_animIsAliveBoolName, _haveAnySpores);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnDestroy()
    {
        GameManager.Instance.Player._hallucinationManager._allMushrooms.Remove(this);
    }
}
