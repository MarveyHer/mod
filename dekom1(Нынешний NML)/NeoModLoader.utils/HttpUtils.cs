using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using NeoModLoader.services;

namespace NeoModLoader.utils;

public static class HttpUtils
{
	public static HttpResponseMessage Get(string url, Dictionary<string, string> headers)
	{
		using HttpClient httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Clear();
		foreach (KeyValuePair<string, string> header in headers)
		{
			httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
		}
		return httpClient.GetAsync(url).Result;
	}

	public static string Post(string url, Dictionary<string, string> @params, Dictionary<string, string> headers = null, double timeout = 30.0)
	{
		using HttpClient httpClient = new HttpClient();
		FormUrlEncodedContent content = new FormUrlEncodedContent(@params);
		if (headers != null)
		{
			httpClient.DefaultRequestHeaders.Clear();
			foreach (KeyValuePair<string, string> header in headers)
			{
				httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
			}
		}
		httpClient.Timeout = TimeSpan.FromSeconds(timeout);
		try
		{
			HttpResponseMessage result = httpClient.PostAsync(url, content).Result;
			return (result.StatusCode == HttpStatusCode.OK) ? result.Content.ReadAsStringAsync().Result : "";
		}
		catch (Exception ex)
		{
			LogService.LogErrorConcurrent(ex.Message);
			LogService.LogErrorConcurrent(ex.StackTrace);
		}
		return "";
	}

	public static string Request(string url, string param = "", string method = "get")
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		string result = "";
		HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
		HttpWebResponse httpWebResponse = null;
		if (httpWebRequest == null)
		{
			return result;
		}
		httpWebRequest.Method = method;
		httpWebRequest.ContentType = "application/octet-stream";
		httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
		byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(param);
		if (bytes.Length != 0)
		{
			httpWebRequest.ContentLength = bytes.Length;
			httpWebRequest.Timeout = 15000;
			Stream requestStream = httpWebRequest.GetRequestStream();
			requestStream.Write(bytes, 0, bytes.Length);
			requestStream.Flush();
			requestStream.Close();
			try
			{
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				Stream responseStream = httpWebResponse.GetResponseStream();
				Encoding encoding = Encoding.GetEncoding("UTF-8");
				StreamReader streamReader = new StreamReader(responseStream, encoding);
				result = streamReader.ReadToEnd();
			}
			catch (Exception ex)
			{
				LogService.LogErrorConcurrent(ex.Message);
				return result;
			}
		}
		else
		{
			try
			{
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				Stream responseStream2 = httpWebResponse.GetResponseStream();
				Encoding encoding2 = Encoding.GetEncoding("UTF-8");
				StreamReader streamReader2 = new StreamReader(responseStream2, encoding2);
				result = streamReader2.ReadToEnd();
				streamReader2.Close();
			}
			catch (Exception ex2)
			{
				LogService.LogErrorConcurrent(ex2.Message);
				return result;
			}
		}
		return result;
	}
}
