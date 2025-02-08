<Korean>

<div align="center">
  
# RtnaNDungeon - 콘솔 텍스트 RPG
스파르타 내일배움캠프에서 개인 과제로서 진행한 프로젝트입니다.
C# 코드를 작성하고 다양한 기능을 활용하는 방법을 배우고 연습하기 위한 목적으로 제작했습니다.
  
</div>

---

## 프로젝트 개요
- **기간**: 25.02.04 월 - 25.02.05 수 (3일)
- **참여**: 손치완
- **설명**:  
  스파르타 내일배움캠프에서 개인 과제로서 진행한 텍스트 RPG 만들기 프로젝트입니다.  
  C#의 코드와 기능들을 배우고 응용하기 위한 목적으로 진행했으며, 텍스트 RPG 전체 스크립트를 구성하고 작성했습니다.  
  팀 스파르타의 지시 사항에 따라 핵심 기능을 구현했으며, 도전적인 추가 기능까지 구현을 시도했습니다.

---

## 프로젝트 타임라인

---

## 게임의 주요 기능
#### 필수 기능
1) **시작 화면 구성**: 게임 시작, 캐릭터 생성, 시작 메뉴 출력.
2) **스테이터스 확인 화면 구성**: 캐릭터 스테이터스 확인, 레벨 증가 및 장비 장착에 따른 스테이터스 변화 반영.
3) **인벤토리 확인 화면 구성**: 캐릭터 인벤토리 확인, 장착 가능한 장비 장착 및 해제, 장착 중인 아이템 표시.
4) **상점 화면 구성**: 아이템 구매, 구매한 아이템 인벤토리 이동, 구매한 아이템 표시.

#### 추가 (1) 아이템 데이터베이스
- 정적 클래스를 활용한 아이템 데이터베이스 생성함. 아이템을 아이템 이름을 key로 하고 아이템 클래스의 오브젝트를 value로 하는 딕셔너리 형태로 저장함.
- 아이템을 개별적으로 혹은 리스트를 전체적으로 호출할 수 있는 메서드를 작성함. `GetItem(string name)` 메서드를 통해 다른 클래스에서도 개별 아이템을 자유롭게 불러올 수 있음. `GetAllItems()` 메서드를 통해, 게임에서 사용될 수 있는 모든 아이템을 리스트의 형식으로 불러올 수 있음. 후자의 메서드를 사용해 상점에서 구매할 수 있는 아이템의 목록을 불러와 저장함.

#### 추가 (2) 여관 방문
- 여관 방문 페이지를 추가하고, 여관에서 휴식을 취하는 메서드를 추가함.
- 만약 플레이어가 여관에서 휴식을 취하려고 하면: 🔵 a. 플레이어가 충분한 골드(100G)를 소유하고 있는지 확인함. (만약 소유하고 있지 않다면, '잔고가 부족하다'는 경고 메시지를 출력하고 여관 페이지로 되돌아감.) 🔵 b. 플레이어의 체력이 최대치(100)인지 확인함. (만약 최대치라면, '체력이 최대치이기 때문에 휴식할 수 없다'는 경고 메시지를 출력하고 여관 페이지로 되돌아감.) 🔵 c. 위의 조건을 모두 만족한 경우, 플레이어의 잔고(`player.Gold`)로부터 100을 차감하고, 플레이어의 체력(`player.Health`)를 최대치로 회복함. 이후 완료 메시지를 출력하고 여관 페이지로 되돌아감.

#### 추가 (3) 상점 - 아이템 판매
- 상점 페이지 아래에 아이템 판매 페이지를 생성하고, 플레이어의 인벤토리에서 아이템을 판매할 수 있는 기능을 추가함.
- 인벤토리 확인 페이지와 동일한 `ShowItemList()` 메서드를 사용해 판매할 수 있는 아이템 목록을 출력하되, `isSelling`이라는 진리변수를 추가해 아이템의 가격을 출력하도록 변경함. `isSelling == true;`인 경우에만 아이템 각 항목의 마지막에 아이템의 가격을 함께 출력함.
- 아이템의 인덱스 번호를 입력해, 플레이어는 원래 가격의 80%로 아이템을 판매할 수 있음. 판매하기 전에 플레이어가 아이템을 착용하고 있는지 먼저 확인하고, 만약 착용하고 있다면 판매할 수 없다는 경고 메시지를 출력함.

