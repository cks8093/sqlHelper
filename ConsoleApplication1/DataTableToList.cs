using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ConsoleApplication1
{
   public class DataTableToList
    {
       DataTable dt = new DataTable();

       public DataTableToList()
       {           
           dt.Columns.Add("purtype", typeof(string));
           dt.Columns.Add("purtype1", typeof(string));
           dt.Columns.Add("expireDate", typeof(DateTime));
           dt.Columns.Add("price", typeof(int));

           DataRow dr;
           for (int i = 0; i < 100; i++)
           {
               dr = dt.NewRow();
               dr["purtype"] = "purtype" + i;
               dr["purtype1"] = DBNull.Value;
               dr["expireDate"] = DateTime.Now;
               dr["price"] = i;
               dt.Rows.Add(dr);
           }
       }

       public void SingleTest()
       {
           test t = ConvertSingleTo<test>(dt);
       }

       public void ListTest()
       {
           List<test> t = ConvertTo<test>(dt);
       }

       public void ListFilterTest()
       {
           Func<FieldInfo, DataRow, string> act2 = (fi, item) =>
           {
               if (fi.Name.ToLower().Equals("purtype1"))
               {
                   return (int)item["price"] % 2 == 0 ? "짝수" : "홀수";
               }
               else
               {
                   return item[fi.Name].ToString();
               }
           };

           List<test> t = ConvertTo<test>(dt, act2);
       }


        public T ConvertSingleTo<T>(DataTable datatable, Func<FieldInfo, DataRow, string> filter = null) where T : new()
        {
            T Temp;
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);

                if (datatable.Rows.Count > 0)
                {
                    return getObject<T>(datatable.Rows[0], columnsNames, filter);
                }
            }
            catch
            {

            }
            finally { Temp = default(T); }

            return Temp;

        }

        public List<T> ConvertTo<T>(DataTable datatable, Func<FieldInfo, DataRow, string> filter = null) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);

                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames, filter));
                return Temp;
            }
            catch
            {
                return Temp;
            }

        }

        public T getObject<T>(DataRow row, List<string> columnsName, Func<FieldInfo, DataRow, string> filter) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                FieldInfo[] Properties = typeof(T).GetFields();

                foreach (FieldInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        if (filter == null)
                        {
                            value = row[columnname].ToString();
                        }
                        else
                        {
                            value = filter(objProperty, row);
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.FieldType) != null)
                            {
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.FieldType).ToString())));
                            }
                            else
                            {
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.FieldType.ToString())));
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
    }
}
