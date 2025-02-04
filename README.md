# RtanNDungeon
This is a personal project done at the Sparta BootCamp. Learned and applied how to utilize C# codes in real project


### Overview
- Period: 25.02.03(MON) - 25.02.05(WED)
- Participants: Alone
- Description:
  This is a personal project done at the Sparta BootCamp. To learn and apply C# codes and their function, built and wrote a script of a TEXT RPG.
  Followed Team Sparta's instruction for necessary functions, and tried more for additional functions written on instruction page.


### Necessary Functions
#### Start Page

#### Check Status Page

#### Check Inventory Page & Manage Equipment Page

#### Visit Shop Page & Buy Item Page 


### Additional Functions
####


### Trouble Shootings
#### Variable Initiating Layer
- Bug: Had a bug on inventory page, marking whether the item is equiped or not. First and second items were marked as equiped as intended. They were default items given to the player from the beginning. However, third and forth items were marked as equiped, though they were not equiped yet. And their marking followed the mark of second item.
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
