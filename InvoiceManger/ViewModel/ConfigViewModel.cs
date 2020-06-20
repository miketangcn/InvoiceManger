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

namespace InvoiceManger.ViewModel
{
  public class ConfigViewModel:ViewModelBase
    {

        public ConfigViewModel()
        {
            Persons = new ObservableCollection<Person>();
            Departments = new ObservableCollection<Department>();
            Operators = new List<string>();
            AccountantsName = new List<string>();
            if (Information.AccountantName=="陆升")
            {
                ListEna = true;
            }
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
        }

        //两个listview的使能
        private bool listEna;

        public bool ListEna
        {
            get { return listEna; }
            set { listEna = value;RaisePropertyChanged(() => ListEna); }
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
            if (AccountantName!=null)
            {
                BtnAddOperatorEna = Operators.Contains(AccountantName) ? false:true;
            }
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
            if (OperatorName!=null)
            {
                BtnRemoveOperatorEna = true;
            }
        }

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
                    using (var db=new DataModel())
                    {
                        var op = db.Accountants.Where(o => o.AccountantId == Information.AccountantId).First();
                        if (op!=null)
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

        private RelayCommand appendDepartment;

        public RelayCommand AppendDepartment
        {
            get
            {
                if (appendDepartment == null)
                {
                    appendDepartment =  new RelayCommand(() => ExcuteAppendDepartment());
                                        }
                return appendDepartment;
            }
            set { appendDepartment = value; }
        }

        private void ExcuteAppendDepartment()
        {
            if (DepartmentName!= null && DepartmentName != "")
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
                            if (QueryDepartment!=null)
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
            if (DepartmentName != null && DepartmentName != "" && ViewDepartment!=null)
            {
                MessageBoxResult result = MessageBox.Show($"是否将名称为{ViewDepartment.DepartmentName}的部门改为{DepartmentName }", "提示信息",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result==MessageBoxResult.Yes)
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
            if ( ViewDepartment != null)
            {
                if (ViewDepartment.DepartmentName==DepartmentName)
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

        private RelayCommand inputPersons;

        public RelayCommand InputPersons
        {
            get 
            {
                if (inputPersons==null)
                {
                    inputPersons = new RelayCommand(() => ExcuteInputPersons());
                }
                return inputPersons; 
            }
            set { inputPersons = value; }
        }
        //从excel中导入人员
        private void ExcuteInputPersons()
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
            }
           
        }

        private RelayCommand deletePerson;

        public RelayCommand DeletePerson
        {
            get
            {
                if (deletePerson==null)
                {
                    deletePerson = new RelayCommand(() => ExcuteDeletePerson());
                }
                return deletePerson;
            }
            set { deletePerson = value; }
        }

        private void ExcuteDeletePerson()
        {
            bool flag = false;
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
        }

        private RelayCommand modifyPerson;

        public RelayCommand ModifyPerson
        {
            get
            {
                if (modifyPerson==null)
                {
                    modifyPerson = new RelayCommand(() => ExcuteModfiyPerson());
                }
                return modifyPerson;
            }
            set { modifyPerson = value; }
        }

        private void ExcuteModfiyPerson()
        {
            if (Person == null)
            {
                MessageBox.Show("你没有选择任何人员信息，不能执行修改命令", "信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
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
            }
           
        }
    

        private RelayCommand appendPerson;

        public RelayCommand AppendPerson
        {
            get
            {   
                if (appendPerson == null)
                {
                    appendPerson = new RelayCommand(() => ExcuteAppendPerson());
                }
                return appendPerson;
            }
            set { appendPerson = value; }
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
