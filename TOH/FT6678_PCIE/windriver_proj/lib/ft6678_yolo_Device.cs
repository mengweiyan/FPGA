using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Data;

using Jungo.wdapi_dotnet;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using item_types = Jungo.wdapi_dotnet.ITEM_TYPE;
using UINT64 = System.UInt64;
using UINT32 = System.UInt32;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using BYTE = System.Byte;
using BOOL = System.Boolean;
using WDC_DEVICE_HANDLE = System.IntPtr;
using HANDLE = System.IntPtr;
namespace Jungo.ft6678_yolo_lib
{
    /* PCI diagnostics plug-and-play and power management events handler
     * function type */
    public delegate void USER_EVENT_CALLBACK(ref WD_EVENT pEvent, FT6678_YOLO_Device dev);
    /* PCI diagnostics interrupt handler function type */
    public delegate void USER_INTERRUPT_CALLBACK(FT6678_YOLO_Device device);


    public class FT6678_YOLO_Device
    {
        private WDC_DEVICE m_wdcDevice = new WDC_DEVICE();
        protected MarshalWdcDevice m_wdcDeviceMarshaler;
        private USER_EVENT_CALLBACK m_userEventHandler;
        private USER_INTERRUPT_CALLBACK m_userIntHandler;
        private EVENT_HANDLER_DOTNET m_eventHandler;
        private INT_HANDLER m_intHandler;
        protected string m_sDeviceLongDesc;
        protected string m_sDeviceShortDesc;
        private FT6678_YOLO_Regs m_regs;
        public IntPtr pBuffer;
        public IntPtr ppDma;
#region " constructors "
        /* constructors & destructors */
        internal protected FT6678_YOLO_Device(WD_PCI_SLOT slot): this(0, 0, slot) {}

        internal protected FT6678_YOLO_Device(DWORD dwVendorId, DWORD dwDeviceId,
            WD_PCI_SLOT slot)
        {
            m_wdcDevice = new WDC_DEVICE();
            m_wdcDevice.id.pciId.dwVendorId = dwVendorId;
            m_wdcDevice.id.pciId.dwDeviceId = dwDeviceId;
            m_wdcDevice.slot.pciSlot = slot;
            m_wdcDeviceMarshaler = new MarshalWdcDevice();
            m_eventHandler = new EVENT_HANDLER_DOTNET(FT6678_YOLO_EventHandler);
            m_regs = new FT6678_YOLO_Regs();
            SetDescription();
        }

        public void Dispose()
        {
            Close();
        }
#endregion

#region " properties "
        /*********************
         *  properties       *
         *********************/

        public IntPtr Handle
        {
            get
            {
                if (m_wdcDevice == null)
                    return IntPtr.Zero;
                return m_wdcDevice.hDev;
            }
            set
            {
                m_wdcDevice.hDev = value;
            }
        }

        protected WDC_DEVICE wdcDevice
        {
            get
            {
                return m_wdcDevice;
            }
            set
            {
                m_wdcDevice = value;
            }
        }

        public WD_PCI_ID id
        {
            get
            {
                return m_wdcDevice.id.pciId;
            }
            set
            {
                m_wdcDevice.id.pciId = value;
            }
        }

        public WD_PCI_SLOT slot
        {
            get
            {
                return m_wdcDevice.slot.pciSlot;
            }
            set
            {
                m_wdcDevice.slot.pciSlot = value;
            }
        }

        public DWORD NumAddrSpaces
        {
            get
            {
                return m_wdcDevice.dwNumAddrSpaces;
            }
            set
            {
                m_wdcDevice.dwNumAddrSpaces = value;
            }
        }

        public WDC_ADDR_DESC[] AddrDesc
        {
            get
            {
                return m_wdcDevice.pAddrDesc;
            }
            set
            {
                m_wdcDevice.pAddrDesc = value;
            }
        }

        public FT6678_YOLO_Regs Regs
        {
            get
            {
                return m_regs;
            }
        }

#endregion

#region " utilities "
        /********************
         *     utilities    *
         *********************/

        /* public methods */

