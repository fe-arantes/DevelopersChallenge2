using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accountant.WebApplication.Model.Chart
{
    public class TimeChart2d
    {
        public object ChartTitle { get; set; }
        public List<object> Labels { get; set; }
        public List<object> Values { get; set; }
    }
}
