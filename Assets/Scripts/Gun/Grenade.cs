using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod bir el bombasını kontrol eder:

- Fırlatıldığında doğru yöne gider ve döner
- Belirli bir süre sonra patlar
- Patladığında yakınındaki düşmanlara hasar verir
- Patlama efekti (VFX) çıkar
- Ekranı sallar (screen shake)
- Patlamadan önce ışığı yanıp söner ve bip sesi verir
*/

public class Grenade : MonoBehaviour
{
    /*
    OLAYLAR (Action)
    ----------------
    OnExplode : Patladığında çalışacak her şey
    OnBeep    : Işık yanıp sönerken ve bip sesi için
    */
    public Action OnExplode;
    public Action OnBeep;

    /*
    ÖZELLİKLER
    -----------
    _explodeVFX : Patlama efekti
    _grenadeLight : Yanıp sönen ışık
    _launchForce : Fırlatma hızı
    _torqueAmount : Dönme hızı
    _explosionRadius : Patlama alanı
    _enemyLayerMask : Hangi nesneler hasar alabilir
    _damageAmount : Patlamanın vereceği hasar
    _lightBlinkTime : Işığın yanıp sönme süresi
    _totalBlinks : Patlamadan önce kaç kere yanacak
    _explodeTime : Patlamaya kadar geçen toplam süre
    */
    [SerializeField] private GameObject _explodeVFX;
    [SerializeField] private GameObject _grenadeLight;
    [SerializeField] private float _launchForce = 15f;
    [SerializeField] private float _torqueAmount = 2f;
    [SerializeField] private float _explosionRadius = 3.5f;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private int _damageAmount = 3;
    [SerializeField] private float _lightBlinkTime = .15f;
    [SerializeField] private int _totalBlinks = 3;
    [SerializeField] private int _explodeTime = 3;

    /*
    DİĞER DEĞİŞKENLER
    -----------------
    _currentBlink : Şu ana kadar kaç kere ışık yanıp söndü
    _rigidBody : Fizik hareketi için
    _impulseSource : Patlamada ekran sarsması için
    */
    private int _currentBlink;
    private Rigidbody2D _rigidBody;
    private CinemachineImpulseSource _impulseSource;

    /*
    ONENABLE / ONDISABLE
    -------------------
    Bu grenade aktif/pasif olduğunda hangi olaylar çalışacak,
    onları buradan ekleyip çıkarıyoruz
    */
    void OnEnable()
    {
        OnExplode += Explosion;
        OnExplode += GrenadeScreenShake;
        OnExplode += DamageNearBy;
        OnExplode += AudioManager.Instance.Grenade_OnExplode;
        OnBeep += AudioManager.Instance.Grenade_OnBeep;
        OnBeep += BlinkLight;
    }

    void OnDisable()
    {
        OnExplode -= Explosion;
        OnExplode -= GrenadeScreenShake;
        OnExplode -= DamageNearBy;
        OnExplode -= AudioManager.Instance.Grenade_OnExplode;
        OnBeep -= AudioManager.Instance.Grenade_OnBeep;
        OnBeep -= BlinkLight;
    }

    /*
    AWAKE
    -----
    Oyun başlar başlamaz Rigidbody2D ve ekran sarsma bileşenini bulur
    */
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    /*
    START
    -----
    El bombası fırlatılır ve patlama sayacı başlar
    */
    void Start()
    {
        LaunchGrenade();
        StartCoroutine(CountdownExplodeRoutine());
    }

    /*
    ONCOLLISIONENTER2D
    ------------------
    Eğer el bombası düşmana çarparsa hemen patlar
    */
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>())
        {
            OnExplode?.Invoke();
        }
    }

    /*
    LAUNCHGRENADE
    --------------
    El bombasını mouse yönüne doğru fırlatır ve döndürür
    */
    private void LaunchGrenade()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePos - (Vector2)transform.position).normalized;
        _rigidBody.AddForce(directionToMouse * _launchForce, ForceMode2D.Impulse);
        _rigidBody.AddTorque(_torqueAmount, ForceMode2D.Impulse);
    }

    /*
    EXPLOSION
    ---------
    Patlama efekti çıkar ve el bombası yok edilir
    */
    private void Explosion()
    {
        Instantiate(_explodeVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /*
    GRENADESCREENSHAKE
    ------------------
    Patlama sırasında ekranı sallar
    */
    private void GrenadeScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }

    /*
    DAMAGENEARBY
    -------------
    Patlama alanındaki düşmanlara hasar verir
    */
    private void DamageNearBy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _enemyLayerMask);
        foreach (Collider2D hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            health?.TakeDamage(_damageAmount);
        }
    }

    /*
    COUNTDOWNEXPLODEROUTINE
    -----------------------
    Patlamadan önce ışığı yanıp söndürür ve bip sesi çıkarır
    */
    private IEnumerator CountdownExplodeRoutine()
    {
        while (_currentBlink < _totalBlinks)
        {
            yield return new WaitForSeconds(_explodeTime / _totalBlinks);
            OnBeep?.Invoke();
            yield return new WaitForSeconds(_lightBlinkTime);
            _grenadeLight.SetActive(false);
        }
        OnExplode?.Invoke();
    }

    /*
    BLINKLIGHT
    ----------
    Işığı açar ve kaç kez yanıp söndüğünü sayar
    */
    private void BlinkLight()
    {
        _grenadeLight.SetActive(true);
        _currentBlink++;
    }
}
