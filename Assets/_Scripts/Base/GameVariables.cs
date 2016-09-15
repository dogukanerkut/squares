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
public class GameVariables {
	public int score;
	public int highScore;
	public int difficultyCounter;
	public int currentDifficultyBracket;
	public bool isHammerUsed;
	public bool isHintUsed;

	public GameVariables(int _score, int _highScore, int _difficultyCounter, int _currentDifficultyBracket, bool _isHammerUsed, bool _isHintUsed)
	{
		score = _score;
		highScore = _highScore;
		difficultyCounter = _difficultyCounter;
		currentDifficultyBracket = _currentDifficultyBracket;
		isHammerUsed = _isHammerUsed;
		isHintUsed = _isHintUsed;
	}
}
