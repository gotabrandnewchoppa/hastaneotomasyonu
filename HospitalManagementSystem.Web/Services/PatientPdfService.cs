using HospitalManagementSystem.Web.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HospitalManagementSystem.Web.Services
{
    /// <summary>
    /// Generates a PDF patient report using QuestPDF.
    /// </summary>
    public static class PatientPdfService
    {
        static PatientPdfService()
        {
            // Community license – free for open-source / educational projects
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static byte[] Generate(
            Patient patient,
            IEnumerable<Appointment> appointments,
            IEnumerable<LabReport> labReports)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(ts => ts.FontFamily("Arial").FontSize(11));

                    page.Header().Element(ComposeHeader);

                    page.Content().Column(col =>
                    {
                        col.Spacing(16);

                        // ── Patient Info ──────────────────────────────────
                        col.Item().Element(c => ComposePatientInfo(c, patient));

                        // ── Appointment History ───────────────────────────
                        col.Item().Element(c => ComposeAppointments(c, appointments));

                        // ── Lab Reports ───────────────────────────────────
                        col.Item().Element(c => ComposeLabReports(c, labReports));
                    });

                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("MediPanel – Gizli Tıbbi Belge  ·  ").FontSize(9).FontColor(Colors.Grey.Medium);
                        txt.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                        txt.Span(" / ").FontSize(9).FontColor(Colors.Grey.Medium);
                        txt.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        // ── Header ────────────────────────────────────────────────────────────
        private static void ComposeHeader(IContainer c)
        {
            c.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("MediPanel").FontSize(22).Bold().FontColor("#3b82f6");
                    col.Item().Text("Hastane Yönetim Sistemi – Hasta Raporu")
                       .FontSize(11).FontColor(Colors.Grey.Darken2);
                });
                row.ConstantItem(160).AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Text($"Tarih: {DateTime.Now:dd.MM.yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
                    col.Item().AlignRight().Text($"Saat:  {DateTime.Now:HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });

            c.PaddingTop(8).LineHorizontal(1).LineColor("#3b82f6");
        }

        // ── Patient Info Table ─────────────────────────────────────────────────
        private static void ComposePatientInfo(IContainer c, Patient p)
        {
            c.Column(col =>
            {
                col.Item().Text("Hasta Bilgileri").Bold().FontSize(13).FontColor("#1e293b");
                col.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(160);
                        cols.RelativeColumn();
                        cols.ConstantColumn(160);
                        cols.RelativeColumn();
                    });

                    void AddRow(string label1, string val1, string label2, string val2)
                    {
                        table.Cell().Padding(5).Text(label1).SemiBold().FontColor(Colors.Grey.Darken2);
                        table.Cell().Padding(5).Text(val1);
                        table.Cell().Padding(5).Text(label2).SemiBold().FontColor(Colors.Grey.Darken2);
                        table.Cell().Padding(5).Text(val2);
                    }

                    AddRow("Ad Soyad:", p.FullName, "TC Kimlik No:", p.CitizenshipNumber);
                    AddRow("Cinsiyeti:", p.Gender, "Doğum Tarihi:", p.DateOfBirth.ToString("dd.MM.yyyy"));
                    AddRow("Telefon:", p.Phone, "E-Posta:", p.Email ?? "-");
                    AddRow("Adres:", p.Address ?? "-", "Kayıt Tarihi:", p.CreatedAt.ToString("dd.MM.yyyy"));
                });
            });
        }

        // ── Appointments Table ─────────────────────────────────────────────────
        private static void ComposeAppointments(IContainer c, IEnumerable<Appointment> appointments)
        {
            var list = appointments.ToList();
            c.Column(col =>
            {
                col.Item().Text("Randevu Geçmişi").Bold().FontSize(13).FontColor("#1e293b");
                col.Item().Text($"Toplam {list.Count} randevu").FontSize(9).FontColor(Colors.Grey.Medium);

                if (list.Count == 0)
                {
                    col.Item().PaddingTop(4).Text("Kayıtlı randevu bulunmuyor.").FontColor(Colors.Grey.Medium);
                    return;
                }

                col.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(110);
                        cols.RelativeColumn();
                        cols.ConstantColumn(90);
                    });

                    // Header
                    foreach (var h in new[] { "Tarih", "Doktor", "Durum" })
                        table.Cell().Background("#eff6ff").Padding(6).Text(h).SemiBold().FontSize(10);

                    foreach (var a in list)
                    {
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(5).Text(a.AppointmentDate.ToString("dd.MM.yyyy HH:mm")).FontSize(10);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(5).Text(a.Doctor?.FullName ?? "-").FontSize(10);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(5).Text(a.Status).FontSize(10);
                    }
                });
            });
        }

        // ── Lab Reports Table ──────────────────────────────────────────────────
        private static void ComposeLabReports(IContainer c, IEnumerable<LabReport> labs)
        {
            var list = labs.ToList();
            c.Column(col =>
            {
                col.Item().Text("Lab Raporları").Bold().FontSize(13).FontColor("#1e293b");
                col.Item().Text($"Toplam {list.Count} dosya").FontSize(9).FontColor(Colors.Grey.Medium);

                if (list.Count == 0)
                {
                    col.Item().PaddingTop(4).Text("Kayıtlı dosya bulunmuyor.").FontColor(Colors.Grey.Medium);
                    return;
                }

                col.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.ConstantColumn(70);
                        cols.ConstantColumn(110);
                    });

                    foreach (var h in new[] { "Dosya Adı", "Açıklama", "Boyut", "Yükleme Tarihi" })
                        table.Cell().Background("#eff6ff").Padding(6).Text(h).SemiBold().FontSize(10);

                    foreach (var l in list)
                    {
                        var size = l.FileSizeBytes >= 1024 * 1024
                            ? $"{l.FileSizeBytes / (1024.0 * 1024):F1} MB"
                            : $"{l.FileSizeBytes / 1024.0:F0} KB";

                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(5).Text(l.OriginalName).FontSize(10);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(5).Text(l.Description ?? "-").FontSize(10);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(5).Text(size).FontSize(10);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                             .Padding(5).Text(l.UploadedAt.ToString("dd.MM.yyyy HH:mm")).FontSize(10);
                    }
                });
            });
        }
    }
}
