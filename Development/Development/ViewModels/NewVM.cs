using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Development.ViewModels
{
    public class NewVM : INotifyPropertyChanged
    {
        private string _name;
        private string _name2;
        private string _answer;

        public ICommand command { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NewVM()
        {
            command = new Command(NewFunction);
        }

        public string Name
        {
            get => _name;
            set
            {
                _name= value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Name2
        {
            get => _name2;
            set
            {
                _name2 = value;
                OnPropertyChanged(nameof(Name2));
            }
        }

        public string Answer
        {
            get => _answer;
            set
            {
                _answer = value;
                OnPropertyChanged(nameof(Answer));
            }
        }


        private void NewFunction()
        {
            int num1 = int.Parse(_name);
            int num2 = int.Parse(_name2);

            int result = (num1 + num2);
            _answer = result.ToString();
            OnPropertyChanged(nameof(Answer));
        }


        
    }
}
