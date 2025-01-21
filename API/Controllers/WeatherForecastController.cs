using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WeatherForecastController : ControllerBase
    {
        [Route("/wsMB")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket connection established");

                try
                {
                    await HandleWebSocketAsync(webSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        public static async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            // Khởi tạo dữ liệu gốc sử dụng model
            var data = new DataMB
            {
                Date = "20/01/2025",
                Type = "XSMB",
                Datas = new List<DataEntry>
        {
            new DataEntry
            {
                LocationCode = "XSMB",
                LocationName = "",
                Loto_top = Enumerable.Range(0, 10).Select(i => new LotoData { top_number = i, last_number = "" }).ToList(),
                Loto_last = Enumerable.Range(0, 10).Select(i => new LotoLast { last_number = i, top_number = "" }).ToList(),
                DataPrize = Enumerable.Range(0, 9).Select(i => new DataPrize
                {
                    Number = i,
                    Text = $"prize_{i}",
                    Data = new List<PrizeData>()
                }).ToList()
            }
        }
            };

            var rng = new Random();

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    foreach (var loto in data.Datas[0].Loto_top)
                    {
                        loto.last_number = rng.Next(0, 2) == 0
                            ? "" // 50% khả năng là chuỗi rỗng
                            : string.Join(",", Enumerable.Range(0, rng.Next(1, 4)).Select(_ => rng.Next(0, 10))); // Hoặc danh sách số
                    }

                    foreach (var loto in data.Datas[0].Loto_last)
                    {
                        loto.top_number = rng.Next(0, 2) == 0
                            ? "" // 50% khả năng là chuỗi rỗng
                            : string.Join(",", Enumerable.Range(0, rng.Next(1, 4)).Select(_ => rng.Next(0, 10))); // Hoặc danh sách số
                    }
                    // Cập nhật dữ liệu động cho các giải
                    foreach (var prize in data.Datas[0].DataPrize)
                    {
                        int numberOfValues = prize.Number switch
                        {
                            2 => 2, // Giải 2 có 2 số
                            3 => 6, // Giải 3 có 6 số
                            4 => 4, // Giải 4 có 4 số
                            5 => 6, // Giải 5 có 6 số
                            6 => 3, // Giải 6 có 3 số
                            7 => 4, // Giải 7 có 4 số
                            _ => 1  // Các giải khác (prize_8, prize_special) chỉ có 1 số
                        };

                        // Tạo danh sách các giá trị ngẫu nhiên
                        prize.Data = Enumerable.Range(1, numberOfValues).Select(keyId => new PrizeData
                        {
                            keyId = keyId,
                            value = rng.Next(0, 2) == 0
                                ? "" // 50% khả năng là chuỗi rỗng
                                : rng.Next(1000, 9999).ToString() // Hoặc một số ngẫu nhiên 4 chữ số
                        }).ToList();
                    }

                    // Chuyển dữ liệu thành JSON
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented);

                    // Gửi dữ liệu qua WebSocket
                    var buffer = Encoding.UTF8.GetBytes(json);
                    var segment = new ArraySegment<byte>(buffer);

                    try
                    {
                        await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch
                    {
                        // Thoát vòng lặp nếu xảy ra lỗi khi gửi
                        break;
                    }

                    // Đợi 5 giây trước khi gửi tiếp
                    await Task.Delay(5000);
                }
            }
            finally
            {
                // Đảm bảo WebSocket được đóng khi kết nối không còn mở
                if (webSocket.State != WebSocketState.Closed)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }

                Console.WriteLine("WebSocket connection closed");
            }
        }



    }

}
