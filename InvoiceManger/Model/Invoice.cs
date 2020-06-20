using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GalaSoft.MvvmLight;

namespace InvoiceManger.Model
{
  public class Invoice:ObservableObject
    {
        public int ID { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }//发票号码
        public string InvoiceCode { get; set; }//发票代码
        public decimal Amount { get; set; }//发票金额
        [Column(TypeName ="date")]
        public DateTime Date { get; set; }//发票日期
        [Column(TypeName = "date")]
        public DateTime RecDate { get; set; }//录入日期
        public string Verification { get; set; }//校验码
        public string VerificationCode { get; set; }//校验码2
        [ForeignKey("Accountant")]
        public int AccountantId { get; set; }
        public Accountant Accountant { get; set; }//操作员
        [ForeignKey("Person")]
        public int? PersonId { get; set; }//报销的人
        public Person Person { get; set; }
        public string VerifyState { get; set; }//校验状态

    }
}
