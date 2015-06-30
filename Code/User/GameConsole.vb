Imports System.Net
Imports Game.Network
Imports System.IO

Namespace Global.Game
    Module GameConsole
        Dim MultiplayerConfig As XDocument = XDocument.Load("Content/Config/Client.xml")
        Dim WithEvents RunningForm As New Game.GameForm("Content/Config/StartConfig.xml")
        Dim RunningThread As New Threading.Thread(New Threading.ThreadStart(Sub()
                                                                                Application.Run(RunningForm)
                                                                            End Sub))
        Dim Connector As Specific.SimpleUDPSender
        Dim WithEvents Connection As ComplexTCPClient
        Dim Synchronization As New Threading.Thread(AddressOf Sync)
        Dim input As String
        Dim First As Boolean = True
        Dim ID As String
        Dim VersionCounter As New tipfomLib.Version(New tipfomLib.TSLDocument("dev_tima"))

        Public Highscores As New Highscores.HighscoreManager

        Sub Main()
            'Highscores.UploadNewHighscore("tipfom", 48, 0, "Hacker")
            Console.Title = "dev_tima"

            Highscores.AddToConsole(ConsoleColor.DarkRed)
            
            VersionCounter.Count(tipfomLib.VersionCount.Fourth)

            GameForm.WriteLineToConsole("Version = " + VersionCounter.ToString, MessageType.Information)

            RunningForm.WindowState = FormWindowState.Maximized
            RunningForm.Icon = New Icon("Content/Interface/dev_tima_icon.ico")

            If File.Exists("pc.spec") Then File.Delete("pc.spec")
            Using SpecWriter As New StreamWriter(File.Create("pc.spec"))
                For Each Specification As String In dev_tima.LittleSpy.PCSpec
                    SpecWriter.WriteLine(Specification)
                Next
                GameForm.WriteLineToConsole("SpecificationFile Created", MessageType.Information)
                Writer.Log.Write("Created SpecificationFile")
            End Using
            GameForm.WriteLineToConsole("Initializing Network Connection", MessageType.Important)
            Writer.Log.Write("Initializing Network Connection")
            GameForm.WriteLineToConsole(String.Format("Connecting to Server {0}(DyDNS={1}) via {2}", MultiplayerConfig.<Client>.@ServerIP, MultiplayerConfig.<Client>.@DyDNS, MultiplayerConfig.<Client>.<UDP>.@Port), MessageType.Network)
            Select Case Convert.ToBoolean(MultiplayerConfig.<Client>.@DyDNS)
                Case True
                    Connection = New Network.ComplexTCPClient(Dns.Resolve(MultiplayerConfig.<Client>.@ServerIP).AddressList(0), MultiplayerConfig.<Client>.<TCP>.@SendingPort, MultiplayerConfig.<Client>.<TCP>.@ReceivingPort)
                Case False
                    Connection = New Network.ComplexTCPClient(IPAddress.Parse(MultiplayerConfig.<Client>.@ServerIP), MultiplayerConfig.<Client>.<TCP>.@SendingPort, MultiplayerConfig.<Client>.<TCP>.@ReceivingPort)
            End Select
            Select Case Convert.ToBoolean(MultiplayerConfig.<Client>.@DyDNS)
                Case True
                    Connector = New Network.Specific.SimpleUDPSender(Dns.Resolve(MultiplayerConfig.<Client>.@ServerIP).AddressList(0), MultiplayerConfig.<Client>.<UDP>.@Port)
                    GameForm.WriteLineToConsole(String.Format("Server IPV4-Address={0}", Dns.Resolve(MultiplayerConfig.<Client>.@ServerIP).AddressList(0)), MessageType.Network)
                Case False
                    Connector = New Network.Specific.SimpleUDPSender(IPAddress.Parse(MultiplayerConfig.<Client>.@ServerIP), MultiplayerConfig.<Client>.<UDP>.@Port)
            End Select
            Connection.Initialize()
            GameForm.WriteToConsole("Username = ", MessageType.Question)
            Connector.SendMessage(String.Format("{0}|{1}|{2}", Console.ReadLine, MultiplayerConfig.<Client>.<TCP>.@SendingPort, MultiplayerConfig.<Client>.<TCP>.@ReceivingPort))
            Writer.Log.Write("Try to connect to the server")
            GameForm.WriteLineToConsole("Trying to acquire Connection to Server via TCP", MessageType.Network)
            GameForm.WriteLineToConsole(String.Format("Receiving on {0}", MultiplayerConfig.<Client>.<TCP>.@ReceivingPort), MessageType.Network)
            GameForm.WriteLineToConsole(String.Format("Sending on {0}", MultiplayerConfig.<Client>.<TCP>.@SendingPort), MessageType.Network)
            GameForm.WriteLineToConsole("Connection initializated", MessageType.Network)
            GameForm.WriteLineToConsole("Waiting for Server based Answer", MessageType.Network)
            GameForm.WriteLineToConsole("Type 'help' to get all commands listed", MessageType.Information)
            Dim Input As String = ""
            Do Until Input = "CLOSE"
                Input = Console.ReadLine().ToUpper
                Writer.Log.Write("User : " & Input)
                Select Case Input
                    Case "PCSPEC"
                        For Each Specification As String In dev_tima.LittleSpy.PCSpec
                            GameForm.WriteLineToConsole(Specification, MessageType.Information)
                        Next
                    Case "HELP"
                        GameForm.WriteLineToConsole("pcspec", MessageType.Information)
                        GameForm.WriteLineToConsole("highscores", MessageType.Information)
                    Case "HIGHSCORES"
                        Highscores.AddToConsole(ConsoleColor.DarkRed)
                End Select
            Loop
        End Sub

        Sub Sync()
            While True
                If Not IsNothing(RunningForm.Character) Then
                    Connection.SendMessage(String.Format("{0}P={1}={2}", ID, RunningForm.Character.PointToDraw.X, RunningForm.Character.PointToDraw.Y))
                End If
                For Each Command As String In RunningForm.CommandContainer
                    Connection.SendMessage(String.Format("{0}C{1}", ID, Command))
                Next
                RunningForm.CommandContainer.Clear()
                For Each ChatMessage As String In RunningForm.ChatContainer
                    Connection.SendMessage(String.Format("{0}M{1}", ID, ChatMessage))
                Next
                RunningForm.ChatContainer.Clear()
                Synchronization.Join(20)
            End While
        End Sub

        Private Sub Connection_MessageReceived(Sender As System.Net.IPAddress, Message As String) Handles Connection.MessageReceived
            If Message.StartsWith("init") Then
                ID = Message.Split("=")(1)
                GameForm.WriteLineToConsole("Connection to Server acquired", MessageType.Network)
                GameForm.WriteLineToConsole("ID : " & ID, MessageType.Important)
                Writer.Log.Write("Initializing Client (" + ID + ")")
                Connection.SendMessage("Connected")
                RunningThread.Start()
                Synchronization.Start()
            Else
                If Not Message.StartsWith("N") Then
                    Select Case Message.ToUpper()(1)
                        Case "P"
                            For Each Character As Character.ICharacter In RunningForm.CharacterContainer.Container
                                If Character.ID = Message(1) Then
                                    Character.Move(Message.Split("=")(1), Message.Split("=")(2))
                                End If
                            Next
                        Case "P"

                        Case "M"
                            RunningForm.Chat.Add(Message, Message(0))
                    End Select
                Else
                    RunningForm.CharacterContainer.Add(New Character.CharCollection.DrSlime(RunningForm.Container, RunningForm.ImageSize, Message(1)))
                End If

            End If
        End Sub

    End Module

End Namespace
