using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    // Silah ateşlendiğinde tetiklenecek event. Bu sınıf içinde kullanılıyor.
    public static Action OnShoot;
    public static Action OnGrenadeShoot;

    [SerializeField] private Transform _bulletSpawnPoint; // Merminin çıkacağı nokta
    [Header("Bullet")]
    [SerializeField] private Bullet _bulletPrefab;        // Havuzdan üretilecek mermi prefab'ı
    [SerializeField] private float _gunFireCD = .5f;      // Ateş etme gecikmesi (cooldown)
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private float _muzzleFlashTime = 0.05f;

    [Header("Grenade")]
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private float _grenadeShootCD = .9f;      // Ateş etme gecikmesi (cooldown)


    private Coroutine _muzzleFlashRoutine;
    private ObjectPool<Bullet> _bulletPool;               // Mermileri yöneten Object Pool

    private static readonly int FIRE_HASH = Animator.StringToHash("Fire"); // Fire animasyonu hash değeri
    private Vector2 _mousePos;                            // Maus'un world pozisyonu
    private float _lastFireTime = 0f;                     // Son ateş zamanının kaydı
    private float _lastGrenadeTime = 0f;                     // Son ateş zamanının kaydı

    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private Animator _animator;                           // Silah animasyon komponenti
    private CinemachineImpulseSource _impulseSource;      // Kamera sarsıntısı kaynağı

    void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponentInParent<PlayerInput>();
        _frameInput = _playerInput.FrameInput;
    }

    void Start()
    {
        // Mermi havuzunu başlangıçta oluşturuyoruz
        CreateBulletPool();
    }

    private void Update()
    {
        GatherInput();
        Shoot();        // Sol mouse basılıysa ateş etmeye çalış
        RotateGun();    // Silahı mouse yönüne döndür
    }

    void OnEnable()
    {
        // Event’e metodlar ekleniyor (ateş sırası)
        OnShoot += ResetLastFireTime;   // Cooldown yenile
        OnShoot += ShootProjectile;     // Mermiyi oluştur
        OnShoot += FireAnimation;       // Animasyonu tetikle
        OnShoot += GunScreenShake;      // Kamera sarsıntısı yap
        OnShoot += MuzzleFlash;
        OnGrenadeShoot += ShootGrenade;
        OnGrenadeShoot += FireAnimation;
        OnGrenadeShoot += ResetLastGrenadeShootTime;
    }

    void OnDisable()
    {
        // Event’den metodlar çıkarılıyor
        OnShoot -= ResetLastFireTime;
        OnShoot -= ShootProjectile;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
        OnShoot -= MuzzleFlash;
        OnGrenadeShoot -= ShootGrenade;
        OnGrenadeShoot -= FireAnimation;
        OnGrenadeShoot -= ResetLastGrenadeShootTime;


    }

    // Mermiyi havuza geri gönderir
    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }


    private void GatherInput()
    {
        _frameInput = _playerInput.FrameInput;

    }


    // Object Pool ayarları
    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() =>
        {
            // Yeni mermi instantiate edildiğinde
            return Instantiate(_bulletPrefab);

        },
        bullet =>
        {
            // Havuzdan çekildiğinde aktif edilir
            bullet.gameObject.SetActive(true);

        },
        bullet =>
        {
            // Havuzdan çıkarıldığında pasif hale getirilir
            bullet.gameObject.SetActive(false);

        },
        bullet =>
        {
            // Havuz kapasitesi düşerse mermi tamamen yok edilir
            Destroy(bullet.gameObject);

        },
        false,    // Havuz önceden doldurulmayacak
        20,       // Minimum havuz büyüklüğü
        50        // Maksimum havuz büyüklüğü
        );
    }

    // Ateş etme girişini kontrol eder
    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= _lastFireTime)
        {
            OnShoot?.Invoke(); // Event'teki tüm fonksiyonlar çalıştırılır
        }
        if (_frameInput.Grenade && Time.time >= _lastGrenadeTime)
        {
            OnGrenadeShoot?.Invoke();
        }
    }

    // Havuzdan mermi alıp ateşler
    private void ShootProjectile()
    {
        Bullet newBullet = _bulletPool.Get(); // Havuzdan mermi çek
        newBullet.Init(this, _bulletSpawnPoint.position, _mousePos); // Mermiye başlangıç verilerini gönder
    }

    private void ShootGrenade()
    {
        Instantiate(_grenadePrefab, _bulletSpawnPoint.position, Quaternion.identity);
        _lastGrenadeTime = Time.time;
    }

    // Fire animasyonu oynatır
    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }

    // Cooldown zamanını günceller
    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD;
    }
    private void ResetLastGrenadeShootTime()
    {
        _lastGrenadeTime = Time.time + _grenadeShootCD;
    }

    // Kamera sarsıntısı oluşturur
    private void GunScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }

    // Silahı mouse pozisyonuna döndürür
    private void RotateGun()
    {
        // Mouse world pozisyonu alınıyor
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Mouse pozisyonu player'ın local koordinatına çevriliyor
        // (Player child olduğu için böyle kullanılmış)
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePos);
        // Açı hesaplanıyor
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Silaha local olarak açı uygulanıyor
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

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
