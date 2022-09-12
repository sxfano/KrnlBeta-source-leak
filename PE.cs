// PE
// All tree nodes below use the hex editor to modify the PE file
// 
// 00000000 - 0000003F DOS Header
// 
// IMAGE_DOS_HEADER:
// 00000000 - 00000001 5A4D = e_magic
// 00000002 - 00000003 0090 = e_cblp
// 00000004 - 00000005 0003 = e_cp
// 00000006 - 00000007 0000 = e_crlc
// 00000008 - 00000009 0004 = e_cparhdr
// 0000000A - 0000000B 0000 = e_minalloc
// 0000000C - 0000000D FFFF = e_maxalloc
// 0000000E - 0000000F 0000 = e_ss
// 00000010 - 00000011 00B8 = e_sp
// 00000012 - 00000013 0000 = e_csum
// 00000014 - 00000015 0000 = e_ip
// 00000016 - 00000017 0000 = e_cs
// 00000018 - 00000019 0040 = e_lfarlc
// 0000001A - 0000001B 0000 = e_ovno
// 0000001C - 0000001D 0000 = e_res[0]
// 0000001E - 0000001F 0000 = e_res[1]
// 00000020 - 00000021 0000 = e_res[2]
// 00000022 - 00000023 0000 = e_res[3]
// 00000024 - 00000025 0000 = e_oemid
// 00000026 - 00000027 0000 = e_oeminfo
// 00000028 - 00000029 0000 = e_res2[0]
// 0000002A - 0000002B 0000 = e_res2[1]
// 0000002C - 0000002D 0000 = e_res2[2]
// 0000002E - 0000002F 0000 = e_res2[3]
// 00000030 - 00000031 0000 = e_res2[4]
// 00000032 - 00000033 0000 = e_res2[5]
// 00000034 - 00000035 0000 = e_res2[6]
// 00000036 - 00000037 0000 = e_res2[7]
// 00000038 - 00000039 0000 = e_res2[8]
// 0000003A - 0000003B 0000 = e_res2[9]
// 0000003C - 0000003F 00000080 = e_lfanew
// 
// 00000084 - 00000097 File Header
// 
// IMAGE_FILE_HEADER:
// 00000084 - 00000085 014C = Machine
// 00000086 - 00000087 0003 = NumberOfSections
// 00000088 - 0000008B B9A0B003 = TimeDateStamp
// 0000008C - 0000008F 00000000 = PointerToSymbolTable
// 00000090 - 00000093 00000000 = NumberOfSymbols
// 00000094 - 00000095 00E0 = SizeOfOptionalHeader
// 00000096 - 00000097 0022 = Characteristics
// 
// 00000098 - 00000177 Optional Header (32-bit)
// 
// IMAGE_OPTIONAL_HEADER32:
// 00000098 - 00000099 010B = Magic
// 0000009A - 0000009A 30 = MajorLinkerVersion
// 0000009B - 0000009B 00 = MinorLinkerVersion
// 0000009C - 0000009F 000FD600 = SizeOfCode
// 000000A0 - 000000A3 0001AA00 = SizeOfInitializedData
// 000000A4 - 000000A7 00000000 = SizeOfUninitializedData
// 000000A8 - 000000AB 000FF4D6 = AddressOfEntryPoint
// 000000AC - 000000AF 00002000 = BaseOfCode
// 000000B0 - 000000B3 00100000 = BaseOfData
// 000000B4 - 000000B7 00400000 = ImageBase
// 000000B8 - 000000BB 00002000 = SectionAlignment
// 000000BC - 000000BF 00000200 = FileAlignment
// 000000C0 - 000000C1 0004 = MajorOperatingSystemVersion
// 000000C2 - 000000C3 0000 = MinorOperatingSystemVersion
// 000000C4 - 000000C5 0000 = MajorImageVersion
// 000000C6 - 000000C7 0000 = MinorImageVersion
// 000000C8 - 000000C9 0006 = MajorSubsystemVersion
// 000000CA - 000000CB 0000 = MinorSubsystemVersion
// 000000CC - 000000CF 00000000 = Win32VersionValue
// 000000D0 - 000000D3 0011E000 = SizeOfImage
// 000000D4 - 000000D7 00000200 = SizeOfHeaders
// 000000D8 - 000000DB 00120CD5 = CheckSum
// 000000DC - 000000DD 0002 = Subsystem
// 000000DE - 000000DF 8560 = DllCharacteristics
// 000000E0 - 000000E3 00100000 = SizeOfStackReserve
// 000000E4 - 000000E7 00001000 = SizeOfStackCommit
// 000000E8 - 000000EB 00100000 = SizeOfHeapReserve
// 000000EC - 000000EF 00001000 = SizeOfHeapCommit
// 000000F0 - 000000F3 00000000 = LoaderFlags
// 000000F4 - 000000F7 00000010 = NumberOfRvaAndSizes
// 000000F8 - 000000FB 00000000 = Export.VirtualAddress
// 000000FC - 000000FF 00000000 = Export.Size
// 00000100 - 00000103 000FF482 = Import.VirtualAddress
// 00000104 - 00000107 0000004F = Import.Size
// 00000108 - 0000010B 00100000 = Resource.VirtualAddress
// 0000010C - 0000010F 0001A758 = Resource.Size
// 00000110 - 00000113 00000000 = Exception.VirtualAddress
// 00000114 - 00000117 00000000 = Exception.Size
// 00000118 - 0000011B 00118200 = Security.VirtualAddress
// 0000011C - 0000011F 00002DB0 = Security.Size
// 00000120 - 00000123 0011C000 = Base Reloc.VirtualAddress
// 00000124 - 00000127 0000000C = Base Reloc.Size
// 00000128 - 0000012B 000FF3F0 = Debug.VirtualAddress
// 0000012C - 0000012F 00000038 = Debug.Size
// 00000130 - 00000133 00000000 = Architecture.VirtualAddress
// 00000134 - 00000137 00000000 = Architecture.Size
// 00000138 - 0000013B 00000000 = Global Ptr.VirtualAddress
// 0000013C - 0000013F 00000000 = Global Ptr.Size
// 00000140 - 00000143 00000000 = TLS.VirtualAddress
// 00000144 - 00000147 00000000 = TLS.Size
// 00000148 - 0000014B 00000000 = Load Config.VirtualAddress
// 0000014C - 0000014F 00000000 = Load Config.Size
// 00000150 - 00000153 00000000 = Bound Import.VirtualAddress
// 00000154 - 00000157 00000000 = Bound Import.Size
// 00000158 - 0000015B 00002000 = IAT.VirtualAddress
// 0000015C - 0000015F 00000008 = IAT.Size
// 00000160 - 00000163 00000000 = Delay Import.VirtualAddress
// 00000164 - 00000167 00000000 = Delay Import.Size
// 00000168 - 0000016B 00002008 = .NET.VirtualAddress
// 0000016C - 0000016F 00000048 = .NET.Size
// 00000170 - 00000173 00000000 = Reserved15.VirtualAddress
// 00000174 - 00000177 00000000 = Reserved15.Size
// 
// 00000178 - 0000019F Section #0: .text
// 
// IMAGE_SECTION_HEADER:
// 00000178 - 0000017F .text = Name
// 00000180 - 00000183 000FD4DC = VirtualSize
// 00000184 - 00000187 00002000 = VirtualAddress
// 00000188 - 0000018B 000FD600 = SizeOfRawData
// 0000018C - 0000018F 00000200 = PointerToRawData
// 00000190 - 00000193 00000000 = PointerToRelocations
// 00000194 - 00000197 00000000 = PointerToLinenumbers
// 00000198 - 00000199 0000 = NumberOfRelocations
// 0000019A - 0000019B 0000 = NumberOfLinenumbers
// 0000019C - 0000019F 60000020 = Characteristics
// 
// 000001A0 - 000001C7 Section #1: .rsrc
// 
// IMAGE_SECTION_HEADER:
// 000001A0 - 000001A7 .rsrc = Name
// 000001A8 - 000001AB 0001A758 = VirtualSize
// 000001AC - 000001AF 00100000 = VirtualAddress
// 000001B0 - 000001B3 0001A800 = SizeOfRawData
// 000001B4 - 000001B7 000FD800 = PointerToRawData
// 000001B8 - 000001BB 00000000 = PointerToRelocations
// 000001BC - 000001BF 00000000 = PointerToLinenumbers
// 000001C0 - 000001C1 0000 = NumberOfRelocations
// 000001C2 - 000001C3 0000 = NumberOfLinenumbers
// 000001C4 - 000001C7 40000040 = Characteristics
// 
// 000001C8 - 000001EF Section #2: .reloc
// 
// IMAGE_SECTION_HEADER:
// 000001C8 - 000001CF .reloc = Name
// 000001D0 - 000001D3 0000000C = VirtualSize
// 000001D4 - 000001D7 0011C000 = VirtualAddress
// 000001D8 - 000001DB 00000200 = SizeOfRawData
// 000001DC - 000001DF 00118000 = PointerToRawData
// 000001E0 - 000001E3 00000000 = PointerToRelocations
// 000001E4 - 000001E7 00000000 = PointerToLinenumbers
// 000001E8 - 000001E9 0000 = NumberOfRelocations
// 000001EA - 000001EB 0000 = NumberOfLinenumbers
// 000001EC - 000001EF 42000040 = Characteristics
// 
// 00000208 - 0000024F Cor20 Header
// 
// IMAGE_COR20_HEADER:
// 00000208 - 0000020B 00000048 = cb
// 0000020C - 0000020D 0002 = MajorRuntimeVersion
// 0000020E - 0000020F 0005 = MinorRuntimeVersion
// 00000210 - 00000213 0000A83C = MetaData.VirtualAddress
// 00000214 - 00000217 000099BC = MetaData.Size
// 00000218 - 0000021B 00020003 = Flags
// 0000021C - 0000021F 06000008 = EntryPointTokenOrRVA
// 00000220 - 00000223 000141F8 = Resources.VirtualAddress
// 00000224 - 00000227 000EB1F8 = Resources.Size
// 00000228 - 0000022B 00000000 = StrongNameSignature.VirtualAddress
// 0000022C - 0000022F 00000000 = StrongNameSignature.Size
// 00000230 - 00000233 00000000 = CodeManagerTable.VirtualAddress
// 00000234 - 00000237 00000000 = CodeManagerTable.Size
// 00000238 - 0000023B 00000000 = VTableFixups.VirtualAddress
// 0000023C - 0000023F 00000000 = VTableFixups.Size
// 00000240 - 00000243 00000000 = ExportAddressTableJumps.VirtualAddress
// 00000244 - 00000247 00000000 = ExportAddressTableJumps.Size
// 00000248 - 0000024B 00000000 = ManagedNativeHeader.VirtualAddress
// 0000024C - 0000024F 00000000 = ManagedNativeHeader.Size
// 
// 00008A3C - 00008A57 Storage Signature
// 
// STORAGESIGNATURE:
// 00008A3C - 00008A3F 424A5342 = lSignature
// 00008A40 - 00008A41 0001 = iMajorVer
// 00008A42 - 00008A43 0001 = iMinorVer
// 00008A44 - 00008A47 00000000 = iExtraData
// 00008A48 - 00008A4B 0000000C = iVersionString
// 00008A4C - 00008A57 v4.0.30319 = VersionString
// 
// 00008A58 - 00008A5B Storage Header
// 
// STORAGEHEADER:
// 00008A58 - 00008A58 00 = fFlags
// 00008A59 - 00008A59 00 = pad
// 00008A5A - 00008A5B 0005 = iStreams
// 
// 00008A5C - 00008A67 Storage Stream #0: #~
// 
// STORAGESTREAM:
// 00008A5C - 00008A5F 0000006C = iOffset
// 00008A60 - 00008A63 00003C3C = iSize
// 00008A64 - 00008A67 #~ = rcName
// 
// 00008A68 - 00008A7B Storage Stream #1: #Strings
// 
// STORAGESTREAM:
// 00008A68 - 00008A6B 00003CA8 = iOffset
// 00008A6C - 00008A6F 00003D1C = iSize
// 00008A70 - 00008A7B #Strings = rcName
// 
// 00008A7C - 00008A87 Storage Stream #2: #US
// 
// STORAGESTREAM:
// 00008A7C - 00008A7F 000079C4 = iOffset
// 00008A80 - 00008A83 00001054 = iSize
// 00008A84 - 00008A87 #US = rcName
// 
// 00008A88 - 00008A97 Storage Stream #3: #GUID
// 
// STORAGESTREAM:
// 00008A88 - 00008A8B 00008A18 = iOffset
// 00008A8C - 00008A8F 00000010 = iSize
// 00008A90 - 00008A97 #GUID = rcName
// 
// 00008A98 - 00008AA7 Storage Stream #4: #Blob
// 
// STORAGESTREAM:
// 00008A98 - 00008A9B 00008A28 = iOffset
// 00008A9C - 00008A9F 00000F94 = iSize
// 00008AA0 - 00008AA7 #Blob = rcName
