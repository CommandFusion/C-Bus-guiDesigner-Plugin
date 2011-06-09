Public NotInheritable Class aboutBox

    Private Sub AboutBox1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "About CBus Plugin"
        Me.LabelProductName.Text = "CommandFusion CBus Plugin"
        Me.LabelVersion.Text = "v1.1"
        Me.LabelCopyright.Text = "Distribute Freely"
        Me.LabelCompanyName.Text = "Original Source by Ben Nuttall (ben@nuttall.co.nz)"
        Me.TextBoxDescription.Text = "This addin creates a myriad of commands and feedback items based on a physical CBus network." & vbCrLf & vbCrLf & "A JavaScript file is also deployed enabling advanced feedback processing and command generation."
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

    Private Sub LabelProductName_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelProductName.Click

    End Sub
End Class
