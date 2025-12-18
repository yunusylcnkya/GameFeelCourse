using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class DiscoBallManager : MonoBehaviour
{
    public static Action OnDiscoBallHitEvent;

    [SerializeField] private float _discoBallPartyTime = 2f;
    [SerializeField] private float _discoGlobalLightIntensity = .2f;
    [SerializeField] private Light2D _globalLight;

    private float _defaultGlobalLightIntesity;
    private Coroutine _discoCoroutine;
    private ColorSpotlight[] _allSpotLights;
    void Awake()
    {
        _defaultGlobalLightIntesity = _globalLight.intensity;
    }
    void Start()
    {
        _allSpotLights = FindObjectsByType<ColorSpotlight>(FindObjectsSortMode.None);
    }

    void OnEnable()
    {

        OnDiscoBallHitEvent += DimTheLights;
    }
    void OnDisable()
    {
        OnDiscoBallHitEvent -= DimTheLights;

    }
    public void DiscoBallParty()
    {
        if (_discoCoroutine != null)
        { return; }

        OnDiscoBallHitEvent?.Invoke();

    }

    private void DimTheLights()
    {
        foreach (ColorSpotlight spotLight in _allSpotLights)
        {
            StartCoroutine(spotLight.SpotlightDiscoParty(_discoBallPartyTime));
        }
        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }

    private IEnumerator GlobalLightResetRoutine()
    {
        _globalLight.intensity = _discoGlobalLightIntensity;
        yield return new WaitForSeconds(_discoBallPartyTime);
        _globalLight.intensity = _defaultGlobalLightIntesity;
        _discoCoroutine = null;
    }
}
