using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
--------------------
Bir karakter ÖLDÜĞÜNDE 💀

1️⃣ Yere kan / boya izi bırakır (Splatter)
2️⃣ Patlama / parçacık efekti oynatır (VFX)

Bunu kendisi kontrol etmez,
Health scriptinden gelen "öldü" haberini dinler 📡
*/
public class DeathSplatterHandler : MonoBehaviour
{
    /*
    AKTİF OLDUĞUNDA:
    ---------------
    Health.OnDeath olayına abone olur.

    Yani:
    "Biri ölürse bana haber ver"
    */
    void OnEnable()
    {
        Health.OnDeath += SpawnDeathSplatterPrefab;
        Health.OnDeath += SpawnDeathVFX;
    }

    /*
    KAPANDIĞINDA:
    -------------
    Olaydan çıkılır.
    (Yoksa hata + hafıza sızıntısı olur)
    */
    void OnDisable()
    {
        Health.OnDeath -= SpawnDeathSplatterPrefab;
        Health.OnDeath -= SpawnDeathVFX;
    }

    /*
    SPLATTER (KAN / BOYA İZİ):
    ------------------------
    Ölen objenin altına
    bir sprite bırakır.
    */
    private void SpawnDeathSplatterPrefab(Health sender)
    {
        // Splatter prefabını, ölen objenin olduğu yerde oluştur
        GameObject newSplatterPrefab =
            Instantiate(sender.SplatterPrefab, sender.transform.position, transform.rotation);

        // Splatter üzerindeki SpriteRenderer'ı al
        SpriteRenderer deathSplatterSpriteRenderer =
            newSplatterPrefab.GetComponent<SpriteRenderer>();

        // Ölen objenin rengini alabilmek için ColorChanger bak
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();

        /*
        Eğer ölen objenin bir rengi varsa:
        - Splatter da aynı renkte olsun 🎨
        */
        if (colorChanger)
        {
            Color currentColor = colorChanger.DefaultColor;
            deathSplatterSpriteRenderer.color = currentColor;

            // Splatter bu objeye bağlı kalsın
            newSplatterPrefab.transform.SetParent(this.transform);
        }
    }

    /*
    VFX (PARÇACIK EFEKTİ):
    ---------------------
    Patlama, parçalanma gibi
    görsel efektleri oynatır ✨
    */
    private void SpawnDeathVFX(Health sender)
    {
        // Death VFX prefabını oluştur
        GameObject deathVFX =
            Instantiate(sender.DeathVFX, sender.transform.position, transform.rotation);

        // Particle System ayarlarına eriş
        ParticleSystem.MainModule ps =
            deathVFX.GetComponent<ParticleSystem>().main;

        // Renk bilgisini almak için ColorChanger bak
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();

        /*
        Eğer ölen objenin rengi varsa:
        - VFX de aynı renkte olsun
        */
        if (colorChanger)
        {
            Color currentColor = colorChanger.DefaultColor;
            ps.startColor = currentColor;
        }

        // VFX bu objeye bağlı kalsın
        deathVFX.transform.SetParent(this.transform);
    }
}
