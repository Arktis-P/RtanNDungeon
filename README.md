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


### Necessary Functions
#### Start Page

#### Check Status Page

#### Check Inventory Page & Manage Equipment Page

#### Visit Shop Page & Buy Item Page 


### Additional Functions
####


### Trouble Shootings
#### Variable Initiating Layer
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

#### Merging `Use()` and `UnUse()`
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
