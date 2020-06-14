using System;
using System.Collections;
using UnityEngine;

namespace TankGame
{
    public static class Coroutines
    {
        public static IEnumerator WaitForSecondsCoroutine(Action callback, float secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            callback.Invoke();
        }

        public static IEnumerator WaitForEndOfFrameCoroutine(Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback.Invoke();
        }

        public static IEnumerator WaitForFixedUpdateCoroutine(Action callback)
        {
            yield return new WaitForFixedUpdate();
            callback.Invoke();
        }
    }
}
