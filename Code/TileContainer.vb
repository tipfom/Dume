Imports System.IO

Namespace Global.Game
    Namespace Tile
        Public Class TileContainer
            Public ReadOnly Container As New List(Of _Tile)

            Public Sub LoadFrom(ByVal Directory As String)
                For Each Tile As DirectoryInfo In New DirectoryInfo(Directory).GetDirectories
                    Container.Add(New _Tile(XDocument.Load(Tile.FullName + "/Tile.xml"), New Point(0, 0)))
                    Writer.Log.Write("Loaded Tile from " + Tile.FullName)
                Next
            End Sub

            Public Function GetTile(ByVal Name As String) As _Tile
                If Container.Where(Function(Item) Item.Name.ToUpper = Name.ToUpper).Count > 0 Then
                    Return Container.Where(Function(Item) Item.Name.ToUpper = Name.ToUpper)(0)
                Else
                    Return Container.Where(Function(Item) Item.Name.ToUpper = "ERROR")(0)
                End If
            End Function
        End Class

        Public Enum WallMode
            EdgeNorthWest = 1
            EdgeNorthEast = 2
            EdgeSouthWest = 3
            EdgeSouthEast = 4

            Normal = 5

            EastWest = 6

            NorthSouthWest = 8
            NorthSouthEast = 9
            NorthEastWest = 10
            SouthEastWest = 11

            All = 12

            EndNorth = 13
            EndSouth = 14
            EndWest = 15
            EndEast = 16
        End Enum

        Public Class _Tile
            Implements KI.IHeapItem(Of _Tile)

            '' Operatoren
            Public Shared Operator =(ByVal TileOne As _Tile, ByVal TileTwo As _Tile)
                If TileOne.Equals(TileTwo) Then
                    Return True
                End If
                Return False
            End Operator

            Public Shared Operator <>(ByVal TileOne As _Tile, ByVal TileTwo As _Tile)
                If TileOne.Equals(TileTwo) Then
                    Return False
                End If
                Return True
            End Operator


            Dim Config As XDocument

            Public ReadOnly Name As String
            Public ReadOnly ID As String

            Public ReadOnly Shield As Integer
            Public ReadOnly SlowDown As Integer
            Dim _Damage As Integer

            Public ReadOnly Interaction As Boolean
            Public ReadOnly Visitable As Boolean
            Public ReadOnly Permeable As Boolean
            Public ReadOnly Destroyable As Boolean
            Public ReadOnly Wall As Boolean

            Public ReadOnly TexturePath As String
            Public ReadOnly ImageSize As Size

            Dim Modes As New List(Of String)
            Dim TextureSizeIndex As New Dictionary(Of Integer, Size)
            Dim TexturePositionIndex As New Dictionary(Of Integer, Point)
            Dim WallIndex As New Dictionary(Of WallMode, String) 'Jedem Wandmodus wird ein modus untergeordnet
            Dim DamageIndex As New Dictionary(Of Integer, String) 'Jedem Schadensfortschritt wird ein Modus zugeordnet
            Dim DamageList As New List(Of Integer)

            'SPRITE
            Dim StartIndex As New Dictionary(Of String, Integer)
            Dim CountIndex As New Dictionary(Of String, Integer)
            Dim IntervallDictionary As New Dictionary(Of String, Integer)

            Dim CurrentTexture As Integer = 0

            Dim WithEvents TextureChanger As New System.Windows.Forms.Timer

            Dim Mode As String
            Dim _WallMode As WallMode = Tile.WallMode.Normal

            '==== PATHFINDING ====
            Public gCost As Integer
            Public hCost As Integer
            Public ReadOnly MapLocation As Point
            Public ParentLocation As Point
            Dim HeapIndex As Integer

            Public NormalTileImageSize As Integer

            Public ReadOnly Property fCost As Integer
                Get
                    Return gCost + hCost
                End Get
            End Property

            Public Sub New(ByVal TileConfig As XDocument, ByVal Location As Point)
                MapLocation = Location

                'PATHFINDING
                gCost = 0
                hCost = 0

                Config = TileConfig

                Name = TileConfig.<Tile>.@Name
                ID = TileConfig.<Tile>.@ID
                Shield = TileConfig.<Tile>.<Data>.@Shield
                SlowDown = TileConfig.<Tile>.<Data>.@SlowDown

                Interaction = Convert.ToBoolean(TileConfig.<Tile>.@Interaction)
                Visitable = Convert.ToBoolean(TileConfig.<Tile>.<Data>.@Visitable)
                Permeable = Convert.ToBoolean(TileConfig.<Tile>.<Data>.@Permeable)
                Destroyable = Convert.ToBoolean(TileConfig.<Tile>.@Destroyable)
                Wall = Convert.ToBoolean(TileConfig.<Tile>.@Wall)

                NormalTileImageSize = TileConfig.<Tile>.<Resources>.@TileSize

                Select Case Wall
                    Case True
                        For Each Resource In TileConfig.<Tile>.<Resources>.Elements
                            Modes.Add(Resource.@Mode)
                            If Convert.ToBoolean(Resource.@Sprite) = True Then
                                StartIndex.Add(Resource.@Mode, CurrentTexture)
                                CountIndex.Add(Resource.@Mode, Config.<Tile>.Elements(Resource.@SpritesheetElement).Elements.Count)
                                For Each Sprite In Config.<Tile>.Elements(Resource.@SpritesheetElement).Elements
                                    TexturePositionIndex.Add(CurrentTexture, New Point(Resource.@X, Resource.@Y))
                                    TextureSizeIndex.Add(CurrentTexture, New Size(Resource.@Width, Resource.@Height))
                                    CurrentTexture += 1
                                Next
                                IntervallDictionary.Add(Resource.@Mode, Resource.@TextureChangingIntervall)
                            Else
                                TexturePositionIndex.Add(CurrentTexture, New Point(Resource.@X, Resource.@Y))
                                TextureSizeIndex.Add(CurrentTexture, New Size(Resource.@Width, Resource.@Height))
                                StartIndex.Add(Resource.@Mode, CurrentTexture)
                                CountIndex.Add(Resource.@Mode, 1)
                                CurrentTexture += 1
                            End If
                            WallIndex.Add([Enum].Parse(GetType(WallMode), Resource.@WallMode), Resource.@Mode)
                            If Destroyable = True Then
                                DamageList.Add(Resource.@MinDamage)
                                DamageIndex.Add(DamageList.Count, Resource.@Mode)
                            End If
                        Next
                    Case False
                        For Each Resource In TileConfig.<Tile>.<Resources>.Elements
                            Modes.Add(Resource.@Mode)
                            If Convert.ToBoolean(Resource.@Sprite) = True Then
                                StartIndex.Add(Resource.@Mode, CurrentTexture)
                                CountIndex.Add(Resource.@Mode, Config.<Tile>.Elements(Resource.@SpritesheetElement).Elements.Count)
                                For Each Sprite In Config.<Tile>.Elements(Resource.@SpritesheetElement).Elements
                                    TexturePositionIndex.Add(CurrentTexture, New Point(Sprite.@X, Sprite.@Y))
                                    TextureSizeIndex.Add(CurrentTexture, New Size(Config.<Tile>.Elements(Resource.@SpritesheetElement).@Width, Config.<Tile>.Elements(Resource.@SpritesheetElement).@Height))
                                    CurrentTexture += 1
                                Next
                                IntervallDictionary.Add(Resource.@Mode, Resource.@TextureChangingIntervall)
                            Else
                                TexturePositionIndex.Add(CurrentTexture, New Point(Resource.@X, Resource.@Y))
                                TextureSizeIndex.Add(CurrentTexture, New Size(Resource.@Width, Resource.@Height))
                                StartIndex.Add(Resource.@Mode, CurrentTexture)
                                CountIndex.Add(Resource.@Mode, 1)
                                CurrentTexture += 1
                            End If
                            If Destroyable = True Then
                                DamageIndex.Add(Resource.@MinDamage, Resource.@Mode)
                                DamageIndex.Add(DamageList.Count, Resource.@Mode)
                            End If
                        Next
                End Select
                TexturePath = TileConfig.<Tile>.<Resources>.@Image
                ImageSize = New Size(TileConfig.<Tile>.<Resources>.@Width, TileConfig.<Tile>.<Resources>.@Height)

                Mode = TileConfig.<Tile>.<Resources>.@StartMode

                CurrentTexture = 0

                If IntervallDictionary.ContainsKey(Mode) Then
                    TextureChanger.Interval = IntervallDictionary(Mode)
                    TextureChanger.Enabled = True
                End If

                DamageList.Sort()
            End Sub

            Public ReadOnly Property TexturePosition As Point
                Get
                    Return TexturePositionIndex(CurrentTexture)
                End Get
            End Property

            Public ReadOnly Property TextureSize As Size
                Get
                    Return TextureSizeIndex(CurrentTexture)
                End Get
            End Property

            Public Property Damage As Integer
                Get
                    Return _Damage
                End Get
                Set(value As Integer)
                    _Damage = value * (Shield / 100)
                    For Each Item As Integer In DamageList
                        If Item >= _Damage Then
                            Exit For
                            Mode = DamageIndex(DamageList.IndexOf(Item))
                            CurrentTexture = StartIndex(Mode)
                        End If
                    Next
                End Set
            End Property

            Public Function Clone(ByVal Location As Point) As Tile._Tile
                Return New _Tile(Config, Location)
            End Function

            Public Sub UpdateContent(ByVal Scale As Integer)
                For i = 0 To TextureSizeIndex.Count - 1
                    TextureSizeIndex(i) = New Size(TextureSizeIndex(i).Width * Scale, TextureSizeIndex(i).Height * Scale)
                Next
                For i = 0 To TexturePositionIndex.Count - 1
                    TexturePositionIndex(i) = New Point(TexturePositionIndex(i).X * Scale, TexturePositionIndex(i).Y * Scale)
                Next
            End Sub

            Public WriteOnly Property WallMode As WallMode
                Set(value As WallMode)
                    If Wall = True Then
                        _WallMode = value
                        Mode = WallIndex(_WallMode)
                        CurrentTexture = StartIndex(Mode)
                    End If
                End Set
            End Property

            Private Sub TextureChanger_Tick(sender As Object, e As System.EventArgs) Handles TextureChanger.Tick
                CurrentTexture += 1
                If CurrentTexture >= StartIndex(Mode) + CountIndex(Mode) Then
                    CurrentTexture = StartIndex(Mode)
                End If
            End Sub

            Public Property _HeapIndex As Integer Implements KI.IHeapItem(Of _Tile).HeapIndex
                Get
                    Return HeapIndex
                End Get
                Set(value As Integer)
                    HeapIndex = value
                End Set
            End Property
            Public Function CompareTo(other As _Tile) As Integer Implements System.IComparable(Of _Tile).CompareTo
                Dim CompareInteger As Integer = fCost.CompareTo(other.fCost)

                If CompareInteger = 0 Then
                    CompareInteger = hCost.CompareTo(other.hCost)
                End If

                Return -CompareInteger
            End Function
        End Class
    End Namespace
End Namespace