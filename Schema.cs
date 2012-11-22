using System.Collections.Generic;
using System.Data.SqlClient;

namespace SatelliteDataScripter
{
    public class Schema
    {
        private IEnumerable<Table> tables;

        public Connection Connection { get; private set; }

        public string Name { get; set; }

        public IEnumerable<Table> Tables
        {
            get
            {
                if (tables == null)
                {
                    tables = LoadTables();
                }

                return tables;
            }
        }

        public Schema(Connection connection, TablesDataSet.SchemaRow row)
        {
            Connection = connection;
            Name = row.Table_Schema;
        }

        public override string ToString()
        {
            return Name;
        }

        private IEnumerable<Table> LoadTables()
        {
            using (var adapter = new SqlDataAdapter("select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE' and TABLE_SCHEMA like @schema order by 1", Connection.ConnectionString))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@schema", Name);

                var dataTable = new TablesDataSet.TableDataTable();

                adapter.Fill(dataTable);

                foreach (TablesDataSet.TableRow row in dataTable.Rows)
                {
                    yield return new Table(this, row);
                }
            }
        }
    }
}