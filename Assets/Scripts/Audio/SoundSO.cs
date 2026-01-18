using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu script TEK BİR SESİN KARTI gibidir 🎴

Yani:
- Bir silah sesi
- Bir zıplama sesi
- Bir müzik

her biri için ayrı ayrı kullanılan
bir "ses tanımı"dır.

Bu ScriptableObject sayesinde:
- Sesler sahnede dağınık olmaz
- Ayarlar tek yerden yapılır
- AudioManager sesleri kolayca çalar
*/
[CreateAssetMenu()]
public class SoundSO : ScriptableObject
{
    /*
    SES TİPLERİ:
    ------------
    Bu ses bir:
    - Efekt mi? (SFX)
    - Müzik mi? (Music)
    */
    public enum AudioTypes
    {
        SFX,
        Music
    }

    /*
    TEMEL SES BİLGİLERİ:
    -------------------
    */
    public AudioTypes AudioType; // Ses türü
    public AudioClip Clip;       // Asıl ses dosyası

    /*
    LOOP:
    -----
    Açık olursa ses sürekli tekrar eder.
    (Örnek: arka plan müziği)
    */
    public bool Loop = false;

    /*
    RANDOM PITCH:
    -------------
    Açık olursa ses her seferinde
    biraz farklı tonda çalar.
    Aynı ses sıkıcı olmaz 🙂
    */
    public bool RandomizePitch = false;

    /*
    RANDOM PITCH MİKTARI:
    --------------------
    Ses ne kadar farklı çalabilir?
    */
    [Range(0f, 1f)]
    public float RandomizePitchRangeModifier = 0.1f;

    /*
    SES YÜKSEKLİĞİ (VOLUME):
    -----------------------
    Ses ne kadar yüksek?
    */
    [Range(0.1f, 2f)]
    public float Volume = 1f;

    /*
    SES TONU (PITCH):
    -----------------
    Ses ince mi, kalın mı?
    */
    [Range(0.1f, 3f)]
    public float Pitch = 1f;
}
