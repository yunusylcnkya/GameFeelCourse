using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bu script oyundaki TÜM SESLERİN PATRONU 🎧

Yani:
- Silah sesi
- Zıplama sesi
- Jetpack sesi
- Patlama sesi
- Müzikler
- Mega kill gibi özel sesler

Hepsini TEK YERDEN kontrol eder.

Bu script sayesinde:
- Aynı anda iki müzik çalmaz
- Sesler rastgele seçilebilir
- Ses yüksekliği ayarlanabilir
*/
public class AudioManager : MonoBehaviour
{
    // Oyunda sadece 1 tane AudioManager olsun diye
    public static AudioManager Instance;

    /*
    GENEL SES AYARLARI:
    ------------------
    Master volume = bütün seslerin genel sesi
    */
    [Range(0f, 2f)]
    [SerializeField] private float _masterVolume = 1f;

    /*
    SES KOLEKSİYONU:
    ----------------
    Bütün sesler ScriptableObject içinde durur.
    */
    [SerializeField] private SoundsCollectionSO _soundCollectionSO;

    /*
    SES GRUPLARI:
    -------------
    - SFX : efekt sesleri
    - Music : müzikler
    */
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;

    // Şu anda çalan müzik
    private AudioSource _currentMusic;

    #region UNITY METHODS

    /*
    AWAKE:
    ------
    Oyun başlarken çalışır.
    AudioManager'ı hazırlar.
    */
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /*
    START:
    ------
    Oyun başlar başlamaz
    dövüş müziğini çalar.
    */
    private void Start()
    {
        FightMusic();
    }

    /*
    EVENT BAĞLANTILARI:
    ------------------
    Oyunda bir şey olunca
    ses çalması için dinler.
    */
    void OnEnable()
    {
        Gun.OnShoot += Gun_OnShoot;
        Gun.OnGrenadeShoot += Grenade_OnGrenadeShoot;
        PlayerController.OnJump += PlayerController_OnJump;
        Health.OnDeath += HandleDeath;
        DiscoBallManager.OnDiscoBallHitEvent += DiscoBallMusic;
        PlayerController.OnJetpack += PlayerController_OnJetpack;
    }

    void OnDisable()
    {
        Gun.OnShoot -= Gun_OnShoot;
        Gun.OnGrenadeShoot -= Grenade_OnGrenadeShoot;
        PlayerController.OnJump -= PlayerController_OnJump;
        Health.OnDeath -= HandleDeath;
        DiscoBallManager.OnDiscoBallHitEvent -= DiscoBallMusic;
        PlayerController.OnJetpack -= PlayerController_OnJetpack;
    }

    #endregion


    #region SOUND SYSTEM (GENEL SES MANTIĞI)

    /*
    RASTGELE SES:
    -------------
    Aynı türden birden fazla ses varsa
    içlerinden birini rastgele seçer.
    */
    private void PlayRandomSound(SoundSO[] sounds)
    {
        if (sounds != null && sounds.Length > 0)
        {
            SoundSO soundSO = sounds[Random.Range(0, sounds.Length)];
            SoundToPlay(soundSO);
        }
    }

    /*
    SESİ HAZIRLAMA:
    ---------------
    Sesin:
    - yüksekliği
    - tonu
    - loop olup olmadığı
    ayarlanır.
    */
    private void SoundToPlay(SoundSO soundSO)
    {
        AudioClip clip = soundSO.Clip;
        float pitch = soundSO.Pitch;
        float volume = soundSO.Volume * _masterVolume;
        bool loop = soundSO.Loop;

        pitch = RandomizePitch(soundSO, pitch);
        AudioMixerGroup audioMixerGroup = DetermineAudioMixerGroup(soundSO);

        PlaySound(clip, pitch, volume, loop, audioMixerGroup);
    }

    /*
    SES GRUBU SEÇME:
    ----------------
    Ses müzik mi, efekt mi?
    */
    private AudioMixerGroup DetermineAudioMixerGroup(SoundSO soundSO)
    {
        switch (soundSO.AudioType)
        {
            case SoundSO.AudioTypes.SFX:
                return _sfxMixerGroup;

            case SoundSO.AudioTypes.Music:
                return _musicMixerGroup;

            default:
                return null;
        }
    }

