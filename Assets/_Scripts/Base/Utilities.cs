//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
/// <summary>
/// Referring To: 
/// Referenced From: 
/// Attached To: 
/// Description: 
/// </summary>
public static class Utilities
{
	static float curTime;
	public static int IntLerp(int from, int to, float time)
	{

		if (Time.time > curTime + time)
		{
			if (from < to) // hedeflenen sayı şu anki sayımızdan büyükse
			{
				if (to - from >= 100000)
					from += 10000;
				if (to - from >= 10000)
					from += 1000;
				if (to - from >= 1000)
					from += 500;
				if (to - from >= 500)
					from += 100;
				else if (to - from >= 50)
					from += 10;
				else
					from++;
			}

			else if (from > to)
			{
				if (from - to >= 10000)
					from -= 1000;
				if (from - to >= 1000)
					from -= 500;
				else if (from - to >= 500)
					from -= 100;
				else if (from - to >= 100)
					from -= 10;
				else
					from--;

			}

			curTime = Time.time;
		}
		return from;
	}
}
