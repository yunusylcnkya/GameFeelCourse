using UnityEngine;


[CreateAssetMenu()]

public class SoundsCollectionSO : ScriptableObject
{

    [Header("Music")]
    public SoundSO[] FightMusic;
    public SoundSO[] DiscoPartMusic;

    [Header("SFX")]
    public SoundSO[] GunShoot;
    public SoundSO[] Jump;
    public SoundSO[] Splat;
    public SoundSO[] Jetpack;
    public SoundSO[] GrenadeShoot;
    public SoundSO[] GrenadeExplode;
    public SoundSO[] GrenadeBeep;
    public SoundSO[] PlayerHit;
    public SoundSO[] MegaKill;

}
