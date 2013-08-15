Imports CommandFusion
Imports System.Xml
Imports System.IO
Imports System.Windows.Forms

Public Class frmMain
#Region " Declarations "
    Implements CFPlugin

    Private systemList As New List(Of JSONSystem)
    Private macroList As New List(Of SystemMacro)
    Private xmlConfig As Xmlconfig
    Private WithEvents xmlView As New CustomControls.TriStateTreeView
    Private bIsProjectSelected As Boolean
    Private bIsProjectCBusEnabled As Boolean
    Private sProjectPath As String
    Private sConfigFile As String
    Private sScriptFile As String
    Private miDebug As MenuItem
    Private miUiState As MenuItem
    Private bDebug As Boolean = False
    Private sActionSelectors As String
    Private xmlNodeSelected(1) As String

    Private Const _CHECKED As Integer = 0
    Private Const _MIXED As Integer = 1

    Private Const TXTNOSYSTEMS As String = "No Systems Found"
    Private Const TXTNOPROJECT As String = "No Open Projects"
    Private Const TXTMULTISYS As String = "Please Select System"
    Private Const TXTSINGLEGROUP As String = "Add Group Commands"
    Private Const TXTAPPGROUPS As String = "Add All App's Commands"
    Private Const TXTSCENEAREA As String = "Add Scene Area"
    Private Const TXTSCENE As String = "Add Scene"
    Private Const TXTALLSCENES As String = "Add All Scene Areas"

    Private Const _XML_xmlNodeChecked As String = "xmlNodeChecked"
    Private Const _XML_xmlNodeMixed As String = "xmlNodeMixed"
    Private Const _XML_cbusXmlFile As String = "cbusXmlFile"
    Private Const _XML_baseJoin56 As String = "baseJoin56"
    Private Const _XML_baseJoin202 As String = "baseJoin202"
    Private Const _XML_levelDisplayIndex As String = "levelDisplayType"
    Private Const _XML_selectedSystem As String = "selectedSystem"
    Private Const _XML_showUi As String = "showUi"

    Private Const _MENU_PluginShow As String = "Show Toolbox"
    'Private Const _MENU_PluginHide As String = "Hide Toolbox"
    Private Const _MENU_PluginEnable As String = "Enable C-Bus² Toolbox for Project"

#End Region

#Region " Plugin Integration "
    Public Event AddCommand(ByVal sender As CommandFusion.CFPlugin, ByVal newCommand As CommandFusion.SystemCommand) Implements CommandFusion.CFPlugin.AddCommand
    Public Event AddFeedback(ByVal sender As CommandFusion.CFPlugin, ByVal newFB As CommandFusion.SystemFeedback) Implements CommandFusion.CFPlugin.AddFeedback
    Public Event AddMacro(ByVal sender As CommandFusion.CFPlugin, ByVal newMacro As CommandFusion.SystemMacro) Implements CommandFusion.CFPlugin.AddMacro
    Public Event AddMacros(ByVal sender As CommandFusion.CFPlugin, ByVal newMacros As System.Collections.Generic.List(Of CommandFusion.SystemMacro)) Implements CommandFusion.CFPlugin.AddMacros
    Public Event AddSystem(ByVal sender As CommandFusion.CFPlugin, ByVal newSystem As CommandFusion.JSONSystem) Implements CommandFusion.CFPlugin.AddSystem
    Public Event AppendSystem(ByVal sender As CommandFusion.CFPlugin, ByVal newSystem As CommandFusion.JSONSystem) Implements CommandFusion.CFPlugin.AppendSystem
    Public Event RequestSystemList(ByVal sender As CommandFusion.CFPlugin) Implements CommandFusion.CFPlugin.RequestSystemList
    Public Event ToggleWindow(ByVal sender As CommandFusion.CFPlugin) Implements CommandFusion.CFPlugin.ToggleWindow
    Public Event RequestMacroList(ByVal sender As CFPlugin) Implements CommandFusion.CFPlugin.RequestMacroList
    Public Event WriteToLog(ByVal sender As CommandFusion.CFPlugin, ByVal msg As String) Implements CommandFusion.CFPlugin.WriteToLog
    Public Event EditMacro(ByVal sender As CFPlugin, ByVal existingMacro As String, ByVal newMacro As SystemMacro) Implements CommandFusion.CFPlugin.EditMacro
    Public Event RequestProjectFileInfo(ByVal sender As CFPlugin) Implements CommandFusion.CFPlugin.RequestProjectFileInfo
    Public Event AddScript(ByVal sender As CFPlugin, ByVal ScriptRelativePathToProject As String) Implements CommandFusion.CFPlugin.AddScript

    Public ReadOnly Property Author() As String Implements CommandFusion.CFPlugin.Author
        Get
            Return "Ben Nuttall [ben@nuttall.co.nz]"
        End Get
    End Property

    Public Sub DisposePlugin() Implements CommandFusion.CFPlugin.DisposePlugin
        configSave()
        Me.Form.Close()
    End Sub

    Public ReadOnly Property Form() As System.Windows.Forms.Form Implements CommandFusion.CFPlugin.Form
        Get
            Return Me
        End Get
    End Property

    Sub DebugMenu_Click()
        bDebug = Not bDebug
        miDebug.Checked = bDebug
        btnDebug.Visible = bDebug
    End Sub

    Public Sub Init(ByVal menu As System.Windows.Forms.MainMenu) Implements CommandFusion.CFPlugin.Init
        lblVersion.Text = "v0.95"

        Dim pluginMenu As New MenuItem(Me.NamePlugin)
        miUiState = New MenuItem(_MENU_PluginShow)
        AddHandler miUiState.Click, AddressOf DoToggleWindow
        miUiState.Checked = Me.Visible
        pluginMenu.MenuItems.Add(miUiState)

        miDebug = New MenuItem("Show Debug Messages")
        AddHandler miDebug.Click, AddressOf DebugMenu_Click
        pluginMenu.MenuItems.Add(miDebug)

        Dim menuHelp As New MenuItem("Help")
        pluginMenu.MenuItems.Add(menuHelp)
        AddHandler menuHelp.Click, AddressOf help_Click

        Dim menuAbout As New MenuItem("About")
        pluginMenu.MenuItems.Add(menuAbout)
        AddHandler menuAbout.Click, AddressOf about_Click

        menu.MenuItems.Add(pluginMenu)
        For i As Integer = 0 To 1
            xmlNodeSelected(i) = ","
        Next

        ProjectSelected(False)
        If IO.File.Exists(configGet(_XML_cbusXmlFile)) Then ParseCBusXML(configGet(_XML_cbusXmlFile))
        cboLevelDisplay.SelectedIndex = configGet(_XML_levelDisplayIndex)
        If cboLevelDisplay.SelectedIndex < 0 Then cboLevelDisplay.SelectedIndex = 0
        tbJoin56.Text = configGet(_XML_baseJoin56)
        tbJoin202.Text = configGet(_XML_baseJoin202)

        Me.Width = 250

        Me.Controls.Add(xmlView)
        xmlView.Location = New System.Drawing.Point(9, 90)
        xmlView.Anchor = AnchorStyles.Top + AnchorStyles.Left
        xmlView.CheckBoxes = True
        xmlView.CheckBoxesTriState = True
        xmlView.HideSelection = True

        RaiseEvent RequestSystemList(Me)
        RaiseEvent RequestMacroList(Me)

    End Sub

    'Private Sub timerUiState_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerUiState.Tick
    '    miUiState.Checked = Me.Visible
    '    If CBool(configGet(_XML_showUi)) = Me.Visible Then
    '        timerUiState.Enabled = False
    '    Else
    '        DoToggleWindow(Nothing, Nothing)
    '    End If
    'End Sub

    Public ReadOnly Property NamePlugin() As String Implements CommandFusion.CFPlugin.Name
        Get
            Return "C-Bus" & Chr(178)
        End Get
    End Property

    Public Sub ProjectSelected(ByVal selected As Boolean) Implements CommandFusion.CFPlugin.ProjectSelected
        If selected Then
            'The following event manages the loading of the system list, macro list & settings from the xml config file
            RaiseEvent RequestProjectFileInfo(Me)
        Else
            configSave()
            RaiseEvent RequestSystemList(Me)
        End If
    End Sub

    Public Sub UpdateMacroList(ByVal theMacroList As System.Collections.Generic.List(Of SystemMacro)) Implements CommandFusion.CFPlugin.UpdateMacroList
        macroList = theMacroList
    End Sub

    Public Sub UpdateSystemList(ByVal theSystemList As System.Collections.Generic.List(Of CommandFusion.JSONSystem), ByVal systemTypes As System.Collections.Generic.List(Of CommandFusion.JSONSystem)) Implements CommandFusion.CFPlugin.UpdateSystemList
        If bDebug Then MsgBox("Updating System List")
        Dim i As Integer = 0
        Dim iSystem As Integer = -1

        Try
            cboSystems.Items.Clear()
            If Not theSystemList Is Nothing Then
                systemList = theSystemList
                For Each aSystem As JSONSystem In systemList
                    If Not aSystem.GetSetting("ip").Value = "127.0.0.1" And Not aSystem.GetSetting("ip").Value = "255.255.255.255" Then
                        cboSystems.Items.Add(aSystem.Name)
                        If aSystem.Name = configGet(_XML_selectedSystem) Then iSystem = i
                        i += 1
                    End If
                Next

                If cboSystems.Items.Count > 1 Then
                    cboSystems.Items.Insert(0, TXTMULTISYS)
                    If iSystem >= 0 Then cboSystems.SelectedIndex = iSystem + 1 Else cboSystems.SelectedIndex = 0
                    panBottom.Enabled = True
                ElseIf cboSystems.Items.Count = 1 Then
                    cboSystems.SelectedIndex = 0
                    configSet(_XML_selectedSystem, cboSystems.SelectedIndex)
                    panBottom.Enabled = True
                Else
                    cboSystems.Items.Add(TXTNOSYSTEMS)
                    cboSystems.SelectedIndex = 0
                    panBottom.Enabled = True
                End If
            Else
                sConfigFile = ""
                tbJsFile.Text = ""
                cboSystems.Items.Add(TXTNOPROJECT)
                cboSystems.SelectedIndex = 0
                panBottom.Enabled = False

                'Reset Form - This occurs between projects
                cbusUncheckAllNodes()
                miUiState.Text = _MENU_PluginShow
                'miUiState.Enabled = False
                bIsProjectSelected = False
                bIsProjectCBusEnabled = False
            End If

            If bManualRefresh Then
                MsgBox("Systems list reloaded successfully", 64, "Systems Refresh Complete")
                bManualRefresh = False
            End If
        Catch e As Exception
            MsgBox("UpdateSystemCombo: " & e.Message)
        End Try
    End Sub

    Private Sub GetProjectFileInfo(ByVal ProjectFile As System.IO.FileInfo) Implements CommandFusion.CFPlugin.GetProjectFileInfo
        Try
            'Save old settings file if different file 
            If Not sConfigFile = ProjectFile.Directory.FullName & "\cbusConfig.xml" Or Not bIsProjectCBusEnabled Then
                configSave()
                bIsProjectSelected = True
                sScriptFile = ProjectFile.Directory.FullName & "\cbus.js"
                sConfigFile = ProjectFile.Directory.FullName & "\cbusConfig.xml"
                tbJsFile.Text = sScriptFile
                configLoad()
            End If
        Catch ex As Exception
            If bDebug Then MsgBox("GetProjectFileInfo: " & ex.Message())
        End Try
    End Sub

    Private Sub DoToggleWindow(ByVal sender As Object, ByVal e As System.EventArgs)
        If miUiState.Text = _MENU_PluginEnable Then
            If MsgBox("To use the CBus Toolbox in a project, you must have a broadcast system (255.255.255.255), and a config file must be placed in your project root directory (cbusConfig.xml).  Neither of these actions will effect an existing project, but as always we recommend a backup." & vbCrLf & vbCrLf & "Is it ok for me to ensure a broadcase system exists, and create a config file?", 36, "CBus Toolbox Project Initialisation") = vbYes Then
                xmlConfig = New Xmlconfig(sConfigFile, True)
                CheckBroadcastSystem()
            Else
                Exit Sub
            End If
        End If
        RaiseEvent ToggleWindow(Me)
        If bIsProjectSelected And bIsProjectCBusEnabled Then configSet(_XML_showUi, Me.Visible.ToString)
        If Me.Visible Then
            miUiState.Checked = True
            If bIsProjectSelected And Not bIsProjectCBusEnabled Then configLoad()
        Else
            miUiState.Checked = False
        End If
    End Sub

