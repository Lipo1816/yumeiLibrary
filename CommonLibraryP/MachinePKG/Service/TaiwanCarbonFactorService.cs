using DevExpress.Pdf;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

using System.Net.Http;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;


public class TaiwanCarbonFactorService
{
    private readonly HttpClient _http;

    public TaiwanCarbonFactorService(HttpClient http) => _http = http;


    public async Task<(double value, int year, string name)> GetLatestAsync(
        string apiKey,
        string typeHint = "電力間接碳足跡")
    {
        //var url = $"{BaseUrl}&filters=name,like,{Uri.EscapeDataString(typeHint)}&api_key={apiKey}";
        var url =" https://data.moenv.gov.tw/api/v2/cfp_p_02?format=JSON&limit=1&sort=AnnouncementYear%20desc&api_key=f9bda5d4-e3a1-4634-8ef4-85e0fd619bfa";// $"{BaseUrl}&filters=name,eq,{Uri.EscapeDataString(typeHint)}&api_key={apiKey}";
        using var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();


        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        try
        {
            var records = doc.RootElement.GetProperty("records").EnumerateArray().ToList();
            if (records.Count == 0)
                throw new Exception("查無符合條件的碳排係數資料（records 為空）");

            var rec = records[0];
            var name = rec.GetProperty("name").GetString() ?? "";
            var coe = double.Parse(rec.GetProperty("coe").GetString() ?? "0");
            var year = int.Parse(rec.GetProperty("announcementyear").GetString() ?? "-1");

            return (coe, year, name);
        }
        catch (Exception ex)
        {
            // 可加上 log 或自訂錯誤訊息
            throw new Exception("取得碳排係數失敗：" + ex.Message, ex);
        }
    }

    public  async Task<double> GetTaiwanGridEF_FromMOEAAsync()
    {
        // 直接用 113 年度示例；實務上先抓清單頁找最新檔案連結
        try
        {
            var pdfUrl = "https://www.moeaea.gov.tw/ecw/populace/content/wHandMenuFile.ashx?file_id=16728";

            using var pdfBytes = await _http.GetStreamAsync(pdfUrl);
            using var mem = new MemoryStream(); await pdfBytes.CopyToAsync(mem);
            mem.Position = 0;

            using var doc = UglyToad.PdfPig.PdfDocument.Open(mem);
            var allText = string.Join("\n", doc.GetPages().Select(p => p.Text));
            var m = Regex.Match(allText, @"(\d+(\.\d+)?)\s*公斤\s*CO2e\/度");
            if (!m.Success) throw new Exception("未在 PDF 內找到係數");
            return double.Parse(m.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture); // 0.474
        }
        catch (Exception ex)
        {

            throw;
        }
    }

}
