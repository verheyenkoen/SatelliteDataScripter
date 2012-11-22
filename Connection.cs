using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace SatelliteDataScripter
{
    public class Connection
    {
        private IEnumerable<Schema> schemas;

        public MainWindowModel Model { get; set; }

        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public IEnumerable<Schema> Schemas
        {
            get
            {
                if (schemas == null)
                {
                    schemas = TryLoadSchemas();
                }

                return schemas;
            }
        }

        public Connection(MainWindowModel model, ConnectionStringSettings connectionString)
        {
            Name = connectionString.Name;
            ConnectionString = connectionString.ConnectionString;
            Model = model;
        }

        public override string ToString()
        {
            return Name;
        }

        private IEnumerable<Schema> TryLoadSchemas()
        {
            try
            {
                return LoadSchemas().ToList();
            }
            catch (SqlException)
            {
                MessageBox.Show(string.Format("Error connecting to '{0}'. Please check the connection string in {1}.exe.config.", Name, Assembly.GetExecutingAssembly().GetName().Name));

                return new List<Schema>();
            }
        }

        private IEnumerable<Schema> LoadSchemas()
        {
            using (var adapter = new SqlDataAdapter("select distinct TABLE_SCHEMA from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE' order by 1", ConnectionString))
            {
                var dataTable = new TablesDataSet.SchemaDataTable();

                adapter.Fill(dataTable);

                foreach (TablesDataSet.SchemaRow row in dataTable.Rows)
                {
                    yield return new Schema(this, row);
                }
            }
        }
    }
}