#End Region

#Region " User Interface "
    Public Sub showXmlChooser()
        xmlBrowser.InitialDirectory = configGet(_XML_cbusXmlFile)
        xmlBrowser.ShowDialog()

        If xmlBrowser.FileName.Length > 0 Then
            configSet(_XML_cbusXmlFile, xmlBrowser.FileName)
            ParseCBusXML(xmlBrowser.FileName)
        End If
    End Sub

    Private Sub help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'MsgBox("Something helpful here would be of enormous use!", MsgBoxStyle.OkOnly, "C-Bus" & Chr(178) & " Wizard Help")
        Dim dlgHelp As New help
        dlgHelp.Show()
    End Sub

    Private Sub about_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'MsgBox("C-Bus" & Chr(178) & " command generator wizard.  Some info here?", MsgBoxStyle.OkOnly, "About C-Bus" & Chr(178) & " Wizard")
        'Dim dlgAbout As New aboutBox
        'dlgAbout.Show()
    End Sub

    Private Sub cboSystems_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSystems.SelectedIndexChanged
        configSet(_XML_selectedSystem, cboSystems.Text)
    End Sub

    Private Sub frmMain_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        xmlView.Size = New Drawing.Size(Me.Width - 18, Me.Height - panBottom.Height - 50 - 5)
        btnBrowse.Left = Me.Width - 59
        btnReload.Left = Me.Width - 59
        tbXmlFile.Width = Me.Width - 72
        cboSystems.Width = Me.Width - 113
    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        showXmlChooser()
    End Sub

    Private Sub btnReload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReload.Click
        bManualRefresh = True
        RaiseEvent RequestSystemList(Me)
    End Sub

    Private Sub tbJoin_KeyPress202(ByVal sender As System.Object, ByVal e As KeyPressEventArgs) Handles tbJoin202.KeyPress
        Dim allowedChars As String = "0123456789"
        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            If Not e.KeyChar = Chr(8) Then e.Handled = True
        End If
    End Sub

    Private Sub tbJoin202_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbJoin202.TextChanged
        configSet(_XML_baseJoin202, tbJoin202.Text)
    End Sub

    Private Sub tbJoin56_KeyPress(ByVal sender As System.Object, ByVal e As KeyPressEventArgs) Handles tbJoin56.KeyPress
        Dim allowedChars As String = "0123456789"
        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            If Not e.KeyChar = Chr(8) Then e.Handled = True
        End If
    End Sub

    Private Sub tbJoin56_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbJoin56.TextChanged
        configSet(_XML_baseJoin56, tbJoin56.Text)
    End Sub

    Private Sub cboLevelDisplay_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboLevelDisplay.SelectedIndexChanged
        configSet(_XML_levelDisplayIndex, cboLevelDisplay.SelectedIndex)
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        go()
    End Sub

    Private Sub btnDebug_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDebug.Click
        MsgBox(configGet(_XML_xmlNodeChecked))
    End Sub

