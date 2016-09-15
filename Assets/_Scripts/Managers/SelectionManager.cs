//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
/// <summary>
/// Referring To: BoardManager.cs(Singleton)
/// Referenced From: EventController.cs
/// Attached To: SelectionManager
/// Description: Handles selection activities(when player presses and drags).
/// </summary>
public class SelectionManager : MonoBehaviour
{
	//Initial class implementation
	private BlocksArray blocks;
	private ColorBase colorBase = new ColorBase();
	//Default block color of grid
	public Color defaultBlockColor;
	//Blocks placed holders
	Block currentBlock;
	Block previousBlock;
	List<Block> blocksPlaced = new List<Block>();
	// players input on how many blocks he dragged
	private int selectionCount = 0; 
	//General Game Variables
	public GameObject blockPrefab;
	public RectTransform gamePanel;
	public static GameState gameState;
	//New Blocks Variables
	public RectTransform newBlocksPanel;
	private Image[] newBlockImages;
	List<BlockInfo> newBlocks = new List<BlockInfo>();
	//Score Variables
	public Text scoreText;
	private int score;
	private int updatedScore;
	private bool isScoreIncreased;
	//Hint Blocks Variables
	public RectTransform hintBlocksPanel;
	private Image[] hintBlockImages;
	private bool isHintUsed;
	List<BlockInfo> hintBlocks = new List<BlockInfo>();
	//Difficulty settings
	[Header("Hover Difficulty Brackets to see info")]
	[Tooltip("Difficulty counter increases when player matches atleast 3 blocks and resets when player reaches a bracket")]
	public int[] difficultyBrackets;
	private int difficultyCounter;
	private int currentDifficultyBracket = 0;
	//Unity Events
	public UnityEvent gameOverEvent;
	public UnityEvent hammerUsedEvent;
	void Awake()
	{
		#region safecheck
		if (colorBase.GetTotalDifficulty() != difficultyBrackets.Length)
			Debug.LogError("Difficulty Brackets should be equal with Difficulty Color Count!\nDifficulty Brackets = " + difficultyBrackets.Length +
						  "\nDifficulty Color Count = " + colorBase.GetTotalDifficulty() + "\nFix Difficulty Brackets Size in " + gameObject.name +
						  " or Add/Deduct Difficulty Colors in " + colorBase.ToString() + " class");
		#endregion

		ColorBase.defaultColor = defaultBlockColor;

		newBlockImages = newBlocksPanel.GetComponentsInChildren<Image>();
		hintBlockImages = hintBlocksPanel.GetComponentsInChildren<Image>();
		//disable all hintBlockImages when game starts
		foreach (Image img in hintBlockImages)
		{
			img.gameObject.SetActive(false);
		}
		gameState = GameState.Idle;
		GenerateBoard();
		GridLayoutGroup gamePanelGLG = gamePanel.GetComponent<GridLayoutGroup>();
		gamePanelGLG.constraintCount = Constants.RowCount; // if grid size is changed, adjust constrain count accordingly
	}

	/// <summary>
	/// Generates a Board with prearranged Row and Column count(See Constants class) and assigns them to [blocks] 2D array.
	/// </summary>
	void GenerateBoard()
	{
		blocks = new BlocksArray();
		//blocks = new GameObject[_row, _column];
		//Instantiate prefabs and assign them to blocks 2D array.
		for (int i = 0; i < Constants.ColumnCount; i++)
		{
			for (int j = 0; j < Constants.RowCount; j++)
			{
				GameObject tempBlock = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				tempBlock.transform.SetParent(gamePanel);
				tempBlock.transform.localScale = Vector3.one; // Instantiated objects has different scale value than 1 somehow?
				blocks[j, i] = tempBlock;

				tempBlock.GetComponent<Block>().FillInfo(j, i, ColorBase.defaultColor); //Set object's row and column
			}
		}
		CreateBlocks(CreationType.Actual);

	}

	/// <summary>
	/// Resets the board for new game
	/// </summary>
	public void ResetBoard() // used from GameOverPanel(Game object in hierarchy)
	{
		gameState = GameState.Idle;
		for (int i = 0; i < Constants.ColumnCount; i++)
		{
			for (int j = 0; j < Constants.RowCount; j++)
			{
				blocks[j, i].GetComponent<Block>().Clear();
				blocks[j, i].GetComponent<Block>().info.Clear();
			}
		}
		SetScore(0, false); // set score to 0
		ResetDifficulty(); // resets difficulty
		CreateBlocks(CreationType.Actual); // create new blocks
	}

