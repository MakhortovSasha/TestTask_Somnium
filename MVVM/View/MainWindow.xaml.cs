using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestTask_Somnium.Core.Helpers;

namespace TestTask_Somnium
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private  ScrollViewer ContactsScrollViewer;
        public MainWindow()
        {
            InitializeComponent();

            //Link EmojiPicker to the RichTextBox
            EditorPicker.Picked += (o, e) =>
            {
                InputTextBox.CaretPosition.InsertTextInRun(e.Emoji);

            };
        }

        ///Core behaviours
        ///see no reason for moving them to the VM
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
            if (e.RightButton == MouseButtonState.Pressed) { Application.Current.Shutdown(); }
        }

        private void Contacts_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
           
            if (ContactsScrollViewer==null)
                if(!(sender as ItemsControl).TryGetChildOfType<ScrollViewer>(ref ContactsScrollViewer)) 
                    return;

            if (e.Delta < 0)
            {
                ContactsScrollViewer.LineRight();

            }
            else
            {
                ContactsScrollViewer.LineLeft();
            }
            e.Handled = true;
        }

    }
    
}