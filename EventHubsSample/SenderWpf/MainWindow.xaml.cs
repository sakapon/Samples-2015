using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

namespace SenderWpf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MouseLeftButtonDown += (o, e) => DragMove();

            var model = (AppModel)DataContext;

            Observable.FromEventPattern(this, "Loaded")
                .Merge(Observable.FromEventPattern(this, "LocationChanged"))
                .Select(_ => new Point(Math.Round(Left, MidpointRounding.AwayFromZero), Math.Round(Top, MidpointRounding.AwayFromZero)))
                .Subscribe(model.Position);
        }
    }
}
