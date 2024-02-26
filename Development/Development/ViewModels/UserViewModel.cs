using Development.Models;
using Development.StaticFunctions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Development.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        #region Fields

        //Variables
        private string answer = "";
        private string show = "";
        private object _selectedItem;
        readonly char[] deli = { '^', '√', '/', '*', '-', '+' };
        private readonly ObservableCollection<Calculations> calculations;

        //Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Cammands
        public ICommand GetValue { get; private set; }
        public ICommand Calculate { get; private set; }
        public ICommand ItemTapped { get; private set; }

        //Status holders
        private bool last = false;
        private bool calcualted = false;
        private bool negativeNum = false;
        #endregion



        #region Properties

        public string Answer
        {
            get { return answer; }
            set
            {
                answer = value;
                OnPropertyChanged(nameof(Answer));
            }
        }

        public string Show
        {
            get { return show; }
            set
            {
                show = value;
                OnPropertyChanged(nameof(Show));
            }
        }


        public ObservableCollection<Calculations> Calculations
        {
            get
            {
                return calculations;
            }
        }


        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = null;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }


        #endregion



        #region Constructor
        public UserViewModel()
        {
            GetValue = new Command<string>(GetValues);
            Calculate = new Command(Calculates);
            ItemTapped = new Command<object>(ItemTappedd);


            calculations = HistoryKeeper.ReadHistoryList();
            OnPropertyChanged(nameof(Calculations));
        }
        #endregion



        #region Commands

        //When any button beside "Calculate" is pressed
        private void GetValues(string input)
        {
            //Return when invalid input like operators or ac is pressed before a number
            if (FirstTimeSafty(input))
            {
                return;
            }

            string currentInput = input;

            //If the input is an int
            if (int.TryParse(input, out _) || input == ".")
            {
                currentInput = "number";
            }

            //Operators
            if (input == "+" || input == "-" || input == "*" || input == "/" || input == "^" || input == "√")
            {
                currentInput = "operator";
            }

            //Brackets
            if (input == "(" || input == ")")
            {
                currentInput = "bracket";
            }

            //Switching for each case of input value
            switch (currentInput)
            {
                case "del":
                    Delete();
                    break;
                case "ac":
                    AllClear();
                    break;
                case "number":
                    IsNumber(input);
                    break;
                case "operator":
                    IsOperator(input);
                    break;
                case "bracket":
                    IsBracket(input);
                    break;
                default: break;
            }

        }

        //When calculate button is pressed
        private void Calculates()
        {
            if (CalSafty())
            {
                return;
            }

            try
            {
                string outcome = (ToolFunc.Manager(show));

                if (outcome.Equals("Infinity") || outcome.Equals("NaN"))
                {
                    Application.Current.MainPage.DisplayAlert("Attention!", "Invalid input", "Ok");
                    return;
                }

                if (double.TryParse(outcome, out double dou))
                {

                    negativeNum = dou < 0;

                    answer = dou.ToString($"{ToolFunc.DecimalPointManager(dou)}");


                    // Updating history

                    DateTime datetime = DateTime.Now;
                    string date = datetime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    string time = datetime.ToString("HH:mm", CultureInfo.InvariantCulture);

                    HistoryKeeper.AppendToHistory(date, time, show, answer);
                    calculations.Insert(0, new Calculations(date, time, show, answer));
                    OnPropertyChanged(nameof(Calculations));

                    show = "";
                    calcualted = true;
                    OnPropertyChanged(nameof(Show));
                    OnPropertyChanged(nameof(Answer));

                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Attention!", outcome, "Ok");
                }

            }
            catch
            {
                Application.Current.MainPage.DisplayAlert("Error!", "Please check the input and try again!", "Ok");
            }

        }


        private void ItemTappedd(object args)
        {
            if (!(args is Calculations item))
                return;

            show = item.Showw;
            answer = "";

            _selectedItem = null;

            OnPropertyChanged(nameof(SelectedItem));
            OnPropertyChanged(nameof(Show));
            OnPropertyChanged(nameof(Answer));
        }

        #endregion



        #region Helper Functions

        private bool FirstTimeSafty(string input)
        {
            if (show == "" && answer == "" && !last && !negativeNum)
            {
                if (input == "ac")
                {
                    return true;
                }

                if (input == "del")
                {
                    return true;
                }

                if (input == "^" || input == "+" || input == "*" || input == "/" || input == ".")
                {
                    return true;
                }

                if (input == "-")
                {
                    negativeNum = true;
                    last = true;
                    show += input;
                    OnPropertyChanged(nameof(Show));
                    return true;
                }

                if (input == "√")
                {
                    last = true;
                    show = show + "2" + input;
                    OnPropertyChanged(nameof(Show));
                    return true;
                }

            }
            return false;
        }

        private bool CalSafty()
        {
            //Checking if the input is null
            if (show == "")
            {
                App.Current.MainPage.DisplayAlert("Empty input!", "Please enter a number first", "Ok");
                return true;
            }

            //Checking wheather the last input is a number
            string LastOpCheck = (show[show.Length - 1]).ToString();

            if (LastOpCheck.Equals("^") || LastOpCheck.Equals("√") || LastOpCheck.Equals("/") || LastOpCheck.Equals("*") || LastOpCheck.Equals("-") || LastOpCheck.Equals("+"))
            {
                App.Current.MainPage.DisplayAlert("Invalid input!", "Mathamatical statements can't end with operators", "Ok");
                return true;
            }

            //removes unnecessary brackets 
            if (show.StartsWith("(") && show.EndsWith(")"))
            {
                string temp = show.TrimStart('(').TrimEnd(')');
                show = temp;
            }

            //Check if the given input have operators or not 
            if (!show.Any(x => deli.Contains(x)))
            {
                answer = show;
                show = "";
                calcualted = true;

                OnPropertyChanged(nameof(Show));
                OnPropertyChanged(nameof(Answer));

                return true;
            }

            return false;
        }

        private void AllClear()
        {
            last = false;
            calcualted = false;
            negativeNum = false;
            show = "";
            answer = "";
            OnPropertyChanged(nameof(Show));
            OnPropertyChanged(nameof(Answer));
        }

        private void Delete()
        {
            //Preventing unncessary operation when both the show and answer are empty when "del" is pressed 
            if (show == "" && answer == "")
            {
                return;
            }


            //When "del" pressed and show is empty but answer is not
            //The last char of answer will be deleted before putting that value into show
            if (calcualted)
            {
                show = answer;
                answer = "";
                calcualted = false;
                OnPropertyChanged(nameof(Answer));
            }


            //A local variable to store the changed value of show 
            string temp;

            //When deleting, if show has "." and it's in the second last
            if ((show.Contains(".")) && (show[show.Length - 2].Equals('.')))
            {
                //Remove both the number and the decimal point
                temp = show.Remove(show.Length - 2, 2);
            }
            else
            {
                //If the deleted string is an operator
                if (Array.IndexOf(deli, show[show.Length - 1]) >= 0)
                {
                    last = false;
                }


                //Remove the last number
                temp = show.Remove(show.Length - 1, 1);
            }



            //Update the show value
            show = temp;
            OnPropertyChanged(nameof(Show));


        }

        private void IsNumber(string input)
        {
            if (input == ".")
            {
                if (show[show.Length - 1].Equals("."))
                {
                    return;
                }
                last = true;
            }

            //If answer is showing the result of previous operation
            if (calcualted)
            {
                //Start a new operation, Act like when "ac" is pressed with putting the pressed number into show
                AllClear();
                show += input;
                last = false;
                OnPropertyChanged(nameof(Show));
                return;
            }

            //If there is not answer from previous operation, just get the input into show
            show += input;
            //Make the last bool to be false which means the last input is not an operator
            //If last is true, the user can't pressed any operation button.
            last = false;

            //Update the result
            OnPropertyChanged(nameof(Show));
            return;
        }

        private void IsOperator(string input)
        {
            //If the last input was an operator, just return
            if (last)
            {
                return;
            }

            //If answer is showing the result of the last operation and operator is pressed
            if (calcualted)
            {
                show = answer + input;
                OnPropertyChanged(nameof(Show));

                answer = "";
                OnPropertyChanged(nameof(Answer));

                last = true;
                calcualted = false;

            }
            //If no value is present in answer
            else
            {
                last = true;
                show += input;
                OnPropertyChanged(nameof(Show));
            }

        }

        private void IsBracket(string input)
        {

            if (input == ")")
            {
                if (show.Length < 1)
                {
                    return;
                }

                char previous = show[show.Length - 1];

                if (previous.Equals('('))
                {
                    return;
                }

                if (!char.IsDigit(previous))
                {
                    return;
                }

            }

            if (input == "(")
            {
                if (calcualted)
                {
                    AllClear();
                }

                if (show.Length > 1)
                {
                    char previous = show[show.Length - 1];

                    if (previous.Equals(')'))
                    {
                        return;
                    }
                }

            }

            show += input;
            OnPropertyChanged(nameof(Show));
        }

        /* private void UpdateHistory()
         {
             DateTime datetime = DateTime.Now;
             string date = datetime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
             string time = datetime.ToString("HH:mm", CultureInfo.InvariantCulture);

             HistoryKeeper.AppendToHistory(date, time, show, answer);
             calculations.Add(new Calculations(date, time, show, answer));
             OnPropertyChanged(nameof(Calculations));
         }*/

        #endregion


        //Application.Current.MainPage.DisplayAlert("Error!", "Please enter a valid number", "Ok");

    }
}
