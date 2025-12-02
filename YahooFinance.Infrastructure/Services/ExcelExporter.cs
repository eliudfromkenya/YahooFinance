using OfficeOpenXml;
using OfficeOpenXml.Style;
using YahooFinance.Core.Abstractions;
using YahooFinance.Core.Models;

namespace YahooFinance.Infrastructure.Services;

public sealed class ExcelExporter : IExcelExporter
{
    public async Task ExportAsync(IEnumerable<HistoricalQuote> quotes, string filePath, CancellationToken cancellationToken)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("History");

        ws.Cells[1, 1].Value = "Date";
        ws.Cells[1, 2].Value = "Open";
        ws.Cells[1, 3].Value = "High";
        ws.Cells[1, 4].Value = "Low";
        ws.Cells[1, 5].Value = "Close";
        ws.Cells[1, 6].Value = "Adj Close";
        ws.Cells[1, 7].Value = "Volume";

        int row = 2;
        foreach (var q in quotes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ws.Cells[row, 1].Value = q.Date;
            ws.Cells[row, 1].Style.Numberformat.Format = "yyyy-mm-dd";
            ws.Cells[row, 2].Value = q.Open;
            ws.Cells[row, 3].Value = q.High;
            ws.Cells[row, 4].Value = q.Low;
            ws.Cells[row, 5].Value = q.Close;
            ws.Cells[row, 6].Value = q.AdjustedClose;
            ws.Cells[row, 7].Value = q.Volume;
            row++;
        }

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await package.SaveAsAsync(fs, cancellationToken).ConfigureAwait(false);
    }
}

