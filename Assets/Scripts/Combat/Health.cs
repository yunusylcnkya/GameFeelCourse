using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu script bir karakterin CANINI yönetir.

- Kaç canı var?
- Hasar alınca ne olur?
- Can biterse ölür mü?
- Ölünce efekt çıkar mı?

Hepsi burada kontrol edilir.
*/

public class Health : MonoBehaviour, IDamageable
{
    /*
    BU İKİ SATIR NE?
    ----------------
    Dışarıdan bu objenin
    - kan sıçraması (Splatter)
    - ölüm efekti (VFX)
    bilgilerine ulaşmamızı sağlar
    */
    public GameObject SplatterPrefab => _splatterPrefab;
    public GameObject DeathVFX => _deathVFX;

    /*
    ONDEATH NE DEMEK?
    -----------------
    Bu karakter öldüğünde,
    başka script’lere "öldü!" diye haber verir.
    */
    public static Action<Health> OnDeath;

    /*
    EDITOR'DEN AYARLANAN ŞEYLER
    ---------------------------
    _splatterPrefab : Kan efekti
    _deathVFX : Ölüm efekti
    _startingHealth : Başlangıç canı
    */
    [SerializeField] private GameObject _splatterPrefab;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private int _startingHealth = 3;

    /*
    DİĞER SCRIPT REFERANSLARI
    -------------------------
    Flash : Beyaz yanıp sönme efekti
    Health : Kendi Health script’i
    Knockback : Geri savrulma
    */
    private Flash _flash;
    private Health _health;
    private Knockback _knockback;

    /*
    ŞU ANKİ CAN
    ----------
    Oyun sırasında azalır
    */
    private int _currentHealth;

    /*
    AWAKE NE YAPAR?
    --------------
    Oyun başlarken,
    bu objenin üzerindeki
    diğer script’leri bulur.
    */
    void Awake()
    {
        _flash = GetComponent<Flash>();
        _health = GetComponent<Health>();
        _knockback = GetComponent<Knockback>();
    }

    /*
    START NE YAPAR?
    ---------------
    Oyun başladıktan sonra çalışır
    ve canı sıfırlar.
    */
    private void Start()
    {
        ResetHealth();
    }

    /*
    CANI SIFIRLAR
    -------------
    Mevcut canı,
    başlangıç canına eşitler.
    */
    public void ResetHealth()
    {
        _currentHealth = _startingHealth;
    }

    /*
    BASİT HASAR ALMA
    ----------------
    Sadece can düşer.
    */
    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        // Eğer can bittiysa
        if (_currentHealth <= 0)
        {
            // "Öldü" diye herkese haber ver
            OnDeath?.Invoke(this);

            // Objeyi yok et
            Destroy(gameObject);
        }
    }

    /*
    GELİŞMİŞ HASAR ALMA
    ------------------
    - Can düşer
    - Geri savrulur
    */
    public void TakeDamage(Vector2 damageSourceDir, int damageAmount, float knockBackThrust)
    {
        // Normal hasar al
        _health.TakeDamage(damageAmount);

        // Geriye doğru fırlat
        _knockback.GetKnockedBack(damageSourceDir, knockBackThrust);
    }

    /*
    DARBE ALINCA NE OLUR?
    --------------------
    Beyaz yanıp sönme başlar
    */
    public void TakeHit()
    {
        _flash.StartFlash();
    }
}
