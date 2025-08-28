using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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
}
