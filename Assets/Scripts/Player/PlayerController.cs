using System;
using System.Collections;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu script oyuncu karakterinin beynidir.
Karakterin:
- Sağa sola yürümesini
- Zıplamasını
- Havada ekstra zıplamasını
- Jetpack ile uçmasını
- Daha gerçekçi düşmesini
- Mouse’a doğru dönmesini
kontrol eder.
*/
public class PlayerController : MonoBehaviour
{
    // Oyuncunun sağa-sola hareket bilgisini dışarıdan okunabilir yapar
    public Vector2 MoveInput => _frameInput.Move;

    /*
    EVENTLER:
    ---------
    Bunlar "haber verme" sistemidir.
    Zıplama veya jetpack olunca,
    bu eventleri dinleyen fonksiyonlar çalışır.
    */
    public static Action OnJump;
    public static Action OnJetpack;

    // Bu scriptin tek bir tane olmasını sağlar (her yerden ulaşmak için)
    public static PlayerController Instance;

    /*
    UNITY'DEN AYARLANAN ALANLAR:
    ---------------------------
    Bunlar Inspector'dan değiştirilebilir.
    Oyun ayarları gibi düşünebilirsin.
    */
    [SerializeField] private TrailRenderer _jetpackTrailRenderer;
    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _jumpStrength = 7f;
    [SerializeField] private float _extraGravity = 770f;
    [SerializeField] private float _gravityDelay = .2f;
    [SerializeField] private float _coyoteTime = .4f;
    [SerializeField] private float _jetpackTime = .6f;
    [SerializeField] private float _jetpackStrength = 11f;
    [SerializeField] private float _maxFallSpeedVelocity = -25f;

    /*
    ÖZEL DEĞİŞKENLER:
    ----------------
    Bunlar oyuncunun durumu için kullanılır.
    */
    private float _coyoteTimer, _timeInAir;
    private bool _doubleJumpAvailable;
    private Coroutine _jetpackCoroutine;

    // Diğer scriptler ve bileşenler
    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private Rigidbody2D _rigidBody;
    private Movement _movement;

    /*
    AWAKE:
    ------
    Oyun başlar başlamaz çalışır.
    Gerekli parçaları hazırlar.
    */
    public void Awake()
    {
        if (Instance == null) { Instance = this; }

        _rigidBody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<Movement>();
    }

    /*
    ONENABLE / ONDISABLE:
    --------------------
    Zıplama ve jetpack eventlerine
    hangi fonksiyonların cevap vereceğini söyler.
    */
    void OnEnable()
    {
        OnJump += ApplyJumpForce;
        OnJetpack += StartJetpack;
    }

    void OnDisable()
    {
        OnJump -= ApplyJumpForce;
        OnJetpack -= StartJetpack;
    }

    /*
    UPDATE:
    -------
    Her saniye defalarca çalışır.
    Oyuncu ne yapıyor diye kontrol eder.
    */
    private void Update()
    {
        GatherInput();
        Movement();
        CoyoteTimer();
        HandleJump();
        HandleSpriteFlip();
        GravityDelay();
        Jetpack();
    }

    /*
    FIXEDUPDATE:
    ------------
    Fizik işlemleri burada yapılır.
    */
    void FixedUpdate()
    {
        ExtraGravity();
    }

    void OnDestroy()
    {
        Fade fade = FindFirstObjectByType<Fade>();
        fade?.FadeInAndOut();
    }
    /*
    YERDE Mİ KONTROLÜ:
    -----------------
    Karakterin ayağının altına
    görünmez bir kutu koyar.
    Yere değiyorsa yerde demektir.
    */
    public bool CheckGrounded()
    {
        Collider2D isGrounded = Physics2D.OverlapBox(
            _feetTransform.position,
            _groundCheck,
            0f,
            _groundLayer
        );

        return isGrounded;
    }

