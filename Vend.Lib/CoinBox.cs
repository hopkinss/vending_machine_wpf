using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vend.Lib
{
    public class CoinBox : INotifyPropertyChanged
    {
        private List<Coin> box;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CoinBox()
        {
            this.Box = new List<Coin>();
        }
        public CoinBox(List<Coin> SeedMoney)
        {
            this.Box = new List<Coin>();
            Box.AddRange(SeedMoney);
        }

        public void Deposit(Coin Acoin)
        {
            Box.Add(Acoin);
            OnPropertyChanged("ValueOf");
        }
        public List<Coin> Box
        {
            get => box;
            set
            {
                box = value;
            }
        }

        public bool Withdraw(Denomination ACoinDenomination)
        {
            var coinToRemove = this.Box.FirstOrDefault(x => x.CoinEnumeral == ACoinDenomination);
            if (coinToRemove != null)
            {
                this.Box.Remove(coinToRemove);
                OnPropertyChanged("ValueOf");
                return true;
            }
            return false;
        }

        public int HalfDollarCount
        {
            get { return CoinCount(Denomination.HALFDOLLAR); }
        }
        public int QuarterCount
        {
            get { return CoinCount(Denomination.QUARTER); }
        }
        public int DimeCount
        {
            get { return CoinCount(Denomination.DIME); }
        }
        public int NickelCount
        {
            get { return CoinCount(Denomination.NICKEL); }
        }
        public int SlugCount
        {
            get { return CoinCount(Denomination.SLUG); }
        }

        public int CoinCount(Denomination denomination)
        {
            return this.Box.Where(x => x.CoinEnumeral == denomination).Count();
        }

        public decimal ValueOf
        {
            get { return this.Box.Sum(x => x.ValueOf); }

        }

        public static IEnumerable<Coin> GenerateRandomSeed(int coinCount)
        {
            var coins = Enum.GetValues(typeof(Denomination)).Cast<Denomination>().ToArray();
            var random = new Random();
            for (int i = 0; i < coinCount; i++)
            {
                yield return new Coin(coins[random.Next(0, 4)]);
            }
        }

    }
}
