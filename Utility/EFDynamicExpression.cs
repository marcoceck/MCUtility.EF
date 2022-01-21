using MCUtility.EF.Models;
using MCUtility.EF.Models.Concrete;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MCUtility.EF.Utility
{
    public static class EFDynamicExpression
    {
        public const string MATCH_MODE_CONTAINS = "CONTAINS";
        public const string MATCH_MODE_STARTWITH = "STARTWITH";
        public const string MATCH_MODE_EQUALS = "EQUALS";

        public static string BuildLinqDynamicExpressionFilterString(IRepositoryFilter filter)
        {
            if (String.IsNullOrWhiteSpace(filter.MatchMode))
            {
                filter.MatchMode = "CONTAINS";
            }

            string dynamicExpressionFilter = "";

            switch (filter.MatchMode)
            {
                case MATCH_MODE_STARTWITH:
                    dynamicExpressionFilter = String.Format("it.{0}.StartsWith(@0)", filter.FieldName);
                    break;
                case MATCH_MODE_EQUALS:
                    dynamicExpressionFilter = String.Format("it.{0} == @0", filter.FieldName);
                    break;
                case MATCH_MODE_CONTAINS:
                    dynamicExpressionFilter = String.Format("it.{0}.Contains(@0)", filter.FieldName);
                    break;
                default:
                    dynamicExpressionFilter = String.Format("it.{0}.Contains(@0)", filter.FieldName);
                    break;
            }
            //TODO: validare espressione 
            return dynamicExpressionFilter;
        }

        public static FilterModel BuildLinqDynamicExpressionFilterModel(IRepositoryFilter filter)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            if (string.IsNullOrEmpty(filter.FieldName))
            {
                //throw new ApiException(HttpStatusCode.BadRequest, "INVALID_FILTER", "filter.FieldName", $"Invalid FieldName Value : {filter.FieldName}", null);
            }

            FilterModel result = new FilterModel();

            string dynamicExpressionFilter = "";

            //** Enumeratore **//
            //NUMBER = 1,
            //BOOLEAN = 2,
            //TEXT = 3,
            //DATE = 4,
            //MULTI = 5,
            //SELECT = 6,
            //RANGE_NUMBER = 7,
            //RANGE_NUMBER_INT = 8

            switch (filter.FieldType)
            {
                case "TEXT":
                    //dynamicExpressionFilter = String.Format("it.{0}.StartsWith(@0)", filter.FieldName);
                    string[] lst = filter.FieldName.Split(',');

                    for (int j = 0; j < lst.Length; j++)
                    {
                        string item = lst[j].Replace(" ", string.Empty);
                        if (j == 0)
                            dynamicExpressionFilter = String.Format("it.{0}.ToUpper().Contains(\"" + filter.FieldValue.ToUpper() + "\")", item);
                        else
                            dynamicExpressionFilter += " OR " + String.Format("it.{0}.ToUpper().Contains(\"" + filter.FieldValue.ToUpper() + "\")", item);
                    }

                    break;
                case "MULTI_NUMBER_INT_NOT_IN":

                    if (filter.FieldValues != null && filter.FieldValues.Count > 0)
                    {
                        if (string.IsNullOrEmpty(dynamicExpressionFilter))
                        {
                            dynamicExpressionFilter += " ( ";
                        }
                        else
                        {
                            dynamicExpressionFilter += " OR ( ";
                        }

                        for (int i = 0; i < filter.FieldValues.Count; i++)
                        {
                            if (i == 0)
                                dynamicExpressionFilter = String.Format("it.{0} != {1}", filter.FieldName, filter.FieldValues[i].ToString());
                            else
                                dynamicExpressionFilter += String.Format(" And it.{0} != {1}", filter.FieldName, filter.FieldValues[i].ToString());
                        }
                    }

                    break;
                case "MULTI_MULTI_TEXT":
                    string[] lstMM = filter.FieldName.Split(',');
                    for (int j = 0; j < lstMM.Length; j++)
                    {
                        string item = lstMM[j].Replace(" ", string.Empty);

                        if (filter.FieldValues != null && filter.FieldValues.Count > 0)
                        {
                            if (string.IsNullOrEmpty(dynamicExpressionFilter))
                            {
                                dynamicExpressionFilter += " ( ";
                            }
                            else
                            {
                                dynamicExpressionFilter += " OR ( ";
                            }

                            for (int i = 0; i < filter.FieldValues.Count; i++)
                            {
                                //** Text **//
                                if (i == 0)
                                    dynamicExpressionFilter += String.Format("it.{0}.ToUpper().Contains(\"" + filter.FieldValues[i].ToString().ToUpper() + "\")", item);
                                else
                                    dynamicExpressionFilter += " AND " + String.Format("it.{0}.ToUpper().Contains(\"" + filter.FieldValues[i].ToString().ToUpper() + "\")", item);

                            }

                            dynamicExpressionFilter += " ) ";

                        }

                    }
                    break;

                case "BOOLEAN":
                    dynamicExpressionFilter = String.Format("it.{0} == " + bool.Parse(filter.FieldValue), filter.FieldName);
                    break;
                case "DATE_ISNULL":
                    dynamicExpressionFilter = $"it.{filter.FieldName} == Null";
                    break;
                case "DATE_NOTISNULL":
                    dynamicExpressionFilter = $"it.{filter.FieldName} != Null";
                    break;
                case "DATE_GREATER":
                    DateTime? dtRif = ParseDateMinValue(filter);
                    string dateRifString = dtRif.HasValue ?
                        dateRifString = $"it.{filter.FieldName} >= DateTime({dtRif.Value.Year},{dtRif.Value.Month},{dtRif.Value.Day})"
                        : string.Empty;
                    break;
                case "DATE":
                    DateTime? dtMin = ParseDateMinValue(filter);
                    string dateMinString = dtMin.HasValue ?
                        dateMinString = $"it.{filter.FieldName} >= DateTime({dtMin.Value.Year},{dtMin.Value.Month},{dtMin.Value.Day})"
                        : string.Empty;

                    DateTime? dtMax = ParseDateValueMax(filter);
                    string dateMaxString = string.Empty;
                    if (dtMax.HasValue)
                    {
                        dtMax = dtMax.Value.AddDays(1); //workaround per includere la date di fine intervallo fino alle 23:59:59
                        dateMaxString = $"it.{ filter.FieldName} < DateTime({ dtMax.Value.Year},{ dtMax.Value.Month},{ dtMax.Value.Day})";
                    }

                    if (dtMin.HasValue && dtMax.HasValue)
                    {
                        dynamicExpressionFilter = $"{dateMinString} and {dateMaxString}";
                    }
                    else
                    {
                        if (dtMin.HasValue)
                            dynamicExpressionFilter = dateMinString;

                        if (dtMax.HasValue)
                            dynamicExpressionFilter = dateMaxString;
                    }
                    break;
                case string r when (r == "RANGE_NUMBER_INT" || r == "RANGE_NUMBER"):
                    var rMinOk = decimal.TryParse(filter.FieldValueMin, NumberStyles.AllowDecimalPoint, nfi, out var rMin);
                    var rMaxOk = decimal.TryParse(filter.FieldValueMax, NumberStyles.AllowDecimalPoint, nfi, out var rMax);
                    if (rMinOk && rMaxOk)
                        dynamicExpressionFilter = String.Format("it.{0} >= {1} and it.{0} <= {2}", filter.FieldName, rMin.ToString(CultureInfo.InvariantCulture), rMax.ToString(CultureInfo.InvariantCulture));
                    else
                    {
                        if (rMinOk)
                            dynamicExpressionFilter = String.Format("it.{0} >= {1}", filter.FieldName, rMin.ToString(CultureInfo.InvariantCulture));
                        if (rMaxOk)
                            dynamicExpressionFilter = String.Format("it.{0} <= {1}", filter.FieldName, rMax.ToString(CultureInfo.InvariantCulture));
                    }
                    break;

                //case string r when (r == "RANGE_NUMBER_INT" || r == "RANGE_NUMBER"):
                //    bool rIntMinOk = int.TryParse(filter.FieldValueMin, out int rIntMin);
                //    bool rIntMaxOk = int.TryParse(filter.FieldValueMax, out int rIntMax);
                //    if (rIntMinOk && rIntMaxOk)
                //        dynamicExpressionFilter = String.Format("it.{0} >= {1} and it.{0} <= {2}", filter.FieldName, rIntMin, rIntMax);
                //    else
                //    {
                //        if (rIntMinOk)
                //            dynamicExpressionFilter = String.Format("it.{0} >= " + rIntMin, filter.FieldName);
                //        if (rIntMaxOk)
                //            dynamicExpressionFilter = String.Format("it.{0} <= " + rIntMax, filter.FieldName);
                //    }
                //    break;
                case "NUMBER":
                case "NUMBER_INT":
                    bool rIntOk = decimal.TryParse(filter.FieldValue, NumberStyles.AllowDecimalPoint, nfi, out decimal rInt);
                    if (rIntOk)
                        dynamicExpressionFilter = String.Format("it.{0} = {1}", filter.FieldName, rInt.ToString(CultureInfo.InvariantCulture));
                    break;
                case "NUMBER_NOT_IN":
                case "NUMBER_INT_NOT_IN":
                    bool rIntOkNOTIN = decimal.TryParse(filter.FieldValue, NumberStyles.AllowDecimalPoint, nfi, out decimal rIntNOTIN);
                    if (rIntOkNOTIN)
                        dynamicExpressionFilter = String.Format("it.{0} != {1}", filter.FieldName, rIntNOTIN.ToString(CultureInfo.InvariantCulture));
                    break;
                case string r when (r == "MULTI" || r == "SELECT"): //                    "MULTI":
                    if (filter.FieldValues != null && filter.FieldValues.Count > 0)
                    {
                        for (int i = 0; i < filter.FieldValues.Count; i++)
                        {
                            Type valueType = filter.FieldValues[i].GetType();
                            //** Integer **//
                            if (valueType == typeof(Int32) || valueType == typeof(Int64) || valueType == typeof(decimal))
                            {
                                if (i == 0)
                                    dynamicExpressionFilter = String.Format("it.{0} == " + filter.FieldValues[i].ToString(), filter.FieldName);
                                else
                                    dynamicExpressionFilter += " OR " + String.Format("it.{0} == " + filter.FieldValues[i].ToString(), filter.FieldName);
                            }

                            //** Boolean **//
                            if (valueType == typeof(bool))
                            {
                                if (i == 0)
                                    dynamicExpressionFilter = String.Format("it.{0} == " + filter.FieldValues[i].ToString(), filter.FieldName);
                                else
                                    dynamicExpressionFilter += " OR " + String.Format("it.{0} == " + filter.FieldValues[i].ToString(), filter.FieldName);
                            }

                            //** Text **//
                            if (valueType == typeof(string))
                            {
                                if (r == "MULTI")
                                {
                                    if (i == 0)
                                        dynamicExpressionFilter = String.Format("it.{0} == \"" + filter.FieldValues[i].ToString() + "\"", filter.FieldName);
                                    else
                                        dynamicExpressionFilter += " OR " + String.Format("it.{0} == \"" + filter.FieldValues[i].ToString() + "\"", filter.FieldName);
                                }

                                if (r == "SELECT")
                                    dynamicExpressionFilter = String.Format("it.{0}.Contains(" + string.Join(",", filter.FieldValues) + ") ", filter.FieldName);
                            }
                        }

                    }

                    break;
                default:
                    dynamicExpressionFilter = String.Format("it.{0}.Contains(@0)", filter.FieldName);
                    break;
            }

            result.DynamicExpressionFilter = dynamicExpressionFilter;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static DateTime? ParseDateValueMax(IRepositoryFilter filter)
        {
            DateTime? dtMax = null;
            if (!string.IsNullOrWhiteSpace(filter.FieldValueMax))
            {
                try
                {
                    dtMax = DateTime.Parse(filter.FieldValueMax);
                }
                catch (Exception ex)
                {
                    //throw new ApiException(HttpStatusCode.BadRequest, "INVALID_FILTER", $"{filter.FieldName}.FieldValueMax", $"Invalid date filter format {filter.FieldValueMax}", ex);
                }

            }
            return dtMax;
        }

        /// <summary>
        /// /Parse and validate Date value in filter.FieldValueMin
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static DateTime? ParseDateMinValue(IRepositoryFilter filter)
        {
            DateTime? dtMin = null;
            if (!String.IsNullOrWhiteSpace(filter.FieldValueMin))
            {
                try
                {
                    dtMin = DateTime.Parse(filter.FieldValueMin);
                }
                catch (Exception ex)
                {
                    //throw new ApiException(HttpStatusCode.BadRequest, "INVALID_FILTER", $"{filter.FieldName}.FieldValueMin", $"Invalid date filter format {filter.FieldValueMin}", ex);
                }

            }

            return dtMin;
        }
    }
}
