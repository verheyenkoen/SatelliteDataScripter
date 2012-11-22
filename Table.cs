using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SatelliteDataScripter
{
    public class Table
    {
        public Schema Schema { get; private set; }

        public string Name { get; set; }

        protected string FullName
        {
            get { return string.Format("{0}.{1}", Schema.Name, Name); }
        }

        protected string EscapedFullName
        {
            get { return string.Format("[{0}].[{1}]", Schema.Name, Name); }
        }

        public IEnumerable<Column> Columns { get; private set; }

        public IEnumerable<Column> NonIdColumns
        {
            get { return Columns.Where(g => !g.IsIdColumn); }
        }

        public IEnumerable<Column> GeneratedColumns
        {
            get { return Columns.Where(g => g.Generate); }
        }

        public Table(Schema schema, TablesDataSet.TableRow row)
        {
            Schema = schema;

            Name = row.Table_Name;

            Columns = LoadColumns();
        }

        public override string ToString()
        {
            return Name;
        }

        public string ScriptData()
        {
            if (Columns.Count(c => c.IsIdColumn) == 0)
            {
                throw new Exception("The table does not contain a column with name Id which is required.");
            }

            var sb = new StringBuilder();
            sb.Append("-- ");
            sb.AppendLine(FullName);

            foreach (DataRow record in GetTableData().Rows)
            {
                sb.AppendFormat("IF NOT EXISTS(SELECT 1 FROM {0} WHERE Id = {1})", EscapedFullName, GetIdValue(record));
                sb.AppendLine();

                GenerateInsertStatement(sb, record);

                if (Schema.Connection.Model.GenerateUpdateStatements)
                {
                    GenerateUpdateStatement(sb, record);
                }
            }

            return sb.ToString().Trim();
        }

        private void GenerateInsertStatement(StringBuilder sb, DataRow record)
        {
            sb.Append("   INSERT INTO ");
            sb.Append(EscapedFullName);
            sb.Append(" (");
            sb.Append(string.Join(", ", GeneratedColumns.Select(c => c.EscapedName).ToArray()));
            sb.Append(") VALUES (");
            sb.Append(string.Join(", ", GeneratedColumns.Select(c => c.GetValue(record)).ToArray()));
            sb.AppendLine(")");
        }

        private void GenerateUpdateStatement(StringBuilder sb, DataRow record)
        {
            sb.Append("   ELSE UPDATE ");
            sb.Append(EscapedFullName);
            sb.Append(" SET ");
            sb.Append(string.Join(", ", GeneratedColumns.Where(c => !c.IsIdColumn).Select(c => string.Format("{0} = {1}", c.EscapedName, c.GetValue(record))).ToArray()));
            sb.Append(" WHERE Id = ");
            sb.Append(GetIdValue(record));

            if (Schema.Connection.Model.UpdateOnlyIfDataIsChanged)
            {
                sb.Append(" AND (");
                sb.Append(string.Join(" OR ", GeneratedColumns.Where(c => !c.IsIdColumn).Select(c => string.Format("{0} {1} {2}", c.EscapedName, c.GetValue(record) == "NULL" ? "IS NOT" : "<>", c.GetValue(record))).ToArray()));
                sb.Append(")");
            }

            sb.AppendLine();
        }

        private string GetIdValue(DataRow record)
        {
            return Columns.Single(c => c.IsIdColumn).GetValue(record);
        }

        private DataTable GetTableData()
        {
            using (var adapter = new SqlDataAdapter("select * from " + EscapedFullName + " order by Id", Schema.Connection.ConnectionString))
            {
                var dataTable = new DataTable();

                adapter.Fill(dataTable);

                return dataTable;
            }
        }

        private IEnumerable<Column> LoadColumns()
        {
            using (var adapter = new SqlDataAdapter("select COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = @schema and TABLE_NAME = @table order by ORDINAL_POSITION", Schema.Connection.ConnectionString))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@schema", Schema.Name);
                adapter.SelectCommand.Parameters.AddWithValue("@table", Name);

                var dataTable = new TablesDataSet.ColumnDataTable();

                adapter.Fill(dataTable);

                return dataTable.Rows.Cast<TablesDataSet.ColumnRow>().Select(r => new Column(r)).ToList();
            }
        }
    }
}