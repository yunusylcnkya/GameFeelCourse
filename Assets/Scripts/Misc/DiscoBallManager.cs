using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, oyundaki disko topu partisini yönetir:

- Disco topuna vurulunca ışıkları daha hızlı hareket ettirir
- Global ışığı kısarak "disco modu" atmosferi yaratır
- Disco süresi bitince ışıkları ve global ışığı eski haline getirir
*/

public class DiscoBallManager : MonoBehaviour
{
    public static Action OnDiscoBallHitEvent; // Disco topu vurulunca çalışacak olay

    [SerializeField] private float _discoBallPartyTime = 2f;   // Disco süresi
    [SerializeField] private float _discoGlobalLightIntensity = .2f; // Disco sırasında global ışık
    [SerializeField] private Light2D _globalLight;             // Oyundaki global ışık

    private float _defaultGlobalLightIntesity; // Normal ışık
    private Coroutine _discoCoroutine;         // Disco coroutine kontrolü
    private ColorSpotlight[] _allSpotLights;   // Tüm spot ışıkları

    void Awake()
    {
        _defaultGlobalLightIntesity = _globalLight.intensity; // Normal ışık değerini sakla
    }

    void Start()
    {
        // Oyundaki tüm spot ışıkları bulunur
        _allSpotLights = FindObjectsByType<ColorSpotlight>(FindObjectsSortMode.None);
    }

    void OnEnable()
    {
        OnDiscoBallHitEvent += DimTheLights; // Disco topu vurulunca ışıkları kıs
    }

    void OnDisable()
    {
        OnDiscoBallHitEvent -= DimTheLights; // Olaydan çıkar
    }

    // Disco partisi başlat
    public void DiscoBallParty()
    {
        if (_discoCoroutine != null)
        { return; } // Zaten disco partisindeyse tekrar başlatma

        OnDiscoBallHitEvent?.Invoke(); // Disco topu vurulma olayı tetiklenir
    }

    // Işıkları disco moduna geçir
    private void DimTheLights()
    {
        foreach (ColorSpotlight spotLight in _allSpotLights)
        {
            StartCoroutine(spotLight.SpotlightDiscoParty(_discoBallPartyTime)); // Spot ışıkları hızlı döner
        }

        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine()); // Global ışığı ayarla ve sonra geri döndür
    }

    // Global ışığı disco seviyesine indir, sonra geri getir
    private IEnumerator GlobalLightResetRoutine()
    {
        _globalLight.intensity = _discoGlobalLightIntensity; // Disco modu ışık
        yield return new WaitForSeconds(_discoBallPartyTime); // Disco süresi bekle
        _globalLight.intensity = _defaultGlobalLightIntesity; // Normal ışığa dön
        _discoCoroutine = null;
    }
}
