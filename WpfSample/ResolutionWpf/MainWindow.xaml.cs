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

namespace ResolutionWpf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty MouseOnWindowProperty =
            DependencyProperty.Register("MouseOnWindow", typeof(Point), typeof(MainWindow), new PropertyMetadata(new Point()));

        public static readonly DependencyProperty MouseOnScreenProperty =
            DependencyProperty.Register("MouseOnScreen", typeof(Point), typeof(MainWindow), new PropertyMetadata(new Point()));

        public Point MouseOnWindow
        {
            get { return (Point)GetValue(MouseOnWindowProperty); }
            private set { SetValue(MouseOnWindowProperty, value); }
        }

        public Point MouseOnScreen
        {
            get { return (Point)GetValue(MouseOnScreenProperty); }
            private set { SetValue(MouseOnScreenProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();

            // WindowStyle="None" のときにウィンドウを最大化すると、左上のスクリーン座標は (-8, -8) となります (枠の幅が 8)。
            // さらに AllowsTransparency="True" の場合、(-8, -8) からウィンドウが描画されます。
            MouseMove += (o, e) =>
            {
                MouseOnWindow = e.GetPosition(this);
                MouseOnScreen = PointToScreen(MouseOnWindow);
            };
        }
    }
}