#End Region

#Region " guiD API Exensions "
    Function GetSystemByName(ByVal sName As String) As JSONSystem
        Dim ret As New JSONSystem
        For Each sys As JSONSystem In systemList
            If sys.Name = sName Then
                ret = sys
                Exit For
            End If
        Next
        Return ret
    End Function

    Function GetSystemByIP(ByVal sIpAddress As String) As JSONSystem
        Dim ret As New JSONSystem
        Try
            For Each sys As JSONSystem In systemList
                If sys.GetSetting("ip").Value = sIpAddress Then
                    ret = sys
                    Exit For
                End If
            Next
        Catch ex As Exception
            If bDebug Then MsgBox("getSystemByIP: " & ex.Message)
        End Try
        Return ret
    End Function

    Function AddNewCommand(ByVal theSystem As JSONSystem, ByVal sName As String, Optional ByVal sValue As String = "", Optional ByVal sJavascript As String = "", Optional ByVal bAutoOverwrite As Boolean = False) As CommandFusion.SystemCommand
        Dim cmd As SystemCommand
        cmd = New SystemCommand
        cmd.Name = sName
        cmd.Value = sValue
        cmd.Script = sJavascript
        cmd.System = theSystem.Name

        Dim cmdExisting As SystemCommand
        Dim bCmdExists As Boolean
        Dim bCmdOverwrite As Boolean = bCmdOverwriteAll
        Dim bCmdIgnore As Boolean = bCmdIgnoreAll

        cmdExisting = theSystem.GetCommandByName(cmd.Name)
        bCmdExists = Not cmdExisting Is Nothing

        If bCmdExists Then
            If Not cmdExisting.Value = cmd.Value Or Not cmdExisting.Target = cmd.Target Or Not cmdExisting.TargetValue = cmd.TargetValue Or Not cmdExisting.Script = cmd.Script Then
                'Command values changed
                If Not bCmdIgnore And Not bCmdOverwrite And Not bAutoOverwrite Then
                    Dim msg As New MsgDialog
                    msg.TextBox1.Text = "The command '" & cmd.Name & "' exists, but with different values.  Overwrite with new values?"
                    Select Case msg.ShowDialog
                        Case vbYes
                            bCmdOverwrite = True
                        Case vbNo
                            bCmdIgnore = True
                        Case vbCancel
                            'No to All
                            bCmdIgnore = True
                            bCmdIgnoreAll = True
                        Case vbIgnore
                            'Yes to All
                            bCmdOverwrite = True
                            bCmdOverwriteAll = True
                    End Select
                End If
            End If
            If bCmdOverwrite Or bAutoOverwrite Then
                If Not bAutoOverwrite Then iCUpdated += 1
                theSystem.Commands.Remove(cmdExisting)
                theSystem.Commands.Add(cmd)
            Else
                If Not bAutoOverwrite Then iCUnchanged += 1
            End If
        Else
            theSystem.Commands.Add(cmd)
            iCNew += 1
        End If
        Return cmd
    End Function

    Function AddNewMacro(ByVal sName As String, ByVal cmds() As SystemCommand, ByVal delays() As Integer)
        Dim bExisting As Boolean = False
        Dim macro As New SystemMacro
        macro.Name = sName

        Try
            For i = 0 To UBound(cmds)
                macro.Commands.Add(cmds(i))
                Try : macro.Delays.Add(delays(i)) : Catch : End Try
            Next

            For Each existingMacro As SystemMacro In macroList
                If existingMacro.Name = sName Then
                    RaiseEvent EditMacro(Me, sName, macro)
                    bExisting = True
                    'macro = existingMacro
                    'macro.Commands.Clear()
                    'macro.Delays.Clear()
                    Exit For
                End If
            Next
            If Not bExisting Then RaiseEvent AddMacro(Nothing, macro)

        Catch ex As Exception
            MsgBox("NewMacro: " & ex.Message)
        End Try

        Return macro
    End Function

    Function AddNewFb(ByVal theSystem As JSONSystem, ByVal sFbName As String, ByVal sFbValue As String) As SystemFeedback
        Dim fb As SystemFeedback
        Dim bFbExists As Boolean

        fb = theSystem.GetFeedbackByName(sFbName)
        bFbExists = Not fb Is Nothing
        If Not bFbExists Then fb = New SystemFeedback

        fb.Name = sFbName
        fb.Value = sFbValue
        fb.System = theSystem.Name

        If Not bFbExists Then theSystem.Feedback.Add(fb)

        Return fb
    End Function

    Sub AddNewFbMatch(ByVal theSystem As JSONSystem, ByVal sFbName As String, ByVal cmd As SystemCommand, ByVal mac As SystemMacro)
        Dim fb As SystemFeedback
        Dim fbM As New SystemFeedbackMatchElement
        Dim bElementExists As Boolean = False

        fb = theSystem.GetFeedbackByName(sFbName)
        'Exit sub if feedback parent not found
        If fb Is Nothing Then Exit Sub

        For Each element As Object In fb.DataElements
            If TypeOf (element) Is SystemFeedbackMatchElement Then
                If element.CommandName.Length > 0 Then
                    If element.Command.Value = cmd.Value Then
                        fbM = element
                        bElementExists = True
                        Exit For
                    End If
                End If
            End If
            'If element.Macro > 0 And element.Macro = mac.Name Then
            '    fbM = element
            '    bElementExists = True
            '    Exit For
            'End If
        Next

        Try
            fb = theSystem.GetFeedbackByName(sFbName)
            'Exit sub if feedback parent not found
            If fb Is Nothing Then Exit Sub

            If Not cmd Is Nothing Then fbM.Command = cmd
            If Not mac Is Nothing Then fbM.Macro = mac.Name
            fbM.TargetType = SystemFeedbackElement.TargetTypes.Global
            'fbM.Token = False

            If Not bElementExists Then fb.DataElements.Insert(fb.DataElements.Count, fbM) '.Add(fbM)
        Catch ex As Exception
            MsgBox("FbMatch: " & ex.Message)
        End Try

    End Sub

    Sub AddNewFbToken(ByVal theSystem As JSONSystem, ByVal sFbName As String, ByVal sFbElemName As String, ByVal iCaptureIndex As Integer, _
        ByVal DataType As Char, ByVal targetType As Char)
        Dim fb As SystemFeedback
        Dim fbE As New SystemFeedbackElement

        Dim bElementExists As Boolean = False

        Try
            fb = theSystem.GetFeedbackByName(sFbName)
            'Exit sub if feedback parent not found
            If fb Is Nothing Then Exit Sub

            For Each element As Object In fb.DataElements
                If TypeOf (element) Is SystemFeedbackElement Then
                    If element.Name = sFbElemName Then
                        fbE = element
                        bElementExists = True
                        Exit For
                    End If
                End If
            Next

            fbE.Name = sFbElemName
            fbE.CaptureIndex = iCaptureIndex
            fbE.DataType = DataType
            fbE.TargetType = targetType
            fbE.TokenType = SystemFeedbackElement.TokenTypes.Token

            If Not bElementExists Then fb.DataElements.Insert(fb.DataElements.Count, fbE) '.Add(fbE)

        Catch ex As Exception
            MsgBox("FbToken2: " & ex.Message)
        End Try

    End Sub

    Sub AddNewFbJoin(ByVal theSystem As JSONSystem, ByVal sFbName As String, ByVal sFbElemName As String, ByVal iCaptureIndex As Integer, ByVal iJoin As Integer, _
        ByVal DataType As Char, ByVal targetType As Char, Optional ByVal tokenType As CommandFusion.SystemFeedbackElement.TokenTypes = SystemFeedbackElement.TokenTypes.Value, _
        Optional ByVal transform As String = Nothing, Optional ByVal hexType As CommandFusion.SystemFeedbackElement.HexModes = SystemFeedbackElement.HexModes.NonHex, _
        Optional ByVal max As String = Nothing, Optional ByVal min As String = Nothing, Optional ByVal onVal As String = Nothing, Optional ByVal offVal As String = Nothing)

        Try
            Dim fb As SystemFeedback
            Dim fbE As New SystemFeedbackElement
            Dim bElementExists As Boolean = False

            fb = theSystem.GetFeedbackByName(sFbName)
            'Exit sub if feedback parent not found
            If fb Is Nothing Then Exit Sub

            For Each element As Object In fb.DataElements
                If TypeOf (element) Is SystemFeedbackElement Then
                    If element.Name = sFbElemName Then
                        fbE = element
                        bElementExists = True
                        Exit For
                    End If
                End If
            Next

            fbE.Name = sFbElemName
            fbE.CaptureIndex = iCaptureIndex
            fbE.Join = iJoin
            fbE.DataType = DataType
            fbE.TargetType = targetType
            fbE.TokenType = tokenType
            If Not transform Is Nothing Then fbE.Transform = transform
            fbE.Hex = hexType
            If Not max Is Nothing Then fbE.MaxValue = max
            If Not min Is Nothing Then fbE.MinValue = min
            If Not onVal Is Nothing Then fbE.OnValue = onVal
            If Not offVal Is Nothing Then fbE.OffValue = offVal

            If Not bElementExists Then fb.DataElements.Insert(fb.DataElements.Count, fbE) '.Add(fbE)
        Catch ex As Exception
            MsgBox("Add Join Error: " & ex.Message)
        End Try
    End Sub
