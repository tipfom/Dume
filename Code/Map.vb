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
Imports Game.Tile

Namespace Global.Game

    Public Class Map
        'StandartDeklarationen
        Dim Map(,) As _Tile
        Public ReadOnly MapSize As Size

        Dim XMLDoc As XDocument

        'MapEigenschaften
        Dim MapInfo As List(Of String)

        Dim ContentDictionary As New Dictionary(Of String, Tile._Tile)
        Dim InheritedTiles As New List(Of Tile._Tile)

        Dim MapIsBroken As Boolean = False

        'spezifische MapEigs
        Dim Developer As String
        Dim Version As String
        Dim Name As String
        Dim Details As String
        Public ReadOnly Spawn As New Point

        'Mobs auf der Map
        Dim Container As Container

        Public ReadOnly Property MapData As List(Of String)
            Get
                Return MapInfo
            End Get
        End Property

        Public Sub New(ByVal MapNameInDirectory As String, ByVal _Container As Container)
            Container = _Container
            XMLDoc = XDocument.Load(String.Format("Content/Map/{0}.xml", MapNameInDirectory))

            Writer.Log.Write("Loaded Map from " + String.Format("Content/Map/{0}.xml", MapNameInDirectory))

            MapSize.Width = XMLDoc.<Map>.@MapSize.Split(Char.Parse("x"))(0) - 1
            MapSize.Height = XMLDoc.<Map>.@MapSize.Split(Char.Parse("x"))(1) - 1
            Writer.Log.Write("Map = " + MapSize.ToString)

            ReDim Map(MapSize.Width, MapSize.Height)
            Dim MapCode As String() = XMLDoc.<Map>.<MapCode>.Value.ToString.Split(Char.Parse(","))
            Dim EncryptedMapCode As New List(Of String)
            Dim CodeDef As String() = XMLDoc.<Map>.<CodeDef>.Value.ToString.Split(Char.Parse(";"))
            Dim TempTile As Tile._Tile

            Spawn = New Point(XMLDoc.<Map>.@Spawn.ToString.Split(",")(0), XMLDoc.<Map>.@Spawn.ToString.Split(",")(1))

            'LoadsCodeDefinitions
            For Each Definion In CodeDef
                Dim Var As String = Definion.Split(Char.Parse("="))(0).ToUpper
                Dim SVarTile As String = Definion.Split(Char.Parse("="))(1).Split(Char.Parse(","))(0).ToUpper
                Dim TileMeta As String = Definion.Split(Char.Parse("="))(1).Split(Char.Parse(","))(1).ToUpper

                TempTile = Container.TileContainer.GetTile(SVarTile)

                Select Case TileMeta
                    Case "NEW"
                        TempTile.Damage = 0
                    Case "DAMAGED"
                        TempTile.Damage = 30
                    Case "DESTROYED"
                        TempTile.Damage = 100
                    Case Else
                        MapIsBroken = True
                End Select

                If ContentDictionary.ContainsKey(Var) = False Then
                    ContentDictionary.Add(Var, TempTile)
                    InheritedTiles.Add(TempTile)
                End If
            Next

            'Entlädt den Mapcode

            For Each Code In MapCode
                If Not Code = "" Then
                    If IsNumeric(Code(0)) Then
                        If IsNumeric(Code(1)) Then
                            Dim Count As String = Code(0) & Code(1)
                            Dim i As Integer = Count
                            For k = 1 To i
                                Select Case Code.Length
                                    Case 3
                                        EncryptedMapCode.Add(Code(2))
                                    Case 4
                                        EncryptedMapCode.Add(Code(2) & Code(3))
                                    Case 5
                                        EncryptedMapCode.Add(Code(2) & Code(3) & Code(4))
                                End Select
                            Next
                        Else
                            Dim Count As String = Code(0)
                            Dim i As Integer = Count
                            For k = 1 To i
                                Select Case Code.Length
                                    Case 2
                                        EncryptedMapCode.Add(Code(1))
                                    Case 3
                                        EncryptedMapCode.Add(Code(1) & Code(2))
                                    Case 4
                                        EncryptedMapCode.Add(Code(1) & Code(2) & Code(3))
                                End Select
                            Next
                        End If
                    Else
                        Select Case Code.Length
                            Case 1
                                EncryptedMapCode.Add(Code(0))
                            Case 2
                                EncryptedMapCode.Add(Code(0) & Code(1))
                            Case 3
                                EncryptedMapCode.Add(Code(0) & Code(1) & Code(2))
                        End Select
                    End If
                Else
                    EncryptedMapCode.Add("")
                End If
            Next

            'Schreibt die MapTiles
            ' Maptiles sind im fließtext(beschreiben entlang der x achse)
            ' .--...---
            'wird zu
            '.--
            '...
            '---

            For y = 0 To MapSize.Height
                For x = 0 To MapSize.Height
                    If EncryptedMapCode.Count > 0 Then
                        If ContentDictionary.ContainsKey(EncryptedMapCode(0).ToUpper) Then
                            Map(x, y) = ContentDictionary.Item(EncryptedMapCode(0).ToUpper).Clone(New Point(x, y))
                            EncryptedMapCode.RemoveAt(0)
                        Else
                            Map(x, y) = Container.TileContainer.GetTile("error")
                            EncryptedMapCode.RemoveAt(0)
                        End If
                    Else
                        Map(x, y) = Container.TileContainer.GetTile("error")
                        MapIsBroken = True
                    End If
                Next
            Next

            'Loads Config
            Developer = XMLDoc.<Map>.@Developer
            Version = XMLDoc.<Map>.@Version
            Name = XMLDoc.<Map>.@Name
            EncryptedMapCode.RemoveRange(0, EncryptedMapCode.Count)

            'Loads Mobs
            Dim FuuSwarm As New Mob.AngryFuu.Swarm

            Dim Meta() As String

            Dim Type As String
            Dim Location As Point
            Dim Leader As Boolean

            For Each Mob As String In XMLDoc.<Map>.<Spawn>.Value.ToString.Split(";")
                Select Case Mob.Split("{")(0).ToUpper
                    Case "ANGRY FUU"
                        Meta = Mob.Split("{")(1).Split("}")(0).Split(",")
                        For Each Code In Meta
                            Select Case Code.Split("=")(0).ToUpper
                                Case "TYPE"
                                    Type = Code.Split("=")(1).ToUpper
                                Case "X"
                                    Location.X = Code.Split("=")(1)
                                Case "Y"
                                    Location.Y = Code.Split("=")(1)
                                Case "LEADER"
                                    Leader = True
                            End Select
                        Next
                End Select
                Select Case Type
                    Case "SPAWN"
                        FuuSwarm.AddSpawn(Location)
                    Case "MOB"
                        If Not IsNothing(Leader) Then
                            Select Case Leader
                                Case True
                                    FuuSwarm.AddLeader(New Mob.AngryFuu.Mob(Location))
                                Case False
                                    FuuSwarm.AddMob(Location)
                            End Select
                        End If
                End Select
                Location = Nothing
                Leader = Nothing
                Type = Nothing
            Next

            Container.MobContainer.Add(FuuSwarm)

            'Loads the Overlays

            Dim Rotation As Double
            For Each Over As String In XMLDoc.<Map>.<Objects>.Value.ToString.Split(";")
                Meta = Over.Split("{")
                For Each Code As String In Meta(1).Split("}")(0).Split(",")
                    Select Case Code.Split("=")(0).ToUpper
                        Case "X"
                            Location.X = Code.Split("=")(1)
                        Case "Y"
                            Location.Y = Code.Split("=")(1)
                        Case "ROTATION"
                            Rotation = Code.Split("=")(1) * Math.PI / 180
                    End Select
                Next
                Container.OverlayContainer.Add(Meta(0), Location, Rotation)
            Next

            '''''
            'Korigiert die Mauern
            Dim Left, Right, Top, Bottom As Boolean
            For x = 0 To MapSize.Width
                For y = 0 To MapSize.Height
                    If Map(x, y).Wall = True Then
                        If Not x - 1 < 0 Then If Map(x - 1, y).Wall = True Then Left = True
                        If Not x + 1 > MapSize.Width Then If Map(x + 1, y).Wall = True Then Right = True
                        If Not y - 1 < 0 Then If Map(x, y - 1).Wall = True Then Top = True
                        If Not y + 1 > MapSize.Width Then If Map(x, y + 1).Wall = True Then Bottom = True
                        Select Case Left
                            Case True
                                Select Case Right
                                    Case True
                                        Select Case Top
                                            Case True
                                                Select Case Bottom
                                                    Case True
                                                        Map(x, y).WallMode = WallMode.All
                                                    Case False
                                                        Map(x, y).WallMode = WallMode.NorthEastWest
                                                End Select
                                            Case False
                                                Select Case Bottom
                                                    Case True
                                                        Map(x, y).WallMode = WallMode.SouthEastWest
                                                    Case False
                                                        Map(x, y).WallMode = WallMode.EastWest
                                                End Select
                                        End Select
                                    Case False
                                        Select Case Top
                                            Case True
                                                Select Case Bottom
                                                    Case True
                                                        Map(x, y).WallMode = WallMode.NorthSouthWest
                                                    Case False
                                                        Map(x, y).WallMode = WallMode.EdgeSouthWest
                                                End Select
                                            Case False
                                                Select Case Bottom
                                                    Case True
                                                        Map(x, y).WallMode = WallMode.EdgeNorthWest
                                                    Case False
                                                        Map(x, y).WallMode = WallMode.EndEast
                                                End Select
                                        End Select
                                End Select
                            Case False
                                Select Case Right
                                    Case True
                                        Select Case Top
                                            Case True
                                                Select Case Bottom
                                                    Case True
                                                        Map(x, y).WallMode = WallMode.NorthSouthEast
                                                    Case False
                                                        Map(x, y).WallMode = WallMode.EdgeSouthEast
                                                End Select
                                            Case False
                                                Select Case Bottom
                                                    Case True
                                                        Map(x, y).WallMode = WallMode.EdgeNorthEast
                                                    Case False
                                                        Map(x, y).WallMode = WallMode.EndWest
                                                End Select
                                        End Select
                                    Case False
                                        Select Case Top
                                            Case True
                                                Select Case Bottom
                                                    Case True
                                                        Map(x, y).WallMode = WallMode.Normal 'überarbeiten
                                                    Case False
                                                        Map(x, y).WallMode = WallMode.EndSouth 'überarbeiten
                                                End Select
                                            Case False
                                                Select Case Bottom
                                                    Case True
                                                        Map(x, y).WallMode = WallMode.EndNorth 'überarbeiten
                                                    Case False
                                                        Map(x, y).WallMode = WallMode.All 'überarbeiten
                                                End Select
                                        End Select
                                End Select
                        End Select
                        Left = False
                        Right = False
                        Top = False
                        Bottom = False
                    End If
                Next
            Next
        End Sub

        Public ReadOnly Property MapDeveloper As String
            Get
                Return MapDeveloper
            End Get
        End Property

        Public ReadOnly Property MapVersion As String
            Get
                Return Version
            End Get
        End Property

        Public ReadOnly Property MapName As String
            Get
                Return Name
            End Get
        End Property

        Public ReadOnly Property MapDetails As String
            Get
                Return Details
            End Get
        End Property

        Public ReadOnly Property UnIndexedMapSize As Size
            Get
                Return New Size(MapSize.Width + 1, MapSize.Height + 1)
            End Get
        End Property

        Public Function GetTileOf(ByVal XKoordinate As Integer, ByVal YKoordinate As Integer) As _Tile
            Try
                Return Map(XKoordinate, YKoordinate)
            Catch ex As Exception

            End Try
        End Function

        Public Sub DoDamagaToTile(ByVal XKoordinate As Integer, ByVal YKoordinate As Integer, ByVal Damage As Integer)
            Map(XKoordinate, YKoordinate).Damage = Damage
        End Sub

        Public Sub UpdateTileContent(ByVal Textures As Visuals.TextureContainer)
            For Each MapTile As _Tile In Map
                MapTile.UpdateContent(Textures.GetScale(MapTile.ID))
            Next
        End Sub
    End Class
End Namespace