using Polly;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PollyTest
{
	class Program
	{
		private static void Main(string[] args)
		{
			ExceptionReTryForever();
			Console.ReadKey();
		}

		public static void MyT(DelegateResult<bool> delegateResult, int retryCount)
		{
			Console.WriteLine("333333:" + delegateResult.Result + "------" + $"第{retryCount}次重试");
		}

		private static void ResultReTryForever()
		{
			//		var policy = Policy
			//	  .HandleResult<bool>((r) => r == false)
			//	  .WaitAndRetryForeverAsync()
			//		.WaitAndRetry(new[]
			// {
			//TimeSpan.FromSeconds(1),
			//TimeSpan.FromSeconds(3),
			//TimeSpan.FromSeconds(7)
			// })
			//	  .RetryForever(MyT)

			//	  .Execute(() =>
			//	  {

			//		  Thread.Sleep(1000);
			//		  return false;
			//	  });
			//		Console.WriteLine("444444:");
		}


		private static void ResultReTry()
		{
			var policy = Policy
				.HandleResult<bool>((r) => r == false)
				.RetryAsync(2, async (exception, retryCount) =>
				{
					Console.WriteLine("333333:" + exception.ToString() + "------" + $"第{retryCount}次重试");

				});
			try
			{
				var result = policy.ExecuteAsync(async () =>
				{
					//throw new Exception("wo shi yi chang");
					return await Task.FromResult(false);
				}).ContinueWith(t => { Console.WriteLine(t.Exception); Console.WriteLine("cs"); });
				//policy.c
			}
			catch (Exception e)
			{
				Console.WriteLine("oo" + e);
			}

			Console.WriteLine("444444:");

		}

		/// <summary>
		/// 异常重连
		/// </summary>
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


		/// <summary>
		/// 异常无限重连
		/// </summary>
		private static void ExceptionReTryForever()
		{
			CancellationTokenSource tokenSource = new CancellationTokenSource();
			var policy = Policy
			  .Handle<Exception>()
			  .WaitAndRetryForeverAsync((retryCount) =>
			  {
				  Console.WriteLine("333333sleep:" + retryCount);
				  return TimeSpan.FromSeconds(5);
			  }, (e, t) =>
			  {
				  Console.WriteLine($"333333:{e},{t}");
			  });

			//.RetryAsync(2, async (exception, retryCount) =>
			//{
			// Console.WriteLine("333333:" + exception.Message + "------" + $"第{retryCount}次重试");
			//});

			var result = policy.ExecuteAsync((token) => TestException(), tokenSource.Token).ContinueWith(t => Console.WriteLine("完成"));
			var result2 = policy.ExecuteAsync((token) => TestException(), tokenSource.Token).ContinueWith(t => Console.WriteLine("完成"));
			//var result1 = policy.ExecuteAsync(() => TestException()).ContinueWith(t => Console.WriteLine("完成"));
			Task.Run(()=> { Thread.Sleep(10000); tokenSource.Cancel(); });
			
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

		static int TestException_i = 0;
		private static async Task TestException()
		{
			//TestException_i++;
			if (TestException_i <= 3)
			{

				Convert.ToInt32("w");
			}
			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetAsync("http://news.cnblogs.com/Category/GetCategoryList?bigCateId=11&loadType=0").Result;
				var s = await response.Content.ReadAsStringAsync();
				Console.WriteLine("111111:" + s);
			}
		}
	}
}
