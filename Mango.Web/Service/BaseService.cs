using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.utility.SD;

namespace Mango.Web.Service
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public BaseService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
		{
			// Check if the URI is valid before proceeding
			if (!Uri.TryCreate(requestDto.Url, UriKind.Absolute, out Uri uri))
			{
				// Handle the case where the URI is not valid
				Console.WriteLine("Invalid URI format: " + requestDto.Url);
				return null; // or throw an exception, log an error, etc.
			}

			HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
			HttpRequestMessage message = new();
			message.Headers.Add("Accept", "application/json");

			// Use the valid URI
			message.RequestUri = uri;

			if (requestDto.Data != null)
			{
				message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
			}

			HttpResponseMessage? apiResponse = null;

			switch (requestDto.ApiType)
			{
				case ApiType.POST:
					message.Method = HttpMethod.Post;
					break;
				case ApiType.DELETE:
					message.Method = HttpMethod.Delete;
					break;
				case ApiType.PUT:
					message.Method = HttpMethod.Put;
					break;
				default:
					message.Method = HttpMethod.Get;
					break;
			}

			try
			{
				apiResponse = await client.SendAsync(message);

				switch (apiResponse.StatusCode)
				{
					case HttpStatusCode.NotFound:
						return new()
						{
							IsSuccess = false,
							Message = "Not Found"
						};
					case HttpStatusCode.Unauthorized:
						return new()
						{
							IsSuccess = false,
							Message = "Unauthorized"
						};
					case HttpStatusCode.InternalServerError:
						return new()
						{
							IsSuccess = false,
							Message = "Internal Server Error (500)"
						};
					case HttpStatusCode.Forbidden:
						return new()
						{
							IsSuccess = false,
							Message = "Forbidden"
						};
					default:
						var apiContent = await apiResponse.Content.ReadAsStringAsync();
						var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
						return apiResponseDto;
				}
			}
			catch (Exception ex)
			{
				var dto = new ResponseDto
				{
					Message = ex.Message.ToString(),
					IsSuccess = false
				};
				return dto;


			}
		}
	}
}