#End Region

#Region " XML Config Management "
    Function configGet(ByVal sProperty As String) As String
        Try
            If bIsProjectSelected Then
                If Len(xmlConfig.Settings(sProperty).Value) > 0 Then
                    Return xmlConfig.Settings(sProperty).Value
                End If
            End If

            Select Case sProperty
                Case _XML_cbusXmlFile
                    Return My.Settings.lastXmlFile
                Case _XML_baseJoin202
                    Return My.Settings.baseJoin202
                Case _XML_baseJoin56
                    Return My.Settings.baseJoin56
                Case _XML_levelDisplayIndex
                    Return My.Settings.levelDisplayIndex
                Case _XML_xmlNodeChecked
                    Return ","
                Case _XML_xmlNodeMixed
                    Return ","
                Case Else
                    Return ""
            End Select

        Catch ex As Exception
            If bDebug Then MsgBox("configGet: " & ex.Message)
            Return ""
        End Try
    End Function

    Sub configSet(ByVal sProperty As String, ByVal sValue As String)
        Try
            If bIsProjectSelected Then
                xmlConfig.Settings(sProperty).Value = sValue
            End If

            Select Case sProperty
                Case _XML_cbusXmlFile
                    My.Settings.lastXmlFile = sValue
                Case _XML_baseJoin202
                    My.Settings.baseJoin202 = sValue
                Case _XML_baseJoin56
                    My.Settings.baseJoin56 = sValue
                Case _XML_levelDisplayIndex
                    My.Settings.levelDisplayIndex = sValue
                Case Else
            End Select
        Catch ex As Exception
            If bDebug Then MsgBox("configSet: " & ex.Message)
        End Try
    End Sub

    Sub configLoad()
        Try
            RaiseEvent RequestMacroList(Me)
            RaiseEvent RequestSystemList(Me)

            If bDebug Then MsgBox("Loading CBus settings from '" & sConfigFile & "'.")
            If Not File.Exists(sConfigFile) Then
                'Me.Enabled = False
                If Me.Visible Then DoToggleWindow(Nothing, Nothing)
                miUiState.Text = _MENU_PluginEnable
                miUiState.Enabled = True
                bIsProjectCBusEnabled = False
                'Exit Try
                Exit Sub
            End If
            'Me.Enabled = True
            xmlConfig = New Xmlconfig(sConfigFile, False)
            CheckBroadcastSystem()
            miUiState.Text = _MENU_PluginShow
            miUiState.Enabled = True
            bIsProjectCBusEnabled = True
            configSet(_XML_cbusXmlFile, configGet(_XML_cbusXmlFile))
            xmlNodeSelected(_CHECKED) = configGet(_XML_xmlNodeChecked)
            xmlNodeSelected(_MIXED) = configGet(_XML_xmlNodeMixed)
            tbJoin56.Text = configGet(_XML_baseJoin56)
            tbJoin202.Text = configGet(_XML_baseJoin202)
            cboLevelDisplay.SelectedIndex = configGet(_XML_levelDisplayIndex)
            miUiState.Checked = Me.Visible
            If Not CBool(configGet(_XML_showUi)) = Me.Visible Then
                DoToggleWindow(Nothing, Nothing)
            End If
        Catch ex As Exception
            'If bDebug Then MsgBox(ex.Message)
        End Try

        If Not configGet(_XML_cbusXmlFile) = "" Then ParseCBusXML(configGet(_XML_cbusXmlFile))
    End Sub

    Sub configSave()
        If bDebug Then MsgBox("Saving CBus settings to '" & sConfigFile & "'.")
        Try
            If bIsProjectSelected And bIsProjectCBusEnabled Then
                configSet(_XML_baseJoin202, tbJoin202.Text)
                configSet(_XML_baseJoin56, tbJoin56.Text)
                configSet(_XML_levelDisplayIndex, cboLevelDisplay.SelectedIndex)
                configSet(_XML_showUi, Me.Visible.ToString)
                xmlConfig.Save(sConfigFile)
            End If
            My.Settings.Save()
        Catch ex As Exception
            If bDebug Then MsgBox("configSave: " & ex.Message)
        End Try
    End Sub
#End Region

