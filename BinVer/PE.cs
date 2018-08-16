using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BinVer
{
    #region ENUM

    /// <summary>
    /// The machine type this binary code runs on
    /// </summary>
    public enum PEMachineType : ushort
    {
        /// <summary>
        /// The contents of this field are assumed to be applicable to any machine type
        /// </summary>
        IMAGE_FILE_MACHINE_UNKNOWN = 0x0,
        /// <summary>
        /// Matsushita AM33
        /// </summary>
        IMAGE_FILE_MACHINE_AM33 = 0x1d3,
        /// <summary>
        /// x64
        /// </summary>
        IMAGE_FILE_MACHINE_AMD64 = 0x8664,
        /// <summary>
        /// ARM little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARM = 0x1c0,
        /// <summary>
        /// ARM64 little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARM64 = 0xaa64,
        /// <summary>
        /// ARM Thumb-2 little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARMNT = 0x1c4,
        /// <summary>
        /// EFI byte code
        /// </summary>
        IMAGE_FILE_MACHINE_EBC = 0xebc,
        /// <summary>
        /// Intel 386 or later processors and compatible processors
        /// </summary>
        IMAGE_FILE_MACHINE_I386 = 0x14c,
        /// <summary>
        /// Intel Itanium processor family
        /// </summary>
        IMAGE_FILE_MACHINE_IA64 = 0x200,
        /// <summary>
        /// Mitsubishi M32R little endian
        /// </summary>
        IMAGE_FILE_MACHINE_M32R = 0x9041,
        /// <summary>
        /// MIPS16
        /// </summary>
        IMAGE_FILE_MACHINE_MIPS16 = 0x266,
        /// <summary>
        /// MIPS with FPU
        /// </summary>
        IMAGE_FILE_MACHINE_MIPSFPU = 0x366,
        /// <summary>
        /// MIPS16 with FPU
        /// </summary>
        IMAGE_FILE_MACHINE_MIPSFPU16 = 0x466,
        /// <summary>
        /// Power PC little endian
        /// </summary>
        IMAGE_FILE_MACHINE_POWERPC = 0x1f0,
        /// <summary>
        /// Power PC with floating point support
        /// </summary>
        IMAGE_FILE_MACHINE_POWERPCFP = 0x1f1,
        /// <summary>
        /// MIPS little endian
        /// </summary>
        IMAGE_FILE_MACHINE_R4000 = 0x166,
        /// <summary>
        /// RISC-V 32-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV32 = 0x5032,
        /// <summary>
        /// RISC-V 64-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV64 = 0x5064,
        /// <summary>
        /// RISC-V 128-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV128 = 0x5128,
        /// <summary>
        /// Hitachi SH3
        /// </summary>
        IMAGE_FILE_MACHINE_SH3 = 0x1a2,
        /// <summary>
        /// Hitachi SH3 DSP
        /// </summary>
        IMAGE_FILE_MACHINE_SH3DSP = 0x1a3,
        /// <summary>
        /// Hitachi SH4
        /// </summary>
        IMAGE_FILE_MACHINE_SH4 = 0x1a6,
        /// <summary>
        /// Hitachi SH5
        /// </summary>
        IMAGE_FILE_MACHINE_SH5 = 0x1a8,
        /// <summary>
        /// Thumb
        /// </summary>
        IMAGE_FILE_MACHINE_THUMB = 0x1c2,
        /// <summary>
        /// MIPS little-endian WCE v2
        /// </summary>
        IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x169
    }

    /// <summary>
    /// DLL Characteristics for loading and relocation
    /// </summary>
    [Flags]
    public enum PEDllCharacteristics : ushort
    {
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RES0 = 0x0001,
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RES1 = 0x0002,
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RES2 = 0x0004,
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        RES3 = 0x0008,
        /// <summary>
        /// Image can handle a high entropy 64-bit virtual address space.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_HIGH_ENTROPY_VA = 0x0020,
        /// <summary>
        /// DLL can be relocated at load time.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE = 0x0040,
        /// <summary>
        /// Code Integrity checks are enforced.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY = 0x0080,
        /// <summary>
        /// Image is NX compatible.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NX_COMPAT = 0x0100,
        /// <summary>
        /// Isolation aware, but do not isolate the image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,
        /// <summary>
        /// Does not use structured exception (SE) handling.
        /// No SE handler may be called in this image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,
        /// <summary>
        /// Do not bind the image.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,
        /// <summary>
        /// Image must execute in an AppContainer.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_APPCONTAINER = 0x1000,
        /// <summary>
        /// A WDM driver.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,
        /// <summary>
        /// Image supports Control Flow Guard.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_GUARD_CF = 0x4000,
        /// <summary>
        /// Terminal Server aware.
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000
    }

    /// <summary>
    /// Characteristics of a PE image (including DLL)
    /// </summary>
    [Flags]
    public enum PEImageCharacteristics : ushort
    {
        /// <summary>
        /// Image only, Windows CE, and Microsoft Windows NT and later.
        /// This indicates that the file does not contain base relocations
        /// and must therefore be loaded at its preferred base address.
        /// If the base address is not available, the loader reports an error.
        /// The default behavior of the linker is to strip base relocations from executable (EXE) files.
        /// </summary>
        IMAGE_FILE_RELOCS_STRIPPED = 0x0001,
        /// <summary>
        /// Image only. This indicates that the image file is valid and can be run.
        /// If this flag is not set, it indicates a linker error.
        /// </summary>
        IMAGE_FILE_EXECUTABLE_IMAGE = 0x0002,
        /// <summary>
        /// COFF line numbers have been removed.
        /// This flag is deprecated and should be zero.
        /// </summary>
        IMAGE_FILE_LINE_NUMS_STRIPPED = 0x0004,
        /// <summary>
        /// COFF symbol table entries for local symbols have been removed.
        /// This flag is deprecated and should be zero.
        /// </summary>
        IMAGE_FILE_LOCAL_SYMS_STRIPPED = 0x0008,
        /// <summary>
        /// Obsolete. Aggressively trim working set.
        /// This flag is deprecated for Windows 2000 and later and must be zero.
        /// </summary>
        IMAGE_FILE_AGGRESSIVE_WS_TRIM = 0x0010,
        /// <summary>
        /// Application can handle addresses larger than 2-GB.
        /// </summary>
        IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x0020,
        /// <summary>
        /// This flag is reserved for future use.
        /// </summary>
        RES0 = 0x0040,
        /// <summary>
        /// Little endian:
        /// the least significant bit (LSB) precedes the most significant bit (MSB) in memory.
        /// This flag is deprecated and should be zero.
        /// </summary>
        IMAGE_FILE_BYTES_REVERSED_LO = 0x0080,
        /// <summary>
        /// Machine is based on a 32-bit-word architecture.
        /// </summary>
        IMAGE_FILE_32BIT_MACHINE = 0x0100,
        /// <summary>
        /// Debugging information is removed from the image file.
        /// </summary>
        IMAGE_FILE_DEBUG_STRIPPED = 0x0200,
        /// <summary>
        /// If the image is on removable media,
        /// fully load it and copy it to the swap file.
        /// </summary>
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = 0x0400,
        /// <summary>
        /// If the image is on network media,
        /// fully load it and copy it to the swap file.
        /// </summary>
        IMAGE_FILE_NET_RUN_FROM_SWAP = 0x0800,
        /// <summary>
        /// The image file is a system file, not a user program.
        /// </summary>
        IMAGE_FILE_SYSTEM = 0x1000,
        /// <summary>
        /// The image file is a dynamic-link library (DLL).
        /// Such files are considered executable files for almost all purposes,
        /// although they cannot be directly run.
        /// </summary>
        IMAGE_FILE_DLL = 0x2000,
        /// <summary>
        /// The file should be run only on a uniprocessor machine.
        /// </summary>
        IMAGE_FILE_UP_SYSTEM_ONLY = 0x4000,
        /// <summary>
        /// Big endian: the MSB precedes the LSB in memory.
        /// This flag is deprecated and should be zero.
        /// </summary>
        IMAGE_FILE_BYTES_REVERSED_HI = 0x8000
    }

    /// <summary>
    /// Windows type this executable runs on
    /// </summary>
    public enum PEWindowsSubsystem : ushort
    {
        /// <summary>
        /// An unknown subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_UNKNOWN = 0,
        /// <summary>
        /// Device drivers and native Windows processes
        /// </summary>
        IMAGE_SUBSYSTEM_NATIVE = 1,
        /// <summary>
        /// The Windows graphical user interface (GUI) subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_GUI = 2,
        /// <summary>
        /// The Windows character subsystem
        /// </summary>
        /// <remarks>Fancy saying for "Console Application"</remarks>
        IMAGE_SUBSYSTEM_WINDOWS_CUI = 3,
        /// <summary>
        /// The OS/2 character subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_OS2_CUI = 5,
        /// <summary>
        /// The Posix character subsystem
        /// </summary>
        IMAGE_SUBSYSTEM_POSIX_CUI = 7,
        /// <summary>
        /// Native Win9x driver
        /// </summary>
        IMAGE_SUBSYSTEM_NATIVE_WINDOWS = 8,
        /// <summary>
        /// Windows CE
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9,
        /// <summary>
        /// An Extensible Firmware Interface (EFI) application
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_APPLICATION = 10,
        /// <summary>
        /// An EFI driver with boot services
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,
        /// <summary>
        /// An EFI driver with run-time services
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12,
        /// <summary>
        /// An EFI ROM image
        /// </summary>
        IMAGE_SUBSYSTEM_EFI_ROM = 13,
        /// <summary>
        /// XBOX
        /// </summary>
        IMAGE_SUBSYSTEM_XBOX = 14,
        /// <summary>
        /// Windows boot application.
        /// </summary>
        IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION = 16
    }

    /// <summary>
    /// PE Format
    /// </summary>
    public enum PEOptionalHeaderType : ushort
    {
        /// <summary>
        /// A ROM Image
        /// </summary>
        ROM = 0x107,
        /// <summary>
        /// Standard EXE file
        /// </summary>
        PE = 0x10b,
        /// <summary>
        /// EXE file with extensions and some 64 bit fields
        /// </summary>
        PEPlus = 0x20b
    }

    /// <summary>
    /// Type of a Data Directory Entry
    /// </summary>
    public enum DataDirectoryEntryType : int
    {
        /// <summary>
        /// The export table address and size.
        /// </summary>
        ExportTable,
        /// <summary>
        /// The import table address and size.
        /// </summary>
        ImportTable,
        /// <summary>
        /// The resource table address and size.
        /// </summary>
        ResourceTable,
        /// <summary>
        /// The exception table address and size.
        /// </summary>
        ExceptionTable,
        /// <summary>
        /// The attribute certificate table address and size.
        /// </summary>
        CertificateTable,
        /// <summary>
        /// The base relocation table address and size.
        /// </summary>
        BaseRelocationTable,
        /// <summary>
        /// The debug data starting address and size.
        /// </summary>
        DebugTyble,
        /// <summary>
        /// Reserved, must be 0
        /// </summary>
        ArchitectureTable,
        /// <summary>
        /// The RVA of the value to be stored in the global pointer register.
        /// The size member of this structure must be set to zero.
        /// </summary>
        GlobalPtrTable,
        /// <summary>
        /// The thread local storage (TLS) table address and size.
        /// </summary>
        TLSTable,
        /// <summary>
        /// The load configuration table address and size.
        /// </summary>
        LoadConfigTable,
        /// <summary>
        /// The bound import table address and size.
        /// </summary>
        BoundImportTable,
        /// <summary>
        /// The import address table address and size.
        /// </summary>
        IATTable,
        /// <summary>
        /// The delay import descriptor address and size.
        /// </summary>
        DelayImportDescriptorTable,
        /// <summary>
        /// The CLR runtime header address and size.
        /// </summary>
        CLRRuntimeHeaderTable,
        /// <summary>
        /// Reserved, must be 0
        /// </summary>
        Reserved,
        Unknown = int.MaxValue
    }

    /// <summary>
    /// The flags that describe the characteristics of a section
    /// </summary>
    [Flags]
    public enum PESectionFlags : uint
    {
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RES0 = 0x00000000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RES1 = 0x00000001,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RES2 = 0x00000002,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RES3 = 0x00000004,
        /// <summary>
        /// The section should not be padded to the next boundary.
        /// This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES.
        /// This is valid only for object files.
        /// </summary>
        IMAGE_SCN_TYPE_NO_PAD = 0x00000008,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RES4 = 0x00000010,
        /// <summary>
        /// The section contains executable code.
        /// </summary>
        IMAGE_SCN_CNT_CODE = 0x00000020,
        /// <summary>
        /// The section contains initialized data.
        /// </summary>
        IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040,
        /// <summary>
        /// The section contains uninitialized data.
        /// </summary>
        IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_LNK_OTHER = 0x00000100,
        /// <summary>
        /// The section contains comments or other information.
        /// The ".drectve" section has this type.
        /// This is valid for object files only.
        /// </summary>
        IMAGE_SCN_LNK_INFO = 0x00000200,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RES5 = 0x00000400,
        /// <summary>
        /// The section will not become part of the image.
        /// This is valid only for object files.
        /// </summary>
        IMAGE_SCN_LNK_REMOVE = 0x00000800,
        /// <summary>
        /// The section contains COMDAT data.
        /// </summary>
        IMAGE_SCN_LNK_COMDAT = 0x00001000,
        /// <summary>
        /// The section contains data referenced through the global pointer (GP).
        /// </summary>
        IMAGE_SCN_GPREL = 0x00008000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_PURGEABLE = 0x00020000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_16BIT = 0x00020000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_LOCKED = 0x00040000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_PRELOAD = 0x00080000,
        /// <summary>
        /// Align data on a 1-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_1BYTES = 0x00100000,
        /// <summary>
        /// Align data on a 2-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_2BYTES = 0x00200000,
        /// <summary>
        /// Align data on a 4-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_4BYTES = 0x00300000,
        /// <summary>
        /// Align data on a 8-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_8BYTES = 0x00400000,
        /// <summary>
        /// Align data on a 16-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_16BYTES = 0x00500000,
        /// <summary>
        /// Align data on a 32-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_32BYTES = 0x00600000,
        /// <summary>
        /// Align data on a 64-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_64BYTES = 0x00700000,
        /// <summary>
        /// Align data on a 128-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_128BYTES = 0x00800000,
        /// <summary>
        /// Align data on a 256-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_256BYTES = 0x00900000,
        /// <summary>
        /// Align data on a 512-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_512BYTES = 0x00A00000,
        /// <summary>
        /// Align data on a 1024-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000,
        /// <summary>
        /// Align data on a 2048-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000,
        /// <summary>
        /// Align data on a 4096-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000,
        /// <summary>
        /// Align data on a 8192-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000,
        /// <summary>
        /// The section contains extended relocations.
        /// </summary>
        IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000,
        /// <summary>
        /// The section can be discarded as needed.
        /// </summary>
        IMAGE_SCN_MEM_DISCARDABLE = 0x02000000,
        /// <summary>
        /// The section cannot be cached.
        /// </summary>
        IMAGE_SCN_MEM_NOT_CACHED = 0x04000000,
        /// <summary>
        /// The section is not pageable.
        /// </summary>
        IMAGE_SCN_MEM_NOT_PAGED = 0x08000000,
        /// <summary>
        /// The section can be shared in memory.
        /// </summary>
        IMAGE_SCN_MEM_SHARED = 0x10000000,
        /// <summary>
        /// The section can be executed as code.
        /// </summary>
        IMAGE_SCN_MEM_EXECUTE = 0x20000000,
        /// <summary>
        /// The section can be read.
        /// </summary>
        IMAGE_SCN_MEM_READ = 0x40000000,
        /// <summary>
        /// The section can be written to.
        /// </summary>
        IMAGE_SCN_MEM_WRITE = 0x80000000
    }

    #endregion

    #region OPTIONAL_HEADER

    /// <summary>
    /// Encapsulates the Optional Header
    /// </summary>
    public class OptionalHeaderData
    {
        /// <summary>
        /// Fields that are defined for all implementations of COFF, including UNIX.
        /// </summary>
        public OptionalStandardFields StandardFields
        { get; set; }
        /// <summary>
        /// Additional fields to support specific features of Windows (for example, subsystems).
        /// </summary>
        public OptionalWindowsFields WindowsFields
        { get; set; }
        /// <summary>
        /// Address/size pairs for special tables that are found in the image file
        /// and are used by the operating system (for example, the import table and the export table).
        /// </summary>
        public OptionalDataDirectories DataDirectories
        { get; set; }

        /// <summary>
        /// The unsigned integer that identifies the state of the image file.
        /// The most common number is 0x10B, which identifies it as a normal executable file.
        /// 0x107 identifies it as a ROM image, and 0x20B identifies it as a PE32+ executable.
        /// </summary>
        public PEOptionalHeaderType OptionalHeaderType
        { get; set; }

        /// <summary>
        /// String Version of <see cref="OptionalHeaderType"/> 
        /// </summary>
        public string OptionalHeaderTypeName
        {
            get
            {
                return OptionalHeaderType.ToString();
            }
        }

        public OptionalHeaderData(BinaryReader BR)
        {
            OptionalHeaderType = (PEOptionalHeaderType)BR.ReadUInt16();
            if (!Enum.IsDefined(OptionalHeaderType.GetType(), OptionalHeaderType))
            {
                throw new InvalidDataException($"Unknown PE format: {OptionalHeaderType}");
            }
            StandardFields = new OptionalStandardFields(BR, OptionalHeaderType);
            WindowsFields = new OptionalWindowsFields(BR, OptionalHeaderType);
            DataDirectories = new OptionalDataDirectories(BR, (int)WindowsFields.NumberOfRvaAndSizes);
        }

        public void WriteOptionalHeaders(BinaryWriter BW)
        {
            WindowsFields.NumberOfRvaAndSizes = (uint)DataDirectories.Entries.Length;
            BW.Write((ushort)OptionalHeaderType);
            StandardFields.WriteOptionalStandardFields(BW, OptionalHeaderType);
            WindowsFields.WriteOptionalWindowsFields(BW, OptionalHeaderType);
            DataDirectories.WriteDataDirectories(BW);
        }
    }

    /// <summary>
    /// Encapsulates the optional header standard fields
    /// </summary>
    public class OptionalStandardFields
    {
        private byte _MajorLinkerVersion, _MinorLikerVersion;

        /// <summary>
        /// The linker version number
        /// </summary>
        public Version LinkerVersion
        {
            get
            {
                return new Version(_MajorLinkerVersion, _MinorLikerVersion);
            }
            set
            {
                _MajorLinkerVersion = (byte)(value.Major & 0xFF);
                _MinorLikerVersion = (byte)(value.Minor & 0xFF);
            }
        }

        /// <summary>
        /// The size of the code (text) section,
        /// or the sum of all code sections if there are multiple sections.
        /// </summary>
        public uint SizeOfCode
        { get; set; }

        /// <summary>
        /// The size of the initialized data section,
        /// or the sum of all such sections if there are multiple data sections.
        /// </summary>
        public uint SizeOfInitializedData
        { get; set; }

        /// <summary>
        /// The size of the uninitialized data section (BSS),
        /// or the sum of all such sections if there are multiple BSS sections.
        /// </summary>
        public uint SizeOfUninitializedData
        { get; set; }

        /// <summary>
        /// The address of the entry point relative to the image base when
        /// the executable file is loaded into memory.
        /// For program images, this is the starting address. For device drivers,
        /// this is the address of the initialization function. An entry point is optional for DLLs.
        /// When no entry point is present, this field must be zero.
        /// </summary>
        public uint AddressOfEntryPoint
        { get; set; }

        /// <summary>
        /// The address that is relative to the image base of the beginning-of-code section
        /// when it is loaded into memory.
        /// </summary>
        public uint BaseOfCode
        { get; set; }

        /// <summary>
        /// The address that is relative to the image base of the beginning-of-data section
        /// when it is loaded into memory.
        /// </summary>
        /// <remarks>This is a <see cref="PEOptionalHeaderType.PEPlus"/> only field</remarks>
        public uint BaseOfData
        { get; set; }

        public OptionalStandardFields(BinaryReader BR, PEOptionalHeaderType HeaderType)
        {
            _MajorLinkerVersion = BR.ReadByte();
            _MinorLikerVersion = BR.ReadByte();
            SizeOfCode = BR.ReadUInt32();
            SizeOfInitializedData = BR.ReadUInt32();
            SizeOfUninitializedData = BR.ReadUInt32();
            AddressOfEntryPoint = BR.ReadUInt32();
            BaseOfCode = BR.ReadUInt32();
            if (HeaderType == PEOptionalHeaderType.PE)
            {
                BaseOfData = BR.ReadUInt32();
            }
        }

        public void WriteOptionalStandardFields(BinaryWriter BW, PEOptionalHeaderType HeaderType)
        {
            BW.Write(_MajorLinkerVersion);
            BW.Write(_MinorLikerVersion);
            BW.Write(SizeOfCode);
            BW.Write(SizeOfInitializedData);
            BW.Write(SizeOfUninitializedData);
            BW.Write(AddressOfEntryPoint);
            BW.Write(BaseOfCode);
            if (HeaderType == PEOptionalHeaderType.PE)
            {
                BW.Write(BaseOfData);
            }
        }
    }

    /// <summary>
    /// Encapsulates the optional header Windows fields
    /// </summary>
    public class OptionalWindowsFields
    {
        /// <summary>
        /// Minimum Required OS Version to run
        /// </summary>
        public Version OperatingSystemVersion
        {
            get
            {
                return new Version(MajorOperatingSystemVersion, MinorOperatingSystemVersion);
            }
            set
            {
                MajorOperatingSystemVersion = (ushort)(value.Major & 0xFFFF);
                MinorOperatingSystemVersion = (ushort)(value.Minor & 0xFFFF);
            }
        }

        /// <summary>
        /// Image Version
        /// </summary>
        public Version ImageVersion
        {
            get
            {
                return new Version(MajorImageVersion, MinorImageVersion);
            }
            set
            {
                MajorImageVersion = (ushort)(value.Major & 0xFFFF);
                MinorImageVersion = (ushort)(value.Minor & 0xFFFF);
            }
        }

        /// <summary>
        /// Subsystem Version
        /// </summary>
        public Version SubsystemVersion
        {
            get
            {
                return new Version(MajorSubsystemVersion, MinorSubsystemVersion);
            }
            set
            {
                MajorSubsystemVersion = (ushort)(value.Major & 0xFFFF);
                MinorSubsystemVersion = (ushort)(value.Minor & 0xFFFF);
            }
        }

        /// <summary>
        /// The preferred address of the first byte of image when loaded into memory;
        /// must be a multiple of 64 K. The default for DLLs is 0x10000000.
        /// The default for Windows CE EXEs is 0x00010000. The default for Windows NT,
        /// Windows 2000, Windows XP, Windows 95, Windows 98, and Windows Me is 0x00400000.
        /// </summary>
        public ulong ImageBase
        { get; set; }

        /// <summary>
        /// The alignment (in bytes) of sections when they are loaded into memory.
        /// It must be greater than or equal to FileAlignment.
        /// The default is the page size for the architecture. 
        /// </summary>
        public uint SectionAlignment
        { get; set; }

        /// <summary>
        /// The alignment factor (in bytes) that is used to align the raw data of sections in the image file.
        /// The value should be a power of 2 between 512 and 64 K, inclusive.
        /// The default is 512. If the SectionAlignment is less than the architecture's page size,
        /// then FileAlignment must match SectionAlignment. 
        /// </summary>
        public uint FileAlignment
        { get; set; }

        /// <summary>
        /// The major version number of the required operating system.
        /// </summary>
        public ushort MajorOperatingSystemVersion
        { get; set; }

        /// <summary>
        /// The minor version number of the required operating system.
        /// </summary>
        public ushort MinorOperatingSystemVersion
        { get; set; }

        /// <summary>
        /// The major version number of the image.
        /// </summary>
        public ushort MajorImageVersion
        { get; set; }

        /// <summary>
        /// The minor version number of the image.
        /// </summary>
        public ushort MinorImageVersion
        { get; set; }

        /// <summary>
        /// The major version number of the subsystem.
        /// </summary>
        public ushort MajorSubsystemVersion
        { get; set; }

        /// <summary>
        /// The minor version number of the subsystem.
        /// </summary>
        public ushort MinorSubsystemVersion
        { get; set; }

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint Win32VersionValue
        { get; set; }

        /// <summary>
        /// The size (in bytes) of the image, including all headers, as the image is loaded in memory.
        /// It must be a multiple of SectionAlignment.
        /// </summary>
        public uint SizeOfImage
        { get; set; }

        /// <summary>
        /// The combined size of an MS-DOS stub, PE header,
        /// and section headers rounded up to a multiple of FileAlignment.
        /// </summary>
        public uint SizeOfHeaders
        { get; set; }

        /// <summary>
        /// The image file checksum. The algorithm for computing the checksum is incorporated into IMAGHELP.DLL.
        /// The following are checked for validation at load time:
        /// all drivers, any DLL loaded at boot time,
        /// and any DLL that is loaded into a critical Windows process.
        /// </summary>
        public uint CheckSum
        { get; set; }

        /// <summary>
        /// The subsystem that is required to run this image.
        /// </summary>
        public PEWindowsSubsystem Subsystem
        { get; set; }

        /// <summary>
        /// DLL Loading Characteristics
        /// </summary>
        public PEDllCharacteristics DllCharacteristics
        { get; set; }

        /// <summary>
        /// The size of the stack to reserve. Only SizeOfStackCommit is committed;
        /// the rest is made available one page at a time until the reserve size is reached.
        /// </summary>
        public ulong SizeOfStackReserve
        { get; set; }

        /// <summary>
        /// The size of the stack to commit.
        /// </summary>
        public ulong SizeOfStackCommit
        { get; set; }

        /// <summary>
        /// The size of the local heap space to reserve.
        /// Only SizeOfHeapCommit is committed;
        /// the rest is made available one page at a time until the reserve size is reached.
        /// </summary>
        public ulong SizeOfHeapReserve
        { get; set; }

        /// <summary>
        /// The size of the local heap space to commit.
        /// </summary>
        public ulong SizeOfHeapCommit
        { get; set; }

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint LoaderFlags
        { get; set; }

        /// <summary>
        /// The number of data-directory entries in the remainder of the optional header.
        /// Each describes a location and size.
        /// </summary>
        public uint NumberOfRvaAndSizes
        { get; set; }

        /// <summary>
        /// String Version of <see cref="Subsystem"/>
        /// </summary>
        public string SubsystemName
        {
            get
            {
                return Subsystem.ToString();
            }
        }

        /// <summary>
        /// String Version of <see cref="DllCharacteristics"/>
        /// </summary>
        public string[] DllCharacteristicsMap
        {
            get
            {
                return Tools.ExpandFlags(DllCharacteristics);
            }
        }

        public OptionalWindowsFields(BinaryReader BR, PEOptionalHeaderType HeaderType)
        {
            ImageBase = R(BR, HeaderType);
            SectionAlignment = BR.ReadUInt32();
            FileAlignment = BR.ReadUInt32();
            MajorOperatingSystemVersion = BR.ReadUInt16();
            MinorOperatingSystemVersion = BR.ReadUInt16();
            MajorImageVersion = BR.ReadUInt16();
            MinorImageVersion = BR.ReadUInt16();
            MajorSubsystemVersion = BR.ReadUInt16();
            MinorSubsystemVersion = BR.ReadUInt16();
            Win32VersionValue = BR.ReadUInt32();
            SizeOfImage = BR.ReadUInt32();
            SizeOfHeaders = BR.ReadUInt32();
            CheckSum = BR.ReadUInt32();
            Subsystem = (PEWindowsSubsystem)BR.ReadUInt16();
            DllCharacteristics = (PEDllCharacteristics)BR.ReadUInt16();
            SizeOfStackReserve = R(BR, HeaderType);
            SizeOfStackCommit = R(BR, HeaderType);
            SizeOfHeapReserve = R(BR, HeaderType);
            SizeOfHeapCommit = R(BR, HeaderType);
            LoaderFlags = BR.ReadUInt32();
            NumberOfRvaAndSizes = BR.ReadUInt32() & int.MaxValue;
        }

        public void WriteOptionalWindowsFields(BinaryWriter BW, PEOptionalHeaderType HeaderType)
        {
            W(BW, HeaderType, ImageBase);
            BW.Write(SectionAlignment);
            BW.Write(FileAlignment);
            BW.Write(MajorOperatingSystemVersion);
            BW.Write(MinorOperatingSystemVersion);
            BW.Write(MajorImageVersion);
            BW.Write(MinorImageVersion);
            BW.Write(MajorSubsystemVersion);
            BW.Write(MinorSubsystemVersion);
            BW.Write(Win32VersionValue);
            BW.Write(SizeOfImage);
            BW.Write(SizeOfHeaders);
            BW.Write(CheckSum);
            BW.Write((ushort)Subsystem);
            BW.Write((ushort)DllCharacteristics);
            W(BW, HeaderType, SizeOfStackReserve);
            W(BW, HeaderType, SizeOfStackCommit);
            W(BW, HeaderType, SizeOfHeapReserve);
            W(BW, HeaderType, SizeOfHeapCommit);
            BW.Write(LoaderFlags);
            BW.Write(NumberOfRvaAndSizes);
        }

        private void W(BinaryWriter BW, PEOptionalHeaderType HeaderType, ulong Value)
        {
            if (HeaderType == PEOptionalHeaderType.PEPlus)
            {
                BW.Write(Value);
            }
            else
            {
                BW.Write((uint)Value);
            }
        }

        private ulong R(BinaryReader BR, PEOptionalHeaderType HeaderType)
        {
            return HeaderType == PEOptionalHeaderType.PEPlus ? BR.ReadUInt64() : BR.ReadUInt32();
        }
    }

    /// <summary>
    /// Encapsulates the optional header data directories
    /// </summary>
    public class OptionalDataDirectories
    {
        /// <summary>
        /// The data directories, which form the last part of the optional header,
        /// are listed here.
        /// </summary>
        public DataDirectoryEntry[] Entries
        { get; set; }

        public OptionalDataDirectories(BinaryReader BR, int NumberOfEntries)
        {
            Entries = Enumerable
                .Range(0, NumberOfEntries)
                .Select(m => new DataDirectoryEntry(BR, (DataDirectoryEntryType)m))
                .ToArray();
        }

        public void WriteDataDirectories(BinaryWriter BW)
        {
            foreach (var D in Entries)
            {
                D.WriteDataDirectory(BW);
            }
        }
    }

    public class DataDirectoryEntry
    {
        /// <summary>
        /// Type of this Data Directory Entry
        /// </summary>
        public DataDirectoryEntryType EntryType
        { get; set; }

        /// <summary>
        /// This is actually the RVA of the table.
        /// The RVA is the address of the table relative to the base address of the image when the table is loaded.
        /// </summary>
        public uint VirtualAddress
        { get; set; }

        /// <summary>
        /// This field gives the size in bytes.
        /// </summary>
        public uint Size
        { get; set; }

        /// <summary>
        /// String Version of <see cref="EntryType"/>
        /// </summary>
        public string EntryTypeName
        {
            get
            {
                return EntryType.ToString();
            }
        }

        public DataDirectoryEntry(BinaryReader BR, DataDirectoryEntryType EntryType)
        {
            this.EntryType = EntryType;
            VirtualAddress = BR.ReadUInt32();
            Size = BR.ReadUInt32();
            //Test this after reading the two integer values so it's sync with the next entry
            if (!Enum.IsDefined(EntryType.GetType(), EntryType))
            {
                EntryType = DataDirectoryEntryType.Unknown;
            }
        }

        public void WriteDataDirectory(BinaryWriter BW)
        {
            BW.Write(VirtualAddress);
            BW.Write(Size);
        }

    }

    #endregion

    /// <summary>
    /// This represents a data section in the file
    /// </summary>
    public class PESection
    {
        /// <summary>
        /// An 8-byte, null-padded UTF-8 encoded string.
        /// If the string is exactly 8 characters long, there is no terminating null.
        /// For longer names, this field contains a slash (/) that is followed
        /// by an ASCII representation of a decimal number that is an offset into the string table.
        /// Executable images do not use a string table and do not support section names longer than 8 characters.
        /// Long names in object files are truncated if they are emitted to an executable file.
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// The total size of the section when loaded into memory.
        /// If this value is greater than SizeOfRawData, the section is zero-padded.
        /// This field is valid only for executable images and should be set to zero for object files.
        /// </summary>
        public uint VirtualSize
        { get; set; }

        /// <summary>
        /// For executable images,
        /// the address of the first byte of the section relative to the image base when
        /// the section is loaded into memory.
        /// For object files, this field is the address of the first byte before relocation is applied;
        /// for simplicity, compilers should set this to zero.
        /// Otherwise, it is an arbitrary value that is subtracted from offsets during relocation.
        /// </summary>
        public uint VirtualAddress
        { get; set; }

        /// <summary>
        /// The size of the section (for object files)
        /// or the size of the initialized data on disk (for image files).
        /// For executable images, this must be a multiple of FileAlignment from the optional header.
        /// If this is less than VirtualSize, the remainder of the section is zero-filled.
        /// Because the SizeOfRawData field is rounded but the VirtualSize field is not,
        /// it is possible for SizeOfRawData to be greater than VirtualSize as well.
        /// When a section contains only uninitialized data, this field should be zero.
        /// </summary>
        public uint SizeOfRawData
        { get; set; }

        /// <summary>
        /// The file pointer to the first page of the section within the COFF file.
        /// For executable images, this must be a multiple of FileAlignment from the optional header.
        /// For object files, the value should be aligned on a 4-byte boundary for best performance.
        /// When a section contains only uninitialized data, this field should be zero.
        /// </summary>
        public uint PointerToRawData
        { get; set; }

        /// <summary>
        /// The file pointer to the beginning of relocation entries for the section.
        /// This is set to zero for executable images or if there are no relocations.
        /// </summary>
        public uint PointerToRelocations
        { get; set; }

        /// <summary>
        /// The file pointer to the beginning of line-number entries for the section.
        /// This is set to zero if there are no COFF line numbers.
        /// This value should be zero for an image because COFF debugging information is deprecated.
        /// </summary>
        public uint PointerToLinenumbers
        { get; set; }

        /// <summary>
        /// The number of relocation entries for the section.
        /// This is set to zero for executable images.
        /// </summary>
        public ushort NumberOfRelocations
        { get; set; }

        /// <summary>
        /// The number of line-number entries for the section.
        /// This value should be zero for an image because COFF debugging information is deprecated.
        /// </summary>
        public ushort NumberOfLinenumbers
        { get; set; }

        /// <summary>
        /// The flags that describe the characteristics of the section.
        /// </summary>
        public PESectionFlags Characteristics
        { get; set; }

        /// <summary>
        /// String Version of <see cref="Characteristics"/>
        /// </summary>
        public string[] CharacteristicsMap
        {
            get
            {
                return Tools.ExpandFlags(Characteristics);
            }
        }

        public PESection(BinaryReader BR)
        {
            //According to MS, the name must be UTF-8 encoded, 8 bytes long and 0 padded
            Name = Encoding.UTF8.GetString(BR.ReadBytes(8)).TrimEnd('\0');
            VirtualSize = BR.ReadUInt32();
            VirtualAddress = BR.ReadUInt32();
            SizeOfRawData = BR.ReadUInt32();
            PointerToRawData = BR.ReadUInt32();
            PointerToRelocations = BR.ReadUInt32();
            PointerToLinenumbers = BR.ReadUInt32();
            NumberOfRelocations = BR.ReadUInt16();
            NumberOfLinenumbers = BR.ReadUInt16();
            Characteristics = (PESectionFlags)BR.ReadUInt32();
        }

        public void WritePESection(BinaryWriter BW)
        {
            //Ensure the name is at least 8 bytes long by right pdding it
            BW.Write(Encoding.UTF8.GetBytes(Name.PadRight(Math.Max(8, Name.Length + 1), '\0')), 0, 8);
            BW.Write(VirtualSize);
            BW.Write(VirtualAddress);
            BW.Write(SizeOfRawData);
            BW.Write(PointerToRawData);
            BW.Write(PointerToRelocations);
            BW.Write(PointerToLinenumbers);
            BW.Write(NumberOfRelocations);
            BW.Write(NumberOfLinenumbers);
            BW.Write((uint)Characteristics);
        }
    }

    /// <summary>
    /// Represents the Header of executable or DLL files
    /// </summary>
    public class PE
    {
        /// <summary>
        /// Offset at which the pointer to the PE Header is
        /// </summary>
        public const int PE_OFFSET_ADDR = 0x3C;

        /// <summary>
        /// The MS-DOS stub is a valid application that runs under MS-DOS.
        /// It is placed at the front of the EXE image.
        /// The linker places a default stub here,
        /// which prints out the message "This program cannot be run in DOS mode"
        /// when the image is run in MS-DOS.
        /// </summary>
        public byte[] DOSStub
        {
            get; set;
        }

        /// <summary>
        /// Offset from File start of the PE header
        /// </summary>
        public int PEOffset
        { get; set; }

        /// <summary>
        /// After the MS-DOS stub, at the file offset specified at offset 0x3c,
        /// is a 4-byte signature that identifies the file as a PE format image file.
        /// This signature is "PE\0\0" (the letters "P" and "E" followed by two null bytes).
        /// </summary>
        public bool ValidPEHeader
        { get; set; }

        /// <summary>
        /// The number that identifies the type of target machine.
        /// </summary>
        public PEMachineType MachineType
        { get; set; }

        /// <summary>
        /// The number of sections.
        /// This indicates the size of the section table, which immediately follows the headers.
        /// </summary>
        public ushort NumberOfSections
        { get; set; }

        /// <summary>
        /// The low 32 bits of the number of seconds since 00:00 January 1, 1970 (a C run-time time_t value),
        /// that indicates when the file was created.
        /// </summary>
        /// <remarks>Also known as a Linux or Unix timestamp</remarks>
        public DateTime CompileTime
        { get; set; }

        /// <summary>
        /// The file offset of the COFF symbol table, or zero if no COFF symbol table is present.
        /// This value should be zero for an image because COFF debugging information is deprecated.
        /// </summary>
        public uint PointerToSymbolTable
        { get; set; }

        /// <summary>
        /// The number of entries in the symbol table.
        /// This data can be used to locate the string table,
        /// which immediately follows the symbol table.
        /// This value should be zero for an image because COFF debugging information is deprecated.
        /// </summary>
        public uint NumberOfSymbols
        { get; set; }

        /// <summary>
        /// The size of the optional header,
        /// which is required for executable files but not for object files.
        /// This value should be zero for an object file.
        /// </summary>
        public ushort SizeOfOptionalHeader
        { get; set; }

        /// <summary>
        /// The flags that indicate the attributes of the file.
        /// </summary>
        public PEImageCharacteristics Characteristics
        { get; set; }

        /// <summary>
        /// Gets if this image has an optional Header
        /// </summary>
        public bool HasOptionalHeader
        { get { return SizeOfOptionalHeader > 0; } }

        /// <summary>
        /// Optional Header
        /// </summary>
        public OptionalHeaderData OptionalHeader
        { get; set; }

        /// <summary>
        /// String Version of <see cref="MachineType"/>
        /// </summary>
        public string MachineTypeName
        {
            get
            {
                return MachineType.ToString();
            }
        }

        /// <summary>
        /// String Version of <see cref="Characteristics"/>
        /// </summary>
        public string[] CharacteristicsMap
        {
            get
            {
                return Tools.ExpandFlags(Characteristics);
            }
        }

        public PESection[] Sections
        {
            get; set;
        }

        public PE(string FileName)
        {
            using (var FS = File.OpenRead(FileName))
            {
                using (var BR = new BinaryReader(FS))
                {
                    FS.Seek(PE_OFFSET_ADDR, SeekOrigin.Begin);
                    PEOffset = BR.ReadInt32();
                    FS.Seek(0, SeekOrigin.Begin);
                    DOSStub = BR.ReadBytes(PEOffset);
                    BR.BaseStream.Seek(PEOffset, SeekOrigin.Begin);
                    ValidPEHeader = BR.ReadBytes(4).SequenceEqual(new byte[] { 0x50, 0x45, 0x00, 0x00 });
                    MachineType = (PEMachineType)BR.ReadUInt16();
                    NumberOfSections = BR.ReadUInt16();
                    var tempTime = BR.ReadUInt32();
                    if (tempTime == uint.MinValue)
                    {
                        CompileTime = DateTime.MinValue;
                    }
                    else if (tempTime == uint.MaxValue)
                    {
                        CompileTime = DateTime.MaxValue;
                    }
                    else
                    {
                        CompileTime = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(tempTime);
                    }
                    PointerToSymbolTable = BR.ReadUInt32();
                    NumberOfSymbols = BR.ReadUInt32();
                    SizeOfOptionalHeader = BR.ReadUInt16();
                    Characteristics = (PEImageCharacteristics)BR.ReadUInt16();
                    if (SizeOfOptionalHeader > 0)
                    {
                        OptionalHeader = new OptionalHeaderData(BR);
                    }
                    else
                    {
                        OptionalHeader = null;
                    }
                    Sections = Enumerable
                        .Range(0, NumberOfSections)
                        .Select(m => new PESection(BR))
                        .ToArray();
                }
            }
        }

        public void WritePEHeader(BinaryWriter BW)
        {
            //Copy of the DOS Stub
            var Stub = (byte[])DOSStub.Clone();

            //Open Stub as memory
            using (var MS = new MemoryStream(Stub, true))
            {
                //Ensure it's at least as long as needed to reach the offset address and the offset itself
                while (MS.Length < PE_OFFSET_ADDR && MS.Length < PEOffset)
                {
                    MS.WriteByte(0);
                }
                //Seek to the offset address
                MS.Seek(PE_OFFSET_ADDR, SeekOrigin.Begin);
                //Write Offset to stream
                MS.Write(BitConverter.GetBytes(PEOffset), 0, 4);
                //Write entire chunk to the output. Position is now at the start of "PE"
                BW.Write(MS.ToArray());
            }
            BW.Write(new byte[] { 0x50, 0x45, 0x00, 0x00 });
            BW.Write((ushort)MachineType);
            BW.Write(NumberOfSections);
            if (CompileTime == DateTime.MinValue)
            {
                BW.Write(uint.MinValue);
            }
            else if (CompileTime == DateTime.MaxValue)
            {
                BW.Write(uint.MaxValue);
            }
            else
            {
                BW.Write((uint)Math.Floor(CompileTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds));
            }
            BW.Write(PointerToSymbolTable);
            BW.Write(NumberOfSymbols);
            BW.Write(SizeOfOptionalHeader);
            BW.Write((ushort)Characteristics);
        }

        public void WriteCompleteHeaders(BinaryWriter BW)
        {
            WritePEHeader(BW);
            if (OptionalHeader != null)
            {
                OptionalHeader.WriteOptionalHeaders(BW);
            }
            foreach (var Sec in Sections)
            {
                Sec.WritePESection(BW);
            }
        }
    }
}
