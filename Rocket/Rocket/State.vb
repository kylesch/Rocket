Public Class State
    Public Moving As Boolean
    Public Fired As Boolean
    Public UPLimit As Boolean
    Public DownLimit As Boolean
    Public LeftLimit As Boolean
    Public RightLimit As Boolean
    Public MisslesLeft As Integer

    Public Sub New()
        Moving = False
        Fired = False
        UPLimit = False
        DownLimit = False
        LeftLimit = False
        RightLimit = False
        MisslesLeft = 4
    End Sub
End Class
