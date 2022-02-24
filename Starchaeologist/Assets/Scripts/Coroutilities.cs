using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collection of Coroutine helper functions that can be used anywhere that has access to a MonoBehavior.
/// Written and edited by Patrick Mitchell over the course of multiple projects.
/// </summary>
public static class Coroutilities
{
    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> in <paramref name="delay"/> seconds.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called after <paramref name="delay"/> seconds.</param>
    /// <param name="delay">How many seconds to wait before calling <paramref name="thingToDo"/>.</param>
    /// <returns>The delay coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoAfterDelay(MonoBehaviour coroutineCaller, Action thingToDo, float delay)
    {
        //If delay is too low, just call the function immediately and bail out
        if (delay <= 0)
        {
            thingToDo();
            return null;
        }
        //Otherwise, start a coroutine which will call the function in delay seconds
        return coroutineCaller.StartCoroutine(DoAfterDelay(thingToDo, delay));
    }
    private static IEnumerator DoAfterDelay(Action thingToDo, float delay)
    {
        yield return new WaitForSeconds(delay);
        thingToDo();
    }

    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> every <paramref name="interval"/> seconds for <paramref name="duration"/> seconds.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called for <paramref name="delay"/> seconds.</param>
    /// <param name="duration">How many seconds <paramref name="thingToDo"/> should happen for.</param>
    /// <param name="interval"><paramref name="thingToDo"/> will run every <paramref name="interval"/> seconds.<br/>
    ///     At minimum, every frame. <i>(Anything less than <see cref="Time.deltaTime"/> will behave the same as <see cref="Time.deltaTime"/>.)</i></param>
    /// <returns>The coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoForSeconds(MonoBehaviour coroutineCaller, Action thingToDo, float duration, float interval = 0)
    {
        return coroutineCaller.StartCoroutine(DoForSeconds(thingToDo, duration, interval));
    }
    private static IEnumerator DoForSeconds(Action thingToDo, float duration, float interval = 0)
    {
        float intervalTimer = 0;
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            //After interval seconds (tracked using intervalTimer), run thingToDo and reset the interval timer
            intervalTimer += Time.deltaTime;
            if (intervalTimer >= interval)
            {
                intervalTimer = 0;
                thingToDo();
            }

            yield return null;
        }
    }

    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> once <paramref name="predicate"/> evaluates to true.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called once <paramref name="predicate"/> evaluates to true.</param>
    /// <param name="predicate">Delegate or lambda that will be evaluated every frame. Once it evaluates to true, call <paramref name="thingToDo"/>.</param>
    /// <returns>The coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoWhen(MonoBehaviour coroutineCaller, Action thingToDo, Func<bool> predicate)
    {
        return coroutineCaller.StartCoroutine(DoWhen(thingToDo, predicate));
    }
    private static IEnumerator DoWhen(Action thingToDo, Func<bool> predicate)
    {
        yield return new WaitUntil(predicate);
        thingToDo();
    }

    /// <summary>
    /// Calls the function <paramref name="thingToDo"/> every <paramref name="interval"/> seconds, until <paramref name="predicate"/> evaluates to true.<br/>
    /// <i>(<paramref name="coroutineCaller"/> is needed to call the coroutine, since <c>StartCoroutine()</c> is a MonoBehavior 
    /// function, and MonoBehaviours cannot be static.)</i>
    /// </summary>
    /// <param name="coroutineCaller">The <see cref="MonoBehaviour"/> that calls the coroutine.</param>
    /// <param name="thingToDo">The function or lambda expression that will be called until <paramref name="predicate"/> evaluates to true.</param>
    /// <param name="predicate">Delegate or lambda that will be evaluated every frame. Once it evaluates to true, stop calling <paramref name="thingToDo"/>.</param>
    /// <param name="interval"><paramref name="thingToDo"/> will run every <paramref name="interval"/> seconds.<br/>
    ///     At minimum, every frame. <i>(Anything less than <see cref="Time.deltaTime"/> will behave the same as <see cref="Time.deltaTime"/>.)</i></param>
    /// <returns>The coroutine that was started. Use this with <c>StopCoroutine()</c> to cancel <paramref name="thingToDo"/>.</returns>
    public static Coroutine DoUntil(MonoBehaviour coroutineCaller, Action thingToDo, Func<bool> predicate, float interval = 0)
    {
        return coroutineCaller.StartCoroutine(DoUntil(thingToDo, predicate, interval));
    }
    private static IEnumerator DoUntil(Action thingToDo, Func<bool> predicate, float interval)
    {
        float intervalTimer = 0;
        while (!predicate())
        {
            //After interval seconds (tracked using intervalTimer), run thingToDo and reset the interval timer
            intervalTimer += Time.deltaTime;
            if (intervalTimer >= interval)
            {
                intervalTimer = 0;
                thingToDo();
            }

            yield return null;
        }
    }

    /// <summary>
    /// Stops a coroutine and nullifies the given reference to it if it's not null. If it is null (already stopped), does nothing.
    /// </summary>
    /// <param name="coroutineLocation">The <see cref="MonoBehaviour"/> that <paramref name="coroutine"/> was started in.</param>
    /// <param name="coroutine">The coroutine to try to stop. If it is stopped, it will be set to null.</param>
    /// <returns>Whether the coroutine was successfully stopped.</returns>
    public static bool TryStopCoroutine(MonoBehaviour coroutineLocation, ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            coroutineLocation.StopCoroutine(coroutine);
            coroutine = null;
            return true;
        }
        return false;
    }
}
