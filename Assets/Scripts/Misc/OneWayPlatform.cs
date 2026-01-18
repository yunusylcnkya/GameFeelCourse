using System.Collections;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, oyun içinde "tek yönlü platform"ları yönetir:

- Oyuncu platforma çıkabilir
- Yukarıdan atlarken üzerinde durabilir
- Aşağı tuşuna basarsa platformun içinden geçebilir
*/

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private float _disableColliderTime = 1f; // Collider kapalı kalma süresi

    private bool _playerOnPlatform = false; // Oyuncu platformda mı?
    private Collider2D _collider;          // Platformun kendi collider'ı

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>(); // Platform collider'ını al
    }

    void Update()
    {
        DetectPlayerInput(); // Oyuncu aşağı basarsa kontrol et
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            _playerOnPlatform = true; // Oyuncu platforma değdi
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            _playerOnPlatform = false; // Oyuncu platformdan ayrıldı
        }
    }

    // Oyuncu aşağı bastıysa collider'ı geçici olarak devre dışı bırak
    private void DetectPlayerInput()
    {
        if (!_playerOnPlatform) return; // Platformda değilse çık
        if (PlayerController.Instance.MoveInput.y < 0f) // Aşağı tuşu basıldıysa
        {
            StartCoroutine(DisablePlatformColliderRoutine());
        }
    }

    // Platformun collider'ını geçici kapatır, böylece oyuncu içinden geçebilir
    private IEnumerator DisablePlatformColliderRoutine()
    {
        Collider2D[] playerColliders = PlayerController.Instance.GetComponents<Collider2D>();
        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, true); // Geçici olarak çarpışmayı kapat
        }

        yield return new WaitForSeconds(_disableColliderTime); // Bekle

        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, false); // Çarpışmayı geri aç
        }
    }
}
