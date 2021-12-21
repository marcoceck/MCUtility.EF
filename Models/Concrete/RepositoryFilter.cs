using System;
using System.Collections.Generic;
using System.Text;

namespace MCUtility.EF.Models.Concrete
{
    public class RepositoryFilter : IRepositoryFilter
    {
        public RepositoryFilter() { }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public List<object> FieldValues { get; set; }
        public string FieldValueMin { get; set; }
        public string FieldValueMax { get; set; }
        public string MatchMode { get; set; }
        public string FieldType { get; set; }

        public enum FieldTypeEnum
        {
            NUMBER = 1,
            BOOLEAN = 2,
            TEXT = 3,
            DATE = 4,
            MULTI = 5,
            SELECT = 6,
            RANGE_NUMBER = 7,
            RANGE_NUMBER_INT = 8,
            OBJECT_NAVISION = 9
        }
    }
}
