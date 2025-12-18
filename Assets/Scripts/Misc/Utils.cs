using System;
using System.Collections;
using UnityEngine;

public static class Utils
{
    public static void RunAfterDelay(MonoBehaviour monoBehaviour, float delay, Action task)
    {
        monoBehaviour.StartCoroutine(runAfterDelayRoutine(delay, task));
    }

    private static IEnumerator runAfterDelayRoutine(float delay, Action task)
    {
        yield return new WaitForSeconds(delay);
        task.Invoke();
    }

}
