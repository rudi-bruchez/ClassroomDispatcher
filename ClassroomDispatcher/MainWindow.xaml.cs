using ClassroomDispatcher.Dispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClassroomDispatcher
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _version = "1.3";
        private ViewModel.Main _vm;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Classroom Dispatcher v." + _version;
            _vm = new ViewModel.Main();
            DataContext = _vm;

            foreach (var a in _vm.GetActions())
            {
                lvActions.Items.Add(new ListViewItem() { Name = a.ClassName, Content = a.Label });
            }
        }

        private void btnDispatch_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem lvi = (lvActions.SelectedItem as ListViewItem);
            if (lvi == null)
            {
                MessageBox.Show("Vous devez sélectionner une opération", "Sélection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _vm.Dispatch(lvi.Name);
        }
    }
}
