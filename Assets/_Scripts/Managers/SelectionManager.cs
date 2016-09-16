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
[System.Serializable]
public class GridSize
{
	public int X;
	public int Y;
}
public class SelectionManager : MonoBehaviour
{
	//Grid
	public GridSize gridSize;
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
	public Text highScoreText;
	private int score;
	private int highScore;
	private int updatedScore;
	private bool isScoreIncreased;
	//Combo and New Color Text
	public Text comboText;
	public Animator newColorAnim;
	public Animator newColorTextAnim;
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

	#region integration safecheck variables
	//hardcoded default values of Game Panel's Grid Layout Group's spacing and cellsize
	float defaultSpacingX = 8;
	float defaultSpacingY = 8;
	float defaultCellSizeX = 64;
	float defaultCellSizeY = 64;
	#endregion

	#region Unity MonoBehaviours -------------------------------------------------------------------------------------------------------------------------------
	void Awake()
	{
		#region safecheck
		if (colorBase.GetTotalDifficulty() != difficultyBrackets.Length)
			Debug.LogError("Difficulty Brackets should be equal with Difficulty Color Count!\nDifficulty Brackets = " + difficultyBrackets.Length +
						  "\nDifficulty Color Count = " + colorBase.GetTotalDifficulty() + "\nFix Difficulty Brackets Size in " + gameObject.name +
						  " or Add/Deduct Difficulty Colors in " + colorBase.ToString() + " class");
		#endregion
		Constants.RowCount = gridSize.X;
		Constants.ColumnCount = gridSize.Y;
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

		#region safecheck
		if (gamePanelGLG.spacing.x != defaultSpacingX || gamePanelGLG.spacing.y != defaultSpacingY)
		{
			Animator anim = gamePanelGLG.GetComponent<Animator>();
			Debug.LogError("Please update Animator component of [" + anim.runtimeAnimatorController.name + "]'s Board Reset Animation's X and Y spacing values, and [defaultSpacingX] and [defaultSpacingY] values of [" + this.name + " Class]");
		}
		if (gamePanelGLG.cellSize.x != defaultCellSizeX || gamePanelGLG.cellSize.y != defaultCellSizeY)
		{
			Animator anim = gamePanelGLG.GetComponent<Animator>();
			Debug.LogError("Please update [" + anim.runtimeAnimatorController.name + " Animator]'s Board Game Over Animation's X and Y cell size values, and [defaultCellSizeX] and [defaultCellSizeY] values of [" + this.name + " Class]");
		}
		#endregion
	}

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
	#endregion

	#region Save & Load Operations -------------------------------------------------------------------------------------------------------------------------------
	public void FillBlocksArray(BlocksArray loadedBlocks, List<BlockInfo> currentBlockInfos)
	{
		if (loadedBlocks != null)
			blocks = loadedBlocks;
		if (currentBlockInfos != null)
			CreateNewBlocksFromSave(currentBlockInfos);
	}
	public List<BlockInfo> GetBlocks(BlockCreationType type)
	{
		List<BlockInfo> blocks = new List<BlockInfo>();
		switch (type)
		{
			case BlockCreationType.Actual:
				blocks = newBlocks;
				break;
			case BlockCreationType.Hint:
				blocks = hintBlocks;
				break;
			default:
				break;
		}
		return blocks;
	}

	private void CreateNewBlocksFromSave(List<BlockInfo> blockInfos)
	{
		newBlocks = blockInfos;
		newBlockImages = ProcessBlocks(newBlockImages, newBlocks);
	}
	public void CreateHintBlocksFromSave(List<BlockInfo> blockInfos)
	{
		hintBlocks = blockInfos;
		hintBlockImages = ProcessBlocks(hintBlockImages, hintBlocks);
	}

	public BlocksArray GetBlocksArray() { return blocks; }

	public GameVariables GetGameVariables()
	{
		bool isHammerUsed = gameState == GameState.HammerPowerUp ? true : false;
		return new GameVariables(updatedScore, highScore, difficultyCounter, currentDifficultyBracket, (int)gameState, isHammerUsed, isHintUsed);
	}

	public void SetGameVariables(GameVariables gameVariables)
	{
		SetScore(gameVariables.score, false);
		UpdateHighScore(gameVariables.highScore);
		difficultyCounter = gameVariables.difficultyCounter;
		currentDifficultyBracket = gameVariables.currentDifficultyBracket;
		gameState = (GameState)gameVariables.gameStateIndex;

		if (gameVariables.isHammerUsed)
			HammerPowerUp();
		isHintUsed = gameVariables.isHintUsed;
		colorBase.IncreaseDifficulty(currentDifficultyBracket);
		if (gameState == GameState.GameOver)
		{
			gameOverEvent.Invoke();
		}

	}
	#endregion

