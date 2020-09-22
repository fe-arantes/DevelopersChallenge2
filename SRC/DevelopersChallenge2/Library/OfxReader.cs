using Accountant.Library.Enums;
using Accountant.Library.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Accountant.Library
{
    public class OfxReader
    {
        private readonly Stream streamFile;

        public OfxReader(string ofxFilePath)
        {
            streamFile = File.Exists(ofxFilePath) ? File.OpenRead(ofxFilePath) : null;
        }

        public OfxReader(Stream streamFile)
        {
            this.streamFile = streamFile;
        }

        public Ofx Parse()
        {
            var ofx = new Ofx
            {
                Bank = new OfxBank(),
                Transactions = new List<OfxTransaction>(),
                ErrorMessage = string.Empty,
            };

            if (streamFile == null)
            {
                ofx.ErrorMessage = "OFX file not found";
                return ofx;
            }

            ImportFile(ofx);

            return ofx;
        }

        private void ImportFile(Ofx ofx)
        {
            var stacks = new Stack();
            var transaction = new OfxTransaction();
            using var reader = new StreamReader(streamFile);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.StartsWith("<"))
                {
                    continue;
                }

                var tag = line.Substring(1, line.IndexOf('>') - 1).Replace("/", "");

                if (line.StartsWith("</") && line.EndsWith(">"))
                {
                    if (stacks.Pop().ToString() != line.Replace("/", "").ToUpper())
                    {
                        ofx.ErrorMessage = "Invalid File";
                        break;
                    }

                    if (tag == OfxTag.TransactionTags.STMTTRN.ToString())
                    {
                        ofx.Transactions.Add(transaction);
                        transaction = new OfxTransaction();
                    }

                    continue;
                }

                if (line.StartsWith("<") && line.EndsWith(">"))
                {
                    stacks.Push(line.ToUpper());
                    continue;
                }

                if (Enum.TryParse(tag, out OfxTag.BankAccTags bankAccTags))
                {
                    FillBank(ofx.Bank, bankAccTags, line);
                    continue;
                }

                if (Enum.TryParse(tag, out OfxTag.TransactionTags transactionTags))
                {
                    FillTransaction(transaction, transactionTags, line);
                    continue;
                }
            }

            if (stacks.Count > 0)
            {
                ofx.ErrorMessage = "Invalid File";
            }
        }

        private void FillBank(OfxBank ofxBank, OfxTag.BankAccTags bankAccTags, string line)
        {
            switch (bankAccTags)
            {
                case OfxTag.BankAccTags.BANKID:
                    ofxBank.BankId = GetTagValue(line);
                    break;

                case OfxTag.BankAccTags.ACCTID:
                    ofxBank.AccountId = GetTagValue(line);
                    break;
            }
        }

        private void FillTransaction(OfxTransaction ofxTransaction, OfxTag.TransactionTags transactionTags, string line)
        {
            switch (transactionTags)
            {
                case OfxTag.TransactionTags.TRNTYPE:
                    var tagValue = GetTagValue(line);
                    if (Enum.TryParse(tagValue, out OfxTransaction.TransactionType transactionType))
                    {
                        ofxTransaction.Type = transactionType;
                    }
                    break;

                case OfxTag.TransactionTags.DTPOSTED:
                    ofxTransaction.Date = GetDate(line);
                    break;

                case OfxTag.TransactionTags.TRNAMT:
                    ofxTransaction.TransactionValue = Convert.ToDecimal(GetTagValue(line), CultureInfo.InvariantCulture);
                    break;

                case OfxTag.TransactionTags.MEMO:
                    ofxTransaction.Description = GetTagValue(line);
                    break;
            }
        }

        private string GetTagValue(string line)
        {
            var initialIndex = line.IndexOf(">") + 1;

            var tagValue = initialIndex < 1 ? string.Empty : line[initialIndex..].Trim();

            return tagValue;
        }

        private DateTime GetDate(string line)
        {
            var tagValue = GetTagValue(line);

            tagValue = tagValue.Substring(0, tagValue.IndexOf("["));

            var date = DateTime.ParseExact(tagValue, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            return date;
        }
    }
}