	private enum CreationType
	{ Actual, Hint }
	/// <summary>
	/// Create new blocks, assign their colors and arrange the new block images.
	/// </summary>
	/// <param name="type">Actual creates new blocks, Hint creates Hint blocks</param>
	void CreateBlocks(CreationType type)
	{
		List<BlockInfo> blocksInfo = new List<BlockInfo>();
		Image[] blockImages = new Image[0];
		switch (type) //select which blocks to process
		{
			case CreationType.Actual:
				blocksInfo = newBlocks;
				blockImages = newBlockImages;
				break;
			case CreationType.Hint:
				blocksInfo = hintBlocks;
				blockImages = hintBlockImages;
				break;
			default:
				break;
		}
		BlocksCreator blocksCreator = new BlocksCreator();
		if (isHintUsed) //transfer hintblocks to new blocks
		{
			blocksInfo = new List<BlockInfo>(hintBlocks); // create it as new list so we don't reference the hintBlocks and can reset it
			hintBlocks.Clear();
			hintBlockImages = ProcessBlocks(hintBlockImages, hintBlocks); // we don't need hintblocks so disable their image
			isHintUsed = false;
		}
		else // create random new blocks  here
		{
			blocksInfo.Clear();
			blocksInfo = blocksCreator.GetRandomBlocks(); // bring new random blocks
			colorBase.FillColorInfo(blocksInfo); // color newly introduced blocks randomly
		}
		blockImages = ProcessBlocks(blockImages, blocksInfo);
		switch (type) // push processed blocks back to global holders.
		{
			case CreationType.Actual:
				newBlocks = blocksInfo;
				newBlockImages = blockImages;
				break;
			case CreationType.Hint:
				hintBlocks = blocksInfo;
				hintBlockImages = blockImages;
				break;
			default:
				break;
		}

	}
	/// <summary>
	/// Update images' colors with given blocksInfo
	/// </summary>
	/// <param name="blockImages">block image list to be processed</param>
	/// <param name="blocksInfo">blocksInfo to fill color of images</param>
	Image[] ProcessBlocks(Image[] blockImages, List<BlockInfo> blocksInfo)
	{
		for (int i = 0; i < blockImages.Length; i++)
			blockImages[i].gameObject.SetActive(false);

		for (int i = 0; i < blocksInfo.Count; i++)
		{
			blockImages[i].gameObject.SetActive(true);
			blockImages[i].color = blocksInfo[i].BlockColor;
		}
		return blockImages;
	}

	#region Touch & Drag & Release detection of player-------------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// When player first touches a block, Update touched block and start touching process.
	/// </summary>
	/// <param name="selectedBlock"></param>
	public void StartSelection(Block selectedBlock)
	{
		//Checks if block is white
		if (blocks.CheckIfBlockAvailable(selectedBlock.info) && gameState == GameState.Idle)
		{
			gameState = GameState.SelectionStarted; //Start selection process
			currentBlock = selectedBlock;
			previousBlock = selectedBlock;

			UpdateSelectedBlock(selectedBlock);
			SoundManager.Instance.PlayPlaceBlock();
		}
		//if game state is hammer power up and the touched block is a colored one.
		else if(!blocks.CheckIfBlockAvailable(selectedBlock.info) && gameState == GameState.HammerPowerUp)
			RemoveBlock(selectedBlock);
	}

	/// <summary>
	/// Appends the changes on selected block(sets it's color, adds it  to placed blocks list and updates selection count
	/// </summary>
	/// <param name="selectedBlock"></param>
	private void UpdateSelectedBlock(Block selectedBlock)
	{
		selectedBlock.SetColor(newBlocks[selectionCount].BlockColor);//Set color of selected block to queued new block
		blocksPlaced.Add(selectedBlock); // add currently selected block to the list of placed blocks(we will need it later)
		selectionCount++; // increase the selection count so next time the queued block will be placed
	}

	/// <summary>
	/// Remove a single block from the grid.
	/// </summary>
	/// <param name="selectedBlock">Block to be removed.</param>
	private void RemoveBlock(Block selectedBlock)
	{
		selectedBlock.Clear(); // remove it's colors
		SoundManager.Instance.PlayHammerPowerUp();
		gameState = GameState.Idle;
		hammerUsedEvent.Invoke();
	}

