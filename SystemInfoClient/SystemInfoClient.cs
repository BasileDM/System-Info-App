﻿using System.Net;
using System.Runtime.Versioning;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SystemInfoClient.Classes;
using System.Diagnostics;


namespace SystemInfoClient {

    internal class SystemInfoClient {

        public static async Task Main(string[] args) {

            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Systeminfo App Client");

            var route = "https://localhost:7056/api/CustomerModels";
            var customer = new CustomerClass();
            //customer.LogInfo();

            JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
            var json = JsonSerializer.Serialize(customer, jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine(json);

            //var response = await client.PostAsync(route, content);

            //if (response.IsSuccessStatusCode) {
            //    Console.WriteLine($"Customer data sent successfully. {response.StatusCode}");
            //} else {
            //    Console.WriteLine($"Post request failed: {response.StatusCode}");
            //}
            
        }
    }



}