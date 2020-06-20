using InvoiceManger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InvoiceManger.Common;
namespace InvoiceManger.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = 0.0;
            this.Top = 0.0;
            this.WindowState = WindowState.Maximized;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            Oper.Content = Information.AccountantName;

        }

        private void InputInvoice_Click(object sender, RoutedEventArgs e)
        {
            Input inputWindow = new Input();
            inputWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            inputWindow.ShowDialog();

        }

        private void SysConfig_Click(object sender, RoutedEventArgs e)
        {
            Config configWindows = new Config();
            configWindows.WindowStartupLocation= WindowStartupLocation.CenterScreen;
            configWindows.ShowDialog();
        }

    }
}