#Region " Javascript File Management & Generation "
    '/===============================================================================\
    '|  The JS file cbus.js is generated from the embedded resource of the same
    '|  name.  When the file is written, it is prepended with a checksum (MD5 hash).
    '|  The purpose of this is to ascertain if the file has been externally modified,
    '|  or if the file needs updating (because the addin has been updated).
    '\===============================================================================/

    Const _MD5TEXT As Integer = 0
    Const _MD5CALC As Integer = 1

    Private Function jsGetExistingMD5(ByVal sFile As String) As String()
        Dim sMD5(1) As String
        Try
            If File.Exists(sFile) Then
                Dim bMd5Found As Boolean = False
                Dim reader As StreamReader = New StreamReader(sFile)
                Dim sContents As String = reader.ReadToEnd()
                reader.Close()
                If sContents.Contains("JSMD5-->") And sContents.Contains("<--JSMD5") Then
                    sMD5(_MD5TEXT) = Split(Split(sContents, "JSMD5-->")(1), "<--JSMD5")(0)
                    sMD5(_MD5CALC) = MD5CalcString(Split(sContents, "<--JSMD5")(1))
                Else
                    sMD5(_MD5CALC) = MD5CalcString(sContents)
                End If
            End If
        Catch ex As Exception
            If bDebug Then MsgBox("jsGetExistingMD5: " & ex.Message)
        End Try
        Return sMD5
    End Function

    Private Function jsGetScript(ByVal sSystemName As String)
        Dim js As String = My.Resources.cbus
        If js.Contains("***SCRIPT STARTS HERE***") Then
            'js.Split("***SCRIPT STARTS HERE***")(0) Contains version of script, and variables
            js = Split(js, "***SCRIPT STARTS HERE***")(1)
        End If
        js = js.Replace("[baseJoin56]", tbJoin56.Text)
        js = js.Replace("[baseJoin202]", tbJoin202.Text)
        js = js.Replace("[selectedSystem]", sSystemName)
        js = js.Replace("[broadcastSystem]", sysBroadcast.Name)
        js = js.Replace("[actionselectors]", sActionSelectors)
        If cboLevelDisplay.Text.ToLower.Contains("percent") Then
            js = js.Replace("[bPercent]", "1")
        Else
            js = js.Replace("[bPercent]", "0")
        End If
        Return js
    End Function

    Private Sub jsWriteFile(ByVal sSystemName As String)
        Dim sFile As String = sScriptFile
        Dim sJsStr As String = jsGetScript(sSystemName)
        Dim sOldMD5() As String = jsGetExistingMD5(sFile)
        Dim sNewMD5 As String = MD5CalcString(sJsStr)

        Dim bOkToWrite As Boolean = False
        Dim bBackup As Boolean = False

        If sOldMD5(_MD5TEXT) = sOldMD5(_MD5CALC) And sNewMD5 = sOldMD5(_MD5TEXT) Then
            If bDebug Then MsgBox("Skipping script update, already current.")
        ElseIf sOldMD5(_MD5TEXT) = sOldMD5(_MD5CALC) Then
            If bDebug Then MsgBox("Script updated (previously unmodified).")
            bOkToWrite = True
        ElseIf sOldMD5(_MD5TEXT) = "" Then
            If MsgBox("The javascript code file (cbus.js) has been manually modified (and MD5 checksum removed, or never existed).  Are you sure you want to overwrite this file?" & vbCrLf & vbCrLf & "Your existing script will be backed up.", vbYesNo, "CBus.js Overwrite - Are You Sure?") = vbYes Then
                bOkToWrite = True : bBackup = True
            End If
        Else
            If MsgBox("The javascript code file (cbus.js) has been manually modified. Are you sure you want to overwrite this file?" & vbCrLf & vbCrLf & "Your existing script will be backed up.", vbYesNo, "CBus.js Overwrite - Are You Sure?") = vbYes Then
                bOkToWrite = True : bBackup = True
            End If
        End If

        Try
            If bBackup = True Then File.Move(sFile, sFile.Substring(0, sFile.Length - 3) & "." & Now.Year & alz(Now.Month) & alz(Now.Day) & "-" & alz(Now.Hour) & alz(Now.Minute) & alz(Now.Second) & ".js")
            If bOkToWrite Then
                Dim writer As New StreamWriter(sFile)
                writer.Write("//Don't touch this line JSMD5-->" & sNewMD5 & "<--JSMD5" & sJsStr)
                writer.Close()
            End If
        Catch ex As Exception
            If bBackup Then
                MsgBox("Error backing up or writing to Javascript file.  Error information:" & vbCrLf & ex.Message)
            Else
                MsgBox("Error writing to Javascript file.  Error information:" & vbCrLf & ex.Message)
            End If
        End Try

        If bDebug Then MsgBox("1:" & sOldMD5(_MD5TEXT) & vbCrLf & "2:" & sOldMD5(_MD5CALC) & vbCrLf & "3:" & sNewMD5)

    End Sub
#End Region

