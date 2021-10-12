// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmplitudeChart.xaml.cs" company="Tenaris S.A">
//   Tenaris S.A
// </copyright>
// <summary>
//   Interaction logic for Chart.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    public partial class MetterChartView
    {
        private const double cMaxXValue = 30;
        private double maxXValue = cMaxXValue;

        /// <summary>
        /// The series source property.
        /// </summary>
        public static readonly DependencyProperty SeriesSourceProperty = DependencyProperty.Register(
          "SeriesSource", typeof(object), typeof(MetterChartView), new PropertyMetadata(OnChangeSeriesSource));

        /// <summary>
        /// The threshold property.
        /// </summary>
        public static readonly DependencyProperty ThresholdMinProperty = DependencyProperty.Register(
          "ThresholdMin", typeof(double), typeof(MetterChartView), new PropertyMetadata(OnChangeThresholdMin));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ThresholdMaxProperty = DependencyProperty.Register(
          "ThresholdMax", typeof(double), typeof(MetterChartView), new PropertyMetadata(OnChangeThresholdMax));

        /// <summary>
        /// The title property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
          "Title", typeof(string), typeof(MetterChartView), new PropertyMetadata(OnChangeTitle));

        /// <summary>
        /// Flaws Property
        /// </summary>
        public static readonly DependencyProperty FlawsProperty = DependencyProperty.Register(
          "FlawsValues", typeof(object), typeof(MetterChartView), new PropertyMetadata(OnChangeFlaws));

        /// <summary>
        /// AxialOffset Property
        /// </summary>
        public static readonly DependencyProperty AxialOffsetProperty = DependencyProperty.Register(
          "AxialOffset", typeof(object), typeof(MetterChartView), new PropertyMetadata(OnChangeAxialOffset));

        /// <summary>
        /// Length Property
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
          "TubeLength", typeof(object), typeof(MetterChartView), new PropertyMetadata(OnChangeLength));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsInInspectionProperty = DependencyProperty.Register(
          "IsInInspection", typeof(object), typeof(MetterChartView), new PropertyMetadata(OnChangeIsInInspection));

        /// <summary>
        /// Initializes a new instance of the <see cref="AmplitudeChart"/> class.
        /// </summary>
        public MetterChartView()
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
        /// Gets or sets Flaws.
        /// </summary>
        public object FlawsValues
        {
            get
            {
                return this.GetValue(FlawsProperty);
            }

            set
            {
                this.SetValue(FlawsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Threshold.
        /// </summary>
        public double ThresholdMin
        {
            get
            {
                return (double)this.GetValue(ThresholdMinProperty);
            }

            set
            {
                this.SetValue(ThresholdMinProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ThresholdMax
        {
            get
            {
                return (double)this.GetValue(ThresholdMaxProperty);
            }

            set
            {
                this.SetValue(ThresholdMaxProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public object Title
        {
            get
            {
                return this.GetValue(TitleProperty);
            }

            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets AxialOffset.
        /// </summary>
        public object AxialOffset
        {
            get
            {
                return this.GetValue(AxialOffsetProperty);
            }

            set
            {
                this.SetValue(AxialOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Length.
        /// </summary>
        public object TubeLength
        {
            get
            {
                return this.GetValue(LengthProperty);
            }

            set
            {
                this.SetValue(LengthProperty, value);
            }
        }

        public object IsInInspection
        {
            get
            {
                return this.GetValue(IsInInspectionProperty);
            }

            set
            {
                this.SetValue(IsInInspectionProperty, value);
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
            var target = property as MetterChartView;
            if (target != null && ((IList<MetterValue>)args.NewValue).Count() > 0)
            {
                target.SetSeriesSource(args.NewValue);
            }
        }

        /// <summary>
        /// The Change Flaws
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="args">
        /// The args...
        /// </param>
        private static void OnChangeFlaws(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var target = property as MetterChartView;
            if (target != null)
            {
            }
        }

        /// <summary>
        /// OnChange Axial Offset
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="args">
        /// The args...
        /// </param>
        private static void OnChangeAxialOffset(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var target = property as MetterChartView;
            if (target != null)
            {
            }
        }

        /// <summary>
        /// OnChange InspectionEnd
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="args">
        /// The args...
        /// </param>
        private static void OnChangeLength(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var target = property as MetterChartView;
            if (target != null)
            {
            }
        }

        /// <summary>
        /// The on change threshold.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="args">
        /// The args..
        /// </param>
        private static void OnChangeThresholdMin(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var target = property as MetterChartView;
            if (target != null)
            {
                target.SetThresholdMin(args.NewValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="args"></param>
        private static void OnChangeThresholdMax(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var target = property as MetterChartView;
            if (target != null)
            {
                target.SetThresholdMax(args.NewValue);
            }
        }

        /// <summary>
        /// The on change title.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="args">
        /// The args..
        /// </param>
        private static void OnChangeTitle(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var target = property as MetterChartView;
            if (target != null)
            {
                target.SetTitle(args.NewValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="args"></param>
        private static void OnChangeIsInInspection(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var target = property as MetterChartView;
            if (target != null)
            {
                target.SetIsInInspection(args.NewValue);
            }
        }

        /// <summary>
        /// The initialize chart.
        /// </summary>
        private void InitializeChart()
        {
            this.ChartControl.Aspect.View3D = false;
            this.ChartControl.Header.Visible = false;
            this.ChartControl.Legend.Visible = true;

            this.ChartControl.Panel.Bevel.Outer = BevelStyles.None;
            this.ChartControl.Panel.Bevel.Inner = BevelStyles.None;
            this.ChartControl.Panel.Brush.Gradient.Visible = false;

            this.ChartControl.Walls.Back.Bevel.Inner = BevelStyles.None;
            this.ChartControl.Walls.Back.Bevel.Outer = BevelStyles.None;

            this.ChartControl.Axes.Left.Visible = true;
            this.ChartControl.Axes.Left.Title.Text = "Amp.";
            this.ChartControl.Axes.Left.AxisPen.Width = 1;
            this.ChartControl.Axes.Left.Labels.Visible = true;
            this.ChartControl.Axes.Left.AutomaticMinimum = false;
            this.ChartControl.Axes.Left.AutomaticMaximum = false;
            this.ChartControl.Axes.Left.SetMinMax(-1, 15);
            this.ChartControl.Axes.Left.Labels.AutoSize = false;
            this.ChartControl.Axes.Left.Labels.TextAlign = TextAlignment.Left;
            this.ChartControl.Axes.Left.Labels.CustomSize = 30;

            this.ChartControl.Axes.Bottom.Title.Text = "T[s]";
            this.ChartControl.Axes.Bottom.Visible = true;
            this.ChartControl.Axes.Bottom.AxisPen.Width = 1;
            this.ChartControl.Axes.Bottom.Labels.Visible = true;
            this.ChartControl.Axes.Bottom.Visible = true;
            this.ChartControl.Axes.Bottom.Inverted = true;
            this.ChartControl.Axes.Bottom.SetMinMax(0, cMaxXValue);

            this.ChartControl.Panel.MarginRight = 4.5;

            var seriesForward = new Line { Color = Colors.Blue, Title = "ValuesForward" };
            seriesForward.XValues.DataMember = "Index";
            seriesForward.YValues.DataMember = "Value";
            seriesForward.ShowInLegend = false;
            seriesForward.LinePen.Width = 2;
            seriesForward.Pointer.Visible = true;
            seriesForward.Pointer.Style = PointerStyles.Circle;
            seriesForward.Pointer.Color = Colors.Blue;
            this.ChartControl.Series.Add(seriesForward);

            var thresholdMinLine = new ColorLine { Pen = { Color = Colors.Yellow }, Axis = this.ChartControl.Axes.Left, AllowDrag = false };
            this.ChartControl.Tools.Add(thresholdMinLine);

            var thresholdMaxLine = new ColorLine { Pen = { Color = Colors.Red }, Axis = this.ChartControl.Axes.Left, AllowDrag = false };
            this.ChartControl.Tools.Add(thresholdMaxLine);
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
            var seriesForward = this.ChartControl.Series.WithTitle("ValuesForward");

            var index = values.Max(p => p.Index);
            if (index > cMaxXValue)
            {
                this.ChartControl.Axes.Bottom.SetMinMax(index - cMaxXValue, index);
            }
            else
            {
                this.ChartControl.Axes.Bottom.SetMinMax(0, cMaxXValue);
            }

            seriesForward.DataSource = new System.Collections.ObjectModel.ObservableCollection<MetterValue>(values);
            this.SetThresholdMin(values.Min(p => p.Value));
            this.SetThresholdMax(values.Max(p => p.Value));
        }

        /// <summary>
        /// The set threshold.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void SetThresholdMin(object value)
        {
            if (value != null)
            {
                var thresholdMin = this.ChartControl.Tools[0] as ColorLine;
                if (thresholdMin != null)
                {
                    thresholdMin.Value = (double)value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void SetThresholdMax(object value)
        {
            if (value != null)
            {
                var thresholdMax = this.ChartControl.Tools[1] as ColorLine;
                if (thresholdMax != null)
                {
                    thresholdMax.Value = (double)value;
                }
            }
        }

        /// <summary>
        /// The set title.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void SetTitle(object value)
        {
            if (value != null)
            {
                this.ChartControl.Chart.Header.Text = (string)value;
                this.ChartControl.Chart.Header.Visible = true;
            }
        }

        private void SetIsInInspection(object value)
        {
            //if (value != null)
            //{
            //    if (!isInInspection && (bool)value)
            //    {
            //        this.InitializeChart();
            //    }
            //}
        }
    }
}