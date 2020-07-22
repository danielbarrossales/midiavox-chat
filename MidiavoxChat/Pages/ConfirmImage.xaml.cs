using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interação lógica para ConfirmImage.xam
    /// </summary>
    public partial class ConfirmImage : Page
    {
        private ChatPage _chatPage;
        private BitmapImage bitmap;
        private Uri _imageUri;
        public ConfirmImage(Uri imageUri, ChatPage chatPage)
        {
            InitializeComponent();
            bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = imageUri;
            bitmap.EndInit();
            Image.Source = bitmap;
            _chatPage = chatPage;
            _imageUri = imageUri;
        }

        private void Send(object sender, RoutedEventArgs e)
        {
            byte[] data = null;

            using (FileStream fs = new FileStream(_imageUri.OriginalString, FileMode.Open, FileAccess.Read))
            {
                var binaryReader = new BinaryReader(fs);
                data = binaryReader.ReadBytes((int)fs.Length);
            }
            _chatPage.SendFile(data);
            this.NavigationService.GoBack();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
