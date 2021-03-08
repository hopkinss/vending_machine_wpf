using System.Windows;
using Vend.App.Model;

namespace Vend.App
{
    // Shawn Hopkins. Assignment 06
    public partial class MainWindow : Window
    {
        // instance of vendingmachine(cans, price)
        private VendingMachineViewModel vm = new VendingMachineViewModel(3, 55);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