	/// <summary>
	/// <para>When player continues to drag his finger on other blocks</para>
	/// <para>Do stuff accordingly(revert selection or add newblocks in queue).</para>
	/// </summary>
	/// <param name="selectedBlock"></param>
	public void SetSelectedBlock(Block selectedBlock)
	{
		//if player intends to add new block on queue to grid which is adjacent to previous one 
		if (blocks.IsBlocksAdjacentAndAvailable(currentBlock.info, selectedBlock.info))
		{
			// if number of selected blocks is not bigger than newly created blocks
			if (selectionCount < newBlocks.Count)
			{
				// Purpose of this to track current and previous blocks 
				// In order to revert our selection
				previousBlock = currentBlock; // Set current block as previous block as it will become previous block after this selection
				currentBlock = selectedBlock; // Set newly selected block as current block

				UpdateSelectedBlock(selectedBlock); // hover to method to see description
				SoundManager.Instance.PlayPlaceBlock();
			}
		}
		// if player intends to revert his placed block by going backwards in placed blocks.
		else if(blocksPlaced.Contains(selectedBlock) && selectedBlock == previousBlock)
		{
			//player's finger is on previous block so remove the last placed block
			int selectedBlockIndex = blocksPlaced.IndexOf(selectedBlock);//get index of previous block

			//this check is required to prevent error when there is only one block exists and therefore index+1 is out of range
			if (selectedBlockIndex+1 < blocksPlaced.Count) // if the last placed block's index is smaller than number of blocksPlaced
			{
				//remove last placed block and update selection count accordingly
				blocksPlaced[selectedBlockIndex+1].Clear();
				blocksPlaced.RemoveAt(selectedBlockIndex+1);
				selectionCount--;
				SoundManager.Instance.PlayRetrieveBlock();
			}
			currentBlock = selectedBlock; // previously selected block is now currently selected block
			if (selectedBlockIndex > 0) //this check is required to prevent error when there is only one block exists and therefore index-1 is out of range
				previousBlock = blocksPlaced[selectedBlockIndex - 1]; // update previousBlock

		}
	}

	/// <summary>
	/// <para>This code runs when player releases finger.</para>
	/// <para>If player wants to terminate the selection(either by trying to place all queued new blocks or simply revert it's selection.</para>
	/// <para>If player places blocks, check for adjacency, if any 3 or more same color adjacent blocks found, destroy them and create new blocks.</para>
	/// <para>Check the board after player placed blocks. If there are no more place left , game is over.</para>
	/// </summary>
	public void TerminateSelection()
	{
		if (selectionCount > 0)// check if any selection made //
		{
			if (blocksPlaced.Count < newBlocks.Count) // if player didn't place all of given blocks yet
			{
				//remove all blocks that are temporarily placed to grid
				for (int i = 0; i < blocksPlaced.Count; i++)
					blocksPlaced[i].Clear();

				SoundManager.Instance.PlayRetrieveBlock();
				SoundManager.Instance.ResetPlaceBlockPitch();
				gameState = GameState.Idle;
			}
			else // player placed all of given blocks to grid
			{
				SoundManager.Instance.ResetPlaceBlockPitch();
				

				if (blocks.CheckAdjacentBlocks(blocksPlaced)) // send placed block list for adjacency check and if there are any 3 or more adjacent block found
				{
					ExplodeBlocks(); // explode blocks if there are any
				}

				CreateBlocks(CreationType.Actual); // create new blocks to continue the game

				if (blocks.CheckEmptyBlocks(newBlocks.Count)) // Use this method after creating blocks!
					gameState = GameState.Idle;
				else // game over here
				{
					gameState = GameState.GameOver;
					gameOverEvent.Invoke();
					SoundManager.Instance.PlayGameOver();
				}

			}
			//regardless of the conditions above, the player is released fingers
			//and we need to reset the selectionCount and blocksPlaced list
			//and set the gamestate back to idle
			selectionCount = 0;
			blocksPlaced.Clear();
		}
	}

	public void ClearSelection()
	{
		currentBlock = null;
		selectionCount = 0;
	}

	/// <summary>
	/// Removes adjacent blocks, calculates score.
	/// </summary>
	private void ExplodeBlocks()
	{
		List<List<Block>> matchedBlocksList = blocks.GetMatchedBlockLists(); // retrieve matched block list
		if (matchedBlocksList.Count == 1) // if no combo occurs
			SoundManager.Instance.PlayBlockExplode();
		else // if combo occurs
			SoundManager.Instance.PlayBlockExplodeCombo();

		int scoreMultiplier = matchedBlocksList.Count;
		int score = 0;
		int totalScore = 0;
		foreach (List<Block> adjacentBlocksWithSameColor in matchedBlocksList)
		{
			//Score system is: a single block will yield total number of same-color adjacent blocks
			//and if there is combo occurs, double the points given.
			//For example 3 red blocks yield 3*3 = 9 points(3 points per block)
			//3 red blocks and 4 yellow blocks yield (3*3*2)+(4*4*2) = 50 points. Nnumber 2 is because there is two different same-color adjacent blocks
			score = adjacentBlocksWithSameColor.Count * scoreMultiplier; // score per block
			totalScore += adjacentBlocksWithSameColor.Count * score; //score for total blocks added
			foreach (Block adjacentBlock in adjacentBlocksWithSameColor)
			{
				StartTextAnimation(adjacentBlock, score);
				adjacentBlock.Clear(); // explode blocks(set their color to white etc.)
			}
		}
		//SetScore(updatedScore)
		ControlDifficulty();
		AddToScore(totalScore);
		blocks.ClearMatchedBlockLists(); // we are done with the list and we can clear it now for further turns
	}
	#endregion