    /*
    KIRMIZI KUTU:
    -------------
    Scene ekranında yerde mi
    kontrolünü görmek için.
    */
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }

    /*
    HAVADA KALMA SÜRESİ:
    -------------------
    Karakter havadaysa zaman sayar,
    yere inince sıfırlar.
    */
    private void GravityDelay()
    {
        if (!CheckGrounded())
            _timeInAir += Time.deltaTime;
        else
            _timeInAir = 0f;
    }

    /*
    EKSTRA YERÇEKİMİ:
    ----------------
    Bir süre sonra karakteri
    daha hızlı aşağı çeker.
    */
    private void ExtraGravity()
    {
        if (_timeInAir > _gravityDelay)
        {
            _rigidBody.AddForce(new Vector2(0f, -_extraGravity * Time.deltaTime));

            if (_rigidBody.linearVelocityY < _maxFallSpeedVelocity)
            {
                _rigidBody.linearVelocity =
                    new Vector2(_rigidBody.linearVelocityX, _maxFallSpeedVelocity);
            }
        }
    }

    /*
    INPUT ALMA:
    ----------
    Klavye veya gamepad
    bilgilerini alır.
    */
    private void GatherInput()
    {
        _frameInput = _playerInput.FrameInput;
    }

    /*
    HAREKET:
    --------
    Sağa sola hareket bilgisini
    Movement scriptine gönderir.
    */
    private void Movement()
    {
        _movement.SetCurrentDirection(_frameInput.Move.x);
    }

    /*
    ZIPLAMA KONTROLÜ:
    ----------------
    Yerdeyse, yeni düşmüşse
    veya çift zıplama hakkı varsa
    zıplamasına izin verir.
    */
    private void HandleJump()
    {
        if (!_frameInput.Jump) return;

        if (CheckGrounded())
            OnJump?.Invoke();
        else if (_coyoteTimer > 0f)
            OnJump?.Invoke();
        else if (_doubleJumpAvailable)
        {
            _doubleJumpAvailable = false;
            OnJump?.Invoke();
        }
    }

    /*
    COYOTE TIME:
    ------------
    Yeni düşmüşken kısa süreli
    zıplama hakkı verir.
    */
    private void CoyoteTimer()
    {
        if (CheckGrounded())
        {
            _coyoteTimer = _coyoteTime;
            _doubleJumpAvailable = true;
        }
        else
            _coyoteTimer -= Time.deltaTime;
    }

    /*
    ZIPLAMA KUVVETİ:
    ----------------
    Karakteri yukarı doğru iter.
    */
    private void ApplyJumpForce()
    {
        _rigidBody.linearVelocity = Vector2.zero;
        _timeInAir = 0f;
        _coyoteTimer = 0f;

        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    /*
    KARAKTERİN YÖNÜ:
    ---------------
    Mouse neredeyse
    karakter oraya döner.
    */
    private void HandleSpriteFlip()
    {
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x < transform.position.x)
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        else
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    /*
    JETPACK KONTROLÜ:
    ----------------
    Tuşa basıldıysa ve
    jetpack çalışmıyorsa başlatır.
    */
    private void Jetpack()
    {
        if (!_frameInput.Jetpack || _jetpackCoroutine != null) return;
        OnJetpack?.Invoke();
    }

    /*
    JETPACK BAŞLATMA:
    ----------------
    Ateş efektini açar
    ve coroutine başlatır.
    */
    private void StartJetpack()
    {
        _jetpackTrailRenderer.emitting = true;
        _jetpackCoroutine = StartCoroutine(JetpackRoutine());
    }

    /*
    JETPACK ROUTINE:
    ----------------
    Belli süre boyunca
    karakteri yukarı iter.
    */
    private IEnumerator JetpackRoutine()
    {
        float jetTime = 0f;

        while (jetTime < _jetpackTime)
        {
            jetTime += Time.deltaTime;
            _rigidBody.linearVelocity = Vector2.up * _jetpackStrength;
            yield return null;
        }

        _jetpackTrailRenderer.emitting = false;
        _jetpackCoroutine = null;
    }
}
