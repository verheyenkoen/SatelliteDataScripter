using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace SatelliteDataScripter
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event RoutedEventHandler OptionsChanged = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ObservableCollection<Connection> connections;
        private Connection selectedConnection;
        private Schema selectedSchema;
        private Table selectedTable;
        private bool generateUpdateStatements = true;
        private bool updateOnlyIfDataIsChanged = true;
        private bool isUpdateOnlyIfDataIsChangedCheckboxEnabled = true;

        public ObservableCollection<Connection> Connections
        {
            get { return connections; }
            private set
            {
                connections = value;
                NotifyPropertyChanged("Connections");
            }
        }

        public Connection SelectedConnection
        {
            get { return selectedConnection; }
            set
            {
                selectedConnection = value;
                NotifyPropertyChanged("SelectedConnection");
            }
        }

        public Schema SelectedSchema
        {
            get { return selectedSchema; }
            set
            {
                selectedSchema = value;
                NotifyPropertyChanged("SelectedSchema");
            }
        }

        public Table SelectedTable
        {
            get { return selectedTable; }
            set
            {
                selectedTable = value;
                NotifyPropertyChanged("SelectedTable");
                NotifyPropertyChanged("IsTableSelected");
            }
        }

        public bool IsTableSelected
        {
            get { return SelectedTable != null; }
        }

        public bool GenerateUpdateStatements
        {
            get { return generateUpdateStatements; }
            set
            {
                generateUpdateStatements = value;
                NotifyPropertyChanged("GenerateUpdateStatements");

                IsUpdateOnlyIfDataIsChangedCheckboxEnabled = value;

                if (!value)
                {
                    UpdateOnlyIfDataIsChanged = false;
                }

                OptionsChanged(this, new RoutedEventArgs());
            }
        }

        public bool IsUpdateOnlyIfDataIsChangedCheckboxEnabled
        {
            get { return isUpdateOnlyIfDataIsChangedCheckboxEnabled; }
            set
            {
                isUpdateOnlyIfDataIsChangedCheckboxEnabled = value;
                NotifyPropertyChanged("IsUpdateOnlyIfDataIsChangedCheckboxEnabled");
            }
        }

        public bool UpdateOnlyIfDataIsChanged
        {
            get { return updateOnlyIfDataIsChanged; }
            set
            {
                updateOnlyIfDataIsChanged = value;
                NotifyPropertyChanged("UpdateOnlyIfDataIsChanged");
                OptionsChanged(this, new RoutedEventArgs());
            }
        }

        public MainWindowModel()
        {
            Connections = new ObservableCollection<Connection>(GetConnections());
            SelectedConnection = Connections.FirstOrDefault();
        }

        private IEnumerable<Connection> GetConnections()
        {
            return ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().Select(cs => new Connection(this, cs));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}