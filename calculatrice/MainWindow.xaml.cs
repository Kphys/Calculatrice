using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
using org.mariuszgromada.math.mxparser;

namespace calculatrice
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, object> _propertyValues = new Dictionary<string, object>();

        public T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (_propertyValues.ContainsKey(propertyName))
                return (T)_propertyValues[propertyName];
            return default(T);
        }
        public bool SetValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            var currentValue = GetValue<T>(propertyName);
            if (currentValue == null && newValue != null
             || currentValue != null && !currentValue.Equals(newValue))
            {
                _propertyValues[propertyName] = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }


        #endregion
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            CalcString = "";
            CurrentValue = "";
        }

        public string CalcString
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string CurrentValue
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        private ObservableCollection<String> _historique;
        public ObservableCollection<String> Historique
        {
            get
            {
                if (_historique == null) _historique = new ObservableCollection<String>();
                return _historique;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentValue == "NaN")
            {
                CurrentValue = "";
            }
            string input = (sender as Button).Content as string;

            Regex isNumber = new Regex(@"^\d$");

            if (isNumber.IsMatch(input))
            {
                CurrentValue += input;
            }
            else if (input == "Supprimer l'historique")
            {
                Historique.Clear();
            }
            else if (input == "C")
            {
                CalcString = "";
                CurrentValue = "";
            }
            else if (input == "CE")
            {
                CurrentValue = "";
            }
            else if (input == "=")
            {
                if (CalcString.Length > 0)
                {
                    if (CurrentValue.Length == 0)
                    {
                        CalcString = CalcString.Substring(0, CalcString.Length - 1);
                    }
                    CalcString += CurrentValue;
                    CalcString = CalcString.Replace(",", ".");
                    org.mariuszgromada.math.mxparser.Expression expression = new org.mariuszgromada.math.mxparser.Expression(CalcString);
                    double result = expression.calculate();
                    CurrentValue = result.ToString();
                    Historique.Add(CalcString + "=" + result.ToString());
                    CalcString = "";
                }
            }
            else if (input == ",")
            {
                if (CurrentValue.IndexOf(',') == -1 && CurrentValue.Length != 0)
                {
                    CurrentValue += input;
                }
            }
            else
            {
                if (CurrentValue.Substring(CurrentValue.Length - 1) == ",")
                {
                    CurrentValue = CurrentValue.Substring(0, CurrentValue.Length - 1);
                }
                if (CurrentValue != "")
                {
                    CalcString += CurrentValue + input;
                    CurrentValue = "";
                } else if (CalcString.Length > 0)
                {
                    CalcString = CalcString.Substring(0, CalcString.Length - 1) + input;
                }
            }

        }
    }
}
