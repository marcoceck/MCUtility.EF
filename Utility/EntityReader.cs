using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MCUtility.EF.Utility
{
    public static class EntityReader<T>
    {

        public static List<string> ListEntityToCSV(List<T> elements, string separator)
        {
            Type t = typeof(T);
            PropertyInfo[] propInfos = t.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            List<string> lstRows = new List<string>();
            string lineHeader = string.Empty;
            string lineRow = string.Empty;

            // Aggiungo la riga di intestazione
            foreach (var item in propInfos)
            {
                lineHeader = string.Concat(lineHeader, item.Name, separator);
            }
            lstRows.Add(lineHeader);
            // Aggiungo le righe di dati
            foreach (var elem in elements)
            {
                lineRow = string.Empty;
                foreach (var item in propInfos)
                {
                    lineRow = string.Concat(lineRow, item.GetValue(elem), separator);
                }
                lstRows.Add(lineRow);
            }

            return lstRows;
        }

    }
}
