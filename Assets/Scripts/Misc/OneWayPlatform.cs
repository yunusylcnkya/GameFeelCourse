using System.Collections;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private float _disableColliderTime = 1f;

    private bool _playerOnPlatform = false;
    private Collider2D _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        DetectPlayerInput();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            _playerOnPlatform = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            _playerOnPlatform = false;
        }

    }

    private void DetectPlayerInput()
    {
        if (!_playerOnPlatform) return;
        if (PlayerController.Instance.MoveInput.y < 0f)
        {
            StartCoroutine(DisablePlatformColliderRoutine());
        }
    }

    private IEnumerator DisablePlatformColliderRoutine()
    {
        Collider2D[] playerColliders = PlayerController.Instance.GetComponents<Collider2D>();
        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, true);
        }
        yield return new WaitForSeconds(_disableColliderTime);
        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, false);
        }
    }
}