#### 추가 (4) 던전 입장 및 레벨 업
- 플레이어는 세 개의 난이도(Easy, Normal, Hard)로 구분된 던전에 입장할 수 있으며, 던전을 클리어한 경우 보상과 경험치를 받을 수 있음.
- `Dungeon` 클래스와 `EnterDungeon()` 메서드를 추가해 던전 기능을 제어함.
- 플레이어가 던전에 입장하려고 하면, 난이도를 선택할 수 있도록 메시지를 출력함. 던전은 플레이어의 레벨에 따라 클리어에 필요한 방어력의 수치와 클리어 보상이 동적으로 변화함.
- 플레이어가 던전을 클리어한 경우, 랜덤한 데미지와 보상, 경험치를 얻을 수 있음. 클리어에 실패한 경우, 데미지만 받게 됨(50). 플레이어의 체력이 0 이하로 떨어지는 경우, 게임 오버 페이지로 이동하며 게임을 재시작할 것인지 물음.

#### 추가 (5) 데이터 저장 및 로드
- 게임 시작 메뉴에서 데이터를 저장할 수 있는 기능과, 게임이 시작되었을 때 저장 파일에서 데이터를 불러올 수 있는 기능을 추가함.
- `Player` 클래스에 `SaveData()` 메서드를 추가해, 플레이어의 여러 데이터가 `save.txt` 파일에 저장됨.
- 게임을 시작할 때, 저장된 데이터가 있는지 체크하고 불러오는 기능을 추가함. 🔵 a. 현재 디렉토리 내에 `save.txt` 파일이 있는지 확인함. 🔵 b. (만약 파일이 있다면) 플레이어에게 데이터를 불러올 것인지 물음. 만약 불러오겠다고 하면, 미리 생성되어 있던 기본 `player` 오브젝트에 불러온 데이터를 덮어씌움. 만약 불러오지 않는다면 새로운 캐릭터를 생성하는 과정을 진행함.

---

## 트러블 슈팅
#### (1) 변수 초기화 위치
- **문제**: 인벤토리 페이지에서 아이템이 장비되었는지 아닌지를 출력하는 변수에 버그가 발생함. 첫번째와 두번째 아이템은 의도한 대로 장비된 아이템을 뜻하는 `(E)`가 표시되지만, 세번째 및 네번째 아이템은 아직 장비되지 않았음에도 장비되었다고 표시됨. 또한, 두번째 아이템의 장비 현황에 따라 변화하는 문제가 있음.
- **원인**: 아이템의 장비 현황을 표시하기 위해 선언한 변수가 `for`문 안에서 초기화되지 않음. `for`문 밖에서 초기화되었기 때문에, 인벤토리 내의 아이템 각 항목을 출력할 때마다 초기화되지 않아서 기존의 변수 값을 그대로 사용하게 됨.
- **해결**: 각 변수가 `for` 반복 안에서 초기화되도록 변경함.
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

#### (2) `Use()` 메서드와 `UnUse()` 메서드 병합
- **문제**: `IEquipable` 인터페이스 내에 거의 같은 로직을 공유하는 `Use()` 및 `UnUse()` 메서드가 존재함. 거의 같은 로직을 공유하기 때문에, 코드가 중복되는 문제가 있음.
- **해결**: `IEquipable` 오브젝트를 사용하는 것인지 아닌지를 구분하는 방향 변수를 추가해, `Use()`와 `UnUse()` 메서드를 하나로 병합함. 변수를 통해 공격력/방어력의 부호와 장비/해제 중 문자열 출력을 결정함.
```
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

---
---

<English>

<div align="center">
  
# RtanNDungeon - Text RPG
This is a personal project done at the Sparta BootCamp. Learned and applied how to utilize C# codes in real project

</div>

---

### Overview
- Period: 25.02.03 (MON) - 25.02.05 (WED)
- Participants: Alone
- Description:
  This is a personal project done at the Sparta BootCamp. To learn and apply C# codes and their function, built and wrote a script of a TEXT RPG.
  Followed Team Sparta's instruction for necessary functions, and tried more for additional functions written on instruction page.

---

### Timeline
- 25.02.03 (MON) PM : Started project
- 25.02.04 (TUE) AM : Finished necesary functions + Fixed some bugs
- 24.02.05 (WED) AM : Finished additional functions + Added game over process
- 24.02.05 (WED) PM : Submitted personal project

---

### Necessary Functions
#### Start Page

#### Check Status Page

#### Check Inventory Page & Manage Equipment Page

#### Visit Shop Page & Buy Item Page 

---

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

---

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
