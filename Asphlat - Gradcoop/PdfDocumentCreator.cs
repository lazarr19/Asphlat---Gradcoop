using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Asphalt__Gradcoop
{
    static class PdfDocumentCreator
    {
        public static void CreateAsphaltOffer(string fileNameBase, List<Offer> offers) {
            PdfDocumentBuilder pdfDocumentBuilder = new PdfDocumentBuilder(fileNameBase);
            pdfDocumentBuilder.CreatePdf(offers);
        }
    }
}