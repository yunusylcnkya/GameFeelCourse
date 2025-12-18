using System;
using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public Action OnKnockbackStart;
    public Action OnKnockbackEnd;

    [SerializeField] private float _knockBackTime = .2f;

    private Vector3 _hitDirection;
    private float _knockBackThrust;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        OnKnockbackStart += ApplyKnockbackForce;
        OnKnockbackStart += StopKnockRoutine;
    }
    void OnDisable()
    {
        OnKnockbackStart -= ApplyKnockbackForce;
        OnKnockbackStart -= StopKnockRoutine;
    }
    public void GetKnockedBack(Vector3 hitDirection, float knockBackThrust)
    {
        Debug.Log("knockeed");
        _hitDirection = hitDirection;
        _knockBackThrust = knockBackThrust;

        OnKnockbackStart?.Invoke();
    }

    private void ApplyKnockbackForce()
    {
        Vector3 difference = (transform.position - _hitDirection).normalized * _knockBackThrust * _rigidBody.mass;
        _rigidBody.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(_knockBackTime);
        OnKnockbackEnd?.Invoke();
    }
    private void StopKnockRoutine()
    {
        _rigidBody.linearVelocity = Vector2.zero;
    }
}
