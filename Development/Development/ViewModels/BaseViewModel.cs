using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Development.ViewModels
{
    public abstract class BaseViewModel : BindableObject
    {
        public Dictionary<string, ICommand> Commands { get; protected set; }

        public BaseViewModel()
        {
            Commands = new Dictionary<string, ICommand>();
        }

    }
}
