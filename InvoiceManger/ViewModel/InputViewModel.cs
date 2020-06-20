using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using InvoiceManger.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InvoiceManger.Common;
using System;
using System.Windows;
using System.Globalization;

namespace InvoiceManger.ViewModel
{
    public class InputViewModel : ViewModelBase
    {
        private  ScanerHook listener;
        public InputViewModel()
        {
            //listener.ScanerEvent += Listener_ScanerEvent;
        }

        private void Listener_ScanerEvent(ScanerHook.ScanerCodes codes)
        {
            string[] content = codes.Result.ToString().Split(',');
            try
            {
                DateTime dateTime;
                if (!DateTime.TryParseExact(content[5], "yyyy/M/ddHH:mm:ss", CultureInfo.InvariantCulture,DateTimeStyles.None,out dateTime))
                {
                    DateTime.TryParseExact(content[5], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
                }

                InputInvoice invoice = new InputInvoice()
                {
                    InvoiceCode = content[2],
                    InvoiceNumber = content[3],
                    Date = dateTime,
                    RecDate = DateTime.Now,
                    Verification = content[6],
                    AcctId = Information.AccountantId,
                    PersonId = Properties.Settings.Default.PersonId,
                    VerificationCode = (content.Length==8)?content[7] : null
                };
                decimal amount = 0;
                //invoice.AcctId = Information.AccountantId;
                //invoice.PersonId = Properties.Settings.Default.PersonId;
                using (var db=new DataModel ())
                {
                    invoice.PersonName = db.Persons.Where(p => p.PersonId == invoice.PersonId).FirstOrDefault().PersonName;
                }
                if (decimal.TryParse(content[4], out amount))
                {
                    invoice.Amount = amount;
                }
                if (Invoices.Any(i=>i.InvoiceNumber==invoice.InvoiceNumber && i.InvoiceCode == invoice.InvoiceCode))
                {
                    string time = DateTime.Now.ToString("HH:mm:ss");
                    ScanMessage = time+"  该发票已扫描，等待入库!";
                
                   //MessageBox.Show("该发票已经扫描等待入库", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    using (var db = new DataModel())
                    {
                        if (db.Invoices.Any(a => a.InvoiceNumber == invoice.InvoiceNumber && a.InvoiceCode == invoice.InvoiceCode))
                        {
                            string time = DateTime.Now.ToString("HH:mm:ss");
                            ScanMessage = time+"  该发票已经报销过!";
                            //MessageBox.Show("该发票已经报销过", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            string time = DateTime.Now.ToString("HH:mm:ss");
                            ScanMessage =time+"  发票扫描成功!";
                            Message = $"发票代码: {invoice.InvoiceCode} 发票号码: {invoice.InvoiceNumber} 税前金额: {invoice.Amount} " +
                                $"发票日期: {invoice.Date.ToString("yyyy年MM月dd日")}";
                            invoice.AcctName = db.Accountants.Where(o => o.AccountantId == Information.AccountantId).FirstOrDefault().Person.PersonName;
                            invoice.VerifyState = "未校验";
                            Invoices.Add(invoice);
                            InputCount=Invoices.Count;
                            BtnEnableJudgement();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ScanMessage = "非正常发票数据";
            }
        }

        public void Inital()
        {
            //这个类在窗口关闭后就没了，必须在这里创建新的
            listener = new ScanerHook();
            listener.ScanerEvent += Listener_ScanerEvent;
            listener.Start();
            Invoice = null;
            //noPerson = 0;
            Message = "";
            //InputCount = 5;
            using (var db = new DataModel())
            {
                Departments = db.Departments.ToList();
                Peoples = db.Persons.Where(p => p.PersonName != "").OrderBy(p => p.PersonName).ToList();
            }
            Invoices = new ObservableCollection<InputInvoice>  //一定要用ObservableCollection
            {
                //new InputInvoice(){ InvoiceNumber="5",AcctId=1,PersonId=11,RecDate = DateTime.Now.Date,Date=DateTime.Now.Date},
                //new InputInvoice(){ InvoiceNumber="6",AcctId=2,PersonId=12,RecDate = DateTime.Now.Date,Date=DateTime.Now.Date},
                //new InputInvoice(){ InvoiceNumber="7",AcctId=3,PersonId=13,RecDate = DateTime.Now.Date,Date=DateTime.Now.Date},
                //new InputInvoice(){ InvoiceNumber="8",AcctId=3,PersonId=6,RecDate = DateTime.Now.Date,Date=DateTime.Now.Date},
                //new InputInvoice(){ InvoiceNumber="9",AcctId=3,PersonId=5,RecDate = DateTime.Now.Date,Date=DateTime.Now.Date},
            };
        }
        public void Close()//窗口关闭执行
        {
            listener.Stop() ;
        }
        #region 变量定义
        //private int noPerson;//没有报销人员的待输入发票数量
        private bool btnAppendEna;//添加按钮使能

        public bool BtnAppendEna
        {
            get { return btnAppendEna; }
            set { btnAppendEna = value;RaisePropertyChanged(() => BtnAppendEna); }
        }
        private bool btnModifyEna;

        public bool BtnModifyEna
        {
            get { return btnModifyEna; }
            set { btnModifyEna = value;RaisePropertyChanged(() => BtnModifyEna); }
        }

        private int inputCount;//待入库发票数量

        public int InputCount
        {
            get { return inputCount; }
            set { inputCount = value; RaisePropertyChanged(() => InputCount); }
        }

        private int index;// 选中待输入发票的索引

        public int Index
        {
            get { return index; }
            set { index = value; RaisePropertyChanged(() => Index); }
        }

        private List<Accountant> operators;

        public List<Accountant> Operators
        {
            get { return operators; }
            set { operators = value; RaisePropertyChanged(() => Operators); }
        }
        private Accountant op;

        public Accountant Op
        {
            get { return op; }
            set { op = value; RaisePropertyChanged(() => Op); }
        }
        private List<Person> peoples;

        public List<Person> Peoples
        {
            get { return peoples; }
            set { peoples = value; RaisePropertyChanged(() => Peoples); }
        }


        private Person people;

        public Person People
        {
            get { return people; }
            set { people = value; RaisePropertyChanged(() => People); }
        }
        private List<Department> departments;

        public List<Department> Departments
        {
            get { return departments; }
            set { departments = value; RaisePropertyChanged(() => Departments); }
        }

        private Department department;

        public Department Department
        {
            get { return department; }
            set { department = value; RaisePropertyChanged(() => Department); }
        }

        private InputInvoice invoice;

        public InputInvoice Invoice
        {
            get { return invoice; }
            set { invoice = value; RaisePropertyChanged(() => Invoice); }
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(() => Message); }
        }

        private string scanMessage;

        public string ScanMessage
        {
            get { return scanMessage; }
            set { scanMessage = value; RaisePropertyChanged(() => ScanMessage); }
        }


        private ObservableCollection<InputInvoice> invoices;

        public ObservableCollection<InputInvoice> Invoices
        {
            get { return invoices; }
            set { invoices = value; RaisePropertyChanged(() => Invoices); }
        }
        #endregion
        #region 按钮命令定义

        private RelayCommand cbDepartMentChange;

        public RelayCommand CbDepartMentChange
        {
            get
            {
                if (cbDepartMentChange == null)
                {
                    cbDepartMentChange = new RelayCommand(() => CbDepartMentChangeExcute());
                }
                return cbDepartMentChange;
            }
            set { cbDepartMentChange = value; }
        }

        private void CbDepartMentChangeExcute()
        {
            if (Department!=null)
            {
                using (var db = new DataModel())
                {
                    Peoples = db.Persons.Where(p => p.DepId == Department.DepartmentId).ToList();
                }
            }
        }
        private RelayCommand<int> dbGridSelectChange;

        public RelayCommand<int> DbGridSelectChange
        {
            get
            {
                if (dbGridSelectChange == null)
                {
                    dbGridSelectChange = new RelayCommand<int>((id) => DbGridSelectChangeExcute(id));
                }
                return dbGridSelectChange;
            }
            set { dbGridSelectChange = value; }
        }

        private void DbGridSelectChangeExcute(int id)
        {
            //using (var db = new DataModel())
            //{
            //    People = db.People.Where(p => p.PersonId == id).FirstOrDefault();
            //}
            BtnEnableJudgement();
        }
        private RelayCommand cmdModify;

        public RelayCommand CmdModify
        {
            get
            {
                if (cmdModify == null)
                {
                    cmdModify = new RelayCommand(() => CmdModifyExcute());
                }
                return cmdModify;
            }
            set { cmdModify = value; }
        }

        private void CmdModifyExcute()
        {
            for (int i = 0; i < Invoices.Count; i++)
            {
                if (Invoices[i].InvoiceNumber == Invoice.InvoiceNumber)
                {
                    index = i;
                    break;
                }
            }

            if (Invoices.Count > 0 && Index < Invoices.Count && index>=0)
            {
                if (People!=null)
                {
                    Invoices[Index].PersonId = People.PersonId;
                    Invoices[Index].PersonName = People.PersonName;
                    Properties.Settings.Default.PersonId = People.PersonId;
                    Properties.Settings.Default.Save();
                }
                Invoices[Index].AcctId = Information.AccountantId;
                using (var db = new DataModel())
                {
                    Invoices[Index].AcctName = db.Accountants.Where(o => o.AccountantId == Information.AccountantId).FirstOrDefault().Person.PersonName;
                }
            }
        }
        private RelayCommand cmdAppend;//添加按钮

        public RelayCommand CmdAppend
        {
            get
            {
                if (cmdAppend == null)
                {
                    cmdAppend = new RelayCommand(() => CmdAppendExcute());
                }
                return cmdAppend;
            }
            set { cmdAppend = value; }
        }

        private void CmdAppendExcute()//（目前的逻辑用来测试自动查询报销人姓名和操作员姓名的功能的）
        {
          //InputInvoice invoice=new InputInvoice() { InvoiceNumber = "9999999", OperId = 3, PersonId = 8 };
            using (var db = new DataModel())
            {
                List<Invoice> InputInvoice = new List<Invoice>();
                foreach (var item in invoices)
                {
                    Invoice tempInvoice = new Invoice();
                    tempInvoice.Amount = item.Amount;
                    tempInvoice.Date = item.Date;
                    tempInvoice.InvoiceCode = item.InvoiceCode;
                    tempInvoice.InvoiceNumber = item.InvoiceNumber;
                    tempInvoice.AccountantId = item.AcctId;
                    tempInvoice.PersonId = item.PersonId;
                    tempInvoice.RecDate = item.RecDate;
                    tempInvoice.Verification = item.Verification;
                    tempInvoice.VerificationCode = item.VerificationCode;
                    tempInvoice.VerifyState = item.VerifyState;
                    InputInvoice.Add(tempInvoice);
                }
                db.Invoices.AddRange(InputInvoice);
                db.SaveChanges();
                InputCount = Invoices.Count;
                MessageBox.Show($"有条{InputCount}发票入库成功", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                InputCount = 0;
                BtnEnableJudgement();
                Message = "";
                ScanMessage = "";
                Invoices.Clear();
                //invoice.OperName = db.Operators.Where(o => o.OperatorId == invoice.OperId).FirstOrDefault().Person.PersonName;
                //invoice.PersonName = db.People.Where(p => p.PersonId == invoice.PersonId).FirstOrDefault().PersonName;
            }
            // Invoices.Add(invoice);
        }
        private RelayCommand cmdDelete;//添加按钮

        public RelayCommand CmdDelete
        {
            get
            {
                if (cmdDelete == null)
                {
                    cmdDelete = new RelayCommand(() => CmdDeleteExcute());
                }
                return cmdDelete;
            }
            set { cmdDelete = value; }
        }

        private void CmdDeleteExcute()
        {
            if (invoice!=null)
            {
                MessageBoxResult result = MessageBox.Show($"确定要删除当前发票号为{invoice.InvoiceNumber}待输入发票记录?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result==MessageBoxResult.Yes)
                {
                    Invoices.Remove(invoice);
                    InputCount = Invoices.Count;
                    invoice = null;
                    BtnEnableJudgement();
                }
      
            }

        }
        #endregion
        private void BtnEnableJudgement()//按钮使能判断
        {
            if (InputCount > 0)
            {
                BtnAppendEna = true;
            }
            else BtnAppendEna = false;
            if (Invoice != null)
            {
                BtnModifyEna = true;
            }
            else BtnModifyEna = false;
        }

    }
}
