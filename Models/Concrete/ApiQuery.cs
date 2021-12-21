using MCUtility.EF.Models.Abstract;
using MCUtility.EF.Models.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.App_Framework.Code
{
    public class ApiQuery : IApiQuery
    {
        public ApiQuery() {
            filters = new List<RepositoryFilter>();
        }
        //
        // Summary:
        //     Lista di filtri da applicare alla query
        public List<RepositoryFilter> filters { get; set; }
        //
        // Summary:
        //     Numero di record da saltare
        public int Skip { get; set; }
        //
        // Summary:
        //     Numero di record richiesti
        public int Take { get; set; }
        //
        // Summary:
        //     Ordinamento dei risultati [property] [order]
        //     Ex: "name asc"
        public string Sorting { get; set; }
    }
}
