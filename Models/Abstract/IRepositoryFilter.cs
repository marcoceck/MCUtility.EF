using System;
using System.Collections.Generic;
using System.Text;

namespace MCUtility.EF.Models
{
    public interface IRepositoryFilter
    {
        string FieldName { get; set; }
        string FieldValue { get; set; }
        string FieldValueMax { get; set; }
        string FieldValueMin { get; set; }
        List<object> FieldValues { get; set; }
        string MatchMode { get; set; }
        string FieldType { get; set; }
    }
}
