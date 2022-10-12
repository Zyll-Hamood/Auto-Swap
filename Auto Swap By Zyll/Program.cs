using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
namespace Auto_Swap_By_Zyll
{
    internal class Program
    {
        static bool stopped;

        static string oldSession;
        static string newSession;

        static string oldUsername = "";
        static string oldName = "";
        static string oldBio = "";
        static string oldPhone = "";
        static string oldUrl = "";
		static string oldEmail = "";
		static string oldID = "";
		static string oldCsrf = "";

		static string newUsername = "";
		static string newName = "";
		static string newBio = "";
		static string newPhone = "";
		static string newUrl = "";
		static string newEmail = "";
		static string newID = "";
		static string newCsrf = "";
		static void Main(string[] args)
        {
			Console.Title = "Auto Swap By Zyll";



			Console.WriteLine("Old Sessionid : ");
			oldSession = Console.ReadLine();
			Console.WriteLine("");
			if (oldInfo().Contains("ok"))
            {
				Console.WriteLine("Done Login To Old Account.");
			}
			Console.WriteLine("");

			Console.WriteLine("New Sessionid : ");
			newSession = Console.ReadLine();
			Console.WriteLine();
            if (newInfo().Contains("ok"))
			{
				Console.WriteLine("Done Login To New Account.");
            }
			Console.WriteLine("");

			
			Console.WriteLine("Started ...");
			Console.WriteLine("================================");

			new Thread(new ThreadStart(StartThread)) { Priority = ThreadPriority.BelowNormal, IsBackground = false }.Start();

			Console.ReadKey();
        }

		#region Working
		static void StartThread()
		{
			try
			{
				if (stopped == false)
				{
					for (int i = 0; i <= Convert.ToInt32(100); i++)
					{
						Thread thread = new Thread(new ThreadStart(work));
						thread.Priority = ThreadPriority.Highest;
						thread.Start();
						thread.IsBackground = false;
						Thread.Sleep(10);
					}
				}
			}
			catch (Exception)
			{
			}
		}
		static void work()
		{
			
			string oldResp = string.Empty;
			string newResp = string.Empty;
			while (stopped == false)
			{
				try
				{

					oldResp = oldEdit();

					newResp = newEdit();

					File.AppendAllText("old response.txt", oldResp + "\r\n");
					File.AppendAllText("new response.txt", newResp + "\r\n");
					if (newResp.Contains("pk"))
					{
						Console.WriteLine("Done Moved");
						stopped = true;
					}
					else if (newResp.Contains("challenge_required") || newResp.Contains("consent_required") || newResp.Contains("login_required"))
					{
						stopped = true;
						Console.WriteLine("Account Blocked");
					}

				}
				catch (Exception)
				{

				}
				Thread.Sleep(10);
			}
		}

		#endregion

