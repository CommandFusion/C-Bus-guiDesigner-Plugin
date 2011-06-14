Imports System.Collections.Generic
Imports System.Text
Imports System.Xml

''' <summary>
''' The class which represents a configuration xml file
''' </summary>
Public Class Xmlconfig
    Implements IDisposable
    Private xmldoc As XmlDocument
    Private originalFile As String
    Private m_commitonunload As Boolean = True
    Private m_cleanuponsave As Boolean = False


    ''' <summary>
    ''' Create an XmlConfig from an empty xml file 
    ''' containing only the rootelement named as 'xml'
    ''' </summary>
    Public Sub New()
        xmldoc = New XmlDocument()
        LoadXmlFromString("<xml></xml>")
    End Sub

    ''' <summary>
    ''' Create an XmlConfig from an existing file, or create a new one
    ''' </summary>
    ''' <param name="loadfromfile">
    ''' Path and filename from where to load the xml file
    ''' </param>
    ''' <param name="create">
    ''' If file does not exist, create it, or throw an exception?
    ''' </param>
    Public Sub New(ByVal loadfromfile As String, ByVal create As Boolean)
        xmldoc = New XmlDocument()
        LoadXmlFromFile(loadfromfile, create)
    End Sub

    ''' <summary>
    ''' Check XML file if it conforms the config xml restrictions
    ''' 1. No nodes with two children of the same name
    ''' 2. Only alphanumerical names
    ''' </summary>
    ''' <param name="silent">
    ''' Whether to return a true/false value, or throw an exception on failiure
    ''' </param>
    ''' <returns>
    ''' True on success and in case of silent mode false on failiure
    ''' </returns>
    Public Function ValidateXML(ByVal silent As Boolean) As Boolean
        If Not Settings.Validate() Then
            If silent Then
                Return False
            Else
                Throw New Exception("This is not a valid configuration xml file! Probably duplicate children with the same names, or non-alphanumerical tagnames!")
            End If
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Strip empty nodes from the whole tree.
    ''' </summary>
    Public Sub Clean()
        Settings.Clean()
    End Sub

    ''' <summary>
    ''' Whether to clean the tree by stripping out
    ''' empty nodes before saving the XML config
    ''' file
    ''' </summary>
    Public Property CleanUpOnSave() As Boolean
        Get
            Return m_cleanuponsave
        End Get
        Set(ByVal value As Boolean)
            m_cleanuponsave = value
        End Set
    End Property


    ''' <summary>
    ''' When unloading the current XML config file
    ''' shold any changes be saved back to the file?
    ''' </summary>
    ''' <remarks>
    ''' <list type="bullet">
    ''' <item>Only applies if it was loaded from a local file</item>
    ''' <item>True by default</item>
    ''' </list>
    ''' </remarks>
    Public Property CommitOnUnload() As Boolean
        Get
            Return m_commitonunload
        End Get
        Set(ByVal value As Boolean)
            m_commitonunload = value
        End Set
    End Property

    ''' <summary>
    ''' Save any modifications to the XML file before destruction
    ''' if CommitOnUnload is true
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        If CommitOnUnload Then
            Commit()
        End If
    End Sub

    ''' <summary>
    ''' Load a new XmlDocument from a file
    ''' </summary>
    ''' <param name="filename">
    ''' Path and filename from where to load the xml file
    ''' </param>
    ''' <param name="create">
    ''' If file does not exist, create it, or throw an exception?
    ''' </param>
    Public Sub LoadXmlFromFile(ByVal filename As String, ByVal create As Boolean)
        If CommitOnUnload Then
            Commit()
        End If
        Try
            xmldoc.Load(filename)
        Catch
            If Not create Then
                Throw New Exception("xmldoc.Load() failed! Probably file does NOT exist!")
            Else
                xmldoc.LoadXml("<xml></xml>")
                Save(filename)
            End If
        End Try
        ValidateXML(False)
        originalFile = filename

    End Sub

    ''' <summary>
    ''' Load a new XmlDocument from a file
    ''' </summary>
    ''' <param name="filename">
    ''' Path and filename from where to load the xml file
    ''' </param>
    ''' <remarks>
    ''' Throws an exception if file does not exist
    ''' </remarks>
    Public Sub LoadXmlFromFile(ByVal filename As String)
        LoadXmlFromFile(filename, False)
    End Sub

    ''' <summary>
    ''' Load a new XmlDocument from a string
    ''' </summary>
    ''' <param name="xml">
    ''' XML string
    ''' </param>
    Public Sub LoadXmlFromString(ByVal xml As String)
        If CommitOnUnload Then
            Commit()
        End If
        xmldoc.LoadXml(xml)
        originalFile = Nothing
        ValidateXML(False)
    End Sub

    ''' <summary>
    ''' Load an empty XmlDocument
    ''' </summary>
    ''' <param name="rootelement">
    ''' Name of root element
    ''' </param>
    Public Sub NewXml(ByVal rootelement As String)
        If CommitOnUnload Then
            Commit()
        End If
        LoadXmlFromString([String].Format("<{0}></{0}>", rootelement))
    End Sub

    ''' <summary>
    ''' Save configuration to an xml file
    ''' </summary>
    ''' <param name="filename">
    ''' Path and filname where to save
    ''' </param>
    Public Sub Save(ByVal filename As String)
        ValidateXML(False)
        If CleanUpOnSave Then
            Clean()
        End If
        xmldoc.Save(filename)
        originalFile = filename
    End Sub

    ''' <summary>
    ''' Save configuration to a stream
    ''' </summary>
    ''' <param name="stream">
    ''' Stream where to save
    ''' </param>
    Public Sub Save(ByVal stream As System.IO.Stream)
        ValidateXML(False)
        If CleanUpOnSave Then
            Clean()
        End If
        xmldoc.Save(stream)
    End Sub

    ''' <summary>
    ''' If loaded from a file, commit any changes, by overwriting the file
    ''' </summary>
    ''' <returns>
    ''' True on success
    ''' False on failiure, probably due to the file was not loaded from a file
    ''' </returns>

    Public Function Commit() As Boolean
        If originalFile IsNot Nothing Then
            Save(originalFile)
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' If loaded from a file, trash any changes, and reload the file
    ''' </summary>
    ''' <returns>
    ''' True on success
    ''' False on failiure, probably due to file was not loaded from a file
    ''' </returns>
    Public Function Reload() As Boolean
        If originalFile IsNot Nothing Then
            LoadXmlFromFile(originalFile)
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Gets the root ConfigSetting
    ''' </summary>
    Public ReadOnly Property Settings() As ConfigSetting
        Get
            Return New ConfigSetting(xmldoc.DocumentElement)
        End Get
    End Property

End Class