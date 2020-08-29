using pdfToText.Interfaces;
using pdfToText.ViewModels;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pdfToText.Logic
{
    public class PdfConverter : IPdfConverter
    {
        public async void ConvertFromPdfToText(string path, MainViewModel mainViewModel)
        {
            await Task.Run(() =>
            {
                SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
                f.OpenPdf(path);

                string final = f.ToText();
                //string header = @"[A-Z]+\s[A-Z]+";
                //string pattern = @"Length:\s\d+.\d+\s\w+\s\(\d+'\s\d""\)|Beam:\s\d+.\d+\s\w+\s\(\d+'\s\d""\)|Draft:\s\d.\d\s\w+\s\(\d'\s\d+""\)|Number\sof\sGuests:\s\d+|Number\sof\sCrew:\s\d|Built:\s\d+|Refit:\s\d+|Builder:\s\w+\s\w+\s\w+|Naval\sArchitect:\s\w+\s\w+\s\w+|Flag:\s\w+|Hull\sConstruction:\s\w+|Hull\sConfiguration:\s\w+";
                //Regex headerR = new Regex(header);

                //mainViewModel.Title += "Yacht's name: " + headerR.Match(final) + System.Environment.NewLine;

                string pattern = @"Yacht's name:\s+\w+\/\w+\s+[A-Z]+\s[A-Z]+|Yacht's name:\s+\w+\/\w+\s+\w+|Previously:\s+\w+|Previously:\s+[A-Z]+\s+[A-Z]+|Length:\s+\d+.\d+\w+\s+\(\d+'\s+\d+""\)|Beam:\s+\d+.\d+\w+\s+\(\d +'\s+\d+""\)|Draft:\s+\d+.\d+\w+\s+\(\d+'\s+\d+""\)|Year Built:\s+\d+|Builder:\s+\w+|Builder:\s+\w+\s+\w+|Guests\s+\(\w+\):\s+\d+|No\. of Cabins:\s+\d+|Cruising Speed:\s+\d+\s+\w+|Cruising Speed:\s+\d+\.\d\s+\w+|Summer \d+\w+ Price:\s+\d+\s+\w+|Winter\s+\d+\/\d+\s+\w+\s+\w+:\s+\d+\s+\w+";
                Regex regex = new Regex(pattern);

                foreach (Match match in regex.Matches(final))
                {
                    mainViewModel.Title += match.Value + System.Environment.NewLine;
                }

            });
        }
    }
}
