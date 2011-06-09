Imports System.Collections.Generic
Imports System.Text
Imports System.Xml

''' <summary>
''' Represents a Configuration Node in the XML file
''' </summary>
Public Class ConfigSetting
    ''' <summary>
    ''' The node from the XMLDocument, which it describes
    ''' </summary>
    Private node As XmlNode

    ''' <summary>
    ''' This class cannot be constructed directly. You will need to give a node to describe
    ''' </summary>
    Private Sub New()
        Throw New Exception("Cannot be created directly. Needs a node parameter")
    End Sub

    ''' <summary>
    ''' Creates an instance of the class
    ''' </summary>
    ''' <param name="node">
    ''' the XmlNode to describe
    ''' </param>
    Public Sub New(ByVal node As XmlNode)
        If node Is Nothing Then
            Throw New Exception("Node parameter can NOT be null!")
        End If
        Me.node = node
    End Sub

    ''' <summary>
    ''' The Name of the element it describes
    ''' </summary>
    ''' <remarks>Read only property</remarks>        
    Public ReadOnly Property Name() As String
        Get
            Return node.Name
        End Get
    End Property

    ''' <summary>
    ''' Gets the number of children of the specific node
    ''' </summary>
    ''' <param name="unique">
    ''' If true, get only the number of children with distinct names.
    ''' So if it has two nodes with "foo" name, and three nodes
    ''' named "bar", the return value will be 2. In the same case, if unique
    ''' was false, the return value would have been 2 + 3 = 5
    ''' </param>
    ''' <returns>
    ''' The number of (uniquely named) children
    ''' </returns>
    Public Function ChildCount(ByVal unique As Boolean) As Integer
        '#Warning Code in ChildCoutn(bool) is NOT optimised. If you can help me out with a better algorithm selecting unique child names (probably using XPath), please contact me at axos88@gmail.com Thanks!
        Dim names As IList(Of String) = ChildrenNames(unique)
        If names IsNot Nothing Then
            Return names.Count
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' Gets the names of children of the specific node
    ''' </summary>
    ''' <param name="unique">
    ''' If true, get only distinct names.
    ''' So if it has two nodes with "foo" name, and three nodes
    ''' named "bar", the return value will be {"bar","foo"} . 
    ''' In the same case, if unique was false, the return value 
    ''' would have been {"bar","bar","bar","foo","foo"}
    ''' </param>
    ''' <returns>
    ''' An IList object with the names of (uniquely named) children
    ''' </returns>

    Public Function ChildrenNames(ByVal unique As Boolean) As IList(Of [String])
        '#Warning Code in ChildrenNames(bool) is NOT optimised. If you can help me out with a better algorithm selecting unique child names (probably using XPath), please contact me at axos88@gmail.com Thanks!

        If node.ChildNodes.Count = 0 Then
            Return Nothing
        End If
        Dim stringlist As List(Of [String]) = New List(Of String)()

        For Each achild As XmlNode In node.ChildNodes
            Dim name As String = achild.Name
            If (Not unique) OrElse (Not stringlist.Contains(name)) Then
                stringlist.Add(name)
            End If
        Next

        stringlist.Sort()
        Return stringlist
    End Function



    ''' <summary>
    ''' An IList compatible object describing each and every child node
    ''' </summary>
    ''' <remarks>Read only property</remarks>
    Public Function Children() As IList(Of ConfigSetting)
        If ChildCount(False) = 0 Then
            Return Nothing
        End If
        Dim list As New List(Of ConfigSetting)()

        For Each achild As XmlNode In node.ChildNodes
            list.Add(New ConfigSetting(achild))
        Next
        Return list
    End Function
    ''' <summary>
    ''' Get all children with the same name, specified in the name parameter
    ''' </summary>
    ''' <param name="name">
    ''' An alphanumerical string, containing the name of the child nodes to return
    ''' </param>
    ''' <returns>
    ''' An array with the child nodes with the specified name, or null 
    ''' if no childs with the specified name exist
    ''' </returns>
    Public Function GetNamedChildren(ByVal name As [String]) As IList(Of ConfigSetting)
        For Each c As [Char] In name
            If Not [Char].IsLetterOrDigit(c) Then
                Throw New Exception("Name MUST be alphanumerical!")
            End If
        Next
        Dim xmlnl As XmlNodeList = node.SelectNodes(name)
        Dim NodeCount As Integer = xmlnl.Count
        Dim css As New List(Of ConfigSetting)()
        For Each achild As XmlNode In xmlnl
            css.Add(New ConfigSetting(achild))
        Next
        Return css
    End Function

    ''' <summary>
    ''' Gets the number of childs with the specified name
    ''' </summary>
    ''' <param name="name">
    ''' An alphanumerical string with the name of the nodes to look for
    ''' </param>
    ''' <returns>
    ''' An integer with the count of the nodes
    ''' </returns>
    Public Function GetNamedChildrenCount(ByVal name As [String]) As Integer
        For Each c As [Char] In name
            If Not [Char].IsLetterOrDigit(c) Then
                Throw New Exception("Name MUST be alphanumerical!")
            End If
        Next
        Return node.SelectNodes(name).Count
    End Function

    ''' <summary>
    ''' String value of the specific Configuration Node
    ''' </summary>
    Public Property Value() As String
        Get
            Dim xmlattrib As XmlNode = node.Attributes.GetNamedItem("value")
            If xmlattrib IsNot Nothing Then
                Return xmlattrib.Value
            Else
                Return ""
            End If
        End Get

        Set(ByVal value As String)
            Dim xmlattrib As XmlNode = node.Attributes.GetNamedItem("value")
            If value <> "" Then
                If xmlattrib Is Nothing Then
                    xmlattrib = node.Attributes.Append(node.OwnerDocument.CreateAttribute("value"))
                End If
                xmlattrib.Value = value
            ElseIf xmlattrib IsNot Nothing Then
                node.Attributes.RemoveNamedItem("value")
            End If
        End Set
    End Property

    ''' <summary>
    ''' int value of the specific Configuration Node
    ''' </summary>
    Public Property intValue() As Integer
        Get
            Dim i As Integer
            Integer.TryParse(Value, i)
            Return i
        End Get
        Set(ByVal value As Integer)
            value = value.ToString()
        End Set
    End Property

    ''' <summary>
    ''' bool value of the specific Configuration Node
    ''' </summary>
    Public Property boolValue() As Boolean
        Get
            Dim b As Boolean
            Boolean.TryParse(Value, b)
            Return b
        End Get
        Set(ByVal value As Boolean)
            value = value.ToString()
        End Set
    End Property

    ''' <summary>
    ''' float value of the specific Configuration Node
    ''' </summary>
    Public Property floatValue() As Single
        Get
            Dim f As Single
            Single.TryParse(Value, f)
            Return f
        End Get
        Set(ByVal value As Single)
            value = value.ToString()
        End Set
    End Property


    ''' <summary>
    ''' Get a specific child node
    ''' </summary>
    ''' <param name="path">
    ''' The path to the specific node. Can be either only a name, or a full path separated by '/' or '\'
    ''' </param>
    ''' <example>
    ''' <code>
    ''' XmlConfig conf = new XmlConfig("configuration.xml");
    ''' screenname = conf.Settings["screen"].Value;
    ''' height = conf.Settings["screen/height"].IntValue;
    '''  // OR
    ''' height = conf.Settings["screen"]["height"].IntValue;
    ''' </code>
    ''' </example>
    ''' <returns>
    ''' The specific child node
    ''' </returns>
    Default Public ReadOnly Property Item(ByVal path As String) As ConfigSetting
        Get
            Dim separators As Char() = {"/"c, "\"c}
            path.Trim(separators)
            Dim pathsection As [String]() = path.Split(separators)

            Dim selectednode As XmlNode = node
            Dim newnode As XmlNode

            For Each asection As String In pathsection
                Dim nodename As [String], nodeposstr As [String]
                Dim nodeposition As Integer
                Dim indexofdiez As Integer = asection.IndexOf("#"c)

                If indexofdiez = -1 Then
                    ' No position defined, take the first one by default
                    nodename = asection
                    nodeposition = 1
                Else
                    nodename = asection.Substring(0, indexofdiez)
                    ' Node name is before the diez character
                    nodeposstr = asection.Substring(indexofdiez + 1)
                    If nodeposstr = "#" Then
                        ' Double diez means he wants to create a new node
                        nodeposition = GetNamedChildrenCount(nodename) + 1
                    Else
                        nodeposition = Integer.Parse(nodeposstr)
                    End If
                End If

                ' Verify name
                For Each c As Char In nodename
                    If (Not [Char].IsLetterOrDigit(c)) Then
                        Return Nothing
                    End If
                Next

                Dim transformedpath As [String] = [String].Format("{0}[{1}]", nodename, nodeposition)
                newnode = selectednode.SelectSingleNode(transformedpath)

                While newnode Is Nothing
                    Dim newelement As XmlElement = selectednode.OwnerDocument.CreateElement(nodename)
                    selectednode.AppendChild(newelement)
                    newnode = selectednode.SelectSingleNode(transformedpath)
                End While
                selectednode = newnode
            Next

            Return New ConfigSetting(selectednode)
        End Get
    End Property

    ''' <summary>
    ''' Check if the node conforms with the config xml restrictions
    ''' 1. No nodes with two children of the same name
    ''' 2. Only alphanumerical names
    ''' </summary>
    ''' <returns>
    ''' True on success and false on failiure
    ''' </returns>        
    Public Function Validate() As Boolean
        ' Check this node's name for validity
        For Each c As [Char] In Me.Name
            If Not [Char].IsLetterOrDigit(c) Then
                Return False
            End If
        Next

        ' If there are no children, the node is valid.
        ' If there the node has other children, check all of them for validity
        If ChildCount(False) = 0 Then
            Return True
        Else
            For Each cs As ConfigSetting In Me.Children()
                If Not cs.Validate() Then
                    Return False
                End If
            Next
        End If
        Return True
    End Function

    ''' <summary>
    ''' Removes any empty nodes from the tree, 
    ''' that is it removes a node, if it hasn't got any
    ''' children, or neither of its children have got a value.
    ''' </summary>
    Public Sub Clean()
        If ChildCount(False) <> 0 Then
            For Each cs As ConfigSetting In Me.Children()
                cs.Clean()
            Next
        End If
        If (ChildCount(False) = 0) AndAlso (Me.Value = "") Then
            Me.Remove()
        End If
    End Sub

    ''' <summary>
    ''' Remove the specific node from the tree
    ''' </summary>
    Public Sub Remove()
        If node.ParentNode Is Nothing Then
            Return
        End If
        node.ParentNode.RemoveChild(node)
    End Sub

    ''' <summary>
    ''' Remove all children of the node, but keep the node itself
    ''' </summary>
    Public Sub RemoveChildren()
        node.RemoveAll()
    End Sub


End Class
