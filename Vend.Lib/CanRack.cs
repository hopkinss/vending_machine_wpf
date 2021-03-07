using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Vend.Lib
{
   
    public class CanRack : INotifyPropertyChanged
    {
        private int maxInventory;
        private Dictionary<int,CanInventory> cans;
        private int cansOfOrange;
        private int cansOfLemon;
        private int cansOfRegular;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CanRack()
        {
            this.cans = new Dictionary<int, CanInventory>();
        }
        public CanRack(int inventory)
        {
            this.maxInventory = inventory;
            this.cans = new Dictionary<int, CanInventory>();
            FillTheCanRack();
        }

        public int MaxInventory
        {
            get { return maxInventory; }
            set { maxInventory = value; }
        }

        public int CansOfOrange
        {
            get { return cansOfOrange; }
            set
            {
                cansOfOrange = value;
                OnPropertyChanged();
            }
        }
        public int CansOfLemon
        {
            get { return cansOfLemon; }
            set
            {
                cansOfLemon = value;
                OnPropertyChanged();
            }
        }
        public int CansOfRegular
        {
            get { return cansOfRegular; }
            set
            {
                cansOfRegular = value;
                OnPropertyChanged();
            }
        }

        public void AddACanOf(Flavor FlavorOfCanToBeAdded)
        {
            if (this.cans.ContainsKey((int)FlavorOfCanToBeAdded))
            {
                if(!IsFull(FlavorOfCanToBeAdded))
                {
                    cans[(int)FlavorOfCanToBeAdded].Amount++;
                    SetCanCount();

                }
            }
            else
            {
                cans.Add((int)FlavorOfCanToBeAdded,new CanInventory(1,new Can(FlavorOfCanToBeAdded)));
            }
            
        }

        public void RemoveACanOf(Flavor FlavorOfCanToBeRemoved)
        {
            if (this.cans.ContainsKey((int)FlavorOfCanToBeRemoved))
            {
                if (!IsEmpty(FlavorOfCanToBeRemoved))
                {
                    
                    cans[(int)FlavorOfCanToBeRemoved].Amount--;
                    SetCanCount();
                }
            }           
        }

        public void SetCanCount()
        {
            CansOfLemon = this.cans.Where(x => x.Key == (int)Flavor.Lemon).Select(x => x.Value.Amount).FirstOrDefault();
            CansOfRegular= this.cans.Where(x => x.Key == (int)Flavor.Regular).Select(x => x.Value.Amount).FirstOrDefault();
            CansOfOrange = this.cans.Where(x => x.Key == (int)Flavor.Orange).Select(x => x.Value.Amount).FirstOrDefault();
        }

        public void FillTheCanRack()
        {
            foreach (Flavor f in FlavorOps.AllFlavors)
            {                
                while (!IsFull(f))
                {
                    AddACanOf(f);
                }
            }
            SetCanCount();
        }

        public void EmptyCanRackOf(Flavor flavor)
        {
            if (this.cans.ContainsKey((int)flavor))
            {
                if (!IsEmpty(flavor))
                {
                    cans[(int)flavor].Amount=0;
                }
            }
        }

        public bool IsFull(Flavor flavor)
        {
            if (this.cans.TryGetValue((int)flavor,out CanInventory can))
            {
                return can.Amount >= this.maxInventory;
            }
            return false;
        }

        public bool IsEmpty(Flavor flavor)
        {
            if (this.cans.TryGetValue((int)flavor, out CanInventory can))
            {
                return can.Amount == 0;
            }
            return true;
        }

        public List<CanInventory> DisplayCanRack()
        {
            return this.cans.Values.Select(x => new CanInventory(x.Amount, x.Can)).ToList();
        }

        public CanInventory DisplayCanRack(Flavor flavor)
        {
            return this.cans.Values.Where(x => x.Can.Flavor == flavor)
                .Select(x => new CanInventory(x.Amount, x.Can)).FirstOrDefault();
        }


    }

    public class CanInventory
    {
        public CanInventory(int amount,Can can)
        {
            Amount = amount;
            Can = can;
        }
        public int Amount { get; set; }
        public Can Can { get; set; }
    }
}
