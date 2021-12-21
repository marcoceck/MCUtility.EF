using MCUtility.EF.Models.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCUtility.EF.Models.Abstract
{
    public interface IApiQuery
    {
        List<RepositoryFilter> filters { get; set; }
        //
        // Summary:
        //     Record to skip
        int Skip { get; set; }
        //
        // Summary:
        //     Number of record to take
        int Take { get; set; }
        //
        // Summary:
        //     Dinamyc sorting string. Examples: "FildName" "FieldName1 ASC, FieldName2 DESC"
        string Sorting { get; set; }
    }
}
