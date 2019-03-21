using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace UniconGS.UI.Spinner
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class Spinner : UserControl
    {
        public Spinner()
        {
            InitializeComponent();

            // Comment the following...
            KeyTime time = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            int numFrames = canvas.Children.Count;

            frameAnim.Duration = new Duration(TimeSpan.FromSeconds(numFrames));

            for (int i = 0; i < numFrames; i++)
            {
                Frame f = new Frame();
                frameAnim.Frames.Add(f);

                f.KeyTime = time;
                time = KeyTime.FromTimeSpan(time.TimeSpan + TimeSpan.FromSeconds(1)); // one frame per second (we can speed this up with SpeedRatio)

                // Line size:
                f.Add(new Setter(Line.Y2Property, "-14", ((Line)canvas.Children[(i + numFrames - 1) % numFrames]).Name));
                f.Add(new Setter(Line.Y2Property, "-16", ((Line)canvas.Children[(i) % numFrames]).Name));
                f.Add(new Setter(Line.Y2Property, "-14", ((Line)canvas.Children[(i + 1) % numFrames]).Name));

                // Line opacity:
                f.Add(new Setter(Line.OpacityProperty, (0.2).ToString(), ((Line)canvas.Children[(i + numFrames - 4) % numFrames]).Name));
                f.Add(new Setter(Line.OpacityProperty, (1).ToString(), ((Line)canvas.Children[(i) % numFrames]).Name));
                f.Add(new Setter(Line.OpacityProperty, (0.2).ToString(), ((Line)canvas.Children[(i + 1) % numFrames]).Name));
            }
            // ...to here when switching to manual (XAML-based) declaration of frames.

            frameAnim.Render();
        }
    }
}
