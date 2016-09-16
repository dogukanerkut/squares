//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// Referring To: 
/// Referenced From: 
/// Attached To: 
/// Description: 
/// </summary>
public class UIManager : MonoBehaviour
{
	//Grid
	public GameObject gameOverPanel;
	public SelectionManager selectionManager;
	public Transform gameBoardCanvas;
	//Hammer Bonus
	public Transform hammerTransform;
	//sound button
	public Image soundImage;
	public Sprite soundSprite;
	public Sprite soundSilencedSprite;
	//
	// Use this for initialization 
	public void Awake()
	{
		if (SoundManager.Instance.isSoundsActive)
			soundImage.sprite = soundSprite;
		else
			soundImage.sprite = soundSilencedSprite;
			
	}
	public void GameOver()
	{
		gameOverPanel.SetActive(true);
	}
	public void RestartGame()
	{
		gameOverPanel.SetActive(false);
	}

	public void HammerPressed()
	{
		if (SelectionManager.gameState == GameState.HammerPowerUp)
		{
			Animator anim = hammerTransform.GetComponent<Animator>();
			anim.SetTrigger("isPressed");
			//	hammerButton.image.sprite = hammerSpritePressed;
			SoundManager.Instance.PlayButtonClick();
		}
		else SoundManager.Instance.PlayInvalid();
		
	}
	public void SoundButton()
	{
		if (SoundManager.Instance.isSoundsActive)
			soundImage.sprite = soundSilencedSprite;
		else
			soundImage.sprite = soundSprite;
		SoundManager.Instance.isSoundsActive = !SoundManager.Instance.isSoundsActive;
    }
}
