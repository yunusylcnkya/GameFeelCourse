using UnityEngine;
using TMPro;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, oyundaki skor sistemini yönetir:

- Düşman öldüğünde skoru artırır
- Skor UI'da (ekranda) gösterilir
*/

public class Score : MonoBehaviour
{
    private int _currentScore = 0; // Şu anki skor
    private TMP_Text _scoreText;   // TextMeshPro UI bileşeni

    void Awake()
    {
        _scoreText = GetComponent<TMP_Text>(); // Text bileşenini al
    }

    void OnEnable()
    {
        Health.OnDeath += EnemyDestroyed; // Düşman öldüğünde EnemyDestroyed çalışır
    }

    void OnDisable()
    {
        Health.OnDeath -= EnemyDestroyed; // Olaydan çık
    }

    // Düşman öldüğünde skoru artır
    private void EnemyDestroyed(Health sender)
    {
        _currentScore++; // Skoru 1 artır
        _scoreText.text = _currentScore.ToString("D3"); // 3 basamaklı olarak ekrana yaz (ör: 001, 002)
    }
}
