# About
[![MIT](https://img.shields.io/badge/license-MIT-green.svg?style=plastic)](https://opensource.org/licenses/MIT)
[![MIT](https://img.shields.io/badge/engine-Unity3D-blue.svg?style=plastic)](http://unity3d.com)
![MIT](https://img.shields.io/badge/platform-Android-green.svg?style=plastic)
![MIT](https://img.shields.io/badge/language-CSharp-orange.svg?style=plastic)

This is a clone of [Squares Game](https://play.google.com/store/apps/details?id=com.sm.squares), made for fun, practice and educational purposes. This place will be your general guideline. Project files include detailed commenting and explanation but I highly recommend you to check this place first.

Sounds in the project are courtesy of [Yagızcan Mutlu](https://soundcloud.com/ya-zcan-mutlu) (most of them are not very delicately processed).

Images are mostly my own creations or modified versions of images that are under "free for commercial use"

## Setup in Unity

Skip these steps if you know what you are doing.

###### Opening Project
* Open Unity, if you are on landing page, select **OPEN**
* If you are within Unity **File>Open** will give you landing page
* Select the project folder(it should contain **ProjectSettings** and **Assets** folder) and open it

###### Setting Up Project
* Select **File>Build Settings**
* Select **Android** in Build Settings Window
* Click **Switch Platform**
* Unity will switch platform to **Android**
* Close **Build Settings**
* Open **Assets>Scenes>main** in project explorer
* Under **Game** panel, set aspect ratio **16:10 Portrait (10:16)** or any Portrait ratio

###### Building APK file
* You need to have Android SDK and JDK installed in your computer in order to get an APK build.
* Unity will ask for SDK and JDK paths if you try to build an APK.
* You can download/select your SDK and JDK files from **Edit>Preferences>External Tools**
* Enlarge the opened window a little bit to see SDK and JDK paths, if they are null you need to either give path by clicking to Browse button or download if you don't have them.
* If you have SDK and JDK, you can Build the game with **File>Build Settings>Build**

You are good to go! Now play the game before digging the codes to understand the game.

## Glossary

Some of terms might be confusing without visualisation(like adjacent same-color blocks), so here is a little bit information before reading furthermore.

![alt text](https://github.com/dogukanerkut/squares/blob/master/documentation%20pictures/overallgame.png?raw=true "Overall Game Guide")

Also,

| Definition | Description |
| ---------- | ----------- |
| New Blocks | New blocks appear every time player puts them |
| BlockInfo  | Data class that holds basic block info(row, column etc)
| Block      | MonoBehaviour inherited class that represents both Block game object and BlockInfo class |


# Code Guideline

This section will explain some of important classes and their functions in general. Note that not all classes are going to be explained here in details, as some of them are self-explanatory or easy to understand.

## Input System

Player input is handled by Unity's `EventSystem`. Each block has an `Event Trigger` component (see **Prefabs>Block**). You can see  `Pointer Enter` and `Pointer Down` events are hooked. `OnPointerEnter` is called by the by the `EventSystem` when the pointer(mouse or touch) enters the object associated with this `EventTrigger`. `OnPointerDown` is called by the `EventSystem` when a `PointerDown`(when touched or mouse pressed) event occurs. These events fires `OnPointerEnter` and `OnPointerDown` methods in `EventController.cs`. The only job of this class is to receive event calls and inform our `BoardManager.cs` which will be explained in a bit.


## `BlocksArray.cs`

This class handles all main activities that game can make. Those activities are:

* Detect adjacency of blocks
* Detect grid's availability for user input(placing new blocks, detecting empty blocks etc.)
* Detect if grid has empty blocks for new blocks to be put into

##### Concerns
Detecting:

* **if there are atleast 3 adjacent same-color blocks to eachothers**
* **if there are no place left to put new blocks(for game over)**

One can think that we can achieve second condition if we achieve the first one, because basically it is almost same thing. If the game's rule is to be able to put maximum of 3 blocks we were totally fine, we could achieve second condition if we could achieve one. But game's rule is to be able to put maximum of 4 blocks at a time. Therefore, we need an additional control. But why and how?

##### Solutions

First solution is simple: **[Flood-Fill algorithm](https://en.wikipedia.org/wiki/Flood_fill)**

A simple explanation of algorithm is basically give algorithm a block to start, and it will look adjacent blocks(left, right, up and down) to see if they are same color as initial block. If it is, look for their adjacent blocks too, and this goes like this until the condition is not met(not same color). This is called a **recursive method/function**. The algorithm applied in `Check3Match`, stores list of adjacent same-color blocks found to the list seperately, named `matchedBlockLists`. It is a `List<List<Block>>`, a list that holds lists of adjacent blocks. These pictures should make this a bit clearer to grasp:

![alt text](https://github.com/dogukanerkut/squares/blob/master/documentation%20pictures/Check3Match_iteration.png?raw=true "Visualisation of Check3Match Iterations")

![alt text](https://github.com/dogukanerkut/squares/blob/master/documentation%20pictures/matchedBlockLists_visualisation.png?raw=true "matchedBlockLists' collected block list")

So `matchedBlockLists[0]` is blue blocks(red container) and `matchedBlockLists[1]` is green blocks(blue container).

Now to the reason why only using this control is not enough to control **if there are no place to left to put new blocks(for game over)**. To reveal the problem check these pictures:


![alt text](https://github.com/dogukanerkut/squares/blob/master/documentation%20pictures/no%20place%20to%20put%20blocks(4)marked.png?raw=true "Red container shows there are 4 empty blocks but player can't place to there")

![alt text](https://github.com/dogukanerkut/squares/blob/master/documentation%20pictures/no%20place%20to%20put%20blocks(5)marked.png?raw=true "Red container shows there are 5 empty blocks but player still can't place to there")

As you can see, there are 4 or 5 empty blocks but player can't place new blocks. `Check3Match` method will not be able to detect that in this kind of situation that 4 blocks can not be placed. Because it sees 4 or 5 adjacent blocks(respectively) are available and will say that we can put blocks. Nevertheless, we can use this method to do some pre-checking and preparation for deep check:

* Detect empty blocks and store them seperately in `matchedBlockLists`
* Can detect empty blocks properly if new blocks are smaller that 4(no need to use deep check)
* Lastly, does a simple if-check that when 4 new blocks appear if adjacent empty blocks in grid are atleast 6. If this condition met, it is certain for player to be able to put new blocks in grid without a problem(no need to use deep check)

If all above conditions are failed, `AdvancedEmptyBlockCheck` method comes in. This code only works if adjacent empty block count is atleast 4 and new block count is atleast 4. Code takes a single reference point and starts to iterate it's adjacent empty colors. Now in codes, there is a term `deadEnd` which is to indicate if the block is dead end(surrounded by colored blocks and nothing else). This code adds it's adjacent blocks one by one in order to ensure a single path is taken. It marks dead end blocks, remove it from list(code can't continue on this path anymore because that path does not contain atleast 4 adjacent blocks) and continue checking. If there are no available check for this reference point(initial block) method refreshes all variables and starts a clean control from another reference point. This continues until the method iterates through all possible paths in all reference points, meaning it either will find a single path with 4 blocks or there is no empty blocks enough left for player.

![alt text](https://github.com/dogukanerkut/squares/blob/master/documentation%20pictures/no%20place%20to%20put%20blocks(5)deep%20check.png?raw=true "Red container shows there are 4 empty blocks but player can't place to there")
--------
![alt text](https://github.com/dogukanerkut/squares/blob/master/documentation%20pictures/no%20place%20to%20put%20blocks(5)deep%20check%20found.png?raw=true "Red container shows there are 4 empty blocks but player can't place to there")

Eventually code will check path shown in above picture and accept that player can place blocks.


## `BlockInfo.cs` and `Block.cs`

`BlockInfo.cs` is the data class that holds basic information of blocks(row, column, block color etc.). `Block.cs` is inherited from MonoBehaviour so it can be attached to block game objects(See **Prefabs>Block**), it is basically `BlockInfo.cs`'s representation class to Unity. `BlockInfo.cs` is reached through `Block.cs`. There are 2 reasons for me to seperate them:

* `BlocksArray.cs` mainly utilizes `BlockInfo.cs`, which is a lean class so I believe it's more performant than doing with a MonoBehaviour inherited class
* Persistancy, a class that is inherited from MonoBehaviour can not be saved(without any external tools or custom works anyway) because MonoBehaviour can't be serialized.



## `BlocksCreator.cs` and `ColorBase.cs`

`BlocksCreator.cs`'s job is to create list of `BlockInfo.cs` randomly. Random values are adjusted so one or four blocks' chance to appear(35%) is lower than two and three blocks(65%). Because one block is fairly simple and 4 blocks is fairly hard, so we make them appear less than moderate blocks.

`ColorBase.cs`'s job is to get created blocks and colourize them. Game starts with 3 colors and can become up to 8 colors at total. Every time new blocks are created from `BlocksCreator.cs`, `ColorBase.cs` colourizes them according to current difficulty bracket. Difficulty brackets can be managed in BoardManager game object(see it in hierarchy **__Managers>BoardManager>Difficulty Brackets in Inspector**). Difficulty is managed in `BoardManager.cs` which will be explained in further. Colourization process is wildly random, there is no control over it except 2 rules:

* If new blocks are 4, all of them can not be same color
* When difficulty is just increased, include the newly added color if it's not already added

These classes can be merged into one but I decided to keep them seperately. 

## `SaveLoad.cs` and `SaveLoadManager.cs`

Saving and loading process ir rather simple. If any save data exist, `SaveLoadManager.cs` loads data on `Awake()` and data is saved on `OnApplicationPause()` if it's android(see [MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html), [OnApplicationPause](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html), [OnApplicationQuit](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html)).

All data that needs to be saved and loaded(grid, new blocks, diamonds, score, high score etc.) are wrapped in a `[System.Serializable]` attributed classes such as `BlockInfo.cs` and `GameVariables.cs`. These data are serialized and deserialized with [BinaryFormatter](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary.binaryformatter(v=vs.110).aspx). Note that this comes with a drawback: It requires [WRITE_EXTERNAL_STORAGE](https://developer.android.com/training/basics/data-storage/files.html) permission which may be undesirable in some cases. Saving and Loading could also be done with [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html) but its very primitive and wouldn't meet our needs. Another way of doing it is to write a java plugin and use it, but unfortunately that's a topic that I can't cover.

## `BoardManager.cs`

This is main class that manages all of board related processes(placing blocks, bringing new blocks, updating score, difficulty managing, etc.). There is not much complexity here, it uses all of previously explained base classes and reflects them to in-game board. I think it is very organized, and can be easily explored by closing all #regions and checking part by part. Some of it's parts could be seperated to different classes but since it is rather simple game, I thought keeping them in one class would not hurt.

## `UIManager.cs`

I wanted to try Unity's new Event system  that's why this class basically exists. BoardManager game object(see **Managers>BoardManager** in hierarchy) has a UnityEvent which listens to `GameOver()` method in `UIManager.cs`. It's pretty cool to hook events in Editor any use it somewhere.

# Customization

**Warning:** Before doing any customization, note that game is saving itself and some changes may not take effect because game will load from previous session; for this reason please disable loading game. To do so, go hierearchy **__Managers>SaveLoadManager>Untick "Load Data"**. Game is being saved in `Application.persistantDataPath\gamesave.data` which is in **Application.persistantDataPath which is in C:\Users\[user]\AppData\LocalLow\dogukanerkut\Squares_Demo\gamesave.data**.

Here is some of the things you can do to change the ruleset of the game:

##### Changing Grid Size

In hierarchy, go **__Managers>BoardManager>Grid Size** currently, Grid Layout is adjusted automatically same as **X** of **Grid Size**. If you want to, lets say 10x10 grid, you might want to change block Size, **GameBoardCanvas>GamePanel>Cell Size**. 32x32 **Cell Size** is proper with 10x10 grid. But animations are arranged according to default value(64x64). If you play with a different **Cell Size**, you will get an error about updating animations.

##### Increasing Number of Maximum New Blocks(Currently 4)

* In hierarchy, go **GameBoardCanvas>ScoreAndNewBlocksPanel>NewBlocksPanel** and duplicate one of **BlockVisualsOnly**(with CTRL+D in windows and I believe Command+D in Mac or right click>Duplicate) as much as you want.
* You need to change ruleset yourself if you want to have different size of new blocks, but to try if it works, go `BlocksCreator.cs` one of lines(34,36,46,48) that is like `blockGroup = GetBlocks([number]);`. Change any lines that is like this line with desired number(if you increased new blocks number to 7, you need to have a `blockGroup = GetBlocks(7);`

##### Change Colors of the Game

Go `ColorBase.cs` and change colors there. I didn't need to adjust them on-the go so I didn't make it to be able to change in Editor. `currentColors` is the current color of the game, it starts with 3 colors and as the difficulty increases, `BoardManager.cs` ask `ColorBase.cs` to add another color in the queue in `difficultyColors`.

##### Changing Default Block Color

In hierarchy, go **__Managers>BoardManager>Default Block Color**.

##### Changing Difficulty Progression & Increasing/Decreasing Brackets

In hierarchy, go **__Managers>BoardManager>Difficulty Brackets**. Every time player does atleast 3 same-color match, `BoardManager.cs` increases `difficultyCounter` by one. if `difficultyCounter` reaches to one these Difficulty Brackets, a new color is introduced to color pool and therefore difficulty is increased. You can adjust brackets here if you find game's difficulty is increased too fast/slow.

If you want to add or remove a bracket, increase/decrease the size of the array in editor and also update either add or remove a `difficultyColors.Add` line in `ColorBase.cs`.

##### Currency(Diamond) Settings

Currently, player gains 25 diamond every 10 times he/she matches atleast 3 same-color blocks. The gaining rate is multiplied by itself, so if player does 2 or 3 matches in one turn, gain is 4 or 9 respectively. You need to re-code this if you want to have a different kind of diamond gain system. Other than that, you can change the "every 10 times" in **__Managers>BoardManager>Matches Needed to Give Diamond** you can also change **Diamond Reward** and **Power Up Diamond Price** here. By default, price of using Power Ups is 250 diamond.

# Known Bugs

If the board has, lets say 2 adjacent empty blocks left, and if Skip is used in this situation, player may receive new blocks more than 2 blocks, if it happens the game will stuck. A quick fix would be to use `blocks.CheckEmptyBlocks(newBlocks.Count))` after Skip is used and call game over if it returns false but that means if the situation occurs again like that, game will be over and basically you punish player because of using Skip. A smarter thing to do is prevent generating blocks(when skip is used) that are more than how many adjacent blocks currently grid is containing. I didn't bother doing it, well because original game has this issue as well.

# License

The MIT License (MIT)

Copyright (c) 2016 Doğukan Erkut

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.