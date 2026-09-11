using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalFinanceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AccountTypeTranslations",
                newName: "Translation");

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "Id", "CanDeleteDirectly", "Category", "NormalBalance" },
                values: new object[,]
                {
                    { 1, true, 1, 1 },
                    { 2, true, 2, 2 },
                    { 3, false, 3, 3 },
                    { 4, false, 4, 3 },
                    { 5, false, 5, 1 },
                    { 6, false, 7, 3 },
                    { 7, false, 6, 3 }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code", "DecimalPlaces", "DisplayOrder", "IsActive", "Name", "Symbol" },
                values: new object[,]
                {
                    { 1, "USD", (byte)2, 1, true, "US Dollar", "$" },
                    { 2, "EUR", (byte)2, 2, true, "Euro", "€" },
                    { 3, "IRR", (byte)0, 3, true, "Iranian Rial", "﷼" },
                    { 4, "TRY", (byte)2, 4, true, "Turkish Lira", "₺" },
                    { 5, "AED", (byte)2, 5, true, "UAE Dirham", "د.إ" },
                    { 6, "GBP", (byte)2, 6, true, "British Pound", "£" },
                    { 7, "CAD", (byte)2, 7, true, "Canadian Dollar", "C$" },
                    { 8, "AUD", (byte)2, 8, true, "Australian Dollar", "A$" },
                    { 9, "CHF", (byte)2, 9, true, "Swiss Franc", "CHF" },
                    { 10, "JPY", (byte)0, 10, true, "Japanese Yen", "¥" },
                    { 11, "CNY", (byte)2, 11, true, "Chinese Yuan", "¥" },
                    { 12, "SEK", (byte)2, 12, true, "Swedish Krona", "kr" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "DisplayOrder", "IsActive", "IsRightToLeft", "Name" },
                values: new object[,]
                {
                    { 1, "en", 1, true, false, "English" },
                    { 2, "fa", 2, true, true, "فارسی" },
                    { 3, "tr", 3, true, false, "Türkçe" },
                    { 4, "fr", 4, true, false, "Français" },
                    { 5, "de", 5, true, false, "Deutsch" },
                    { 6, "ar", 6, true, true, "العربية" },
                    { 7, "es", 7, true, false, "Español" },
                    { 8, "it", 8, true, false, "Italiano" },
                    { 9, "ru", 9, true, false, "Русский" },
                    { 10, "zh", 10, true, false, "中文" },
                    { 11, "ja", 11, true, false, "日本語" },
                    { 12, "sv", 12, true, false, "Svenska" }
                });

            migrationBuilder.InsertData(
                table: "AccountTypeTranslations",
                columns: new[] { "Id", "AccountTypeId", "Description", "LanguageId", "Translation" },
                values: new object[,]
                {
                    { 1, 1, null, 1, "Expense" },
                    { 2, 1, null, 2, "هزینه" },
                    { 3, 1, null, 3, "Gider" },
                    { 4, 1, null, 4, "Dépense" },
                    { 5, 1, null, 5, "Aufwand" },
                    { 6, 1, null, 6, "مصروف" },
                    { 7, 1, null, 7, "Gasto" },
                    { 8, 1, null, 8, "Spesa" },
                    { 9, 1, null, 9, "Расход" },
                    { 10, 1, null, 10, "费用" },
                    { 11, 1, null, 11, "費用" },
                    { 12, 1, null, 12, "Kostnad" },
                    { 13, 2, null, 1, "Income" },
                    { 14, 2, null, 2, "درآمد" },
                    { 15, 2, null, 3, "Gelir" },
                    { 16, 2, null, 4, "Revenu" },
                    { 17, 2, null, 5, "Ertrag" },
                    { 18, 2, null, 6, "إيراد" },
                    { 19, 2, null, 7, "Ingreso" },
                    { 20, 2, null, 8, "Entrata" },
                    { 21, 2, null, 9, "Доход" },
                    { 22, 2, null, 10, "收入" },
                    { 23, 2, null, 11, "収益" },
                    { 24, 2, null, 12, "Intäkt" },
                    { 25, 3, null, 1, "Person" },
                    { 26, 3, null, 2, "شخص" },
                    { 27, 3, null, 3, "Kişi" },
                    { 28, 3, null, 4, "Personne" },
                    { 29, 3, null, 5, "Person" },
                    { 30, 3, null, 6, "شخص" },
                    { 31, 3, null, 7, "Persona" },
                    { 32, 3, null, 8, "Persona" },
                    { 33, 3, null, 9, "Контрагент" },
                    { 34, 3, null, 10, "个人" },
                    { 35, 3, null, 11, "個人" },
                    { 36, 3, null, 12, "Person" },
                    { 37, 4, null, 1, "Bank Account" },
                    { 38, 4, null, 2, "حساب بانکی" },
                    { 39, 4, null, 3, "Banka Hesabı" },
                    { 40, 4, null, 4, "Compte bancaire" },
                    { 41, 4, null, 5, "Bankkonto" },
                    { 42, 4, null, 6, "حساب مصرفي" },
                    { 43, 4, null, 7, "Cuenta bancaria" },
                    { 44, 4, null, 8, "Conto bancario" },
                    { 45, 4, null, 9, "Банковский счет" },
                    { 46, 4, null, 10, "银行账户" },
                    { 47, 4, null, 11, "銀行口座" },
                    { 48, 4, null, 12, "Bankkonto" },
                    { 49, 5, null, 1, "Cash" },
                    { 50, 5, null, 2, "حساب نقدی" },
                    { 51, 5, null, 3, "Kasa" },
                    { 52, 5, null, 4, "Caisse" },
                    { 53, 5, null, 5, "Kasse" },
                    { 54, 5, null, 6, "نقدية" },
                    { 55, 5, null, 7, "Caja" },
                    { 56, 5, null, 8, "Cassa" },
                    { 57, 5, null, 9, "Касса" },
                    { 58, 5, null, 10, "现金" },
                    { 59, 5, null, 11, "現金" },
                    { 60, 5, null, 12, "Kassa" },
                    { 61, 6, null, 1, "Currency Exchange Clearing" },
                    { 62, 6, null, 2, "تسویه تبدیل ارز" },
                    { 63, 6, null, 3, "Döviz Değişim Takas" },
                    { 64, 6, null, 4, "Compte de compensation de change" },
                    { 65, 6, null, 5, "Währungsumrechnungs-Verrechnung" },
                    { 66, 6, null, 6, "تسوية صرف العملات" },
                    { 67, 6, null, 7, "Compensación de cambio de divisas" },
                    { 68, 6, null, 8, "Compensazione cambio valuta" },
                    { 69, 6, null, 9, "Расчетный счет обмена валют" },
                    { 70, 6, null, 10, "货币兑换结算" },
                    { 71, 6, null, 11, "為替交換精算" },
                    { 72, 6, null, 12, "Valutaväxlingsavräkning" },
                    { 73, 7, null, 1, "Opening Balance Equity" },
                    { 74, 7, null, 2, "حقوق صاحبان سهام افتتاحیه" },
                    { 75, 7, null, 3, "Açılış Bakiyesi Özsermayesi" },
                    { 76, 7, null, 4, "Capitaux propres du solde d'ouverture" },
                    { 77, 7, null, 5, "Eigenkapital der Eröffnungsbilanz" },
                    { 78, 7, null, 6, "حقوق الملكية للرصيد الافتتاحي" },
                    { 79, 7, null, 7, "Patrimonio del saldo inicial" },
                    { 80, 7, null, 8, "Patrimonio netto del saldo iniziale" },
                    { 81, 7, null, 9, "Капитал начального остатка" },
                    { 82, 7, null, 10, "期初余额权益" },
                    { 83, 7, null, 11, "開始残高資本" },
                    { 84, 7, null, 12, "Eget kapital för ingående balans" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "AccountTypeTranslations",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.RenameColumn(
                name: "Translation",
                table: "AccountTypeTranslations",
                newName: "Name");
        }
    }
}
