using System;
using System.ComponentModel;
using System.Windows;

namespace SatelliteDataScripter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel Model { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = Model = new MainWindowModel();
                Model.OptionsChanged += MainWindowModel_OnOptionsChanged;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectionsComboBox.Focus();
        }

        private void MainWindowModel_OnOptionsChanged(object sender, RoutedEventArgs e)
        {
            UpdateScript(true);
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateScript(false);
        }

        private void UpdateScript(bool silent)
        {
            if (Model.SelectedTable != null)
            {
                ScriptTextBox.Text = GenerateScript(silent);
                ScriptTextBox.SelectAll();
            }
            else
            {
                ScriptTextBox.Text = string.Empty;
            }
        }

        private string GenerateScript(bool silent)
        {
            try
            {
                var script = Model.SelectedTable.ScriptData();

                Clipboard.SetText(script);

                return script;
            }
            catch (Exception exception)
            {
                if (!silent)
                {
                    MessageBox.Show(this, exception.Message, exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return string.Empty;
            }
        }
    }
}
