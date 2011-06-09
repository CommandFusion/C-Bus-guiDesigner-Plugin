Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles

Namespace CustomControls

    ''' <summary>
    ''' Provides a tree view
    ''' control supporting
    ''' tri-state checkboxes.
    ''' </summary>
    Public Class TriStateTreeView
        Inherits TreeView

        ' ~~~ fields ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Private _ilStateImages As ImageList
        Private _bUseTriState As Boolean
        Private _bCheckBoxesVisible As Boolean
        Private _bPreventCheckEvent As Boolean
        ' ~~~ constructor ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        ''' <summary>
        ''' Creates a new instance
        ''' of this control.
        ''' </summary>
        Public Sub New()
            MyBase.New()
            Dim cbsState As CheckBoxState
            Dim gfxCheckBox As Graphics
            Dim bmpCheckBox As Bitmap

            _ilStateImages = New ImageList()
            ' first we create our state image
            cbsState = CheckBoxState.UncheckedNormal
            ' list and pre-init check state.
            For i As Integer = 0 To 2
                ' let's iterate each tri-state
                bmpCheckBox = New Bitmap(16, 16)
                ' creating a new checkbox bitmap
                gfxCheckBox = Graphics.FromImage(bmpCheckBox)
                ' and getting graphics object from
                Select Case i
                    ' it...
                    Case 0
                        cbsState = CheckBoxState.UncheckedNormal
                        Exit Select
                    Case 1
                        cbsState = CheckBoxState.CheckedNormal
                        Exit Select
                    Case 2
                        cbsState = CheckBoxState.MixedNormal
                        Exit Select
                End Select
                CheckBoxRenderer.DrawCheckBox(gfxCheckBox, New Point(2, 2), cbsState)
                ' ...rendering the checkbox and...
                gfxCheckBox.Save()
                _ilStateImages.Images.Add(bmpCheckBox)
                ' ...adding to sate image list.
                _bUseTriState = True
            Next
        End Sub

        ' ~~~ properties ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        ''' <summary>
        ''' Gets or sets to display
        ''' checkboxes in the tree
        ''' view.
        ''' </summary>
        <Category("Appearance")> _
        <Description("Sets tree view to display checkboxes or not.")> _
        <DefaultValue(False)> _
        Public Shadows Property CheckBoxes() As Boolean
            Get
                Return _bCheckBoxesVisible
            End Get
            Set(ByVal value As Boolean)
                _bCheckBoxesVisible = value
                MyBase.CheckBoxes = _bCheckBoxesVisible
                Me.StateImageList = If(_bCheckBoxesVisible, _ilStateImages, Nothing)
            End Set
        End Property

        <Browsable(False)> _
        Public Shadows Property StateImageList() As ImageList
            Get
                Return MyBase.StateImageList
            End Get
            Set(ByVal value As ImageList)
                MyBase.StateImageList = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets to support
        ''' tri-state in the checkboxes
        ''' or not.
        ''' </summary>
        <Category("Appearance")> _
        <Description("Sets tree view to use tri-state checkboxes or not.")> _
        <DefaultValue(True)> _
        Public Property CheckBoxesTriState() As Boolean
            Get
                Return _bUseTriState
            End Get
            Set(ByVal value As Boolean)
                _bUseTriState = value
            End Set
        End Property

        ' ~~~ functions ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        ''' <summary>
        ''' Refreshes this
        ''' control.
        ''' </summary>
        Public Overrides Sub Refresh()
            Dim stNodes As Stack(Of TreeNode)
            Dim tnStacked As TreeNode

            MyBase.Refresh()

            If Not CheckBoxes Then
                ' nothing to do here if
                Return
            End If
            ' checkboxes are hidden.
            MyBase.CheckBoxes = False
            ' hide normal checkboxes...
            stNodes = New Stack(Of TreeNode)(Me.Nodes.Count)
            ' create a new stack and
            For Each tnCurrent As TreeNode In Me.Nodes
                ' push each root node.
                stNodes.Push(tnCurrent)
            Next

            While stNodes.Count > 0
                ' let's pop node from stack,
                tnStacked = stNodes.Pop()
                ' set correct state image
                If tnStacked.StateImageIndex = -1 Then
                    ' index if not already done
                    tnStacked.StateImageIndex = If(tnStacked.Checked, 1, 0)
                End If
                ' and push each child to stack
                For i As Integer = 0 To tnStacked.Nodes.Count - 1
                    ' too until there are no
                    stNodes.Push(tnStacked.Nodes(i))
                    ' nodes left on stack.
                Next
            End While
        End Sub

        Protected Overrides Sub OnLayout(ByVal levent As LayoutEventArgs)
            MyBase.OnLayout(levent)

            Refresh()
        End Sub

        Protected Overrides Sub OnAfterExpand(ByVal e As TreeViewEventArgs)
            MyBase.OnAfterExpand(e)

            For Each tnCurrent As TreeNode In e.Node.Nodes
                ' set tree state image
                If tnCurrent.StateImageIndex = -1 Then
                    ' to each child node...
                    tnCurrent.StateImageIndex = If(tnCurrent.Checked, 1, 0)
                End If
            Next
        End Sub

        Protected Overrides Sub OnAfterCheck(ByVal e As TreeViewEventArgs)
            MyBase.OnAfterCheck(e)

            If _bPreventCheckEvent Then
                Return
            End If

            OnNodeMouseClick(New TreeNodeMouseClickEventArgs(e.Node, MouseButtons.None, 0, 0, 0))
        End Sub

        Protected Overrides Sub OnNodeMouseClick(ByVal e As TreeNodeMouseClickEventArgs)
            Dim stNodes As Stack(Of TreeNode)
            Dim tnBuffer As TreeNode
            Dim bMixedState As Boolean
            Dim iSpacing As Integer
            Dim iIndex As Integer

            MyBase.OnNodeMouseClick(e)

            _bPreventCheckEvent = True

            iSpacing = If(ImageList Is Nothing, 0, 18)
            ' if user clicked area
            ' *not* used by the state
            ' image we can leave here.
            If (e.X > e.Node.Bounds.Left - iSpacing OrElse e.X < e.Node.Bounds.Left - (iSpacing + 16)) AndAlso e.Button <> MouseButtons.None Then
                Return
            End If

            tnBuffer = e.Node
            ' buffer clicked node and
            If e.Button = MouseButtons.Left Then
                ' flip its check state.
                tnBuffer.Checked = Not tnBuffer.Checked
            End If

            ' set state image index
            tnBuffer.StateImageIndex = If(tnBuffer.Checked, 1, tnBuffer.StateImageIndex)
            ' correctly.
            OnAfterCheck(New TreeViewEventArgs(tnBuffer, TreeViewAction.ByMouse))

            stNodes = New Stack(Of TreeNode)(tnBuffer.Nodes.Count)
            ' create a new stack and
            stNodes.Push(tnBuffer)
            ' push buffered node first.
            Do
                ' let's pop node from stack,
                tnBuffer = stNodes.Pop()
                ' inherit buffered node's
                tnBuffer.Checked = e.Node.Checked
                ' check state and push
                For i As Integer = 0 To tnBuffer.Nodes.Count - 1
                    ' each child on the stack
                    stNodes.Push(tnBuffer.Nodes(i))
                    ' until there is no node
                Next
            Loop While stNodes.Count > 0
            ' left.
            bMixedState = False
            tnBuffer = e.Node
            ' re-buffer clicked node.
            While tnBuffer.Parent IsNot Nothing
                ' while we get a parent we
                For Each tnChild As TreeNode In tnBuffer.Parent.Nodes
                    ' determine mixed check states
                    ' and convert current check
                    bMixedState = bMixedState Or (tnChild.Checked <> tnBuffer.Checked Or tnChild.StateImageIndex = 2)
                Next
                ' state to state image index.
                iIndex = CInt(Convert.ToUInt32(tnBuffer.Checked))
                ' set parent's check state and
                tnBuffer.Parent.Checked = bMixedState OrElse (iIndex > 0)
                ' state image in dependency
                If bMixedState Then
                    ' of mixed state.
                    tnBuffer.Parent.StateImageIndex = If(CheckBoxesTriState, 2, 1)
                Else
                    tnBuffer.Parent.StateImageIndex = iIndex
                End If
                ' finally buffer parent and
                tnBuffer = tnBuffer.Parent
            End While
            ' loop here.
            _bPreventCheckEvent = False
        End Sub
    End Class
End Namespace

