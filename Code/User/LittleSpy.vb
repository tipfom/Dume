Imports System.Management

Public Class LittleSpy

    Public Shared ReadOnly Property PCSpec() As List(Of String)
        Get
            Dim ReturnedList As New List(Of String)

            ReturnedList.Add("Computer : " & ComputerName)
            ReturnedList.Add("MachineName : " & MachineName)
            ReturnedList.Add("CurrentUser : " & CurrentUser)
            ReturnedList.Add("Language : " & Language)
            ReturnedList.Add("OperatingSystem : " & OperationSystem & "@" & BitRange)
            ReturnedList.Add("BIOS : " & BIOS())
            ReturnedList.Add("ScreenResolution : " & ScreenResolution)
            ReturnedList.Add("Motherboard : " & Motherboard)
            ReturnedList.Add("Processor : " & Processor)
            ReturnedList.Add("Cores : " & ProcessorCores)
            ReturnedList.Add("GraphicsProcessorUnit : " & GraphicsProcessorUnit)
            ReturnedList.Add("RAM : " & RAM)

            ReturnedList.Add("RAMUsage : " & RAMUsage & "MB/" & RAM & "MB")

            Return ReturnedList
        End Get
    End Property

    Public Shared ReadOnly Property ComputerName As String
        Get
            Return My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "SystemVersion", Nothing)
        End Get
    End Property

    Public Shared ReadOnly Property MachineName As String
        Get
            Return System.Environment.MachineName
        End Get
    End Property

    Public Shared ReadOnly Property CurrentUser As String
        Get
            Return System.Environment.UserName
        End Get
    End Property

    Public Shared ReadOnly Property OperationSystem As String
        Get
            Return String.Format("{0}[{1}]", My.Computer.Info.OSFullName, My.Computer.Info.OSVersion)
        End Get
    End Property

    Public Shared ReadOnly Property Language As String
        Get
            Return My.Computer.Info.InstalledUICulture.ToString
        End Get
    End Property

    Public Shared ReadOnly Property ScreenResolution As String
        Get
            Return String.Format("{0}x{1}", My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
        End Get
    End Property

    Public Shared ReadOnly Property BitRange As String
        Get
            Return System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")
        End Get
    End Property

    Public Shared ReadOnly Property Motherboard As String
        Get
            Return String.Format("{0} by {1}", My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "BaseBoardProduct", Nothing), My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "BaseBoardManufacturer", Nothing))
        End Get
    End Property

    Public Shared ReadOnly Property Processor As String
        Get
            Return My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", Nothing)
        End Get
    End Property

    Public Shared ReadOnly Property ProcessorCores As Integer
        Get
            Return System.Environment.ProcessorCount
        End Get
    End Property

    Public Shared ReadOnly Property RAM As Integer
        Get
            Return My.Computer.Info.TotalPhysicalMemory / 1024 / 1024
        End Get
    End Property

    Public Shared ReadOnly Property GraphicsProcessorUnit
        Get
            Dim objQuery As New ObjectQuery("SELECT * FROM Win32_VideoController")

            Using objSearcher As New ManagementObjectSearcher(objQuery)
                For Each MemObj As ManagementObject In objSearcher.Get
                    Return String.Format("{0} with {1} MB", MemObj("Name"), MemObj("AdapterRAM") / 1024 / 1024)
                Next
            End Using
            Return "NONE"
        End Get
    End Property

    Public Shared ReadOnly Property BIOS As String
        Get
            Return String.Format("{1}@{0}[{2}]", My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "BIOSVersion", Nothing), My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "BIOSVendor", Nothing), My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "BIOSReleaseDate", Nothing))
        End Get
    End Property

    Public Shared ReadOnly Property RAMUsage As Integer
        Get
            Return My.Computer.Info.TotalPhysicalMemory / 1024 / 1024 - My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024
        End Get
    End Property
End Class
