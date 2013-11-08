Imports System.Globalization

Public Class Configuration
    Public VID As Int32 = 8483
    Public PID As Int32 = 4112
    Public UP As New Commands(2, 2)
    Public DOWN As New Commands(2, 1)
    Public LEFT As New Commands(2, 4)
    Public RIGHT As New Commands(2, 8)
    Public FIRE As New Commands(2, 16)
    Public STOPMOVE As New Commands(2, 32)
    Public GETSTATUS As New Commands(1, 0)

    Public DOWMLIMIT As Byte = 1
    Public UPLIMIT As Byte = 2
    Public LEFTLIMIT As Byte = 4
    Public RIGHTLIMIT As Byte = 8
    Public FIRED As Byte = 16

End Class
