using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MidiavoxChat.Pages
{
    /// <summary>
    /// Interação lógica para StartingPage.xam
    /// </summary>
    public partial class StartingPage : Page
    {
        public StartingPage()
        {
            InitializeComponent();
        }

        private void HostChatBtn(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new HostPage());
        }

        private void OnConnectToChatBtnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ConnectToPage());
        }
    }
}
