using System.Collections.Generic;

namespace Accountant.WebApplication.Model
{
    public class Table
    {
        public List<string> Header { get; set; }
        public List<List<object>> Items { get; set; }
    }
}
