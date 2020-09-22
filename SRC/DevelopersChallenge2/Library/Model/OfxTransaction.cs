using System;
using System.Collections.Generic;
using System.Text;

namespace Accountant.Library.Model
{
    public class OfxTransaction
    {
        public TransactionType Type { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public decimal TransactionValue { get; set; }

        public enum TransactionType
        {
            DEBIT = 0,
            CREDIT = 1
        }
    }
}
