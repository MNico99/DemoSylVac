namespace Tenaris.AutoAr.Sylvac.App.Metter.View
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using Steema.TeeChart.WPF.Drawing;
    using Steema.TeeChart.WPF.Styles;
    using Steema.TeeChart.WPF.Tools;
    using Shape = Steema.TeeChart.WPF.Styles.Shape;
    using Steema.TeeChart.WPF;

    using Tenaris.AutoAr.Sylvac.Library.Metter.Model;

    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class MetterGaugeView
    {
        /// <summary>
        /// The series source property.
        /// </summary>
        public static readonly DependencyProperty SeriesSourceProperty = DependencyProperty.Register(
          "SeriesSource", typeof(object), typeof(MetterGaugeView), new PropertyMetadata(OnChangeSeriesSource));

        /// <summary>
        /// Initializes a new instance of the <see cref="AmplitudeChart"/> class.
        /// </summary>
        public MetterGaugeView()
        {
            this.InitializeComponent();
            this.InitializeChart();
        }

        /// <summary>
        /// Gets or sets SeriesSource.
        /// </summary>
        public object SeriesSource
        {
            get
            {
                return this.GetValue(SeriesSourceProperty);
            }

            set
            {
                this.SetValue(SeriesSourceProperty, value);
            }
        }

        /// <summary>
        /// The on change series source.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="args">
        /// The args...
        /// </param>
        private static void OnChangeSeriesSource(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var target = property as MetterGaugeView;
            if (target != null)
            {
                target.SetSeriesSource(args.NewValue);
            }
        }

        /// <summary>
        /// The initialize chart.
        /// </summary>
        private void InitializeChart()
        {
            this.ChartControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.ChartControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            this.ChartControl.Header.Visible = false;
            this.ChartControl.Legend.Visible = false;
            this.ChartControl.Chart.Title.Visible = false;
            this.ChartControl.Aspect.View3D = false;
            this.ChartControl.Walls.Back.Transparency = 100;
            this.ChartControl.Axes.Left.Increment = 1;
            var series = new CircularGauge { Title = "Values", RedLineStartValue = 10, RedLineEndValue = 15, GreenLineStartValue = 0, GreenLineEndValue = 5 };
            series.ShowInLegend = false;
            series.Minimum = 0;
            series.Maximum = 15;
            series.Clear();
            series.Value = 0;
            this.ChartControl.Series.Add(series);
        }

        /// <summary>
        /// The set series source.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void SetSeriesSource(object value)
        {
            var values = (IList<MetterValue>)value;
            if (values != null && values.Count() > 0)
            {
                var series = this.ChartControl.Series.WithTitle("Values");
                var item = values.Last();
                ((CircularGauge)series).Value = item.Value > 0 ? item.Value > 15 ? 15 : item.Value : 0;
            }
        }
    }
}