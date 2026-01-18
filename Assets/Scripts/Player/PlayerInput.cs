using UnityEngine;
using UnityEngine.InputSystem;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu script oyuncunun KUMANDASIDIR 🎮

Yani:
- Klavye / gamepad tuşlarına bakar
- Hangi tuşa basıldığını anlar
- Bu bilgileri tek bir paket haline getirir
- Diğer scriptlere (PlayerController gibi) verir

Bu script HAREKET YAPMAZ,
sadece "oyuncu neye bastı?" diye söyler.
*/
public class PlayerInput : MonoBehaviour
{
    /*
    FRAME INPUT:
    ------------
    Bu, o anki tuş bilgilerini tutan kutudur.
    Diğer scriptler sadece buraya bakar.
    */
    public FrameInput FrameInput { get; private set; }

    /*
    INPUT SYSTEM NESNELERİ:
    ----------------------
    Unity'nin yeni Input System'i kullanılıyor.
    */
    private PlayerInputActions _playerInputActions;
    private InputAction _move, _jump, _jetpack, _grenade;

    /*
    AWAKE:
    ------
    Oyun başlarken çalışır.
    Hangi tuş ne işe yarıyor burada hazırlanır.
    */
    void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        // Hareket (sağa / sola)
        _move = _playerInputActions.Player.Move;

        // Zıplama tuşu
        _jump = _playerInputActions.Player.Jump;

        // Jetpack tuşu
        _jetpack = _playerInputActions.Player.Jetpack;

        // El bombası tuşu
        _grenade = _playerInputActions.Player.Grenade;
    }

    /*
    ONENABLE / ONDISABLE:
    --------------------
    Script aktifken tuşları dinler,
    kapalıyken dinlemez.
    */
    void OnEnable()
    {
        _playerInputActions.Enable();
    }

    void OnDisable()
    {
        _playerInputActions.Disable();
    }

    /*
    UPDATE:
    -------
    Her kare çalışır.
    O an basılan tuşları alır.
    */
    void Update()
    {
        FrameInput = GatherInput();
    }

    /*
    INPUT TOPLAMA:
    --------------
    Tüm tuşlar okunur
    ve tek bir paket haline getirilir.
    */
    private FrameInput GatherInput()
    {
        return new FrameInput
        {
            // Sağa - sola hareket (joystick veya A-D)
            Move = _move.ReadValue<Vector2>(),

            // Bu karede zıplama tuşuna basıldı mı?
            Jump = _jump.WasPressedThisFrame(),

            // Bu karede jetpack tuşuna basıldı mı?
            Jetpack = _jetpack.WasPressedThisFrame(),

            // Bu karede bomba tuşuna basıldı mı?
            Grenade = _grenade.WasPressedThisFrame()
        };
    }
}

/*
FRAME INPUT NE?
---------------
Bu bir veri kutusu 📦

İçinde:
- Hareket yönü
- Zıpladı mı
- Jetpack bastı mı
- Bomba bastı mı

bilgileri vardır.
*/
public struct FrameInput
{
    public Vector2 Move;
    public bool Jump;
    public bool Jetpack;
    public bool Grenade;
}