	#region Grid Management & Block Creation -------------------------------------------------------------------------------------------------------------------------------
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
		#region safecheck
		if (blockPrefab.GetComponent<Image>().color != Color.white)
			Debug.LogError("Please set [" + blockPrefab.name + " Prefab]s Color attribute back to white. You can change color of blocks in [" + gameObject.name + " Game Object]'s [Default Block Color] property in Editor." );
		#endregion
		//blocks[2, 2].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[2, 3].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[3, 3].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		////blocks[2, 4].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[0, 3].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[1, 3].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[1,2].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[1, 4].GetComponent<Block>().SetColor(ColorBase.defaultColor);

		//blocks[0, 0].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[1, 0].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[2, 0].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[3, 0].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[3, 2].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[1, 3].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[1, 2].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[2, 2].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[2, 1].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[0, 0].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[0, 1].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[0, 2].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		//blocks[0, 3].GetComponent<Block>().SetColor(ColorBase.defaultColor);
		CreateBlocks(BlockCreationType.Actual);
	}
	//Color RandomColor()
	//{
	//	return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
	//}
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
		UpdateHighScore(); // update highscore
		SetScore(0, false); // save if highscore and set score to 0
		ResetDifficulty(); // resets difficulty
		CreateBlocks(BlockCreationType.Actual); // create new blocks
		Animator boardAnimator = gamePanel.GetComponent<Animator>();
		boardAnimator.SetTrigger("reset");
	}

	public enum BlockCreationType
	{ Actual, Hint }
	/// <summary>
	/// Create new blocks, assign their colors and arrange the new block images.
	/// </summary>
	/// <param name="type">Actual creates new blocks, Hint creates Hint blocks</param>
	void CreateBlocks(BlockCreationType type)
	{
		List<BlockInfo> blocksInfo = new List<BlockInfo>();
		Image[] blockImages = new Image[0];
		switch (type) //select which blocks to process
		{
			case BlockCreationType.Actual:
				blocksInfo = newBlocks;
				blockImages = newBlockImages;
				break;
			case BlockCreationType.Hint:
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
			case BlockCreationType.Actual:
				newBlocks = blocksInfo;
				newBlockImages = blockImages;
				break;
			case BlockCreationType.Hint:
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
			blockImages[i].color = blocksInfo[i].BlockColor.GetColor();
		}
		//Sequential new block enabling, did not like it. So it is deactivated, see below explanation for more info
		//blockImages[0].gameObject.SetActive(true);
		//activateBlockIndex = 1;
		//if (blocksInfo.Count > 1)
		//	StartCoroutine(ActiveBlocksWithDelay(blockImages, blocksInfo.Count));
		return blockImages;
	}
	#endregion

	//Sequential new block enabling, did not like it. So it is deactivated.-------------------------------------------------------------------------------------------------------------------------------
	//Activate below and above lines and deactivate "blockImages[i].gameObject.SetActive(true);" on above to see changes.
	//private int activateBlockIndex;
	//IEnumerator ActiveBlocksWithDelay(Image[] blockImages, int maxCount)
	//{
	//	yield return new WaitForSeconds(0.05f);
	//	blockImages[activateBlockIndex].gameObject.SetActive(true);
	//	activateBlockIndex++;
	//	if (activateBlockIndex != maxCount)
	//		StartCoroutine(ActiveBlocksWithDelay(blockImages, maxCount));
	//}

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
			UpdateNewBlocksAnimation(newBlockImages[0].gameObject, true);
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
		selectedBlock.SetColor(newBlocks[selectionCount].BlockColor.GetColor());//Set color of selected block to queued new block
		blocksPlaced.Add(selectedBlock); // add currently selected block to the list of placed blocks(we will need it later)
		selectionCount++; // increase the selection count so next time the queued block will be placed
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
				UpdateNewBlocksAnimation(newBlockImages[selectionCount].gameObject, true);
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
				UpdateNewBlocksAnimation(newBlockImages[selectedBlockIndex+1].gameObject, false);
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
				{
					blocksPlaced[i].Clear();
					UpdateNewBlocksAnimation(newBlockImages[i].gameObject, false);
				}

				SoundManager.Instance.PlayRetrieveBlock();
				SoundManager.Instance.ResetPlaceBlockPitch();
				gameState = GameState.Idle;
			}
			else // player placed all of given blocks to grid
			{
				AppendSelection();

			}
			//regardless of the conditions above, the player is released fingers
			//and we need to reset the selectionCount and blocksPlaced list
			//and set the gamestate back to idle
			selectionCount = 0;
			blocksPlaced.Clear();
		}
	}
	/// <summary>
	/// Stuff to do when player places blocks to grid
	/// </summary>
	private void AppendSelection()
	{
		SoundManager.Instance.ResetPlaceBlockPitch();

		if (blocks.CheckAdjacentBlocks(blocksPlaced)) // send placed block list for adjacency check and if there are any 3 or more adjacent block found
			ExplodeBlocks(); // explode blocks if there are any

		CreateBlocks(BlockCreationType.Actual); // create new blocks to continue the game

		if (blocks.CheckEmptyBlocks(newBlocks.Count)) // Use this method after creating blocks!
			gameState = GameState.Idle;
		else // game over here
		{
			gameState = GameState.GameOver;
			gameOverEvent.Invoke();

			Animator boardAnimator = gamePanel.GetComponent<Animator>();
			boardAnimator.SetTrigger("gameOver");
			SoundManager.Instance.PlayGameOver();
		}
	}
	public void ClearSelection()
	{
		currentBlock = null;
		selectionCount = 0;
	}

	/// <summary>
	/// Removes adjacent blocks, calculates score, shows combo.
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
		// if player does combo, show it
		if (matchedBlocksList.Count > 1)
		{
			comboText.text = "x" + matchedBlocksList.Count.ToString();
			comboText.GetComponent<Animator>().SetTrigger("combo");
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
				newColorAnim.GetComponent<Image>().color = colorBase.GetLatestColor();
				newColorAnim.SetTrigger("newColor");
				newColorTextAnim.SetTrigger("shadow");
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

	#region PowerUp Section-------------------------------------------------------------------------------------------------------------------------------
	public void HammerPowerUp()// called from Hammer Button
	{
		//only enable hammer power up button to be clicked if there are any colored blocks exist in grid
		if (gameState == GameState.Idle)
			ProcessHammerPowerUp(true);
	}

	public void HintPowerUp()// called from Hint Button
	{
		if (gameState == GameState.Idle)
		{
		CreateBlocks(BlockCreationType.Hint);
		isHintUsed = true;
		}
	}

	public void SkipPowerUp()// called from Skip Button
	{
		if (gameState == GameState.Idle)
			CreateBlocks(BlockCreationType.Actual);
	}

	/// <summary>
	/// Checks whole grid to see if there are any colored block exists, terminates check immeditely when it founds a colored block
	/// </summary>
	/// <returns></returns>
	public void ProcessHammerPowerUp(bool isActivate) // this check is needed for hammer powerup
	{
		bool isAnyColoredBlockExist = false;
		for (int i = 0; i < Constants.ColumnCount; i++)
		{
			for (int j = 0; j < Constants.RowCount; j++)
			{
				if (blocks[j, i].GetComponent<Block>().info.BlockColor.GetColor() != ColorBase.defaultColor) // if color of block is not white
				{
					blocks[j, i].GetComponent<Block>().hammerImage.SetActive(isActivate);
					isAnyColoredBlockExist = true;
				}
			}
		}
		if (isAnyColoredBlockExist && isActivate) gameState = GameState.HammerPowerUp;
	}

	/// <summary>
	/// Remove a single block from the grid.
	/// </summary>
	/// <param name="selectedBlock">Block to be removed.</param>
	private void RemoveBlock(Block selectedBlock)
	{
		ProcessHammerPowerUp(false);
		selectedBlock.Clear(); // remove it's colors
		SoundManager.Instance.PlayHammerPowerUp();
		gameState = GameState.Idle;
		hammerUsedEvent.Invoke();
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
	private void UpdateHighScore(int optionalScore = -1)
	{
		if (optionalScore == -1)
		{
			if (updatedScore > highScore)
			{
				highScore = updatedScore;
			
			}
		}
		else
			highScore = optionalScore;

		highScoreText.text = highScore.ToString();

	}
	#endregion

	#region Animations -------------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Updates new blocks animation according to condition.
	/// </summary>
	/// <param name="blockGO">GameObject of block to be updated.</param>
	/// <param name="isPlaced">if is block put on grid or retrieved from it.</param>
	void UpdateNewBlocksAnimation(GameObject blockGO, bool isPlaced)
	{
		Animator anim = blockGO.GetComponent<Animator>();
		if (blockGO.activeSelf)
		{
			anim.SetBool("shrink", isPlaced);
		}
	}

	/// <summary>
	/// Starts floating text animation for every block that is going to be exploded.
	/// </summary>
	/// <param name="block">block to be exploded.</param>
	/// <param name="points">points to be shown.</param>
	private void StartTextAnimation(Block block, int points)
	{
		Animator an = block.GetComponentInChildren<Animator>();
		Text tx = block.GetComponentInChildren<Text>();
		tx.color = block.info.BlockColor.GetColor();
		tx.text = "+" + points.ToString();
		an.SetTrigger("startFloat");
	}
	#endregion

	
}
