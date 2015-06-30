Imports Microsoft.DirectX.Direct3D
Imports Microsoft.DirectX
Imports Game.Character

Namespace Global.Game

    Namespace Mob
        Public Enum MobInformation
            TextureID
            History
            AttackInformation
            AttackSpecifications
            AttackingStyle
            AttacksPlayer
            AttackType
            ItemsToAttack
        End Enum

        Public Enum SpeedKind
            ProjectileSpeed
            MovementSpeed
        End Enum

        Public Enum DamageKind
            Collusion
            Projectile
            Hit
        End Enum

        Public Enum SwarmMentality
            Passiv
            Agressiv
            Scared
        End Enum

        Public Enum MobMode
            Stunned
            Normal
            Healing 'Erstmal nicht
            Hasting
            Sleeping
            Agressiv 'Hasted
            Scared
        End Enum

        Enum Health
            Destroyed
            Damaged
            Healthy
        End Enum

        Public Interface IMob
            ReadOnly Property TexturePosition As Point
            ReadOnly Property TextureSize As Size
            ReadOnly Property Speed(ByVal NeededSpeed As SpeedKind) As Integer
            ReadOnly Property Damage As Integer
            ReadOnly Property SpecialInformation(ByVal NeededInformation As MobInformation)
            ReadOnly Property Location As Point
            ReadOnly Property PointToDraw As Point
            Sub Move(ByVal Moving As Point)
            Sub CheckAttack()
            Function Collides(ByVal Shot As Weapons.IShot) As Boolean
            Function GetDamage(ByVal Damage As Integer) As Boolean
            ReadOnly Property ScorePoints As Integer
            ReadOnly Property ImagePath As String
            ReadOnly Property ImageSize As Size
            ReadOnly Property ImageScale As Double
            Property Standing As Boolean
        End Interface

        Public Interface ISpawn
            ReadOnly Property TexturePosition As Point
            ReadOnly Property TextureSize As Size
            ReadOnly Property Location As Point
            ReadOnly Property Rotation As Double
            Function GetDamage(ByVal Damage As Integer) As Boolean
            Function Collides(ByVal Shot As Weapons.IShot) As Boolean
            ReadOnly Property ScorePoints As Integer
        End Interface

        Public Interface ISwarm
            Sub AddMob(ByVal Location As Point)
            Sub AddSpawn(ByVal Location As Point)
            Sub Move()
            ReadOnly Property MobList As List(Of Mob.IMob)
            ReadOnly Property SpawnList As List(Of Mob.ISpawn)
            Property Mentality As SwarmMentality
            Sub AddLeader(ByVal Mob As Mob.IMob)
            Property Target As Character.ICharacter
        End Interface

        Structure AngryFuu
            Friend Shared MainDoc As XDocument = XDocument.Load("Content/Mob/Angry_Fuu/Mob.xml")

            Public Class Mob
                Implements IMob

                Dim Position As Point
                Dim TextureIndex As Integer
                Dim TextureCount As Integer
                Dim WithEvents TextureForwarder As New Timer
                Dim Mode As MobMode = MobMode.Normal

                Dim Health As Integer = MainDoc.<Mob>.@Health

                Dim ImageIndex As New List(Of Point)
                Dim SizeIndex As New Dictionary(Of Point, Size)
                Dim CountIndex As New Dictionary(Of MobMode, Integer)
                Dim StartIndex As New Dictionary(Of MobMode, Integer)

                Dim CurrentTexture As Integer

                Public Sub New(ByVal StartLocation As Point)
                    Dim Fortschritt As Integer = 0

                    Position = StartLocation

                    TextureCount = MainDoc.<Mob>.<Config>.<Resources>.@Count

                    For Each Resource In MainDoc.<Mob>.<Config>.<Resources>.Elements
                        Select Case Resource.@Mode.ToUpper
                            Case "NORMAL"
                                CountIndex.Add(MobMode.Normal, MainDoc.<Mob>.<Config>.Elements(Resource.@SpritesheetElement).@Count)
                                StartIndex.Add(MobMode.Normal, Resource.@StartIndex)
                            Case "STUNNED"
                                CountIndex.Add(MobMode.Stunned, MainDoc.<Mob>.<Config>.Elements(Resource.@SpritesheetElement).@Count)
                                StartIndex.Add(MobMode.Stunned, Resource.@StartIndex)
                            Case "AGRESSIV"
                                CountIndex.Add(MobMode.Agressiv, MainDoc.<Mob>.<Config>.Elements(Resource.@SpritesheetElement).@Count)
                                StartIndex.Add(MobMode.Agressiv, Resource.@StartIndex)
                        End Select

                        For i As Integer = 1 To MainDoc.<Mob>.<Config>.Elements(Resource.@SpritesheetElement).@Count
                            For Each Sprite In MainDoc.<Mob>.<Config>.Elements(Resource.@SpritesheetElement).Elements
                                If (Sprite.@ID = i.ToString) = True Then
                                    ImageIndex.Add(New Point(Sprite.@X, Sprite.@Y))
                                    SizeIndex.Add(New Point(Sprite.@X, Sprite.@Y), New Size(MainDoc.<Mob>.<Config>.Elements(Resource.@SpritesheetElement).@Width, MainDoc.<Mob>.<Config>.Elements(Resource.@SpritesheetElement).@Height))
                                End If
                            Next
                        Next
                    Next
                    TextureForwarder.Interval = MainDoc.<Mob>.<Config>.@TextureChangingIntervall
                    TextureForwarder.Enabled = True
                End Sub

                Public Sub CheckAttack() Implements IMob.CheckAttack

                End Sub

                Public ReadOnly Property Damage As Integer Implements IMob.Damage
                    Get
                        Return Health
                    End Get
                End Property

                Public Function GetDamage(ByVal Damage As Integer) As Boolean Implements IMob.GetDamage
                    Health -= Damage
                End Function

                Public Sub Move(ByVal Moving As Point) Implements IMob.Move
                    Position.X += Moving.X
                    Position.Y += Moving.Y
                End Sub

                Public ReadOnly Property SpecialInformation(ByVal NeededInformation As MobInformation) As Object Implements IMob.SpecialInformation
                    Get
                        Select Case NeededInformation
                            Case MobInformation.ItemsToAttack
                                Return MainDoc.<Mob>.<Attacking>.@Items
                            Case MobInformation.History
                                Return MainDoc.<Mob>.<Information>.<History>.Value
                            Case MobInformation.AttackType
                                Return MainDoc.<Mob>.<Attacking>.@Type
                            Case MobInformation.AttacksPlayer
                                Return Convert.ToBoolean(MainDoc.<Mob>.<Attacking>.@Player)
                            Case MobInformation.AttackSpecifications
                                Return New Object() {MainDoc.<Mob>.<Attacking>.<Stun>.@Time, MainDoc.<Mob>.<Attacking>.<Stun>.@DamageWhileStun}
                            Case MobInformation.AttackingStyle
                                Return New Object() {MainDoc.<Mob>.<Attacking>.<Style>.@Kind}
                            Case MobInformation.AttackInformation
                                Return MainDoc.<Mob>.<Information>.<Attack>.Value
                        End Select
                    End Get
                End Property

                Public ReadOnly Property Speed(ByVal NeededSpeed As SpeedKind) As Integer Implements IMob.Speed
                    Get
                        Select Case NeededSpeed
                            Case SpeedKind.MovementSpeed
                                Return MainDoc.<Mob>.<Speed>.@Movement
                            Case SpeedKind.ProjectileSpeed
                                Return MainDoc.<Mob>.<Speed>.@Hit
                        End Select
                    End Get
                End Property

                Public ReadOnly Property Location As Point Implements IMob.Location
                    Get
                        Return Position
                    End Get
                End Property

                Public ReadOnly Property PointToDraw As Point Implements IMob.PointToDraw
                    Get
                        Return Position
                    End Get
                End Property

                'Public Property _Mode As MobMode Implements IMob.Mode
                '    Get
                '        Return Mode
                '    End Get
                '    Set(ByVal value As MobMode)
                '        Mode = value
                '    End Set
                'End Property

                Public Shared ReadOnly Property _DirectXInfo As List(Of String)
                    Get
                        Dim MainDoc As XDocument = XDocument.Load(String.Format("{0}/Mob.xml", "Content/Mob/Angry_Fuu"))
                        Return New List(Of String) From {MainDoc.<Mob>.<Config>.<Resources>.@MainImage, MainDoc.<Mob>.<Config>.<Resources>.@Width, MainDoc.<Mob>.<Config>.<Resources>.@Height}
                    End Get
                End Property

                Public ReadOnly Property TexturePosition As Point Implements IMob.TexturePosition
                    Get
                        Return ImageIndex.Item(TextureIndex)
                    End Get
                End Property

                Public ReadOnly Property TextureSize As Size Implements IMob.TextureSize
                    Get
                        Return SizeIndex(ImageIndex.Item(TextureIndex))
                    End Get
                End Property

                Private Sub TimerTick() Handles TextureForwarder.Tick
                    TextureIndex += 1
                    If TextureIndex > CountIndex(Mode) - 1 + StartIndex(Mode) Then
                        TextureIndex = StartIndex(Mode)
                    End If
                End Sub

                'Public ReadOnly Property IsUsed As Boolean Implements IMob.IsUsed
                '    Get
                '        If Mode = MobMode.Agressiv Or Mode = MobMode.Normal Or Mode = MobMode.Scared Then
                '            Return False
                '        Else
                '            Return True
                '        End If
                '    End Get
                'End Property

                Public Function Collides(ByVal Shot As Weapons.IShot) As Boolean Implements IMob.Collides
                    If Math.Pow(Shot.PointToDraw.X - Location.X, 2) + Math.Pow(Shot.PointToDraw.Y - Location.Y, 2) <= 16 ^ 2 Then
                        Return True
                    Else
                        Return False
                    End If
                End Function

                Public ReadOnly Property ScorePoints As Integer Implements IMob.ScorePoints
                    Get
                        Return MainDoc.<Mob>.@ScorePoints
                    End Get
                End Property

                Public ReadOnly Property ImagePath As String Implements IMob.ImagePath
                    Get

                    End Get
                End Property

                Public ReadOnly Property ImageSize As Size Implements IMob.ImageSize
                    Get

                    End Get
                End Property

                Public Property Standing As Boolean Implements IMob.Standing

                Public ReadOnly Property ImageScale As Double Implements IMob.ImageScale
                    Get

                    End Get
                End Property
            End Class

            Public Class Spawn
                Implements ISpawn

                Dim Rotation As Integer
                Dim TextureIndex As Integer = 0
                Dim WithEvents TextureChanger As New Timer
                Dim WithEvents Spawner As New Timer

                Dim ImageIndex As New List(Of Point)
                Dim SizeIndex As New Dictionary(Of Point, Size)
                Dim CountIndex As New Dictionary(Of Health, Integer)
                Dim StartIndex As New Dictionary(Of Health, Integer)

                Dim Health As Health = Health.Healthy

                Dim Position As Point
                Dim Swarm As Swarm
                Dim Life(1) As Integer

                Sub New(ByVal Location As Point, ByVal RelatedSwarm As Swarm)
                    Position = Location
                    Swarm = RelatedSwarm
                    Life(0) = MainDoc.<Mob>.<Spawn>.@Health
                    Life(1) = MainDoc.<Mob>.<Spawn>.@Health
                    For Each Resource In MainDoc.<Mob>.<Spawn>.<Resources>.Elements
                        Select Case Resource.@Health.ToUpper
                            Case "NEW"
                                CountIndex.Add(Health.Healthy, MainDoc.<Mob>.<Spawn>.Elements(Resource.@SpritesheetElement).@Count)
                                StartIndex.Add(Health.Healthy, Resource.@StartIndex)
                            Case "DAMAGED"
                                CountIndex.Add(Health.Damaged, MainDoc.<Mob>.<Spawn>.Elements(Resource.@SpritesheetElement).@Count)
                                StartIndex.Add(Health.Damaged, Resource.@StartIndex)
                            Case "DESTROYED"
                                CountIndex.Add(Health.Destroyed, MainDoc.<Mob>.<Spawn>.Elements(Resource.@SpritesheetElement).@Count)
                                StartIndex.Add(Health.Destroyed, Resource.@StartIndex)
                        End Select

                        For i As Integer = 1 To MainDoc.<Mob>.<Spawn>.Elements(Resource.@SpritesheetElement).@Count
                            For Each Sprite In MainDoc.<Mob>.<Spawn>.Elements(Resource.@SpritesheetElement).Elements
                                If (Sprite.@ID = i.ToString) = True Then
                                    ImageIndex.Add(New Point(Sprite.@X, Sprite.@Y))
                                    SizeIndex.Add(New Point(Sprite.@X, Sprite.@Y), New Size(MainDoc.<Mob>.<Spawn>.Elements(Resource.@SpritesheetElement).@Width, MainDoc.<Mob>.<Spawn>.Elements(Resource.@SpritesheetElement).@Height))
                                End If
                            Next
                        Next
                    Next

                    TextureChanger.Interval = MainDoc.<Mob>.<Spawn>.@TextureChangingIntervall
                    TextureChanger.Start()

                    Spawner.Interval = MainDoc.<Mob>.<Spawn>.@SpawnRate
                    Spawner.Start()
                End Sub

                Public Function GetDamage(ByVal Damage As Integer) As Boolean Implements ISpawn.GetDamage
                    Life(0) -= Damage
                    If Life(0) <= 1 / 3 * Life(1) Then
                        Me.Health = Game.Mob.Health.Destroyed
                        Spawner.Stop()
                        TextureIndex = StartIndex(Game.Mob.Health.Destroyed)
                    ElseIf Life(0) <= 2 / 3 * Life(1) Then
                        Me.Health = Game.Mob.Health.Damaged
                        TextureIndex = StartIndex(Game.Mob.Health.Damaged)
                    Else
                        Me.Health = Game.Mob.Health.Healthy
                    End If
                    If Life(0) > 0 Then
                        Return True
                    Else
                        Return False
                    End If
                End Function

                Public ReadOnly Property TexturePosition As Point Implements ISpawn.TexturePosition
                    Get
                        Return ImageIndex(TextureIndex)
                    End Get
                End Property

                Public ReadOnly Property TextureSize As Size Implements ISpawn.TextureSize
                    Get
                        Return SizeIndex(ImageIndex(TextureIndex))
                    End Get
                End Property

                Public ReadOnly Property Location As Point Implements ISpawn.Location
                    Get
                        Return Position
                    End Get
                End Property

                Public Shared ReadOnly Property _DirectXInfo As List(Of String)
                    Get
                        Dim MainDoc As XDocument = XDocument.Load(String.Format("{0}/Mob.xml", "Content/Mob/Angry_Fuu"))
                        Return New List(Of String) From {MainDoc.<Mob>.<Spawn>.@Image, MainDoc.<Mob>.<Spawn>.@Width, MainDoc.<Mob>.<Spawn>.@Height}
                    End Get
                End Property

                Public ReadOnly Property Rotation1 As Double Implements ISpawn.Rotation
                    Get
                        If Not Me.Health = Game.Mob.Health.Destroyed Then
                            Rotation += 1
                        End If
                        Return Rotation * (Math.PI / 180)
                    End Get
                End Property

                Private Sub SpawnMob() Handles Spawner.Tick
                    Swarm.AddMob(Location)
                End Sub

                Private Sub TextureChange() Handles TextureChanger.Tick
                    TextureIndex += 1
                    If TextureIndex > CountIndex(Health) - 1 + StartIndex(Health) Then
                        TextureIndex = StartIndex(Health)
                    End If
                End Sub

                Public Function Collides(ByVal Shot As Weapons.IShot) As Boolean Implements ISpawn.Collides
                    If (Shot.PointToDraw.X - Me.Location.X) ^ 2 + (Shot.PointToDraw.Y - Me.Location.Y) ^ 2 <= 30 ^ 2 Then
                        Return True
                    Else
                        Return False
                    End If
                End Function

                Public ReadOnly Property ScorePoints As Integer Implements ISpawn.ScorePoints
                    Get
                        Return MainDoc.<Mob>.<Spawn>.@ScorePoints
                    End Get
                End Property
            End Class

            Class Swarm
                Implements ISwarm

                Dim MobContainer As New List(Of IMob)
                Dim SpawnContainer As New List(Of ISpawn)
                Dim Leader As Mob
                Dim Enemy As Character.ICharacter
                Dim Mentality As SwarmMentality = SwarmMentality.Agressiv

                Public Sub AddMob(ByVal Location As Point) Implements ISwarm.AddMob
                    MobContainer.Add(New Mob(Location))
                End Sub

                Public ReadOnly Property MobList As List(Of IMob) Implements ISwarm.MobList
                    Get
                        Return MobContainer
                    End Get
                End Property

                Public ReadOnly Property SpawnList As List(Of ISpawn) Implements ISwarm.SpawnList
                    Get
                        Return SpawnContainer
                    End Get
                End Property

                Sub AddLeader(ByVal Mob As IMob) Implements ISwarm.AddLeader
                    MobContainer.Add(Mob)
                    Leader = Mob
                End Sub

                Public Sub Move() Implements ISwarm.Move
                    For Each Mob As IMob In MobContainer
                        '''''''''
                        'Select Case Mob.Mode
                        '    Case MobMode.Agressiv

                        '    Case MobMode.Normal

                        '    Case MobMode.Scared

                        '    Case MobMode.Stunned
                        '        'NOTHIIIIIING!!! :D
                        '    Case MobMode.Sleeping
                        '        'WIEDER NOTHIIIIIIIIING!! :D
                        '    Case MobMode.Healing
                        '        'UND WIEDER NICHTS
                        '    Case MobMode.Hasting

                        '        ':(
                        'End Select
                        ' '''''''''
                        'If Mob.IsUsed = False Then
                        '    Select Case Mentality
                        '        Case SwarmMentality.Agressiv
                        '            Mob.Mode = MobMode.Agressiv
                        '        Case SwarmMentality.Passiv
                        '            Mob.Mode = MobMode.Normal
                        '        Case SwarmMentality.Scared
                        '            Mob.Mode = MobMode.Normal
                        '    End Select
                        'End If
                    Next
                End Sub

                Public Property _Mentality As SwarmMentality Implements ISwarm.Mentality
                    Get
                        Return Mentality
                    End Get
                    Set(ByVal value As SwarmMentality)
                        Mentality = value
                    End Set
                End Property

                Public Sub AddSpawn(ByVal Location As Point) Implements ISwarm.AddSpawn
                    SpawnContainer.Add(New AngryFuu.Spawn(Location, Me))
                End Sub

                Public Property Target As ICharacter Implements ISwarm.Target
                    Get
                        Return Enemy
                    End Get
                    Set(value As ICharacter)
                        Enemy = value
                    End Set
                End Property
            End Class
        End Structure

    End Namespace

End Namespace