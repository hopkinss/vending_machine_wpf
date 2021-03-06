using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vend.App.Model;
using Vend.Lib;

namespace Vend.App
{

    public partial class MainWindow : Window
    {
        private VendingMachineViewModel vm = new VendingMachineViewModel(3, 50);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
