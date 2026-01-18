using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, bir karakterin veya düşmanın hareket etmesini sağlar:

- X yönünde hareket eder
- Knockback (geriye tepme) sırasında hareketi durdurur
- Hareket hızı ve yönünü ayarlayabilir
*/

public class Movement : MonoBehaviour
{
    public bool CanMove => _canMove; // Başka scriptler buradan karakterin hareket edip edemediğini kontrol edebilir

    [SerializeField] private float _moveSpeed = 10f; // Hareket hızı

    private float _moveX; // Şu anki yatay yön
    private bool _canMove = true; // Karakterin hareket edip edemeyeceği
    private Knockback _knockback; // Knockback script referansı
    private Rigidbody2D _rigidBody; // Fizik hareketi

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    void OnEnable()
    {
        _knockback.OnKnockbackStart += CanMoveFalse; // Knockback başlarsa hareketi durdur
        _knockback.OnKnockbackEnd += CanMoveTrue;   // Knockback bitince hareketi aç
    }

    void OnDisable()
    {
        _knockback.OnKnockbackStart -= CanMoveFalse;
        _knockback.OnKnockbackEnd -= CanMoveTrue;
    }

    private void CanMoveTrue()
    {
        _canMove = true; // Hareket açıldı
    }

    private void CanMoveFalse()
    {
        _canMove = false; // Hareket kapandı
    }

    void FixedUpdate()
    {
        Move(); // Her fizik frame’de hareket et
    }

    // Dışarıdan hareket yönünü belirle
    public void SetCurrentDirection(float currentDirection)
    {
        _moveX = currentDirection;
    }

    // Hareket işlemi
    private void Move()
    {
        if (!_canMove) { return; } // Hareket kapalıysa çık

        // Yatay hız = yön * hız, dikey hız = Rigidbody’nin mevcut dikey hızı
        Vector2 movement = new Vector2(_moveX * _moveSpeed, _rigidBody.linearVelocityY);
        _rigidBody.linearVelocity = movement; // Rigidbody’ye uygula
    }
}