#Region " Command Generation Routines "
    Dim bCmdOverwriteAll As Boolean = False
    Dim bCmdIgnoreAll As Boolean = False
    Dim iCUpdated As Integer
    Dim iCNew As Integer
    Dim iCUnchanged As Integer
    Dim bSkipCmd As Boolean
    Shared bManualRefresh As Boolean = False

    Dim sysBroadcast As JSONSystem

    Sub CheckBroadcastSystem()
        Try
            sysBroadcast = GetSystemByIP("255.255.255.255")
            If sysBroadcast.Name = "New System" Then
                'sysBroadcast = New SystemClass
                sysBroadcast.Name = "broadcast"
                sysBroadcast.GetSetting("ip").Value = "255.255.255.255"
                sysBroadcast.GetSetting("port").Value = "2048"
                sysBroadcast.GetSetting("origin").Value = "2048"
                sysBroadcast.GetSetting("alwayson").Value = True

                'systemList.Add(sysBroadcast)
                RaiseEvent AddSystem(Me, sysBroadcast)
                RaiseEvent RequestSystemList(Me)
            Else
                If Not sysBroadcast.GetSetting("port").Value = sysBroadcast.GetSetting("origin").Value Or _
                    Not sysBroadcast.GetSetting("alwayson").Value = True Or _
                    Not sysBroadcast.ID = "udp-socket" Then
                    'If MsgBox("A broadcast system was found (" & sysBroadcast.Name & "), but for this to function correctly origin & destination ports must match, always on must be selected, and it must use the UDP protocol." & vbCrLf & vbCrLf & "Can I make these changes now?", 36, "Broadcast System Found - Incorrect Settings") = vbYes Then
                    '    sysBroadcast.PortDestination = sysBroadcast.PortOrigin
                    '    sysBroadcast.AlwaysOn = True
                    '    sysBroadcast.ProtocolUsed = SystemClass.Protocol.UDP
                    'End If
                    'RaiseEvent AppendSystem(Me, sysBroadcast)
                    MsgBox("A broadcast system was found (" & sysBroadcast.Name & "), but for this to function correctly origin & destination ports must match, always on must be selected, and it must use the UDP protocol." & vbCrLf & vbCrLf & "Please ensure these changes are made.", 49, "Broadcast System Found - Incorrect Settings")
                End If
            End If
        Catch ex As Exception
            If bDebug Then MsgBox("checkBroadcastSystem: " & ex.Message)
        End Try
    End Sub

    Sub go()
        ' Add commands to the selected system
        Dim sysSelect As JSONSystem = Nothing
        Try
            Try
                If Not IsNumeric(tbJoin56.Text) Or Not IsNumeric(tbJoin202.Text) Then
                    MsgBox("All joins must be a number greater than 0", MsgBoxStyle.Critical, "Join ID Error")
                    Exit Sub
                End If

                If cboSystems.Items.Count > 1 And cboSystems.SelectedIndex = 0 Then
                    MsgBox("Please select a system from the system combo box", MsgBoxStyle.Critical, "No System Selected")
                    Exit Sub
                End If
            Catch ex As Exception
                If bDebug Then MsgBox("btnAdd, Warnings: " & ex.Message)
            End Try

            Try
                sysSelect = GetSystemByName(cboSystems.Text)
                'Dim sysBroadcast As SystemClass = GetSystemByName("broadcast")
            Catch ex As Exception
                If bDebug Then MsgBox("btnAdd, Load System: " & ex.Message)
            End Try

            Try
                jsWriteFile(sysSelect.Name)
                RaiseEvent AddScript(Me, "cbus.js")
            Catch ex As Exception
                If bDebug Then MsgBox("btnAdd, Write Script: " & ex.Message)
            End Try

            Try
                AddNewFb(sysSelect, "cbus network traffic", ".*")
                AddNewFb(sysBroadcast, "cbus broadcast traffic", "cbus:..:..:..;")
            Catch ex As Exception
                If bDebug Then MsgBox("btnAdd, Add Feedback: " & ex.Message)
            End Try

            Try
                doNodes(sysSelect, xmlView.Nodes(0))
            Catch ex As Exception
                If bDebug Then MsgBox("btnAdd, DoNodes: " & ex.Message)
            End Try

            Try
                Dim msgText As String = "Command summary:" & vbCrLf
                If iCNew > 0 Then msgText += "  " & Chr(149) & " " & iCNew & " commands added" & vbCrLf
                If iCUpdated > 0 Then msgText += "  " & Chr(149) & " " & iCUpdated & " existing commands updated" & vbCrLf
                If iCUnchanged > 0 Then msgText += "  " & Chr(149) & " " & iCUnchanged & " existing commands unchanged" & vbCrLf
                MsgBox(msgText, 64, "Task Complete")
            Catch ex As System.Exception
                If bDebug Then MsgBox("btnAdd, Command Summary: " & ex.Message)
            End Try

            bCmdIgnoreAll = False
            bCmdOverwriteAll = False
            iCNew = 0
            iCUpdated = 0
            iCUnchanged = 0
            RaiseEvent AppendSystem(Me, sysSelect)
            'RaiseEvent AppendSystem(Me, sysBroadcast)
            'RaiseEvent AppendSystem(Me, sysLocal)
        Catch ex As Exception
            If bDebug Then MsgBox("addBtn: " & ex.Message)
        End Try
    End Sub

    Sub doNodes(ByVal sysSelect As JSONSystem, ByVal parentNode As TreeNode)
        For Each node As TreeNode In parentNode.Nodes
            If node.Checked Then
                If node.Name.StartsWith("grp_254_56_") Then
                    'Lighting Group
                    Dim sProject As String = node.Parent.Parent.Parent.Tag
                    Dim iNetwork As Integer = node.Parent.Parent.Tag
                    Dim iApp As Integer = node.Parent.Tag
                    Dim iGroup As Integer = node.Tag.ToString.Split(Chr(30))(1)
                    Dim sGroup As String = node.Tag.ToString.Split(Chr(30))(0)

                    AddLightingCommands(sysSelect, sProject, iNetwork, iApp, iGroup, sGroup)

                ElseIf node.Name.StartsWith("as_254_202_") Then
                    'Action Selector (Scene)
                    Dim sProject As String = node.Parent.Parent.Parent.Parent.Tag
                    Dim iNetwork As Integer = node.Parent.Parent.Parent.Tag
                    Dim iApp As Integer = node.Parent.Parent.Tag
                    Dim iGroup As Integer = node.Parent.Tag.ToString.Split(Chr(30))(1)
                    Dim sGroup As String = node.Parent.Tag.ToString.Split(Chr(30))(0)
                    Dim iActionSelector As Integer = node.Tag.ToString.Split(Chr(30))(1)
                    Dim sActionSelector As String = node.Tag.ToString.Split(Chr(30))(0)

                    AddSceneCommands(sysSelect, sProject, iNetwork, iApp, iGroup, sGroup, iActionSelector, sActionSelector)

                End If
                doNodes(sysSelect, node)
            End If
        Next
    End Sub

    Sub AddSceneCommands(ByVal theSystem As JSONSystem, ByVal sProject As String, ByVal iNetwork As Integer, ByVal iApp As Integer, ByVal iGroup As Integer, ByVal sGroup As String, ByVal iActionSelector As Integer, ByVal sActionSelector As String)
        If iApp = 202 Then
            Dim sEOL As String = "\x0D"
            'Commands
            Try
                AddNewCommand(theSystem, "[scene] " & FixString(sGroup) & ", " & FixString(sActionSelector) & " (cbus" & iApp & ") (js)", ".", "cbus.scene(" & iApp & "," & iGroup & "," & iActionSelector & ");")
            Catch ex As Exception
                If bDebug Then MsgBox("Commands: " & ex.Message)
            End Try

            '# Status Request
            Dim iFbRequest As Integer = Math.Floor(iGroup / 32) * 32
            AddNewCommand(theSystem, "[level] Grp" & iFbRequest & "_" & (iFbRequest + 31) & " (cbus" & iApp & ")", GetCheckSum("\05FF007307CA" & dec2hexPair(iFbRequest)) & sEOL, , True)
        End If
    End Sub

    Sub AddLightingCommands(ByVal theSystem As JSONSystem, ByVal sProject As String, ByVal iNetwork As Integer, ByVal iApp As Integer, ByVal iGroup As Integer, ByVal sGroup As String)
        If iApp = 56 Then
            Dim sEOL As String = "\x0D"

            'Commands
            Try
                AddNewCommand(theSystem, "[ramp] " & FixString(sGroup) & " (cbus" & iApp & ") (js)", ".", "cbus.ramp(" & iApp & "," & iGroup & ");")
                AddNewCommand(theSystem, "[toggle] " & FixString(sGroup) & " (cbus" & iApp & ") (js)", ".", "cbus.toggle(" & iApp & "," & iGroup & ");")
            Catch ex As Exception
                If bDebug Then MsgBox("Commands: " & ex.Message)
            End Try

            '# Status Request
            Dim iFbRequest As Integer = Math.Floor(iGroup / 32) * 32
            AddNewCommand(theSystem, "[level] Grp" & iFbRequest & "_" & (iFbRequest + 31) & " (cbus" & iApp & ")", GetCheckSum("\05FF00730738" & dec2hexPair(iFbRequest)) & sEOL, , True)
        End If
    End Sub
#End Region

