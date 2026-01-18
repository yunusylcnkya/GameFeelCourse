using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu script bir objenin rengini değiştirir.

- Normal rengi ne?
- Rengi değiştir
- Rastgele renk seç

gibi işleri yapar.
*/

public class ColorChanger : MonoBehaviour
{
    /*
    DEFAULTCOLOR NE?
    ----------------
    Objeye ait "asıl" renk.
    Flash bitince bu renge geri dönülür.

    Dışarıdan okunabilir,
    ama sadece bu script değiştirebilir.
    */
    public Color DefaultColor { get; private set; }

    /*
    RENK LİSTESİ
    ------------
    Rastgele seçilecek renkler
    */
    [SerializeField] private Color[] _colors;

    /*
    RENK DEĞİŞTİRİLECEK SPRITE
    -------------------------
    Ekranda görünen resim
    */
    [SerializeField] private SpriteRenderer _fillSpriteRenderer;

    /*
    VARSAYILAN RENGİ AYARLAR
    -----------------------
    Bu rengi kaydeder ve hemen uygular
    */
    public void SetDefaultColor(Color color)
    {
        DefaultColor = color;
        SetColor(color);
    }

    /*
    RENK DEĞİŞTİRİR
    ---------------
    Sprite’ın rengini direkt değiştirir
    */
    public void SetColor(Color color)
    {
        _fillSpriteRenderer.color = color;
    }

    /*
    RASTGELE RENK SEÇER
    ------------------
    Renk listesinden bir tane seçer
    ve onu yeni varsayılan renk yapar
    */
    public void SetRandomColor()
    {
        int randomNum = Random.Range(0, _colors.Length);
        DefaultColor = _colors[randomNum];
        _fillSpriteRenderer.color = DefaultColor;
    }
}
