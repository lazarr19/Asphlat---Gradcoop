using System;
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
    public static class PDF
    {


        public static void CreatePDF(AsphaltBase asphaltBase)
        {
            //"/storage/emulated/0/Download"

            var downloadsPath = "/storage/emulated/0/Download";
            var filename = GenerateFileName() + ".pdf";
            var filePath = Path.Combine(downloadsPath, filename);

            var writer = new PdfWriter(filePath);
            var pdfDoc = new PdfDocument(writer);
            var document = new Document(pdfDoc);
            var ps = pdfDoc.GetDefaultPageSize();

            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            document.SetFont(font);

            //Create header of document
            Table header = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            header.AddCell(GetImageFromAsset("logo.png"));
            header.AddCell(new Paragraph("GRADCOOP d.o.o.\nKaradjordjeva 120, Umka, 11260 Beograd, Srbija\nTel: +381 11 8026 720\n +381 11 8025 020\nEmail: gradcoop.1@gmail.com").SetTextAlignment(TextAlignment.RIGHT));
            document.Add(header);

            // Add text
            Paragraph name = new Paragraph("Ponuda za izradu asfalta").SetMargin(40).SetFontSize(28).SetTextAlignment(TextAlignment.CENTER);
            document.Add(name);

            Paragraph description = new Paragraph();
            description.Add(new Tab());
            description.Add("U tabeli ispod je prikazana ponuda usluga izrade asfalta. Lokacija na kojoj bi se vrsila ugradnja asfalta je " + User.Location.Name + ". Tabela sadrzi razdaljinu od asfaltne baze " + asphaltBase.Location.Name +" do gradilista, kolicinu asfalta koja se ugradjuje kao i njihove cene:");
            document.Add(description);

            var distance = User.Distance[asphaltBase.Location];
            var distancePrice = Constants.PriceForDistance(distance);
            var asphlatPrice = User.AsphaltWeight * asphaltBase.GetPrice(User.TypeOfAsphalt);

            //Add table to document
            Table content = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth();
            content.SetMargin(40);

            content.AddHeaderCell("Naziv").SetTextAlignment(TextAlignment.CENTER);
            content.AddHeaderCell("Kolicina").SetTextAlignment(TextAlignment.CENTER);
            content.AddHeaderCell("Merna jed.").SetTextAlignment(TextAlignment.CENTER);
            content.AddHeaderCell("Cena/din").SetTextAlignment(TextAlignment.CENTER);

            content.AddCell("Razdaljina");
            content.AddCell(Constants.GetInvariantCultureString(distance));
            content.AddCell("km");
            content.AddCell(Constants.GetInvariantCultureString(Math.Round(distancePrice)));

            content.AddCell("Asfalt");
            content.AddCell(Constants.GetInvariantCultureString(Math.Round(User.AsphaltWeight,2)));
            content.AddCell("t");
            content.AddCell(Constants.GetInvariantCultureString(asphlatPrice));

            Cell emptyCell = new Cell();
            emptyCell.SetPadding(0);
            emptyCell.SetBorder(Border.NO_BORDER);
            content.AddCell(emptyCell);
            content.AddCell(emptyCell);
            content.AddCell(emptyCell);
            content.AddCell(Constants.GetInvariantCultureString(Math.Round(asphlatPrice + distancePrice, 2)));

            document.Add(content);

            //Add information text
            Paragraph terms = new Paragraph("- Rok vazenja ovog dokumenta je 30 dana od njegove izrade.\n\nVise informacija u vezi isporuke na tel: +381 64 84 19 500, Nemanja Radojevic\nPonuda izdata od strane GRADCOOP d.o.o.");
            document.Add(terms);
            Paragraph date = new Paragraph("Napravljeno dana: " + DateTime.Now.ToString("dd/MM/yyyy"));
            date.SetFixedPosition(40, 80, 200);
            document.Add(date);

            //Add footer
            Paragraph generalInfo = new Paragraph("Maticni broj: 06325114   PIB: 100420572   Website: www.gradcoop.rs").SetTextAlignment(TextAlignment.CENTER);
            generalInfo.SetFixedPosition(document.GetLeftMargin(), document.GetBottomMargin(), ps.GetWidth() - document.GetLeftMargin() - document.GetRightMargin());
            document.Add(generalInfo);

            // Close document
            document.Close();
        }

        private static Cell GetImageFromAsset(string fileName) {
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

        private static string GenerateFileName() {

            var filename = "Ponuda" + User.Location.Name.Replace(' ', '_') + "_";
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