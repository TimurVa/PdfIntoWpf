using pdfToText.ViewModels;

namespace pdfToText.Interfaces
{
    public interface IWordConverter
    {
        void ConvertFromWordToText(string path, MainViewModel mainViewModel);
    }
}
