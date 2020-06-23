using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using InvoiceManger.Common;
using Microsoft.Win32;
using InvoiceManger.Model;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.Entity;
using InvoiceManger.View;

namespace InvoiceManger.ViewModel
{
  public class ConfigViewModel:ViewModelBase
    {
        private Object page;

        public Object Page
        {
            get { return page; }
            set { page = value; RaisePropertyChanged(() => Page); }
        }

        private Object PageDepartmentMange;
        private Object PageAccountMange;
        private Object PagePasswordMange;
        private Object PageStaffMange;

        public ConfigViewModel()
        {
            Persons = new ObservableCollection<Person>();
            Departments = new ObservableCollection<Department>();
            Operators = new List<string>();
            AccountantsName = new List<string>();
            PageDepartmentMange = new DepartmentMange();
            PageAccountMange = new AccountMange();
            PagePasswordMange = new PasswordMange();
            PageStaffMange = new StaffMange();
            Page = PagePasswordMange;
            BtnPageAccountEna = BtnPageDepartmentEna = BtnPageStaffEna = true;
            StaffOperateMode = "浏览模式";
            if (Information.AccountantName=="陆升")
            {
                ListEna = true;
            }
            CurrentOperator = Information.AccountantName;
            Inital();
        }
        public void Inital()
        {

            Departments.Clear();
            using (var db = new DataModel())
            {
                
                tempPersons = new List<Person>();
                var tempdeps = db.Departments.ToList();
                tempdeps.ForEach(d => Departments.Add(d));
                //people中有外键相连的Departments属性，好像一定要通过Departments = db.Departments.ToList()一下 生成的list中才会生成Departments属性，好奇怪
                tempPersons = db.Persons.OrderBy(p => p.DepId).ToList();
                Operators = db.Accountants.Select(p => p.Person.PersonName).ToList(); //查询报销操作人员的姓名到List
                AccountantsName = db.Persons.Where(p => p.Department.DepartmentName == "财务部").Select(p =>  p.PersonName).ToList();//查询财务部人员的姓名到List
                Persons.Clear();
                tempPersons.ForEach(p => Persons.Add(p)); //list 转 obserbableColletion
            }
            Department temp = new Department() { DepartmentId = 0, DepartmentName = "" };
            Departments.Insert(0, temp);
            StaffOperateMode = "浏览模式";
            BtnStaffAppendEna = true;
            BtnStaffModifyEna = true;
            BtnStaffCancelConfirmEna = false;
            ComStaffDepartmentEna = false;
            TextStaffEna = false;
        }
        
        #region 控件使能
        //两个listview的使能
        private bool listEna;

        public bool ListEna
        {
            get { return listEna; }
            set { listEna = value;RaisePropertyChanged(() => ListEna); }
        }

        //config画面切换按钮使能
        private bool btnPagePasswordEna;

        public bool BtnPagePasswordEna
        {
            get { return btnPagePasswordEna; }
            set { btnPagePasswordEna = value; RaisePropertyChanged(() => BtnPagePasswordEna); }
        }

        private bool btnPageAccountEna;

        public bool BtnPageAccountEna
        {
            get { return btnPageAccountEna; }
            set { btnPageAccountEna = value; RaisePropertyChanged(() => BtnPageAccountEna); }
        }

        private bool btnPageDepartmentEna;

        public bool BtnPageDepartmentEna
        {
            get { return btnPageDepartmentEna; }
            set { btnPageDepartmentEna = value;RaisePropertyChanged(() => BtnPageDepartmentEna); }
        }

        private bool btnPageStaffEna;

        public bool BtnPageStaffEna
        {
            get { return btnPageStaffEna; }
            set { btnPageStaffEna = value; RaisePropertyChanged(() => BtnPageStaffEna); }
        }

        // 部门输入文本框获得焦点
        private bool isTextDepartmentFocus;

        public bool IsTextDepartmentFocus
        {
            get { return isTextDepartmentFocus; }
            set { isTextDepartmentFocus = value; RaisePropertyChanged(() => IsTextDepartmentFocus); }
        }

        private bool btnDepartmentAppendEna;

