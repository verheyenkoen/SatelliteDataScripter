using System;
using System.Data;
using System.Globalization;

namespace SatelliteDataScripter
{
    public class Column
    {
        public string Name { get; set; }

        public string DataType { get; set; }

        public string EscapedName
        {
            get { return string.Format("[{0}]", Name); }
        }

        public bool IsIdColumn
        {
            get { return Name.ToLower() == "id"; }
        }

        public bool Generate { get; set; }

        public Column(TablesDataSet.ColumnRow row)
        {
            Name = row.Column_Name;
            DataType = row.Data_Type;

            Generate = true;
        }

        public string GetValue(DataRow row)
        {
            var value = row[Name];

            if (value == DBNull.Value)
            {
                return "NULL";
            }

            switch (DataType.ToLower())
            {
                case "int":
                case "bigint":
                case "smallint":
                    return value.ToString();

                case "char":
                case "varchar":
                case "uniqueidentifier":
                    return string.Format("'{0}'", Convert.ToString(value).Replace("'", "''"));

                case "nvarchar":
                case "nchar":
                    return string.Format("N'{0}'", Convert.ToString(value).Replace("'", "''"));

                case "datetime":
                case "smalldatetime":
                    return string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);

                case "bit":
                    return (bool)value ? "1" : "0";

                case "decimal":
                    return Convert.ToString(value, CultureInfo.GetCultureInfo("en-US"));

                default:
                    throw new Exception(string.Format("The data type '{0}' is not supported for generation.", DataType));
            }
        }
    }
}