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
	public bool isSoundsActive;
	public AudioSource placeBlock;

	[Range(0f,.5f)]
	public float placeBlockPitchRate;
	public AudioSource retrieveBlock;
	public AudioSource blockExplode;
	public AudioSource blockExplodeCombo;
	public AudioSource gameOver;
	public AudioSource buttonClick;
	public AudioSource hammerPowerUp;
	public AudioSource hintPowerUp;

	void Awake()
	{
		#region safecheck
#if UNITY_EDITOR
		if (!isSoundsActive) Debug.Log("All sounds are disabled.");
#endif
		#endregion
	}
	private void IncreasePlaceBlockPitch()
	{
		if(isSoundsActive)	placeBlock.pitch += placeBlockPitchRate;
	}
	private void DecreasePlaceBlockPitch()
	{
		if (isSoundsActive) placeBlock.pitch -= placeBlockPitchRate;
	}
	public void ResetPlaceBlockPitch()
	{
		if (isSoundsActive) placeBlock.pitch = 1;
	}
	public void PlayPlaceBlock()
	{
		if (isSoundsActive)
		{
			placeBlock.Play();
		IncreasePlaceBlockPitch();
		}
	}
	public void PlayRetrieveBlock()
	{
		if (isSoundsActive)
		{
			retrieveBlock.Play();
			DecreasePlaceBlockPitch();
		}
	}
	public void PlayBlockExplode()
	{
		if (isSoundsActive) blockExplode.Play();
	}
	public void PlayBlockExplodeCombo()
	{
		if (isSoundsActive) blockExplodeCombo.Play();
	}
	public void PlayGameOver()
	{
		if (isSoundsActive) gameOver.Play();
	}
	public void PlayButtonClick()
	{
		if (isSoundsActive) buttonClick.Play();
	}
	public void PlayHammerPowerUp()
	{
		if (isSoundsActive) hammerPowerUp.Play();
	}
	public void PlayHintPowerUp()
	{
		if (isSoundsActive) hintPowerUp.Play();
	}
}
