Option Strict On
Option Explicit On

Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices
Imports Rocket.FileIO

''' <summary>
''' For communicating with HID-class USB devices.
''' The ReportIn class handles Input reports and Feature reports that carry data to the host.
''' The ReportOut class handles Output reports and Feature reports that that carry data to the device.
''' Other routines retrieve information about and configure the HID.
''' </summary>
''' 
Partial Friend NotInheritable Class Hid

	' Used in error messages.

	Const MODULE_NAME As String = "Hid"

	Friend Capabilities As HIDP_CAPS
	Friend DeviceAttributes As HIDD_ATTRIBUTES

	' For viewing results of API calls in debug.write statements:

    'Shared MyDebugging As New Debugging()

	''' <summary>
	''' reads a Feature report from the device.
	''' </summary>
	''' 
	''' <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>
	''' <param name="inFeatureReportBuffer"> contains the requested report.</param>
	''' <returns> read success</returns>

	Friend Function GetFeatureReport _
	(ByVal hidHandle As SafeFileHandle, _
	ByRef inFeatureReportBuffer() As Byte) _
	As Boolean

		Dim success As Boolean

		Try
			' ***
			' API function: HidD_GetFeature
			' Attempts to read a Feature report from the device.

			' Requires:
			' A handle to a HID
			' A pointer to a buffer containing the report ID and report
			' The size of the buffer. 

			' Returns: true on success, false on failure.
			' ***

			success = HidD_GetFeature _
			   (hidHandle, _
			   inFeatureReportBuffer, _
			   inFeatureReportBuffer.Length)

			Debug.Print("HidD_GetFeature success = " & success)
			Return success

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function

	''' <summary>
	''' reads an Input report from the device using a control transfer.
	''' </summary>	 
	''' <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>
	''' <param name="inputReportBuffer"> contains the requested report. </param>
	''' <returns> read success </returns>

	Friend Function GetInputReportViaControlTransfer _
	(ByVal hidHandle As SafeFileHandle, _
	 ByRef inputReportBuffer() As Byte) _
	 As Boolean

		Dim success As Boolean

		Try
			' ***
			' API function: HidD_GetInputReport

			' Purpose: Attempts to read an Input report from the device using a control transfer.
			' Supported under Windows XP and later only.

			' Requires:
			' A handle to a HID
			' A pointer to a buffer containing the report ID and report
			' The size of the buffer. 

			' Returns: true on success, false on failure.
			' ***

			success = HidD_GetInputReport _
			   (hidHandle, _
			   inputReportBuffer, _
			   inputReportBuffer.Length)

			Debug.Print("HidD_GetInputReport success = " & success)
			Return success

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function

	''' <summary>
	''' writes a Feature report to the device.
	''' </summary>
	''' 
	''' <param name="hidHandle"> handle to the device.  </param>
	''' <param name="outFeatureReportBuffer"> contains the report ID and report data. </param>
	''' 
	''' <returns>
	'''  True on success. False on failure.
	''' </returns>

	Friend Function SendFeatureReport _
	 (ByVal hidHandle As SafeFileHandle, _
	 ByVal outFeatureReportBuffer() As Byte) _
	 As Boolean

		Dim success As Boolean

		Try
			' ***
			' API function: HidD_SetFeature

			' Purpose: Attempts to send a Feature report to the device.

			' Accepts:
			' A handle to a HID
			' A pointer to a buffer containing the report ID and report
			' The size of the buffer. 

			' Returns: true on success, false on failure.
			' ***

			success = HidD_SetFeature _
			 (hidHandle, _
			 outFeatureReportBuffer, _
			 outFeatureReportBuffer.Length)

			Debug.Print("HidD_SetFeature success = " & success)

			Return success

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function



	''' <summary>
	''' writes an Output report to the device using a control transfer.
	''' </summary>
	''' 
	''' <param name="hidHandle"> handle to the device.  </param>
	''' <param name="outputReportBuffer"> contains the report ID and report data. </param> 
	''' <returns>
	'''  True on success. False on failure.
	''' </returns>

	Friend Function SendOutputReportViaControlTransfer _
	  (ByVal hidHandle As SafeFileHandle, _
	   ByVal outputReportBuffer() As Byte) _
	   As Boolean

		Dim success As Boolean

		Try
			' ***
			' API function: HidD_SetOutputReport

			' Purpose: 
			' Attempts to send an Output report to the device using a control transfer.
			' Requires Windows XP or later.

			' Accepts:
			' A handle to a HID
			' A pointer to a buffer containing the report ID and report
			' The size of the buffer. 

			' Returns: true on success, false on failure.
			' ***

			success = HidD_SetOutputReport _
			 (hidHandle, _
			 outputReportBuffer(0), _
			 outputReportBuffer.Length)

			Debug.Print("HidD_SetOutputReport success = " & success)

			Return success

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function


	''' <summary>
	''' Remove any Input reports waiting in the buffer.
	''' </summary>
	''' 
	''' <param name="hidHandle"> a handle to a device.   </param>
	''' 
	''' <returns>
	''' True on success, False on failure.
	''' </returns>

	Friend Function FlushQueue _
	 (ByVal hidHandle As SafeFileHandle) _
	 As Boolean

		Dim success As Boolean

		Try
			' ***
			' API function: HidD_FlushQueue

			' Purpose: Removes any Input reports waiting in the buffer.

			' Accepts: a handle to the device.

			' Returns: True on success, False on failure.
			' ***

			success = HidD_FlushQueue(hidHandle)

			Return success

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function

	''' <summary>
	''' Retrieves a structure with information about a device's capabilities. 
	''' </summary>
	''' 
	''' <param name="hidHandle"> a handle to a device. </param>
	''' 
	''' <returns>
	''' An HIDP_CAPS structure.
	''' </returns>

	Friend Function GetDeviceCapabilities _
	 (ByVal hidHandle As SafeFileHandle) _
	 As HIDP_CAPS

		Dim preparsedData As IntPtr
		Dim result As Int32
		Dim success As Boolean

		Try

			' ***
			' API function: HidD_GetPreparsedData

			' Purpose: retrieves a pointer to a buffer containing information about the device's capabilities.
			' HidP_GetCaps and other API functions require a pointer to the buffer.

			' Requires: 
			' A handle returned by CreateFile.
			' A pointer to a buffer.

			' Returns:
			' True on success, False on failure.
			' ***

			success = HidD_GetPreparsedData(hidHandle, preparsedData)

			' ***
			' API function: HidP_GetCaps

			' Purpose: find out a device's capabilities.
			' For standard devices such as joysticks, you can find out the specific
			' capabilities of the device.
			' For a custom device where the software knows what the device is capable of,
			' this call may be unneeded.

			' Accepts:
			' A pointer returned by HidD_GetPreparsedData
			' A pointer to a HIDP_CAPS structure.

			' Returns: True on success, False on failure.
			' ***

			result = HidP_GetCaps(preparsedData, Capabilities)
			If (result <> 0) Then

				Debug.WriteLine("")

				Debug.WriteLine("  Usage: " & Hex(Capabilities.Usage))
				Debug.WriteLine("  Usage Page: " & Hex(Capabilities.UsagePage))
				Debug.WriteLine("  Input Report Byte Length: " & Capabilities.InputReportByteLength)
				Debug.WriteLine("  Output Report Byte Length: " & Capabilities.OutputReportByteLength)
				Debug.WriteLine("  Feature Report Byte Length: " & Capabilities.FeatureReportByteLength)
				Debug.WriteLine("  Number of Link Collection Nodes: " & Capabilities.NumberLinkCollectionNodes)
				Debug.WriteLine("  Number of Input Button Caps: " & Capabilities.NumberInputButtonCaps)
				Debug.WriteLine("  Number of Input Value Caps: " & Capabilities.NumberInputValueCaps)
				Debug.WriteLine("  Number of Input Data Indices: " & Capabilities.NumberInputDataIndices)
				Debug.WriteLine("  Number of Output Button Caps: " & Capabilities.NumberOutputButtonCaps)
				Debug.WriteLine("  Number of Output Value Caps: " & Capabilities.NumberOutputValueCaps)
				Debug.WriteLine("  Number of Output Data Indices: " & Capabilities.NumberOutputDataIndices)
				Debug.WriteLine("  Number of Feature Button Caps: " & Capabilities.NumberFeatureButtonCaps)
				Debug.WriteLine("  Number of Feature Value Caps: " & Capabilities.NumberFeatureValueCaps)
				Debug.WriteLine("  Number of Feature Data Indices: " & Capabilities.NumberFeatureDataIndices)

				' ***
				' API function: HidP_GetValueCaps

				' Purpose: retrieves a buffer containing an array of HidP_ValueCaps structures.
				' Each structure defines the capabilities of one value.
				' This application doesn't use this data.

				' Accepts:
				' A report type enumerator from hidpi.h,
				' A pointer to a buffer for the returned array,
				' The NumberInputValueCaps member of the device's HidP_Caps structure,
				' A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.

				' Returns: True on success, False on failure.
				' ***
				Dim vcSize As Int32 = Capabilities.NumberInputValueCaps
				Dim valueCaps(vcSize - 1) As Byte

				result = HidP_GetValueCaps(HidP_Input, valueCaps, vcSize, preparsedData)

				result = HidP_GetValueCaps _
				 (HidP_Input, _
				 valueCaps, _
				 vcSize, _
				 preparsedData)

				'(To use this data, copy the ValueCaps byte array into an array of structures.)               

			End If

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw

		Finally
			' ***
			' API function: HidD_FreePreparsedData

			' Purpose: frees the buffer reserved by HidD_GetPreparsedData.

			' Accepts: A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.

			' Returns: True on success, False on failure.
			' ***
			If Not (preparsedData = IntPtr.Zero) Then
				success = HidD_FreePreparsedData(preparsedData)
			End If
		End Try

		Return Capabilities

	End Function

	''' <summary>
	''' Creates a 32-bit Usage from the Usage Page and Usage ID. 
	''' Determines whether the Usage is a system mouse or keyboard.
	''' Can be modified to detect other Usages.
	''' </summary>
	''' 
	''' <param name="MyCapabilities"> a HIDP_CAPS structure retrieved with HidP_GetCaps. </param>
	''' 
	''' <returns>
	''' A string describing the Usage.
	''' </returns>

	Friend Function GetHidUsage _
	 (ByVal MyCapabilities As HIDP_CAPS) _
	 As String

		Dim usage As Int32
		Dim usageDescription As String = ""

		Try
			' Create32-bit Usage from Usage Page and Usage ID.

			usage = MyCapabilities.UsagePage * 256 + MyCapabilities.Usage

			If usage = Convert.ToInt32(&H102) Then usageDescription = "mouse"
			If usage = Convert.ToInt32(&H106) Then usageDescription = "keyboard"

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

		Return usageDescription

	End Function

	''' <summary>
	''' Retrieves the number of Input reports the host can store.
	''' </summary>
	''' 
	''' <param name="hidDeviceObject"> a handle to a device  </param>
	''' <param name="numberOfInputBuffers"> an integer to hold the returned value. </param>
	''' 
	''' <returns>
	''' True on success, False on failure.
	''' </returns>

	Friend Function GetNumberOfInputBuffers _
	  (ByVal hidDeviceObject As SafeFileHandle, _
	  ByRef numberOfInputBuffers As Int32) _
	  As Boolean

		Dim success As Boolean

		Try

			If Not (IsWindows98Gold()) Then

				' ***
				' API function: HidD_GetNumInputBuffers

				' Purpose: retrieves the number of Input reports the host can store.
				' Not supported by Windows 98 Gold.
				' If the buffer is full and another report arrives, the host drops the 
				' ldest report.

				' Accepts: a handle to a device and an integer to hold the number of buffers. 

				' Returns: True on success, False on failure.
				' ***

				success = HidD_GetNumInputBuffers _
				  (hidDeviceObject, _
				  numberOfInputBuffers)

			Else

				' Under Windows 98 Gold, the number of buffers is fixed at 2.

				numberOfInputBuffers = 2
				success = True
			End If

			Return success

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function

	''' <summary>
	''' sets the number of input reports the host will store.
	''' Requires Windows XP or later.
	''' </summary>
	''' 
	''' <param name="hidDeviceObject"> a handle to the device.</param>
	''' <param name="numberBuffers"> the requested number of input reports.  </param>
	''' 
	''' <returns>
	''' True on success. False on failure.
	''' </returns>

	Friend Function SetNumberOfInputBuffers _
	  (ByVal hidDeviceObject As SafeFileHandle, _
	  ByVal numberBuffers As Int32) _
	  As Boolean

		Dim success As Boolean

		Try
			If Not (IsWindows98Gold()) Then

				' ***
				' API function: HidD_SetNumInputBuffers

				' Purpose: Sets the number of Input reports the host can store.
				' If the buffer is full and another report arrives, the host drops the 
				' oldest report.

				' Requires:
				' A handle to a HID
				' An integer to hold the number of buffers. 

				' Returns: true on success, false on failure.
				' ***

				HidD_SetNumInputBuffers _
				  (hidDeviceObject, _
				  numberBuffers)

				Return success

			Else
				' Not supported under Windows 98 Gold.

				Return False
			End If

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function

	''' <summary>
	''' Find out if the current operating system is Windows XP or later.
	''' (Windows XP or later is required for HidD_GetInputReport and HidD_SetInputReport.)
	''' </summary>

	Friend Function IsWindowsXpOrLater() As Boolean

		Try
			Dim myEnvironment As OperatingSystem = Environment.OSVersion

			' Windows XP is version 5.1.

			Dim versionXP As New System.Version(5, 1)

			If (Version.op_GreaterThanOrEqual(myEnvironment.Version, versionXP) = True) Then
				Debug.Write("The OS is Windows XP or later.")
				Return True
			Else
				Debug.Write("The OS is earlier than Windows XP.")
				Return False
			End If

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function

	''' <summary>
	''' Find out if the current operating system is Windows 98 Gold (original version).
	''' Windows 98 Gold does not support the following:
	''' Interrupt OUT transfers (WriteFile uses control transfers and Set_Report).
	''' HidD_GetNumInputBuffers and HidD_SetNumInputBuffers
	''' (Not yet tested on a Windows 98 Gold system.)
	''' </summary>

	Friend Function IsWindows98Gold() As Boolean

		Try
			Dim myEnvironment As OperatingSystem = Environment.OSVersion

			' Windows 98 Gold is version 4.10 with a build number less than 2183.

			Dim version98SE As New System.Version(4, 10, 2183)

			If (Version.op_LessThan(myEnvironment.Version, version98SE) = True) Then
				Debug.Write("The OS is Windows 98 Gold.")
				Return True
			Else
				Debug.Write("The OS is more recent than Windows 98 Gold.")
				Return False
			End If

		Catch ex As Exception
			DisplayException(MODULE_NAME, ex)
			Throw
		End Try

	End Function

	''' <summary>
	''' Provides a central mechanism for exception handling.
	''' Displays a message box that describes the exception.
	''' </summary>
	''' 
	''' <param name="moduleName">  the module where the exception occurred. </param>
	''' <param name="e"> the exception </param>

	Shared Sub DisplayException(ByVal moduleName As String, ByVal e As Exception)

		Dim message As String
		Dim caption As String

		' Create an error message.

		message = "Exception: " & e.Message & ControlChars.CrLf & _
		"Module: " & moduleName & ControlChars.CrLf & _
		 "Method: " & e.TargetSite.Name

		caption = "Unexpected Exception"

		MessageBox.Show(message, caption, MessageBoxButtons.OK)
		Debug.Write(message)

	End Sub

End Class
