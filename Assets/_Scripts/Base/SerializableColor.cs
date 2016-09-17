//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
/// <summary>
/// Description: UnityEngine's Color class can't be serialized, we need this class to serialize our colors.
/// </summary>
[System.Serializable]
public class SerializableColor
{
	private float r;
	private float g;
	private float b;
	private float a;

	public SerializableColor(Color color)
	{
		r = color.r;
		g = color.g;
		b = color.b;
		a = color.a;
	}

	public Color GetColor()
	{
		return new Color(r, g, b, a);
	}
}