	#region Difficulty Settings-------------------------------------------------------------------------------------------------------------------------------
	private void ControlDifficulty()
	{
		if (currentDifficultyBracket != difficultyBrackets.Length) // if we didn't reach our last bracket
		{
			difficultyCounter++; // increase difficulty counter
			if (difficultyCounter == difficultyBrackets[currentDifficultyBracket]) // if we reach our current bracket
			{
				currentDifficultyBracket++; //increase our current bracket
				difficultyCounter = 0; //reset our counter
				colorBase.IncreaseDifficulty();// add another color in our color pool
				print("NEW COLOR!");
			}
		}
	}
	
	private void ResetDifficulty()
	{
		difficultyCounter = 0;
		currentDifficultyBracket = 0;
		colorBase.ResetToDefault();
	}
#endregion

	/// <summary>
	/// Starts floating text animation for every block that is going to be exploded.
	/// </summary>
	/// <param name="block">block to be exploded.</param>
	/// <param name="points">points to be shown.</param>
	private void StartTextAnimation(Block block, int points)
	{
		Animator an = block.GetComponentInChildren<Animator>();
		Text tx = block.GetComponentInChildren<Text>();
		tx.color = block.info.BlockColor;
		tx.text = "+" + points.ToString();
		an.SetTrigger("startFloat");
	}
	#region PowerUp Section-------------------------------------------------------------------------------------------------------------------------------
	public void HammerPowerUp()// called from Hammer Button
	{
		//only enable hammer power up button to be clicked if there are any colored blocks exist in grid
		if (gameState == GameState.Idle)
		{
			if (IsAnyColoredBlockExist())
				gameState = GameState.HammerPowerUp;
		}

	}

	public void HintPowerUp()// called from Hint Button
	{
		if (gameState == GameState.Idle)
		{
		CreateBlocks(CreationType.Hint);
		isHintUsed = true;
		}
	}

	public void SkipPowerUp()// called from Skip Button
	{
		if (gameState == GameState.Idle)
			CreateBlocks(CreationType.Actual);
	}
	/// <summary>
	/// Checks whole grid to see if there are any colored block exists, terminates check immeditely when it founds a colored block
	/// </summary>
	/// <returns></returns>
	private bool IsAnyColoredBlockExist() // this check is needed for hammer powerup
	{
		bool rBool = false;
		for (int i = 0; i < Constants.ColumnCount; i++)
		{
			if (rBool)
				break;
			for (int j = 0; j < Constants.RowCount; j++)
			{
				if (blocks[j, i].GetComponent<Block>().info.BlockColor != ColorBase.defaultColor) // if color of block is not white
				{
					rBool = true;
					break;
				}
			}
		}
		return rBool;
	}
	#endregion
	#region Score section-------------------------------------------------------------------------------------------------------------------------------
	

	/// <summary>
	/// Sets score either with animation or directly.
	/// </summary>
	/// <param name="newScore">new score</param>
	/// <param name="isAnimated">is score should be animated.</param>
	private void SetScore(int newScore, bool isAnimated)
	{
		if (isAnimated)
		{
			updatedScore = newScore;
			isScoreIncreased = true;
		}
		else
		{
			score = newScore;
			updatedScore = newScore;
			scoreText.text = newScore.ToString();
		}
	}
	/// <summary>
	/// Adds to total score
	/// </summary>
	/// <param name="addition"></param>
	private void AddToScore(int addition)
	{
		SetScore(updatedScore + addition, true);
	}
	#endregion

	void Update()
	{
		//Score Text Animation
		if (isScoreIncreased)
		{
			score = Utilities.IntLerp(score, updatedScore, 0.03f);
			scoreText.text = score.ToString();
			if (score == updatedScore)
				isScoreIncreased = false;
		}
	}
}
