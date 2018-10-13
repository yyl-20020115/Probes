using System.Windows.Media.Media3D;

namespace Probes
{
    public abstract class NineAxesMeasurementNetControl : MeasurementBaseNetControl, INineAxesMeasurementNetControl
    {
        public abstract AxisType AxisType { get; }
        protected abstract AxisDisplayControl Display { get; }
        protected override int LineGroupLength => 3;
        public virtual void OnReceiveData(Vector3D data)
        {
            this.Display.AddData(data);
        }
        protected virtual void EnableDisplay_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Display.Visibility = System.Windows.Visibility.Visible;
        }

        protected virtual void EnableDisplay_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Display.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
