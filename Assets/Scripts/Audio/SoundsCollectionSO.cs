using UnityEngine;

/*
 * 📦 SES KÜTÜPHANESİ (SOUNDS COLLECTION) 📦
 * -----------------------------------------
 * Bu scripti bir "Ses Çantası" veya "Albüm" gibi düşünebilirsin.
 * İçinde hiçbir ses çalmaz, sadece seslerin nerede durduğunu listeler.
 * * * NEDEN VAR?
 * AudioManager (Ses Yönetmeni), çalacağı sesleri buradan seçer. 
 * Eğer bir silah sesini değiştirmek istersen, tek tek kodlarla uğraşmak yerine
 * bu çantanın içindeki dosyayı değiştirmen yeterli olur.
 */

[CreateAssetMenu()]
public class SoundsCollectionSO : ScriptableObject
{
    /*
     * [MÜZİK BÖLÜMÜ]
     * Burası oyunun radyosu gibidir. 
     * Arka planda çalan uzun şarkıları burada saklarız.
     */
    [Header("Music")]
    public SoundSO[] FightMusic;      // Aksiyon müziği listesi
    public SoundSO[] DiscoPartMusic;  // Disko topu müziği listesi

    /*
     * [EFEKT BÖLÜMÜ (SFX)]
     * Burası kısa ve anlık seslerin listesidir. 
     * Her bir hareket için ayrı bir çekmece gibi düşünebilirsin.
     */
    [Header("SFX")]
    public SoundSO[] GunShoot;        // Ateş etme sesleri
    public SoundSO[] Jump;            // Zıplama sesleri
    public SoundSO[] Splat;           // Düşman patlama sesleri
    public SoundSO[] Jetpack;         // Uçuş sesleri
    public SoundSO[] GrenadeShoot;    // Bomba atma sesleri
    public SoundSO[] GrenadeExplode;  // Bomba patlama sesleri
    public SoundSO[] GrenadeBeep;     // Bomba geri sayım sesleri
    public SoundSO[] PlayerHit;       // Canın yanınca çıkan sesler
    public SoundSO[] MegaKill;        // Çok havalı bir şey yapınca çıkan ses
}