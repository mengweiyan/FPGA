
using System;
using System.Collections;

using Jungo.wdapi_dotnet;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using DWORD = System.UInt32;
using BOOL = System.Boolean;
using WDC_DRV_OPEN_OPTIONS = System.UInt32;

namespace Jungo.ft6678_yolo_lib
{
    public class FT6678_YOLO_DeviceList: ArrayList
    {
        private string FT6678_YOLO_DEFAULT_LICENSE_STRING  = "6C3C3225C73EFB96D73EADCFE321F554FB60D65C.A9AB070E";
        // TODO: If you have renamed the WinDriver kernel module
        // (windrvr1221.sys), change the driver name below accordingly
        private string FT6678_YOLO_DEFAULT_DRIVER_NAME  = "windrvr1221";
        private DWORD FT6678_YOLO_DEFAULT_VENDOR_ID = 0x16C3;
        private DWORD FT6678_YOLO_DEFAULT_DEVICE_ID = 0x8358;

        private static FT6678_YOLO_DeviceList instance;

        public static FT6678_YOLO_DeviceList TheDeviceList()
        {
            if (instance == null)
            {
                instance = new FT6678_YOLO_DeviceList();
            }
            return instance;
        }

        private FT6678_YOLO_DeviceList(){}

        public DWORD Init()
        {
            if (windrvr_decl.WD_DriverName(FT6678_YOLO_DEFAULT_DRIVER_NAME) == null)
            {
                Log.ErrLog(
                    "FT6678_YOLO_DeviceList.Init: Failed to set driver name for the "
                    + "WDC library.");
                return (DWORD)wdc_err.WD_SYSTEM_INTERNAL_ERROR;
            }

            DWORD dwStatus =
                wdc_lib_decl.WDC_SetDebugOptions(wdc_lib_consts.WDC_DBG_DEFAULT,
                null);
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                Log.ErrLog("FT6678_YOLO_DeviceList.Init: Failed to initialize debug "
                    + "options for the WDC library. Error 0x" +
                    dwStatus.ToString("X") + utils.Stat2Str(dwStatus));
                return dwStatus;
            }

            dwStatus = wdc_lib_decl.WDC_DriverOpen(
                (WDC_DRV_OPEN_OPTIONS)wdc_lib_consts.WDC_DRV_OPEN_DEFAULT,
                FT6678_YOLO_DEFAULT_LICENSE_STRING);
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                Log.ErrLog("FT6678_YOLO_DeviceList.Init: Failed to initialize the " +
                    "WDC library. Error 0x" + dwStatus.ToString("X") +
                    utils.Stat2Str(dwStatus));
                return dwStatus;
            }
            
            return Populate();
        }

        public FT6678_YOLO_Device Get(int index)
        {
            if (index >= this.Count || index < 0)
                return null;
            return (FT6678_YOLO_Device)this[index];
        }

        public FT6678_YOLO_Device Get(WD_PCI_SLOT slot)
        {
            foreach(FT6678_YOLO_Device device in this)
            {
                if (device.IsMySlot(ref slot))
                    return device;
            }
            return null;
        }

        private DWORD Populate()
        {
            DWORD dwStatus;
            WDC_PCI_SCAN_RESULT scanResult = new WDC_PCI_SCAN_RESULT();

            dwStatus = wdc_lib_decl.WDC_PciScanDevices(FT6678_YOLO_DEFAULT_VENDOR_ID,
                FT6678_YOLO_DEFAULT_DEVICE_ID, scanResult);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("FT6678_YOLO_DeviceList.Populate: Failed scanning "
                    + "the PCI bus. Error 0x" + dwStatus.ToString("X") +
                    utils.Stat2Str(dwStatus));
                return dwStatus;
            }

            if (scanResult.dwNumDevices == 0)
            {
                Log.ErrLog("FT6678_YOLO_DeviceList.Populate: No matching PCI " +
                    "device was found for search criteria " +
                    FT6678_YOLO_DEFAULT_VENDOR_ID.ToString("X") + ", " +
                    FT6678_YOLO_DEFAULT_DEVICE_ID.ToString("X"));
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            for (int i = 0; i < scanResult.dwNumDevices; ++i)
            {
                FT6678_YOLO_Device device;
                WD_PCI_SLOT slot = scanResult.deviceSlot[i];

                device = new FT6678_YOLO_Device(scanResult.deviceId[i].dwVendorId,
                    scanResult.deviceId[i].dwDeviceId, slot);

                this.Add(device);
            }
            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        public void Dispose()
        {
            foreach (FT6678_YOLO_Device device in this)
                device.Dispose();
            this.Clear();

            DWORD dwStatus = wdc_lib_decl.WDC_DriverClose();
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                Exception excp = new Exception("FT6678_YOLO_DeviceList.Dispose: " +
                    "Failed to uninit the WDC library. Error 0x" +
                    dwStatus.ToString("X") + utils.Stat2Str(dwStatus));
                throw excp;
            }
        }
    };
}

