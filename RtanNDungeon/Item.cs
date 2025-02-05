using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtanNDungeon
{
    // class item
    abstract class Item
    {
        public string Name { get; }  // item name
        public string Desc { get; }  // item description
        public ItemType Type { get; }  // item type
        public int Price { get; }  // item price

        // initialize
        protected Item(string name, string desc, ItemType type, int price)
        {
            Name = name; Desc = desc; Type = type; Price = price;
        }
    }
    // types of item
    enum ItemType { Weapon, Armor, Potion, Misc }

    interface IUsable
    {
        void Use(Player player, bool isSilent = false);
    }

    interface IEquipable
    {
        public bool Equip { get; set; }
    }

    class Weapon : Item, IUsable, IEquipable
    {
        public int Bonus { get; set; }
        public bool Equip { get; set; }

        // initialize // base class Item
        public Weapon(string name, string desc, int bonus, bool equip, int price) : base(name, desc, ItemType.Weapon, price)
        {
            Bonus = bonus; Equip = equip;
        }

        // merged function of use & unuse (equip & unequip) weapon
        public void Use(Player player, bool isSilent = false)
        {
            // convert Equip into marks
            // initializing mark-values to when item is not equiped
            int markBonus = -1;
            string markEquip = "해제";

            // when it is equipment process (from false Equip to true Equip)
            // check if player already equipped weapon (player.equippedWeapon)
            // if Equip, mark minus // if !Equip, mark plus
            if (!Equip)
            {
                if (player.equippedWeapon == null)
                {
                    markBonus = 1; markEquip = "장비";
                    player.equippedWeapon = this;
                }
                else
                {
                    Console.WriteLine($"{Name} 아이템을 장비할 수 없습니다. 이미 무기가 장비되어 있습니다.");
                    return;
                }
            }

            Equip = !Equip;
            player.Attack += markBonus * Bonus;
            // completion message
            // show only !isSilent
            if (!isSilent)
            {
                Console.WriteLine();
                Console.WriteLine($"{Name} 장비를 {markEquip}했습니다. 공격이 {markBonus * Bonus} 만큼 변화했습니다.");
            }
        }
    }

    class Armor : Item, IUsable, IEquipable
    {
        public int Bonus { get; set; }
        public bool Equip { get; set; }

        // initialize // base class Item
        public Armor(string name, string desc, int bonus, bool equip, int price) : base(name, desc, ItemType.Armor, price)
        {
            Bonus = bonus; Equip = equip;
        }

        // use(equip) armor
        public void Use(Player player, bool isSilent = false)
        {
            // convert Equip into marks
            // initializing mark-values to when item is not equiped
            int markBonus = -1;
            string markEquip = "해제";

            // when it is equipment process (from false Equip to true Equip)
            // check if player already equipped weapon (player.equippedWeapon)
            // if Equip, mark minus // if !Equip, mark plus
            if (!Equip)
            {
                if (player.equippedArmor == null)
                {
                    markBonus = 1; markEquip = "장비";
                    player.equippedArmor = this;
                }
                else
                {
                    Console.WriteLine($"{Name} 아이템을 장비할 수 없습니다. 이미 방어구가 장비되어 있습니다.");
                    return;
                }
            }

            Equip = !Equip;
            player.Defense += markBonus * Bonus;
            // completion message
            // show only !isSilent
            if (!isSilent)
            {
                Console.WriteLine();
                Console.WriteLine($"{Name} 장비를 {markEquip}했습니다. 방어가 {markBonus * Bonus} 만큼 변화했습니다.");
            }
        }
    }

    // item database
    static class ItemDB
    {
        private static Dictionary<string, Item> items;

        // initiating
        static ItemDB()
        {
            items = new Dictionary<string, Item>
            {
                { "나무 막대", new Weapon("나무 막대", "길에서 주운 단단한 나무 막대기입니다.", 0, false, 0) },  // default weapon
                { "흰 셔츠", new Armor("흰 셔츠", "집에서 입고 나온 흰 셔츠입니다.", 0, false, 0) },  // default armor
                { "낡은 검", new Weapon("낡은 검", "누군가 사용한 적이 있는 낡은 검입니다.", 2, false, 100) },
                { "천 갑옷", new Armor("천 갑옷", "마을 사람들이 쉽게 만들 수 있는 천 갑옷입니다.", 2, false, 100) },
                { "철검", new Weapon("철검", "마을 대장장이가 만든 가장 기본적인 검입니다.", 5, false, 200) },
                { "나무 단궁", new Weapon("나무 단궁", "한 가지 나무로 만들어진 짧은 활입니다.", 5, false, 200) },
                { "나무 갑옷", new Armor("나무 갑옷", "천 갑옷 위에 부분적으로 나무 판자를 붙여 방어력을 높인 갑옷입니다.", 5, false, 200) },
                { "강철검", new Weapon("강철검", "큰 도시의 대장장이가 강철을 두드려 만든 검입니다.", 10, false, 500) },
                { "고급 나무활", new Weapon("고급 나무활", "마을 밖의 장인이 다양한 재료로 만든 튼튼한 활입니다.", 10, false, 500) },
                { "철 갑옷", new Armor("철 갑옷", "마을 대장장이가 여러 날을 두드려 만든 고급 철판을 덧댄 갑옷입니다.", 10, false, 500) },
                { "사파달의 검", new Weapon("사파달의 검", "사파달 지역에서 사용되었던 것으로 알려진 검입니다. 과거의 유물이라고는 생각할 수 없을 정도로 잘 관리되어 있습니다.", 20, false, 1000) },
                { "컴파운드 보우", new Weapon("컴파운드 보우", "도시의 최신 기술로 만들어진 활입니다. 더 먼 거리까지 더 강력하게 화살을 쏠 수 있습니다.", 20, false, 1000)},
                { "사파달의 갑옷", new Armor("사파달의 갑옷", "사파달 지역에서 사용되었던 것으로 알려진 유물 갑옷입니다. 상체와 급소를 완벽하게 방어하면서도 가볍습니다.", 20, false, 1000) }
            };
        }

        // return one item
        public static Item GetItem(string name)
        {
            // get key return value
            if (items.ContainsKey(name)) { return items[name]; }
            return null;
        }
        // return every items in the dictionary
        public static List<Item> GetAllItems()
        {
            // return only value
            return new List<Item>(items.Values);
        }
    }
}
