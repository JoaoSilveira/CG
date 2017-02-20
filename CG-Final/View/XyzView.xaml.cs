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
using CG_Final.Commands;

namespace CG_Final.View
{
    /// <summary>
    /// Interaction logic for XyzView.xaml
    /// </summary>
    public partial class XyzView : UserControl
    {
        public XyzView()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var cmd = DataContext as IObjectCommand;

            cmd?.Apply();
        }
    }
}
