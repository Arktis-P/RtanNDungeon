# RtanNDungeon
This is a personal project done at the Sparta BootCamp. Learned and applied how to utilize C# codes in real project


### Overview
- Period: 25.02.03 (MON) - 25.02.05 (WED)
- Participants: Alone
- Description:
  This is a personal project done at the Sparta BootCamp. To learn and apply C# codes and their function, built and wrote a script of a TEXT RPG.
  Followed Team Sparta's instruction for necessary functions, and tried more for additional functions written on instruction page.

### Timeline
- 25.02.03 (MON) PM : Started project
- 25.02.04 (TUE) AM : Finished necesary functions + Fixed some bugs
- 24.02.05 (WED) AM : Finished additional functions + Added game over process
- 24.02.05 (WED) PM : Submitted personal project


### Necessary Functions
#### Start Page

#### Check Status Page

#### Check Inventory Page & Manage Equipment Page

#### Visit Shop Page & Buy Item Page 


### Additional Functions
#### (1) Item Databse
- Added item database using static class. This keeps list of items into a dictionay, which has item's name as key and item class object as value.
- Added methods to return item object as individual and all items in list.
  With method `GetItem(name)`, can load individual item object freely in other classes.
  With method `GetAllItems()`, can load list of all items in game as List. Using this list, can initialize list of shop's items.

#### (2) Visit Inn
- Added Visit Inn page and a method to take a rest.
- If player choose to take a rest at Inn,
  first, checkes if player has enough moeny (100 G); (if not, game prints out warning message and returns to Visit Inn page)
  second, checkes if player's health is not max (100); (if max, game prints out warning message and returns to Visit Inn page)
  third, (if player has enough money and less health than max), take 100 G from player's gold, restore player' health to max, and prints completion message, then returns to Visit Inn page.

#### (3) Sell Item
- Added sell item page and functions to sell items from player's inventory.
- On sell item page, shows player's inventory like inventory checking, but added `isSelling` in `ShowItemList()` method to show item price when sold.
  If `isSelling == true`, `ShowItemList()` method print item price at the end of each item.
- With the number input, player can sell own item in inventory at 80% of item's original price.
  When player tries to sell item, checks if the item is equipped. (If equipped, player cannot sell the item, and sees caution page.)

#### (4) Enter Dungeon and Level Up
- Player can now enter the dungeon of three different difficulties (Easy, Normal and Hard), and get reward and xp if cleared.
- Added `Dungeon` Class, and `EnterDungeon` method.
- If player decides to enter dungeon, asks to choose difficulty.
  Dungeon has dynamic defense, reward values according to its difficulty and player's level.
- If player clears dungeon, gets random damage, reward and xp.
  If fails to clear, gets only damage.
  If player's health drop below or same as 0, shows game over page, and asks if player wants to retry.

#### (5) Save and Load Data
- Added functions to save data to file and load data file when game is started.
- Added `SaveData()` on `Player` class. Saves every player data in `save.txt` file line by line.
- Added process to check and load saved data. When game is started, 
  first, checks if there is `save.txt` file in the directory.
  second, asks if player wants to load saved data. If yes, load player data and rewrite player object. If no, do not load player data and generates new player object.


### Trouble Shootings
#### (1) Variable Initiating Layer
- Problem: Had a bug on inventory page, marking whether the item is equiped or not. First and second items were marked as equiped as intended. They were default items given to the player from the beginning. However, third and forth items were marked as equiped, though they were not equiped yet. And their marking followed the mark of second item.
- Cause: Found variables to mark as equiped were not in the individual roop of `for` block to show item list in inventory.
- Solution: Changed layer of variables to each `for` roop. Put them inside of each `for` loop from outside of `for` block.
  ```
  for (int i = 0; i < inven.Count; i++)
  {
    // initiating variables
    string itemNumber = "-";
    string equiped = "";
    string bonusLabel = "";

    // check if manage page
    if (isManaging) { itemNumber = $"[{i + 1}]"; }
    // check if equiped already
    if (inven[i] is IEquipable equipItem && equipItem.Equip) { equiped = "(E)"; }
    // check item bonus
    if (inven[i] is Weapon weaponItem) { bonusLabel = $"공격 +{weaponItem.Bonus}"; }
    else if (inven[i] is Armor armorItem) { bonusLabel = $"방어 +{armorItem.Bonus}"; }

    Console.WriteLine($"{itemNumber} {equiped}{inven[i].Name}\t| {bonusLabel}\t| {inven[i].Desc}");
  }
  ```
- Lessons: Have to check well the layers where variables are initiated.

#### (2) Merging `Use()` and `UnUse()`
- Problem: In `IEquipable` interface, there were two methods (Use(player) and UnUse(player) which had almost same functions. Because these two methods were devided into two, the script became longer and had to write almost same code twice.
- Solution: Merged two methods into one, Use(player), and added marks to check whether player is using or unusing the `IEquipable` item. Added `markBonus` to convert +/- and `markEquip` to convert string equiped/unequiped.
  ```
  // merged function of use & unuse (equip & unequip) weapon
  public void Use(Player player)
  {
    // convert Equip into marks
    // initializing mark-values to when item is not equiped
    int markBonus = -1;
    string markEquip = "해제";
    // if Equip, mark minus // if !Equip, mark plus
    if (!Equip) { markBonus = 1; markEquip = "장비"; }
    
    Equip = !Equip;
    player.Attack += markBonus * Bonus;

    Console.WriteLine($"{Name} 장비를 {markEquip}했습니다. 공격이 {markBonus * Bonus} 만큼 변화했습니다.");
  }
  ```
- Lessons: First write codes robust, then refactoring it to make entire script compact.
