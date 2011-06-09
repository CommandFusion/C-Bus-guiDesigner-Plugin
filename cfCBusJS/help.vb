Public Class help
    Const _DEFAULTHTML = "<html><body><font face='arial' size=2>Please select what you're trying to do from the combo box above.</font></body></html>"

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub help_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        mainPane.DocumentText = _DEFAULTHTML
        cboActions.Items.Clear()
        cboActions.Items.Add("a slider to ramp a group")
        cboActions.Items.Add("a button to toggle a group")
        'cboActions.Items.Add("up/down buttons to adjust a group")
    End Sub

    Private Sub cboActions_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboActions.SelectedIndexChanged
        Select Case cboActions.SelectedIndex
            Case 0
                mainPane.DocumentText = My.Resources.help_slider
            Case 1
                mainPane.DocumentText = My.Resources.help_toggle
            Case Else
                mainPane.DocumentText = _DEFAULTHTML
        End Select
    End Sub
End Class