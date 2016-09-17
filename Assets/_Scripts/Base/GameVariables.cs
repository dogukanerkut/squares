//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
/// <summary>
/// Description: Our Game variables data holder class.
/// </summary>
[System.Serializable]
public class GameVariables {
	public int score; //our current session's score
	public int highScore; //our highscore
	public int diamondBank; // our diamonds
	public int matchCount; // our current number of matched colors(number of blocks we've exploded)
	public int difficultyCounter; // our current difficulty counter(difficulty is increased every X amount of difficultyBracket we reach, and we reach them by destroying blocks)
	public int currentDifficultyBracket; // our current difficulty level
	public int gameStateIndex; // our Game State's index(Game state is enum so we cast it)
	public bool isHammerUsed; // if the hammer is used before leaving the game
	public bool isHintUsed; // if the hint is used before leaving the game
	public bool isSoundActive; // if sounds active
	public GameVariables(int _score, int _highScore, int _diamondBank, int _matchCount, int _difficultyCounter, int _currentDifficultyBracket, int _gameStateIndex, bool _isHammerUsed, bool _isHintUsed, bool _isSoundActive)
	{
		score = _score;
		highScore = _highScore;
		diamondBank = _diamondBank;
		matchCount = _matchCount;
		difficultyCounter = _difficultyCounter;
		currentDifficultyBracket = _currentDifficultyBracket;
		gameStateIndex = _gameStateIndex;
		isHammerUsed = _isHammerUsed;
		isHintUsed = _isHintUsed;
		isSoundActive = _isSoundActive;

	}
}
