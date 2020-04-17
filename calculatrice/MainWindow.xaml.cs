using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
            
            string input = (sender as Button).Content as string;

            calc(input);

        }
        private string format(string number)
        {
            string[] tmp = number.Split(',');
            tmp[0] = Convert.ToDouble(tmp[0]).ToString("#,##0", new CultureInfo("fr-FR"));
            return tmp.Length > 1 ? tmp[0] + "," + tmp[1] : tmp[0];

        }
        private void calc(string input)
        {

            if (CurrentValue == "NaN")
            {
                CurrentValue = "";
            }
            Regex isNumber = new Regex(@"^\d$");

            if (isNumber.IsMatch(input))
            {
                CurrentValue += input;
                CurrentValue = format(CurrentValue);
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
                    if (CurrentValue.Length == 0 && CalcString.Substring(CalcString.Length - 1) != "!" && CalcString.Substring(CalcString.Length - 1) != ")")
                    {
                        CalcString = CalcString.Substring(0, CalcString.Length - 1);
                    }
                    CalcString += CurrentValue;
                    if (CalcString.Count(x => x == '(') > CalcString.Count(x => x == ')'))
                    {
                        int numberOfOpen = CalcString.Count(x => x == '(');
                        int numberOfClose = CalcString.Count(x => x == ')');
                        int rest = numberOfOpen - numberOfClose;
                        for (int i = 0; i < rest; i++)
                        {
                            CalcString += ')';
                        }
                    }
                    CalcString = CalcString.Replace(",", ".");
                    org.mariuszgromada.math.mxparser.Expression expression = new org.mariuszgromada.math.mxparser.Expression(new string(CalcString.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray()));
                    double result = expression.calculate();
                    CurrentValue = format(result.ToString());
                    Historique.Add(CalcString + "=" + format(result.ToString()));
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
            else if (input == "(")
            {
                CalcString += input;
            }
            else if (input == ")")
            {
                CalcString += CurrentValue + ")";
                CurrentValue = "";
            }
            else
            {
                // Supprime la virgule en fin de lignesi un opérateur est choisie
                if (CurrentValue.Length > 0 && CurrentValue.Substring(CurrentValue.Length - 1) == ",")
                {
                    CurrentValue = CurrentValue.Substring(0, CurrentValue.Length - 1);
                }
                // Ajoute un opérateur s'il y a déjà une valeur enregistré 
                if (CurrentValue != "" || (CalcString.Length > 0 && CalcString.Substring(CalcString.Length - 1) == "!") || (CalcString.Length > 0 && CalcString.Substring(CalcString.Length - 1) == ")"))
                {
                    CalcString += CurrentValue + input;
                    CurrentValue = "";
                }
                else if (CalcString.Length > 0)
                {
                    CalcString = CalcString.Substring(0, CalcString.Length - 1) + input;
                }
                if (input == "-" && CalcString.Length == 0)
                {
                    CalcString = "0-";
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad0 || (IsKeyboardShift && e.Key == Key.D0))
            {
                calc("0");
            }
            else if (e.Key == Key.NumPad1 || (IsKeyboardShift && e.Key == Key.D1))
            {
                calc("1");
            }
            else if (e.Key == Key.NumPad2 || (IsKeyboardShift && e.Key == Key.D2))
            {
                calc("2");
            }
            else if (e.Key == Key.NumPad3 || (IsKeyboardShift && e.Key == Key.D3))
            {
                calc("3");
            }
            else if (e.Key == Key.NumPad4 || (IsKeyboardShift && e.Key == Key.D4))
            {
                calc("4");
            }
            else if (e.Key == Key.NumPad5 || (IsKeyboardShift && e.Key == Key.D5))
            {
                calc("5");
            }
            else if (e.Key == Key.NumPad6 || (IsKeyboardShift && e.Key == Key.D6))
            {
                calc("6");
            }
            else if (e.Key == Key.NumPad7 || (IsKeyboardShift && e.Key == Key.D7))
            {
                calc("7");
            }
            else if (e.Key == Key.NumPad8 || (IsKeyboardShift && e.Key == Key.D8))
            {
                calc("8");
            }
            else if (e.Key == Key.NumPad9 || (IsKeyboardShift && e.Key == Key.D9))
            {
                calc("9");
            }
            else if (e.Key == Key.D5)
            {
                calc("(");
            }
            else if (e.Key == Key.Oem4)
            {
                calc(")");
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                calc("+");
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                calc("-");
            }
            else if (e.Key == Key.Multiply || e.Key == Key.Oem5)
            {
                calc("*");
            }
            else if (e.Key == Key.Divide || (IsKeyboardShift && e.Key == Key.Oem2))
            {
                calc("/");
            }
            else if (e.Key == Key.OemComma || (IsKeyboardShift && e.Key == Key.OemPeriod) || e.Key == Key.Decimal)
            {
                calc(",");
            }
            else if (e.Key == Key.S)
            {
                org.mariuszgromada.math.mxparser.Expression expression = new org.mariuszgromada.math.mxparser.Expression("sin(90)");
                double result = expression.calculate();
            }
            else if (e.Key == Key.Enter)
            {
                calc("=");
            }
            else if (IsKeyboardShift && e.Key == Key.Oem3)
            {
                calc("#");
            }

            else if (e.Key == Key.Oem8)
            {
                calc("!");
            }
        }

        public bool IsKeyboardShift
        {
            get
            {
                return Keyboard.Modifiers == ModifierKeys.Shift && !Keyboard.IsKeyToggled(Key.CapsLock) || Keyboard.Modifiers == ModifierKeys.None && Keyboard.IsKeyToggled(Key.CapsLock);
            }
        }

        private void LoadHistorique(object sender, MouseButtonEventArgs e)
        {
            string selectedLine = (sender as ListView).SelectedItem as string;
            CalcString = "";
            Regex pattern = new Regex(".*(?<==)");
            CurrentValue = pattern.Replace(selectedLine, "");
        }
    }
}
