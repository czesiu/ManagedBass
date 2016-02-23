﻿using ManagedBass.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedBass
{
    /// <summary>
    /// Wraps a Recording Device
    /// </summary>
    /// <remarks>All Devices need to initialized before use</remarks>
    public class RecordingDevice : IDisposable
    {
        #region Singleton
        static Dictionary<int, RecordingDevice> Singleton = new Dictionary<int, RecordingDevice>();

        RecordingDevice(int DeviceIndex) { this.DeviceIndex = DeviceIndex; }

        public static RecordingDevice Get(int Device)
        {
            if (Singleton.ContainsKey(Device)) return Singleton[Device];
            else
            {
                DeviceInfo info;
                if (!Bass.RecordGetDeviceInfo(Device, out info))
                    throw new ArgumentException("Invalid RecordingDevice Index");

                var Dev = new RecordingDevice(Device);
                Singleton.Add(Device, Dev);

                return Dev;
            }
        }
        #endregion

        /// <summary>
        /// Enumerates available Recording Devices
        /// </summary>
        public static IEnumerable<RecordingDevice> Devices
        {
            get
            {
                DeviceInfo info;

                for (int i = 0; Bass.RecordGetDeviceInfo(i, out info); ++i)
                    yield return Get(i);
            }
        }

        /// <summary>
        /// The Index of the Device as identified by Bass
        /// </summary>
        public int DeviceIndex { get; }

        /// <summary>
        /// Number of available Recording Devices
        /// </summary>
        public static int Count => Bass.RecordingDeviceCount;

        /// <summary>
        /// Gets a DeviceInfo object containing information on the Device like Name, Type, IsEnabled, etc.
        /// </summary>
        public DeviceInfo DeviceInfo => Bass.RecordGetDeviceInfo(DeviceIndex);

        /// <summary>
        /// Initialize a Device for Recording
        /// </summary>
        /// <returns>A Return&lt;bool&gt; object containing success and error info</returns>
        public Return<bool> Init() => Bass.RecordInit(DeviceIndex);

        /// <summary>
        /// Frees an initialized Device
        /// </summary>
        public void Dispose()
        {
            Bass.CurrentRecordingDevice = DeviceIndex;
            Bass.RecordFree();
        }

        /// <summary>
        /// Default Audio Recording Devices
        /// </summary>
        public static RecordingDevice DefaultDevice => Devices.First((dev) => dev.DeviceInfo.IsDefault);

        public static RecordingDevice CurrentDevice
        {
            get { return Get(Bass.CurrentRecordingDevice); }
            set { Bass.CurrentRecordingDevice = value.DeviceIndex; }
        }

        /// <summary>
        /// Returns the Name of the Device
        /// </summary>
        public override string ToString() => DeviceInfo.Name;
    }
}