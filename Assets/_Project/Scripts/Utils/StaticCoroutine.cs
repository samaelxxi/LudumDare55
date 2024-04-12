using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StaticCoroutine 
{
    private static StaticCoroutineRunner runner;
 
    public static Coroutine Start(IEnumerator coroutine)
    {
        EnsureRunner();
        return runner.StartCoroutine(coroutine);
    }

    public static Coroutine StartInSec(IEnumerator coroutine, float sec)
    {
        EnsureRunner();
        return runner.StartCoroutine(StartInSecInner(coroutine, sec));
    }

    public static Coroutine StartInSec(Action action, float sec)
    {
        EnsureRunner();
        return runner.StartCoroutine(StartInSecInner(action, sec));
    }

    static IEnumerator StartInSecInner(IEnumerator coroutine, float sec)
    {
        yield return new WaitForSeconds(sec);
        Start(coroutine);
    }

    static IEnumerator StartInSecInner(Action action, float sec)
    {
        yield return new WaitForSeconds(sec);
        action?.Invoke();
    }
 
    private static void EnsureRunner()
    {
        if (runner == null)
        {
            runner = new GameObject("[Static Coroutine Runner]").AddComponent<StaticCoroutineRunner>();
            UnityEngine.Object.DontDestroyOnLoad(runner.gameObject);
        }
    }
 
    private class StaticCoroutineRunner : MonoBehaviour {}
}