#Region " CBus-Specific Functions"
    Sub ParseCBusXML(ByVal sFileName As String)
        'xmlView
        Try
            Dim xmlDoc As XmlDocument

            'Dim OID As Integer = 0
            Dim TagName As Integer = 0
            Dim Address As Integer = 1

            Dim tnProject As TreeNode
            Dim tnNet As TreeNode
            Dim tnApp As TreeNode
            Dim iNodeCount As Integer

            sActionSelectors = ""

            xmlView.Nodes.Clear()

            Dim sr As System.IO.StreamReader = File.OpenText(sFileName)
            xmlDoc = New XmlDocument()
            Dim xmlR As XmlReader = XmlReader.Create(sr)
            xmlDoc.Load(xmlR)
            sr.Close()

            'xmlDoc.SelectSingleNode("/gui/properties/imagefolder").InnerText = sThemeFolder

            For Each nProj As XmlNode In xmlDoc.SelectNodes("/Installation/Project")

                'If nodeSys.Attributes.GetNamedItem("name").Value.ToLower = "[[" & drSys("sysTag").ToString.ToLower & "]]" Or nodeSys.Attributes.GetNamedItem("name").Value.ToLower = drSys("sysTag").ToString.ToLower Then
                '    nodeSys.Attributes.GetNamedItem("name").Value = drSys("sysName")
                '    nodeSys.Attributes.GetNamedItem("ip").Value = drSys("sysIp")
                '    nodeSys.Attributes.GetNamedItem("port").Value = drSys("sysPort")
                'End If

                'Print Project Node
                tnProject = xmlView.Nodes.Add(nProj.SelectSingleNode("TagName").InnerText & " (Project)")
                tnProject.Tag = nProj.SelectSingleNode("Address").InnerText
                tnProject.Name = "proj"
                checkNode(tnProject)
                For Each nNet As XmlNode In nProj.SelectNodes("Network")
                    tnNet = tnProject.Nodes.Add(nNet.SelectSingleNode("TagName").InnerText & " (" & nNet.SelectSingleNode("Address").InnerText & ")")
                    tnNet.Tag = nNet.SelectSingleNode("Address").InnerText
                    tnNet.Name = "net_" & nNet.SelectSingleNode("Address").InnerText
                    checkNode(tnNet)
                    For Each nApp As XmlNode In nNet.SelectNodes("Application")
                        Dim nodeOrder As New SortedList(Of Integer, String)
                        tnApp = tnNet.Nodes.Insert(CInt(nApp.SelectSingleNode("Address").InnerText), nApp.SelectSingleNode("TagName").InnerText & " (" & nApp.SelectSingleNode("Address").InnerText & ")")
                        tnApp.Tag = nApp.SelectSingleNode("Address").InnerText
                        tnApp.Name = "app_" & tnApp.Parent.Tag & "_" & tnApp.Tag
                        tnApp.EnsureVisible()
                        checkNode(tnApp)
                        iNodeCount = 0
                        For Each nGrp As XmlNode In nApp.SelectNodes("Group")
                            Dim tnGroup As New TreeNode
                            Try
                                Dim iGroup As Integer = nGrp.SelectSingleNode("Address").InnerText
                                Dim sLocalAs = "    triggerGroup" & iGroup & ": ["
                                tnGroup.Name = tnApp.Text & "_" & iGroup
                                tnGroup.Text = nGrp.SelectSingleNode("TagName").InnerText & " (" & nGrp.SelectSingleNode("Address").InnerText & ")"
                                tnGroup.Tag = nGrp.SelectSingleNode("TagName").InnerText & Chr(30) & iGroup
                                If iGroup >= 0 And iGroup < 255 Then
                                    nodeOrder.Add(iGroup, tnGroup.Name)
                                    tnApp.Nodes.Insert(nodeOrder.IndexOfKey(iGroup), tnGroup)
                                    tnGroup.Name = "grp_" & tnGroup.Parent.Parent.Tag & "_" & tnGroup.Parent.Tag & "_" & iGroup
                                    checkNode(tnGroup)
                                    For Each nLvl As XmlNode In nGrp.SelectNodes("level")
                                        Dim tnActionSelector As TreeNode = tnGroup.Nodes.Add(nLvl.SelectSingleNode("TagName").InnerText & " (" & nLvl.SelectSingleNode("Address").InnerText & ")")
                                        Dim iActionSelector As Integer = nLvl.SelectSingleNode("Address").InnerText
                                        sLocalAs += iActionSelector & ","
                                        tnActionSelector.Name = "as_" & tnActionSelector.Parent.Parent.Parent.Tag & "_" & tnActionSelector.Parent.Parent.Tag & "_" & iGroup & "_" & iActionSelector
                                        tnActionSelector.Tag = nLvl.SelectSingleNode("TagName").InnerText & Chr(30) & iActionSelector
                                        checkNode(tnActionSelector)
                                    Next
                                    If sLocalAs.Substring(sLocalAs.Length - 1) = "," Then
                                        If sActionSelectors.Length = 0 Then
                                            sActionSelectors = sLocalAs.Substring(0, sLocalAs.Length - 1) & "],"
                                        Else
                                            sActionSelectors += vbCrLf & sLocalAs.Substring(0, sLocalAs.Length - 1) & "],"
                                        End If
                                    End If
                                    iNodeCount += 1
                                End If
                            Catch
                            End Try
                        Next
                        If iNodeCount = 0 Then tnApp.Remove()
                    Next
                Next
            Next

            tbXmlFile.Text = sFileName
            xmlView.Refresh()

        Catch ex As Exception
            If bDebug Then MsgBox("ParseCBusXML: " & ex.Message)
        End Try
    End Sub

    'Sub ParseCBusXML(ByVal sFileName As String)
    '    'xmlView
    '    Try
    '        Dim xmld As XmlDocument
    '        Dim nodelist As XmlNodeList
    '        Dim nodeL1 As XmlNode
    '        Dim nodeL2 As XmlNode
    '        Dim nodeL3 As XmlNode
    '        Dim nodeL4 As XmlNode
    '        Dim nodeL5 As XmlNode
    '        Dim nodeL6 As XmlNode

    '        Dim sr As System.IO.StreamReader = File.OpenText(sFileName)
    '        xmld = New XmlDocument()
    '        Dim xmlR As XmlReader = XmlReader.Create(sr)
    '        xmld.Load(xmlR)
    '        sr.Close()

    '        nodelist = xmld.SelectNodes("/Installation")

    '        'Dim OID As Integer = 0
    '        Dim TagName As Integer = 0
    '        Dim Address As Integer = 1

    '        Dim tnodeProject As TreeNode
    '        Dim tnodeNetwork As TreeNode
    '        Dim tnodeApp As TreeNode
    '        Dim iNodeCount As Integer

    '        sActionSelectors = ""

    '        xmlView.Nodes.Clear()

    '        'Find Network Node
    '        For Each nodeL1 In nodelist
    '            For Each nodeL2 In nodeL1.ChildNodes
    '                If nodeL2.Name.ToLower = "project" Then
    '                    'Print Project Node
    '                    tnodeProject = xmlView.Nodes.Add(nodeL2.ChildNodes.Item(TagName).InnerText & " (Project)")
    '                    tnodeProject.Tag = nodeL2.ChildNodes.Item(Address).InnerText
    '                    tnodeProject.Name = "proj"
    '                    checkNode(tnodeProject)
    '                    For Each nodeL3 In nodeL2.ChildNodes
    '                        If nodeL3.Name.ToLower = "network" Then
    '                            tnodeNetwork = tnodeProject.Nodes.Add(nodeL3.ChildNodes.Item(TagName).InnerText & " (" & nodeL3.ChildNodes.Item(Address).InnerText & ")")
    '                            tnodeNetwork.Tag = nodeL3.ChildNodes.Item(Address).InnerText
    '                            tnodeNetwork.Name = "net_" & nodeL3.ChildNodes.Item(Address).InnerText
    '                            checkNode(tnodeNetwork)
    '                            For Each nodeL4 In nodeL3.ChildNodes
    '                                If nodeL4.Name.ToLower = "application" Then
    '                                    Dim nodeOrder As New SortedList(Of Integer, String)
    '                                    tnodeApp = tnodeNetwork.Nodes.Insert(CInt(nodeL4.ChildNodes.Item(Address).InnerText), nodeL4.ChildNodes.Item(TagName).InnerText & " (" & nodeL4.ChildNodes.Item(Address).InnerText & ")")
    '                                    tnodeApp.Tag = nodeL4.ChildNodes.Item(Address).InnerText
    '                                    tnodeApp.Name = "app_" & tnodeApp.Parent.Tag & "_" & nodeL4.ChildNodes.Item(Address).InnerText
    '                                    tnodeApp.EnsureVisible()
    '                                    checkNode(tnodeApp)
    '                                    iNodeCount = 0
    '                                    For Each nodeL5 In nodeL4.ChildNodes
    '                                        If nodeL5.Name.ToLower = "group" Then
    '                                            Dim tnGroup As New TreeNode
    '                                            Dim iGroup As Integer = nodeL5.ChildNodes.Item(Address).InnerText
    '                                            Dim sLocalAs = "    triggerGroup" & iGroup & ": ["
    '                                            tnGroup.Name = tnodeApp.Text & "_" & iGroup
    '                                            tnGroup.Text = nodeL5.ChildNodes.Item(TagName).InnerText & " (" & nodeL5.ChildNodes.Item(Address).InnerText & ")"
    '                                            tnGroup.Tag = nodeL5.ChildNodes.Item(TagName).InnerText & Chr(30) & iGroup
    '                                            If iGroup >= 0 And iGroup < 255 Then
    '                                                nodeOrder.Add(iGroup, tnGroup.Name)
    '                                                tnodeApp.Nodes.Insert(nodeOrder.IndexOfKey(iGroup), tnGroup)
    '                                                tnGroup.Name = "grp_" & tnGroup.Parent.Parent.Tag & "_" & tnGroup.Parent.Tag & "_" & iGroup
    '                                                checkNode(tnGroup)
    '                                                For Each nodeL6 In nodeL5.ChildNodes
    '                                                    If nodeL6.Name.ToLower = "level" Then
    '                                                        Dim tnActionSelector As TreeNode = tnGroup.Nodes.Add(nodeL6.ChildNodes.Item(TagName).InnerText & " (" & nodeL6.ChildNodes.Item(Address).InnerText & ")")
    '                                                        Dim iActionSelector As Integer = nodeL6.ChildNodes.Item(Address).InnerText
    '                                                        sLocalAs += iActionSelector & ","
    '                                                        tnActionSelector.Name = "as_" & tnActionSelector.Parent.Parent.Parent.Tag & "_" & tnActionSelector.Parent.Parent.Tag & "_" & iGroup & "_" & iActionSelector
    '                                                        tnActionSelector.Tag = nodeL6.ChildNodes.Item(TagName).InnerText & Chr(30) & iActionSelector
    '                                                        checkNode(tnActionSelector)
    '                                                    End If
    '                                                Next
    '                                                If sLocalAs.Substring(sLocalAs.Length - 1) = "," Then
    '                                                    If sActionSelectors.Length = 0 Then
    '                                                        sActionSelectors = sLocalAs.Substring(0, sLocalAs.Length - 1) & "],"
    '                                                    Else
    '                                                        sActionSelectors += vbCrLf & sLocalAs.Substring(0, sLocalAs.Length - 1) & "],"
    '                                                    End If
    '                                                End If
    '                                                iNodeCount += 1
    '                                            End If
    '                                        End If
    '                                    Next
    '                                    If iNodeCount = 0 Then tnodeApp.Remove()
    '                                End If
    '                            Next
    '                        End If
    '                    Next
    '                End If
    '            Next
    '        Next
    '        tbXmlFile.Text = sFileName
    '        xmlView.Refresh()

    '    Catch ex As Exception
    '        If bDebug Then MsgBox("ParseCBusXML: " & ex.Message)
    '    End Try
    'End Sub

    Shared Function GetCheckSum(ByVal sInput As String, Optional ByVal bAppendToInputForOutput As Boolean = True) As String
        If sInput.StartsWith("/") Then sInput = "\" & sInput.Substring(1)
        Dim sOutput = sInput
        Try
            'If Not IsNumeric(sInput) Then Return "0"
            If sInput.StartsWith("\") Then sInput = sInput.Substring(1)
            Dim iLen As Decimal = Len(sInput)
            Dim iLenDivBy2 As Decimal = iLen / 2
            If (Math.Floor(iLenDivBy2) - iLenDivBy2) < 0 Then Return "0"

            Dim iSum As Integer = 0
            For i = 0 To iLenDivBy2 - 1
                iSum += Convert.ToString("&h" & sInput.Substring((i * 2), 2))
            Next
            If bAppendToInputForOutput Then
                Return sOutput & Hex((Not (iSum Mod 256)) + 1).Substring(6, 2)
            Else
                Return Hex((Not (iSum Mod 256)) + 1).Substring(6, 2)
            End If
        Catch
            Return sOutput
        End Try
    End Function
#End Region

#Region " CBus Treeview Management "
    Sub xmlView_AfterCheck(ByVal sender As Object, ByVal e As System.EventArgs) Handles xmlView.AfterCheck
        If bIsProjectSelected Then timerSaveConfig.Enabled = True
    End Sub

    Private Sub timerSaveConfig_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerSaveConfig.Tick
        timerSaveConfig.Enabled = False
        For i As Integer = 0 To 1
            xmlNodeSelected(i) = ","
        Next
        saveCheckedItems(xmlView.Nodes(0))
        configSet(_XML_xmlNodeChecked, xmlNodeSelected(_CHECKED))
        configSet(_XML_xmlNodeMixed, xmlNodeSelected(_MIXED))
    End Sub

    Sub saveCheckedItems(ByVal node As TreeNode)
        If node.StateImageIndex = 2 And node.Checked Then
            xmlNodeSelected(_MIXED) += node.Name & ","
        ElseIf node.Checked Then
            xmlNodeSelected(_CHECKED) += node.Name & ","
        End If

        If node.Nodes.Count = 0 Then Exit Sub
        For Each childNode As TreeNode In node.Nodes
            saveCheckedItems(childNode)
        Next
    End Sub

    Sub checkNode(ByVal node As TreeNode)
        Try
            If xmlNodeSelected(_CHECKED).Contains("," & node.Name & ",") Then
                node.Checked = True
                node.StateImageIndex = 1
            ElseIf xmlNodeSelected(_MIXED).Contains("," & node.Name & ",") Then
                node.Checked = True
                node.StateImageIndex = 2
            Else
                node.Checked = False
                node.StateImageIndex = 0
            End If
        Catch ex As Exception
            If bDebug Then MsgBox("checkNode: " & ex.Message)
        End Try
    End Sub

    Private Sub cbusUncheckAllNodes()
        If xmlView.Nodes.Count > 0 Then cbusUncheckNode(xmlView.Nodes(0))
    End Sub

    Private Sub cbusUncheckNode(ByVal node As TreeNode)
        node.Checked = False
        If node.Nodes.Count = 0 Then Exit Sub
        For Each childNode In node.Nodes
            cbusUncheckNode(childNode)
        Next
    End Sub
#End Region

#Region " Utility Functions"
    Shared Function dec2hexPair(ByVal sDec As String) As String
        Dim iDec As Integer = CInt(sDec)
        Dim sRetVal As String = Hex(iDec)
        If Len(sRetVal) = 1 Then Return "0" & sRetVal Else Return sRetVal
    End Function

    Public Shared Function Hex2Dec(ByVal sHex As String) As Integer
        If sHex = "ERROR" Then Return -1
        If Not sHex.Contains("&") Then sHex = "&h" & sHex
        Return Convert.ToString(sHex)
    End Function

    Public Function MD5CalcString(ByVal strData As String) As String
        Dim objMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim arrData() As Byte
        Dim arrHash() As Byte

        arrData = System.Text.Encoding.UTF8.GetBytes(strData)
        arrHash = objMD5.ComputeHash(arrData)
        objMD5 = Nothing

        Dim strOutput As New System.Text.StringBuilder(arrHash.Length)
        For i As Integer = 0 To arrHash.Length - 1
            strOutput.Append(arrHash(i).ToString("X2"))
        Next
        Return strOutput.ToString().ToLower
    End Function

    Function alz(ByVal sIn As String, Optional ByVal iTotalChars As Integer = 2)
        'Add Leading Zeroes
        Dim sRetVal As String = sIn
        If Len(sIn) < iTotalChars Then
            For i = 1 To iTotalChars - Len(sIn)
                sRetVal = "0" & sRetVal
            Next
        End If
        Return sRetVal
    End Function

    Function FixString(ByVal sIn As String) As String
        sIn = sIn
        Return sIn
    End Function
#End Region

End Class