        public string[] AddrDescToString(bool bMemOnly)
        {
            string[] sAddr = new string[AddrDesc.Length];
            for (int i = 0; i < sAddr.Length; ++i)
            {
                sAddr[i] = "BAR " + AddrDesc[i].dwAddrSpace.ToString() +
                     ((AddrDesc[i].fIsMemory)? " Memory " : " I/O ");

                if (wdc_lib_decl.WDC_AddrSpaceIsActive(Handle,
                    AddrDesc[i].dwAddrSpace))
                {
                    WD_ITEMS item =
                        m_wdcDevice.cardReg.Card.Item[AddrDesc[i].dwItemIndex];
                    UINT64 pAddr = (UINT64)(AddrDesc[i].fIsMemory ?
                        item.I.Mem.pPhysicalAddr : item.I.IO.pAddr);

                    sAddr[i] += pAddr.ToString("X") + " - " +
                        (pAddr + AddrDesc[i].qwBytes - 1).ToString("X") +
                        " (" + AddrDesc[i].qwBytes.ToString("X") + " bytes)";
                }
                else
                    sAddr[i] += "Inactive address space";
            }
            return sAddr;
        }
        //20220819 wm 
        public UINT64 RCAddrToUint64(bool bMemOnly)
        {
            for (int i = 0; i < AddrDesc.Length; ++i)
            {
                if (wdc_lib_decl.WDC_AddrSpaceIsActive(Handle,
                    AddrDesc[i].dwAddrSpace))
                {
                    WD_ITEMS item =
                        m_wdcDevice.cardReg.Card.Item[AddrDesc[i].dwItemIndex];
                    UINT64 pAddr = (UINT64)(AddrDesc[i].fIsMemory ?
                        item.I.Mem.pPhysicalAddr : item.I.IO.pAddr);
                    return pAddr;
                }
                else
                    return 0;
            }
            return 0;
        }
        //20220819 wm 
        public UINT64 RCSizeToUint64(bool bMemOnly)
        {
            for (int i = 0; i < AddrDesc.Length; ++i)
            {
                if (wdc_lib_decl.WDC_AddrSpaceIsActive(Handle,
                    AddrDesc[i].dwAddrSpace))
                {
                    return AddrDesc[i].qwBytes;
                }
                else
                    return 0;
            }
            return 0;
        }
        public string ToString(BOOL bLong)
        {
            return (bLong)? m_sDeviceLongDesc: m_sDeviceShortDesc;
        }

        public bool IsMySlot(ref WD_PCI_SLOT slot)
        {
            if (m_wdcDevice.slot.pciSlot.dwBus == slot.dwBus &&
                m_wdcDevice.slot.pciSlot.dwSlot == slot.dwSlot &&
                m_wdcDevice.slot.pciSlot.dwFunction == slot.dwFunction)
                return true;

            return false;
        }

        /* protected methods */

        protected void SetDescription()
        {
            m_sDeviceLongDesc = string.Format("FT6678_YOLO Device: Vendor ID 0x{0:X}, "
                + "Device ID 0x{1:X}, Physical Location {2:X}:{3:X}:{4:X}",
                id.dwVendorId, id.dwDeviceId, slot.dwBus, slot.dwSlot,
                slot.dwFunction);

            m_sDeviceShortDesc = string.Format("Device " +
                "{0:X},{1:X} {2:X}:{3:X}:{4:X}", id.dwVendorId,
                id.dwDeviceId, slot.dwBus, slot.dwSlot, slot.dwFunction);
        }

        /* private methods */

        private bool DeviceValidate()
        {
            DWORD i, dwNumAddrSpaces = m_wdcDevice.dwNumAddrSpaces;

            /* NOTE: You can modify the implementation of this function in     *
             * order to verify that the device has the resources you expect to *
             * find */

            /* Verify that the device has at least one active address space */
            for (i = 0; i < dwNumAddrSpaces; i++)
            {
                if (wdc_lib_decl.WDC_AddrSpaceIsActive(Handle, i))
                    return true;
            }

            Log.TraceLog("FT6678_YOLO_Device.DeviceValidate: Device does not have "
                + "any active memory or I/O address spaces " + "(" +
                this.ToString(false) + ")" );
            return true;
        }

#endregion

#region " Device Open/Close "
        /****************************
         *  Device Open & Close      *
         *****************************/

        /* public methods */

        public virtual DWORD Open()
        {
            DWORD dwStatus;
            WD_PCI_CARD_INFO deviceInfo = new WD_PCI_CARD_INFO();

            /* Retrieve the device's resources information */
            deviceInfo.pciSlot = slot;
            dwStatus = wdc_lib_decl.WDC_PciGetDeviceInfo(deviceInfo);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("FT6678_YOLO_Device.Open: Failed retrieving the "
                    + "device's resources information. Error 0x" +
                    dwStatus.ToString("X") + ": " + utils.Stat2Str(dwStatus) +
                    "(" + this.ToString(false) +")" );
                return dwStatus;
            }

