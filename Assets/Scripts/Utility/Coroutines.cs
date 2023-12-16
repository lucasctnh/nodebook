using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Coroutines
{
	public static IEnumerator CallbackWithTimeout(Func<bool> condition, float timeout, Action callback)
	{
		float timeoutTimer = timeout > 0f ? timeout : Mathf.Infinity;

		while (condition.Invoke() == false && timeoutTimer > 0f)
		{
			timeoutTimer -= UnityEngine.Time.deltaTime;
			yield return null;
		}
		callback?.Invoke();
	}

	public static IEnumerator WaitForSeconds(float seconds)
	{
		float startTime = Time.time;
		while (Time.time < startTime + seconds)
			yield return null;
	}

	public static IEnumerator WaitForSecondsRealtime(float seconds)
	{
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < startTime + seconds)
			yield return null;
	}

	public static IEnumerator WaitForFrames(int frames)
	{
		for (int i = 0; i < frames; i++)
			yield return null;
	}

	public static IEnumerator DoAfterTime(float time, Action effect)
	{
		yield return WaitForSeconds(time);
		effect?.Invoke();
	}

	public static IEnumerator DoAfterRealTime(float time, Action effect)
	{
		yield return WaitForSecondsRealtime(time);
		effect?.Invoke();
	}

	public static IEnumerator DoAfterFrames(int frames, Action callback)
	{
		yield return WaitForFrames(frames);
		callback?.Invoke();
	}
}
