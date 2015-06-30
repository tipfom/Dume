Imports Game.Character.CharCollection

Namespace Global.Game
    Namespace Mob
        Structure Slime
            Public Class Mob
                Implements IMob

                Dim Position As New Point

                Dim Config As XDocument = XDocument.Load("Content/Mob/Slime/Mob.xml")

                Dim Modes As New List(Of String) '
                Dim PositionIndex As New Dictionary(Of Integer, Point)
                Dim SizeIndex As New Dictionary(Of Integer, Size)
                Dim IntervallIndex As New Dictionary(Of String, Integer) '
                Dim DirectionIndex As New Dictionary(Of Direction, String) '
                Dim StartIndex As New Dictionary(Of String, Integer) '
                Dim CountIndex As New Dictionary(Of String, Integer) '

                Dim CurrentMode As String
                Dim CurrentTexture As Integer
                Dim CurrentDirection As Direction = Direction.Left

                Dim WithEvents TextureChanger As New Timer

                Public Sub New(ByVal StartLocation As Point)
                    Position = StartLocation

                    For Each Resource In Config.<Mob>.<Resources>.Elements
                        Modes.Add(Resource.@Mode)
                        StartIndex.Add(Resource.@Mode, CurrentTexture)
                        DirectionIndex.Add([Enum].Parse(GetType(Direction), Resource.@Direction), Resource.@Mode)

                        Select Case Convert.ToBoolean(Resource.@Sprite)
                            Case True
                                CountIndex.Add(Resource.@Mode, Config.<Mob>.Elements(Resource.@SpritesheetElement).Count)
                                IntervallIndex.Add(Resource.@Mode, Resource.@TextureChangingIntervall)

                                For Each Sprite In Config.<Mob>.Elements(Resource.@SpritesheetElement).Elements
                                    PositionIndex.Add(CurrentTexture, New Point(Sprite.@X, Sprite.@Y))
                                    SizeIndex.Add(CurrentTexture, New Point(Resource.@Width, Resource.@Height))
                                    CurrentTexture += 1
                                Next
                            Case False
                                CountIndex.Add(Resource.@Mode, 1)

                                PositionIndex.Add(CurrentTexture, New Point(Resource.@X, Resource.@Y))
                                SizeIndex.Add(CurrentTexture, New Point(Resource.@Width, Resource.@Height))

                                CurrentTexture += 1
                        End Select
                    Next

                    CurrentMode = Modes(0)
                    CurrentTexture = StartIndex(CurrentMode)
                End Sub

                Public Sub CheckAttack() Implements IMob.CheckAttack

                End Sub

                Public Function Collides(Shot As Weapons.IShot) As Boolean Implements IMob.Collides
                    Return True
                End Function

                Public ReadOnly Property Damage As Integer Implements IMob.Damage
                    Get
                        Return Config.<Mob>.<Damage>.@Hit
                    End Get
                End Property

                Public Function GetDamage(Damage As Integer) As Boolean Implements IMob.GetDamage
                    Return True
                End Function

                Public ReadOnly Property Location As Point Implements IMob.Location
                    Get
                        Return Position
                    End Get
                End Property

                Public Sub Move(Moving As Point) Implements IMob.Move
                    Position.X += Moving.X
                    Position.Y += Moving.Y
                    Select Case Moving.X
                        Case Is > 0
                            CurrentDirection = Direction.Right
                        Case Is < 0
                            CurrentDirection = Direction.Left
                        Case Else
                            Select Case Moving.Y
                                Case Is > 0
                                    CurrentDirection = Direction.Bottom
                                Case Is < 0
                                    CurrentDirection = Direction.Top
                            End Select
                    End Select
                    If Not CurrentMode = DirectionIndex(CurrentDirection) Then
                        CurrentMode = DirectionIndex(CurrentDirection)
                        CurrentTexture = StartIndex(CurrentMode)
                        If IntervallIndex.ContainsKey(CurrentMode) Then
                            TextureChanger.Interval = IntervallIndex(CurrentMode)
                            TextureChanger.Enabled = True
                        Else
                            TextureChanger.Enabled = False
                        End If
                    End If
                End Sub

                Public ReadOnly Property PointToDraw As Point Implements IMob.PointToDraw
                    Get
                        Return New Point(Position.X + TextureSize.Width / 2, Position.Y + TextureSize.Height / 2)
                    End Get
                End Property

                Public ReadOnly Property ScorePoints As Integer Implements IMob.ScorePoints
                    Get
                        Return Config.<Mob>.@ScorePoints
                    End Get
                End Property

                Public ReadOnly Property SpecialInformation(NeededInformation As MobInformation) As Object Implements IMob.SpecialInformation
                    Get
                        Select Case NeededInformation
                            Case MobInformation.TextureID
                                Return Config.<Mob>.@ID
                        End Select
                        Return False
                    End Get
                End Property

                Public ReadOnly Property Speed(NeededSpeed As SpeedKind) As Integer Implements IMob.Speed
                    Get
                        Return Config.<Mob>.<Speed>.@Movement
                    End Get
                End Property

                Public ReadOnly Property TexturePosition As Point Implements IMob.TexturePosition
                    Get
                        Return PositionIndex(CurrentTexture)
                    End Get
                End Property

                Public ReadOnly Property TextureSize As Size Implements IMob.TextureSize
                    Get
                        Return SizeIndex(CurrentTexture)
                    End Get
                End Property

                Public ReadOnly Property ImagePath As String Implements IMob.ImagePath
                    Get
                        Return Config.<Mob>.<Resources>.@Image
                    End Get
                End Property

                Public ReadOnly Property ImageSize As Size Implements IMob.ImageSize
                    Get
                        Return New Size(Config.<Mob>.<Resources>.@Width, Config.<Mob>.<Resources>.@Height)
                    End Get
                End Property

                Public Property Standing As Boolean Implements IMob.Standing
                    Get
                        If CurrentDirection = Direction.BottomStanding Or CurrentDirection = Direction.TopStanding Then Return True
                        Return False
                    End Get
                    Set(value As Boolean)
                        If value = True Then
                            If CurrentDirection = Direction.Top Or CurrentDirection = Direction.TopStanding Then
                                CurrentDirection = Direction.TopStanding
                            Else
                                CurrentDirection = Direction.BottomStanding
                            End If
                        Else

                        End If
                        If Not CurrentMode = DirectionIndex(CurrentDirection) Then
                            CurrentMode = DirectionIndex(CurrentDirection)
                            CurrentTexture = StartIndex(CurrentMode)
                            If IntervallIndex.ContainsKey(CurrentMode) Then
                                TextureChanger.Interval = IntervallIndex(CurrentMode)
                                TextureChanger.Enabled = True
                            Else
                                TextureChanger.Enabled = False
                            End If
                        End If
                    End Set
                End Property

                Private Sub TextureChanger_Tick(sender As Object, e As EventArgs) Handles TextureChanger.Tick
                    CurrentTexture += 1
                    If Not CurrentTexture <= StartIndex(CurrentMode) + CountIndex(CurrentMode) Then
                        CurrentTexture = StartIndex(CurrentMode)
                    End If
                End Sub

                Public ReadOnly Property ImageScale As Double Implements IMob.ImageScale
                    Get
                        Return Val(Config.<Mob>.<Resources>.@ImageScale) / 100
                    End Get
                End Property
            End Class

            Public Class Spawn
                Implements ISpawn

                Public Function Collides(Shot As Weapons.IShot) As Boolean Implements ISpawn.Collides

                End Function

                Public Function GetDamage(Damage As Integer) As Boolean Implements ISpawn.GetDamage

                End Function

                Public ReadOnly Property Location As Point Implements ISpawn.Location
                    Get

                    End Get
                End Property

                Public ReadOnly Property Rotation As Double Implements ISpawn.Rotation
                    Get

                    End Get
                End Property

                Public ReadOnly Property ScorePoints As Integer Implements ISpawn.ScorePoints
                    Get

                    End Get
                End Property

                Public ReadOnly Property TexturePosition As Point Implements ISpawn.TexturePosition
                    Get

                    End Get
                End Property

                Public ReadOnly Property TextureSize As Size Implements ISpawn.TextureSize
                    Get

                    End Get
                End Property
            End Class

            Public Class Swarm
                Implements ISwarm

                Dim MobContainer As New List(Of IMob)
                Dim SpawnContainer As New List(Of ISpawn)

                Dim Leader As Mob

                Dim Path As New List(Of Tile._Tile)
                Dim Pathfinding As KI.PathFinding

                Dim Enemy As Character.ICharacter

                Dim Container As Container

                Public Sub New(ByVal _Container As Container)
                    Container = _Container
                    Pathfinding = New KI.PathFinding(Container.Map)
                End Sub

                Public Sub AddLeader(Mob As IMob) Implements ISwarm.AddLeader
                    Leader = Mob
                    If Not MobContainer.Contains(Leader) Then MobContainer.Add(Leader)

                    Writer.Log.Write("Set Leader @SlimeSwarm")
                End Sub

                Public Sub AddMob(Location As Point) Implements ISwarm.AddMob
                    MobContainer.Add(New Mob(Location))
                    Writer.Log.Write("Added Mob @SlimeSwarm")
                End Sub

                Public Sub AddSpawn(Location As Point) Implements ISwarm.AddSpawn
                    Writer.Log.Write("Added Spawn @SlimeSwarm")
                End Sub

                Public Property Mentality As SwarmMentality Implements ISwarm.Mentality

                Public ReadOnly Property MobList As List(Of IMob) Implements ISwarm.MobList
                    Get
                        Return MobContainer
                    End Get
                End Property

                Public Sub Move() Implements ISwarm.Move
                    If Path.Count > 0 Then
                        For Each Mob As IMob In MobContainer
                            Select Case Mob.Location.X \ Container.ImageSize - Path(Path.Count - 1).MapLocation.X
                                Case Is < 0
                                    Mob.Move(New Point(Mob.Speed(SpeedKind.MovementSpeed), 0))
                                Case Is > 0
                                    Mob.Move(New Point(-Mob.Speed(SpeedKind.MovementSpeed), 0))
                                Case Else
                                    Select Case Mob.Location.Y \ Container.ImageSize - Path(Path.Count - 1).MapLocation.Y
                                        Case Is > 0
                                            Mob.Move(New Point(0, -Mob.Speed(SpeedKind.MovementSpeed)))
                                        Case Is < 0
                                            Mob.Move(New Point(0, Mob.Speed(SpeedKind.MovementSpeed)))
                                        Case Else
                                            Path = Pathfinding.FindPath(New Point(Leader.Location.X \ Container.ImageSize, Leader.Location.Y \ Container.ImageSize), New Point(Enemy.PointToDraw.X \ Container.ImageSize, Enemy.PointToDraw.Y \ Container.ImageSize))
                                            If Not Path.Count = 0 Then Path.RemoveAt(Path.Count - 1)
                                    End Select
                            End Select
                        Next
                    Else
                        For Each Mob As IMob In MobContainer
                            Mob.Standing = True
                        Next
                        Path = Pathfinding.FindPath(New Point(Leader.Location.X \ Container.ImageSize, Leader.Location.Y \ Container.ImageSize), New Point(Enemy.PointToDraw.X \ Container.ImageSize, Enemy.PointToDraw.Y \ Container.ImageSize))
                    End If
                End Sub

                Public ReadOnly Property SpawnList As List(Of ISpawn) Implements ISwarm.SpawnList
                    Get
                        Return SpawnContainer
                    End Get
                End Property

                Public Property Target As Character.ICharacter Implements ISwarm.Target
                    Get
                        Return Enemy
                    End Get
                    Set(value As Character.ICharacter)
                        Enemy = value
                        Path = Pathfinding.FindPath(New Point(Leader.Location.X \ Container.ImageSize, Leader.Location.Y \ Container.ImageSize), New Point(Enemy.PointToDraw.X \ Container.ImageSize, Enemy.PointToDraw.Y \ Container.ImageSize))

                    End Set
                End Property
            End Class
        End Structure
    End Namespace
End Namespace