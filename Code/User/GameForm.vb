Imports Game

Namespace Global.Game
    Public Class GameForm
        Inherits Form

        'Standart Deklaration(==Game.Main)

        Dim ConfigDoc As XDocument
        Dim MainDoc As XDocument = XDocument.Load("Content/Config/Main.xml")

        Public ReadOnly CommandContainer As New List(Of String)
        Public ReadOnly ChatContainer As New List(Of String)

        Dim Running As Boolean = False

        Public Character As Character.ICharacter
        Public ReadOnly CharacterContainer As New CharacterContainer
        Dim Gun As Weapons.IGun
        Public Map As Map
        Dim FuuSwarm As New Mob.AngryFuu.Swarm
        Public ReadOnly Mobs As New MobContainer
        Public ReadOnly Overlays As New Overlay.OverlayContainer
        Public ReadOnly Tiles As New Tile.TileContainer
        'CONFIGS
        Public ImageSize As Integer
        Dim ImageScale As Double
        Dim FontSize As Integer

        Dim Tracker As Tracker
        'Dim WithEvents Drawer As Visuals.Drawer
        Dim WithEvents DrawerDX As Visuals.DirectXDisplayer

        Public Container As Container
      
        Public Chat As ChatBox

        Sub New(ByVal ConfigFile As String)
            ConfigDoc = XDocument.Load(ConfigFile)
        End Sub

        Public Sub StartGame()
            Writer.Log.Write("Start Loading Overlays")
            Overlays.LoadFrom("Content/Overlay")
            Writer.Log.Write("Start Loading Tiles")
            Tiles.LoadFrom("Content/Tile")
            Container = New Container(Mobs, Tiles, Overlays, CharacterContainer)
            Writer.Log.Write("Start Loading Map")
            Map = New Map(ConfigDoc.<Config>.<Map>.Value.ToString, Container)
            Container.Form = Me
            Container.Tracker = Tracker
            Container.Map = Map
            Chat = New ChatBox(New Size(800, 600), Style.Shadowed, Me)

            Select Case ConfigDoc.<Config>.<Resolution>.Value.ToString
                Case "1366px"
                    Writer.Log.Write("Loading Settings @1366px")
                    ImageSize = Convert.ToInt32(MainDoc.<Game>.<Configuration>.<_1366px>.@ImageSize.ToString)
                    ImageScale = Val(MainDoc.<Game>.<Configuration>.<_1366px>.@ImageScale)
                    Container.ImageScale = ImageScale
                    Container.ImageSize = ImageSize

                    Character = New Character.CharCollection.DrSlime(Container, ImageSize, "ME")

                    CharacterContainer.Add(Character)
                    CharacterContainer.MainCharacter = Character
                    FontSize = MainDoc.<Game>.<Configuration>.<_1366px>.@FontSize
                Case "1600px"
                    Writer.Log.Write("Loading Settings @1600px")
                    ImageSize = Convert.ToInt32(MainDoc.<Game>.<Configuration>.<_1600px>.@ImageSize.ToString)
                    ImageScale = MainDoc.<Game>.<Configuration>.<_1600px>.@ImageScale
                    Container.ImageScale = ImageScale
                    Container.ImageSize = ImageSize

                    Character = New Character.CharCollection.DrSlime(Container, ImageSize, "ME")

                    CharacterContainer.Add(Character)
                    CharacterContainer.MainCharacter = Character
                    FontSize = MainDoc.<Game>.<Configuration>.<_1600px>.@FontSize
                Case "1920px"
                    Writer.Log.Write("Loading Settings @1920px")
                    ImageSize = Convert.ToInt32(MainDoc.<Game>.<Configuration>.<_1920px>.@ImageSize.ToString)
                    ImageScale = MainDoc.<Game>.<Configuration>.<_1920px>.@ImageScale
                    Container.ImageScale = ImageScale
                    Container.ImageSize = ImageSize

                    Character = New Character.CharCollection.DrSlime(Container, ImageSize, "ME")

                    CharacterContainer.Add(Character)
                    CharacterContainer.MainCharacter = Character
                    FontSize = MainDoc.<Game>.<Configuration>.<_1920px>.@FontSize
            End Select
            Tracker = New Tracker(Me, Character)

            Character.SlimeBall = New Weapon.SlimeBall.FirstSlimeBall(Character)
            Character.Laser = New Weapon.Laser.NormalLaser(Character)

            DrawerDX = New Visuals.DirectXDisplayer(Me, Map, ImageSize, FontSize, Container, ImageScale)
            GameForm.WriteLineToConsole("CONFIGS LOADED", MessageType.Information)
            Writer.Log.Write("CONFIGS LOADED")
            '''''''''''''''''''''''''''''''''''
            ''''''''''''''''''''''''''''''''''''
            ''''''''''''''''''''''''''''''''''''
            DrawerDX.Initialize()

            ''Wegen des komischen interface bugs
            DrawerDX.ReInitialize()

            Running = True

            Dim TEST As New Mob.Slime.Swarm(Container)
            TEST.AddLeader(New Mob.Slime.Mob(New Point(10, 10)))
            TEST.Target = Character
            Container.MobContainer.Add(TEST)

            Writer.Log.Write("Entering Game Loop")
            While Running ' MainLoop
                DrawerDX.Render()
                DrawerDX.GetFPS()
                Tracker.DoActions()
                Character.UpdateEntitys()
                Character.MoveShots()
                Mobs.MoveAll()
                Application.DoEvents()
            End While
        End Sub

        Public Sub Close() Handles Me.FormClosing
            Running = False
        End Sub

        Private Sub GameForm_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
            Writer.Log.Write("Resized Form")
            If Not IsNothing(DrawerDX) Then DrawerDX.ReInitialize()
        End Sub

        Private Sub Started() Handles Me.Shown
            Me.StartGame()
        End Sub

        Public Sub ChangeValue(ByVal ValueToChange As ChangeableValue, ByVal Params As String)
            Select Case ValueToChange
                Case ChangeableValue.EntityPosition

                Case ChangeableValue.PlayerPosition
                    Character.Move(Params.Split(" ")(0), Params.Split(" ")(1))
                Case ChangeableValue.MobMentality
                    Mobs.List(Params.Split(" ")(0)).Mentality = Params.Split(" ")(1)
            End Select
        End Sub

        Public Shared Sub WriteLineToConsole(ByVal Text As String, ByVal Type As MessageType)
            Dim Before As ConsoleColor = Console.ForegroundColor
            Select Case Type
                Case MessageType.Important
                    Console.ForegroundColor = ConsoleColor.DarkRed
                Case MessageType.Information
                    Console.ForegroundColor = ConsoleColor.Blue
                Case MessageType.Question
                    Console.ForegroundColor = ConsoleColor.Yellow
                Case MessageType.Network
                    Console.ForegroundColor = ConsoleColor.DarkGreen
            End Select
            Console.WriteLine(Text)
            Console.ForegroundColor = Before
        End Sub

        Public Shared Sub WriteToConsole(ByVal Text As String, ByVal Type As MessageType)
            Dim Before As ConsoleColor = Console.ForegroundColor
            Select Case Type
                Case MessageType.Important
                    Console.ForegroundColor = ConsoleColor.DarkRed
                Case MessageType.Information
                    Console.ForegroundColor = ConsoleColor.Blue
                Case MessageType.Question
                    Console.ForegroundColor = ConsoleColor.Yellow
                Case MessageType.Network
                    Console.ForegroundColor = ConsoleColor.DarkGreen
            End Select
            Console.Write(Text)
            Console.ForegroundColor = Before
        End Sub
    End Class

    Public Enum MessageType
        Important
        Information
        Question
        Network
    End Enum

    Public Enum ChangeableValue
        EntityPosition
        PlayerPosition
        MobMentality
    End Enum
End Namespace