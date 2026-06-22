using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HospitalManagementSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RoomNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CitizenshipNumber = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PatientId = table.Column<int>(type: "INTEGER", nullable: false),
                    DoctorId = table.Column<int>(type: "INTEGER", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "Email", "FullName", "Phone", "RoomNumber", "Specialization" },
                values: new object[,]
                {
                    { 1, null, "Dr. Mehmet Öz", "05551112233", "K-02", "Kardiyoloji" },
                    { 2, null, "Op. Dr. Ayşe Yılmaz", "05552223344", "G-05", "Genel Cerrahi" },
                    { 3, null, "Uzm. Dr. Kemal Sunal", "05553334455", "D-10", "Dahiliye" },
                    { 4, null, "Op. Dr. Cüneyt Arkın", "05554445566", "O-01", "Ortopedi" },
                    { 5, null, "Dr. Fatma Girik", "05555556677", "C-08", "Çocuk Hastalıkları" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "CitizenshipNumber", "CreatedAt", "DateOfBirth", "Email", "FullName", "Gender", "Phone" },
                values: new object[,]
                {
                    { 1, null, "12345678901", new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1980, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ahmet Yılmaz", "Erkek", "05321112233" },
                    { 2, null, "23456789012", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1992, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Fatma Şahin", "Kadın", "05322223344" },
                    { 3, null, "34567890123", new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1975, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ali Koç", "Erkek", "05323334455" },
                    { 4, null, "45678901234", new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1988, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ayşe Demir", "Kadın", "05324445566" },
                    { 5, null, "56789012345", new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1960, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mustafa Çelik", "Erkek", "05325556677" },
                    { 6, null, "67890123456", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Zeynep Kaya", "Kadın", "05326667788" },
                    { 7, null, "78901234567", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1995, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mehmet Can", "Erkek", "05327778899" },
                    { 8, null, "89012345678", new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1985, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Elif Yıldırım", "Kadın", "05328889900" },
                    { 9, null, "90123456789", new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1970, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Hasan Aydın", "Erkek", "05329990011" },
                    { 10, null, "01234567890", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Sibel Ak", "Kadın", "05320001122" }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentDate", "DoctorId", "Notes", "PatientId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 1, "Onaylandı" },
                    { 2, new DateTime(2026, 3, 11, 14, 0, 0, 0, DateTimeKind.Unspecified), 2, null, 2, "Onaylandı" },
                    { 3, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 3, "İptal" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
