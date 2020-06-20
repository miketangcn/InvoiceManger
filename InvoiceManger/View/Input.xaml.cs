using System.Windows;
using InvoiceManger.ViewModel;

namespace InvoiceManger.View
{
    /// <summary>
    /// Input.xaml 的交互逻辑
    /// </summary>
    public partial class Input : Window
    {
        public Input()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.ToolWindow;
            this.Loaded += Input_Loaded;
            this.Closed += Input_Closed;
        }

        private void Input_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.InputInital();
        }

        private void Input_Closed(object sender, System.EventArgs e)
        {
            ViewModelLocator.InputClose();
        }
    }
}
