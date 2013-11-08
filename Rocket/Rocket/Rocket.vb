Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports DirectShowLib
Imports System.Runtime.InteropServices.ComTypes
Imports System.IO
Imports Microsoft.Win32.SafeHandles
Imports Rocket.Hid
Imports Rocket.FileIO
Imports Rocket.DeviceManagement


Public Class Rocket

    Public Config As New Configuration
    Public RocketState As New State
    Private Shared tmrReadTimeout As System.Timers.Timer
    Private deviceNotificationHandle As IntPtr
    Private exclusiveAccess As Boolean
    Private fileStreamdevicedata As FileStream
    Private hidHandle As SafeFileHandle
    Private hidUsage As String
    Private myDeviceDetected As Boolean
    Private myDevicePathName As String
    Private transferInProgress As Boolean = False
    Private writeHandle As SafeFileHandle
    Private MyDeviceManagement As New DeviceManagement()
    Private MyHid As New Hid()
    Friend FrmMy As Rocket
    Private savedArray As Byte()
    Private Pic1True As Boolean


    Private PB1 As New System.Windows.Forms.PictureBox
    Private PB2 As New System.Windows.Forms.PictureBox


    Enum PlayState
        Stopped
        Paused
        Running
        Init
    End Enum
    Private CurrentState As PlayState = PlayState.Stopped

    Private D As Integer = Convert.ToInt32("0X8000", 16)
    Public WM_GRAPHNOTIFY As Integer = D + 1

    Private VideoWindow As IVideoWindow = Nothing
    Private MediaControl As IMediaControl = Nothing
    Private MediaEventEx As IMediaEventEx = Nothing
    Private GraphBuilder As IGraphBuilder = Nothing
    Private CaptureGraphBuilder As ICaptureGraphBuilder2 = Nothing

    Private rot As DsROTEntry = Nothing

    Const WM_CAP As Short = &H400S

    Const WM_CAP_DRIVER_CONNECT As Integer = WM_CAP + 10
    Const WM_CAP_DRIVER_DISCONNECT As Integer = WM_CAP + 11
    Const WM_CAP_EDIT_COPY As Integer = WM_CAP + 30

    Const WM_CAP_SET_PREVIEW As Integer = WM_CAP + 50
    Const WM_CAP_SET_PREVIEWRATE As Integer = WM_CAP + 52
    Const WM_CAP_SET_SCALE As Integer = WM_CAP + 53
    Const WS_CHILD As Integer = &H40000000
    Const WS_VISIBLE As Integer = &H10000000
    Const SWP_NOMOVE As Short = &H2S
    Const SWP_NOSIZE As Short = 1
    Const SWP_NOZORDER As Short = &H4S
    Const HWND_BOTTOM As Short = 1

    Dim iDevice As Integer = 0 ' Current device ID
    Dim hHwnd As Integer ' Handle to preview window

    Dim MotionDet As Boolean

    Dim RecTop, RecButtom, RecLeft, RecRight As Integer


    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Shared Function CreateFile _
  (ByVal lpFileName As String, _
  ByVal dwDesiredAccess As UInt32, _
  ByVal dwShareMode As Int32, _
  ByVal lpSecurityAttributes As IntPtr, _
  ByVal dwCreationDisposition As Int32, _
  ByVal dwFlagsAndAttributes As Int32, _
  ByVal hTemplateFile As Int32) _
  As SafeFileHandle
    End Function

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        CloseCommunications()
        hidHandle.Close()
        ' closeinterfaces()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tmrReadTimeout = New System.Timers.Timer(5000)
        AddHandler tmrReadTimeout.Elapsed, AddressOf OnReadTimeout
        tmrReadTimeout.SynchronizingObject = Me
        tmrReadTimeout.Stop()
        Pic1True = True
        FrmMy = Me
        FindTheHid()

        PB1.Width = piccapture.Width
        PB1.Height = piccapture.Height
        PB1.Visible = False
        PB2.Width = piccapture.Width
        PB2.Height = piccapture.Height
        PB2.Visible = False

        OpenPreviewWindow()
        'CaptureVideo()
    End Sub
    'Move Up
    Private Sub MoveUP_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MoveUP.MouseDown
        SendCommand(Config.GETSTATUS.Data, False)
        If RocketState.UPLimit = False Then
            SendCommand(Config.UP.Data)
        End If
    End Sub
    Private Sub MoveUP_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MoveUP.MouseUp
        SendCommand(Config.STOPMOVE.Data)
    End Sub
    'Fire Missle
    Private Sub FireMis_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FireMis.Click
        FireMis.Enabled = False
        MoveUP.Enabled = False
        MoveDOWN.Enabled = False
        MoveLeft.Enabled = False
        MoveRIGHT.Enabled = False

        If RocketState.MisslesLeft > 0 Then
            SendCommand(Config.FIRE.Data)
        End If
    End Sub

    Private Sub GotMessage(ByVal Response As Object)
        RocketState.DownLimit = False
        RocketState.UPLimit = False
        RocketState.LeftLimit = False
        RocketState.RightLimit = False

        Select Case Response
            Case Config.DOWMLIMIT
                RocketState.DownLimit = True
            Case Config.UPLIMIT
                RocketState.UPLimit = True
            Case Config.LEFTLIMIT
                RocketState.LeftLimit = True
            Case Config.RIGHTLIMIT
                RocketState.RightLimit = True
            Case Config.FIRED
                RocketState.Fired = True
                RocketState.MisslesLeft = RocketState.MisslesLeft - 1
                FireMis.Enabled = True
                MoveUP.Enabled = True
                MoveDOWN.Enabled = True
                MoveLeft.Enabled = True
                MoveRIGHT.Enabled = True

        End Select
    End Sub
    'Move Down
    Private Sub MoveDOWN_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MoveDOWN.MouseDown
        SendCommand(Config.GETSTATUS.Data, False)
        If RocketState.DownLimit = False Then
            SendCommand(Config.DOWN.Data)
        End If
    End Sub
    Private Sub MoveDOWN_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MoveDOWN.MouseUp
        SendCommand(Config.STOPMOVE.Data)
    End Sub

    'Move Left
    Private Sub MoveLeft_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MoveLeft.MouseDown
        SendCommand(Config.GETSTATUS.Data, False)
        If RocketState.LeftLimit = False Then
            SendCommand(Config.LEFT.Data)
        End If
    End Sub
    Private Sub MoveLeft_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MoveLeft.MouseUp
        SendCommand(Config.STOPMOVE.Data)
    End Sub

    Private Sub MoveRIGHT_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MoveRIGHT.MouseDown
        SendCommand(Config.GETSTATUS.Data, False)
        If RocketState.RightLimit = False Then
            SendCommand(Config.RIGHT.Data)
        End If
    End Sub
    Private Sub MoveRIGHT_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MoveRIGHT.MouseUp
        SendCommand(Config.STOPMOVE.Data)
    End Sub


    Private Sub SendCommand(ByVal SendData() As Byte, Optional ByVal Control As Boolean = True)


        Dim inputReportBuffer() As Byte = Nothing
        Dim outputReportBuffer() As Byte = Nothing
        Dim success As Boolean

        Try
            success = False

            ' Don't attempt to exchange reports if valid handles aren't available
            ' (as for a mouse or keyboard under Windows 2000/XP.)



            ' Don't attempt to send an Output report if the HID has no Output report.



            ' Set the upper bound of the Output report buffer. 
            ' Subtract 1 from OutputReportByteLength because the array begins at index 0.

            Array.Resize(outputReportBuffer, 9)
            Array.Resize(inputReportBuffer, 9)

            ' Store the report ID in the first byte of the buffer:

            outputReportBuffer(0) = 0

            ' Store the report data following the report ID.
            ' Use the data in the combo boxes on the form.

            outputReportBuffer(1) = SendData(0)


            outputReportBuffer(2) = SendData(1)



            If (fileStreamdevicedata.CanWrite) Then
                fileStreamdevicedata.Write(outputReportBuffer, 0, outputReportBuffer.Length)
                success = True
            Else
                CloseCommunications()
            End If


            success = False
            If (Control = True) Then

                ' Read a report using a control transfer.

                success = MyHid.GetInputReportViaControlTransfer(hidHandle, inputReportBuffer)

                If success = False Then
                    CloseCommunications()
                End If

            Else

                Dim ar As IAsyncResult = Nothing
                transferInProgress = True

                ' Timeout if no report is available.

                tmrReadTimeout.Start()

                If (fileStreamdevicedata.CanRead) Then

                    fileStreamdevicedata.BeginRead(inputReportBuffer, 0, inputReportBuffer.Length, New AsyncCallback(AddressOf GetInputReportData), inputReportBuffer)

                Else
                    CloseCommunications()

                End If

            End If


        Catch ex As Exception
            Debug.Print(Me.Name, ex)
            Throw
        End Try

    End Sub
    Private Sub CloseCommunications()

        hHwnd = SendMessage(HWND_BOTTOM, WM_CAP_DRIVER_DISCONNECT, iDevice, 0)
        DestroyWindow(hHwnd)
        If (Not (fileStreamdevicedata Is Nothing)) Then

            fileStreamdevicedata.Close()
        End If
        If ((Not (hidHandle Is Nothing)) And (Not hidHandle.IsInvalid)) Then

            hidHandle.Close()
        End If
    End Sub
    Private Sub GetInputReportData(ByVal ar As IAsyncResult)

        Dim inputReportBuffer As Byte() = Nothing

        Try
            inputReportBuffer = CType(ar.AsyncState, Byte())

            fileStreamdevicedata.EndRead(ar)

            tmrReadTimeout.Stop()

            ' Display the received report data in the form's list box.

            If (ar.IsCompleted) Then

                GotMessage(inputReportBuffer(2))


            Else

                Debug.Write("The attempt to read an Input report has failed")
            End If

        Catch ex As Exception
            Debug.Print(Me.Name, ex)
            Throw
        End Try

    End Sub

    Private Sub OnReadTimeout(ByVal source As Object, ByVal e As Timers.ElapsedEventArgs)

        CloseCommunications()

        tmrReadTimeout.Stop()


    End Sub
    Private Function FindTheHid() As Boolean

        Dim deviceFound As Boolean
        Dim devicePathName(127) As String
        Dim hidGuid As System.Guid
        Dim inputReportBuffer As Byte() = Nothing
        Dim memberIndex As Int32
        Dim myProductID As Int32
        Dim myVendorID As Int32
        Dim outputReportBuffer As Byte() = Nothing
        Dim success As Boolean

        Try
            myDeviceDetected = False

            ' Get the device's Vendor ID and Product ID from the form's text boxes.
            myVendorID = Config.VID
            myProductID = Config.PID


            ' ***
            ' API function: 'HidD_GetHidGuid

            ' Purpose: Retrieves the interface class GUID for the HID class.

            ' Accepts: 'A System.Guid object for storing the GUID.
            ' ***

            HidD_GetHidGuid(hidGuid)

            Debug.WriteLine("  GUID for system HIDs: " & hidGuid.ToString)

            ' Fill an array with the device path names of all attached HIDs.

            deviceFound = MyDeviceManagement.FindDeviceFromGuid _
             (hidGuid, _
             devicePathName)

            ' If there is at least one HID, attempt to read the Vendor ID and Product ID
            ' of each device until there is a match or all devices have been examined.

            If deviceFound Then

                memberIndex = 0

                Do
                    ' ***
                    ' API function:
                    ' CreateFile

                    ' Purpose:
                    ' Retrieves a handle to a device.

                    ' Accepts:
                    ' A device path name returned by SetupDiGetDeviceInterfaceDetail
                    ' The type of access requested (read/write).
                    ' FILE_SHARE attributes to allow other processes to access the device while this handle is open.
                    ' A Security structure or IntPtr.Zero. 
                    ' A creation disposition value. Use OPEN_EXISTING for devices.
                    ' Flags and attributes for files. Not used for devices.
                    ' Handle to a template file. Not used.

                    ' Returns: a handle without read or write access.
                    ' This enables obtaining information about all HIDs, even system
                    ' keyboards and mice. 
                    ' Separate handles are used for reading and writing.
                    ' ***

                    ' Open the handle without read/write access to enable getting information about any HID, even system keyboards and mice.

                    hidHandle = CreateFile _
                     (devicePathName(memberIndex), _
                     0, _
                     FILE_SHARE_READ Or FILE_SHARE_WRITE, _
                     IntPtr.Zero, _
                     OPEN_EXISTING, _
                     0, _
                     0)


                    Debug.WriteLine("  Returned handle: " & hidHandle.ToString)

                    If Not (hidHandle.IsInvalid) Then

                        ' The returned handle is valid, 
                        ' so find out if this is the device we're looking for.

                        ' Set the Size property of DeviceAttributes to the number of bytes in the structure.

                        MyHid.DeviceAttributes.Size = Marshal.SizeOf(MyHid.DeviceAttributes)

                        ' ***
                        ' API function:
                        ' HidD_GetAttributes

                        ' Purpose:
                        ' Retrieves a HIDD_ATTRIBUTES structure containing the Vendor ID, 
                        ' Product ID, and Product Version Number for a device.

                        ' Accepts:
                        ' A handle returned by CreateFile.
                        ' A pointer to receive a HIDD_ATTRIBUTES structure.

                        ' Returns:
                        ' True on success, False on failure.
                        ' ***

                        success = HidD_GetAttributes(hidHandle, MyHid.DeviceAttributes)

                        If success Then

                            Debug.WriteLine("  HIDD_ATTRIBUTES structure filled without error.")
                            Debug.WriteLine("  Structure size: " & MyHid.DeviceAttributes.Size)
                            Debug.WriteLine("  Vendor ID: " & Hex(MyHid.DeviceAttributes.VendorID))
                            Debug.WriteLine("  Product ID: " & Hex(MyHid.DeviceAttributes.ProductID))
                            Debug.WriteLine("  Version Number: " & Hex(MyHid.DeviceAttributes.VersionNumber))

                            ' Find out if the device matches the one we're looking for.

                            If (MyHid.DeviceAttributes.VendorID = myVendorID) And _
                             (MyHid.DeviceAttributes.ProductID = myProductID) Then

                                Debug.WriteLine("  My device detected")

                                ' Display the information in form's list box.

                                myDeviceDetected = True

                                ' Save the DevicePathName for OnDeviceChange().

                                myDevicePathName = devicePathName(memberIndex)
                            Else

                                ' It's not a match, so close the handle.

                                myDeviceDetected = False

                                hidHandle.Close()

                            End If

                        Else
                            ' There was a problem in retrieving the information.

                            Debug.WriteLine("  Error in filling HIDD_ATTRIBUTES structure.")
                            myDeviceDetected = False
                            hidHandle.Close()
                        End If

                    End If

                    ' Keep looking until we find the device or there are no devices left to examine.

                    memberIndex = memberIndex + 1

                Loop Until (myDeviceDetected Or (memberIndex = devicePathName.Length))

            End If

            If myDeviceDetected Then

                ' The device was detected.
                ' Register to receive notifications if the device is removed or attached.

                success = MyDeviceManagement.RegisterForDeviceNotifications _
                 (myDevicePathName, _
                 FrmMy.Handle, _
                 hidGuid, _
                 deviceNotificationHandle)

                Debug.WriteLine("RegisterForDeviceNotifications = " & success)

                ' Learn the capabilities of the device.

                MyHid.Capabilities = MyHid.GetDeviceCapabilities(hidHandle)

                If success Then

                    ' Find out if the device is a system mouse or keyboard.

                    hidUsage = MyHid.GetHidUsage(MyHid.Capabilities)

                    ' Get the Input report buffer size.

                    GetInputReportBufferSize()

                    'Close the handle and reopen it with read/write access.

                    hidHandle.Close()

                    hidHandle = CreateFile _
                     (myDevicePathName, _
                     GENERIC_READ Or GENERIC_WRITE, _
                     FILE_SHARE_READ Or FILE_SHARE_WRITE, _
                     IntPtr.Zero, _
                     OPEN_EXISTING, _
                     0, _
                     0)

                    If hidHandle.IsInvalid Then

                        exclusiveAccess = True
 
                    Else

                        If (MyHid.Capabilities.InputReportByteLength > 0) Then

                            ' Set the size of the Input report buffer. 
                            ' Subtract 1 from the value in the Capabilities structure because 
                            ' the array begins at index 0.

                            Array.Resize(inputReportBuffer, MyHid.Capabilities.InputReportByteLength)

                            fileStreamdevicedata = New FileStream(hidHandle, FileAccess.Read Or FileAccess.Write, inputReportBuffer.Length, False)

                        End If

                        If (MyHid.Capabilities.OutputReportByteLength > 0) Then

                            ' Set the size of the Output report buffer. 
                            ' Subtract 1 from the value in the Capabilities structure because 
                            ' the array begins at index 0.

                            Array.Resize(outputReportBuffer, MyHid.Capabilities.OutputReportByteLength)

                        End If

                        ' Flush any waiting reports in the input buffer. (optional)

                        MyHid.FlushQueue(hidHandle)

                    End If
                End If
            Else
                ' The device wasn't detected.

                Debug.WriteLine(" Device not found.")


            End If

            Return myDeviceDetected

        Catch ex As Exception
            DisplayException(Me.Name, ex)
            Throw
        End Try
    End Function

    Private Sub GetInputReportBufferSize()

        Dim numberOfInputBuffers As Int32

        Try
            ' Get the number of input buffers.

            MyHid.GetNumberOfInputBuffers _
             (hidHandle, _
             numberOfInputBuffers)


        Catch ex As Exception
            DisplayException(Me.Name, ex)
            Throw
        End Try

    End Sub
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    'Private Sub CaptureVideo()
    '    Dim hr As Integer = 0
    '    Dim sourceFilter As IBaseFilter = Nothing
    '    Try
    '        GetInterfaces()

    '        hr = Me.CaptureGraphBuilder.SetFiltergraph(Me.GraphBuilder) 'Specifies filter graph "graphbuilder" for the capture graph builder "captureGraphBuilder" to use.
    '        Debug.WriteLine("Attach the filter graph to the capture graph : " & DsError.GetErrorText(hr))
    '        DsError.ThrowExceptionForHR(hr)

    '        sourceFilter = FindCaptureDevice()

    '        hr = Me.GraphBuilder.AddFilter(sourceFilter, "Video Capture")
    '        Debug.WriteLine("Add capture filter to our graph : " & DsError.GetErrorText(hr))
    '        DsError.ThrowExceptionForHR(hr)

    '        hr = Me.CaptureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, sourceFilter, Nothing, Nothing)
    '        Debug.WriteLine("Render the preview pin on the video capture filter : " & DsError.GetErrorText(hr))
    '        DsError.ThrowExceptionForHR(hr)

    '        Marshal.ReleaseComObject(sourceFilter)

    '        SetupVideoWindow()

    '        rot = New DsROTEntry(Me.GraphBuilder)

    '        hr = Me.MediaControl.Run()
    '        Debug.WriteLine("Start previewing video data : " & DsError.GetErrorText(hr))
    '        DsError.ThrowExceptionForHR(hr)

    '        Me.CurrentState = PlayState.Running
    '        Debug.WriteLine("The currentstate : " & Me.CurrentState.ToString)

    '    Catch ex As Exception
    '        MessageBox.Show("An unrecoverable error has occurred.With error : " & ex.ToString)
    '    End Try
    'End Sub

    'Private Sub GetInterfaces()
    '    Dim hr As Integer = 0
    '    Me.GraphBuilder = CType(New FilterGraph, IGraphBuilder)
    '    Me.CaptureGraphBuilder = CType(New CaptureGraphBuilder2, ICaptureGraphBuilder2)
    '    Me.MediaControl = CType(Me.GraphBuilder, IMediaControl)
    '    Me.VideoWindow = CType(Me.GraphBuilder, IVideoWindow)
    '    Me.MediaEventEx = CType(Me.GraphBuilder, IMediaEventEx)

    '    hr = Me.MediaEventEx.SetNotifyWindow(Me.Handle, WM_GRAPHNOTIFY, IntPtr.Zero) 'This method designates a window as the recipient of messages generated by or sent to the current DirectShow object

    '    DsError.ThrowExceptionForHR(hr) 'ThrowExceptionForHR is a wrapper for Marshal.ThrowExceptionForHR, but additionally provides descriptions for any DirectShow specific error messages.If the hr value is not a fatal error, no exception will be thrown:
    '    Debug.WriteLine("I started Sub Get interfaces , the result is : " & DsError.GetErrorText(hr))
    'End Sub
    'Public Function FindCaptureDevice() As IBaseFilter
    '    Debug.WriteLine("Start the Sub FindCaptureDevice")
    '    Dim hr As Integer = 0
    '    Dim classEnum As IEnumMoniker = Nothing
    '    Dim moniker As IMoniker() = New IMoniker(0) {}
    '    Dim source As Object = Nothing
    '    Dim devEnum As ICreateDevEnum = CType(New CreateDevEnum, ICreateDevEnum)
    '    hr = devEnum.CreateClassEnumerator(FilterCategory.VideoInputDevice, classEnum, 0)
    '    Debug.WriteLine("Create an enumerator for the video capture devices : " & DsError.GetErrorText(hr))
    '    DsError.ThrowExceptionForHR(hr)
    '    Marshal.ReleaseComObject(devEnum)
    '    If classEnum Is Nothing Then
    '        Throw New ApplicationException("No video capture device was detected.\r\n\r\n" & _
    '                       "This sample requires a video capture device, such as a USB WebCam,\r\n" & _
    '                       "to be installed and working properly.  The sample will now close.")
    '    End If
    '    If classEnum.Next(moniker.Length, moniker, IntPtr.Zero) = 0 Then
    '        Dim iid As Guid = GetType(IBaseFilter).GUID
    '        moniker(0).BindToObject(Nothing, Nothing, iid, source)
    '    Else
    '        Throw New ApplicationException("Unable to access video capture device!")
    '    End If
    '    Marshal.ReleaseComObject(moniker(0))
    '    Marshal.ReleaseComObject(classEnum)
    '    Return CType(source, IBaseFilter)
    'End Function
    'Public Sub SetupVideoWindow()
    '    Dim hr As Integer = 0
    '    'set the video window to be a child of the main window
    '    'putowner : Sets the owning parent window for the video playback window. 
    '    hr = Me.VideoWindow.put_Owner(Me.Handle)
    '    DsError.ThrowExceptionForHR(hr)

    '    hr = Me.VideoWindow.put_WindowStyle(WindowStyle.Child Or WindowStyle.ClipChildren)
    '    DsError.ThrowExceptionForHR(hr)

    '    'Use helper function to position video window in client rect of main application window
    '    ResizeVideoWindow()

    '    'Make the video window visible, now that it is properly positioned
    '    'put_visible : This method changes the visibility of the video window. 
    '    hr = Me.VideoWindow.put_Visible(OABool.True)
    '    DsError.ThrowExceptionForHR(hr)
    'End Sub

    'Protected Overloads Sub WndProc(ByRef m As Message)
    '    Select Case m.Msg
    '        Case WM_GRAPHNOTIFY
    '            HandleGraphEvent()
    '    End Select
    '    If Not (Me.VideoWindow Is Nothing) Then
    '        Me.VideoWindow.NotifyOwnerMessage(m.HWnd, m.Msg, m.WParam.ToInt32, m.LParam.ToInt32)
    '    End If
    '    MyBase.WndProc(m)
    'End Sub
    'Public Sub HandleGraphEvent()
    '    Dim hr As Integer = 0
    '    Dim evCode As EventCode
    '    Dim evParam1 As Integer
    '    Dim evParam2 As Integer
    '    If Me.MediaEventEx Is Nothing Then
    '        Return
    '    End If
    '    While Me.MediaEventEx.GetEvent(evCode, evParam1, evParam2, 0) = 0
    '        '// Free event parameters to prevent memory leaks associated with
    '        '// event parameter data.  While this application is not interested
    '        '// in the received events, applications should always process them.
    '        hr = Me.MediaEventEx.FreeEventParams(evCode, evParam1, evParam2)
    '        DsError.ThrowExceptionForHR(hr)

    '        '// Insert event processing code here, if desired
    '    End While
    'End Sub


    'Public Sub closeinterfaces()
    '    '//stop previewing data
    '    If Not (Me.MediaControl Is Nothing) Then
    '        Me.MediaControl.StopWhenReady()
    '    End If

    '    Me.CurrentState = PlayState.Stopped

    '    '//stop recieving events
    '    If Not (Me.MediaEventEx Is Nothing) Then
    '        Me.MediaEventEx.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero)
    '    End If

    '    '// Relinquish ownership (IMPORTANT!) of the video window.
    '    '// Failing to call put_Owner can lead to assert failures within
    '    '// the video renderer, as it still assumes that it has a valid
    '    '// parent window.
    '    If Not (Me.VideoWindow Is Nothing) Then
    '        Me.VideoWindow.put_Visible(OABool.False)
    '        Me.VideoWindow.put_Owner(IntPtr.Zero)
    '    End If

    '    ' // Remove filter graph from the running object table
    '    If Not (rot Is Nothing) Then
    '        rot.Dispose()
    '        rot = Nothing
    '    End If

    '    '// Release DirectShow interfaces
    '    Marshal.ReleaseComObject(Me.MediaControl) : Me.MediaControl = Nothing
    '    Marshal.ReleaseComObject(Me.MediaEventEx) : Me.MediaEventEx = Nothing
    '    Marshal.ReleaseComObject(Me.VideoWindow) : Me.VideoWindow = Nothing
    '    Marshal.ReleaseComObject(Me.GraphBuilder) : Me.GraphBuilder = Nothing
    '    Marshal.ReleaseComObject(Me.CaptureGraphBuilder) : Me.CaptureGraphBuilder = Nothing

    'End Sub

    Private Sub Form1_Resize1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            'ChangePreviewState(False)
        End If
        If Me.WindowState = FormWindowState.Normal Then
            'ChangePreviewState(True)
        End If
        'ResizeVideoWindow()
        PB1.Width = piccapture.Width
        PB1.Height = piccapture.Height
        PB1.Visible = False
        PB2.Width = piccapture.Width
        PB2.Height = piccapture.Height
        PB2.Visible = False
    End Sub
    'Public Sub ChangePreviewState(ByVal showVideo As Boolean)
    '    Dim hr As Integer = 0
    '    '// If the media control interface isn't ready, don't call it
    '    If Me.MediaControl Is Nothing Then
    '        Debug.WriteLine("MediaControl is nothing")
    '        Return
    '    End If
    '    If showVideo = True Then
    '        If Not (Me.CurrentState = PlayState.Running) Then
    '            Debug.WriteLine("Start previewing video data")
    '            hr = Me.MediaControl.Run
    '            Me.CurrentState = PlayState.Running
    '        End If
    '    Else
    '        Debug.WriteLine("Stop previewing video data")
    '        hr = Me.MediaControl.StopWhenReady
    '        Me.CurrentState = PlayState.Stopped
    '    End If
    'End Sub
    'Public Sub ResizeVideoWindow()
    '    'Resize the video preview window to match owner window size
    '    'left , top , width , height
    '    If Not (Me.VideoWindow Is Nothing) Then 'if the videopreview is not nothing
    '        Me.VideoWindow.SetWindowPosition(0, 0, Me.Width, Me.ClientSize.Height / 2)
    '    End If
    'End Sub
    Private Sub OpenPreviewWindow()

        Dim iHeight As Integer = piccapture.Height
        Dim iWidth As Integer = piccapture.Width


        ' Open Preview window in picturebox
        hHwnd = capCreateCaptureWindowA(iDevice, WS_VISIBLE Or WS_CHILD, 0, 0, 260, _
            230, piccapture.Handle.ToInt32, 0)

        '
        ' Connect to device
        '
        'Dim driver As Integer
        'Do While (SendMessage(hHwnd, WM_CAP_DRIVER_CONNECT, driver, 0) = False) '{ // connect to webcam
        '    driver = driver + 1
        '    If (driver > 9) Then
        '        DestroyWindow(hHwnd)
        '        '//ShowMessage("Error activating camera.");
        '        Exit Do
        '    End If
        'Loop


        If SendMessage(hHwnd, WM_CAP_DRIVER_CONNECT, iDevice, 0) Then
            '
            'Set the preview scale
            '
            SendMessage(hHwnd, WM_CAP_SET_SCALE, True, 0)

            '
            'Set the preview rate in milliseconds
            '
            SendMessage(hHwnd, WM_CAP_SET_PREVIEWRATE, 66, 0)

            '
            'Start previewing the image from the camera
            '
            SendMessage(hHwnd, WM_CAP_SET_PREVIEW, True, 0)

            '
            ' Resize window to fit in picturebox
            '
            SetWindowPos(hHwnd, HWND_BOTTOM, 0, 0, piccapture.Width, piccapture.Height, _
                    SWP_NOMOVE Or SWP_NOZORDER)

        Else
            CloseCommunications()
            End
        End If

        piccapture.Refresh()
        'MotionTimer.Enabled = True
    End Sub



    Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
      (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, _
      <MarshalAs(UnmanagedType.AsAny)> ByVal lParam As Object) As Integer

    Declare Function SetWindowPos Lib "user32" Alias "SetWindowPos" (ByVal hwnd As Integer, _
        ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, _
        ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer

    Declare Function DestroyWindow Lib "user32" (ByVal hndw As Integer) As Boolean

    Declare Function capCreateCaptureWindowA Lib "avicap32.dll" _
        (ByVal lpszWindowName As String, ByVal dwStyle As Integer, _
        ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, _
        ByVal nHeight As Short, ByVal hWndParent As Integer, _
        ByVal nID As Integer) As Integer

    Declare Function capGetDriverDescriptionA Lib "avicap32.dll" (ByVal wDriver As Short, _
        ByVal lpszName As String, ByVal cbName As Integer, ByVal lpszVer As String, _
        ByVal cbVer As Integer) As Boolean


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActivatePatrol.Click
        If MotionTimer.Enabled = False Then
            FireStatus.Text = "Searching for Motion"
            MotionTimer.Enabled = True
            ActivatePatrol.Text = "Patrol Active"
        Else
            FireStatus.Text = "Patrol Stopped"
            MotionTimer.Enabled = False
            ActivatePatrol.Text = "Patrol"
        End If

    End Sub

    Private Sub MotionTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MotionTimer.Tick

        Dim data As System.Windows.Forms.IDataObject
        Dim bmap As Image = Nothing

        ' this Sub : stream video to PB1 (PictureBox)


        ' Copy image to clipboard
        'Debug.Print("Start" & Now.Millisecond)
        SendMessage(hHwnd, WM_CAP_EDIT_COPY, 0, 0)


        ' Get image from clipboard and convert it to a bitmap

        data = Clipboard.GetDataObject()
        'Debug.Print(Now.Millisecond)

        If data.GetDataPresent(GetType(System.Drawing.Bitmap)) Then
            'Debug.Print(Now.Millisecond)
            bmap = CType(data.GetData(GetType(System.Drawing.Bitmap)), Image)
            'Debug.Print(Now.Millisecond)
        End If


        If Not IsNothing(bmap) Then
            If Pic1True Then
                PB1.Image = bmap
                Pic1True = False
            Else
                PB2.Image = bmap
                Pic1True = True
            End If

            If Pic1True = True And Not IsNothing(PB1) And Not IsNothing(PB2) Then
                Compare_Images()
                'Debug.Print(Now.Millisecond)
                'Returner = Comp.CompareImages(PB1.Image, PB2.Image)
                'Debug.Print(If(Returner = ImageComparer.ImageRelation.PixelInqeuality, "Fire", "Don't Fire"))
                'Debug.Print("End" & Now.Millisecond)
            End If
        End If

    End Sub

    Private Sub Compare_Images()
        MotionTimer.Enabled = False
        Dim Density As Double = 75
        Dim c1, c2 As System.Drawing.Color
        Dim x, y As Integer
        Dim dG, dB, dR, c1G, c1B, c1R, c2G, c2B, c2R As Integer
        Dim stepX, stepY As Integer
        Dim SenDeltaR As Double

        MotionDet = False

        RecButtom = 0
        RecRight = 0
        RecTop = PB1.Height
        RecLeft = PB1.Width

        stepX = PB1.Width * (Density / 100)
        stepY = PB1.Height * (Density / 100)

        For x = stepX / 2 To PB1.Width - 1 Step stepX
            For y = stepY / 2 To PB1.Height - 1 Step stepY

                ' getting pixel color (R,G,B) from both PictureBoxes at the same X,Y
                ' ** both PictureBoxes must be 319x239 !!!!!! **
                c1 = CType(PB1.Image, Bitmap).GetPixel(x, y)
                c2 = CType(PB2.Image, Bitmap).GetPixel(x, y)
                ' seperating R B G colors into varibles inorder not to repeat using the functions 
                ' such as c1.R to save time instead to reusing those functions in the following code
                c1R = c1.R
                c1G = c1.G
                c1B = c1.B
                c2R = c2.R
                c2G = c2.G
                c2B = c2.B

                dR = Math.Abs(CInt(c1.R) - CInt(c2.R))
                dG = Math.Abs(CInt(c1.G) - CInt(c2.G))
                dB = Math.Abs(CInt(c1.B) - CInt(c2.B))


                SenDeltaR = MotionDectect.Value
                'check if color Delta between two PictureBoxes (at the same X,Y) crossed the "threshold"
                If dR > SenDeltaR Or dG > SenDeltaR Or dB > SenDeltaR Then

                    If x < RecLeft Then
                        RecLeft = x
                    End If

                    If x > RecRight Then
                        RecRight = x
                    End If

                    If y < RecTop Then
                        RecTop = y
                    End If

                    If y > RecButtom Then
                        RecButtom = y
                    End If

                    ' MOTION DETECTED

                    FireStatus.Visible = True
                    FireStatus.Text = "FIRE"
                    MotionDet = True
                    'FireMis_Click("test", Nothing)
                    'Threading.Thread.Sleep(10000)

                Else
                    ' MOTION NOT DETECTED 
                    FireStatus.Text = ""
                End If

            Next y
        Next x

        If MotionDet Then
            FireWait.Enabled = True
        Else
            MotionTimer.Enabled = True
        End If


    End Sub

    Private Sub FireWait_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FireWait.Tick
        FireWait.Enabled = False
        MotionTimer.Enabled = True
    End Sub
End Class
