using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private PlayerInputActions _playerInputActions;
    private InputAction _move, _jump, _jetpack, _grenade;

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _move = _playerInputActions.Player.Move;
        _jump = _playerInputActions.Player.Jump;
        _jetpack = _playerInputActions.Player.Jetpack;
        _grenade = _playerInputActions.Player.Grenade;
    }

    void OnEnable()
    {
        _playerInputActions.Enable();
    }
    void OnDisable()
    {
        _playerInputActions.Disable();
    }

    void Update()
    {
        FrameInput = GatherInput();
    }


    private FrameInput GatherInput()
    {
        return new FrameInput
        {
            Move = _move.ReadValue<Vector2>(),
            Jump = _jump.WasPressedThisFrame(),
            Jetpack = _jetpack.WasPressedThisFrame(),
            Grenade = _grenade.WasPressedThisFrame()
        };

    }

}

public struct FrameInput
{
    public Vector2 Move;
    public bool Jump;
    public bool Jetpack;
    public bool Grenade;
}
