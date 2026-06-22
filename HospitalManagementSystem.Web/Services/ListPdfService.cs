using HospitalManagementSystem.Web.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HospitalManagementSystem.Web.Services
{
    public static class ListPdfService
    {
        static ListPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        private static void ComposeHeader(IContainer c, string title)
        {
            c.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item().Text("MediPanel")
                            .FontSize(22)
                            .Bold()
                            .FontColor("#3b82f6");

                        inner.Item().Text(title)
                            .FontSize(11)
                            .FontColor(Colors.Grey.Darken2);
                    });

                    row.ConstantItem(160).AlignRight().Column(inner =>
                    {
                        inner.Item().AlignRight()
                            .Text($"Tarih: {DateTime.Now:dd.MM.yyyy}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Medium);

                        inner.Item().AlignRight()
                            .Text($"Saat: {DateTime.Now:HH:mm}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Medium);
                    });
                });

                col.Item()
                    .PaddingTop(8)
                    .LineHorizontal(1)
                    .LineColor("#3b82f6");
            });
        }

        private static void ComposeFooter(IContainer c)
        {
            c.AlignCenter().Text(txt =>
            {
                txt.Span("MediPanel Sistemi  ·  ")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Medium);

                txt.CurrentPageNumber()
                    .FontSize(9)
                    .FontColor(Colors.Grey.Medium);

                txt.Span(" / ")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Medium);

                txt.TotalPages()
                    .FontSize(9)
                    .FontColor(Colors.Grey.Medium);
            });
        }

        public static byte[] ExportPatients(List<Patient> patients)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(ts => ts.FontFamily("Arial").FontSize(11));

                    page.Header().Element(c => ComposeHeader(c, "Tüm Hastalar Listesi"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);
                            cols.RelativeColumn();
                            cols.ConstantColumn(100);
                            cols.ConstantColumn(100);
                            cols.ConstantColumn(80);
                        });

                        foreach (var h in new[] { "ID", "Ad Soyad", "TC Kimlik No", "Telefon", "Cinsiyet" })
                            table.Cell().Background("#eff6ff").Padding(6).Text(h).SemiBold().FontSize(10);

                        foreach (var p in patients)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Id.ToString()).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.FullName).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.CitizenshipNumber).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Phone).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Gender).FontSize(10);
                        }
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        public static byte[] ExportDoctors(List<Doctor> doctors)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(ts => ts.FontFamily("Arial").FontSize(11));

                    page.Header().Element(c => ComposeHeader(c, "Doktor Listesi"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.ConstantColumn(100);
                            cols.ConstantColumn(60);
                        });

                        foreach (var h in new[] { "Ad Soyad", "Uzmanlık Alanı", "Telefon", "Oda" })
                            table.Cell().Background("#eff6ff").Padding(6).Text(h).SemiBold().FontSize(10);

                        foreach (var d in doctors)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(d.FullName).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(d.Specialization).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(d.Phone).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(d.RoomNumber).FontSize(10);
                        }
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        public static byte[] ExportAppointments(List<Appointment> appointments)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(40);
                    page.DefaultTextStyle(ts => ts.FontFamily("Arial").FontSize(11));

                    page.Header().Element(c => ComposeHeader(c, "Randevu Listesi"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(110);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.ConstantColumn(80);
                        });

                        foreach (var h in new[] { "Tarih/Saat", "Hasta", "Doktor", "Sebep", "Durum" })
                            table.Cell().Background("#eff6ff").Padding(6).Text(h).SemiBold().FontSize(10);

                        foreach (var a in appointments)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(a.AppointmentDate.ToString("dd.MM.yyyy HH:mm")).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(a.Patient?.FullName ?? "-").FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(a.Doctor?.FullName ?? "-").FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(a.Reason ?? "-").FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(a.Status).FontSize(10);
                        }
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }
    }
}