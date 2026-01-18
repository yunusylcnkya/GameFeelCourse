using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, oyun içi ekranı siyah yapıp açarak "fade in/out" efekti verir
ve oyuncu öldüğünde veya yenilendiğinde onu belirlenen noktada tekrar doğurur.
*/

[RequireComponent(typeof(Image))]
public class Fade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeTime = 1.4f; // Fade süresi

    [Header("Respawn Settings")]
    [SerializeField] private GameObject playerPrefab; // Yeniden doğacak oyuncu prefab'ı
    [SerializeField] private Transform respawnPoint; // Doğma noktası

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCam; // Kamera

    private Image image; // Ekranı kaplayan UI Image
    private Coroutine fadeCoroutine;
    private Transform currentPlayer; // Şu anki oyuncu

    // Başlangıçta gerekli referansları al
    private void Awake()
    {
        image = GetComponent<Image>();

        if (virtualCam == null)
            virtualCam = FindFirstObjectByType<CinemachineVirtualCamera>();
    }

    // Fade başlatmak için çağrılır
    public void FadeInAndOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine); // Önceki fade durdur

        fadeCoroutine = StartCoroutine(FadeSequence());
    }

    // Fade işlemi ve respawn sırası
    private IEnumerator FadeSequence()
    {
        yield return FadeRoutine(1f); // Ekranı siyah yap
        Respawn();                    // Oyuncuyu tekrar doğur
        yield return FadeRoutine(0f); // Ekranı tekrar aç

        fadeCoroutine = null;
    }

    // Belirli bir alfa değerine fade yap
    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float elapsed = 0f;
        Color color = image.color;
        float startAlpha = color.a;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeTime); // Yumuşak geçiş
            image.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        image.color = color;
    }

    // Oyuncuyu doğur
    private void Respawn()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer.gameObject); // Eski oyuncuyu sil

        // Yeni oyuncuyu spawn noktasında oluştur
        currentPlayer = Instantiate(
            playerPrefab,
            respawnPoint.position,
            Quaternion.identity
        ).transform;

        // Kamera yeni oyuncuyu takip etsin
        if (virtualCam != null)
            virtualCam.Follow = currentPlayer;
    }
}
