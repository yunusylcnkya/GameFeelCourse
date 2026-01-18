using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, oyuncunun silahını kontrol eder:

- Mouse’a doğru döner
- Sol mouse basılıysa mermi ateşler
- G tuşuna basılırsa el bombası fırlatır
- Mermi ve el bombası için cooldown (bekleme) yönetir
- Mermi havuzunu (Object Pool) kullanır
- Ateş animasyonu oynatır
- Kamera sarsıntısı ve namlu flaşı efekti ekler
*/

public class Gun : MonoBehaviour
{
    // Silah ateşlendiğinde tetiklenecek olaylar
    public static Action OnShoot;          // Mermi ateşlendiğinde
    public static Action OnGrenadeShoot;   // El bombası fırlatıldığında

    [SerializeField] private Transform _bulletSpawnPoint; // Merminin çıkacağı nokta

    [Header("Bullet")]
    [SerializeField] private Bullet _bulletPrefab;        // Havuzdan üretilecek mermi prefab'ı
    [SerializeField] private float _gunFireCD = .5f;      // Mermi ateşleme gecikmesi
    [SerializeField] private GameObject _muzzleFlash;     // Ateş efekti
    [SerializeField] private float _muzzleFlashTime = 0.05f;

    [Header("Grenade")]
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private float _grenadeShootCD = .9f; // El bombası ateşleme gecikmesi

    private Coroutine _muzzleFlashRoutine;
    private ObjectPool<Bullet> _bulletPool; // Mermi havuzu

    private static readonly int FIRE_HASH = Animator.StringToHash("Fire"); // Ateş animasyonu hash
    private Vector2 _mousePos;                // Mouse’un oyun içi pozisyonu
    private float _lastFireTime = 0f;         // Son mermi ateş zamanı
    private float _lastGrenadeTime = 0f;      // Son el bombası ateş zamanı

    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private Animator _animator;               // Silah animasyon komponenti
    private CinemachineImpulseSource _impulseSource; // Kamera sarsıntısı kaynağı

    void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponentInParent<PlayerInput>();
        _frameInput = _playerInput.FrameInput;
    }

    void Start()
    {
        CreateBulletPool(); // Mermi havuzunu oluştur
    }

    private void Update()
    {
        GatherInput(); // Inputları al
        Shoot();       // Sol mouse basılıysa mermi veya el bombası ateşle
        RotateGun();   // Silahı mouse yönüne döndür
    }

    void OnEnable()
    {
        // Olaylara metodlar ekleniyor
        OnShoot += ResetLastFireTime;
        OnShoot += ShootProjectile;
        OnShoot += FireAnimation;
        OnShoot += GunScreenShake;
        OnShoot += MuzzleFlash;

        OnGrenadeShoot += ShootGrenade;
        OnGrenadeShoot += FireAnimation;
        OnGrenadeShoot += ResetLastGrenadeShootTime;
    }

    void OnDisable()
    {
        // Olaylardan metodlar çıkarılıyor
        OnShoot -= ResetLastFireTime;
        OnShoot -= ShootProjectile;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
        OnShoot -= MuzzleFlash;

        OnGrenadeShoot -= ShootGrenade;
        OnGrenadeShoot -= FireAnimation;
        OnGrenadeShoot -= ResetLastGrenadeShootTime;
    }

    // Mermiyi havuza geri gönder
    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void GatherInput()
    {
        _frameInput = _playerInput.FrameInput;
    }

    // Mermi havuzunu oluştur
    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() =>
        {
            return Instantiate(_bulletPrefab); // Yeni mermi oluştur
        },
        bullet =>
        {
            bullet.gameObject.SetActive(true);  // Havuzdan çekildiğinde aktif et
        },
        bullet =>
        {
            bullet.gameObject.SetActive(false); // Havuzdan çıkarıldığında pasif yap
        },
        bullet =>
        {
            Destroy(bullet.gameObject);         // Havuz kapasitesi düşerse yok et
        },
        false, 20, 50);
    }

    // Mermi ve el bombası ateş kontrolü
    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= _lastFireTime)
        {
            OnShoot?.Invoke(); // Mermi ateşle
        }

        if (_frameInput.Grenade && Time.time >= _lastGrenadeTime)
        {
            OnGrenadeShoot?.Invoke(); // El bombası fırlat
        }
    }

    // Havuzdan mermi al ve ateşle
    private void ShootProjectile()
    {
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, _mousePos);
    }

    // El bombası fırlat
    private void ShootGrenade()
    {
        Instantiate(_grenadePrefab, _bulletSpawnPoint.position, Quaternion.identity);
        _lastGrenadeTime = Time.time;
    }

    // Ateş animasyonu oynat
    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }

    // Cooldown zamanlarını ayarla
    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD;
    }
    private void ResetLastGrenadeShootTime()
    {
        _lastGrenadeTime = Time.time + _grenadeShootCD;
    }

    // Kamera sarsıntısı
    private void GunScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }

    // Silahı mouse yönüne döndür
    private void RotateGun()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePos);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    // Namlu flaşı efekti
    private void MuzzleFlash()
    {
        if (_muzzleFlash != null)
        { StopCoroutine(MuzzleFlashRoutine()); }
        _muzzleFlashRoutine = StartCoroutine(MuzzleFlashRoutine());
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(_muzzleFlashTime);
        _muzzleFlash.SetActive(false);
    }
}
