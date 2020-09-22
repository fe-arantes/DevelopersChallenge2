using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accountant.WebApplication.Model
{
    public class Totals
    {
        public object total { get; set; }
        public object totalCredits { get; set; }
        public object totalDebits { get; set; }
        public object totalMovements { get; set; }
    }
}
