using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 MoveInput => _frameInput.Move;

    public static Action OnJump;
    public static Action OnJetpack;

    public static PlayerController Instance;

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



    private float _coyoteTimer, _timeInAir;
    private bool _doubleJumpAvailable;
    private Coroutine _jetpackCoroutine;

    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private Rigidbody2D _rigidBody;
    private Movement _movement;



    public void Awake()
    {
        if (Instance == null) { Instance = this; }

        _rigidBody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<Movement>();

    }


    void OnEnable()
    {
        // Zıplama eventine fonksiyon bağlanıyor
        OnJump += ApplyJumpForce;
        OnJetpack += StartJetpack;
    }

    void OnDisable()
    {
        // Eventten fonksiyon kaldırılıyor
        OnJump -= ApplyJumpForce;
        OnJetpack -= StartJetpack;
    }

    private void Update()
    {
        // Her frame çalışacak fonksiyonlar
        GatherInput();      // Inputları al
        Movement();         // Yatay hareketi işle
        CoyoteTimer();      // Coyote time sayacını güncelle
        HandleJump();       // Zıplama kontrolü yap
        HandleSpriteFlip(); // Sprite yönünü mouse’a göre çevir
        GravityDelay();     // Ekstra yerçekimi zamanını güncelle
        Jetpack();
    }

    void FixedUpdate()
    {
        // Fizik güncellemeleri FixedUpdate içinde yapılır
        ExtraGravity();
    }

    void OnDestroy()
    {
        Fade fade = FindFirstObjectByType<Fade>();
        fade?.FadeInAndOut();
    }

    public bool CheckGrounded()
    {
        // Ayak konumunda yer ile çarpışma kontrolü
        Collider2D isGrounded = Physics2D.OverlapBox(
            _feetTransform.position,
            _groundCheck,
            0f,
            _groundLayer
        );

        // Eğer zemin ile temas eden bir şey varsa true döner
        return isGrounded;
    }

    void OnDrawGizmos()
    {
        // Scene view'da zemin kontrol kutusunu görebilmek için
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }

    public bool IsFacingRight()
    {
        // Karakterin yüzü sağa dönük mü?
        return transform.eulerAngles.y == 0;
    }

    private void GravityDelay()
    {
        // Karakter havadaysa zaman sayacı artıyor
        if (!CheckGrounded())
        {
            _timeInAir += Time.deltaTime;
        }
        else
        {
            // Yerdeyse sıfırlanıyor
            _timeInAir = 0f;
        }
    }

    private void ExtraGravity()
    {
        if (_timeInAir > _gravityDelay)
        {
            _rigidBody.AddForce(new Vector2(0f, -_extraGravity * Time.deltaTime));
            if (_rigidBody.linearVelocityY < _maxFallSpeedVelocity)
            {
                _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocityX, _maxFallSpeedVelocity);
            }
        }
    }

    private void GatherInput()
    {
        // PlayerInput scriptinden inputları alıyoruz
        _frameInput = _playerInput.FrameInput;
    }

    private void Movement()
    {
        // Yatay hareketi Movement scriptine gönderiyoruz
        _movement.SetCurrentDirection(_frameInput.Move.x);
    }

    private void HandleJump()
    {
        // Zıplama tuşuna basılmadıysa çık
        if (!_frameInput.Jump) { return; }

        // Yerdeyken ana zıplama
        if (CheckGrounded())
        {
            OnJump?.Invoke();
        }
        // Coyote time süresi boyunca zıplamaya izin ver
        else if (_coyoteTimer > 0f)
        {
            OnJump?.Invoke();
        }
        // Eğer çift zıplama hakkı varsa
        else if (_doubleJumpAvailable)
        {
            _doubleJumpAvailable = false;
            OnJump?.Invoke();
        }
    }

    private void CoyoteTimer()
    {
        // Yerdeyken coyote timer resetlenir
        if (CheckGrounded())
        {
            _coyoteTimer = _coyoteTime;
            _doubleJumpAvailable = true;
        }
        else
        {
            // Havada zaman geri sayar
            _coyoteTimer -= Time.deltaTime;
        }
    }

    private void ApplyJumpForce()
    {
        // Zıplamadan önce dikey hız sıfırlanır
        _rigidBody.linearVelocity = Vector2.zero;

        // Havada geçen süre sıfırlanır
        _timeInAir = 0f;

        // Coyote timer sıfırlanır
        _coyoteTimer = 0f;

        // Yukarı doğru kuvvet verilir (impulse = ani güç)
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    private void HandleSpriteFlip()
    {
        // Mouse pozisyonunu dünya koordinatında alıyoruz
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Mouse soldaysa karakter sola döner
        if (mousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
        // Mouse sağdaysa karakter sağa döner
        else
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    private void Jetpack()
    {
        if (!_frameInput.Jetpack || _jetpackCoroutine != null) return;

        OnJetpack?.Invoke();

    }

    private void StartJetpack()
    {
        _jetpackTrailRenderer.emitting = true;
        _jetpackCoroutine = StartCoroutine(JetpackRoutine());
    }

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
