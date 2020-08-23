using pdfToText.ViewModels;

namespace pdfToText.Interfaces
{
    public interface IPdfConverter
    {
        void ConvertFromPdfToText(string path, MainViewModel mainViewModel);
    }
}
