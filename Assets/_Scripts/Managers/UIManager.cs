//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// Attached To: UIManager
/// Description: Handles basic UI operations.
/// </summary>
public class UIManager : MonoBehaviour
{
	//Grid
	public GameObject gameOverPanel;
	public BoardManager boardManager;
	public Transform gameBoardCanvas;
	//Hammer Bonus
	public Animator hammerButtonAnim;
	//Hint Bonus
	public Animator hintButtonAnim;
	//Skip Bonus
	public Animator skipButtonAnim;
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
		hammerButtonAnim.SetTrigger("isPressed");
		if (BoardManager.gameState == GameState.HammerPowerUp)
			SoundManager.Instance.PlayButtonClick();
		else SoundManager.Instance.PlayInvalid();
		
	}
	public void HintPressed()
	{
		hintButtonAnim.SetTrigger("isPressed");
	}
	public void SkipPressed()
	{
		skipButtonAnim.SetTrigger("isPressed");
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
