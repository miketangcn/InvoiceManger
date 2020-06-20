using GalaSoft.MvvmLight;
using InvoiceManger.Model;
using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.CommandWpf;
using System.Linq;
using System.Data.Entity;
using InvoiceManger.View;
using InvoiceManger.Common;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvoiceManger.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        Object pageDateQuery;
        Object pageOperatorQuery;
        Object pagePeopleQuery;
        Object pageSnQuery;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            InitlGroupRadio();
            ButtonEna = true;
            ButtonEnaN = !ButtonEna;
            QueryDateEna = true;
            State = "查询";
            //DetailEna=false;
            pageDateQuery=new DateQuery();
            pageOperatorQuery = new OperatorQuery();
            pagePeopleQuery = new PeopleQuery();
            pageSnQuery = new SnQuery();
            string year = DateTime.Now.Year.ToString()+"0101";
            FromDate = DateTime.ParseExact(year, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            ToDate = DateTime.Now.AddDays(1);
            Page = pageDateQuery;
            QueryInvoices = new List<Invoice>();
            viewInvoices = new ObservableCollection<InputInvoice>();
            QueryInvoice = new Invoice();
            ViewInvoice = new InputInvoice();
            TempViewInvoice = new InputInvoice();//显示detail的控件用这个值
            using (var db = new DataModel())
            {
                //Operators = db.Operators.Include(p => p.Person).ToList();
                Accountants = db.Accountants.ToList();
                Departments = db.Departments.ToList();
                Persons = db.Persons.ToList();
                //Persons = db.Persons.Where(p => p.DepId == 3).ToList();
            }
            Accountant = Accountants.Where(a => a.AccountantId == Information.AccountantId).FirstOrDefault();
            int i = Accountants.Count;
            ExcuteQueryCommand();
        }
        //克隆对象的函数
        public object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();
            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);
            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    object value = pi.GetValue(o, null);
                    pi.SetValue(p, value, null);
                }
            }
            return p;
        }

        private void InitlGroupRadio()
        {
            RadioButtons = new List<CompButtonModel>()
            {
                 new CompButtonModel(){ Content="所有", IsCheck = true },
                 new CompButtonModel(){ Content="发票号码", IsCheck = false },
                 new CompButtonModel(){ Content="录入部门", IsCheck = false },
                 new CompButtonModel(){ Content="录入人员", IsCheck = false },
                 new CompButtonModel(){ Content="报销人", IsCheck = false },
            };
            RadioTimeButtons = new List<CompButtonModel>()
            {
                 new CompButtonModel(){ Content="所有", IsCheck = false },
                 new CompButtonModel(){ Content="发票日期", IsCheck = false },
                 new CompButtonModel(){ Content="录入日期", IsCheck = true },
            };

        }
        private void calSummary()
        {
            Summary = 0;
            foreach (var item in ViewInvoices)
            {
                Summary = Summary + item.Amount;
            }
            QueryCount = ViewInvoices.Count;
        }
        //清空待输入发票信息
        private void ClearTempInvioice()
        {

                TempViewInvoice.Amount = (Decimal)0.00;
                TempViewInvoice.RecDate = DateTime.Now;
                TempViewInvoice.Date = DateTime.Now;
                TempViewInvoice.InvoiceNumber = "";
                TempViewInvoice.InvoiceCode = "";

        }
        //手动增加发票
        private void AppendInvoice()
        {
            if (TempViewInvoice != null)
            {
                if (Person == null ||Accountant==null)
                {
                    MessageBox.Show("请检查报销人姓名和财务人员", "信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    using (var db = new DataModel())
                    {
                        if (db.Invoices.Any(a => a.InvoiceNumber == TempViewInvoice.InvoiceNumber))
                        {
                            MessageBox.Show("该发票已经报销过", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show($"确认手动添加发票号码为{TempViewInvoice.InvoiceNumber}，金额为{TempViewInvoice.Amount}的发票吗",
                                "提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (result == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    TempViewInvoice.AcctId = Accountant.AccountantId;
                                    TempViewInvoice.PersonId = Person.PersonId;
                                    TempViewInvoice.AcctName = Accountant.Person.PersonName;
                                    TempViewInvoice.PersonName = Person.PersonName;
                                    var tempinvoice = new Invoice()
                                    {
                                        Amount = TempViewInvoice.Amount,
                                        Date = TempViewInvoice.Date,
                                        RecDate = TempViewInvoice.RecDate,
                                        InvoiceNumber = TempViewInvoice.InvoiceNumber,
                                        InvoiceCode = TempViewInvoice.InvoiceCode,
                                        AccountantId = Accountant.AccountantId,
                                        PersonId = Person.PersonId,
                                        Verification = TempViewInvoice.Verification
                                    };
                                    db.Invoices.Add(tempinvoice);
                                    db.SaveChanges();
                                    //var tempViewInvoice = (InputInvoice)(CloneObject(TempViewInvoice));
                                    //这地方用全部重新查询一遍最好
                                    ViewInvoices.Add(TempViewInvoice);
                                    ViewInvoice = ViewInvoices.Last();
                                    calSummary();
                                    ButtonEna = true;
                                    ButtonEnaN = false;
                                    State = "发票查询状态";

                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }
            }
        }
        //发票修改
        private void ModifyInvoice()
        {
            if (TempViewInvoice != null && ViewInvoices!=null)
            {
                if (Person == null || Accountant == null)
                {
                    MessageBox.Show("请检查报销人姓名和财务人员", "信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show($"确认要修改当前发票号码为{ViewInvoice.InvoiceNumber}的发票信息吗？",
                                       "提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            TempViewInvoice.AcctId = Accountant.AccountantId;
                            TempViewInvoice.PersonId = Person.PersonId;
                            TempViewInvoice.AcctName = Accountant.Person.PersonName;
                            TempViewInvoice.PersonName = Person.PersonName;
                            QueryInvoice.ID = TempViewInvoice.ID;
                            QueryInvoice.Amount = TempViewInvoice.Amount;
                            QueryInvoice.Date = TempViewInvoice.Date;
                            QueryInvoice.RecDate = TempViewInvoice.RecDate;
                            QueryInvoice.InvoiceNumber = TempViewInvoice.InvoiceNumber;
                            QueryInvoice.InvoiceCode = TempViewInvoice.InvoiceCode;
                            QueryInvoice.AccountantId = Accountant.AccountantId;
                            QueryInvoice.PersonId = Person.PersonId;
                            QueryInvoice.Verification = TempViewInvoice.Verification;
                            using (var db = new DataModel())
                            {
                                db.Entry(QueryInvoice).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            ViewInvoice = (InputInvoice)(CloneObject(TempViewInvoice));
                            var temp = ViewInvoices.Where(i => i.ID == ViewInvoice.ID).First();
                            if (temp != null)
                            {
                                var index = ViewInvoices.IndexOf(temp);
                                ViewInvoices[index] = ViewInvoice;
                            }
                            ViewInvoice = ViewInvoices.Where(i => i.ID == TempViewInvoice.ID).FirstOrDefault();
                            calSummary();
                            ButtonEna = true;
                            ButtonEnaN = false;
                            State = "发票查询状态";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
               
            }
        }
        #region 变量定义
        static readonly HttpClient client = new HttpClient();

        private string invoiceSN;

        public string InvoiceSN
        {
            get { return invoiceSN; }
            set
            {
                invoiceSN = value;
                RaisePropertyChanged(() => InvoiceSN);
            }
        }
        //状态
        private string state;

        public string State
        {
            get { return state; }
            set { state = value; RaisePropertyChanged(() => State); }
        }

        //数据网格,浏览，修改等按钮使能
        private bool buttonEna;

        public bool ButtonEna
        {
            get { return buttonEna; }
            set { buttonEna = value; RaisePropertyChanged(() => ButtonEna); }
        }
        //数据网格,浏览，修改等按钮使能
        private bool buttonEnaN;

        public bool ButtonEnaN
        {
            get { return buttonEnaN; }
            set { buttonEnaN = value; RaisePropertyChanged(() => ButtonEnaN); }
        }
        //修改，删除按钮使能
        private bool buttonMDEna;

        public bool ButtonMDEna
        {
            get { return buttonMDEna; }
            set { buttonMDEna = value; RaisePropertyChanged(() => ButtonMDEna); }
        }
        //查询日期控件可用
        private bool queryDateEna;

        public bool QueryDateEna
        {
            get { return queryDateEna; }
            set { queryDateEna = value;RaisePropertyChanged(() => QueryDateEna); }
        }
        //查询发票号码文本框可用
        private bool querySnEna;

        public bool QuerySnEna
        {
            get { return querySnEna; }
            set { querySnEna = value; RaisePropertyChanged(() => QuerySnEna); }
        }
        //查询财务人员可用
        private bool queryOperatorEna;

        public bool QueryOperatorEna
        {
            get { return queryOperatorEna; }
            set { queryOperatorEna = value; RaisePropertyChanged(() => QueryOperatorEna); }
        }
        //查询部门可用
        private bool queryDepartmentEna;

        public bool QueryDepartmentEna
        {
            get { return queryDepartmentEna; }
            set { queryDepartmentEna = value; RaisePropertyChanged(() => QueryDepartmentEna); }
        }
        //查询人员可用
        private bool queryPersonEna;

        public bool QueryPersonEna
        {
            get { return queryPersonEna; }
            set { queryPersonEna = value; RaisePropertyChanged(() => QueryPersonEna); }
        }

        //细节显示控件修改使能
        //private bool detailEna;

        //public bool DetailEna
        //{
        //    get { return detailEna; }
        //    set { detailEna = value;RaisePropertyChanged(() => DetailEna); }
        //}

        private int queryCount;

        public int QueryCount
        {
            get { return queryCount; }
            set { queryCount = value;RaisePropertyChanged(() => QueryCount); }
        }
        private decimal summary;

        public decimal Summary
        {
            get { return summary; }
            set { summary = value; RaisePropertyChanged(() => Summary); }
        }


        private DateTime fromDate;

        public DateTime FromDate
        {
            get { return fromDate; }
            set { fromDate = value;RaisePropertyChanged(() => FromDate); }
        }
        private DateTime toDate;

        public DateTime ToDate
        {
            get { return toDate; }
            set { toDate = value;RaisePropertyChanged(() => ToDate); }
        }


        private Object page;

        public Object Page
        {
            get { return page; }
            set { page = value;  RaisePropertyChanged(() => Page); }
        }

        private List<Invoice> queryInvoices;

        public List<Invoice> QueryInvoices
        {
            get { return queryInvoices; }
            set { queryInvoices = value;RaisePropertyChanged(() => QueryInvoices); }
        }
        private Invoice queryinvoice;

        public Invoice QueryInvoice
        {
            get { return queryinvoice; }
            set { queryinvoice = value; RaisePropertyChanged(() => QueryInvoice); }
        }

        private ObservableCollection<InputInvoice> viewInvoices;

        public ObservableCollection<InputInvoice> ViewInvoices
        {
            get { return viewInvoices; }
            set { viewInvoices = value;RaisePropertyChanged(() => ViewInvoices); }
        }

        private InputInvoice viewInvoice;

        public InputInvoice ViewInvoice
        {
            get { return viewInvoice; }
            set { viewInvoice = value;RaisePropertyChanged(() => ViewInvoice); }
        }
        private InputInvoice tempViewInvoice;

        public InputInvoice TempViewInvoice
        {
            get { return tempViewInvoice; }
            set { tempViewInvoice = value;RaisePropertyChanged(() => TempViewInvoice); }
        }


        private List<Accountant> accountants;

        public List<Accountant> Accountants
        {
            get { return accountants; }
            set { accountants = value; RaisePropertyChanged(() => Accountants); }
        }
        private Accountant accountant;

        public Accountant Accountant
        {
            get { return accountant; }
            set { accountant = value;RaisePropertyChanged(() => Accountant); }
        }
        //查询的财务人员
        private Accountant queryAccountant;

        public Accountant QueryAccountant
        {
            get { return queryAccountant; }
            set { queryAccountant = value;RaisePropertyChanged(() => QueryAccountant); }
        }

        private List<Person> persons;

        public List<Person> Persons
        {
            get { return persons; }
            set { persons = value; RaisePropertyChanged(() => Persons); }
        }


        private Person person;

        public Person Person
        {
            get { return person; }
            set { person = value;RaisePropertyChanged(() => Person); }
        }
        private List<Person> queryPersons;

        public List<Person> QueryPersons
        {
            get { return queryPersons; }
            set { queryPersons = value;RaisePropertyChanged(() => QueryPersons); }
        }
        private Person queryPerson;

        public Person QueryPerson
        {
            get { return queryPerson; }
            set { queryPerson = value;RaisePropertyChanged(() => QueryPerson); }
        }


        private List<Department> departments;

        public List<Department> Departments
        {
            get { return departments; }
            set { departments = value;RaisePropertyChanged(() => Departments); }
        }

        private Department department;

        public Department Department
        {
            get { return department; }
            set { department = value;RaisePropertyChanged(() => Department); }
        }
        private Department queryDepartment;

        public Department QueryDepartment
        {
            get { return queryDepartment; }
            set { queryDepartment = value;RaisePropertyChanged(() => QueryDepartment); }
        }


        private List<CompButtonModel> radioButtons;

        public List<CompButtonModel> RadioButtons
        {
            get { return radioButtons; }
            set { radioButtons = value; RaisePropertyChanged(() => RadioButtons); }
        }
        private CompButtonModel radioButton;

        public CompButtonModel RadioButton
        {
            get { return radioButton; }
            set { radioButton = value; RaisePropertyChanged(() => RadioButton); }
        }
        private List<CompButtonModel> radioTimeButtons;

        public List<CompButtonModel> RadioTimeButtons
        {
            get { return radioTimeButtons; }
            set { radioTimeButtons = value; RaisePropertyChanged(() => RadioTimeButtons); }
        }
        private CompButtonModel radioTimeButton;

        public CompButtonModel RadioTimeButton
        {
            get { return radioTimeButton; }
            set { radioTimeButton = value; RaisePropertyChanged(() => RadioTimeButton); }
        }
        #endregion

        #region 命令
        private RelayCommand queryCommand;

        public RelayCommand QueryCommand
        {
            get
            {
                if (queryCommand==null)
                {
                    queryCommand = new RelayCommand(() => ExcuteQueryCommand());
                }
                return queryCommand;
            }
            set { queryCommand = value; }
        }

        private void ExcuteQueryCommand()
        {
            RadioButton = RadioButtons.Where(p => p.IsCheck).First();
            RadioTimeButton = RadioTimeButtons.Where(p => p.IsCheck).First();
            using (var db=new DataModel())
            {
                if (QueryDateEna)
                {
                    switch (RadioButton.Content)
                    {
                        case "录入部门":
                            if (QueryDepartment != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.Person.DepId == QueryDepartment.DepartmentId ).ToList();
                            }
                            break;
                        case "所有":
                            QueryInvoices = db.Invoices.ToList();
                            break;
                        case "发票号码":
                            QueryInvoices = db.Invoices.Where(i => i.InvoiceNumber.ToString().Contains(InvoiceSN) ).ToList();
                            break;
                        case "录入人员":
                            if (QueryAccountant != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.AccountantId == QueryAccountant.AccountantId ).ToList();
                            }
                            break;
                        case "报销人":
                            if (QueryPerson != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.PersonId == QueryPerson.PersonId ).ToList();
                            }
                            break;
                        default:
                            break;
                    }
                    if (RadioTimeButton.Content=="录入日期")
                    {
                        QueryInvoices = QueryInvoices.Where(i => i.RecDate >= FromDate && i.RecDate <= ToDate).ToList();
                    }
                    else if (RadioTimeButton.Content == "发票日期")
                    {
                        QueryInvoices = QueryInvoices.Where(i => i.Date >= FromDate && i.Date <= ToDate).ToList();
                    }
                }
                else
                {
                    switch (RadioButton.Content)
                    {
                        case "录入部门":
                            if (QueryDepartment!=null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.Person.DepId == QueryDepartment.DepartmentId).ToList();
                            }
                            break;
                        case "所有":
                            QueryInvoices = db.Invoices.ToList();
                            break;
                        case "发票号码":
                            QueryInvoices = db.Invoices.Where(i => i.InvoiceNumber.ToString().Contains(InvoiceSN)).ToList();
                            break;
                        case "录入人员":
                            if (QueryAccountant != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.AccountantId == QueryAccountant.AccountantId).ToList();
                            }
                            break;
                        case "报销人":
                            if (QueryPerson != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.PersonId == QueryPerson.PersonId).ToList();
                            }
                            break;
                        default:
                            break;
                    }
                }                 
                    ViewInvoices.Clear();
                    QueryInvoices.ForEach(i => 
                                            {
                                                InputInvoice viewInvoice = new InputInvoice();
                                                i.Person = db.Persons.Where(p => p.PersonId == i.PersonId).FirstOrDefault();
                                                i.Accountant = db.Accountants.Where(o => o.AccountantId == i.AccountantId).FirstOrDefault();
                                                i.Accountant.Person = db.Persons.Where(p => p.PersonId == i.AccountantId).FirstOrDefault();
                                                viewInvoice.ID = i.ID;
                                                viewInvoice.InvoiceNumber = i.InvoiceNumber;
                                                viewInvoice.InvoiceCode = i.InvoiceCode;
                                                viewInvoice.Date = i.Date;
                                                viewInvoice.RecDate = i.RecDate;
                                                viewInvoice.AcctId = i.AccountantId;
                                                viewInvoice.PersonId = i.Person.PersonId;
                                                viewInvoice.PersonName = i.Person.PersonName;
                                                viewInvoice.AcctName = i.Accountant.Person.PersonName;
                                                viewInvoice.Amount = i.Amount;
                                                viewInvoice.Verification = i.Verification;
                                                viewInvoice.VerificationCode = i.VerificationCode;
                                                viewInvoice.VerifyState = i.VerifyState;
                                                ViewInvoices.Add(viewInvoice);                                                
                                              });
                    QueryCount = ViewInvoices.Count;
                    calSummary();
                //TempViewInvoice = null;
                //Department = null;
                //Person = null;
                //Accountant = null;

            }
           
        }

        private RelayCommand modifyCommand;

        public RelayCommand ModifyCommand
        {
            get
            {
                if (modifyCommand==null)
                {
                   modifyCommand=new RelayCommand(() => ExcuteModifyCommand());
                }
                return modifyCommand; 
            }
            set { modifyCommand = value; }
        }

        private void ExcuteModifyCommand()
        {
            
            if (TempViewInvoice!=null)
            {
                ButtonEna = false;
                ButtonEnaN = true;
                State = "手动修改发票";
                ButtonMDEna = false;
            }
        }

        private RelayCommand verifyCommand;

        public RelayCommand VerifyCommand
        {
            get
            {
                if (verifyCommand == null)
                {
                    verifyCommand = new RelayCommand(() => ExcuteVerifyCommand());
                }
                return verifyCommand;
            }
            set { verifyCommand = value; }
        }

        private void ExcuteVerifyCommand()
        {

            if (TempViewInvoice != null)
            {
                Verify();
            }
        }
        private async Task Verify()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string date = TempViewInvoice.Date.ToString("yyyyMMdd");
                string verifyCode = TempViewInvoice.Verification.ToString();
                verifyCode = verifyCode.Substring(verifyCode.Length - 6);
                string getString = string.Format("http://apis.juhe.cn/fp/query?fpdm={0}&fphm={1}&kprq={2}&jym={3}&je={4}&key=e4cdb0800eb9d34b148b54ca90951b3b",
                TempViewInvoice.InvoiceCode.ToString(), TempViewInvoice.InvoiceNumber.ToString(), date, verifyCode, TempViewInvoice.Amount.ToString());
                //HttpResponseMessage response = await client.GetAsync(getString);
                //response.EnsureSuccessStatusCode();
                //string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                string responseBody = await client.GetStringAsync(getString);
                //string responseBody = "{\"reason\":\"成功\",\"result\":{\"orderid\":\"JH336202003031456261476\",\"xfmc\":\"上海圆迈贸易有限公司\",\"xfsh\":\"91310114666025597Y\",\"xfdz\":\"上海市嘉定工业区叶城路1118号19层1901室 021-39915587\",\"xfzh\":\"招商银行股份有限公司北京青年路支行 121907731210104\",\"gfmc\":\"个人\",\"gfsh\":\"\",\"gfdz\":\"\",\"gfzh\":\"\",\"jym\":\"63936313063773165070\",\"jqbm\":\"661619911295\",\"je\":\"76.00\",\"zfbz\":\"N\",\"bz\":\"订单号:111636717032\",\"spxx\":[{\"spmc\":\"*肉及肉制品*元盛 新西兰原切牛腱子肉 1kg 进口草饲牛肉\",\"ggxh\":\"牛腱子1kg\",\"spdw\":\"袋\",\"spsl\":\"1.00000000\",\"spdj\":\"84.00000000\",\"spje\":\"84.00\",\"spslv\":\"免税\",\"spse\":\"***\",\"flbm\":\"\",\"cph\":\"\",\"type\":\"\",\"txrqq\":\"\",\"txrqz\":\"\"},{\"spmc\":\"*肉及肉制品*元盛 新西兰原切牛腱子肉 1kg 进口草饲牛肉\",\"ggxh\":\"\",\"spdw\":\"\",\"spsl\":\"\",\"spdj\":\"\",\"spje\":\"-8.00\",\"spslv\":\"免税\",\"spse\":\"***\",\"flbm\":\"\",\"cph\":\"\",\"type\":\"\",\"txrqq\":\"\",\"txrqz\":\"\"}],\"se\":\"0.00\",\"jshj\":\"76.00\",\"fplx\":\"10\"},\"error_code\":0}";
                Object obj = JsonConvert.DeserializeObject(responseBody) as JObject;
                dynamic d = obj;
                string veriState;
                //string success = d.reason;
                string amount = d.result.je;
                string code = d.error_code;
                string tempx;
                switch (code)
                {
                    case "0":
                        veriState = "校验正确";
                        break;
                    case "233600":
                        veriState = "信息不一致";
                        break;
                    case "233602":
                        veriState = "查无此票";
                        break;
                    case "233601":
                        veriState = "发票错误";
                        break;
                    default:
                        veriState = d.reason; ;
                        break;
                }
                if (code== "0"|| code == "233600" || code == "233602" || code == "233601")
                {
                    using (var db = new DataModel())
                    {
                        QueryInvoice = db.Invoices.Where(i => i.ID == TempViewInvoice.ID).FirstOrDefault();
                        QueryInvoice.VerifyState = veriState;
                        tempx= QueryInvoice.Amount.ToString();
                        if ( tempx!= amount && code == "0")
                        {
                            QueryInvoice.VerifyState = "金额不正确";
                        }
                        db.Entry(QueryInvoice).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    ViewInvoice.VerifyState = QueryInvoice.VerifyState;
                    var temp = ViewInvoices.Where(i => i.ID == ViewInvoice.ID).First();
                    if (temp != null)
                    {
                        var index = ViewInvoices.IndexOf(temp);
                        ViewInvoices[index] = ViewInvoice;
                    }
                    ViewInvoice = ViewInvoices.Where(i => i.ID == TempViewInvoice.ID).FirstOrDefault();
                }
                else
                {
                    MessageBox.Show(veriState+",查询失败!", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
        private RelayCommand appendCommand;

        public RelayCommand AppendCommand
        {
            get
            {
                if (appendCommand == null)
                {
                    appendCommand = new RelayCommand(() => ExcuteAppendCommand());
                }
                return appendCommand;
            }
            set { appendCommand = value; }
        }

        private void ExcuteAppendCommand()
        {
            ClearTempInvioice();
            ButtonEna = false;
            ButtonEnaN = true;
            ButtonMDEna = false;
            State = "手动增加发票";
        }
        //清空待输入发票信息

        private RelayCommand deleteCommand;

        public RelayCommand DeleteCommand
        {
            get
            {
                if (deleteCommand==null)
                {
                    deleteCommand = new RelayCommand(() => ExcuteDeleteCommand());
                }
                return deleteCommand; 
            }
            set { deleteCommand = value; }
        }

        private void ExcuteDeleteCommand()
        {
            
            if (TempViewInvoice != null)
            {
                MessageBoxResult result = MessageBox.Show($"确认要删除发票号码为{TempViewInvoice.InvoiceNumber}，金额为{TempViewInvoice.Amount}的发票吗",
                    "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new DataModel())
                        {
                            QueryInvoice=db.Invoices.Where(i => i.ID == TempViewInvoice.ID).FirstOrDefault();
                            db.Invoices.Remove(QueryInvoice);
                            db.SaveChanges();
                        }
                        ViewInvoice = ViewInvoices.Where(i => i.ID == TempViewInvoice.ID).First();
                       
                        if (ViewInvoice!=null)
                        {
                            int index = ViewInvoices.IndexOf(ViewInvoice);
                            ViewInvoices.Remove(ViewInvoice);
                            QueryCount = ViewInvoices.Count;
                            if (QueryCount >0)
                            {
                                calSummary();
                                if (index > 0)
                                {
                                    ViewInvoice = ViewInvoices[index - 1];
                                }
                                else ViewInvoice = ViewInvoices[0];
                            }
                        }

                        //ClearTempInvioice();
                        //Department =null;
                        //Person = null;
                        //Accountant = null;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        // 确认按钮命令
        private RelayCommand acceptCommand;

        public RelayCommand AcceptCommand
        {
            get
            {
                if (acceptCommand == null)
                {
                    acceptCommand = new RelayCommand(() => ExcuteAcceptCommand());
                }
                return acceptCommand;
            }
            set { acceptCommand = value; }
        }

        private void ExcuteAcceptCommand()
        {
            switch (State)
            {
                case "手动增加发票":
                    AppendInvoice();
                    break;
                case "手动修改发票":
                    ModifyInvoice();
                    break;
                default:
                    break;
            }

        }


        //取消按钮命令
        private RelayCommand cancleCommand;

        public RelayCommand CancleCommand
        {
            get
            {
                if (cancleCommand == null)
                {
                    cancleCommand = new RelayCommand(() => ExcuteCancleCommand());
                }
                return cancleCommand;
            }
            set { cancleCommand = value; }
        }

        private void ExcuteCancleCommand()
        {
            ExcuteDbGridSelectChange();
            ButtonEna = true;
            ButtonEnaN = false;
            ButtonMDEna = true;
            State = "发票查询状态";
        }

        private RelayCommand radioCheckCommand;

        public RelayCommand RadioCheckCommand
        {
            get
            {
                if (radioCheckCommand == null)
                {
                    radioCheckCommand = new RelayCommand(() => ExcuteRadioCommand());
                }
                return radioCheckCommand;
            }
            set { radioCheckCommand = value; }
        }

        private void ExcuteRadioCommand()
        {
            RadioButton = RadioButtons.Where(p => p.IsCheck).First();
            RadioTimeButton = RadioTimeButtons.Where(p => p.IsCheck).First();
            switch (RadioButton.Content)
            {
                case "所有":
                    {
                        QuerySnEna = false;
                        QueryOperatorEna = false;
                        QueryDepartmentEna = false;
                        QueryPersonEna = false;
                    }
                    break;
                case "录入部门":
                    {
                        QuerySnEna = false;
                        QueryOperatorEna = false;
                        QueryDepartmentEna = true;
                        QueryPersonEna = false;
                    }
                    break;
                case "发票号码":
                    {
                        QuerySnEna = true;
                        QueryOperatorEna = false;
                        QueryDepartmentEna = false;
                        QueryPersonEna = false;
                    }
                    break;
                case "报销人":
                    {
                        QuerySnEna = false;
                        QueryOperatorEna = false;
                        QueryDepartmentEna = true;
                        QueryPersonEna = true;
                    }
                    break;
                case "录入人员":
                    {
                        QuerySnEna = false;
                        QueryOperatorEna = true;
                        QueryDepartmentEna = false;
                        QueryPersonEna = false;
                    }
                    break;
                default:
                    break;
            }
            QueryDateEna = RadioTimeButton.Content == "所有" ? false : true;
        }

        private RelayCommand  cbChangeCommand;

        public RelayCommand CbChangeCommand
        {
            get
            {
                if (cbChangeCommand==null)
                {
                    cbChangeCommand = new RelayCommand(() => ExcuteCbChangeCommand());
                }
                return cbChangeCommand;
            }
            set { cbChangeCommand = value; }
        }

        private void ExcuteCbChangeCommand()
        {
            if (Department!=null)
            {
                using (var db = new DataModel())
                {
                    Persons = db.Persons.Where(p => p.DepId == Department.DepartmentId).ToList();
                }
            }
        
        }
        //查询的部门切换
        private RelayCommand queryCbChangeCommand;

        public RelayCommand QueryCbChangeCommand
        {
            get
            {
                if (queryCbChangeCommand == null)
                {
                    queryCbChangeCommand = new RelayCommand(() => ExcuteQueryCbChangeCommand());
                }
                return queryCbChangeCommand;
            }
            set { queryCbChangeCommand = value; }
        }

        private void ExcuteQueryCbChangeCommand()
        {
            if (QueryDepartment != null)
            {
                using (var db = new DataModel())
                {
                    QueryPersons = db.Persons.Where(p => p.DepId == QueryDepartment.DepartmentId).ToList();
                }
            }

        }
        private RelayCommand  dbGridSelectChange;

        public RelayCommand DbGridSelectChange
        {
            get 
            {
                if (dbGridSelectChange==null)
                {
                    dbGridSelectChange=new RelayCommand(()=> ExcuteDbGridSelectChange());
                }
                return dbGridSelectChange;
            }
            set { dbGridSelectChange = value; }
        }

        private void ExcuteDbGridSelectChange()
        {
            ButtonMDEna = (ViewInvoice != null && ViewInvoice.InvoiceNumber !=null) ? true : false;
            if (ViewInvoice!=null && ViewInvoice.InvoiceNumber!=null)
            {
                int DepId;
                using (var db = new DataModel())
                {
                    DepId = db.Persons.Where(p => p.PersonId == ViewInvoice.PersonId).FirstOrDefault().DepId;
                }
                // 选择切换后部门、当前报销人、财务的切换
                Department = Departments.Where(d => d.DepartmentId == DepId).FirstOrDefault();
                Person = Persons.Where(p => p.PersonId == ViewInvoice.PersonId).FirstOrDefault();
                Accountant = Accountants.Where(o => o.AccountantId == ViewInvoice.AcctId).FirstOrDefault();
                TempViewInvoice =(InputInvoice) CloneObject(ViewInvoice);
            }       
        }
        #endregion
    }
}