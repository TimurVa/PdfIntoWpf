﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Input;
using pdfToText.Logic;
using System.Windows;
using pdfToText.Interfaces;
using System.Linq;

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
        public string QueryString { get; set; }

        private IPdfConverter _pdfConverter;
        private IWordConverter _wordConverter;
        System.Drawing.Bitmap bmp;

        private MainWindow _mainWindow;
        public MainViewModel(MainWindow mainWindow, IPdfConverter pdfConverter, IWordConverter wordConverter)
        {
            Title = string.Empty;
            _pdfConverter = pdfConverter;
            _wordConverter = wordConverter;
        }

        private ICommand _ofdCommand;
        public ICommand OfdCommand => _ofdCommand ?? (_ofdCommand = new RelayCommand(OfdCommand_Executed));



        private void OfdCommand_Executed(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Pdf file (*.pdf)|*.pdf|Word file (.docx , .doc)|*.docx;*.doc";

            if (openFileDialog.ShowDialog() == true)
            {

                string extension = System.IO.Path.GetExtension(openFileDialog.FileName);

                // pdf
                if (extension == ".pdf")
                {
                    _pdfConverter.ConvertFromPdfToText(openFileDialog.FileName, this);
                    //DoStuff(openFileDialog.FileName);
                }
                // word
                else
                {
                    _wordConverter.ConvertFromWordToText(openFileDialog.FileName, this);

                }
            }
        }

        //"Pdf file (*.pdf)|*.pdf|Word file (.docx , .doc)|*.docx;*.doc"
        public void DropPdf(object s, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    string extension = System.IO.Path.GetExtension(file);

                    try
                    {
                        // pdf
                        if (extension == ".pdf")
                        {
                            _pdfConverter.ConvertFromPdfToText(file, this);
                            //DoStuff(openFileDialog.FileName);
                        }
                        // word
                        else
                        {
                            _wordConverter.ConvertFromWordToText(file, this);

                        }
                    } 
                    catch (Exception exception)
                    {
                        MessageBox.Show($"One or more files has different format. Accepts only .doc/x or .pdf \nException { exception.ToString() }");
                    }
                }
            }
        }

        public void SearchText()
        {
            int index = 0;
            while (index < Title.LastIndexOf(QueryString))
            {
                
            }
        }

        #region Try parce image
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
            string header = @"[A-Z]+\s[A-Z]+";
            string pattern = @"Length:\s\d+.\d+\s\w+\s\(\d+'\s\d""\)|Beam:\s\d+.\d+\s\w+\s\(\d+'\s\d""\)|Draft:\s\d.\d\s\w+\s\(\d'\s\d+""\)|Number\sof\sGuests:\s\d+|Number\sof\sCrew:\s\d|Built:\s\d+|Refit:\s\d+|Builder:\s\w+\s\w+\s\w+|Naval\sArchitect:\s\w+\s\w+\s\w+|Flag:\s\w+|Hull\sConstruction:\s\w+|Hull\sConfiguration:\s\w+";
            Regex regex = new Regex(pattern);
            Regex headerR = new Regex(header);

            Title += "Yacht's name: " + headerR.Match(final) + System.Environment.NewLine;

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
        #endregion
    }
}
