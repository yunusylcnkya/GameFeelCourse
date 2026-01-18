using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod bir mermiyi kontrol eder:

- Mermiyi ateşler
- Hedefe doğru gider
- Çarptığında hasar verir ve geri savurur
- Çarptığında görsel efekt (VFX) oynatır
- Havuz sistemine geri döner
*/

public class Bullet : MonoBehaviour
{
    /*
    MERMİ ÖZELLİKLERİ
    -----------------
    _bulletVFX : Mermi çarptığında çıkan efekt
    _moveSpeed : Merminin hızı
    _damageAmount : Merminin vurduğunda vereceği hasar
    _knockBackThrust : Merminin vurduğunda nesneyi geri savurma gücü
    */
    [SerializeField] private GameObject _bulletVFX;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _knockBackThrust = 100f;

    /*
    MERMİNİN HAREKETİ İÇİN
    ----------------------
    _fireDirection : Merminin ilerleyeceği yön
    _rigidBody : Fizik bileşeni (hareket için)
    _gun : Mermiyi geri havuza göndermek için referans
    */
    private Vector2 _fireDirection;
    private Rigidbody2D _rigidBody;
    private Gun _gun;

    /*
    AWAKE
    -----
    Oyun başlar başlamaz Rigidbody2D’yi bulur
    */
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    /*
    FIXEDUPDATE
    -----------
    Fizik işlemleri burada yapılır.
    Mermi sürekli ileri doğru gider.
    */
    private void FixedUpdate()
    {
        _rigidBody.linearVelocity = _fireDirection * _moveSpeed;
    }

    /*
    INIT
    ----
    Mermi ateşlendiğinde çağrılır:

    1️⃣ Hangi silah tarafından ateşlendiğini kaydeder (_gun)
    2️⃣ Başlangıç konumunu ayarlar
    3️⃣ Hedef yönünü hesaplar (mouse pozisyonu)
    */
    public void Init(Gun gun, Vector2 bulletSpawnPos, Vector2 mousePos)
    {
        _gun = gun;
        transform.position = bulletSpawnPos;

        // Mouse pozisyonuna doğru yön vektörü
        _fireDirection = (mousePos - bulletSpawnPos).normalized;
    }

    /*
    ONTRIGGERENTER2D
    ----------------
    Mermi bir şeye çarptığında:

    1️⃣ VFX oynatılır
    2️⃣ Eğer çarptığı obje IHitable ise TakeHit çağrılır
    3️⃣ Eğer IDamageable ise TakeDamage çağrılır
    4️⃣ Mermi havuza geri gönderilir (destroy yerine)
    */
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Efekt oluştur
        Instantiate(_bulletVFX, transform.position, Quaternion.identity);

        // Beyaz yanıp sönme (TakeHit)
        IHitable iHitable = other.gameObject.GetComponent<IHitable>();
        iHitable?.TakeHit();

        // Can azalt ve geri savur
        IDamageable iDamageable = other.gameObject.GetComponent<IDamageable>();
        iDamageable?.TakeDamage(_fireDirection, _damageAmount, _knockBackThrust);

        // Mermiyi havuza geri gönder
        _gun.ReleaseBulletFromPool(this);
    }
}