		#region API
		static string oldInfo()
		{
			System.Net.HttpWebRequest HttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://i.instagram.com/api/v1/accounts/current_user/?edit=true");
			HttpWebRequest.Method = "GET";
			HttpWebRequest.Accept = "*/*";
			HttpWebRequest.UserAgent = "Instagram 10.4.0 (iPhone7,2; iOS 12_5_1; en_SA@calendar=gregorian; ar-SA; scale=2.00; gamut=normal; 750x1334) AppleWebKit/420+";
			HttpWebRequest.Headers.Add("X-IG-Capabilities", "3yo=");
			HttpWebRequest.Headers.Add("Cookie", $"sessionid={oldSession}");
			HttpWebRequest.Headers.Add("Accept-Language", "ar-SA;q=1");
			HttpWebRequest.Headers.Add("X-IG-Connection-Type", "WiFi");
			System.Net.HttpWebResponse Response;
			try
			{
				Response = (System.Net.HttpWebResponse)HttpWebRequest.GetResponse();
			}
			catch (System.Net.WebException ex)
			{
				Response = (System.Net.HttpWebResponse)ex.Response;
			}
			System.IO.StreamReader StreamReader = new System.IO.StreamReader(Response.GetResponseStream());
			string Result = StreamReader.ReadToEnd().ToString();
			StreamReader.Dispose();
			StreamReader.Close();
			oldName = Regex.Match(Result, "\"full_name\":\"(.*?)\",").Groups[1].Value;
			oldUsername = Regex.Match(Result, "\"username\":\"(.*?)\",").Groups[1].Value;
			Console.WriteLine($"user : {oldUsername}");
			oldBio = Regex.Match(Result, "\"biography\":\"(.*?)\",").Groups[1].Value;
			oldEmail = Regex.Match(Result, "\"email\":\"(.*?)\",").Groups[1].Value;
			oldPhone = Regex.Match(Result, "\"phone_number\":\"(.*?)\",").Groups[1].Value;
			oldUrl = Regex.Match(Result, "\"external_url\":\"(.*?)\",").Groups[1].Value;
			oldID = Regex.Match(Result, "\"pk\":(.*?),").Groups[1].Value;
			return Result;
		}
		static string HASH(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			HMACSHA256 hmacsha = new HMACSHA256(Encoding.UTF8.GetBytes("673581b0ddb792bf47da5f9ca816b613d7996f342723aa06993a3f0552311c7d"));
			byte[] array = hmacsha.ComputeHash(Encoding.UTF8.GetBytes(value));
			int num = 0;
			checked
			{
				int num2 = array.Length - 1;
				int num3 = num;
				for (; ; )
				{
					int num4 = num3;
					int num5 = num2;
					bool flag = num4 > num5;
					if (flag)
					{
						break;
					}
					stringBuilder.Append(array[num3].ToString("x2"));
					num3++;
				}
				return stringBuilder.ToString();
			}
		}
		static string oldEdit()
		{

			string text = Guid.NewGuid().ToString().ToUpper();
			string text2 = string.Concat(new string[]
			{
			string.Concat(new string[]
			{
				"{\"gender\":\"3\",\"_csrftoken\":\""+oldCsrf+"\",\"_uuid\":\""+text+"\",\"_uid\":\""+oldID+"\",\"external_url\":\""+oldUrl+"\",\"username\":\""+oldUsername+".swapbyzyll"+"\",\"email\":\""+oldEmail+"\",\"phone_number\":\""+oldPhone+"\",\"biography\":\""+oldBio+"\",\"first_name\":\""+oldName+"\"}"
			})
			});
			string data = "ig_sig_key_version=5&signed_body=" + HASH(text2) + "." + text2;

			byte[] Bytes = new System.Text.UTF8Encoding().GetBytes(data);
			System.Net.HttpWebRequest HttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://i.instagram.com/api/v1/accounts/edit_profile/");
			HttpWebRequest.Method = "POST";
			HttpWebRequest.Accept = "*/*";
			HttpWebRequest.UserAgent = "Instagram 10.4.0 (iPhone7,2; iOS 12_5_1; en_SA@calendar=gregorian; ar-SA; scale=2.00; gamut=normal; 750x1334) AppleWebKit/420+";
			HttpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			HttpWebRequest.Headers.Add("Cookie", $"sessionid={oldSession}");
			HttpWebRequest.Headers.Add("X-IG-Capabilities", "3yo=");
			HttpWebRequest.Headers.Add("Accept-Language", "ar-SA;q=1");
			HttpWebRequest.Headers.Add("X-IG-Connection-Type", "WiFi");
			HttpWebRequest.ContentLength = Bytes.Length;
			System.IO.Stream Stream = HttpWebRequest.GetRequestStream();
			Stream.Write(Bytes, 0, Bytes.Length);
			Stream.Dispose();
			Stream.Close();
			System.Net.HttpWebResponse Response;
			try
			{
				Response = (System.Net.HttpWebResponse)HttpWebRequest.GetResponse();
			}
			catch (System.Net.WebException ex)
			{
				Response = (System.Net.HttpWebResponse)ex.Response;
			}
			System.IO.StreamReader StreamReader = new System.IO.StreamReader(Response.GetResponseStream());
			string Result = StreamReader.ReadToEnd().ToString();
			StreamReader.Dispose();
			StreamReader.Close();

			return Result;
		}
		///////////////////////////////////////////////////////
		static string newInfo()
		{
			System.Net.HttpWebRequest HttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://i.instagram.com/api/v1/accounts/current_user/?edit=true");
			HttpWebRequest.Method = "GET";
			HttpWebRequest.Accept = "*/*";
			HttpWebRequest.UserAgent = "Instagram 10.4.0 (iPhone7,2; iOS 12_5_1; en_SA@calendar=gregorian; ar-SA; scale=2.00; gamut=normal; 750x1334) AppleWebKit/420+";
			HttpWebRequest.Headers.Add("X-IG-Capabilities", "3yo=");
			HttpWebRequest.Headers.Add("Cookie", $"sessionid={newSession}");
			HttpWebRequest.Headers.Add("Accept-Language", "ar-SA;q=1");
			HttpWebRequest.Headers.Add("X-IG-Connection-Type", "WiFi");
			System.Net.HttpWebResponse Response;
			try
			{
				Response = (System.Net.HttpWebResponse)HttpWebRequest.GetResponse();
			}
			catch (System.Net.WebException ex)
			{
				Response = (System.Net.HttpWebResponse)ex.Response;
			}
			System.IO.StreamReader StreamReader = new System.IO.StreamReader(Response.GetResponseStream());
			string Result = StreamReader.ReadToEnd().ToString();
			StreamReader.Dispose();
			StreamReader.Close();
			newName = Regex.Match(Result, "\"full_name\":\"(.*?)\",").Groups[1].Value;
			newBio = Regex.Match(Result, "\"biography\":\"(.*?)\",").Groups[1].Value;
			newEmail = Regex.Match(Result, "\"email\":\"(.*?)\",").Groups[1].Value;
			newPhone = Regex.Match(Result, "\"phone_number\":\"(.*?)\",").Groups[1].Value;
			newUrl = Regex.Match(Result, "\"external_url\":\"(.*?)\",").Groups[1].Value;
			newID = Regex.Match(Result, "\"pk\":(.*?),").Groups[1].Value;
			return Result;
		}
		static string newEdit()
		{

			string text = Guid.NewGuid().ToString().ToUpper();
			string text2 = string.Concat(new string[]
			{
			string.Concat(new string[]
			{
				"{\"gender\":\"3\",\"_csrftoken\":\""+newCsrf+"\",\"_uuid\":\""+text+"\",\"_uid\":\""+newID+"\",\"external_url\":\""+newUrl+"\",\"username\":\""+oldUsername+"\",\"email\":\""+newEmail+"\",\"phone_number\":\""+newPhone+"\",\"biography\":\""+newBio+"\",\"first_name\":\""+newName+"\"}"
			})
			});
			string data = "ig_sig_key_version=5&signed_body=" + HASH(text2) + "." + text2;

			byte[] Bytes = new System.Text.UTF8Encoding().GetBytes(data);
			System.Net.HttpWebRequest HttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://i.instagram.com/api/v1/accounts/edit_profile/");
			HttpWebRequest.Method = "POST";
			HttpWebRequest.Accept = "*/*";
			HttpWebRequest.UserAgent = "Instagram 10.4.0 (iPhone7,2; iOS 12_5_1; en_SA@calendar=gregorian; ar-SA; scale=2.00; gamut=normal; 750x1334) AppleWebKit/420+";
			HttpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			HttpWebRequest.Headers.Add("Cookie", $"sessionid={newSession}");
			HttpWebRequest.Headers.Add("X-IG-Capabilities", "3yo=");
			HttpWebRequest.Headers.Add("Accept-Language", "ar-SA;q=1");
			HttpWebRequest.Headers.Add("X-IG-Connection-Type", "WiFi");
			HttpWebRequest.ContentLength = Bytes.Length;
			System.IO.Stream Stream = HttpWebRequest.GetRequestStream();
			Stream.Write(Bytes, 0, Bytes.Length);
			Stream.Dispose();
			Stream.Close();
			System.Net.HttpWebResponse Response;
			try
			{
				Response = (System.Net.HttpWebResponse)HttpWebRequest.GetResponse();
			}
			catch (System.Net.WebException ex)
			{
				Response = (System.Net.HttpWebResponse)ex.Response;
			}
			System.IO.StreamReader StreamReader = new System.IO.StreamReader(Response.GetResponseStream());
			string Result = StreamReader.ReadToEnd().ToString();
			StreamReader.Dispose();
			StreamReader.Close();

			return Result;
		}
		#endregion
	}
}
