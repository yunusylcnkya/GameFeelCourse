using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu script oyuncunun ANİMASYON ve GÖRSEL efektlerini kontrol eder.

Yani:
- Yürürken çıkan tozları
- Zıplarken çıkan puf efektini
- Yere sert düşünce kamera sarsıntısını
- Karakterin sağa sola eğilmesini
- Şapkanın ayrı ayrı hareket etmesini

Bu script OYUN HİSSİNİ güzelleştirir.
Karakteri canlı ve eğlenceli yapar.
*/
public class PlayerAnimations : MonoBehaviour
{
    /*
    GÖRSEL EFEKTLER:
    ---------------
    - _moveDustVFX : Yürürken çıkan toz
    - _poofDustVFX : Zıplama / yere düşme pufu
    */
    [SerializeField] private ParticleSystem _moveDustVFX;
    [SerializeField] private ParticleSystem _poofDustVFX;

    /*
    EĞİLME (TILT) AYARLARI:
    ----------------------
    Karakter yürürken biraz eğilir.
    */
    [SerializeField] private float _tiltAngle = 20f;
    [SerializeField] private float _tiltSpeed = 4f;

    /*
    TRANSFORM REFERANSLARI:
    ----------------------
    - Karakterin sprite'ı
    - Kovboy şapkası
    */
    [SerializeField] private Transform _characterSpriteTransform;
    [SerializeField] private Transform _cowboyHatTransform;

    /*
    ŞAPKA AYARLARI:
    ---------------
    Şapka karakterden daha yavaş ve
    daha az eğilir.
    */
    [SerializeField] private float _cowboyHattiltModifier = 4f;

    /*
    YERE SERT DÜŞME KONTROLÜ:
    ------------------------
    Eğer çok hızlı aşağı düşmüşse
    yere çarpınca efekt oynatılır.
    */
    [SerializeField] private float _ylandVelocityCheck = -20f;

    // Fizik güncellemesinden önceki hız
    private Vector2 _velocityBeforePhysicsUpdate;

    // Gerekli bileşenler
    private Rigidbody2D _rigidBody;
    private CinemachineImpulseSource _impulseSource;

    /*
    AWAKE:
    ------
    Oyun başında çalışır.
    Gerekli bileşenleri alır.
    */
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    /*
    UPDATE:
    -------
    Her kare çalışır.
    Görsel işlemler burada yapılır.
    */
    private void Update()
    {
        DetectMoveDust(); // Yürürken toz çıkar
        ApplyTilt();     // Karakteri sağa sola eğ
    }

    /*
    FIXEDUPDATE:
    ------------
    Fizik çalışmadan önce
    karakterin hızını kaydeder.
    */
    private void FixedUpdate()
    {
        _velocityBeforePhysicsUpdate = _rigidBody.linearVelocity;
    }

    /*
    EVENT BAĞLANTILARI:
    ------------------
    Zıplama olunca puf efekti oynatılır.
    */
    void OnEnable()
    {
        PlayerController.OnJump += PlayPoofDustVFX;
    }

    void OnDisable()
    {
        PlayerController.OnJump -= PlayPoofDustVFX;
    }

    /*
    YERE ÇARPMA KONTROLÜ:
    --------------------
    Eğer karakter çok hızlı düşmüşse:
    - Toz efekti çıkar
    - Kamera sarsılır
    */
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_velocityBeforePhysicsUpdate.y < _ylandVelocityCheck)
        {
            PlayPoofDustVFX();
            _impulseSource.GenerateImpulse();
        }
    }

    /*
    YÜRÜME TOZU:
    ------------
    Karakter yerdeyken hareket ediyorsa
    toz efekti oynatılır.
    */
    private void DetectMoveDust()
    {
        if (PlayerController.Instance.CheckGrounded())
        {
            if (!_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Play();
            }
        }
        else
        {
            if (_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Stop();
            }
        }
    }

    /*
    PUFF EFEKTİ:
    ------------
    Zıplarken veya yere sert düşünce
    çıkan toz efekti.
    */
    private void PlayPoofDustVFX()
    {
        _poofDustVFX.Play();
    }

    /*
    KARAKTER EĞİLME (TILT):
    ----------------------
    - Sola gidiyorsa sola eğilir
    - Sağa gidiyorsa sağa eğilir
    - Duruyorsa düzelir

    Şapka ise:
    - Daha az
    - Daha yumuşak
    hareket eder
    */
    private void ApplyTilt()
    {
        float targetAngle;

        if (PlayerController.Instance.MoveInput.x < 0f)
            targetAngle = _tiltAngle;
        else if (PlayerController.Instance.MoveInput.x > 0f)
            targetAngle = -_tiltAngle;
        else
            targetAngle = 0f;

        Quaternion currentCharacterRotation = _characterSpriteTransform.rotation;
        Quaternion targetCharacterRotation =
            Quaternion.Euler(
                currentCharacterRotation.eulerAngles.x,
                currentCharacterRotation.eulerAngles.y,
                targetAngle
            );

        _characterSpriteTransform.rotation =
            Quaternion.Lerp(
                currentCharacterRotation,
                targetCharacterRotation,
                _tiltSpeed * Time.deltaTime
            );

        // ŞAPKA EĞİLME
        Quaternion currentHatRotation = _cowboyHatTransform.rotation;
        Quaternion targetHatRotation =
            Quaternion.Euler(
                currentHatRotation.eulerAngles.x,
                currentHatRotation.eulerAngles.y,
                -targetAngle / _cowboyHattiltModifier
            );

        _cowboyHatTransform.rotation =
            Quaternion.Lerp(
                currentHatRotation,
                targetHatRotation,
                _tiltSpeed * _cowboyHattiltModifier * Time.deltaTime
            );
    }
}