            /* NOTE: You can modify the device's resources information here,
             * if necessary (mainly the deviceInfo.Card.Items array or the
             * items number - deviceInfo.Card.dwItems) in order to register
             * only some of the resources or register only a portion of a
             * specific address space, for example. */

            dwStatus = wdc_lib_decl.WDC_PciDeviceOpen(ref m_wdcDevice,
                deviceInfo, IntPtr.Zero);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("FT6678_YOLO_Device.Open: Failed opening a " +
                    "WDC device handle. Error 0x" + dwStatus.ToString("X") +
                    ": " + utils.Stat2Str(dwStatus) + "(" +
                    this.ToString(false) + ")");
                goto Error;
            }

            Log.TraceLog("FT6678_YOLO_Device.Open: Opened a PCI device " +
                this.ToString(false));

            /* Validate device information */
            if (DeviceValidate() != true)
            {
                dwStatus = (DWORD)wdc_err.WD_NO_RESOURCES_ON_DEVICE;
                goto Error;
            }

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;

Error:
            if (Handle != IntPtr.Zero)
                Close();

            return dwStatus;
        }

        public virtual bool Close()
        {
            DWORD dwStatus;

            if (Handle == IntPtr.Zero)
            {
                Log.ErrLog("FT6678_YOLO_Device.Close: Error - NULL "
                    + "device handle");
                return false;
            }

            /* unregister events*/
            dwStatus = EventUnregister();

            /* Disable interrupts */
            dwStatus = DisableInterrupts();

            /* Close the device */
            dwStatus = wdc_lib_decl.WDC_PciDeviceClose(Handle);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("FT6678_YOLO_Device.Close: Failed closing a "
                    + "WDC device handle (0x" + Handle.ToInt64().ToString("X")
                    + ". Error 0x" + dwStatus.ToString("X") + ": " +
                    utils.Stat2Str(dwStatus) + this.ToString(false));
            }
            else
            {
                Log.TraceLog("FT6678_YOLO_Device.Close: " +
                    this.ToString(false) + " was closed successfully");
            }

            return ((DWORD)wdc_err.WD_STATUS_SUCCESS == dwStatus);
        }

#endregion

