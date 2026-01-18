/*
BU DOSYA NE?
------------
Bu bir "interface"tir.
Yani bir KURAL defteri gibidir 📘

Diyor ki:
"Benimle çalışan herkes şu işi yapabilmeli!"
*/

public interface IHitable
{
    /*
    TAKEHIT NE DEMEK?
    -----------------
    Bu kuralı kullanan her obje:

    - Bir darbe aldığını anlayabilmeli
    - Vurulduğunda tepki verebilmeli

    Mesela:
    - Beyaz yanıp sönmek
    - Ses çıkarmak
    - Titremek

    Bu fonksiyon sadece "vuruldum" demek içindir.
    */
    void TakeHit();
}
