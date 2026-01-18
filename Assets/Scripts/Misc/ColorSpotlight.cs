using System.Collections;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, bir renkli spot ışığını kontrol eder:

- Spot ışığı belirli bir açıda ileri-geri döner
- "Disco Party" modu açılırsa ışık daha hızlı döner
- Başlangıçta spot ışığı rastgele bir açıyla başlar
*/

public class ColorSpotlight : MonoBehaviour
{
    [SerializeField] private GameObject _spotlightHead; // Dönecek spot ışığı
    [SerializeField] private float _rotationSpeed = 20f; // Normal döndürme hızı
    [SerializeField] private float _discoRotSpeed = 40f; // Disco modu döndürme hızı
    [SerializeField] private float _maxRotation = 45f;   // Maksimum açı sınırı

    private float _currentRotation; // Şu anki açı durumu

    void Start()
    {
        RandomStartingRotation(); // Başlangıçta rastgele bir açı ayarla
    }

    void Update()
    {
        RotateHead(); // Her frame spot ışığını döndür
    }

    /*
    SPOTLIGHTDISCOPARTY
    -------------------
    Belirli süre için ışığı daha hızlı döndürür (disco modu)
    */
    public IEnumerator SpotlightDiscoParty(float discoPartyTime)
    {
        float defaultRotSpeed = _rotationSpeed; // Normal hızı sakla
        _rotationSpeed = _discoRotSpeed;        // Disco hızına geçir
        yield return new WaitForSeconds(discoPartyTime); // Disco süresi bekle
        _rotationSpeed = defaultRotSpeed;       // Normal hıza dön
    }

    /*
    ROTATEHEAD
    ----------
    Işığı ileri-geri döndürür
    Mathf.PingPong ile açı -maxRotation ile maxRotation arasında ileri geri gider
    */
    private void RotateHead()
    {
        _currentRotation += Time.deltaTime * _rotationSpeed;
        float z = Mathf.PingPong(_currentRotation, _maxRotation);
        _spotlightHead.transform.localRotation = Quaternion.Euler(0f, 0f, z);
    }

    /*
    RANDOMSTARTINGROTATION
    ---------------------
    Başlangıçta spot ışığı rastgele bir açıyla başlar
    */
    private void RandomStartingRotation()
    {
        float randomStartingZ = Random.Range(-_maxRotation, _maxRotation);
        _spotlightHead.transform.localRotation = Quaternion.Euler(0f, 0f, randomStartingZ);
        _currentRotation = randomStartingZ + _maxRotation; // PingPong için başlangıç açısını ayarla
    }
}
