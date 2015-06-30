Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Threading
Imports MySql.Data.MySqlClient
Imports Microsoft.DirectX.Direct3D
Imports Microsoft.DirectX

Namespace Game
    Namespace Multiplayer
        Public Class TCPNetwork 'Klasse zum Aufbau eines Netzwerkes :D
            Private IPAdresse As IPAddress
            Private ReceivePort As Integer = 0
            Private SendPort As Integer = 0
            Private ListenThread As New Thread(New ThreadStart(AddressOf Listen))
            Dim Listener As TcpListener
            Dim Client As TcpClient
            Dim Reader As StreamReader
            'IP und Port(PortI[Intern]) werden belegt


            Public Event NachrichtAlarm(Nachricht As String)

            Sub New(ByVal GPort As Integer, ByVal SPort As Integer, ByVal Address As IPAddress)
                ReceivePort = GPort
                SendPort = SPort
                IPAdresse = Address
            End Sub

            Public ReadOnly Property IP
                Get
                    Dim Addresslist() As IPAddress = Dns.GetHostEntry(IPAddress.Parse("127.0.0.1")).AddressList
                    Return Addresslist
                End Get
            End Property

            Public ReadOnly Property TCPReceivePort
                Get
                    Return ReceivePort
                End Get
            End Property

            Public ReadOnly Property TCPSendPort
                Get
                    Return SendPort
                End Get
            End Property

            Private Sub Listen()
                Do
                    If Listener.Pending = True Then
                        Client = Listener.AcceptTcpClient
                        Reader = New StreamReader(Client.GetStream)
                    End If

                    RaiseEvent NachrichtAlarm(Reader.ReadLine)
                Loop
            End Sub

            Public Sub SendMessage(ByVal Message As String) ' Sendemodul
                Try
                    Client = New TcpClient(IPAdresse.ToString, SendPort)
                    Dim StreamS As New StreamWriter(Client.GetStream) 'Schreiberling xD

                    StreamS.Write(Message)
                    StreamS.Flush()
                Catch ex As Exception

                End Try
            End Sub

            Public Sub Close()
                ListenThread.Abort()
                Client.Close()
            End Sub

            Public Sub Initialize()
                Listener = New TcpListener(IPAdresse, ReceivePort)
                Listener.Start()

                ListenThread.Start()
            End Sub
        End Class

        Public Class MySQLManagement
            'Klasse für MySQL

            Dim Connection As MySqlConnection
            Dim Adapter As New MySqlDataAdapter
            Dim Command As New MySqlCommand
            Dim DataReader As MySqlDataReader

            Dim ConnString As String

            Sub New(ByVal ConnectionString As String)
                ConnString = ConnectionString
            End Sub

            Public Shared Function Parse(ByVal Server As String, ByVal User As String, ByVal Password As String, _
                                         ByVal DataBase As String) As String
                Return String.Format("server={0};user id={1};password={2};database={3}", Server, User, Password, DataBase)
            End Function

            Private Function Open(ByVal ConnectionString As String) As Boolean
                Try
                    Connection = New MySqlConnection
                    Connection.ConnectionString = ConnectionString
                    Connection.Open()

                    Return True
                Catch ex As Exception
                    Return False
                End Try
            End Function

            Private Function ExecuteCommand(ByVal CommandToExecute As String) As String
                If Open(ConnString) = True Then
                    Command.Connection = Connection
                    Command.CommandText = CommandToExecute
                    Adapter.SelectCommand = Command
                    DataReader = Command.ExecuteReader()

                    Return DataReader.Read()

                    DataReader.Close()
                    Connection.Close()
                Else
                    Return "ERROR!"
                End If
            End Function

            Public Function GetValue(ByVal Table As String, ByVal WantedValues As String, _
                                     ByVal WhereSlot As String, ByVal WhereValue As String) As String
                Return ExecuteCommand(String.Format("select {0} from {1} where {2} = {3}", WantedValues, Table, WhereSlot, WhereValue))
            End Function

            Public Sub AddValue(ByVal Table As String, ByVal ValuesToAdd As String, ByVal Values As String)
                ExecuteCommand(String.Format("insert into {1} ({2}) values ({3})", Table, ValuesToAdd, Values))
            End Sub

            Public Sub DeleteValue(ByVal Table As String, ByVal WhereSlot As String, ByVal WhereValue As String)
                ExecuteCommand(String.Format("delete from {1} where {2} = {3}", Table, WhereSlot, WhereValue))
            End Sub

            Public Sub UpdateValue(ByVal Table As String, ByVal ValueToSet As String, ByVal Value As String, _
                                   ByVal WhereSlot As String, ByVal WhereValue As String)
                ExecuteCommand(String.Format("update {1} set {2} = {3} where {4} = {5}", Table, ValueToSet, Value, WhereSlot, WhereValue))
            End Sub
        End Class
    End Namespace
End Namespace