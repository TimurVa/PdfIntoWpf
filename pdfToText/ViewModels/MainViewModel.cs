using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace pdfToText.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly List<string> _topics = new List<string> { "dog", "car", "truck", "cat", "florida" };

        private string _title;
        public string Title 
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string result;

        System.Drawing.Bitmap bmp;


        private MainWindow _mainWindow;
        public MainViewModel(MainWindow mainWindow)
        {
            Title = string.Empty;
        }

        private ICommand _ofdCommand;
        public ICommand OfdCommand => _ofdCommand ?? (_ofdCommand = new RelayCommand(OfdCommand_Executed));

        //private void ExtractTextFromPdf(string path)
        //{
        //    //using (PdfReader reader = new PdfReader(path))
        //    //{
        //    //    StringBuilder text = new StringBuilder();
        //    //    ITextExtractionStrategy Strategy = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();

        //    //    for (int i = 1; i <= reader.NumberOfPages; i++)
        //    //    {
        //    //        string page = "";

        //    //        page = PdfTextExtractor.GetTextFromPage(reader, i, Strategy);
        //    //        string[] lines = page.Split('\n');
        //    //        foreach (string line in lines)
        //    //        {
        //    //            Title += line;
        //    //        }
        //    //    }
        //    //}
        //}

        private void DoStuff(string path)
        {
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            f.OpenPdf(path);

            string final = f.ToText();
            string pattern = @"Length:\s\d+.\d+\s\w+\s\(\d+'\s\d""\)|Beam:\s\d+.\d+\s\w+\s\(\d+'\s\d""\)|Draft:\s\d.\d\s\w+\s\(\d'\s\d+""\)|Number\sof\sGuests:\s\d+|Number\sof\sCrew:\s\d|Built:\s\d+|Refit:\s\d+|Builder:\s\w+\s\w+\s\w+|Naval\sArchitect:\s\w+\s\w+\s\w+|Flag:\s\w+|Hull\sConstruction:\s\w+|Hull\sConfiguration:\s\w+";
            Regex regex = new Regex(pattern);

            int i = 0;

            foreach (Match match in regex.Matches(final))
            {
                Title += match.Value + System.Environment.NewLine;

                i++;

                if (i > 9)
                {
                    break;
                }

                //if (i == 1)
                //{
                //    _topics.Add(Title);
                //}
            }
        }

        private void OfdCommand_Executed(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Text files (*.pdf)|*.pdf";

            if (openFileDialog.ShowDialog() == true)
            {
                //ExtractTextFromPdf(openFileDialog.FileName);
                DoStuff(openFileDialog.FileName);

                string html = GetHtmlCode();
                List<string> urls = GetUrls(html);
                //var rnd = new Random();

                //int randomUrl = rnd.Next(0, urls.Count + 1);

                string luckyUrl = urls[0];
                byte[] image = GetImage(luckyUrl);


                using (var ms = new MemoryStream(image))
                {
                    //_mainWindow.Canvas.Children.Add(Image.FromStream(ms));

                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();

                    System.Windows.Controls.Image image1 = new System.Windows.Controls.Image();
                    image1.Source = bi;

                    _mainWindow.Canvas.Children.Add(image1);
                }
            }
        }

        private string GetHtmlCode()
        {
            var rnd = new Random();

            int topic = rnd.Next(0, _topics.Count - 1);

            string url = "https://www.google.com/search?q=" + _topics[topic] + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }

        private List<string> GetUrls(string html)
        {
            var urls = new List<string>();

            string search = @",""ou"":""(.*?)"",";
            MatchCollection matches = Regex.Matches(html, search);

            foreach (Match match in matches)
            {
                urls.Add(match.Groups[1].Value);
            }

            return urls;
        }

        //private List<string> GetUrls(string html)
        //{
        //    var urls = new List<string>();

        //    int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

        //    while (ndx >= 0)
        //    {
        //        ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
        //        ndx++;
        //        int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
        //        string url = html.Substring(ndx, ndx2 - ndx);
        //        urls.Add(url);
        //        ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
        //    }
        //    return urls;
        //}

        //private List<string> GetUrls(string html)
        //{
        //    var urls = new List<string>();
        //    int ndx = html.IndexOf("class=\"images_table\"", StringComparison.Ordinal);
        //    ndx = html.IndexOf("<img", ndx, StringComparison.Ordinal);

        //    while (ndx >= 0)
        //    {
        //        ndx = html.IndexOf("src=\"", ndx, StringComparison.Ordinal);
        //        ndx = ndx + 5;
        //        int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
        //        string url = html.Substring(ndx, ndx2 - ndx);
        //        urls.Add(url);
        //        ndx = html.IndexOf("<img", ndx, StringComparison.Ordinal);
        //    }
        //    return urls;
        //}

        private byte[] GetImage(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return null;
                using (var sr = new BinaryReader(dataStream))
                {
                    byte[] bytes = sr.ReadBytes(100000000);

                    return bytes;
                }
            }
            return null;
        }


    }
}
