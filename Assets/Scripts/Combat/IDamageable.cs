using UnityEngine;

/*
BU DOSYA NE?
------------
Bu bir "interface"tir.
Interface = KURAL LİSTESİ

Yani diyor ki:
"Bunu kullanan herkes şu işi yapabilmeli!"
*/

public interface IDamageable : IHitable
{
    /*
    BU METOT NE DEMEK?
    ------------------
    Bu interface’i kullanan bir obje:

    - Hasar alabilmeli
    - Hasarın nereden geldiğini bilmeli
    - Geriye doğru savrulabilmeli

    PARAMETRELER:
    -------------
    damageSourceDir :
    Hasarın geldiği yön (sağdan mı soldan mı?)

    _damageAmount :
    Ne kadar can gidecek?

    _knockBackThrust :
    Ne kadar sert geriye fırlatılacak?
    */
    void TakeDamage(Vector2 damageSourceDir, int _damageAmount, float _knockBackThrust);
}
