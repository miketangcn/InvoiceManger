using InvoiceManger.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InvoiceManger.Common;

namespace InvoiceManger.View
{
    /// <summary>
    /// Config.xaml 的交互逻辑
    /// </summary>
    public partial class Config : Window
    {
        public Config()
        {
            InitializeComponent();
            this.Loaded += Config_Loaded;
        }

        private void Config_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.ConfigInital();
            //OperatorName.Content = Information.AccountantName;
        }
    }
}
