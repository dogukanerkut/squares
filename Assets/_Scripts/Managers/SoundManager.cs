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
public class SoundManager : MonoBehaviour {

	private static SoundManager instance;
	private SoundManager() { }
	public static SoundManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
			}
			return instance;
		}
	}
	public AudioSource placeBlock;

	[Range(0f,.5f)]
	public float placeBlockPitchRate;
	public AudioSource retrieveBlock;
	public AudioSource blockExplode;
	public AudioSource blockExplodeCombo;
	public AudioSource gameOver;
	public AudioSource buttonClick;
	public AudioSource hammerPowerUp;

	private void IncreasePlaceBlockPitch()
	{
		placeBlock.pitch += placeBlockPitchRate;
	}
	private void DecreasePlaceBlockPitch()
	{
		placeBlock.pitch -= placeBlockPitchRate;
	}
	public void ResetPlaceBlockPitch()
	{
		placeBlock.pitch = 1;
	}
	public void PlayPlaceBlock()
	{
		placeBlock.Play();
		IncreasePlaceBlockPitch();
	}
	public void PlayRetrieveBlock()
	{
		retrieveBlock.Play();
		DecreasePlaceBlockPitch();
	}
	public void PlayBlockExplode()
	{
		blockExplode.Play();
	}
	public void PlayBlockExplodeCombo()
	{
		blockExplodeCombo.Play();
	}
	public void PlayGameOver()
	{
		gameOver.Play();
	}
	public void PlayButtonClick()
	{
		buttonClick.Play();
	}
	public void PlayHammerPowerUp()
	{
		hammerPowerUp.Play();
	}
}
