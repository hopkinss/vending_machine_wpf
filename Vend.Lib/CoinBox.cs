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


        public List<Coin> Box
        {
            get => box;
            set
            {
                box = value;
            }
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

        public int CoinCount(Denomination denomination)
        {
            return this.Box.Where(x => x.CoinEnumeral == denomination).Count();
        }

        public void Deposit(Coin Acoin)
        {
            Box.Add(Acoin);
            OnPropertyChanged("ValueOf");
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

        public void Transfer(CoinBox destination,int price)
        {
            foreach (var c in MakePurchase(destination,price))
            {
                destination.Deposit(c);
            }
        }

        public List<Coin> MakePurchase( CoinBox destination, int price)
        {
            return ProcessPayment(price).ToList();
        }

        public IEnumerable<Coin> ProcessPayment(int price)
        {
            var remainder = Math.Abs(price);
            while (remainder > 0)
            {
                foreach (var d in Enum.GetValues(typeof(Denomination)).Cast<Denomination>().Where(x => (int)x > 0).Reverse())
                {
                    
                    while ((int)d <= remainder)
                    {
                        
                        var coinToRemove = this.Box.Where(x => x.CoinEnumeral == d).FirstOrDefault();
                        if (coinToRemove != null)
                        {
                            remainder -= (int)d;
                            this.Withdraw(coinToRemove.CoinEnumeral);
                            yield return new Coin(d);
                        }
                        else
                            break;                            
                    }
                }
            }
        }

        public bool CanMakeChange(int price)
        {
            if (this.ValueOf < price) 
                return true;

            var remainder = Math.Abs(price);
            var coins = this.box.Select(x => x).ToList();

            int counter = 0;

            while (remainder > 0)
            {
                foreach (var d in Enum.GetValues(typeof(Denomination)).Cast<Denomination>().Where(x => (int)x > 0).Reverse())
                {
                    
                    while ((int)d <= remainder)
                    {
                        var coinToRemove = coins.Where(x => x.CoinEnumeral == d).FirstOrDefault();
                        if (coinToRemove != null)
                        {
                            coins.Remove(coinToRemove);
                            remainder -= (int)d;
                        }
                        else
                        {
                            if (counter > 1)
                                return false;
                            counter++;
                            break;
                        }
                        if (coins.Count == 0)
                            return true;
                    }
                }
            }
            return true;
        }
    }
}
