using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Development.StaticFunctions;

namespace Development.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        #region Fields
        private string answer = "";
        private string show = "";

        string[] deli = { "^", "√", "/", "*", "-", "+"};
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand GetValue { get; private set; }
        public ICommand Calculate { get; private set; }

        private bool last = false;
        private bool calcualted = false;
        private bool negativeNum = false;
        #endregion


        //Adjust the brackets with the existing system!!


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

        #endregion



        #region Constructor
        public UserViewModel()
        {
            GetValue = new Command<string>(GetValues);
            Calculate = new Command(Calculates);
        }
        #endregion



        #region Commands


        private void GetValues(string input)
        {
            //Return when invalid input like operators or ac is pressed before a number
            if (FirstTimeSafty(input))
            {
                return;
            }

            if (input == "del")
            {
                Delete(input);
            }

            //When AC is pressed
            if (input == "ac")
            {
                //Clear every array and string invlove with the help of AllClear(); function
                AllClear();
                return;
            }



            //If the input is an int
            if (int.TryParse(input, out _) || input == "." || input == "(" || input == ")")
            {
                if (last && input == ")")
                {
                    return;
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
                else
                {
                    //If there is not answer from previous operation, just get the input into show
                    show += input;
                    //Make the last bool to be false which means the last input is not an operator
                    //If last is true, the user can't pressed any operation button.
                    last = false;

                    //Update the result
                    OnPropertyChanged(nameof(Show));
                    return;
                }

            }

            if (input == "(" || input == ")")
            {
                if(input.Length > 1)
                {
                    char previous = input[input.Length - 1];

                    if (previous == '(')
                    {
                        return;
                    }

                    if (char.IsDigit(previous))
                    {
                        return;
                    }
                }

                show+= input;
                OnPropertyChanged(nameof(Show));

            }

            //Operators
            if (input == "+" || input == "-" || input == "*" || input == "/" || input == "^" || input == "√" || input == ".")
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

        }



        //When calculate button is pressed

        private void Calculates()
        {
            //Checking if the input is null
            if (show == "")
            {
                App.Current.MainPage.DisplayAlert("Empty input!", "Please enter a number first", "Ok");
                return;
            }

            //Checking wheather the last input is a number
            string LastOpCheck = (show[show.Length - 1]).ToString();

            if (LastOpCheck.Equals("^") || LastOpCheck.Equals("√") || LastOpCheck.Equals("/") || LastOpCheck.Equals("*") || LastOpCheck.Equals("-") || LastOpCheck.Equals("+"))
            {
                App.Current.MainPage.DisplayAlert("Error!", "Invalid input, please provide correct mathematical convention", "Ok");
                return;
            }

            if(show.StartsWith("(") && show.EndsWith(")"))
            {
                string temp = show.TrimStart('(').TrimEnd(')');
                show = temp;
            }

            List<string> buffer = show.Split(deli, StringSplitOptions.None).ToList();

            if (negativeNum)
            {
                buffer[1] = "-" + buffer[1];
                buffer.RemoveAt(0);
            }

            if (buffer.Count == 1)
            {
                if (buffer[0].Contains("(") && buffer[0].Contains(")"))
                {
                    string buffer12 = buffer[0].TrimStart('(').TrimEnd(')');
                    show = buffer12;
                }

                answer = show;
                show = "";
                calcualted= true;

                OnPropertyChanged(nameof(Show));
                OnPropertyChanged(nameof(Answer));

                return;
            }

            buffer = null;


            //Getting the answer from the tool function Solver which takes the arraylist of the input values
            //Use the arraylist named array which contains all the operators required

            try
            {
                double outcome = double.Parse(ToolFunc.Manager(show));

                negativeNum = outcome < 0;

                answer = outcome.ToString($"{ToolFunc.DecimalPointManager(outcome)}");

                show = "";
                OnPropertyChanged(nameof(Show));
                OnPropertyChanged(nameof(Answer));

                calcualted = true;
            }catch(Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error!", "Please check the input and try again!", "Ok");
            }

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

                if(input == "√")
                {
                    last = true;
                    show = show + "2" + input;
                    OnPropertyChanged(nameof(Show));
                    return true;
                }

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


        private void Delete(string input)
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
                    last= false;
                }

                //Remove the last number
                temp = show.Remove(show.Length - 1, 1);
            }

            

            //Update the show value
            show = temp;
            OnPropertyChanged(nameof(Show));


        }

        #endregion


        //Application.Current.MainPage.DisplayAlert("Error!", "Please enter a valid number", "Ok");

    }
}
