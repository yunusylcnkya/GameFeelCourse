
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private ParticleSystem _moveDustVFX;
    [SerializeField] private ParticleSystem _poofDustVFX;
    [SerializeField] private float _tiltAngle = 20f;
    [SerializeField] private float _tiltSpeed = 4f;
    [SerializeField] private Transform _characterSpriteTransform;
    [SerializeField] private Transform _cowboyHatTransform;
    [SerializeField] private float _cowboyHattiltModifier = 4f;
    [SerializeField] private float _ylandVelocityCheck = -20f;
    private Vector2 _velocityBeforePhysicsUpdate;
    private Rigidbody2D _rigidBody;
    private CinemachineImpulseSource _impulseSource;
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void Update()
    {
        DetectMoveDust();
        ApplyTilt();
    }
    private void FixedUpdate()
    {
        _velocityBeforePhysicsUpdate = _rigidBody.linearVelocity;
    }
    void OnEnable()
    {
        PlayerController.OnJump += PlayPoofDustVFX;
    }

    void OnDisable()
    {
        PlayerController.OnJump -= PlayPoofDustVFX;

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_velocityBeforePhysicsUpdate.y < _ylandVelocityCheck)
        {
            PlayPoofDustVFX();
            _impulseSource.GenerateImpulse();
        }
    }
    private void DetectMoveDust()
    {
        if (!PlayerController.Instance.CheckGrounded())
        {
            if (!_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Play();
            }
            else
            {
                if (_moveDustVFX.isPlaying)
                {
                    _moveDustVFX.Stop();
                }
            }
        }
    }
    private void PlayPoofDustVFX()
    {
        _poofDustVFX.Play();
    }

    private void ApplyTilt()
    {
        float targetAngle;
        if (PlayerController.Instance.MoveInput.x < 0f)
        {
            targetAngle = _tiltAngle;
        }
        else if (PlayerController.Instance.MoveInput.x > 0f)
        {
            targetAngle = -_tiltAngle;
        }
        else
        {
            targetAngle = 0f;
        }

        Quaternion currentCharacterRotation = _characterSpriteTransform.rotation;
        Quaternion targetCharacterRotation =
        Quaternion.Euler(currentCharacterRotation.eulerAngles.x, currentCharacterRotation.eulerAngles.y, targetAngle);

        _characterSpriteTransform.rotation =
        Quaternion.Lerp(currentCharacterRotation, targetCharacterRotation, _tiltSpeed * Time.deltaTime);

        //cowboy hat
        Quaternion currentHatRotation = _cowboyHatTransform.rotation;
        Quaternion targetHatRotation =
        Quaternion.Euler(currentHatRotation.eulerAngles.x, currentHatRotation.eulerAngles.y, -targetAngle / _cowboyHattiltModifier);

        _cowboyHatTransform.rotation =
        Quaternion.Lerp(currentHatRotation, targetHatRotation, _tiltSpeed * _cowboyHattiltModifier * Time.deltaTime);


    }
}
