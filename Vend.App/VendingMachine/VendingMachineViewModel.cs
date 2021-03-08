using System;
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
        private string canPriceMessage;
        private string uiMessage;
        private BitmapImage imgSoda;
        private bool canMakeChange;

        public VendingMachineViewModel(int inventory, dynamic price)
        {
            canRack = new CanRack(inventory);
            purchasePrice = new PurchasePrice(price);
            trxBox = new CoinBox();
            box = new CoinBox();
            canPriceMessage = $"Please deposit {(this.purchasePrice.Price * .01).ToString("C2")}";
            MainTitle = "WPF Vending Machine - Assignment 6";
            canMakeChange = true;
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


        public bool CanMakeChange
        {
            get { return canMakeChange; }
            set
            {
                canMakeChange = value;
                OnPropertyChanged("CanMakeChange");
            }
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

        // Coinbox for current transaction
        public CoinBox TrxBox
        {
            get { return trxBox; }
            set
            {
                trxBox = value;
            }
        }

        // Vending machines permanent coinbox
        public CoinBox Box
        {
            get { return box; }
            set { box = value; }
        }
        
        // Title for the Window
        public string MainTitle { get; set; }
      
        public string CanPriceMessage
        {
            get { return canPriceMessage; }
            set
            {
                canPriceMessage = value;
            }
        }

        // Message to display to user about transaction as animation
        public string UiMessage
        {
            get { return uiMessage; }
            set
            {
                uiMessage = value;
                OnPropertyChanged();
            }
        }

        // Can dispensed to user as animation
        public BitmapImage ImgSoda
        {
            get => imgSoda;
            set
            {
                imgSoda = value;
                OnPropertyChanged("ImgSoda");
            }
        }

        private void OnDeposit(object obj)
        {
            TrxBox.Deposit(new Coin((Denomination)obj));
            this.CanMakeChange = TrxBox.CanMakeChange(PurchasePrice.Price);
        }

        private void OnEjectCan(object flavor)
        {
            if (this.canMakeChange)
            {
                if (IsAmountSufficient())
                {
                    var f = (FlavorOps.ToFlavor(flavor.ToString()));
                    if (!canRack.IsEmpty(f))
                    {
                        canRack.RemoveACanOf(f);
                        trxBox.Transfer(box, purchasePrice.Price);
                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        img.UriSource = new Uri($"/Images/{flavor.ToString().ToLower()}.jpg", UriKind.Relative);
                        img.Rotation = Rotation.Rotate270;
                        img.EndInit();

                        this.ImgSoda = img;

                        var refundMsg = string.Empty;

                        if (this.trxBox.Box.Count>0)
                        {
                           refundMsg = $" and {((Convert.ToInt32(RefundCoins())*.01).ToString("C2"))} change";
                        }
      
                        this.UiMessage = $"Here is your {f} soda{refundMsg}";
                    }
                    else
                    {
                        this.UiMessage = $"Sorry, we are out of {f}";
                    }
                }
                else
                {              
                    this.UiMessage = $"Please deposit an additional {((this.purchasePrice.Price - Convert.ToInt32(this.trxBox.ValueOf)) * .01).ToString("C2")}";
                }
            }
            else
            {
                this.UiMessage = $"Exact change required. Eject coins and try again";
            }
        }

        private void OnReturnCoins(object obj)
        {
            var refund = RefundCoins();
            this.UiMessage = $"Here's you refund of {(Convert.ToInt32(refund)*.01).ToString("C2")}";
            this.CanMakeChange = true;
        }

        private decimal RefundCoins()
        {
            var refund = 0M;
            while (this.trxBox.ValueOf > 0)
            {
                var coin = this.trxBox.Box.LastOrDefault();
                refund += coin.ValueOf;
                trxBox.Withdraw(coin.CoinEnumeral);
            }
            return refund;
        }

        public bool IsAmountSufficient()
        {
            return trxBox.Box.Sum(x => x.ValueOf) >= purchasePrice.Price;
        }
    }
}