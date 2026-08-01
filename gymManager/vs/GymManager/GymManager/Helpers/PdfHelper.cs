using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using iText.Layout.Borders;


namespace GymManager.Helpers
{
    public class PdfHelper : IDisposable
    {
        protected PdfWriter Writer;
        protected PdfDocument Pdf;
        protected Document Documento;

        protected PdfFont FonteNormal;
        protected PdfFont FonteNegrito;

        public PdfHelper(string caminho)
        {
            Writer = new PdfWriter(caminho);

            Pdf = new PdfDocument(Writer);

            Documento = new Document(Pdf, PageSize.A4);

            Documento.SetMargins(40, 40, 40, 40);

            FonteNormal =
                PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            FonteNegrito =
                PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        }

        public void Titulo(string texto)
        {
            Paragraph p = new Paragraph(texto);

            p.SetFont(FonteNegrito);

            p.SetFontSize(22);

            p.SetFontColor(new DeviceRgb(30, 64, 175));

            p.SetTextAlignment(TextAlignment.CENTER);

            p.SetMarginBottom(20);

            Documento.Add(p);
        }

        public void SubTitulo(string texto)
        {
            Paragraph p = new Paragraph(texto);

            p.SetFont(FonteNegrito);

            p.SetFontSize(15);

            p.SetMarginTop(10);

            p.SetMarginBottom(10);

            Documento.Add(p);
        }

        public void Texto(string texto)
        {
            Paragraph p = new Paragraph(texto);

            p.SetFont(FonteNormal);

            p.SetFontSize(11);

            Documento.Add(p);
        }

        public void Campo(string nome, string valor)
        {
            Paragraph p = new Paragraph();

            p.Add(
                new Text(nome + ": ")
                .SetFont(FonteNegrito));

            p.Add(
                new Text(string.IsNullOrWhiteSpace(valor) ? "-" : valor)
                .SetFont(FonteNormal));

            p.SetMarginBottom(6);

            Documento.Add(p);
        }
        public void Linha()
        {
            Table t = new Table(1);

            t.SetWidth(UnitValue.CreatePercentValue(100));

            Cell c = new Cell();

            c.SetHeight(1);

            c.SetBorder(null);

            c.SetBackgroundColor(ColorConstants.LIGHT_GRAY);

            t.AddCell(c);

            Documento.Add(t);
        }

        public void Espaco(float altura = 10)
        {
            Paragraph p = new Paragraph("");

            p.SetMarginBottom(altura);

            Documento.Add(p);
        }

        public void Fechar()
        {
            Documento.Close();
        }

        public void Dispose()
        {
            Documento?.Close();
            Pdf?.Close();
            Writer?.Close();
        }
        public Table CriarTabela(params float[] larguras)
        {
            Table tabela =
                new Table(UnitValue.CreatePercentArray(larguras));

            tabela.SetWidth(
                UnitValue.CreatePercentValue(100));

            tabela.SetMarginTop(10);

            tabela.SetMarginBottom(15);

            return tabela;
        }
        public void AdicionarCabecalhoTabela(
    Table tabela,
    params string[] colunas)
        {
            foreach (string coluna in colunas)
            {
                Cell cell = new Cell();

                cell.SetBackgroundColor(
                    new DeviceRgb(30, 64, 175));

                cell.SetPadding(8);

                cell.SetBorder(Border.NO_BORDER);

                Paragraph p =
                    new Paragraph(coluna);

                p.SetFont(FonteNegrito);

                p.SetFontColor(ColorConstants.WHITE);

                p.SetTextAlignment(TextAlignment.CENTER);

                cell.Add(p);

                tabela.AddHeaderCell(cell);
            }
        }
        public void AdicionarLinhaTabela(
    Table tabela,
    params string[] valores)
        {
            foreach (string valor in valores)
            {
                Cell cell = new Cell();

                cell.SetPadding(6);

                cell.SetBorder(
                    new SolidBorder(
                        ColorConstants.LIGHT_GRAY,
                        0.5f));

                Paragraph p =
                    new Paragraph(valor);

                p.SetFont(FonteNormal);

                p.SetFontSize(10);

                cell.Add(p);

                tabela.AddCell(cell);
            }
        }
        public void AdicionarTabela(
    Table tabela)
        {
            Documento.Add(tabela);
        }
        public void CaixaInformacao(
    string titulo,
    string valor)
        {
            Table tabela =
                new Table(1);

            tabela.SetWidth(
                UnitValue.CreatePercentValue(100));

            Cell cell =
                new Cell();

            cell.SetPadding(10);

            cell.SetBorder(
                new SolidBorder(
                    new DeviceRgb(220, 220, 220),
                    1));

            cell.SetBackgroundColor(
                new DeviceRgb(248, 250, 252));

            Paragraph p1 =
                new Paragraph(titulo);

            p1.SetFont(FonteNegrito);

            p1.SetFontSize(11);

            p1.SetFontColor(
                new DeviceRgb(30, 64, 175));

            cell.Add(p1);

            Paragraph p2 =
                new Paragraph(valor);

            p2.SetFont(FonteNormal);

            p2.SetFontSize(12);

            cell.Add(p2);

            tabela.AddCell(cell);

            Documento.Add(tabela);
        }
    }
}