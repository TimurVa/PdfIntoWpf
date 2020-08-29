using Ninject;
using pdfToText.Interfaces;
using pdfToText.ViewModels;
using System.Windows;

namespace pdfToText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this, IoC.Get<IPdfConverter>(), IoC.Get<IWordConverter>()); ;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            ((MainViewModel)DataContext).DropPdf(sender, e);
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ((MainViewModel)DataContext).SearchText();
        }
    }
}
