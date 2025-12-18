using UnityEngine;
using TMPro;
public class Score : MonoBehaviour
{
    private int _currentScore = 0;
    private TMP_Text _scoreText;

    void Awake()
    {
        _scoreText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        Health.OnDeath += EnemyDestroyed;
    }
    void OnDisable()
    {
        Health.OnDeath -= EnemyDestroyed;

    }
    private void EnemyDestroyed(Health sender)
    {
        _currentScore++;
        _scoreText.text = _currentScore.ToString("D3");

    }
}
