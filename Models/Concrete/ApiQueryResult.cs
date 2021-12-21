using MCUtility.EF.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCUtility.EF.Models.Concrete
{
    //
    // Type parameters:
    //   T:
    public class ApiQueryResult<T> : IApiQueryResult<T>
    {
        public ApiQueryResult(List<T> results, int size = 0)
        {
            this.Results = results;
            this.Size = size;
        }

        //
        // Summary:
        //     Paged Results Set from api-query
        public List<T> Results { get; set; }
        //
        // Summary:
        //     Total number of records
        public int Size { get; set; }
    }
}
