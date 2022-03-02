using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using Vector = nelder_meade_method.Models.Vector;
using System.Text.RegularExpressions;

namespace nelder_meade_method.ViewModels
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private PlotModel _plotModel;
        private List<DataPoint> _points;
        private List<float> _listResultFunction;
        private string _function;
        private string _outputResultFunction;
        private string _outputPoint;
        private string _errorRate;
        private string _infoAboutPoint;
        private bool _isThreePoints;
        private int _step;
        private int _valStep;
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
        public bool TypeIsThreePoints
        {
            get { return _isThreePoints; }
            set
            {
                if (!bool.Equals(_isThreePoints, value))
                {
                    _isThreePoints = value;
                    if (value) _valStep = 3;
                    else _valStep = 1;
                    _step = 3;
                    RefreshPlot();
                    OnPropertyChanged("TypeIsThreePoints");
                }
            }
        }
        public string OutputResultFunction
        {
            get { return _outputResultFunction; }
            set
            {
                if (!string.Equals(_outputResultFunction, value))
                {
                    _outputResultFunction = value;
                    OnPropertyChanged("OutputResultFunction");
                }
            }
        }
        public string ErrorRate
        {
            get { return _errorRate; }
            set
            {
                if (!string.Equals(_errorRate, value))
                {
                    _errorRate = value;
                    OnPropertyChanged("ErrorRate");
                }
            }
        }
        public string InfoPoint
        {
            get { return _infoAboutPoint; }
            set
            {
                if (!string.Equals(_infoAboutPoint, value))
                {
                    _infoAboutPoint = value;
                    OnPropertyChanged("InfoPoint");
                }
            }
        }
        public string OutputPoint
        {
            get { return _outputPoint; }
            set
            {
                if (!string.Equals(_outputPoint, value))
                {
                    _outputPoint = value;
                    OnPropertyChanged("OutputPoint");
                }
            }
        }

        public MainWindowModel()
        {
            PlotModel = new PlotModel();
            ErrorRate = "0.0001";
            InfoPoint = "Ожидание функции...";
            Function = "x^2+x*y+y^2-6*x-9*y";
            _valStep = 1;
            _points = new List<DataPoint>();
            _listResultFunction = new List<float>();
        }
        public List<DataPoint> Points { get { return _points; } private set { _points = value; } }
        #region Command

        private DelegateCommand _beginCalculate;
        public DelegateCommand BeginCalculate
        {
            get { return _beginCalculate ?? (_beginCalculate = new DelegateCommand(CalculateExecute, CanCalculateExecute)); }
        }
        private DelegateCommand _prevStep;
        public DelegateCommand PrevStep
        {
            get { return _prevStep ?? (_prevStep = new DelegateCommand(GoToPrevStep, PrevStepExecute)); }
        }
        private DelegateCommand _nextStep;
        public DelegateCommand NextStep
        {
            get { return _nextStep ?? (_nextStep = new DelegateCommand(GoToNextStep, NextStepExecute)); }
        }
        private DelegateCommand _presentAllPoints;
        public DelegateCommand PresentAllPoints
        {
            get { return _presentAllPoints ?? (_presentAllPoints = new DelegateCommand(PresentAllPointsExecute, CanPresentAllPointsExecute)); }
        }
        private bool CanPresentAllPointsExecute(object obj)
        {
            if (Points.Count == 0) return false;
            else return true;
        }
        private void PresentAllPointsExecute(object obj)
        {
            TypeIsThreePoints = false;
            _step = Points.Count - 1;
            RefreshPlot();
        }
        private bool CanCalculateExecute(object obj)
        {
            return true;
        }
        private bool PrevStepExecute(object obj)
        {
            if (_step == 0) return false;
            else return true;
        }
        private void GoToPrevStep(object obj)
        {
            _step -= _valStep;
            RefreshPlot();
        }
        private void GoToNextStep(object obj)
        {
            _step += _valStep;
            RefreshPlot();
        }
        private bool NextStepExecute(object obj)
        {
            if ((_step+1) == Points.Count || Points.Count == 0) return false;
            else return true;
        }

        private void CalculateExecute(object obj)
        {
            float eps;
            if (Regex.IsMatch(ErrorRate.Replace('.', ','), @"0\,[0]*[1]{1}$"))
            {
                eps = float.Parse(ErrorRate.Replace('.', ','));
            }
            else
            {
                MessageBox.Show("Введите корректно точность вычисления!");
                return;
            }
            float alpha = 1, beta = 0.5f, gamma = 2;
            float best = 0, worst, good;
            float disspersion = 0;
            _step = 3;

            Vector bestVector = new Vector(0, 0);
            Vector averageVector = new Vector(1, 0);
            Vector worstVector = new Vector(0, 1);

            List<Vector> points = new List<Vector>();
            List<float> res = new List<float>();

            points.Add(new Vector(0, 0));
            points.Add(new Vector(1, 0));
            points.Add(new Vector(0, 1));

            ClearPoints();

            foreach (var point in points)
            {
                AddPoint(new DataPoint(point.X, point.Y));
            }
            AddPoint(new DataPoint(points[0].X, points[0].Y));

            int count = 0;
            do
            {
                int iMin = 0, iMax = 0, iAverage = 0; //индексы
                foreach (var point in points)
                {
                    float? val = Calculation.CalcFunc(Function, (float)point.X, (float)point.Y);
                    if (val == null)
                    {
                        MessageBox.Show("Обнаружена ошибка в формуле!");
                        return;
                    }
                    res.Add((float)val);
                }
                best = res.Min(); //поиск лучшей точки
                worst = res.Max(); //поиск худшей точки
                iMin = res.IndexOf(best); //запись индексов точек
                iMax = res.IndexOf(worst); //запись индексов точек
                //Поиск индекса среднего числа
                if (iMin == 0 && iMax == 1 || iMax == 0 && iMin == 1) iAverage = 2;
                else if (iMin == 0 && iMax == 2 || iMax == 0 && iMin == 2) iAverage = 1;
                else iAverage = 0;
                good = res[iAverage];

                Vector b = points[iMin]; //инициализация векторов
                Vector g = points[iAverage];
                Vector w = points[iMax];

                //Запись результатов
                _listResultFunction.Add(best);
                _listResultFunction.Add(worst);
                _listResultFunction.Add(res[iAverage]);

                Vector mid = (b + g) / 2; //поиск средней точки между точками best и good

                //reflection
                Vector xr = mid + (mid - w) * alpha;
                if (Calculation.CalcFunc(Function, (float)xr.X, (float)xr.Y) < good) w = xr; //good
                else
                {
                    if (Calculation.CalcFunc(Function, (float)xr.X, (float)xr.Y) < worst) w = xr; //worst
                    Vector c = (w + mid) / 2;
                    if (Calculation.CalcFunc(Function, (float)c.X, (float)c.Y) < worst) w = c;
                }
                //expansion
                if (Calculation.CalcFunc(Function, (float)xr.X, (float)xr.Y) < best)
                {
                    Vector xe = mid + (xr - mid) * gamma;
                    if (Calculation.CalcFunc(Function, (float)xe.X, (float)xe.Y) < Calculation.CalcFunc(Function, (float)xr.X, (float)xr.Y))
                    {
                        w = xe;
                    }
                    else
                    {
                        w = xr;
                    }
                }
                //contraction
                if (Calculation.CalcFunc(Function, (float)xr.X, (float)xr.Y) > good)
                {
                    Vector xc = mid + (w - mid) * beta;
                    if (Calculation.CalcFunc(Function, (float)xc.X, (float)xc.Y) < worst) w = xc;
                    {
                        w = xc;
                    }
                }
                count++;
                averageVector = g;
                worstVector = w;
                bestVector = b;
                points.Clear();
                points.Add(worstVector);
                points.Add(averageVector);
                points.Add(bestVector);
                res.Clear();
                foreach (var point in points)
                {
                    AddPoint(new DataPoint(point.X, point.Y));
                }
                disspersion = FindDispersion(_listResultFunction);
                _listResultFunction.Clear();
                if (count == 100000)
                {
                    OutputResultFunction = "Экстремума нет";
                    return;
                }
            } while (disspersion > eps);
            if (float.IsNaN(disspersion))
            {
                OutputResultFunction = "Экстремума нет";
                return;
            }
            RefreshPlot();
            OutputResultFunction = System.Math.Round(best, 4).ToString();
            OutputPoint = System.Math.Round(bestVector.X, 4).ToString() + ";" + System.Math.Round(bestVector.Y, 4).ToString();
        }
        private float FindDispersion(List<float> nums)
        {
            int n = nums.Count;
            float average = nums.Sum() / n;
            float sum = 0;
            foreach (float x in nums)
            {
                sum += (float)System.Math.Pow(x - average,2);
            }
            return sum / (n - 1);
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
            List<DataPoint> datas = new List<DataPoint>();
            if (TypeIsThreePoints)
            {
                for (int i = _step - 3; i <= _step && i < Points.Count; i++)
                {
                    datas.Add(Points[i]);
                }
            } else
            for (int i = 0; i <= _step && i < Points.Count; i++)
            {
                datas.Add(Points[i]);
            }
            PlotModel.Series.Clear();
            LineSeries series = new LineSeries();
            series.Points.AddRange(datas);
            PlotModel.Series.Add(series);
            PlotModel.InvalidatePlot(true);
            PrevStep.RaiseCanExecuteChanged();
            NextStep.RaiseCanExecuteChanged();
            PresentAllPoints.RaiseCanExecuteChanged();
            InfoPoint = string.Format("Точка номер {0}: ({1}; {2})",_step,
                System.Math.Round(Points[_step].X,4),
                System.Math.Round(Points[_step].Y,4));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
