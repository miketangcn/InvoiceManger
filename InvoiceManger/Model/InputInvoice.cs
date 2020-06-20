using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace InvoiceManger.Model
{
   public class InputInvoice:ObservableObject
    {
        private int iD;

        public int ID
        {
            get { return iD; }
            set { iD = value;RaisePropertyChanged(() => ID); }
        }

        private string invoiceNumber;

        public string InvoiceNumber
        {
            get { return invoiceNumber; }
            set { invoiceNumber = value;RaisePropertyChanged(() => InvoiceNumber); }
        }
        private string invoiceCode;

        public string InvoiceCode
        {
            get { return invoiceCode; }
            set { invoiceCode = value;RaisePropertyChanged(() => InvoiceCode); }
        }

        private decimal amount;

        public decimal Amount //发票金额
        {
            get { return amount; }
            set { amount = value; RaisePropertyChanged(() => Amount); }
        }
        private DateTime  date;

        public DateTime Date //发票日期
        {
            get { return date; }
            set { date = value;RaisePropertyChanged(() => Date); }
        }
        private DateTime recDate;

        public DateTime RecDate //录入日期
        {
            get { return recDate; }
            set { recDate = value;RaisePropertyChanged(() => RecDate); }
        }
        private string verification;//校验码

        public string Verification
        {
            get { return verification; }
            set { verification = value; RaisePropertyChanged(() => Verification); }
        }
        private string verificationCode;//校验码2

        public string VerificationCode
        {
            get { return verificationCode; }
            set { verificationCode = value; RaisePropertyChanged(() => VerificationCode); }
        }
        private int personId;//报销人编码

        public int PersonId
        {
            get { return personId; }
            set { personId = value; RaisePropertyChanged(() => PersonId); }
        }
        private string personName;// 报销人姓名

        public string PersonName
        {
            get { return personName; }
            set { personName = value;RaisePropertyChanged(() => PersonName); }
        }

        private int acctId;//财务人员编码

        public int AcctId
        {
            get { return acctId; }
            set { acctId = value;RaisePropertyChanged(() => AcctId); }
        }
        private string acctName;

        public string AcctName
        {
            get { return acctName; }
           set { acctName = value;RaisePropertyChanged(() => AcctName); }
        }
        private string verifyState;

        public string VerifyState
        {
            get { return verifyState; }
            set { verifyState = value; RaisePropertyChanged(() => VerifyState); }
        }



    }
}
