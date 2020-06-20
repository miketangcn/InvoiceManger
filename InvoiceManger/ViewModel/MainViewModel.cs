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
            State = "��ѯ";
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
            TempViewInvoice = new InputInvoice();//��ʾdetail�Ŀؼ������ֵ
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
        //��¡����ĺ���
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
                 new CompButtonModel(){ Content="����", IsCheck = true },
                 new CompButtonModel(){ Content="��Ʊ����", IsCheck = false },
                 new CompButtonModel(){ Content="¼�벿��", IsCheck = false },
                 new CompButtonModel(){ Content="¼����Ա", IsCheck = false },
                 new CompButtonModel(){ Content="������", IsCheck = false },
            };
            RadioTimeButtons = new List<CompButtonModel>()
            {
                 new CompButtonModel(){ Content="����", IsCheck = false },
                 new CompButtonModel(){ Content="��Ʊ����", IsCheck = false },
                 new CompButtonModel(){ Content="¼������", IsCheck = true },
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
        //��մ����뷢Ʊ��Ϣ
        private void ClearTempInvioice()
        {

                TempViewInvoice.Amount = (Decimal)0.00;
                TempViewInvoice.RecDate = DateTime.Now;
                TempViewInvoice.Date = DateTime.Now;
                TempViewInvoice.InvoiceNumber = "";
                TempViewInvoice.InvoiceCode = "";

        }
        //�ֶ����ӷ�Ʊ
        private void AppendInvoice()
        {
            if (TempViewInvoice != null)
            {
                if (Person == null ||Accountant==null)
                {
                    MessageBox.Show("���鱨���������Ͳ�����Ա", "��Ϣ��ʾ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    using (var db = new DataModel())
                    {
                        if (db.Invoices.Any(a => a.InvoiceNumber == TempViewInvoice.InvoiceNumber))
                        {
                            MessageBox.Show("�÷�Ʊ�Ѿ�������", "����", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show($"ȷ���ֶ���ӷ�Ʊ����Ϊ{TempViewInvoice.InvoiceNumber}�����Ϊ{TempViewInvoice.Amount}�ķ�Ʊ��",
                                "��ʾ", MessageBoxButton.YesNo, MessageBoxImage.Information);
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
                                    //��ط���ȫ�����²�ѯһ�����
                                    ViewInvoices.Add(TempViewInvoice);
                                    ViewInvoice = ViewInvoices.Last();
                                    calSummary();
                                    ButtonEna = true;
                                    ButtonEnaN = false;
                                    State = "��Ʊ��ѯ״̬";

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
        //��Ʊ�޸�
        private void ModifyInvoice()
        {
            if (TempViewInvoice != null && ViewInvoices!=null)
            {
                if (Person == null || Accountant == null)
                {
                    MessageBox.Show("���鱨���������Ͳ�����Ա", "��Ϣ��ʾ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show($"ȷ��Ҫ�޸ĵ�ǰ��Ʊ����Ϊ{ViewInvoice.InvoiceNumber}�ķ�Ʊ��Ϣ��",
                                       "��ʾ", MessageBoxButton.YesNo, MessageBoxImage.Information);
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
                            State = "��Ʊ��ѯ״̬";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
               
            }
        }
        #region ��������
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
        //״̬
        private string state;

        public string State
        {
            get { return state; }
            set { state = value; RaisePropertyChanged(() => State); }
        }

        //��������,������޸ĵȰ�ťʹ��
        private bool buttonEna;

        public bool ButtonEna
        {
            get { return buttonEna; }
            set { buttonEna = value; RaisePropertyChanged(() => ButtonEna); }
        }
        //��������,������޸ĵȰ�ťʹ��
        private bool buttonEnaN;

        public bool ButtonEnaN
        {
            get { return buttonEnaN; }
            set { buttonEnaN = value; RaisePropertyChanged(() => ButtonEnaN); }
        }
        //�޸ģ�ɾ����ťʹ��
        private bool buttonMDEna;

        public bool ButtonMDEna
        {
            get { return buttonMDEna; }
            set { buttonMDEna = value; RaisePropertyChanged(() => ButtonMDEna); }
        }
        //��ѯ���ڿؼ�����
        private bool queryDateEna;

        public bool QueryDateEna
        {
            get { return queryDateEna; }
            set { queryDateEna = value;RaisePropertyChanged(() => QueryDateEna); }
        }
        //��ѯ��Ʊ�����ı������
        private bool querySnEna;

        public bool QuerySnEna
        {
            get { return querySnEna; }
            set { querySnEna = value; RaisePropertyChanged(() => QuerySnEna); }
        }
        //��ѯ������Ա����
        private bool queryOperatorEna;

        public bool QueryOperatorEna
        {
            get { return queryOperatorEna; }
            set { queryOperatorEna = value; RaisePropertyChanged(() => QueryOperatorEna); }
        }
        //��ѯ���ſ���
        private bool queryDepartmentEna;

        public bool QueryDepartmentEna
        {
            get { return queryDepartmentEna; }
            set { queryDepartmentEna = value; RaisePropertyChanged(() => QueryDepartmentEna); }
        }
        //��ѯ��Ա����
        private bool queryPersonEna;

        public bool QueryPersonEna
        {
            get { return queryPersonEna; }
            set { queryPersonEna = value; RaisePropertyChanged(() => QueryPersonEna); }
        }

        //ϸ����ʾ�ؼ��޸�ʹ��
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
        //��ѯ�Ĳ�����Ա
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

        #region ����
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
                        case "¼�벿��":
                            if (QueryDepartment != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.Person.DepId == QueryDepartment.DepartmentId ).ToList();
                            }
                            break;
                        case "����":
                            QueryInvoices = db.Invoices.ToList();
                            break;
                        case "��Ʊ����":
                            QueryInvoices = db.Invoices.Where(i => i.InvoiceNumber.ToString().Contains(InvoiceSN) ).ToList();
                            break;
                        case "¼����Ա":
                            if (QueryAccountant != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.AccountantId == QueryAccountant.AccountantId ).ToList();
                            }
                            break;
                        case "������":
                            if (QueryPerson != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.PersonId == QueryPerson.PersonId ).ToList();
                            }
                            break;
                        default:
                            break;
                    }
                    if (RadioTimeButton.Content=="¼������")
                    {
                        QueryInvoices = QueryInvoices.Where(i => i.RecDate >= FromDate && i.RecDate <= ToDate).ToList();
                    }
                    else if (RadioTimeButton.Content == "��Ʊ����")
                    {
                        QueryInvoices = QueryInvoices.Where(i => i.Date >= FromDate && i.Date <= ToDate).ToList();
                    }
                }
                else
                {
                    switch (RadioButton.Content)
                    {
                        case "¼�벿��":
                            if (QueryDepartment!=null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.Person.DepId == QueryDepartment.DepartmentId).ToList();
                            }
                            break;
                        case "����":
                            QueryInvoices = db.Invoices.ToList();
                            break;
                        case "��Ʊ����":
                            QueryInvoices = db.Invoices.Where(i => i.InvoiceNumber.ToString().Contains(InvoiceSN)).ToList();
                            break;
                        case "¼����Ա":
                            if (QueryAccountant != null)
                            {
                                QueryInvoices = db.Invoices.Where(i => i.AccountantId == QueryAccountant.AccountantId).ToList();
                            }
                            break;
                        case "������":
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
                State = "�ֶ��޸ķ�Ʊ";
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
                //string responseBody = "{\"reason\":\"�ɹ�\",\"result\":{\"orderid\":\"JH336202003031456261476\",\"xfmc\":\"�Ϻ�Բ��ó�����޹�˾\",\"xfsh\":\"91310114666025597Y\",\"xfdz\":\"�Ϻ��мζ���ҵ��Ҷ��·1118��19��1901�� 021-39915587\",\"xfzh\":\"�������йɷ����޹�˾��������·֧�� 121907731210104\",\"gfmc\":\"����\",\"gfsh\":\"\",\"gfdz\":\"\",\"gfzh\":\"\",\"jym\":\"63936313063773165070\",\"jqbm\":\"661619911295\",\"je\":\"76.00\",\"zfbz\":\"N\",\"bz\":\"������:111636717032\",\"spxx\":[{\"spmc\":\"*�⼰����Ʒ*Ԫʢ ������ԭ��ţ������ 1kg ���ڲ���ţ��\",\"ggxh\":\"ţ����1kg\",\"spdw\":\"��\",\"spsl\":\"1.00000000\",\"spdj\":\"84.00000000\",\"spje\":\"84.00\",\"spslv\":\"��˰\",\"spse\":\"***\",\"flbm\":\"\",\"cph\":\"\",\"type\":\"\",\"txrqq\":\"\",\"txrqz\":\"\"},{\"spmc\":\"*�⼰����Ʒ*Ԫʢ ������ԭ��ţ������ 1kg ���ڲ���ţ��\",\"ggxh\":\"\",\"spdw\":\"\",\"spsl\":\"\",\"spdj\":\"\",\"spje\":\"-8.00\",\"spslv\":\"��˰\",\"spse\":\"***\",\"flbm\":\"\",\"cph\":\"\",\"type\":\"\",\"txrqq\":\"\",\"txrqz\":\"\"}],\"se\":\"0.00\",\"jshj\":\"76.00\",\"fplx\":\"10\"},\"error_code\":0}";
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
                        veriState = "У����ȷ";
                        break;
                    case "233600":
                        veriState = "��Ϣ��һ��";
                        break;
                    case "233602":
                        veriState = "���޴�Ʊ";
                        break;
                    case "233601":
                        veriState = "��Ʊ����";
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
                            QueryInvoice.VerifyState = "����ȷ";
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
                    MessageBox.Show(veriState+",��ѯʧ��!", "��ʾ", MessageBoxButton.OK, MessageBoxImage.Error);
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
            State = "�ֶ����ӷ�Ʊ";
        }
        //��մ����뷢Ʊ��Ϣ

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
                MessageBoxResult result = MessageBox.Show($"ȷ��Ҫɾ����Ʊ����Ϊ{TempViewInvoice.InvoiceNumber}�����Ϊ{TempViewInvoice.Amount}�ķ�Ʊ��",
                    "����", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
        // ȷ�ϰ�ť����
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
                case "�ֶ����ӷ�Ʊ":
                    AppendInvoice();
                    break;
                case "�ֶ��޸ķ�Ʊ":
                    ModifyInvoice();
                    break;
                default:
                    break;
            }

        }


        //ȡ����ť����
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
            State = "��Ʊ��ѯ״̬";
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
                case "����":
                    {
                        QuerySnEna = false;
                        QueryOperatorEna = false;
                        QueryDepartmentEna = false;
                        QueryPersonEna = false;
                    }
                    break;
                case "¼�벿��":
                    {
                        QuerySnEna = false;
                        QueryOperatorEna = false;
                        QueryDepartmentEna = true;
                        QueryPersonEna = false;
                    }
                    break;
                case "��Ʊ����":
                    {
                        QuerySnEna = true;
                        QueryOperatorEna = false;
                        QueryDepartmentEna = false;
                        QueryPersonEna = false;
                    }
                    break;
                case "������":
                    {
                        QuerySnEna = false;
                        QueryOperatorEna = false;
                        QueryDepartmentEna = true;
                        QueryPersonEna = true;
                    }
                    break;
                case "¼����Ա":
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
            QueryDateEna = RadioTimeButton.Content == "����" ? false : true;
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
        //��ѯ�Ĳ����л�
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
                // ѡ���л����š���ǰ�����ˡ�������л�
                Department = Departments.Where(d => d.DepartmentId == DepId).FirstOrDefault();
                Person = Persons.Where(p => p.PersonId == ViewInvoice.PersonId).FirstOrDefault();
                Accountant = Accountants.Where(o => o.AccountantId == ViewInvoice.AcctId).FirstOrDefault();
                TempViewInvoice =(InputInvoice) CloneObject(ViewInvoice);
            }       
        }
        #endregion
    }
}