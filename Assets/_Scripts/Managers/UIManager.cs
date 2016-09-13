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
public class UIManager : MonoBehaviour {

	public GameObject gameOverPanel;
	// Use this for initialization
	public void GameOver()
	{
		gameOverPanel.SetActive(true);
	}
	public void RestartGame()
	{
		gameOverPanel.SetActive(false);
	}
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
