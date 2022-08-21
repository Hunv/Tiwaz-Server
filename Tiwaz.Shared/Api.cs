﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiwaz.Shared.DtoModel;
using Newtonsoft.Json;

namespace Tiwaz.Shared
{
    public class Api
    {
        private static readonly string _ServerBaseUrl = "https://localhost:7077/api/";

        public Api()
        {
            Console.WriteLine("Using Serverbase URL " + _ServerBaseUrl);
        }

        #region Device

        /// <summary>
        /// Gets all devices
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoDevice>?> GetDeviceAsync()
        {
            var setting = (List<DtoDevice>?)await Helper.ApiRequestGet(_ServerBaseUrl + "Device");
            return setting;
        }

        /// <summary>
        /// Gets all devices
        /// </summary>
        /// <returns></returns>
        public async Task<DtoDevice?> GetDeviceAsync(string deviceId)
        {
            var device = (DtoDevice?)await Helper.ApiRequestGet(_ServerBaseUrl + "Device/" + deviceId);
            return device;
        }


        public async Task<string?> RegisterDevice(string json)
        {
            Console.WriteLine("Registering Device...");

            var responseBody = await Helper.ApiRequestPost(_ServerBaseUrl + "Device", json);

            return responseBody;
        }
        #endregion
        #region DeviceSetting

        /// <summary>
        /// Gets all Device Settings
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<List<DtoDeviceSetting>?> GetDeviceSettingsAsync(string deviceId)
        {
            var setting = (List<DtoDeviceSetting>?)await Helper.ApiRequestGet(_ServerBaseUrl + "Device/" + deviceId);
            return setting;
        }

        /// <summary>
        /// Gets a Device Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<DtoDeviceSetting?> GetDeviceSettingAsync(string deviceId, string settingName)
        {
            var setting = (DtoDeviceSetting?)await Helper.ApiRequestGet(_ServerBaseUrl + "Device/" + deviceId + "/" + settingName);
            return setting;
        }

        /// <summary>
        /// Sets a Device Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task SetDeviceSettingAsync(string deviceId, DtoDeviceSetting setting)
        {
            var json = JsonConvert.SerializeObject(setting, Helper.GetJsonSerializer());

            var response = await Helper.ApiRequestPut(_ServerBaseUrl + "Device", json);

            if (response == null)
            {
                Console.WriteLine("Failed to set device setting {0} for deviceId {1}", setting.Name, deviceId);
            }
        }

        /// <summary>
        /// Deletes a Device Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task DeleteDeviceSettingAsync(string deviceId, string settingName)
        {
            await Helper.ApiRequestDelete(_ServerBaseUrl + "Device/" + deviceId + "/" + settingName);
        }

        #endregion
        #region Setting

        /// <summary>
        /// Deletes a Device  to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task DeleteDeviceAsync(string deviceId)
        {
            await Helper.ApiRequestDelete(_ServerBaseUrl + "Device/" + deviceId);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoSetting>?> GetSettingAsync()
        {
            var setting = (List<DtoSetting>?)await Helper.ApiRequestGet(_ServerBaseUrl + "Setting");
            return setting;
        }

        /// <summary>
        /// Gets a Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<DtoSetting?> GetSettingAsync(string settingName)
        {
            var setting = (DtoSetting?)await Helper.ApiRequestGet(_ServerBaseUrl + "Setting/" + settingName);
            return setting;
        }

        /// <summary>
        /// Sets a Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task SetSettingAsync(DtoSetting setting)
        {
            var json = JsonConvert.SerializeObject(setting, Helper.GetJsonSerializer());

            var response = await Helper.ApiRequestPut(_ServerBaseUrl + "Setting", json);

            if (response == null)
            {
                Console.WriteLine("Failed to set setting {0}", setting.Name);
            }
        }

        #endregion
        #region Match

        /// <summary>
        /// Gets a Match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<DtoMatch?> GetMatchAsync(int matchId)
        {
            var setting = (DtoMatch?)await Helper.ApiRequestGet(_ServerBaseUrl + "Match/" + matchId);
            return setting;
        }

        /// <summary>
        /// Gets the time left of a match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<int> GetMatchTimeAsync(int matchId)
        {
            var setting = (int?)await Helper.ApiRequestGet(_ServerBaseUrl + "Match/" + matchId + "/time");
            return setting ?? -1;
        }

        /// <summary>
        /// Gets all Matches
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoMatch>> GetMatchListAsync()
        {
            var setting = (List<DtoMatch>?)await Helper.ApiRequestGet(_ServerBaseUrl + "Match");
            return setting ?? new List<DtoMatch>();
        }


        /// <summary>
        /// Add a new Tournament
        /// </summary>
        /// <param name="match"></param>
        public async Task AddMatchAsync(DtoMatch match)
        {
            var json = JsonConvert.SerializeObject(match, Helper.GetJsonSerializer());

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("POST", _ServerBaseUrl + "Match", json);
            if (requestMessage.Content == null)
            {
                Console.WriteLine("No content set. Not setting content type...");
            }
            else
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Add a new Tournament
        /// </summary>
        /// <param name="match"></param>
        public async Task SetMatchAsync(DtoMatch match)
        {
            var json = JsonConvert.SerializeObject(match, Helper.GetJsonSerializer());
            var response = await Helper.ApiRequestPut(_ServerBaseUrl + "Match/" + match.Id, json);

            if (response == null)
            {
                Console.WriteLine("Failed to set match {0}", match.Id);
            }
        }

        /// <summary>
        /// Send Control Commands of a Match
        /// </summary>
        public async Task ControlMatchtimeAsync(int matchId, string command)
        {
            var response = await Helper.ApiRequestPut(_ServerBaseUrl + "Match/" + matchId + "/time/" + command, "");

            if (response == null)
            {
                Console.WriteLine("Failed to set match {0}", matchId);
            }
        }

        public async Task SetMatchGoalAsync(int matchId, int teamId, int amount)
        {
            var response = await Helper.ApiRequestPut(_ServerBaseUrl + "Match/" + matchId + "/goal/" + teamId + "/" + amount, "");

            if (response == null)
            {
                Console.WriteLine("Failed to set match goal for {0} and team {1} amount {2}", matchId, teamId, amount);
            }
        }

        #endregion
    }
}
