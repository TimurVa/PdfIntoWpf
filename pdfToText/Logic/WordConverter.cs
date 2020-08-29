using pdfToText.Interfaces;
using pdfToText.ViewModels;
using Spire.Doc;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pdfToText.Logic
{
    public class WordConverter : IWordConverter
    {
        public async void ConvertFromWordToText(string path, MainViewModel mainViewModel)
        {
            await Task.Run(() =>
            {
                Document document = new Document();
                document.LoadFromFile(path);

                string final = document.GetText();
                final = final.ToString();

                string pattern = @"Yacht's name:\s+\w+\/\w+\s+[A-Z]+\s[A-Z]+|Yacht's name:\s+\w+\/\w+\s+\w+|Previously:\s+\w+|Previously:\s+[A-Z]+\s+[A-Z]+|Length:\s+\d+.\d+\w+\s+\(\d+'\s+\d+""\)| Beam:\s +\d +.\d +\w +\s +\(\d + '\s+\d+""\)|Draft:\s+\d+.\d+\w+\s+\(\d+'\s +\d + ""\)|Year Built:\s+\d+|Builder:\s+\w+|Builder:\s+\w+\s+\w+|Guests\s+\(\w+\):\s+\d+|No\. of Cabins:\s+\d+|Cruising Speed:\s+\d+\s+\w+|Cruising Speed:\s+\d+\.\d\s+\w+|Summer \d+ \w+ Price:\s+\d+\s+\w+|Winter\s+\d+\/\d+\s+\w+\s+\w+:\s+\d+\s+\w+";
                Regex regex = new Regex(pattern);

                foreach (Match match in regex.Matches(final))
                {
                    mainViewModel.Title += match.Value + System.Environment.NewLine;
                }
            });
           
        }
    }
}
