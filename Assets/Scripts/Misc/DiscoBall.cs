using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, bir disko topunu kontrol eder:

- Topa vurulduğunda ışıkları yanıp söner (Flash)
- DiscoBallManager üzerinden "Disco Party" modunu başlatır
*/

public class DiscoBall : MonoBehaviour, IHitable
{
    private Flash _flash;                     // Işığı yanıp söndüren script
    private DiscoBallManager _discoBallManager; // Disco Party'yi yöneten script

    void Awake()
    {
        _flash = GetComponent<Flash>();                         // Flash script'ini bul
        _discoBallManager = FindFirstObjectByType<DiscoBallManager>(); // DiscoBallManager'ı bul
    }

    // Topa vurulunca çalışır
    public void TakeHit()
    {
        _discoBallManager.DiscoBallParty(); // Disco Party başlat
        _flash.StartFlash();                // Top ışığı yanıp söner
    }
}
