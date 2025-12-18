
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //!Mediator pattern buna daha iyi bir alternatif olabilir araştır!!

    public static AudioManager Instance;

    [Range(0f, 2f)]
    [SerializeField] private float _masterVolume = 1f;
    [SerializeField] private SoundsCollectionSO _soundCollectionSO;

    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;

    private AudioSource _currentMusic;

    #region  Unity Methods

    void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    private void Start()
    {
        FightMusic();
    }
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

    #region  Sound Methods

    private void PlayRandomSound(SoundSO[] sounds)
    {
        if (sounds != null && sounds.Length > 0)
        {
            SoundSO soundSO = sounds[Random.Range(0, sounds.Length)];
            SoundToPlay(soundSO);
        }
    }
    private void SoundToPlay(SoundSO soundSO)
    {
        AudioClip clip = soundSO.Clip;
        float pitch = soundSO.Pitch;
        float volume = soundSO.Volume * _masterVolume;
        bool loop = soundSO.Loop;
        AudioMixerGroup audioMixerGroup;

        pitch = RandomizePitch(soundSO, pitch);

        audioMixerGroup = DetermineAudioMixerGroup(soundSO);
        PlaySound(clip, pitch, volume, loop, audioMixerGroup);
    }

    private AudioMixerGroup DetermineAudioMixerGroup(SoundSO soundSO)
    {
        AudioMixerGroup audioMixerGroup;
        switch (soundSO.AudioType)
        {
            case SoundSO.AudioTypes.SFX:
                audioMixerGroup = _sfxMixerGroup;
                break;
            case SoundSO.AudioTypes.Music:
                audioMixerGroup = _musicMixerGroup;

                break;
            default:
                audioMixerGroup = null;
                break;
        }

        return audioMixerGroup;
    }

    private float RandomizePitch(SoundSO soundSO, float pitch)
    {
        if (soundSO.RandomizePitch)
        {
            float RandomPitchModifier = Random.Range(-soundSO.RandomizePitchRangeModifier, soundSO.RandomizePitchRangeModifier);
            pitch = soundSO.Pitch + RandomPitchModifier;
        }

        return pitch;
    }

    private void PlaySound(AudioClip clip, float pitch, float volume, bool loop, AudioMixerGroup audioMixerGroup)
    {
        GameObject soundObject = new GameObject("Temp Audio source");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.Play();

        if (!loop) { Destroy(soundObject, clip.length); }

        DetermineMusic(audioMixerGroup, audioSource);
    }

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


    #region SFX 
    private void Gun_OnShoot()
    {
        PlayRandomSound(_soundCollectionSO.GunShoot);
    }
    private void PlayerController_OnJump()
    {
        PlayRandomSound(_soundCollectionSO.Jump);
    }
    private void Health_OnDeath(Health health)
    {
        PlayRandomSound(_soundCollectionSO.Splat);
    }
    private void Health_OnDeath()
    {
        PlayRandomSound(_soundCollectionSO.Splat);
    }
    private void PlayerController_OnJetpack()
    {
        PlayRandomSound(_soundCollectionSO.Jetpack);
    }

    public void Grenade_OnBeep()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeBeep);
    }
    public void Grenade_OnExplode()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeExplode);
    }
    private void Grenade_OnGrenadeShoot()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeShoot);
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


    #region  Music

    private void FightMusic()
    {
        PlayRandomSound(_soundCollectionSO.FightMusic);
    }
    private void DiscoBallMusic()
    {
        PlayRandomSound(_soundCollectionSO.DiscoPartMusic);
        float soundLength = _soundCollectionSO.DiscoPartMusic[0].Clip.length;
        Utils.RunAfterDelay(this, soundLength, FightMusic);
    }

    #endregion


    #region Custom SFX Logic

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
