﻿using Tiwaz.Server.DatabaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tiwaz.Server.Api.DtoModel;

namespace Tiwaz.Server.Api
{
    public static class ApiDevice
    {
        /// <summary>
        /// Get all Devices
        /// </summary>
        public static string? GetDeviceList()
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Device != null)
            {
                List<DtoDevice>? dto = dbContext.Device.Select(aDevice => aDevice.ToDto()).ToList();
                return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
            }
            return null;
        }

        /// <summary>
        /// Get a Device Setting
        /// </summary>
        public static string? GetDeviceSetting(string deviceId, string setting)
        {
            using var dbContext = new TwDbContext();

            var sets = dbContext.DeviceSettings;
            if (sets != null)
            {
                var set = sets.SingleOrDefault(x => x.DeviceId == deviceId && x.SettingName == setting);
                if (set != null)
                {
                    var dto = set.ToDto();

                    return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                }
            }
            return null;
        }

        /// <summary>
        /// Creates or sets a device Setting
        /// </summary>
        public static async Task SetDeviceSetting(string deviceId, string settingName, string settingValue)
        {
            using var dbContext = new TwDbContext();
            var set = dbContext.DeviceSettings;
            if (set != null)
            {
                var tm = set.SingleOrDefault(x => x.DeviceId == deviceId && x.SettingName == settingName);
                if (tm != null)
                {
                    tm.SettingValue = settingValue;
                }
                else
                {
                    set.Add(new DeviceSetting(deviceId, settingName, settingValue));
                }

                await dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Creates a new device for device setting configuration
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static async Task<string?> NewDevice(DtoDevice device)
        {
            using var dbContext = new TwDbContext();
            var dev = dbContext.Device;
            if (dev != null)
            {
                //Create a new device ID
                var devId = Guid.NewGuid().ToString();
                var newDevice = new Device(devId, device.DeviceModel, device.DeviceType);
                dev.Add(newDevice);

                await dbContext.SaveChangesAsync();
                Console.WriteLine("Created new Device with GUID {0}, Type {1} and Model {2}", devId, device.DeviceType, device.DeviceModel);

                var retDevice = new DtoDevice(newDevice.DeviceId, newDevice.DeviceModel, newDevice.DeviceType);
                var dto = retDevice.ToDto();

                return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
            }
            return null;
        }
    }
}
