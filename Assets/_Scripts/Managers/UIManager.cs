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

	public GameObject gameOverPanel;
	public SelectionManager selectionManager;
	public Transform gameBoardCanvas;

	//hammer bonus related
	public Transform hammerTransform;
	public Sprite hammerSprite;
	public Sprite hammerSpritePressed;
	private bool isHammerPressed;
	// Use this for initialization
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
		if (!isHammerPressed && SelectionManager.gameState == GameState.HammerPowerUp)
		{
			isHammerPressed = true;
			Animator anim = hammerTransform.GetComponent<Animator>();
			anim.SetBool("isPressed", true);
		//	hammerButton.image.sprite = hammerSpritePressed;
		}
		
	}
	public void HammerUsed() //callback from BlockManager's hammerUsedEvent
	{
		isHammerPressed = false;
		Animator anim = hammerTransform.GetComponent<Animator>();
		anim.SetBool("isPressed", false);
	}
}
