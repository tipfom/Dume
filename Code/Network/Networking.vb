Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.IO

Namespace Global.Game
    Namespace Network
        Public Class SimpleUDPClient
            Dim WithEvents Receiver As Specific.SimpleUDPReceiver
            Dim Sender As Specific.SimpleUDPSender

            Public Event MessageReceived(ByVal Sender As IPEndPoint, ByVal Message As String)

            Sub New(ByVal OpponentIPAddress As IPAddress, ByVal Port As Integer)
                Receiver = New Specific.SimpleUDPReceiver(Port)
                Sender = New Specific.SimpleUDPSender(OpponentIPAddress, Port)
            End Sub

            Public Sub Initialize()
                Receiver.Initialize()
                AddHandler Receiver.MessageReceived, AddressOf EventRaise
            End Sub

            Public Sub SendMessage(ByVal Message As String)
                Sender.SendMessage(Message)
            End Sub

            Private Sub EventRaise(ByVal Sender As IPEndPoint, ByVal Message As String)
                RaiseEvent MessageReceived(Sender, Message)
            End Sub
        End Class

        Public Class SimpleTCPClient
            Dim WithEvents Receiver As Specific.SimpleTCPReceiver
            Dim Sender As Specific.SimpleTCPSender

            Public Event MessageReceived(ByVal Sender As IPAddress, ByVal Message As String)

            Sub New(ByVal OpponentIPAddress As IPAddress, ByVal Port As Integer)
                Receiver = New Specific.SimpleTCPReceiver(OpponentIPAddress, Port)
                Sender = New Specific.SimpleTCPSender(OpponentIPAddress, Port)
            End Sub

            Public Sub Initialize()
                Receiver.Initialize()
                AddHandler Receiver.MessageReceived, AddressOf EventRaise
            End Sub

            Public Sub SendMessage(ByVal Message As String)
                Sender.SendMessage(Message)
            End Sub

            Private Sub EventRaise(ByVal Sender As IPAddress, ByVal Message As String)
                RaiseEvent MessageReceived(Sender, Message)
            End Sub
        End Class

        Public Class ComplexUDPClient
            Dim WithEvents Receiver As Specific.SimpleUDPReceiver
            Dim Sender As Specific.SimpleUDPSender

            Public Event MessageReceived(ByVal Sender As IPEndPoint, ByVal Message As String)

            Sub New(ByVal OpponentIPAddress As IPAddress, ByVal ReceivingPort As Integer, ByVal SendingPort As Integer)
                Receiver = New Specific.SimpleUDPReceiver(ReceivingPort)
                Sender = New Specific.SimpleUDPSender(OpponentIPAddress, SendingPort)
            End Sub

            Public Sub Initialize()
                Receiver.Initialize()
                AddHandler Receiver.MessageReceived, AddressOf EventRaise
            End Sub

            Public Sub SendMessage(ByVal Message As String)
                Sender.SendMessage(Message)
            End Sub

            Private Sub EventRaise(ByVal Sender As IPEndPoint, ByVal Message As String)
                RaiseEvent MessageReceived(Sender, Message)
            End Sub

            Public Sub Close()
                Sender.CloseConnections()
                Receiver.CloseConnections()

            End Sub
        End Class

        Public Class ComplexTCPClient
            Dim WithEvents Receiver As Specific.SimpleTCPReceiver
            Dim Sender As Specific.SimpleTCPSender

            Public Event MessageReceived(ByVal Sender As IPAddress, ByVal Message As String)

            Sub New(ByVal OpponentIPAddress As IPAddress, ByVal SendingPort As Integer, ByVal ReceivingPort As Integer)
                Receiver = New Specific.SimpleTCPReceiver(IPAddress.Any, ReceivingPort)
                Sender = New Specific.SimpleTCPSender(OpponentIPAddress, SendingPort)
            End Sub

            Public Sub Initialize()
                Receiver.Initialize()
                AddHandler Receiver.MessageReceived, AddressOf EventRaise
            End Sub

            Public Sub SendMessage(ByVal Message As String)
                Sender.SendMessage(Message)
            End Sub

            Private Sub EventRaise(ByVal Sender As IPAddress, ByVal Message As String)
                RaiseEvent MessageReceived(Sender, Message)
            End Sub

            Public Sub Close()
                Sender.CloseConnections()
                Receiver.CloseConnections()
            End Sub
        End Class

        Namespace Specific
            Public Class SimpleUDPReceiver
                Dim _Listener As UdpClient
                Dim _Port As Integer
                Dim _Byte() As Byte
                Dim Running As Boolean = False

                Dim ListenThread As New Thread(New ThreadStart(AddressOf Listen))

                Dim _EndPoint As New IPEndPoint(IPAddress.Any, 0)

                Public Event MessageReceived(ByVal Sender As IPEndPoint, ByVal Message As String)

                Public Sub New(ByVal PortToListen As Integer)
                    _Port = PortToListen
                End Sub

                Public ReadOnly Property IsRunning As Boolean
                    Get
                        Return Running
                    End Get
                End Property

                Public Sub Initialize()
                    _Listener = New UdpClient(_Port)
                    Running = True
                    ListenThread.Start()
                End Sub

                Private Sub Listen()
                    While Running
                        _Byte = _Listener.Receive(_EndPoint)
                        RaiseEvent MessageReceived(_EndPoint, Encoding.ASCII.GetString(_Byte))
                    End While
                End Sub

                Public Sub CloseConnections()
                    Running = False
                    _Listener.Close()
                End Sub
            End Class

            Public Class SimpleUDPSender
                Dim _Sender As UdpClient
                Dim _Port As Integer
                Dim _OpponentIPAddress As IPAddress
                Dim _Byte() As Byte

                Public Sub New(ByVal OpponentIPAddress As IPAddress, ByVal PortToSend As Integer)
                    _OpponentIPAddress = OpponentIPAddress
                    _Port = PortToSend
                    _Sender = New UdpClient() '_OpponentIPAddress.ToString, _Port)
                End Sub

                Public Sub SendMessage(ByVal Message As String)
                    _Sender.Connect(_OpponentIPAddress, _Port)
                    _Byte = Encoding.ASCII.GetBytes(Message)
                    _Sender.Send(_Byte, _Byte.Length)
                End Sub

                Public Sub CloseConnections()
                    _Sender.Close()
                End Sub
            End Class

            Public Class SimpleTCPReceiver
                Dim _OpponentIP As IPAddress
                Dim _Reader As StreamReader
                Dim _Port As Integer
                Dim _Listener As TcpListener
                Dim _Client As TcpClient

                Dim ListenThread As New Thread(New ThreadStart(AddressOf Listen))

                Dim Running As Boolean = True

                Public Event MessageReceived(ByVal Sender As IPAddress, ByVal Message As String)

                Public Sub New(ByVal OpponentIPAddress As IPAddress, ByVal Port As Integer)
                    _OpponentIP = OpponentIPAddress
                    _Port = Port
                End Sub

                Public ReadOnly Property IsRunning As Boolean
                    Get
                        Return Running
                    End Get
                End Property

                Public Sub Initialize()
                    _Listener = New TcpListener(New IPEndPoint(_OpponentIP, _Port))

                    _Listener.Start()

                    ListenThread.Start()
                End Sub

                Private Sub Listen()
                    Do While Running = True
                        If _Listener.Pending = True Then
                            _Client = _Listener.AcceptTcpClient
                            _Reader = New StreamReader(_Client.GetStream)
                            RaiseEvent MessageReceived(_OpponentIP, _Reader.ReadLine)
                        End If
                        ListenThread.Join(20)
                    Loop
                End Sub

                Public Sub CloseConnections()
                    Running = False
                    _Listener.Stop()
                    If Not IsNothing(_Client) Then
                        _Client.Close()
                    End If
                End Sub
            End Class

            Public Class SimpleTCPSender
                Dim _OpponentIP As IPAddress
                Dim _Writer As StreamWriter
                Dim _Port As Integer
                Dim _Sender As TcpClient
                Dim _FirstRun As Boolean = True

                Public Sub New(ByVal OpponentIPAddress As IPAddress, ByVal Port As Integer)
                    _OpponentIP = OpponentIPAddress
                    _Port = Port
                End Sub

                Public Sub SendMessage(ByVal Message As String)
                    _Sender = New TcpClient(_OpponentIP.ToString, _Port)
                    _Writer = New StreamWriter(_Sender.GetStream)
                    _Writer.WriteLine(Message)
                    _Writer.Flush()
                End Sub

                Public Sub CloseConnections()
                    If Not IsNothing(_Sender) Then
                        _Sender.Close()
                    End If
                End Sub
            End Class
        End Namespace
    End Namespace
End Namespace