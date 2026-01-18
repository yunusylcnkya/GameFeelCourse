using System;
using System.Collections;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu script, bir karakter vurulduğunda
GERİYE DOĞRU FIRLATILMASINI sağlar.

Yani:
👊 Vurulursun
➡️ Geri savrulursun
⏱️ Kısa süre sonra durursun
*/

public class Knockback : MonoBehaviour
{
    /*
    BU İKİ OLAY (Action) NE?
    -----------------------
    OnKnockbackStart : Geri savrulma başladığında çalışır
    OnKnockbackEnd   : Geri savrulma bittiğinde çalışır

    Başka script’ler de bunları dinleyebilir
    */
    public Action OnKnockbackStart;
    public Action OnKnockbackEnd;

    /*
    GERİ SAVRULMA SÜRESİ
    -------------------
    Ne kadar süre sonra duracak? (saniye)
    */
    [SerializeField] private float _knockBackTime = .2f;

    /*
    VURUŞ BİLGİLERİ
    ---------------
    _hitDirection : Darbenin geldiği yer
    _knockBackThrust : Ne kadar sert itilecek
    */
    private Vector3 _hitDirection;
    private float _knockBackThrust;

    /*
    RIGIDBODY2D
    -----------
    Fizik kurallarıyla hareket etmek için kullanılır
    */
    private Rigidbody2D _rigidBody;

    /*
    AWAKE
    -----
    Oyun başlarken Rigidbody2D’yi bulur
    */
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    /*
    ONENABLE
    --------
    Script aktif olunca çalışır.
    Geri savrulma başlayınca hangi fonksiyonlar çalışacak,
    burada söylenir.
    */
    void OnEnable()
    {
        OnKnockbackStart += ApplyKnockbackForce;
        OnKnockbackStart += StopKnockRoutine;
    }

    /*
    ONDISABLE
    ---------
    Script kapanınca çalışır.
    Bağlanan fonksiyonlar temizlenir.
    */
    void OnDisable()
    {
        OnKnockbackStart -= ApplyKnockbackForce;
        OnKnockbackStart -= StopKnockRoutine;
    }

    /*
    BU METOT NE YAPAR?
    -----------------
    Dışarıdan çağrılır.

    Mesela:
    Enemy vurdu → Player geri savrulsun
    */
    public void GetKnockedBack(Vector3 hitDirection, float knockBackThrust)
    {
        Debug.Log("knocked");

        // Darbenin geldiği yeri kaydet
        _hitDirection = hitDirection;

        // Ne kadar sert itileceğini kaydet
        _knockBackThrust = knockBackThrust;

        // Geri savrulma başlasın
        OnKnockbackStart?.Invoke();
    }

    /*
    GERİ SAVRULMA KUVVETİ
    --------------------
    Fiziksel olarak objeyi iter
    */
    private void ApplyKnockbackForce()
    {
        // Vurulan yerden ters yöne doğru bir kuvvet hesaplanır
        Vector3 difference =
            (transform.position - _hitDirection).normalized
            * _knockBackThrust
            * _rigidBody.mass;

        // O kuvvet aniden uygulanır
        _rigidBody.AddForce(difference, ForceMode2D.Impulse);

        // Süreyi başlat
        StartCoroutine(KnockRoutine());
    }

    /*
    GERİ SAVRULMA SÜRESİ
    -------------------
    Biraz bekler, sonra bittiğini söyler
    */
    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(_knockBackTime);

        // Geri savrulma bitti
        OnKnockbackEnd?.Invoke();
    }

    /*
    HAREKETİ DURDURUR
    -----------------
    Hız sıfırlanır, obje durur
    */
    private void StopKnockRoutine()
    {
        _rigidBody.linearVelocity = Vector2.zero;
    }
}
