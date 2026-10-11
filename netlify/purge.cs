#!/usr/bin/dotnet

var token = Environment.GetEnvironmentVariable("NETLIFY_TOKEN");
var client = new HttpClient
{
	BaseAddress = new Uri("https://api.netlify.com/api/v1/")
};
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

var response = await client.GetStringAsync("sites");
Console.WriteLine(response);
