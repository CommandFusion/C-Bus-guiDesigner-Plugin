<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class help
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
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cboActions = New System.Windows.Forms.ComboBox()
        Me.mainPane = New System.Windows.Forms.WebBrowser()
        Me.SuspendLayout()
        '
        'btnClose
        '
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Location = New System.Drawing.Point(333, 328)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(90, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "I am trying to use:"
        '
        'cboActions
        '
        Me.cboActions.FormattingEnabled = True
        Me.cboActions.Location = New System.Drawing.Point(99, 6)
        Me.cboActions.Name = "cboActions"
        Me.cboActions.Size = New System.Drawing.Size(195, 21)
        Me.cboActions.TabIndex = 2
        '
        'mainPane
        '
        Me.mainPane.IsWebBrowserContextMenuEnabled = False
        Me.mainPane.Location = New System.Drawing.Point(-3, 28)
        Me.mainPane.MinimumSize = New System.Drawing.Size(20, 20)
        Me.mainPane.Name = "mainPane"
        Me.mainPane.ScriptErrorsSuppressed = True
        Me.mainPane.ScrollBarsEnabled = False
        Me.mainPane.Size = New System.Drawing.Size(414, 297)
        Me.mainPane.TabIndex = 3
        Me.mainPane.WebBrowserShortcutsEnabled = False
        '
        'help
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(413, 356)
        Me.ControlBox = False
        Me.Controls.Add(Me.cboActions)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.mainPane)
        Me.Name = "help"
        Me.Text = "CBus Plugin Help"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cboActions As System.Windows.Forms.ComboBox
    Friend WithEvents mainPane As System.Windows.Forms.WebBrowser
End Class