#region " Interrupts "
            /* public methods */
        public bool IsEnabledInt()
        {
            return wdc_lib_decl.WDC_IntIsEnabled(this.Handle);
        }

        protected virtual DWORD CreateIntTransCmds(out WD_TRANSFER[]
            pIntTransCmds, out DWORD dwNumCmds)
        {
            /* Define the number of interrupt transfer commands to use */
            DWORD NUM_TRANS_CMDS = 0;
            pIntTransCmds = new WD_TRANSFER[NUM_TRANS_CMDS];
            /*
            TODO: Your hardware has level sensitive interrupts, which must be
          acknowledged in the kernel immediately when they are received.
                  Since the information for acknowledging the interrupts is
            hardware-specific, YOU MUST ADD CODE to read/write the relevant
            register(s) in order to correctly acknowledge the interrupts
            on your device, as dictated by your hardware's specifications.
            When adding transfer commands, be sure to also modify the
            definition of NUM_TRANS_CMDS (above) accordingly.

            *************************************************************************
            * NOTE: If you attempt to use this code without first modifying it in   *
            *       order to correctly acknowledge your device's interrupts, as     *
            *       explained above, the OS will HANG when an interrupt occurs!     *
            *************************************************************************
            */
            dwNumCmds = NUM_TRANS_CMDS;
            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        protected virtual DWORD DisableCardInts()
        {
            /* TODO: You can add code here to write to the device in order
             * to physically disable the hardware interrupts */
            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        protected BOOL IsItemExists(WDC_DEVICE Dev, DWORD item)
        {
            DWORD i, dwNumItems = Dev.cardReg.Card.dwItems;

            for (i = 0; i < dwNumItems; i++)
            {
                if (Dev.cardReg.Card.Item[i].item == item)
                    return true;
            }

            return false;
        }


      public bool IsMsiInt()
      {
          return wdc_defs_macros.WDC_INT_IS_MSI(
              wdc_defs_macros.WDC_GET_ENABLED_INT_TYPE(wdcDevice));
      }

      public DWORD GetEnableIntLastMsg()
      {
          return wdc_defs_macros.WDC_GET_ENABLED_INT_LAST_MSG(wdcDevice);
      }

       public string WDC_DIAG_IntTypeDescriptionGet()
       {
           DWORD dwIntType = wdc_defs_macros.WDC_GET_ENABLED_INT_TYPE(wdcDevice);

           if ((dwIntType & (DWORD)(WD_INTERRUPT_TYPE.INTERRUPT_MESSAGE_X)) != 0)
               return "Extended Message-Signaled Interrupt (MSI-X)";
           else if ((dwIntType & (DWORD)(WD_INTERRUPT_TYPE.INTERRUPT_MESSAGE)) != 0)
               return "Message-Signaled Interrupt (MSI)";
           else if ((dwIntType & (DWORD)(WD_INTERRUPT_TYPE.INTERRUPT_LEVEL_SENSITIVE)) != 0)
               return "Level-Sensitive Interrupt";
           return "Edge-Triggered Interrupt";
        }

        public DWORD EnableInterrupts(USER_INTERRUPT_CALLBACK userIntCb, IntPtr pData)
        {
            DWORD dwStatus;
            WD_TRANSFER[] pIntTransCmds = null;
            DWORD dwNumCmds;
            if (userIntCb == null)
            {
                Log.TraceLog("FT6678_YOLO_Device.EnableInterrupts: "
                    + "user callback is invalid");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            if (!IsItemExists(m_wdcDevice, (DWORD)item_types.ITEM_INTERRUPT))
            {
                Log.TraceLog("FT6678_YOLO_Device.EnableInterrupts: "
                    + "Device doesn't have any interrupts");
                return (DWORD)wdc_err.WD_OPERATION_FAILED;
            }

            m_userIntHandler = userIntCb;

            m_intHandler = new INT_HANDLER(FT6678_YOLO_IntHandler);
            if (m_intHandler == null)
            {
                Log.ErrLog("FT6678_YOLO_Device.EnableInterrupts: interrupt handler is " +
                    "null (" + this.ToString(false) + ")" );
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            if (wdc_lib_decl.WDC_IntIsEnabled(Handle))
            {
                Log.ErrLog("FT6678_YOLO_Device.EnableInterrupts: "
                    + "interrupts are already enabled (" +
                    this.ToString(false) + ")" );
                return (DWORD)wdc_err.WD_OPERATION_ALREADY_DONE;
            }

            dwStatus = CreateIntTransCmds(out pIntTransCmds, out dwNumCmds);
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
                return dwStatus;
            dwStatus = wdc_lib_decl.WDC_IntEnable(wdcDevice, pIntTransCmds,
                dwNumCmds, 0, m_intHandler, pData, wdc_defs_macros.WDC_IS_KP(wdcDevice));

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("FT6678_YOLO_Device.EnableInterrupts: Failed "
                    + "enabling interrupts. Error " + dwStatus.ToString("X") + ": "
                    + utils.Stat2Str(dwStatus) + "(" + this.ToString(false) + ")");
                m_intHandler = null;
                return dwStatus;
            }
            /* TODO: You can add code here to write to the device in order
                 to physically enable the hardware interrupts */

            Log.TraceLog("FT6678_YOLO_Device: enabled interrupts (" + this.ToString(false) + ")");
            return dwStatus;
        }

        public DWORD DisableInterrupts()
        {
            DWORD dwStatus;

            if (!wdc_lib_decl.WDC_IntIsEnabled(this.Handle))
            {
                Log.ErrLog("FT6678_YOLO_Device.DisableInterrupts: interrupts are already disabled... " +
                    "(" + this.ToString(false) + ")");
                return (DWORD)wdc_err.WD_OPERATION_ALREADY_DONE;
            }

            /* Physically disabling the hardware interrupts */
            dwStatus = DisableCardInts();

            dwStatus = wdc_lib_decl.WDC_IntDisable(m_wdcDevice);
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                Log.ErrLog("FT6678_YOLO_Device.DisableInterrupts: Failed to" +
                    "disable interrupts. Error " + dwStatus.ToString("X")
                    + ": " + utils.Stat2Str(dwStatus) + " (" +
                    this.ToString(false) + ")");
            }
            else
            {
                Log.TraceLog("FT6678_YOLO_Device.DisableInterrupts: Interrupts are disabled" +
                    "(" + this.ToString(false) + ")");
            }

            return dwStatus;
        }

            /* private methods */
        private void FT6678_YOLO_IntHandler(IntPtr pDev)
        {
            wdcDevice.Int =
                (WD_INTERRUPT)m_wdcDeviceMarshaler.MarshalDevWdInterrupt(pDev);

            /* to obtain the data that was read at interrupt use:
             * WD_TRANSFER[] transCommands;
             * transCommands = (WD_TRANSFER[])m_wdcDeviceMarshaler.MarshalDevpWdTrans(
             *     wdcDevice.Int.Cmd, wdcDevice.Int.dwCmds); */

            if (m_userIntHandler != null)
                m_userIntHandler(this);
        }

#endregion

#region " Events"
        /****************************
         *          Events          *
         * **************************/

        /* public methods */

        public bool IsEventRegistered()
        {
            if (Handle == IntPtr.Zero)
                return false;

            return wdc_lib_decl.WDC_EventIsRegistered(Handle);
        }

        public DWORD EventRegister(USER_EVENT_CALLBACK userEventHandler)
        {
            DWORD dwStatus;
            DWORD dwActions = (DWORD)windrvr_consts.WD_ACTIONS_ALL;
            /* TODO: Modify the above to set up the plug-and-play/power
             * management events for which you wish to receive notifications.
             * dwActions can be set to any combination of the WD_EVENT_ACTION
             * flags defined in windrvr.h */

            if (userEventHandler == null)
            {
                Log.ErrLog("FT6678_YOLO_Device.EventRegister: user callback is "
                    + "null");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            /* Check if event is already registered */
            if (wdc_lib_decl.WDC_EventIsRegistered(Handle))
            {
                Log.ErrLog("FT6678_YOLO_Device.EventRegister: Events are already "
                    + "registered ...");
                return (DWORD)wdc_err.WD_OPERATION_ALREADY_DONE;
            }

            m_userEventHandler = userEventHandler;

            /* Register event */
            dwStatus = wdc_lib_decl.WDC_EventRegister(m_wdcDevice, dwActions,
                m_eventHandler, Handle, wdc_defs_macros.WDC_IS_KP(wdcDevice));

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("FT6678_YOLO_Device.EventRegister: Failed to register "
                    + "events. Error 0x" + dwStatus.ToString("X")
                    + utils.Stat2Str(dwStatus));
                m_userEventHandler = null;
            }
            else
            {
                Log.TraceLog("FT6678_YOLO_Device.EventRegister: events are " +
                    " registered (" + this.ToString(false) +")" );
            }

            return dwStatus;
        }

        public DWORD EventUnregister()
        {
            DWORD dwStatus;

            if (!wdc_lib_decl.WDC_EventIsRegistered(Handle))
            {
                Log.ErrLog("FT6678_YOLO_Device.EventUnregister: No events " +
                    "currently registered ...(" + this.ToString(false) + ")");
                return (DWORD)wdc_err.WD_OPERATION_ALREADY_DONE;
            }

            dwStatus = wdc_lib_decl.WDC_EventUnregister(m_wdcDevice);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("FT6678_YOLO_Device.EventUnregister: Failed to " +
                    "unregister events. Error 0x" + dwStatus.ToString("X") +
                    ": " + utils.Stat2Str(dwStatus) + "(" +
                    this.ToString(false) + ")");
            }
            else
            {
                Log.TraceLog("FT6678_YOLO_Device.EventUnregister: Unregistered " +
                    " events (" + this.ToString(false) + ")" );
            }

            return dwStatus;
        }

        /** private methods **/

        /* event callback method */
        private void FT6678_YOLO_EventHandler(IntPtr pWdEvent, IntPtr pDev)
        {
            MarshalWdEvent wdEventMarshaler = new MarshalWdEvent();
            WD_EVENT wdEvent = (WD_EVENT)wdEventMarshaler.MarshalNativeToManaged(pWdEvent);
            m_wdcDevice.Event =
                (WD_EVENT)m_wdcDeviceMarshaler.MarshalDevWdEvent(pDev);
            if (m_userEventHandler != null)
                m_userEventHandler(ref wdEvent, this);
        }
#endregion

#region "Registers Read-Write "
        // Function: FT6678_YOLO_ReadImage()
        //   Read from Image register.
        // Parameters:
        //   None.
        // Return Value:
        //   The value read from the register.
        UINT32 FT6678_YOLO_ReadImage ()
        {
            UINT32 data = 0;

            wdc_lib_decl.WDC_ReadAddr32(Handle,
                m_regs.gFT6678_YOLO_RT_Regs[0].dwAddrSpace,
                m_regs.gFT6678_YOLO_RT_Regs[0].dwOffset,
                ref data);
            return data;
        }

        // Function: FT6678_YOLO_WriteImage()
        //   Write to Image register.
        // Parameters:
        //   data [in] the data to write to the register.
        // Return Value:
        //   None.
        void FT6678_YOLO_WriteImage (UINT32 data)
        {
            wdc_lib_decl.WDC_WriteAddr32(Handle,
                m_regs.gFT6678_YOLO_RT_Regs[0].dwAddrSpace,
                m_regs.gFT6678_YOLO_RT_Regs[0].dwOffset,
                data);
        }

#endregion

    }
}
