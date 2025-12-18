using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeTime = 1.4f;

    [Header("Respawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCam;

    private Image image;
    private Coroutine fadeCoroutine;
    private Transform currentPlayer;

    private void Awake()
    {
        image = GetComponent<Image>();

        if (virtualCam == null)
            virtualCam = FindFirstObjectByType<CinemachineVirtualCamera>();
    }

    public void FadeInAndOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        yield return FadeRoutine(1f);
        Respawn();
        yield return FadeRoutine(0f);

        fadeCoroutine = null;
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float elapsed = 0f;
        Color color = image.color;
        float startAlpha = color.a;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeTime);
            image.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        image.color = color;
    }

    private void Respawn()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer.gameObject);

        currentPlayer = Instantiate(
            playerPrefab,
            respawnPoint.position,
            Quaternion.identity
        ).transform;

        if (virtualCam != null)
            virtualCam.Follow = currentPlayer;
    }
}