        public bool BtnDepartmentAppendEna
        {
            get { return btnDepartmentAppendEna; }
            set { btnDepartmentAppendEna = value;RaisePropertyChanged(() => BtnDepartmentAppendEna); }
        }
        private bool btnDepartmentModifyEna;

        public bool BtnDepartmentModifyEna
        {
            get { return btnDepartmentModifyEna; }
            set { btnDepartmentModifyEna = value; RaisePropertyChanged(() => BtnDepartmentModifyEna); }
        }
        private bool btnDepartmentCancelConfirm;

        public bool BtnDepartmentCancelConfirm
        {
            get { return btnDepartmentCancelConfirm; }
            set { btnDepartmentCancelConfirm = value;RaisePropertyChanged(() => BtnDepartmentCancelConfirm); }
        }

        //确认添加操作员的按钮使能
        private bool btnAddOperatorEna;

        public bool BtnAddOperatorEna
        {
            get { return btnAddOperatorEna; }
            set { btnAddOperatorEna = value; RaisePropertyChanged(() => BtnAddOperatorEna); }
        }

        //确认移除操作员的按钮使能
        private bool btnRemoveOpertorEna;

        public bool BtnRemoveOperatorEna
        {
            get { return btnRemoveOpertorEna; }
            set { btnRemoveOpertorEna = value;RaisePropertyChanged(() => BtnRemoveOperatorEna); }
        }


        //员工增加查询按钮使能
        private bool btnStaffAppendEna;

        public bool BtnStaffAppendEna
        {
            get { return btnStaffAppendEna; }
            set { btnStaffAppendEna = value;RaisePropertyChanged(() => BtnStaffAppendEna); }
        }
        //员工修改删除按钮使能
        private bool btnStaffModifyEna;

        public bool BtnStaffModifyEna
        {
            get { return btnStaffModifyEna; }
            set { btnStaffModifyEna = value; RaisePropertyChanged(() => BtnStaffModifyEna); }
        }
        //员工操作确认取消按钮使能
        private bool btnStaffCancelConfirmEna;

        public bool BtnStaffCancelConfirmEna
        {
            get { return btnStaffCancelConfirmEna; }
            set { btnStaffCancelConfirmEna = value; RaisePropertyChanged(() => BtnStaffCancelConfirmEna); }
        }

        //员工输入文本框获得焦点
        private bool isTextStaffFocus;

        public bool IsTextStaffFocus
        {
            get { return isTextStaffFocus; }
            set { isTextStaffFocus = value; RaisePropertyChanged(() => IsTextStaffFocus); }
        }

        //输入文本框使能
        private bool textStaffEna;

        public bool TextStaffEna
        {
            get { return textStaffEna; }
            set { textStaffEna = value; RaisePropertyChanged(() => TextStaffEna); }
        }

        //员工画面部门combox使能 ComStaffDepartmentEna 
        private bool comStaffDepartmentEna;

        public bool ComStaffDepartmentEna
        {
            get { return comStaffDepartmentEna; }
            set { comStaffDepartmentEna = value; RaisePropertyChanged(() => ComStaffDepartmentEna); }
        }
        #endregion

        #region 变量定义

        //部门操作模式
        private string departmentOperateMode;

        public string DepartmentOperateMode
        {
            get { return departmentOperateMode; }
            set { departmentOperateMode = value; RaisePropertyChanged(() => DepartmentOperateMode); }
        }
        //员工操作模式
        private string staffOperateMode;

        public string StaffOperateMode
        {
            get { return staffOperateMode; }
            set { staffOperateMode = value;RaisePropertyChanged(() => StaffOperateMode); }
        }

        
        //当前操作人员
        private string currentOperator;

        public string CurrentOperator
        {
            get { return currentOperator; }
            set { currentOperator = value; RaisePropertyChanged(() => CurrentOperator); }
        }

        private List<string> accountantsName;

        public List<string> AccountantsName
        {
            get { return accountantsName; }
            set { accountantsName = value;RaisePropertyChanged(() => AccountantsName); }
        }
        private string accountantName;

        public string AccountantName
        {
            get { return accountantName; }
            set { accountantName = value;RaisePropertyChanged(() => AccountantName); }
        }
        private List<string> operators;

