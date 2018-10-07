using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Arkanoid
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        DispatcherTimer _timer = new DispatcherTimer();
        int _shift;

        readonly Dictionary<int, string> _colors = new Dictionary<int, string>
                     {
                         {0, "BrickRed"},
                         {1, "BrickGreen"},
                         {2, "BrickBlue"},
                         {3, "BrickYellow"},
                         {4, "BrickCyan"},
                         {5, "BrickMagenta"}
                     };

        public MenuWindow()
        {
            InitializeComponent();

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += PaintTimer;
            PaintName();
            _timer.Start();
        }

        void PaintTimer(object sender, EventArgs e)
        {
             PaintName();
        }

        void PaintName()
        {
            _shift++;

            for (var i = 0; i < canvasStart.Children.Count; i++)
            {
                if (canvasStart.Children[i].GetType() != typeof(Rectangle)) return;
                
                var rect = (Rectangle)canvasStart.Children[i];
                var leftPos = Canvas.GetLeft(rect);
                var color = GetColor(leftPos);
                var coloredBrick = (Rectangle)FindResource(_colors[color]);
                rect.Fill = coloredBrick.Fill;
                rect.Stroke = coloredBrick.Stroke;
            }
            
        }

        int GetColor(double pos)
        {
            var left = pos;

            var color = 0;

            if (_shift > 5) _shift = 0;

            switch (left)
            {
                case double mrg when (mrg <= 150):
                    color = GetShift(0);
                    break;
                case double mrg when (mrg >= 220 && mrg <= 360):
                    color = GetShift(1);
                    break;
                case double mrg when (mrg >= 430 && mrg <= 570):
                    color = GetShift(2);
                    break;
                case double mrg when (mrg >= 640 && mrg <= 780):
                    color = GetShift(3);
                    break;
                case double mrg when (mrg >= 850 && mrg <= 990):
                    color = GetShift(4);
                    break;
                case double mrg when (mrg >= 1060):
                    color = GetShift(5);
                    break;
            }

            int GetShift(int clr)
            {
                var c = (clr + _shift > 5)? clr + _shift - 6: clr + _shift;
                return c;
            }

            return color;
        }

        void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Level1();
        }

        void Level1()
        {
            var level1 = new GameWindow {Owner = GetWindow(this)};
            _timer.Stop();
            Visibility = Visibility.Hidden;
            level1.ShowDialog();
        }
    }
}