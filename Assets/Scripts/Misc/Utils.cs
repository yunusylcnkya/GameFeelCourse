using System;
using System.Collections;
using UnityEngine;

/*
BU SCRIPT NE YAPIYOR?
---------------------
Bu kod, istediğimiz bir işi belirli bir süre sonra çalıştırmamızı sağlar.
Yani "X saniye bekle sonra bu şeyi yap" demek için kullanılır.
*/

public static class Utils
{
    // DIŞARIDAN KULLANIM:
    // Utils.RunAfterDelay(this, 2f, () => Debug.Log("2 saniye sonra çalıştı!"));
    public static void RunAfterDelay(MonoBehaviour monoBehaviour, float delay, Action task)
    {
        // Coroutine başlatıyor ve belirli süre sonra işi çalıştırıyor
        monoBehaviour.StartCoroutine(runAfterDelayRoutine(delay, task));
    }

    // GERÇEK İŞİ YAPAN FONKSİYON
    private static IEnumerator runAfterDelayRoutine(float delay, Action task)
    {
        yield return new WaitForSeconds(delay); // Bekle
        task.Invoke(); // İşlem başlat
    }
}