        public List<string> Operators
        {
            get { return operators; }
            set { operators = value;RaisePropertyChanged(() => Operators); }
        }
        private string operatorName;

        public string OperatorName
        {
            get { return operatorName; }
            set { operatorName = value; RaisePropertyChanged(() => OperatorName); }
        }


        private List<Person> tempPersons;

        private string newPassword;

        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = value;RaisePropertyChanged(() => NewPassword); }
        }
        private string secondPassword;

        public string SecondPassword
        {
            get { return secondPassword; }
            set { secondPassword = value;RaisePropertyChanged(() => SecondPassword); }
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
            set { queryDepartment = value; RaisePropertyChanged(() => QueryDepartment); }
        }

        private Department viewDepartment;

        public Department ViewDepartment
        {
            get { return viewDepartment; }
            set { viewDepartment = value; RaisePropertyChanged(() => ViewDepartment); }
        }

        private Person person;

        public Person Person
        {
            get { return person; }
            set { person = value;RaisePropertyChanged(() => Person); }
        }


        private ObservableCollection<Department> departments;

        public ObservableCollection<Department> Departments
        {
            get { return departments; }
            set { departments = value; RaisePropertyChanged(() => Departments); }
        }
        private ObservableCollection<Person> persons;

        public ObservableCollection<Person> Persons
        {
            get { return persons; }
            set { persons = value; RaisePropertyChanged(() => Persons); }
        }

        private string personName;

        public string PersonName
        {
            get { return personName; }
            set { personName = value; RaisePropertyChanged(() => PersonName); }
        }
        private string departmentName;

        public string DepartmentName
        {
            get { return departmentName; }
            set { departmentName = value;RaisePropertyChanged(() => DepartmentName); }
        }
        #endregion

        #region 主画面控件操作
        //画面切换按钮事件
        private RelayCommand<string> cmdPage;

        public RelayCommand<string> CmdPage
        {
            get
            {
                if (cmdPage == null)
                {
                    cmdPage = new RelayCommand<string>((p) => ExcuteCmdPage(p));
                }
                return cmdPage;
            }
            set { cmdPage = value; }
        }

        private void ExcuteCmdPage(string p)
        {
            switch (p)
            {
               case "密码管理":
                Page = PagePasswordMange;
                    BtnPagePasswordEna = false;
                    BtnPageAccountEna = BtnPageDepartmentEna = BtnPageStaffEna = true;
                    break;
                case "员工管理":
                    Page = PageStaffMange;
                    BtnPageStaffEna = false;
                    BtnPageAccountEna = BtnPageDepartmentEna = BtnPagePasswordEna = true;
                    break;
                case "操作员管理":
                    Page = PageAccountMange;
                    BtnPageAccountEna = false;
                    BtnPagePasswordEna = BtnPageDepartmentEna = BtnPageStaffEna = true;
                    break;
                case "部门管理":
                    Page = PageDepartmentMange;
                    BtnPageDepartmentEna = false;
                    BtnPageAccountEna = BtnPagePasswordEna = BtnPageStaffEna = true;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 操作员管理画面事件

        //添加财务操作员按钮事件
        private RelayCommand cmdAddOperator;

        public RelayCommand CmdAddOperator
        {
            get
            {
                if (cmdAddOperator == null)
                {
                    cmdAddOperator = new RelayCommand(() => ExcuteCmdAddOperator());
                }
                return cmdAddOperator;
            }
            set { cmdAddOperator = value; }
        }

        private void ExcuteCmdAddOperator()
        {
            
            using (var db = new DataModel())
            {
                var person = db.Persons.Where(p => p.PersonName == AccountantName).First();
                Accountant op = new Accountant();
               if (person!=null)
                {
                    op = new Accountant() { Password = "111111",Person= person };
                    MessageBoxResult result = MessageBox.Show($"确定要增加{AccountantName}为操作员权限？", "信息提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        db.Accountants.Add(op);
                        db.SaveChanges();
                        Inital();
                        BtnAddOperatorEna = false;
                    }
                }
            }
        }
        //移除财务操作员按钮事件
        private RelayCommand cmdRemoveOperator;

        public RelayCommand CmdRemoveOperator
        {
            get
            {
                if (cmdRemoveOperator == null)
                {
                    cmdRemoveOperator = new RelayCommand(() => ExcuteCmdRemoveOperator());
                }
                return cmdRemoveOperator;
            }
            set { cmdRemoveOperator = value; }
        }

        private void ExcuteCmdRemoveOperator()
        {
            using (var db=new DataModel())
            {
                var op = db.Accountants.Where(a => a.Person.PersonName == OperatorName).First();
                if (op!=null)
                {
                   MessageBoxResult result= MessageBox.Show($"确定要将{OperatorName}操作员权限取消？", "信息提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result==MessageBoxResult.Yes)
                    {
                        db.Accountants.Remove(op);
                        db.SaveChanges();
                        Inital();                   
                    }
                }
            }
            BtnRemoveOperatorEna = false;
        }
        //财务部人员listview 变化事件
        private RelayCommand acctListChange;

        public RelayCommand AcctListChange
        {
            get
            {
                if (acctListChange == null)
                {
                    acctListChange = new RelayCommand(() => ExcuteAcctListChange());
                }
                return acctListChange;
            }
            set { acctListChange = value; }
        }

        private void ExcuteAcctListChange()
        {
            if (AccountantName != null)
            {
                BtnAddOperatorEna = Operators.Contains(AccountantName) ? false : true;
            }
            else BtnAddOperatorEna = false;
        }

        //操作人员listview变化事件
        private RelayCommand operatorListChange;

        public RelayCommand OperatorListChange
        {
            get
            {
                if (operatorListChange == null)
                {
                    operatorListChange = new RelayCommand(() => ExcuteOperatorListChange());
                }
                return operatorListChange;
            }
            set { operatorListChange = value; }
        }

        private void ExcuteOperatorListChange()
        {
            if (OperatorName!=null && OperatorName!="陆升")
            {
                BtnRemoveOperatorEna = true;
            }
            else BtnRemoveOperatorEna = false;
        }
        #endregion

        #region 部门管理画面事件
        private RelayCommand departmentListChange;

        public RelayCommand DepartmentListChange
        {
            get
            {
                if (departmentListChange == null)
                {
                    departmentListChange = new RelayCommand(() => ExcuteDepartmentListChange());
                }
                return departmentListChange;
            }
            set { departmentListChange = value; }
        }

        private void ExcuteDepartmentListChange()
        {
            if (ViewDepartment!=null)
            {
                DepartmentName = ViewDepartment.DepartmentName;
            }

        }

        private RelayCommand<string> cmdDepartment;

        public RelayCommand<string> CmdDepartment
        {
            get
            {
                if (cmdDepartment == null)
                {
                    cmdDepartment = new RelayCommand<string>((p) => ExcuteCmdDepartment(p));
                }
                return cmdDepartment;
            }
            set { cmdDepartment = value; }
        }

        private void ExcuteCmdDepartment(string p)
        {
            switch (p)
            {
                //case "新增人员":
                //    StaffOperateMode = "新增人员";
                //    BtnStaffAppendEna = false;
                //    BtnStaffCancelConfirmEna = true;
                //    BtnStaffModifyEna = false;
                //    ComStaffDepartmentEna = true;
                //    TextStaffEna = true;
                //    IsTextStaffFocus = true;
                //    PersonName = "";
                //    break;
                //case "修改人员":
                //    StaffOperateMode = "修改人员";
                //    BtnStaffAppendEna = false;
                //    BtnStaffCancelConfirmEna = true;
                //    BtnStaffModifyEna = false;
                //    ComStaffDepartmentEna = true;
                //    TextStaffEna = true;
                //    IsTextStaffFocus = true;
                //    break;
                //case "查询人员":
                //    staffOperateMode = "查询人员";
                //    BtnStaffAppendEna = false;
                //    BtnStaffCancelConfirmEna = true;
                //    BtnStaffModifyEna = false;
                //    TextStaffEna = true;
                //    IsTextStaffFocus = true;
                //    break;
                //default:
                //    break;
            }
        }

        private RelayCommand appendDepartment;

        public RelayCommand AppendDepartment
        {
            get
            {
                if (appendDepartment == null)
                {
                    appendDepartment = new RelayCommand(() => ExcuteAppendDepartment());
                }
                return appendDepartment;
            }
            set { appendDepartment = value; }
        }

        private void ExcuteAppendDepartment()
        {
            if (DepartmentName != null && DepartmentName != "")
            {
                MessageBoxResult result = MessageBox.Show($"是否添加名称为{DepartmentName }的部门", "提示信息",
                     MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new DataModel())
                    {
                        if (db.Departments.Where(d => d.DepartmentName == DepartmentName).Count() > 0)
                        {
                            MessageBox.Show("请检查存在相同部门名称", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            Department department = new Department() { DepartmentName = DepartmentName };
                            db.Departments.Add(department);
                            db.SaveChanges();
                            int tempid = 0;
                            if (QueryDepartment != null)
                            {
                                tempid = QueryDepartment.DepartmentId;
                            };
                            Inital();
                            QueryDepartment = Departments.Where(d => d.DepartmentId == tempid).FirstOrDefault();
                            ExcuteCbChangeCommand();
                            ViewDepartment = Departments.Where(d => d.DepartmentId == department.DepartmentId).FirstOrDefault();
                        }
                    }
                }
            }
        }
        private RelayCommand modifyDepartment;

        public RelayCommand ModifyDepartment
        {
            get
            {
                if (modifyDepartment == null)
                {
                    modifyDepartment = new RelayCommand(() => ExcuteModifyDepartment());
                }
                return modifyDepartment;
            }
            set { modifyDepartment = value; }
        }

        private void ExcuteModifyDepartment()
        {
            if (DepartmentName != null && DepartmentName != "" && ViewDepartment != null)
            {
                MessageBoxResult result = MessageBox.Show($"是否将名称为{ViewDepartment.DepartmentName}的部门改为{DepartmentName }", "提示信息",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new DataModel())
                    {
                        var department = db.Departments.Where(d => d.DepartmentId == ViewDepartment.DepartmentId).First();
                        if (department != null)
                        {
                            department.DepartmentName = DepartmentName;
                            db.SaveChanges();
                            int tempid = 0;
                            if (QueryDepartment != null)
                            {
                                tempid = QueryDepartment.DepartmentId;
                            };
                            Inital();
                            QueryDepartment = Departments.Where(d => d.DepartmentId == tempid).FirstOrDefault();
                            ExcuteCbChangeCommand();
                            ViewDepartment = Departments.Where(d => d.DepartmentId == department.DepartmentId).FirstOrDefault();
                        }
                    };
                }
            }
        }
        private RelayCommand deleteDepartment;

        public RelayCommand DeleteDepartment
        {
            get
            {
                if (deleteDepartment == null)
                {
                    deleteDepartment = new RelayCommand(() => ExcuteDeleteDepartment());
                }
                return deleteDepartment;
            }
            set { deleteDepartment = value; }
        }

        private void ExcuteDeleteDepartment()
        {
            if (ViewDepartment != null)
            {
                if (ViewDepartment.DepartmentName == DepartmentName)
                {
                    MessageBoxResult result = MessageBox.Show($"是否将名称为{ViewDepartment.DepartmentName}的部门删除", "提示信息",
                                       MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var db = new DataModel())
                        {
                            var department = db.Departments.Where(d => d.DepartmentId == ViewDepartment.DepartmentId).First();
                            if (department != null)
                            {
                                db.Departments.Remove(department);
                                db.SaveChanges();
                                int tempid = 0;
                                if (QueryDepartment != null)
                                {
                                    tempid = QueryDepartment.DepartmentId;
                                };
                                Inital();
                                QueryDepartment = Departments.Where(d => d.DepartmentId == tempid).FirstOrDefault();
                                ExcuteCbChangeCommand();
                                ViewDepartment = Departments.Where(d => d.DepartmentId == 0).FirstOrDefault();
                            }
                        };
                    }
                }
                else MessageBox.Show("部门名称已经修改", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        #region 员工管理画面事件
        private RelayCommand cbChangeCommand;

        public RelayCommand CbChangeCommand
        {
            get
            {
                if (cbChangeCommand == null)
                {
                    cbChangeCommand = new RelayCommand(() => ExcuteCbChangeCommand());
                }
                return cbChangeCommand;
            }
            set { cbChangeCommand = value; }
        }

        private void ExcuteCbChangeCommand()
        {
            
            using (var db = new DataModel())
            {
                if (QueryDepartment != null)
                {
                    Persons.Clear();
                    if (QueryDepartment.DepartmentId==0)
                    {
                       tempPersons.ForEach(p => Persons.Add(p));
                    }
                    else
                    { 
                       var temp1persons = tempPersons.Where(p=>p.DepId==QueryDepartment.DepartmentId).ToList();
                       temp1persons.ForEach(p => Persons.Add(p));
                    }
                }
            }

        }

        private RelayCommand  peopleGridSelectChange;

        public RelayCommand  PeopleGridSelectChange
        {
            get
            {
                if (peopleGridSelectChange == null)
                {
                    peopleGridSelectChange = new RelayCommand(() =>ExcutePeopleGridSelectChange());
                }
                return peopleGridSelectChange;
            }
            set { peopleGridSelectChange = value; }
        }

        private void ExcutePeopleGridSelectChange()
        {
            // EnaButton = ViewInvoice != null ? true : false;
            if (Person != null)
            {
                PersonName = Person.PersonName;
                Department = Person.Department;
            }
            else PersonName = "";
        }

    
        private RelayCommand cmdInputPersons;

        public RelayCommand CmdInputPersons
        {
            get 
            {
                if (cmdInputPersons == null)
                {
                    cmdInputPersons = new RelayCommand(() => ExcuteCmdInputPersons());
                }
                return cmdInputPersons; 
            }
            set { cmdInputPersons = value; }
        }
        //从excel中导入人员
        private void ExcuteCmdInputPersons()
        {
            string xpath;
            List<viewPerson> viewPeople = new List<viewPerson>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            if (openFileDialog.ShowDialog()==true)
            {
                xpath= openFileDialog.FileName;
                viewPeople =NpoiHelper.ReadToList<viewPerson>(xpath, 0);
            }
            if (viewPeople.Count>0)
            {
                using (var db = new DataModel())
                {
                    foreach (var item in viewPeople)
                    {
                        if (db.Persons.Where(p => p.PersonName == item.Name && p.Department.DepartmentName == item.Department).Count() <= 0)
                        {
                            Department department = db.Departments.Where(d => d.DepartmentName == item.Department).First();
                            if (department!=null)
                            {
                                Person person = new Person() { DepId = department.DepartmentId, PersonName = item.Name };
                                db.Persons.Add(person);
                            }
                        }
                    }
                    db.SaveChanges();
                }
                int tempid = 0;
                if (QueryDepartment != null)
                {
                    tempid = QueryDepartment.DepartmentId;
                }
                Inital();
                QueryDepartment = Departments.Where(d => d.DepartmentId == tempid).FirstOrDefault();
                ExcuteCbChangeCommand();
            }
           
        }
        //员工操作
        private RelayCommand<string> cmdPerson;

        public RelayCommand<string> CmdPerson
        {
            get
            {
                if (cmdPerson == null)
                {
                    cmdPerson = new RelayCommand<string>((p) => ExcuteCmdPerson(p));
                }
                return cmdPerson;
            }
            set { cmdPerson = value; }
        }

        private void ExcuteCmdPerson(string p)
        {
            switch (p)
            {
                case "新增人员":
                    StaffOperateMode = "新增人员";
                    BtnStaffAppendEna = false;            
                    BtnStaffCancelConfirmEna = true;
                    BtnStaffModifyEna = false;
                    ComStaffDepartmentEna = true;
                    TextStaffEna = true;
                    IsTextStaffFocus = true;
                    PersonName = "";
                    break;
                case "修改人员":
                    StaffOperateMode = "修改人员";
                    BtnStaffAppendEna = false;
                    BtnStaffCancelConfirmEna = true;
                    BtnStaffModifyEna = false;
                    ComStaffDepartmentEna = true;
                    TextStaffEna = true;
                    IsTextStaffFocus = true;
                    break;
                case "查询人员":
                    staffOperateMode = "查询人员";
                    BtnStaffAppendEna = false;
                    BtnStaffCancelConfirmEna = true;
                    BtnStaffModifyEna = false;
                    TextStaffEna = true;
                    IsTextStaffFocus = true;
                    break;
                default:
                    break;
            }
        }

        private RelayCommand cmdStaffMangeConfirm;
        public RelayCommand CmdStaffMangeConfirm
        {
            get
            {
                if (cmdStaffMangeConfirm == null)
                {
                    cmdStaffMangeConfirm = new RelayCommand(() => ExcuteCmdStaffMangeConfirm());
                }
                return cmdStaffMangeConfirm;
            }
            set { cmdStaffMangeConfirm = value; }
        }

        private void ExcuteCmdStaffMangeConfirm()
        {
            switch (StaffOperateMode)
            {
                case "新增人员":
                    ExcuteAppendPerson();
                    break;
                case "修改人员":
                    ExcuteModfiyPerson();
                    break;
                case "查询人员":
                    ExcuteQueryPerson();
                    break;
                default:
                    break;
            }
        }

        private RelayCommand cmdStaffMangeCancle;
        public RelayCommand CmdStaffMangeCancle
        {
            get
            {
                if (cmdStaffMangeCancle == null)
                {
                    cmdStaffMangeCancle = new RelayCommand(() => ExcuteCmdStaffMangeCancle());
                }
                return cmdStaffMangeCancle;
            }
            set { cmdStaffMangeCancle = value; }
        }

        private void ExcuteCmdStaffMangeCancle()
        {
            StaffOperateMode = "浏览模式";
            BtnStaffAppendEna = true;
            BtnStaffModifyEna = true;
            ComStaffDepartmentEna = false;
            TextStaffEna = false;
        }

        private RelayCommand cmddeletePerson;

        public RelayCommand CmdDeletePerson
        {
            get
            {
                if (cmddeletePerson==null)
                {
                    cmddeletePerson = new RelayCommand(() => ExcuteCmdDeletePerson());
                }
                return cmddeletePerson;
            }
            set { cmddeletePerson = value; }
        }

        private void ExcuteCmdDeletePerson()
        {
            bool flag = false;
            StaffOperateMode = "删除人员";
            BtnStaffAppendEna = false;
            BtnStaffModifyEna = false;
            ComStaffDepartmentEna = true;
            TextStaffEna = true;
            try
            {
                using (var db = new DataModel())
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show($"是否在人员清册中删除{Department.DepartmentName}的{Person.PersonName}的信息?"
                            , "修改人员提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        Person tempperson = db.Persons.Where(p => p.PersonId == Person.PersonId).First();
                        if (tempperson != null)
                        {
                            db.Persons.Remove(tempperson);
                            db.SaveChanges();
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    var tempid = QueryDepartment.DepartmentId;
                    Inital();
                    QueryDepartment = Departments.Where(d => d.DepartmentId == tempid).FirstOrDefault();
                    ExcuteCbChangeCommand();
                }
            }
            catch (Exception)
            {
            }
            StaffOperateMode = "浏览模式";
            BtnStaffAppendEna = true;
            BtnStaffModifyEna = true;
            ComStaffDepartmentEna = false;
            TextStaffEna = false;            
        }


        private void ExcuteModfiyPerson()
        {
            //if (Person == null)
            //{
            //    MessageBox.Show("你没有选择任何人员信息，不能执行修改命令", "信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //else
            //{
                bool flag = false;
                try
                {
                    using (var db = new DataModel())
                    {
                        MessageBoxResult messageBoxResult = MessageBox.Show($"是否修改{Person.Department.DepartmentName}的{Person.PersonName}信息为{Department.DepartmentName}的{PersonName}?"
                                , "修改人员提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            Person person = db.Persons.Where(p => p.PersonId == Person.PersonId).First();
                                person.PersonName = PersonName;
                                person.DepId = Department.DepartmentId;
                            if (db.ChangeTracker.HasChanges())
                            {
                                flag = true;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (flag)
                    {
                        int tempid = 0;
                        if (QueryDepartment != null)
                        {
                            tempid = QueryDepartment.DepartmentId;
                        }
                        Inital();
                        QueryDepartment = Departments.Where(d => d.DepartmentId == tempid).FirstOrDefault();
                        ExcuteCbChangeCommand();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            StaffOperateMode = "浏览模式";
            BtnStaffAppendEna = true;
            BtnStaffModifyEna = true;
            BtnStaffCancelConfirmEna = false;
            ComStaffDepartmentEna = false;
            TextStaffEna = false;
            //}

        }
              
        private void ExcuteAppendPerson()
        {
            bool flag =false;
            if (Department!=null && Department.DepartmentId!=0 && PersonName!=""&& PersonName!=null)
            {
                try
                {
                    using (var db = new DataModel())
                    {
                        if (db.Persons.Where(p => p.PersonName == PersonName).Count() > 0)
                        {
                            MessageBoxResult messageBoxResult = MessageBox.Show($"已经存在姓名为{PersonName}的人员，确定还要添加同名人员到部门：{Department.DepartmentName}中?"
                                , "添加人员提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                Person person = new Person() { PersonName = PersonName, DepId = Department.DepartmentId };
                                db.Persons.Add(person);
                                flag = true;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            MessageBoxResult messageBoxResult = MessageBox.Show($"添加姓名为{PersonName}的人员到部门：{Department.DepartmentName}中?"
                                  , "添加人员提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                Person person = new Person() { PersonName = PersonName, DepId = Department.DepartmentId };
                                db.Persons.Add(person);
                                flag = true;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (flag)
                    {
                        int tempid = 0;
                        if (QueryDepartment!=null)
                        {
                            tempid = QueryDepartment.DepartmentId;
                        }
                        Inital();
                        PersonName = "";
                        QueryDepartment = Departments.Where(d => d.DepartmentId == tempid).FirstOrDefault();
                        ExcuteCbChangeCommand();
                    }

                }
                catch (Exception)
                {
                }
            }
            else MessageBox.Show($"请检查待添加人员信息是否正确"
                                  , "信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExcuteQueryPerson()
        {
            if (PersonName!=null && PersonName!="")
            {
                using (var db = new DataModel())
                {
                    var temp1persons = tempPersons.Where(p => p.PersonName.Contains(PersonName)).ToList();
                    Persons.Clear();
                    temp1persons.ForEach(p => Persons.Add(p));                  
                }
            }
            StaffOperateMode = "浏览模式";
            BtnStaffAppendEna = true;
            BtnStaffModifyEna = true;
            BtnStaffCancelConfirmEna = false;
            ComStaffDepartmentEna = false;
            TextStaffEna = false;
        }

        #endregion

        #region 密码管理画面事件
        private RelayCommand modifyPassword;
        public RelayCommand ModifyPassword
        {
            get
            {
                if (modifyPassword == null)
                {
                    modifyPassword = new RelayCommand(() => ExcuteModifyPassword());
                }
                return modifyPassword;
            }
            set { modifyPassword = value; }
        }

        private void ExcuteModifyPassword()
        {
            if (NewPassword == SecondPassword && NewPassword.Length >= 6)
            {
                MessageBoxResult result = MessageBox.Show($"是否修改当前用户：{Information.AccountantName}的用户密码？", "提示信息",
                     MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new DataModel())
                    {
                        var op = db.Accountants.Where(o => o.AccountantId == Information.AccountantId).First();
                        if (op != null)
                        {
                            op.Password = NewPassword;
                            db.SaveChanges();
                            MessageBox.Show("密码修改完成，请退出软件后重新登陆！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            else MessageBox.Show("两次密码不同，请检查！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private RelayCommand clear;

        public RelayCommand Clear
        {
            get
            {
                if (clear == null)
                {
                    clear = new RelayCommand(() => ExcuteClear());
                }
                return clear;
            }
            set { clear = value; }
        }

        private void ExcuteClear()
        {
            NewPassword = SecondPassword = "";
        }
        #endregion
    }
    public class viewPerson:ObservableObject
    {
        private string department;

        public string Department
        {
            get { return department; }
            set { department = value;RaisePropertyChanged(() => Department); }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(() => Name); }
        }

    }
}
