using System.Collections;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu kod, bir oyun karakteri hasar aldığında
kısa bir an için BEYAZ PARLAMASINI sağlar.
Yani karakter "çak!" diye beyaz olur, sonra eski haline döner.
*/

public class Flash : MonoBehaviour
{
    /*
    BU DEĞİŞKENLER NE?
    -----------------
    _defaultMaterial : Karakterin normal (eski) görüntüsü
    _whiteFlashMaterial : Beyaz parlamada kullanılacak görüntü
    _flashTime : Beyaz kalacağı süre (saniye cinsinden)
    */
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _whiteFlashMaterial;
    [SerializeField] private float _flashTime = 0.1f;

    /*
    Karakterin ve çocuklarının (alt objelerinin)
    SpriteRenderer’larını tutar.
    SpriteRenderer = ekrana resmi çizen parça
    */
    private SpriteRenderer[] _spriteRenderers;

    /*
    Renk değiştirmek için kullanılan başka bir script
    (Eğer varsa kullanılır, yoksa sorun olmaz)
    */
    private ColorChanger _colorChanger;

    /*
    AWAKE NE ZAMAN ÇALIŞIR?
    ----------------------
    Oyun başlar başlamaz,
    bu obje sahneye geldiği anda çalışır.
    */
    void Awake()
    {
        // Bu objenin içindeki TÜM sprite’ları bulur
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // Eğer ColorChanger script’i varsa alır
        _colorChanger = GetComponent<ColorChanger>();
    }

    /*
    BU METOT NE YAPAR?
    -----------------
    Dışarıdan çağrılır.
    Mesela: düşman vurduğunda StartFlash() denir.
    */
    public void StartFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    /*
    FLASHROUTINE NE?
    ----------------
    Bu bir "korutin"dir.
    Yani:
    - Bir şey yap
    - Biraz bekle
    - Sonra devam et
    */
    private IEnumerator FlashRoutine()
    {
        // Tüm sprite’ları dolaş
        foreach (var sr in _spriteRenderers)
        {
            // Sprite’ı beyaz yap
            sr.material = _whiteFlashMaterial;

            // Eğer ColorChanger varsa rengi beyaz yap
            if (_colorChanger)
            {
                _colorChanger.SetColor(Color.white);
            }
        }

        // Belirlenen süre kadar bekle
        yield return new WaitForSeconds(_flashTime);

        // Eski haline geri döndür
        SetDefaultMaterial();
    }

    /*
    BU METOT NE YAPAR?
    -----------------
    Karakteri eski görüntüsüne geri çevirir
    */
    private void SetDefaultMaterial()
    {
        foreach (var sr in _spriteRenderers)
        {
            // Normal materyale geri dön
            sr.material = _defaultMaterial;

            // Eğer ColorChanger varsa eski rengi geri ver
            if (_colorChanger)
            {
                _colorChanger.SetColor(_colorChanger.DefaultColor);
            }
        }
    }
}
