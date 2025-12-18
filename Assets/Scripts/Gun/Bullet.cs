using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletVFX;
    [SerializeField] private float _moveSpeed = 10f;     // Merminin hareket hızı
    [SerializeField] private int _damageAmount = 1;      // Merminin vereceği hasar miktarı
    [SerializeField] private float _knockBackThrust = 100f;


    private Vector2 _fireDirection;                      // Merminin ilerleyeceği yön
    private Rigidbody2D _rigidBody;                      // Fizik bileşeni
    private Gun _gun;                                    // Mermiyi havuza geri göndermek için Gun referansı

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidBody.linearVelocity = _fireDirection * _moveSpeed;
    }

    public void Init(Gun gun, Vector2 bulletSpawnPos, Vector2 mousePos)
    {
        _gun = gun;                      // Havuz kontrolü için Gun referansı
        transform.position = bulletSpawnPos; // Mermiye başlangıç konumu verilir

        // Ateş yönü = mouse world pozisyonu - mermi çıkış pozisyonu
        // normalized → yön vektörü uzunluğunu 1 yapar (sabit hız için)
        _fireDirection = (mousePos - bulletSpawnPos).normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Instantiate(_bulletVFX, transform.position, Quaternion.identity);


        IHitable iHitable = other.gameObject.GetComponent<IHitable>();
        iHitable?.TakeHit();

        IDamageable iDamageable = other.gameObject.GetComponent<IDamageable>();
        iDamageable?.TakeDamage(_fireDirection, _damageAmount, _knockBackThrust);

        _gun.ReleaseBulletFromPool(this);
    }
}
