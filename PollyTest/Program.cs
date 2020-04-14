using Polly;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PollyTest
{
	class Program
	{
		private static void Main(string[] args)
		{


			ResultReTry();
			Console.ReadKey();
		}


		private static void ResultReTry()
		{
			var policy = Policy
				  .HandleResult<bool>((r) => r==false)
				  .RetryAsync(2, async (exception, retryCount) =>
				  {
					  Console.WriteLine("333333:" + exception.ToString() + "------" + $"第{retryCount}次重试");

				  });
			var result = policy.ExecuteAsync(async () => await Task.FromResult(true));
			Console.WriteLine("444444:");

		}


		private static void ExceptionReTry()
		{
			var policy = Policy
			  .Handle<Exception>()
			  .RetryAsync(2, async (exception, retryCount) =>
			  {
				  Console.WriteLine("333333:" + exception.Message + "------" + $"第{retryCount}次重试");
			  });

			var result = policy.ExecuteAsync(() => Test());
			Console.WriteLine("444444:");
		}

		private static async Task Test()
		{
			//Convert.ToInt32("w");
			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetAsync("http://news.cnblogs.com/Category/GetCategoryList?bigCateId=11&loadType=0").Result;
				var s = await response.Content.ReadAsStringAsync();
				Console.WriteLine("111111:" + s);
			}
		}


	}
}
