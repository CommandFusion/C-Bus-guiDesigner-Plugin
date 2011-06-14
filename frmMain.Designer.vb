<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.xmlBrowser = New System.Windows.Forms.OpenFileDialog()
        Me.lblXmlPath = New System.Windows.Forms.Label()
        Me.panBottom = New System.Windows.Forms.Panel()
        Me.btnDebug = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.cboLevelDisplay = New System.Windows.Forms.ComboBox()
        Me.lblDisplay = New System.Windows.Forms.Label()
        Me.tbJoin202 = New System.Windows.Forms.TextBox()
        Me.lblBaseJoin202 = New System.Windows.Forms.Label()
        Me.tbJoin56 = New System.Windows.Forms.TextBox()
        Me.btnReload = New System.Windows.Forms.Button()
        Me.lblBaseJoin56 = New System.Windows.Forms.Label()
        Me.lblType = New System.Windows.Forms.Label()
        Me.rbCgate = New System.Windows.Forms.RadioButton()
        Me.rbSerial = New System.Windows.Forms.RadioButton()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.cboSystems = New System.Windows.Forms.ComboBox()
        Me.lblSystem = New System.Windows.Forms.Label()
        Me.tbXmlFile = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.tbJsFile = New System.Windows.Forms.TextBox()
        Me.lblJsPath = New System.Windows.Forms.Label()
        Me.timerSaveConfig = New System.Windows.Forms.Timer(Me.components)
        Me.panBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'xmlBrowser
        '
        Me.xmlBrowser.Filter = "Clipsal XML Files|*.xml"
        Me.xmlBrowser.InitialDirectory = "C:\Clipsal\C-Gate2\tag"
        Me.xmlBrowser.Title = "Select Clipsal C-Gate XML Configuration File..."
        '
        'lblXmlPath
        '
        Me.lblXmlPath.AutoSize = True
        Me.lblXmlPath.Location = New System.Drawing.Point(6, 45)
        Me.lblXmlPath.Name = "lblXmlPath"
        Me.lblXmlPath.Size = New System.Drawing.Size(82, 13)
        Me.lblXmlPath.TabIndex = 15
        Me.lblXmlPath.Text = "C-Bus XML File:"
        '
        'panBottom
        '
        Me.panBottom.Controls.Add(Me.btnDebug)
        Me.panBottom.Controls.Add(Me.btnClear)
        Me.panBottom.Controls.Add(Me.lblVersion)
        Me.panBottom.Controls.Add(Me.cboLevelDisplay)
        Me.panBottom.Controls.Add(Me.lblDisplay)
        Me.panBottom.Controls.Add(Me.tbJoin202)
        Me.panBottom.Controls.Add(Me.lblBaseJoin202)
        Me.panBottom.Controls.Add(Me.tbJoin56)
        Me.panBottom.Controls.Add(Me.btnReload)
        Me.panBottom.Controls.Add(Me.lblBaseJoin56)
        Me.panBottom.Controls.Add(Me.lblType)
        Me.panBottom.Controls.Add(Me.rbCgate)
        Me.panBottom.Controls.Add(Me.rbSerial)
        Me.panBottom.Controls.Add(Me.btnAdd)
        Me.panBottom.Controls.Add(Me.cboSystems)
        Me.panBottom.Controls.Add(Me.lblSystem)
        Me.panBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panBottom.Location = New System.Drawing.Point(0, 180)
        Me.panBottom.Name = "panBottom"
        Me.panBottom.Size = New System.Drawing.Size(250, 170)
        Me.panBottom.TabIndex = 25
        '
        'btnDebug
        '
        Me.btnDebug.Location = New System.Drawing.Point(9, 140)
        Me.btnDebug.Name = "btnDebug"
        Me.btnDebug.Size = New System.Drawing.Size(52, 23)
        Me.btnDebug.TabIndex = 36
        Me.btnDebug.Text = "Debug"
        Me.btnDebug.UseVisualStyleBackColor = True
        Me.btnDebug.Visible = False
        '
        'btnClear
        '
        Me.btnClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClear.Location = New System.Drawing.Point(66, 140)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(85, 23)
        Me.btnClear.TabIndex = 38
        Me.btnClear.Text = "Clear System"
        Me.btnClear.UseVisualStyleBackColor = True
        Me.btnClear.Visible = False
        '
        'lblVersion
        '
        Me.lblVersion.AutoSize = True
        Me.lblVersion.Enabled = False
        Me.lblVersion.Location = New System.Drawing.Point(6, 145)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(34, 13)
        Me.lblVersion.TabIndex = 37
        Me.lblVersion.Text = "v0.85"
        '
        'cboLevelDisplay
        '
        Me.cboLevelDisplay.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboLevelDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboLevelDisplay.FormattingEnabled = True
        Me.cboLevelDisplay.Items.AddRange(New Object() {"Percent (max 100%)", "CBus (max 255)"})
        Me.cboLevelDisplay.Location = New System.Drawing.Point(80, 112)
        Me.cboLevelDisplay.Name = "cboLevelDisplay"
        Me.cboLevelDisplay.Size = New System.Drawing.Size(161, 21)
        Me.cboLevelDisplay.TabIndex = 34
        '
        'lblDisplay
        '
        Me.lblDisplay.AutoSize = True
        Me.lblDisplay.Location = New System.Drawing.Point(6, 115)
        Me.lblDisplay.Name = "lblDisplay"
        Me.lblDisplay.Size = New System.Drawing.Size(73, 13)
        Me.lblDisplay.TabIndex = 35
        Me.lblDisplay.Text = "Level Display:"
        '
        'tbJoin202
        '
        Me.tbJoin202.Location = New System.Drawing.Point(126, 86)
        Me.tbJoin202.Name = "tbJoin202"
        Me.tbJoin202.Size = New System.Drawing.Size(54, 20)
        Me.tbJoin202.TabIndex = 32
        '
        'lblBaseJoin202
        '
        Me.lblBaseJoin202.AutoSize = True
        Me.lblBaseJoin202.Location = New System.Drawing.Point(5, 89)
        Me.lblBaseJoin202.Name = "lblBaseJoin202"
        Me.lblBaseJoin202.Size = New System.Drawing.Size(119, 13)
        Me.lblBaseJoin202.TabIndex = 33
        Me.lblBaseJoin202.Text = "Trigger (202) Base Join:"
        '
        'tbJoin56
        '
        Me.tbJoin56.Location = New System.Drawing.Point(127, 60)
        Me.tbJoin56.Name = "tbJoin56"
        Me.tbJoin56.Size = New System.Drawing.Size(54, 20)
        Me.tbJoin56.TabIndex = 30
        '
        'btnReload
        '
        Me.btnReload.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.btnReload.Location = New System.Drawing.Point(191, 7)
        Me.btnReload.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.btnReload.Name = "btnReload"
        Me.btnReload.Size = New System.Drawing.Size(50, 22)
        Me.btnReload.TabIndex = 28
        Me.btnReload.Text = "Reload"
        Me.btnReload.UseVisualStyleBackColor = True
        '
        'lblBaseJoin56
        '
        Me.lblBaseJoin56.AutoSize = True
        Me.lblBaseJoin56.Location = New System.Drawing.Point(6, 63)
        Me.lblBaseJoin56.Name = "lblBaseJoin56"
        Me.lblBaseJoin56.Size = New System.Drawing.Size(117, 13)
        Me.lblBaseJoin56.TabIndex = 31
        Me.lblBaseJoin56.Text = "Lighting (56) Base Join:"
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(6, 37)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(34, 13)
        Me.lblType.TabIndex = 29
        Me.lblType.Text = "Type:"
        '
        'rbCgate
        '
        Me.rbCgate.AutoSize = True
        Me.rbCgate.Enabled = False
        Me.rbCgate.Location = New System.Drawing.Point(112, 35)
        Me.rbCgate.Name = "rbCgate"
        Me.rbCgate.Size = New System.Drawing.Size(58, 17)
        Me.rbCgate.TabIndex = 28
        Me.rbCgate.Text = "C-Gate"
        Me.rbCgate.UseVisualStyleBackColor = True
        '
        'rbSerial
        '
        Me.rbSerial.AutoSize = True
        Me.rbSerial.Checked = True
        Me.rbSerial.Location = New System.Drawing.Point(55, 35)
        Me.rbSerial.Name = "rbSerial"
        Me.rbSerial.Size = New System.Drawing.Size(51, 17)
        Me.rbSerial.TabIndex = 27
        Me.rbSerial.TabStop = True
        Me.rbSerial.Text = "Serial"
        Me.rbSerial.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAdd.Location = New System.Drawing.Point(157, 140)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(85, 23)
        Me.btnAdd.TabIndex = 26
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'cboSystems
        '
        Me.cboSystems.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboSystems.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSystems.FormattingEnabled = True
        Me.cboSystems.Location = New System.Drawing.Point(51, 8)
        Me.cboSystems.Name = "cboSystems"
        Me.cboSystems.Size = New System.Drawing.Size(137, 21)
        Me.cboSystems.TabIndex = 25
        '
        'lblSystem
        '
        Me.lblSystem.AutoSize = True
        Me.lblSystem.Location = New System.Drawing.Point(6, 11)
        Me.lblSystem.Name = "lblSystem"
        Me.lblSystem.Size = New System.Drawing.Size(44, 13)
        Me.lblSystem.TabIndex = 24
        Me.lblSystem.Text = "System:"
        '
        'tbXmlFile
        '
        Me.tbXmlFile.Enabled = False
        Me.tbXmlFile.Location = New System.Drawing.Point(9, 61)
        Me.tbXmlFile.Name = "tbXmlFile"
        Me.tbXmlFile.Size = New System.Drawing.Size(178, 20)
        Me.tbXmlFile.TabIndex = 26
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(191, 60)
        Me.btnBrowse.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(50, 21)
        Me.btnBrowse.TabIndex = 27
        Me.btnBrowse.Text = "Browse"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'tbJsFile
        '
        Me.tbJsFile.Enabled = False
        Me.tbJsFile.Location = New System.Drawing.Point(9, 22)
        Me.tbJsFile.Name = "tbJsFile"
        Me.tbJsFile.Size = New System.Drawing.Size(229, 20)
        Me.tbJsFile.TabIndex = 29
        '
        'lblJsPath
        '
        Me.lblJsPath.AutoSize = True
        Me.lblJsPath.Location = New System.Drawing.Point(6, 6)
        Me.lblJsPath.Name = "lblJsPath"
        Me.lblJsPath.Size = New System.Drawing.Size(83, 13)
        Me.lblJsPath.TabIndex = 28
        Me.lblJsPath.Text = "Javascript Path:"
        '
        'timerSaveConfig
        '
        Me.timerSaveConfig.Interval = 250
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(250, 350)
        Me.ControlBox = False
        Me.Controls.Add(Me.tbJsFile)
        Me.Controls.Add(Me.lblJsPath)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.tbXmlFile)
        Me.Controls.Add(Me.panBottom)
        Me.Controls.Add(Me.lblXmlPath)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmMain"
        Me.Text = "C-Bus² Toolbox"
        Me.panBottom.ResumeLayout(False)
        Me.panBottom.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents xmlBrowser As System.Windows.Forms.OpenFileDialog
    Friend WithEvents lblXmlPath As System.Windows.Forms.Label
    Friend WithEvents panBottom As System.Windows.Forms.Panel
    Friend WithEvents lblType As System.Windows.Forms.Label
    Friend WithEvents rbCgate As System.Windows.Forms.RadioButton
    Friend WithEvents rbSerial As System.Windows.Forms.RadioButton
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents cboSystems As System.Windows.Forms.ComboBox
    Friend WithEvents lblSystem As System.Windows.Forms.Label
    Friend WithEvents tbXmlFile As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents btnReload As System.Windows.Forms.Button
    Friend WithEvents tbJoin202 As System.Windows.Forms.TextBox
    Friend WithEvents lblBaseJoin202 As System.Windows.Forms.Label
    Friend WithEvents tbJoin56 As System.Windows.Forms.TextBox
    Friend WithEvents lblBaseJoin56 As System.Windows.Forms.Label
    Friend WithEvents tbJsFile As System.Windows.Forms.TextBox
    Friend WithEvents lblJsPath As System.Windows.Forms.Label
    Friend WithEvents cboLevelDisplay As System.Windows.Forms.ComboBox
    Friend WithEvents lblDisplay As System.Windows.Forms.Label
    Friend WithEvents btnDebug As System.Windows.Forms.Button
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents timerSaveConfig As System.Windows.Forms.Timer
End Class
