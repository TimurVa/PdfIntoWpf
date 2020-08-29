using Ninject;
using pdfToText.Interfaces;
using pdfToText.Logic;
using pdfToText.ViewModels;
using System.ComponentModel;

namespace pdfToText
{
    public static class IoC
    {
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        public static void Setup()
        {
            Bind();
        }

        private static void Bind()
        {
            Kernel.Bind<IPdfConverter>().To<PdfConverter>().InTransientScope();
            Kernel.Bind<IWordConverter>().To<WordConverter>().InTransientScope();
        }

        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}
