<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Rocket
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.FireMis = New System.Windows.Forms.Button()
        Me.MoveUP = New System.Windows.Forms.Button()
        Me.MoveRIGHT = New System.Windows.Forms.Button()
        Me.MoveLeft = New System.Windows.Forms.Button()
        Me.MoveDOWN = New System.Windows.Forms.Button()
        Me.ActivatePatrol = New System.Windows.Forms.Button()
        Me.piccapture = New System.Windows.Forms.PictureBox()
        Me.MotionTimer = New System.Windows.Forms.Timer(Me.components)
        Me.FireStatus = New System.Windows.Forms.Label()
        Me.MotionDectect = New System.Windows.Forms.TrackBar()
        Me.FireWait = New System.Windows.Forms.Timer(Me.components)
        CType(Me.piccapture, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MotionDectect, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'FireMis
        '
        Me.FireMis.Location = New System.Drawing.Point(106, 319)
        Me.FireMis.Name = "FireMis"
        Me.FireMis.Size = New System.Drawing.Size(75, 23)
        Me.FireMis.TabIndex = 0
        Me.FireMis.Text = "Fire"
        Me.FireMis.UseVisualStyleBackColor = True
        '
        'MoveUP
        '
        Me.MoveUP.Location = New System.Drawing.Point(106, 273)
        Me.MoveUP.Name = "MoveUP"
        Me.MoveUP.Size = New System.Drawing.Size(75, 23)
        Me.MoveUP.TabIndex = 1
        Me.MoveUP.Text = "UP"
        Me.MoveUP.UseVisualStyleBackColor = True
        '
        'MoveRIGHT
        '
        Me.MoveRIGHT.Location = New System.Drawing.Point(197, 319)
        Me.MoveRIGHT.Name = "MoveRIGHT"
        Me.MoveRIGHT.Size = New System.Drawing.Size(75, 23)
        Me.MoveRIGHT.TabIndex = 2
        Me.MoveRIGHT.Text = "RIGHT"
        Me.MoveRIGHT.UseVisualStyleBackColor = True
        '
        'MoveLeft
        '
        Me.MoveLeft.Location = New System.Drawing.Point(12, 319)
        Me.MoveLeft.Name = "MoveLeft"
        Me.MoveLeft.Size = New System.Drawing.Size(75, 23)
        Me.MoveLeft.TabIndex = 3
        Me.MoveLeft.Text = "LEFT"
        Me.MoveLeft.UseVisualStyleBackColor = True
        '
        'MoveDOWN
        '
        Me.MoveDOWN.Location = New System.Drawing.Point(106, 364)
        Me.MoveDOWN.Name = "MoveDOWN"
        Me.MoveDOWN.Size = New System.Drawing.Size(75, 23)
        Me.MoveDOWN.TabIndex = 4
        Me.MoveDOWN.Text = "DOWN"
        Me.MoveDOWN.UseVisualStyleBackColor = True
        '
        'ActivatePatrol
        '
        Me.ActivatePatrol.Location = New System.Drawing.Point(197, 364)
        Me.ActivatePatrol.Name = "ActivatePatrol"
        Me.ActivatePatrol.Size = New System.Drawing.Size(75, 23)
        Me.ActivatePatrol.TabIndex = 5
        Me.ActivatePatrol.Text = "Patrol"
        Me.ActivatePatrol.UseVisualStyleBackColor = True
        '
        'piccapture
        '
        Me.piccapture.Location = New System.Drawing.Point(12, 12)
        Me.piccapture.Name = "piccapture"
        Me.piccapture.Size = New System.Drawing.Size(260, 230)
        Me.piccapture.TabIndex = 6
        Me.piccapture.TabStop = False
        '
        'MotionTimer
        '
        Me.MotionTimer.Interval = 33
        '
        'FireStatus
        '
        Me.FireStatus.AutoSize = True
        Me.FireStatus.Location = New System.Drawing.Point(122, 257)
        Me.FireStatus.Name = "FireStatus"
        Me.FireStatus.Size = New System.Drawing.Size(0, 13)
        Me.FireStatus.TabIndex = 7
        '
        'MotionDectect
        '
        Me.MotionDectect.Location = New System.Drawing.Point(12, 395)
        Me.MotionDectect.Maximum = 100
        Me.MotionDectect.Name = "MotionDectect"
        Me.MotionDectect.Size = New System.Drawing.Size(250, 45)
        Me.MotionDectect.TabIndex = 8
        Me.MotionDectect.Value = 100
        '
        'FireWait
        '
        Me.FireWait.Interval = 10000
        '
        'Rocket
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 452)
        Me.Controls.Add(Me.MotionDectect)
        Me.Controls.Add(Me.FireStatus)
        Me.Controls.Add(Me.piccapture)
        Me.Controls.Add(Me.ActivatePatrol)
        Me.Controls.Add(Me.MoveDOWN)
        Me.Controls.Add(Me.MoveLeft)
        Me.Controls.Add(Me.MoveRIGHT)
        Me.Controls.Add(Me.MoveUP)
        Me.Controls.Add(Me.FireMis)
        Me.Name = "Rocket"
        Me.Text = "Rocket"
        CType(Me.piccapture, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MotionDectect, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents FireMis As System.Windows.Forms.Button
    Friend WithEvents MoveUP As System.Windows.Forms.Button
    Friend WithEvents MoveRIGHT As System.Windows.Forms.Button
    Friend WithEvents MoveLeft As System.Windows.Forms.Button
    Friend WithEvents MoveDOWN As System.Windows.Forms.Button
    Friend WithEvents ActivatePatrol As System.Windows.Forms.Button
    Friend WithEvents piccapture As System.Windows.Forms.PictureBox
    Friend WithEvents MotionTimer As System.Windows.Forms.Timer
    Friend WithEvents FireStatus As System.Windows.Forms.Label
    Friend WithEvents MotionDectect As System.Windows.Forms.TrackBar
    Friend WithEvents FireWait As System.Windows.Forms.Timer

End Class
