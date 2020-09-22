using System;
using System.Collections.Generic;
using System.Text;

namespace Accountant.Library.Model
{
    public class Ofx
    {
        public OfxBank Bank { get; set; }
        public List<OfxTransaction> Transactions { get; set; }
        public string ErrorMessage { get; set; }
    }
}
