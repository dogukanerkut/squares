//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
/// <summary>
/// Attached To: SoundManager
/// Description: Manager class to handle all sound operations[Singleton Class]
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
	public bool areSoundsActive;
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
	public AudioSource invalid;
	void Awake()
	{
		#region safecheck
#if UNITY_EDITOR
		if (!areSoundsActive) Debug.Log("All sounds are disabled.");
#endif
		#endregion

	}
	public void PlayPlaceBlock()
	{
		if (areSoundsActive)
		{
			placeBlock.Play();
		IncreasePlaceBlockPitch();
		}
	}

	public void PlayRetrieveBlock()
	{
		if (areSoundsActive)
		{
			retrieveBlock.Play();
			DecreasePlaceBlockPitch();
		}
	}
	#region Pitch settings
	private void IncreasePlaceBlockPitch()
	{
		if (areSoundsActive) placeBlock.pitch += placeBlockPitchRate;
	}
	private void DecreasePlaceBlockPitch()
	{
		if (areSoundsActive) placeBlock.pitch -= placeBlockPitchRate;
	}
	public void ResetPlaceBlockPitch()
	{
		if (areSoundsActive) placeBlock.pitch = 1;
	}
#endregion

	public void PlayBlockExplode()
	{
		if (areSoundsActive) blockExplode.Play();
	}
	public void PlayBlockExplodeCombo()
	{
		if (areSoundsActive) blockExplodeCombo.Play();
	}
	public void PlayGameOver()
	{
		if (areSoundsActive) gameOver.Play();
	}
	public void PlayButtonClick()
	{
		if (areSoundsActive) buttonClick.Play();
	}
	public void PlayHammerPowerUp()
	{
		if (areSoundsActive) hammerPowerUp.Play();
	}
	public void PlayHintPowerUp()
	{
		if (areSoundsActive) hintPowerUp.Play();
	}
	public void PlayInvalid()
	{
		if (areSoundsActive) invalid.Play();
	}
}
