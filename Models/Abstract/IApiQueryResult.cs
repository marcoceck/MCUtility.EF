using System;
using System.Collections.Generic;
using System.Text;

namespace MCUtility.EF.Models.Abstract
{
    public interface IApiQueryResult<T>
    {
        List<T> Results { get; set; }
        int Size { get; set; }
    }
}
