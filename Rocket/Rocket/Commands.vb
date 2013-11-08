Public Class Commands

    Public Name As String
    Public Data(7) As Byte
    Public Sub New(ByVal byte1 As Byte, ByVal byte2 As Byte)
        Data(0) = byte1
        Data(1) = byte2

        Dim i As Integer

        For i = 2 To 7
            Data(i) = 0
        Next

    End Sub
End Class