    /*
    SES TONU RASTGELE:
    -----------------
    Aynı ses her seferinde
    biraz farklı çalsın diye.
    */
    private float RandomizePitch(SoundSO soundSO, float pitch)
    {
        if (soundSO.RandomizePitch)
        {
            float random =
                Random.Range(-soundSO.RandomizePitchRangeModifier,
                              soundSO.RandomizePitchRangeModifier);
            pitch += random;
        }
        return pitch;
    }

    /*
    SESİ ÇALMA:
    -----------
    Geçici bir GameObject oluşturur
    ve sesi çalar.
    */
    private void PlaySound(AudioClip clip, float pitch, float volume, bool loop, AudioMixerGroup audioMixerGroup)
    {
        GameObject soundObject = new GameObject("Temp Audio Source");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.Play();

        if (!loop)
        {
            Destroy(soundObject, clip.length);
        }

        DetermineMusic(audioMixerGroup, audioSource);
    }

    /*
    MÜZİK KONTROLÜ:
    ---------------
    Yeni müzik başlayınca
    eskisi durdurulur.
    */
    private void DetermineMusic(AudioMixerGroup audioMixerGroup, AudioSource audioSource)
    {
        if (audioMixerGroup == _musicMixerGroup)
        {
            if (_currentMusic != null)
            {
                _currentMusic.Stop();
            }
            _currentMusic = audioSource;
        }
    }

    #endregion


    #region SFX (EFEKT SESLERİ)

    private void Gun_OnShoot()
    {
        PlayRandomSound(_soundCollectionSO.GunShoot);
    }

    private void PlayerController_OnJump()
    {
        PlayRandomSound(_soundCollectionSO.Jump);
    }

    private void Health_OnDeath()
    {
        PlayRandomSound(_soundCollectionSO.Splat);
    }

    private void PlayerController_OnJetpack()
    {
        PlayRandomSound(_soundCollectionSO.Jetpack);
    }

    private void Grenade_OnGrenadeShoot()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeShoot);
    }

    public void Grenade_OnBeep()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeBeep);
    }

    public void Grenade_OnExplode()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeExplode);
    }

    public void Enemy_OnPlayerHit()
    {
        PlayRandomSound(_soundCollectionSO.PlayerHit);
    }

    private void AudioManager_MegaKill()
    {
        PlayRandomSound(_soundCollectionSO.MegaKill);
    }

    #endregion


    #region MUSIC

    /*
    NORMAL OYUN MÜZİĞİ
    */
    private void FightMusic()
    {
        PlayRandomSound(_soundCollectionSO.FightMusic);
    }

    /*
    DİSCO BALL MÜZİĞİ:
    ------------------
    Disco bitince tekrar
    normal müziğe döner.
    */
    private void DiscoBallMusic()
    {
        PlayRandomSound(_soundCollectionSO.DiscoPartMusic);
        float length = _soundCollectionSO.DiscoPartMusic[0].Clip.length;
        Utils.RunAfterDelay(this, length, FightMusic);
    }

    #endregion


    #region ÖZEL SES MANTIĞI (MEGA KILL)

    /*
    KISA SÜREDE ÇOK ÖLDÜRME:
    -----------------------
    Eğer kısa sürede
    çok düşman ölürse
    özel ses çalar.
    */
    private List<Health> _deathList = new List<Health>();
    private Coroutine _deathRoutine;

    private void HandleDeath(Health health)
    {
        bool isEnemy = health.GetComponent<Enemy>();
        if (isEnemy)
        {
            _deathList.Add(health);
        }

        if (_deathRoutine == null)
        {
            _deathRoutine = StartCoroutine(DeatWindowRoutine());
        }
    }

    private IEnumerator DeatWindowRoutine()
    {
        yield return null;

        int megaKillAmount = 3;

        if (_deathList.Count >= megaKillAmount)
        {
            AudioManager_MegaKill();
        }

        Health_OnDeath();
        _deathList.Clear();
        _deathRoutine = null;
    }

    #endregion
}
