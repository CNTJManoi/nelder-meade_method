using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Windows.Input;

namespace nelder_meade_method.ViewModels
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private PlotModel _plotModel;
        private List<DataPoint> _points;
        private string _function;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged("PlotModel"); }
        }
        public string Function
        {
            get { return _function; }
            set
            {
                if (!string.Equals(_function, value))
                {
                    _function = value;
                    OnPropertyChanged("Function");
                }
            }
        }

        public MainWindowModel()
        {
            PlotModel = new PlotModel();
            Function = "x^2+x*y+y^2-6*x-9*y";
            _points = new List<DataPoint>();
        }
        public List<DataPoint> Points { get { return _points; } private set { _points = value; } }
        #region Command

        private DelegateCommand _beginCalculate;
        public DelegateCommand BeginCalculate
        {
            get { return _beginCalculate ?? (_beginCalculate = new DelegateCommand(CalculateExecute, CanCalculateExecute)); }
        }

        private bool CanCalculateExecute(object obj)
        {
            return true;
        }

        private void CalculateExecute(object obj)
        {
            DataPoint bestVector = new DataPoint(0, 0);
            DataPoint averageVector = new DataPoint(1, 0);
            DataPoint worstVector = new DataPoint(0, 1);
            float alpha = 1, beta = 0.5f, omega;
            float best, worst, good;
            List<DataPoint> points = new List<DataPoint>();
            points.Add(new DataPoint(0, 0));
            points.Add(new DataPoint(1, 0));
            points.Add(new DataPoint(0, 1));
            ClearPoints();
            List<float> res = new List<float>();
            foreach (var point in points)
            {
                AddPoint(point);
            }
            AddPoint(points[0]);
            int count = 0;
            best = (float)Calculation.CalcFunc(Function, (float)bestVector.X, (float)bestVector.Y);
            worst = best;
            while (count != 11)
            {
                int iMin = 0, iMax = 0, iAverage = 0;
                int i = 0;
                best = (float)Calculation.CalcFunc(Function, (float)bestVector.X, (float)bestVector.Y);
                worst = best;
                foreach (var point in points)
                {
                    float? val = Calculation.CalcFunc(Function, (float)point.X, (float)point.Y);
                    if (val == null)
                    {
                        MessageBox.Show("Обнаружена ошибка!");
                        return;
                    }
                    res.Add((float)val);
                }
                best = res.Min();
                worst = res.Max();
                iMin = res.IndexOf(best);
                iMax = res.IndexOf(worst);
                if (iMin == 0 && iMax == 1 || iMax == 0 && iMin == 1) iAverage = 2;
                else if (iMin == 0 && iMax == 2 || iMax == 0 && iMin == 2) iAverage = 1;
                else iAverage = 0;
                good = res[iAverage];
                float xMid = (float)((points[iMin].X + points[iAverage].X) / 2);
                float yMid = (float)((points[iMin].Y + points[iAverage].Y) / 2);
                DataPoint xr = new DataPoint(xMid + (xMid - points[iMax].X), yMid + (yMid - points[iMax].Y));
                DataPoint xe;
                if (Calculation.CalcFunc(Function, (float)xr.X, (float)xr.Y) < best)
                {
                    xe = new DataPoint(xr.X * 2 - xMid, xr.Y * 2 - yMid);
                    if (Calculation.CalcFunc(Function, (float)xe.X, (float)xe.Y) < best)
                    {
                        bestVector = xe;
                        AddPoint(xe);
                    }
                    else
                    {
                        bestVector = xr;
                        AddPoint(xr);
                    }
                }
                else
                {
                    DataPoint xc = new DataPoint(xMid + beta * (worstVector.X - xMid), yMid + beta * (worstVector.Y - yMid));
                    if (Calculation.CalcFunc(Function, (float)xc.X, (float)xc.Y) < best)
                    {
                        bestVector = xc;
                        AddPoint(xc);
                    }
                }
                count++;
                averageVector = points[iMin];
                worstVector = points[iAverage];
                points.Clear();
                points.Add(bestVector);
                points.Add(averageVector);
                points.Add(worstVector);
                res.Clear();

            }
            RefreshPlot();
        }
        #endregion
        public void AddPoint(DataPoint point)
        {
            _points.Add(point);
        }
        public void ClearPoints()
        {
            _points.Clear();
        }
        public void RemovePoint(DataPoint point)
        {
            _points.Remove(point);
        }
        public void RefreshPlot()
        {
            PlotModel.Series.Clear();
            LineSeries series = new LineSeries();
            series.Points.AddRange(Points);
            PlotModel.Series.Add(series);
            PlotModel.InvalidatePlot(true);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
