using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCUtility.EF.Models.Concrete
{
    public class FilterModel
    {
        public FilterModel() { }

        public string DynamicExpressionFilter { get; set; }
        public object[] FilterValuesArray { get; set; }
    }
}
