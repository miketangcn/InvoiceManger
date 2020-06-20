using InvoiceManger.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using InvoiceManger.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InvoiceManger.View
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        //private int OperatorId;
        private List<Accountant> acctountants = new List<Accountant>();
        public  Login()
        {
            InitializeComponent();
            this.Loaded += Login_Loaded;
            Seekaccountants();

        }


        private  void Login_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private async Task Seekaccountants()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var db = new DataModel())
                    {
                        acctountants = db.Accountants.Include(p => p.Person).ToList();
                        //wpf中要用dispatcher,invoke
                        Dispatcher.Invoke(() =>
                        {
                            cb1.DisplayMemberPath = "Person.PersonName";
                            cb1.SelectedValuePath = "Password";
                            cb1.ItemsSource = acctountants;
                            cb1.SelectedIndex = Properties.Settings.Default.AccountantId;
                            tiptext.Text = "";
                        });
                    }
                }
                catch (Exception)
                {
                    Dispatcher.Invoke(() =>
                    {
                        tiptext.Text = "数据库连接不上，请检查网络和系统配置！";
                    });
                }
            });
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new DataModel())
            {
                if (cb1.Text != "")
                {
                    var accountant = db.Accountants.Include(p => p.Person).Where(p => p.Person.PersonName == cb1.Text).FirstOrDefault<Accountant>();
                    Information.AccountantId = accountant.AccountantId;
                    Information.AccountantName = accountant.Person.PersonName;
                }

            }
            if (txtPassword.Text ==  cb1.SelectedValue.ToString())
            {
                Properties.Settings.Default.AccountantId = cb1.SelectedIndex;
                Properties.Settings.Default.Save();
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            else MessageBox.Show("密码错误", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }

    }
}
