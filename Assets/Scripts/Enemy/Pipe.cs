using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod bir "boru" gibi düşmanları sürekli spawn (doğurma) eder.

- Belirli aralıklarla düşman çıkarır
- Her düşmanın rengi rastgele olur
- Borunun kendi ColorChanger'ı ile renk belirlenir
*/

public class Pipe : MonoBehaviour
{
    /*
    DÜŞMAN ÖRNEĞİ
    ---------------
    _enemyPrefab : Bu borudan hangi düşman çıkacak
    _spawnTimer : Ne kadar aralıklarla düşman doğacak (saniye)
    */
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnTimer = 3f;

    /*
    RENK DEĞİŞTİRİCİ
    -----------------
    Bu borunun ColorChanger script’i.
    Spawn edeceği düşmanın rengi buradan alınır
    */
    private ColorChanger _colorChanger;

    /*
    AWAKE
    -----
    Boru başladığında ColorChanger’ı bulur
    */
    void Awake()
    {
        _colorChanger = GetComponent<ColorChanger>();
    }

    /*
    START
    -----
    Oyun başladığında düşman spawn rutini başlar
    */
    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    /*
    SPAWN ROUTINE
    -------------
    1️⃣ Renkleri rastgele seçer
    2️⃣ Düşmanı borunun pozisyonunda oluşturur
    3️⃣ Düşmana borunun rengini verir
    4️⃣ Belirlenen süre bekler, sonra tekrar eder
    */
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Borunun rengini rastgele seç
            _colorChanger.SetRandomColor();

            // Düşmanı oluştur
            Enemy enemy = Instantiate(_enemyPrefab, transform.position, transform.rotation);

            // Düşmanın rengini ayarla
            enemy.Init(_colorChanger.DefaultColor);

            // Bir sonraki spawn için bekle
            yield return new WaitForSeconds(_spawnTimer);
        }
    }
}
