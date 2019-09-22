using System.Windows;

namespace TwitchGUI
{
    /// <summary>
    /// Interaction logic for NewChannelDialog.xaml
    /// </summary>
    public partial class NewChannelDialog : Window
    {
        public NewChannelDialog()
        {
            InitializeComponent();
            Title = "Enter Channel Name";
            nameTextBox.Focus();
        }

        private void BtnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public string Answer
        {
            get
            {
                return nameTextBox.Text;
            }
        }
    }
}
