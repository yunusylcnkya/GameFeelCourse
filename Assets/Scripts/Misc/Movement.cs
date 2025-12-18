using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool CanMove => _canMove;

    [SerializeField] private float _moveSpeed = 10f;

    private float _moveX;
    private bool _canMove = true;
    private Knockback _knockback;
    private Rigidbody2D _rigidBody;


    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    void OnEnable()
    {
        _knockback.OnKnockbackStart += CanMoveFalse;
        _knockback.OnKnockbackEnd += CanMoveTrue;

    }
    void OnDisable()
    {
        _knockback.OnKnockbackStart -= CanMoveFalse;
        _knockback.OnKnockbackEnd -= CanMoveTrue;

    }

    private void CanMoveTrue()
    {
        _canMove = true;
    }

    private void CanMoveFalse()
    {
        _canMove = false;
    }



    void FixedUpdate()
    {
        Move();
    }

    public void SetCurrentDirection(float currentDirection)
    {
        _moveX = currentDirection;
    }
    private void Move()
    {
        if (!_canMove) { return; }

        Vector2 movement = new Vector2(_moveX * _moveSpeed, _rigidBody.linearVelocityY);
        _rigidBody.linearVelocity = movement;
    }
}
