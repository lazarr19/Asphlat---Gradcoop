using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content.Res;


using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Asphalt__Gradcoop
{
    public  class PdfDocumentBuilder
    {
        public PdfWriter PdfWriter { get; }
        public PdfDocument PdfDocument { get; }
        public Document Document { get; }

        private const string downloadsPath = "/storage/emulated/0/Download";

        public PdfDocumentBuilder(string fileName)
        {
            var filename = GenerateFileName(fileName) + ".pdf";
            var filePath = Path.Combine(downloadsPath, filename);

            PdfWriter = new PdfWriter(filePath);
            PdfDocument = new PdfDocument(PdfWriter);
            Document = new Document(PdfDocument);
        }


        public void CreatePdf(List<Offer> offers)
        {
            var ps = PdfDocument.GetDefaultPageSize();

            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            Document.SetFont(font);

            //Create header of document
            Table header = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            header.AddCell(GetImageFromAsset("logo.png"));
            header.AddCell(new Paragraph("GRADCOOP d.o.o.\nKaradjordjeva 120, Umka, 11260 Beograd, Srbija\nTel: +381 11 8026 720\n +381 11 8025 020\nEmail: gradcoop.1@gmail.com").SetTextAlignment(TextAlignment.RIGHT));
            Document.Add(header);

            // Add text
            Paragraph name = new Paragraph("Ponuda za izradu asfalta").SetMargin(40).SetFontSize(28).SetTextAlignment(TextAlignment.CENTER);
            Document.Add(name);

            Paragraph description = new Paragraph();
            description.Add(new Tab());
            description.Add("U tabeli ispod je prikazana ponuda usluga izrade asfalta. Lokacija na kojoj bi se vrsila ugradnja asfalta je " + User.Location.Name + ". Tabela sadrzi razdaljinu do gradilista, kolicinu asfalta koja se ugradjuje, njihove cene, troskove transporta, ugradnje i emulzije ukoliko je potrebno:");
            Document.Add(description);

            String origin = "";
            foreach (var offer in offers)
            {
                if (offer.AsphaltBase.Location.Name.Trim().EndsWith('.'))
                    origin += String.Format("Asfalt {0} je dobavljan sa baze {1} ", offer.TypeOfAsphalt.Name, offer.AsphaltBase.Location.Name.Trim());
                else
                    origin += String.Format("Asfalt {0} je dobavljan sa baze {1}. ", offer.TypeOfAsphalt.Name, offer.AsphaltBase.Location.Name.Trim());
            }
            Paragraph originP = new Paragraph();
            originP.Add(new Tab());
            originP.Add(origin);
            Document.Add(originP);

            //Add table to document
            double total = 0;
            Document.Add(CreateContentTable(offers, ref total));

            //Add information text
            Paragraph terms = new Paragraph("- Ukupna cena izrade je "+ Constants.GetInvariantCultureString(total) + ".\n- Rok vazenja ovog dokumenta je 10 dana od njegove izrade.\n\nVise informacija u vezi isporuke na tel: +381 64 84 19 500, Nemanja Radojevic");
            Document.Add(terms);
            Paragraph date = new Paragraph("Napravljeno dana: " + DateTime.Now.ToString("dd/MM/yyyy"));
            date.SetFixedPosition(40, 80, 200);
            Document.Add(date);

            //Add footer
            Paragraph generalInfo = new Paragraph("Maticni broj: 06325114   PIB: 100420572   Website: www.gradcoop.rs").SetTextAlignment(TextAlignment.CENTER);
            generalInfo.SetFixedPosition(Document.GetLeftMargin(), Document.GetBottomMargin(), ps.GetWidth() - Document.GetLeftMargin() - Document.GetRightMargin());
            Document.Add(generalInfo);

            // Close document
            Document.Close();
        }

        private Table CreateContentTable(List<Offer> offers, ref double total)
        {
            Table content = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
            content.SetMarginLeft(40);
            content.SetMarginRight(40);
            content.SetMarginBottom(25);
            content.SetMarginTop(25);

            total = 0;

            content.SetTextAlignment(TextAlignment.RIGHT);

            content.AddHeaderCell(new Paragraph("Pozicija").SetBold());
            content.AddHeaderCell(new Paragraph("Kolicina").SetBold());
            content.AddHeaderCell(new Paragraph("Merna jed.").SetBold());
            content.AddHeaderCell(new Paragraph("Cena po jed.").SetBold());
            content.AddHeaderCell(new Paragraph("Ukupno").SetBold());


            Cell emptyCell = new Cell();
            emptyCell.SetPadding(0);
            emptyCell.SetBorder(Border.NO_BORDER);

            foreach (var offer in offers) {

                double asphalt = Math.Round(offer.GetAsphaltPrice(), 2);
                content.AddCell("Asfalt " + offer.TypeOfAsphalt.Name);
                content.AddCell(Constants.GetInvariantCultureString(Math.Round(offer.AsphaltWeight, 2)));
                content.AddCell("t");
                content.AddCell(Constants.GetInvariantCultureString(offer.AsphaltBase.GetPrice(offer.TypeOfAsphalt)) + " din/t");
                content.AddCell(Constants.GetInvariantCultureString(asphalt));

                double transport = Math.Round(User.PriceForDistance(offer.AsphaltBase), 2);
                content.AddCell("Transport");
                content.AddCell(Constants.GetInvariantCultureString(User.Distance[offer.AsphaltBase.Location]));
                content.AddCell("km");
                content.AddCell(Constants.GetInvariantCultureString(User.TransportPrice[offer.AsphaltBase])+" din/(km*t)");
                content.AddCell(Constants.GetInvariantCultureString(transport));

                double installation = Math.Round(offer.InstallationPrice * offer.AsphaltWeight, 2);
                content.AddCell("Ugradnja");
                content.AddCell(Constants.GetInvariantCultureString(offer.AsphaltWeight));
                content.AddCell("t");
                content.AddCell(Constants.GetInvariantCultureString(offer.InstallationPrice) + " din/t");
                content.AddCell(Constants.GetInvariantCultureString(installation));

                offer.TotalPrice = transport + asphalt + installation;
                total += offer.TotalPrice;

                content.AddCell(emptyCell);
                content.AddCell(emptyCell);
                content.AddCell(emptyCell);
                content.AddCell(emptyCell);
                content.AddCell(new Paragraph(Constants.GetInvariantCultureString(offer.TotalPrice)).SetBold());
            }
            if (User.Emulsion)
            {
                double emulsion = Math.Round(User.PriceForEmulsion(), 2);
                content.AddCell("Emulzija");
                content.AddCell(Constants.GetInvariantCultureString(User.EmulsionSurface()));
                content.AddCell("m2");
                content.AddCell(Constants.GetInvariantCultureString(Constants.EmulsionPrice) + " din/m2");
                content.AddCell(Constants.GetInvariantCultureString(emulsion));
                total += emulsion;
            }
            if (offers.Count > 1 || User.Emulsion)
            {
                content.AddCell(emptyCell);
                content.AddCell(emptyCell);
                content.AddCell(emptyCell);
                content.AddCell(emptyCell);
                content.AddCell(new Paragraph(Constants.GetInvariantCultureString(total)).SetBold());
            }
            return content;
        }

        private void AddTableHeaders(Table table, string[] headers) {
            foreach (string header in headers)
                table.AddHeaderCell(header).SetTextAlignment(TextAlignment.CENTER);
        }

        private void AddTableCells(Table table, string[] cellContents) {
            foreach (string cellContent in cellContents)
                table.AddCell(cellContent);
        }

        // Returns false if string is not standard font
        private bool SetDocumentFont(string standardFont) {

            if (!StandardFonts.IsStandardFont(standardFont))
                return false;
            PdfFont font = PdfFontFactory.CreateFont(standardFont);
            Document.SetFont(font);
            return true;
        }

        private Cell GetImageFromAsset(string fileName) {
            AssetManager assets = Application.Context.Assets;
            var bytes = default(byte[]);
            using (StreamReader reader = new StreamReader(assets.Open(fileName)))
            {
                using (var memstream = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(memstream);
                    bytes = memstream.ToArray();
                }
            }

            Image image = new Image(ImageDataFactory.Create(bytes));

            Cell cell = new Cell().Add(image);
            cell.SetPadding(0);
            cell.SetBorder(Border.NO_BORDER);
            return cell;
        }

        private string GenerateFileName(string baseName) {

            var filename =  baseName.Replace(' ', '_') + "_";
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rand = new Random();

            for (int i = 0; i < 5; i++)
            {
                filename += chars[rand.Next(0, chars.Length - 1)];
            }
            filename += "_";
            filename += DateTime.Now.ToString("dd-MM-yyyy");

            return filename;
        }

    }
}