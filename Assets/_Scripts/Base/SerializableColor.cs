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
