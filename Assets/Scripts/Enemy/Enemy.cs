using System.Collections;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu kod bir düşmanın hareketini ve oyuncuya vurmasını kontrol eder.

- Rastgele sağa/sola yürür
- Belirli aralıklarla zıplar
- Oyuncuya çarptığında hasar verir ve geri savurur
- Oyuncuya çarptığında ses çalar
- Rengini başlatabilir
*/

public class Enemy : MonoBehaviour
{
    /*
    DÜŞMANIN ÖZELLİKLERİ
    -------------------
    _jumpForce : Zıplarken ne kadar yükseğe gider
    _jumpInterval : Ne sıklıkla zıplar (saniye)
    _changeDirectionInterval : Yön değiştirme aralığı (saniye)
    _damageAmount : Oyuncuya vurunca ne kadar can kaybı
    _knockBackThrust : Oyuncuyu geri ne kadar savurur
    */
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _jumpInterval = 4f;
    [SerializeField] private float _changeDirectionInterval = 3f;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _knockBackThrust = 24f;

    /*
    REFERANSLAR
    ------------
    _rigidBody : Fizik kurallarıyla zıplamak ve hareket için
    _movement : Yürümeyi kontrol eden başka bir script
    _colorChanger : Rengini değiştirmek için
    */
    private Rigidbody2D _rigidBody;
    private Movement _movement;
    private ColorChanger _colorChanger;

    /*
    AWAKE
    -----
    Oyun başlar başlamaz referansları bulur.
    */
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _movement = GetComponent<Movement>();
        _colorChanger = GetComponent<ColorChanger>();
    }

    /*
    PLAYER İLE ÇARPIŞMA
    -------------------
    Oyuncuya çarpınca ne olur?
    1️⃣ TakeHit() çağrılır → beyaz yanıp sönme
    2️⃣ TakeDamage() çağrılır → can düşer + geri savrulur
    3️⃣ Ses çalar
    */
    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (!player) return;

        Movement playerMovement = collision.gameObject.GetComponent<Movement>();
        if (playerMovement.CanMove)
        {
            IHitable iHitable = collision.gameObject.GetComponent<IHitable>();
            iHitable?.TakeHit();

            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
            iDamageable?.TakeDamage(transform.position, _damageAmount, _knockBackThrust);

            AudioManager.Instance.Enemy_OnPlayerHit();
        }
    }

    /*
    START
    -----
    Oyun başladığında düşmanın rutin hareketleri başlar
    */
    private void Start()
    {
        StartCoroutine(ChangeDirectionRoutine());
        StartCoroutine(RandomJumpRoutine());
    }

    /*
    INIT
    ----
    Düşmanın rengini başlatır
    */
    public void Init(Color color)
    {
        _colorChanger.SetDefaultColor(color);
    }

    /*
    YÖN DEĞİŞTİRME
    ----------------
    Belirli aralıklarla rastgele sağa ya da sola yürür
    */
    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            float _currentDirection = UnityEngine.Random.Range(0, 2) * 2 - 1; // 1 or -1
            _movement.SetCurrentDirection(_currentDirection);
            yield return new WaitForSeconds(_changeDirectionInterval);
        }
    }

    /*
    RASTGELE ZIPLAMA
    ----------------
    Belirli aralıklarla yukarı doğru ve biraz sağa/sola zıplar
    */
    private IEnumerator RandomJumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_jumpInterval);

            float randomDirection = Random.Range(-1, 1); // -1 ile 1 arasında
            Vector2 jumpDirection = new Vector2(randomDirection, 1f).normalized;
            _rigidBody.AddForce(jumpDirection * _jumpForce, ForceMode2D.Impulse);
        }
    }
}
