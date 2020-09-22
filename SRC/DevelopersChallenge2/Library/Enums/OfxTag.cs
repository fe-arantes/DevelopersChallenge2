using System;
using System.Collections.Generic;
using System.Text;

namespace Accountant.Library.Enums
{
    public static class OfxTag
    {
        public enum BankAccTags
        {
            BANKACCTFROM,
            BANKID,
            ACCTID,
            ACCTTYPE
        }

        public enum TransactionTags
        {
            STMTTRN,
            TRNTYPE,
            DTPOSTED,
            TRNAMT,
            MEMO
        }

        public enum Balance
        {
            LEDGERBAL,
            BALAMT,
            DTASOF
        }

    }
}
