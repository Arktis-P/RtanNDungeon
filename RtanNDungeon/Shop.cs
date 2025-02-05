using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtanNDungeon
{
    // class shop
    class Shop
    {
        public List<Item> items;

        // initiate shop
        public Shop()
        {
            items = new List<Item>();
            // get all items in static class ItemDB (one by one)
            foreach (var item in ItemDB.GetAllItems()) { items.Add(item); }
        }
        // add item in shop's item list (manual)
        public void AddItem(Item item) { items.Add(item); }
        // remove item from shop's item list (manual)
        public void RemoveItem(Item item) { items.Remove(item); }
    }
}
