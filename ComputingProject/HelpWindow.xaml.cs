using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComputingProject
{
	/// <summary>
	/// Interaction logic for HelpWindow.xaml
	/// Testing URL: ("https://tbeakl.github.io/ALevelComputingProjectHelp/")
	/// </summary>
	public partial class HelpWindow : Window
    {
        //To start a help window you need to say what is the url of the webpage which has the help for that part of the program on it
        public HelpWindow(string url)
        {
            InitializeComponent();
            //Navigates the webbrowser in the window to the correct help site
            WebBrowserHelpDisplay.Navigate(url);
        }
    }
}
