using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, arka planın (veya herhangi bir katmanın) hareketini kameraya göre kaydırır
ve böylece “parallax efekti” oluşturur. Yani kamera hareket ettikçe arka plan daha yavaş
hareket eder ve derinlik hissi verir.
*/

public class Parallax : MonoBehaviour
{
    [SerializeField]
    private float _parallaxOffset = -0.1f; // Arka planın ne kadar kayacağı (negatif olursa tersi yönde hareket eder)
    private Vector2 _startPos;             // Başlangıç pozisyonu
    private Camera _mainCamera;            // Ana kamera

    // Kameranın ne kadar hareket ettiğini hesaplar
    private Vector2 _travel => (Vector2)_mainCamera.transform.position - _startPos;

    void Awake()
    {
        _mainCamera = Camera.main; // Ana kamerayı bul
    }

    void Start()
    {
        _startPos = transform.position; // Başlangıç pozisyonunu kaydet
    }

    void FixedUpdate()
    {
        // Yeni pozisyon = başlangıç + kamera hareketi * offset
        Vector2 newPosition = _startPos + new Vector2(_travel.x * _parallaxOffset, 0f);
        transform.position = new Vector2(newPosition.x, transform.position.y); // Yalnızca X eksenini kaydır
    }
}
