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
	// Use this for initialization
	public void GameOver()
	{
		gameOverPanel.SetActive(true);
	}
	public void RestartGame()
	{
		gameOverPanel.SetActive(false);
	}
	// Update is called once per frame
}
