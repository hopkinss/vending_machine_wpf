using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vend.Lib;

namespace Vend.App.Model
{

    public class VendingMachineViewModel : INotifyPropertyChanged
    {

        private PurchasePrice purchasePrice;
        private CanRack canRack;
        private CoinBox trxBox;
        private CoinBox box;
        private int balance;
        private string title;
        private string uiMessage;
        private BitmapImage imgSoda;

        public VendingMachineViewModel(int inventory, dynamic price)
        {
            canRack = new CanRack(inventory);
            purchasePrice = new PurchasePrice(price);
            trxBox = new CoinBox();
            box = new CoinBox();
            title = $"Price of soda is {this.purchasePrice.Price}";
            MainTitle = "WPF Vending Machine - Assignment 7";

        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand DepositCommand { get { return new RelayCommand(e => true, this.OnDeposit); } }
        public ICommand ReturnCoinsCommand { get { return new RelayCommand(e => this.trxBox.ValueOf>0, 
            this.OnReturnCoins); } }
        public ICommand EjectCanCommand { get { return new RelayCommand(e => true, this.OnEjectCan); } }


        private void OnDeposit(object obj)
        {            
            TrxBox.Deposit(new Coin((Denomination)obj));
        }

        private void OnEjectCan(object flavor)
        {
            if (IsAmountSufficient())
            {
                var f = (FlavorOps.ToFlavor(flavor.ToString()));
                if (!canRack.IsEmpty(f))
                {
                    canRack.RemoveACanOf(f);
                    Transfer(Box);
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri($"/Images/{flavor.ToString().ToLower()}.jpg", UriKind.Relative);
                    img.Rotation = Rotation.Rotate270;
                    img.EndInit();

                    this.ImgSoda = img;
                    this.UiMessage = $"Here is your {f} soda";

                }
            }
            else
            {
                this.UiMessage = $"Please deposit {(this.purchasePrice.Price - this.trxBox.ValueOf)} more cents!";
            }
        }



        private void OnReturnCoins(object obj)
        {
            var refund = 0M;
            while (this.trxBox.ValueOf > 0)
            {
                var coin = this.trxBox.Box.LastOrDefault();
                refund += coin.ValueOf;
                trxBox.Withdraw(coin.CoinEnumeral);
            }
            this.UiMessage = $"Here's you refund of ${refund}";
        }

        public CanRack CanRack
        {
            get { return canRack; }
            set
            {
                if (canRack != null)
                {
                    canRack = value;
                }
            }
        }

        public PurchasePrice PurchasePrice
        {
            get { return purchasePrice; }
            set { purchasePrice = value; }
        }

        public CoinBox TrxBox
        {
            get { return trxBox; }
            set
            {
                trxBox = value;
            }
        }

        public CoinBox Box
        {
            get { return box; }
            set { box = value; }
        }
        public void Transfer(CoinBox destination)
        {

            var test = MakePurchase();
            foreach (var c in test)
            {
                destination.Deposit(c);
            }
        }
        public string MainTitle { get; set; }
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
            }
        }

        public string UiMessage
        {
            get { return uiMessage; }
            set
            {
                uiMessage = value;
                OnPropertyChanged();
            }
        }
        public BitmapImage ImgSoda
        {
            get => imgSoda;
            set
            {
                imgSoda = value;
                OnPropertyChanged("ImgSoda");
            }
        }
        public int Balance
        {
            get { return balance; }
            set
            {
                balance = value;
            }
        }

        public bool IsAmountSufficient()
        {
            return trxBox.Box.Sum(x => x.ValueOf) >= purchasePrice.Price;
        }

        private void SetBalance()
        {
            this.balance = purchasePrice.Price - (int)trxBox.Box.Sum(x => x.ValueOf);
        }

        public List<Coin> MakePurchase()
        {
            return ProcessPayment(PurchasePrice.Price).ToList();
        }


        public IEnumerable<Coin> ProcessPayment(int amount)
        {
            var remainder = Math.Abs(amount);
            var returnedCoins = new List<Coin>();
            while (remainder > 0)
            {
                foreach (var d in Enum.GetValues(typeof(Denomination)).Cast<Denomination>().Where(x => (int)x > 0).Reverse())
                {
                    while ((int)d <= remainder)
                    {

                        var coinToRemove = trxBox.Box.Where(x => x.CoinEnumeral == d).FirstOrDefault();
                        if (coinToRemove != null)
                        {
                            remainder -= (int)d;
                            trxBox.Withdraw(coinToRemove.CoinEnumeral);
                            SetBalance();
                            yield return new Coin(d);
                        }
                        else
                            break;
                    }
                }
            }
        }
    }
}