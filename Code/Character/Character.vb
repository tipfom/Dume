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

Namespace Global.Game
    Public Class CharacterContainer
        Dim _Container As New List(Of Character.ICharacter)
        Dim MainCharacterIndex As Integer

        Public ReadOnly Property Container As List(Of Character.ICharacter)
            Get
                Return _Container
            End Get
        End Property

        Public Sub Add(ByVal Character As Character.ICharacter)
            _Container.Add(Character)
        End Sub

        Property MainCharacter As Character.ICharacter
            Get
                Return _Container(MainCharacterIndex)
            End Get
            Set(value As Character.ICharacter)
                MainCharacterIndex = _Container.IndexOf(value)
            End Set
        End Property
    End Class

    Namespace Character
        Friend Class MoveChecker
            Public Shared Function WouldGoOutOfMapByMovingLeft(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WGOOM As Boolean = False

                If CuChar.PointToDraw.X - CuChar.Speed - 2 * CuChar.ImageSize <= 0 Then WGOOM = True

                Return WGOOM
            End Function

            Public Shared Function WouldGoOutOfMapByMovingRight(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WGOOM As Boolean = False

                If CuChar.PointToDraw.X + CuChar.Speed + 2 * CuChar.ImageSize >= CuChar.ImageSize * CuChar.Container.Map.UnIndexedMapSize.Width Then WGOOM = True

                Return WGOOM
            End Function

            Public Shared Function WouldGoOutOfMapByMovingTop(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WGOOM As Boolean = False

                If CuChar.PointToDraw.Y - CuChar.Speed - CuChar.ImageSize <= 0 Then WGOOM = True

                Return WGOOM
            End Function

            Public Shared Function WouldGoOutOfMapByMovingBottom(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WGOOM As Boolean = False

                'Select Case CuChar.Facing
                '    Case Facing.North, Facing.South
                '        If CuChar.PointToDraw.Y + CuChar.Speed + CuChar.ImageSize / 2 >= CuChar.ImageSize * (CuChar.Map.MapSize.Height - 0.5) Then WGOOM = True
                '    Case Facing.East, Facing.West
                '        If CuChar.PointToDraw.Y + CuChar.Speed + CuChar.ImageSize >= CuChar.ImageSize * (CuChar.Map.MapSize.Height - 1) Then WGOOM = True
                '    Case Else 'Da die anderen alle die selbe hitbox haben(selbe wie north auf x)
                '        If CuChar.PointToDraw.Y + CuChar.Speed + CuChar.ImageSize >= CuChar.ImageSize * (CuChar.Map.MapSize.Height - 1) Then WGOOM = True
                'End Select

                Return WGOOM
            End Function

            Public Shared Function WouldCollideOnLeft(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WC As Boolean = False

                If CuChar.PointToDraw.Y Mod CuChar.ImageSize = 0 Then
                    Dim CurrentCharTilesX As Integer
                    Dim CurrentCharTileY(1) As Integer
                    'Den 32,32 nullpunkt beachten
                    CurrentCharTilesX = ((CuChar.PointToDraw.X - CuChar.ImageSize) - CuChar.Speed) \ CuChar.ImageSize

                    CurrentCharTileY(0) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize
                    CurrentCharTileY(1) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize + 1

                    Try
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(0)).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(1)).Visitable = False Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try
                Else
                    Dim CurrentCharTilesX As Integer
                    Dim CurrentCharTileY(2) As Integer

                    CurrentCharTilesX = ((CuChar.PointToDraw.X - CuChar.ImageSize) - CuChar.Speed) \ CuChar.ImageSize

                    CurrentCharTileY(0) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize
                    CurrentCharTileY(1) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize + 1
                    CurrentCharTileY(2) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize + 2

                    Try
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(0)).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(1)).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(2)).Visitable = False Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try

                End If


                Return WC
            End Function

            Public Shared Function WouldCollideOnRight(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WC As Boolean = False

                If CuChar.PointToDraw.Y Mod CuChar.ImageSize = 0 Then
                    Dim CurrentCharTilesX As Integer
                    Dim CurrentCharTileY(1) As Integer
                    'Den 32,32 nullpunkt beachten
                    CurrentCharTilesX = ((CuChar.PointToDraw.X - CuChar.ImageSize) + CuChar.Speed) \ CuChar.ImageSize + 2

                    CurrentCharTileY(0) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize
                    CurrentCharTileY(1) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize + 1

                    Try
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(0)).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(1)).Visitable = False Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try
                Else
                    Dim CurrentCharTilesX As Integer
                    Dim CurrentCharTileY(2) As Integer

                    CurrentCharTilesX = ((CuChar.PointToDraw.X - CuChar.ImageSize) + CuChar.Speed) \ CuChar.ImageSize + 2

                    CurrentCharTileY(0) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize
                    CurrentCharTileY(1) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize + 1
                    CurrentCharTileY(2) = (CuChar.PointToDraw.Y - CuChar.ImageSize) \ CuChar.ImageSize + 2

                    Try
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(0)).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(1)).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(2)).Visitable = False Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try

                End If


                Return WC
            End Function

            Public Shared Function WouldCollideOnTop(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WC As Boolean = False

                If CuChar.PointToDraw.X Mod CuChar.ImageSize = 0 Then
                    'Auch hier den 32 32 nullpunkt beachten
                    Dim CurrentCharTilesX(1) As Integer
                    Dim CurrentCharTileY As Integer


                    CurrentCharTilesX(0) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize
                    CurrentCharTilesX(1) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize + 1

                    CurrentCharTileY = ((CuChar.PointToDraw.Y - CuChar.ImageSize) - CuChar.Speed) \ CuChar.ImageSize

                    Try
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(0), CurrentCharTileY).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(1), CurrentCharTileY).Visitable = False Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try
                Else
                    Dim CurrentCharTilesX(2) As Integer
                    Dim CurrentCharTileY As Integer


                    CurrentCharTilesX(0) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize
                    CurrentCharTilesX(1) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize + 1
                    CurrentCharTilesX(2) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize + 2

                    CurrentCharTileY = ((CuChar.PointToDraw.Y - CuChar.ImageSize) - CuChar.Speed) \ CuChar.ImageSize

                    Try
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(0), CurrentCharTileY).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(1), CurrentCharTileY).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(2), CurrentCharTileY).Visitable = False Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try
                End If


                Return WC
            End Function

            Public Shared Function WouldCollideOnBottom(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WC As Boolean = False


                If CuChar.PointToDraw.X Mod CuChar.ImageSize = 0 Then
                    'Auch hier den 32 32 nullpunkt beachten
                    Dim CurrentCharTilesX(1) As Integer
                    Dim CurrentCharTileY As Integer


                    CurrentCharTilesX(0) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize
                    CurrentCharTilesX(1) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize + 1

                    CurrentCharTileY = ((CuChar.PointToDraw.Y - CuChar.ImageSize) + CuChar.Speed) \ CuChar.ImageSize + 2

                    Try
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(0), CurrentCharTileY).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(1), CurrentCharTileY).Visitable = False Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try
                Else
                    Dim CurrentCharTilesX(2) As Integer
                    Dim CurrentCharTileY As Integer


                    CurrentCharTilesX(0) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize
                    CurrentCharTilesX(1) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize + 1
                    CurrentCharTilesX(2) = (CuChar.PointToDraw.X - CuChar.ImageSize) \ CuChar.ImageSize + 2

                    CurrentCharTileY = ((CuChar.PointToDraw.Y - CuChar.ImageSize) + CuChar.Speed) \ CuChar.ImageSize + 2

                    Try
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(0), CurrentCharTileY).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(1), CurrentCharTileY).Visitable = False Then WC = True
                        If CuChar.Container.Map.GetTileOf(CurrentCharTilesX(2), CurrentCharTileY).Visitable = False Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try
                End If

                Return WC
            End Function
        End Class

        Public Enum WeaponSlot
            Primary
            Secondary
        End Enum

        Public Interface ICharacter
            ReadOnly Property Health As Integer
            Property Armor As Integer
            ReadOnly Property Name As String
            Function GetDamage(ByVal OriginalDamage As Integer)
            Sub Move(ByVal MovingOnX As Integer, ByVal MovingOnY As Integer)
            ReadOnly Property Gender As Gender
            ReadOnly Property Speed As Integer
            ReadOnly Property PointToDraw As Point
            ReadOnly Property GunPointToDraw As Point
            Sub AddShot(ByVal ShotIncrease As Double, ByVal Slot As WeaponSlot, ByVal MouseLocation As Point)
            Sub UpdateEntitys()
            Sub MoveShots()
            ReadOnly Property ImageSize As Integer
            Property MiddlePointDrawedOnScreen As Point
            ReadOnly Property PointDrawedOnScreen As Point
            ReadOnly Property _DirectXTexturePath As String
            Property Rotation As Double
            Property TakeScreen As Boolean
            Property Score As Integer
            Property Sneaking As Boolean
            Property ID As String
            Sub Transform()
            ReadOnly Property TextureLocation As Point
            ReadOnly Property TextureSize As Size
            ReadOnly Property MainTextureSize As Size
            ReadOnly Property Container As Container
            ReadOnly Property TextureID As String
            Property Standing As Boolean
            Property Direction As CharCollection.Direction
            ReadOnly Property ShadowTexture As Rectangle
            Property SlimeBall As Weapon.ISlimeBall
            Property Laser As Weapon.ILaser
            Property Punch As Weapon.IPunch
            Property AdvancedPunch As Weapon.IAdvancedPunch
        End Interface

        Public Enum Gender
            Women
            Men
        End Enum

        Public Enum Facing
            North
            NorthWest
            NorthEast
            West
            South
            SouthWest
            SouthEast
            East
        End Enum

        Namespace CharCollection
            'Class Rick
            '    Implements ICharacter

            '    Dim NetworkID As String

            '    Dim XMLDoc As XDocument = XDocument.Load("Content/Config/Main.xml")

            '    Dim StandartArmor As Integer = XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<StandartArmor>.Value
            '    Dim AddedArmor As Integer = 0

            '    Dim HealthPoints As Integer = XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<HealthPoints>.Value

            '    Dim IMGSize As Integer

            '    Dim CurrentFacing As Facing = Character.Facing.North

            '    Dim Location(2) As Integer '(1) = x Achse (2) = y Achse

            '    Dim Gun As Weapons.IGun = Nothing
            '    Dim CurrentShots As New List(Of Weapons.IShot)
            '    Dim ShotRemoveQueue As New List(Of Weapons.IShot)
            '    Dim MobRemoveQueue As New Dictionary(Of Mob.IMob, Mob.ISwarm)
            '    Dim ShotQueue As New List(Of Weapons.IShot)
            '    Dim TempShot As Weapons.IShot

            '    Dim Container As Container

            '    Dim CurrentMap As Map

            '    Dim SavedScore As Integer

            '    Dim Sneak As Boolean

            '    Dim PointDrawed As Point
            '    Dim CurrentRotation As Double = 0

            '    Dim ScreenNeeded As Boolean = False

            '    Sub New(ByVal StartLocationX As Integer, ByVal StartLocationY As Integer, ByVal ImageSize As Integer, ByVal _Container As Container, ByVal _ID As String)
            '        Location(1) = StartLocationX
            '        Location(2) = StartLocationY
            '        IMGSize = ImageSize

            '        NetworkID = _ID
            '        Container = _Container
            '    End Sub

            '    Public Property Armor As Integer Implements ICharacter.Armor
            '        Get
            '            Return StandartArmor + AddedArmor
            '        End Get
            '        Set(ByVal value As Integer)
            '            AddedArmor = value
            '        End Set
            '    End Property

            '    Public Function GetDamage(ByVal OriginalDamage As Integer) As Object Implements ICharacter.GetDamage
            '        Dim Damage2Get As Integer

            '        Damage2Get = OriginalDamage * (Armor / 100)

            '        HealthPoints -= Damage2Get

            '        Return Damage2Get
            '    End Function

            '    Public ReadOnly Property Health As Integer Implements ICharacter.Health
            '        Get
            '            Return HealthPoints
            '        End Get
            '    End Property

            '    Public Sub Move(ByVal MovingOnX As Integer, ByVal MovingOnY As Integer) Implements ICharacter.Move
            '        'Aus irgendwelchen gründen ist der char nullpunkt bei 32,32 :)
            '        'Bitte beachten
            '        If MovingOnX > 0 And MoveChecker.WouldGoOutOfMapByMovingRight(Me) = False And MoveChecker.WouldCollideOnRight(Me) = False Then
            '            Location(1) += MovingOnX
            '        ElseIf MovingOnX > 0 And MoveChecker.WouldCollideOnRight(Me) = True Then
            '            Location(1) = (Math.Round((PointToDraw.X - ImageSize) / ImageSize + 1, 0)) * ImageSize
            '        ElseIf MovingOnX > 0 And MoveChecker.WouldGoOutOfMapByMovingRight(Me) = True Then
            '            Location(1) = IMGSize * (CurrentMap.MapSize.Width - 1)
            '        End If

            '        If MovingOnX < 0 And MoveChecker.WouldGoOutOfMapByMovingLeft(Me) = False And MoveChecker.WouldCollideOnLeft(Me) = False Then
            '            Location(1) += MovingOnX
            '        ElseIf MovingOnX < 0 And MoveChecker.WouldCollideOnLeft(Me) = True Then
            '            Location(1) = (Math.Round((PointToDraw.X - ImageSize) / ImageSize + 1, 0)) * ImageSize
            '        ElseIf MovingOnX < 0 And MoveChecker.WouldGoOutOfMapByMovingLeft(Me) = True Then
            '            Location(1) = IMGSize
            '        End If

            '        If MovingOnY < 0 And MoveChecker.WouldGoOutOfMapByMovingTop(Me) = False And MoveChecker.WouldCollideOnTop(Me) = False Then
            '            Location(2) += MovingOnY
            '        ElseIf MovingOnY < 0 And MoveChecker.WouldCollideOnTop(Me) = True Then
            '            Location(2) = (Math.Round((PointToDraw.Y - ImageSize) / ImageSize + 1, 0)) * ImageSize
            '        ElseIf MovingOnY < 0 And MoveChecker.WouldGoOutOfMapByMovingTop(Me) = True Then
            '            Location(2) = ImageSize
            '        End If

            '        If MovingOnY > 0 And MoveChecker.WouldGoOutOfMapByMovingBottom(Me) = False And MoveChecker.WouldCollideOnBottom(Me) = False Then
            '            Location(2) += MovingOnY
            '        ElseIf MovingOnY > 0 And MoveChecker.WouldCollideOnBottom(Me) = True Then
            '            Location(2) = (Math.Round((PointToDraw.Y - ImageSize) / ImageSize + 1, 0)) * ImageSize
            '        ElseIf MovingOnY > 0 And MoveChecker.WouldGoOutOfMapByMovingBottom(Me) = True Then
            '            Location(2) = IMGSize * (CurrentMap.MapSize.Height - 1)
            '        End If
            '    End Sub

            '    Public ReadOnly Property Name As String Implements ICharacter.Name
            '        Get
            '            Return XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<Name>.Value.ToString
            '        End Get
            '    End Property

            '    Public ReadOnly Property Gender As Gender Implements ICharacter.Gender
            '        Get
            '            Return Character.Gender.Men
            '        End Get
            '    End Property

            '    Public ReadOnly Property Speed As Integer Implements ICharacter.Speed
            '        Get
            '            Select Case Sneaking
            '                Case True
            '                    Return XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<Speed>.@Sneaking
            '                Case False
            '                    Return XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<Speed>.Value
            '            End Select

            '        End Get
            '    End Property

            '    Public ReadOnly Property PointToDraw As Point Implements ICharacter.PointToDraw
            '        Get
            '            Return New Point(Location(0), Location(1))
            '        End Get
            '    End Property

            '    Public ReadOnly Property GunPointToDraw As Point Implements ICharacter.GunPointToDraw
            '        Get
            '            Select Case CurrentFacing
            '                Case Character.Facing.North
            '                    Return New Point(PointToDraw.X, PointToDraw.Y - IMGSize * 0.5)
            '                Case Character.Facing.South
            '                    Return New Point(PointToDraw.X, PointToDraw.Y + IMGSize * 0.5)
            '                Case Character.Facing.East
            '                    Return New Point(PointToDraw.X + IMGSize * 0.5, PointToDraw.Y)
            '                Case Character.Facing.West
            '                    Return New Point(PointToDraw.X - IMGSize * 0.5, PointToDraw.Y)
            '                Case Else 'Character.Facing.NorthEast
            '                    Return New Point(PointToDraw.X, PointToDraw.Y)
            '            End Select
            '        End Get
            '    End Property

            '    Public Sub UpdateEntitys() Implements ICharacter.UpdateEntitys
            '        For Each Shot In ShotQueue
            '            CurrentShots.Add(Shot)
            '        Next
            '        For Each Shot In ShotRemoveQueue
            '            CurrentShots.Remove(Shot)
            '        Next
            '        For i = 0 To MobRemoveQueue.Count - 1
            '            MobRemoveQueue.Item(MobRemoveQueue.ElementAt(i).Key).MobList.Remove(MobRemoveQueue.ElementAt(i).Key)
            '        Next
            '        ShotQueue.Clear()
            '        ShotRemoveQueue.Clear()
            '        MobRemoveQueue.Clear()
            '    End Sub

            '    Public Sub AddShot(ByVal ShotIncrease As Double, ByVal Slot As WeaponSlot) Implements ICharacter.AddShot

            '        TempShot = Gun.SummonProjectile(ShotIncrease)
            '        If Not TempShot Is Nothing Then
            '            ShotQueue.Add(TempShot)
            '        End If
            '    End Sub

            '    Public Sub MoveShots() Implements ICharacter.MoveShots
            '        For Each Shot In CurrentShots
            '            If Weapons.GunMath.ShotIsOutOfRange(Shot, CurrentMap, ImageSize) = False And Shot.IsAlive = True Then
            '                Shot.Move()
            '                For Each Swarm As Mob.ISwarm In Container.MobContainer.List
            '                    For Each Spawn As Mob.ISpawn In Swarm.SpawnList
            '                        If Spawn.Collides(Shot) = True Then
            '                            ShotRemoveQueue.Add(Shot)
            '                            Spawn.GetDamage(Shot.Damage)
            '                            Score += Spawn.ScorePoints
            '                        End If
            '                    Next
            '                    For Each Mob As Mob.IMob In Swarm.MobList
            '                        If Mob.Collides(Shot) = True And Not MobRemoveQueue.ContainsKey(Mob) Then
            '                            ShotRemoveQueue.Add(Shot)
            '                            If Mob.GetDamage(Shot.Damage) = False Then
            '                                MobRemoveQueue.Add(Mob, Swarm)
            '                                Score += Mob.ScorePoints
            '                            End If
            '                        End If
            '                    Next
            '                Next
            '                For Each Overlay As Overlay.Overlay In Container.OverlayContainer.Container
            '                    If Overlay.CheckCollusion(Shot.PointToDraw) = True Then
            '                        ShotRemoveQueue.Add(Shot)
            '                        Overlay.Damage += Shot.Damage
            '                    End If
            '                Next
            '            ElseIf Shot.IsAlive = False And Weapons.GunMath.ShotIsOutOfRange(Shot, CurrentMap, ImageSize) = False Then
            '                ShotRemoveQueue.Add(Shot)
            '                CurrentMap.DoDamagaToTile(Shot.DeathTile.X, Shot.DeathTile.Y, Shot.Damage)
            '            Else
            '                ShotRemoveQueue.Add(Shot)
            '            End If
            '        Next
            '    End Sub

            '    Public ReadOnly Property ImageSize As Integer Implements ICharacter.ImageSize
            '        Get
            '            Return IMGSize
            '        End Get
            '    End Property

            '    Public Property MiddlePointDrawedOnScreen As Point Implements ICharacter.MiddlePointDrawedOnScreen
            '        Get
            '            Return PointDrawed
            '        End Get
            '        Set(ByVal value As Point)
            '            PointDrawed = New Point(value.X, _
            '                             value.Y)
            '        End Set
            '    End Property

            '    Public ReadOnly Property PointDrawedOnScreen As Point Implements ICharacter.PointDrawedOnScreen
            '        Get

            '            Return New Point(MiddlePointDrawedOnScreen.X - IMGSize, _
            '                    MiddlePointDrawedOnScreen.Y - 0.5 * IMGSize)

            '        End Get
            '    End Property

            '    Public ReadOnly Property _DirectXTexturePath As String Implements ICharacter._DirectXTexturePath
            '        Get
            '            Return XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<Resource>.Value.ToString
            '        End Get
            '    End Property

            '    Public Property Rotation As Double Implements ICharacter.Rotation
            '        Get
            '            Return CurrentRotation
            '        End Get
            '        Set(ByVal value As Double)
            '            CurrentRotation = value
            '        End Set
            '    End Property

            '    Public Property TakeScreen As Boolean Implements ICharacter.TakeScreen
            '        Get
            '            Return ScreenNeeded
            '        End Get
            '        Set(ByVal value As Boolean)
            '            ScreenNeeded = value
            '        End Set
            '    End Property

            '    Public Property Score As Integer Implements ICharacter.Score
            '        Get
            '            Return SavedScore
            '        End Get
            '        Set(ByVal value As Integer)
            '            SavedScore = value
            '        End Set
            '    End Property

            '    Public Property Sneaking As Boolean Implements ICharacter.Sneaking
            '        Get
            '            Return Sneak
            '        End Get
            '        Set(ByVal value As Boolean)
            '            Sneak = value
            '        End Set
            '    End Property

            '    Public Property ID As String Implements ICharacter.ID
            '        Get
            '            Return NetworkID
            '        End Get
            '        Set(value As String)
            '            NetworkID = value
            '        End Set
            '    End Property

            '    Public Sub Transform() Implements ICharacter.Transform

            '    End Sub

            '    Public ReadOnly Property TextureLocation As System.Drawing.Point Implements ICharacter.TextureLocation
            '        Get

            '        End Get
            '    End Property

            '    Public ReadOnly Property TextureSize As System.Drawing.Size Implements ICharacter.TextureSize
            '        Get

            '        End Get
            '    End Property

            '    Public ReadOnly Property MainTextureSize As System.Drawing.Size Implements ICharacter.MainTextureSize
            '        Get

            '        End Get
            '    End Property

            '    Public ReadOnly Property _Container As Container Implements ICharacter.Container
            '        Get
            '            Return Container
            '        End Get
            '    End Property

            '    Public ReadOnly Property TextureID As String Implements ICharacter.TextureID
            '        Get

            '        End Get
            '    End Property

            '    Public Property Standing As Boolean Implements ICharacter.Standing

            '    Public Property Direction As Direction Implements ICharacter.Direction

            '    Public ReadOnly Property ShadowTexture As System.Drawing.Rectangle Implements ICharacter.ShadowTexture
            '        Get

            '        End Get
            '    End Property

            '    Public Property AdvancedPunch As Weapon.IAdvancedPunch Implements ICharacter.AdvancedPunch

            '    Public Property Laser As Weapon.ILaser Implements ICharacter.Laser

            '    Public Property Punch As Weapon.IPunch Implements ICharacter.Punch

            '    Public Property SlimeBall As Weapon.ISlimeBall Implements ICharacter.SlimeBall
            'End Class

            Public Enum Direction
                Left
                Bottom
                Right
                Top
                BottomStanding
                TopStanding
            End Enum

            Enum Transformation
                Human
                Slime
                Transform
            End Enum

            Public Class DrSlime
                Implements ICharacter

                Dim Config As XDocument = XDocument.Load("Content/Character/Dr.Slime/Character.xml")

                Dim Container As Container
                Dim AddedArmor As Integer
                Dim StandartArmor As Integer = Config.<Character>.<Data>.<Human>.@Armor

                Dim Weapon As Weapons.IGun = New Weapons.GunCollection.DuX_127_S(Me)
                Dim Health As Integer

                Dim Location As Point
                Dim SeenPoint As Point

                Dim ID As String
                Dim ImageSize As Integer
                Dim Score As Integer
                Dim Sneaking As Boolean

                Dim SizeIndex As New Dictionary(Of Integer, Size) 'Jedem Index wird eine Größe zugeordnet
                Dim PositionIndex As New Dictionary(Of Integer, Point) 'Jedem Index wird ein Punkt zugeordnet
                Dim CountIndex As New Dictionary(Of String, Integer)
                Dim StartIndexIndex As New Dictionary(Of String, Integer)
                Dim TransformationIndex As New Dictionary(Of String, Transformation) 'Jedem Mode eine Transformation
                Dim DirectionIndex As New Dictionary(Of String, Direction) 'Jeder Richting wird ein Mode zugeordnet
                Dim TextureChangingIndex As New Dictionary(Of String, Integer) 'Jedem Mode wird ein Intervall, in welchem sich die Texturen ändern zugeordnet
                Dim HitBoxIndex As New Dictionary(Of String, Size)
                Dim Modes As New List(Of String)

                Dim Transformation As Transformation = CharCollection.Transformation.Human
                Dim Direction As Direction

                Dim CurrentTexture As Integer = 0
                Dim WithEvents TextureChanger As New System.Windows.Forms.Timer

                Dim CurrentMode As String

                Dim SlimeBall As Weapon.ISlimeBall
                Dim Laser As Weapon.ILaser
                Dim Punch As Weapon.IPunch
                Dim AdvancedPunch As Weapon.IAdvancedPunch

                Sub New(ByRef _Container As Container, ByVal _ImageSize As Integer, ByVal _ID As String)
                    Writer.Log.Write("Spawned Character")

                    Container = _Container
                    ID = _ID
                    ImageSize = _ImageSize

                    For Each Resource In Config.<Character>.<Resources>.Elements
                        Modes.Add(Resource.@Mode)
                        Select Case Convert.ToBoolean(Resource.@Sprite)
                            Case True
                                StartIndexIndex.Add(Resource.@Mode, CurrentTexture)
                                CountIndex.Add(Resource.@Mode, Config.<Character>.Elements(Resource.@SpritesheetElement).Elements.Count)
                                For Each Sprite In Config.<Character>.Elements(Resource.@SpritesheetElement).Elements
                                    SizeIndex.Add(CurrentTexture, New Size(Config.<Character>.Elements(Resource.@SpritesheetElement).@Width, Config.<Character>.Elements(Resource.@SpritesheetElement).@Height))
                                    PositionIndex.Add(CurrentTexture, New Size(Sprite.@X, Sprite.@Y))
                                    CurrentTexture += 1
                                Next
                                TextureChangingIndex.Add(Resource.@Mode, Resource.@TextureChangingIntervall)
                            Case False
                                StartIndexIndex.Add(Resource.@Mode, CurrentTexture)
                                SizeIndex.Add(CurrentTexture, New Size(Resource.@Width, Resource.@Height))
                                PositionIndex.Add(CurrentTexture, New Size(Resource.@X, Resource.@Y))

                                CountIndex.Add(Resource.@Mode, 1)
                                CurrentTexture += 1
                        End Select
                        Select Case Resource.@Transform.ToUpper
                            Case "HUMAN"
                                TransformationIndex.Add(Resource.@Mode, CharCollection.Transformation.Human)
                                Select Case Resource.@Direction.ToUpper
                                    Case "L"
                                        DirectionIndex.Add(Resource.@Mode, Direction.Left)
                                    Case "R"
                                        DirectionIndex.Add(Resource.@Mode, Direction.Right)
                                    Case "T"
                                        DirectionIndex.Add(Resource.@Mode, Direction.Top)
                                    Case "B"
                                        DirectionIndex.Add(Resource.@Mode, Direction.Bottom)
                                    Case "ST"
                                        DirectionIndex.Add(Resource.@Mode, Direction.TopStanding)
                                    Case "SB"
                                        DirectionIndex.Add(Resource.@Mode, Direction.BottomStanding)
                                End Select
                            Case "SLIME"
                                TransformationIndex.Add(Resource.@Mode, CharCollection.Transformation.Slime)
                                Select Case Resource.@Direction.ToUpper
                                    Case "L"
                                        DirectionIndex.Add(Resource.@Mode, Direction.Left)
                                    Case "R"
                                        DirectionIndex.Add(Resource.@Mode, Direction.Right)
                                    Case "T"
                                        DirectionIndex.Add(Resource.@Mode, Direction.Top)
                                    Case "B"
                                        DirectionIndex.Add(Resource.@Mode, Direction.Bottom)
                                    Case "ST"
                                        DirectionIndex.Add(Resource.@Mode, Direction.TopStanding)
                                    Case "SB"
                                        DirectionIndex.Add(Resource.@Mode, Direction.BottomStanding)
                                End Select
                            Case "TRANSFORM"
                                TransformationIndex.Add(Resource.@Mode, CharCollection.Transformation.Transform)
                        End Select
                    Next

                    For Each HitBox In Config.<Character>.<HitBox>.Elements
                        HitBoxIndex.Add(HitBox.@Mode, New Size(HitBox.@Width, HitBox.@Height))
                    Next

                    CurrentTexture = 0
                    CurrentMode = Modes(0)
                    Me.Location = Container.Map.Spawn
                End Sub

                Public ReadOnly Property _DirectXTexturePath As String Implements ICharacter._DirectXTexturePath
                    Get
                        Return Config.<Character>.<Resources>.@Image
                    End Get
                End Property

                Public Sub AddShot(ShotIncrease As Double, ByVal Slot As WeaponSlot, ByVal MouseLocation As Point) Implements ICharacter.AddShot
                    Select Case Transformation
                        Case CharCollection.Transformation.Human
                            Select Case Slot

                            End Select
                        Case CharCollection.Transformation.Slime
                            Select Case Slot
                                Case WeaponSlot.Primary
                                    Dim CurrentSlimeBall As Weapon.SlimeBall.SlimeBall = SlimeBall.Snot(ShotIncrease, MouseLocation)
                                    If Not IsNothing(CurrentSlimeBall) Then
                                        Container.SnotContainer.Add(CurrentSlimeBall)
                                    End If
                                Case WeaponSlot.Secondary
                                    Select Case TrackMath.IsLeft(MiddlePointDrawedOnScreen.X, MouseLocation.X)
                                        Case True
                                            Select Case ShotIncrease
                                                Case Is > 1
                                                    Container.LaserContainer.Add(Laser.Laser(Game.Weapon.Direction.Bottom, MouseLocation))
                                                Case Is > -1
                                                    Container.LaserContainer.Add(Laser.Laser(Game.Weapon.Direction.Left, MouseLocation))
                                                Case Else
                                                    Container.LaserContainer.Add(Laser.Laser(Game.Weapon.Direction.Top, MouseLocation))
                                            End Select
                                        Case False
                                            Select Case ShotIncrease
                                                Case Is > 1
                                                    Container.LaserContainer.Add(Laser.Laser(Game.Weapon.Direction.Top, MouseLocation))
                                                Case Is > -1
                                                    Container.LaserContainer.Add(Laser.Laser(Game.Weapon.Direction.Right, MouseLocation))
                                                Case Else
                                                    Container.LaserContainer.Add(Laser.Laser(Game.Weapon.Direction.Bottom, MouseLocation))
                                            End Select
                                    End Select
                            End Select
                        Case CharCollection.Transformation.Transform

                    End Select
                End Sub

                Public Property Armor As Integer Implements ICharacter.Armor
                    Get
                        Return StandartArmor + AddedArmor
                    End Get
                    Set(value As Integer)
                        AddedArmor = value
                    End Set
                End Property

                Public ReadOnly Property Gender As Gender Implements ICharacter.Gender
                    Get
                        Return Character.Gender.Men
                    End Get
                End Property

                Public Function GetDamage(OriginalDamage As Integer) As Object Implements ICharacter.GetDamage
                    Health -= OriginalDamage * (100 / Armor)

                    Writer.Log.Write("Received " + OriginalDamage + " Damage @" + Me.ID)

                    If Health <= 0 Then Return False
                    Return True
                End Function

                Public ReadOnly Property GunPointToDraw As System.Drawing.Point Implements ICharacter.GunPointToDraw
                    Get

                    End Get
                End Property

                Public ReadOnly Property _Health As Integer Implements ICharacter.Health
                    Get
                        Return Health
                    End Get
                End Property


                Public Property _ID As String Implements ICharacter.ID
                    Get
                        Return ID
                    End Get
                    Set(value As String)
                        ID = value
                    End Set
                End Property

                Public ReadOnly Property _ImageSize As Integer Implements ICharacter.ImageSize
                    Get
                        Return ImageSize
                    End Get
                End Property

                Public Property MiddlePointDrawedOnScreen As System.Drawing.Point Implements ICharacter.MiddlePointDrawedOnScreen
                    Get
                        Return SeenPoint
                    End Get
                    Set(value As System.Drawing.Point)
                        SeenPoint = value
                    End Set
                End Property

                Private Function WouldLeave(ByVal Direction As Direction) As Boolean
                    Select Case Direction
                        Case CharCollection.Direction.Top
                            If Me.Location.Y - Me.Speed < 0 Then Return True
                        Case CharCollection.Direction.Bottom
                            If Me.Location.Y + Me.Speed + Me.HitBoxIndex(CurrentMode).Height * Container.ImageScale > Container.Map.UnIndexedMapSize.Height * ImageSize Then Return True
                        Case CharCollection.Direction.Left
                            If Me.Location.X - Me.Speed < 0 Then Return True
                        Case CharCollection.Direction.Right
                            If Me.Location.X + Me.Speed + Me.HitBoxIndex(CurrentMode).Width * Container.ImageScale > Container.Map.UnIndexedMapSize.Width * ImageSize Then Return True
                    End Select
                    Return False
                End Function

                Private Function WouldMerge(ByVal Direction As Direction, ByVal Move As Integer) As Boolean
                    Select Case Direction
                        Case CharCollection.Direction.Left
                            For x = Me.Location.X \ ImageSize To (Me.Location.X + Move) \ ImageSize Step -1
                                If x >= 0 And x <= Container.Map.MapSize.Width Then
                                    If Not (Me.Location.Y + HitBoxIndex(CurrentMode).Height * Container.ImageScale) Mod ImageSize = 0 Then
                                        For y = Me.Location.Y \ ImageSize To (Me.Location.Y + HitBoxIndex(CurrentMode).Height * Container.ImageScale) \ ImageSize Step 1
                                            If Container.Map.GetTileOf(x, y).Visitable = False Then Return True
                                        Next
                                    Else
                                        For y = Me.Location.Y \ ImageSize To (Me.Location.Y + HitBoxIndex(CurrentMode).Height * Container.ImageScale - 1) \ ImageSize Step 1
                                            If Container.Map.GetTileOf(x, y).Visitable = False Then Return True
                                        Next
                                    End If
                                End If
                            Next
                        Case CharCollection.Direction.Right
                            For x = Me.Location.X \ ImageSize To (Me.Location.X + Move + HitBoxIndex(CurrentMode).Width * Container.ImageScale) \ ImageSize Step 1
                                If x >= 0 And x <= Container.Map.MapSize.Width Then
                                    If Not (Me.Location.Y + HitBoxIndex(CurrentMode).Height * Container.ImageScale) Mod ImageSize = 0 Then
                                        For y = Me.Location.Y \ ImageSize To (Me.Location.Y + HitBoxIndex(CurrentMode).Height * Container.ImageScale) \ ImageSize Step 1
                                            If Container.Map.GetTileOf(x, y).Visitable = False Then Return True
                                        Next
                                    Else
                                        For y = Me.Location.Y \ ImageSize To (Me.Location.Y + HitBoxIndex(CurrentMode).Height * Container.ImageScale - 1) \ ImageSize Step 1
                                            If Container.Map.GetTileOf(x, y).Visitable = False Then Return True
                                        Next
                                    End If
                                End If
                            Next
                        Case CharCollection.Direction.Top
                            For y = Me.Location.Y \ ImageSize To (Me.Location.Y + Move) \ ImageSize Step -1
                                If y >= 0 And y <= Container.Map.MapSize.Height Then
                                    If Not (Me.Location.X + HitBoxIndex(CurrentMode).Width * Container.ImageScale) Mod ImageSize = 0 Then
                                        For x = Me.Location.X \ ImageSize To (Me.Location.X + HitBoxIndex(CurrentMode).Width * Container.ImageScale) \ ImageSize
                                            If x >= 0 And x <= Container.Map.MapSize.Width Then
                                                If Container.Map.GetTileOf(x, y).Visitable = False Then Return True
                                            End If
                                        Next
                                    Else
                                        For x = Me.Location.X \ ImageSize To (Me.Location.X + HitBoxIndex(CurrentMode).Width * Container.ImageScale - 1) \ ImageSize
                                            If x >= 0 And x <= Container.Map.MapSize.Width Then
                                                If Container.Map.GetTileOf(x, y).Visitable = False Then Return True
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        Case CharCollection.Direction.Bottom
                            For y = (Me.Location.Y + HitBoxIndex(CurrentMode).Height * Container.ImageScale) \ ImageSize To (Me.Location.Y + Move + HitBoxIndex(CurrentMode).Height * Container.ImageScale) \ ImageSize Step 1
                                If y >= 0 And y <= Container.Map.MapSize.Height Then
                                    If Not (Me.Location.X + HitBoxIndex(CurrentMode).Width * Container.ImageScale) Mod ImageSize = 0 Then
                                        For x = Me.Location.X \ ImageSize To (Me.Location.X + HitBoxIndex(CurrentMode).Width * Container.ImageScale) \ ImageSize
                                            If x >= 0 And x <= Container.Map.MapSize.Width Then
                                                If Container.Map.GetTileOf(x, y).Visitable = False Then Return True
                                            End If
                                        Next
                                    Else
                                        For x = Me.Location.X \ ImageSize To (Me.Location.X + HitBoxIndex(CurrentMode).Width * Container.ImageScale) \ ImageSize
                                            If x >= 0 And x <= Container.Map.MapSize.Width Then
                                                If Container.Map.GetTileOf(x, y).Visitable = False Then Return True
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                    End Select
                End Function

                Public Sub Move(MovingOnX As Integer, MovingOnY As Integer) Implements ICharacter.Move
                    Select Case MovingOnX
                        Case Is = 0
                            Select Case MovingOnY
                                Case Is > 0
                                    Direction = CharCollection.Direction.Bottom
                                    If Not WouldLeave(CharCollection.Direction.Bottom) = True Then
                                        If Not WouldMerge(CharCollection.Direction.Bottom, MovingOnY) = True Then
                                            Location.Y += MovingOnY
                                        Else
                                            Location.Y = ((Location.Y + MovingOnY + (HitBoxIndex(CurrentMode).Height * Container.ImageScale)) \ ImageSize) * ImageSize - HitBoxIndex(CurrentMode).Height * Container.ImageScale
                                        End If
                                    Else
                                        Location.Y = Container.Map.UnIndexedMapSize.Height * ImageSize - HitBoxIndex(CurrentMode).Height * Container.ImageScale
                                    End If
                                Case Is < 0
                                    Direction = CharCollection.Direction.Top
                                    If Not WouldLeave(CharCollection.Direction.Top) = True Then
                                        If Not WouldMerge(CharCollection.Direction.Top, MovingOnY) = True Then
                                            Location.Y += MovingOnY
                                        Else
                                            Location.Y = (Location.Y \ ImageSize) * ImageSize
                                        End If
                                    Else
                                        Location.Y = 0
                                    End If
                            End Select
                        Case Is < 0
                            Direction = CharCollection.Direction.Left
                            If Not WouldLeave(CharCollection.Direction.Left) = True Then
                                If Not WouldMerge(CharCollection.Direction.Left, MovingOnX) = True Then
                                    Location.X += MovingOnX
                                Else
                                    Location.X = (Location.X \ ImageSize) * ImageSize
                                End If
                            Else
                                Location.X = 0
                            End If
                        Case Is > 0
                            Direction = CharCollection.Direction.Right
                            If Not WouldLeave(CharCollection.Direction.Right) = True Then
                                If Not WouldMerge(CharCollection.Direction.Right, MovingOnX) = True Then
                                    Location.X += MovingOnX
                                Else
                                    Location.X = ((Location.X + MovingOnX + HitBoxIndex(CurrentMode).Width * Container.ImageScale) \ ImageSize) * ImageSize - HitBoxIndex(CurrentMode).Width * Container.ImageScale
                                End If
                            Else
                                Location.X = Container.Map.UnIndexedMapSize.Width * ImageSize - HitBoxIndex(CurrentMode).Width * Container.ImageScale
                            End If
                    End Select
                    For Each Mode As String In Modes
                        If DirectionIndex.ContainsKey(Mode) And TransformationIndex.ContainsKey(Mode) Then
                            If DirectionIndex(Mode) = Direction And TransformationIndex(Mode) = Transformation Then
                                CurrentMode = Mode
                                Exit For
                            End If
                        End If
                    Next
                    If TextureChangingIndex.ContainsKey(CurrentMode) Then
                        TextureChanger.Interval = TextureChangingIndex(CurrentMode)
                        If CurrentTexture >= StartIndexIndex(CurrentMode) And CurrentTexture < StartIndexIndex(CurrentMode) + CountIndex(CurrentMode) Then
                        Else
                            CurrentTexture = StartIndexIndex(CurrentMode)
                        End If
                        If Not TextureChanger.Enabled = True Then
                            TextureChanger.Enabled = True
                        End If
                    Else
                        TextureChanger.Enabled = False
                        CurrentTexture = StartIndexIndex(CurrentMode)
                    End If
                End Sub

                Public Sub MoveShots() Implements ICharacter.MoveShots
                    For Each SlimeBall As Weapon.SlimeBall.SlimeBall In Container.SnotContainer
                        SlimeBall.Move()
                        If SlimeBall.Alive = False Then Container.Remove(SlimeBall)
                    Next
                    For Each Laser As Weapon.Laser.Laser In Container.LaserContainer
                        Laser.Move()
                    Next
                    Container.Flush()
                End Sub

                Public ReadOnly Property Name As String Implements ICharacter.Name
                    Get
                        Return Config.<Character>.@Name
                    End Get
                End Property

                Public ReadOnly Property PointDrawedOnScreen As System.Drawing.Point Implements ICharacter.PointDrawedOnScreen
                    Get
                        Return New Point(MiddlePointDrawedOnScreen.X - TextureSize.Width / 2, MiddlePointDrawedOnScreen.Y - TextureSize.Height / 2)
                    End Get
                End Property

                Public ReadOnly Property PointToDraw As System.Drawing.Point Implements ICharacter.PointToDraw
                    Get
                        Return New Point(Location.X + Me.TextureSize.Width * Container.ImageScale / 2, Location.Y + Me.TextureSize.Height * Container.ImageScale / 2)
                    End Get
                End Property

                Public Property Rotation As Double Implements ICharacter.Rotation
                    Get
                        Return 0
                    End Get
                    Set(value As Double)

                    End Set
                End Property

                Public Property _Score As Integer Implements ICharacter.Score
                    Get
                        Return Score
                    End Get
                    Set(value As Integer)
                        Score = value
                    End Set
                End Property

                Public Property _Sneaking As Boolean Implements ICharacter.Sneaking
                    Get
                        Return Sneaking
                    End Get
                    Set(value As Boolean)
                        Sneaking = value
                    End Set
                End Property

                Public ReadOnly Property Speed As Integer Implements ICharacter.Speed
                    Get
                        Select Case Transformation
                            Case CharCollection.Transformation.Human
                                Return Config.<Character>.<Data>.<Human>.@Speed
                            Case CharCollection.Transformation.Slime
                                Return Config.<Character>.<Data>.<Slime>.@Speed
                            Case CharCollection.Transformation.Transform
                                Return 0
                        End Select
                    End Get
                End Property

                Public Property TakeScreen As Boolean Implements ICharacter.TakeScreen

                Public Sub Transform() Implements ICharacter.Transform
                    Select Case Transformation
                        Case CharCollection.Transformation.Human
                            Transformation = CharCollection.Transformation.Slime
                            Move(0, 0)
                        Case CharCollection.Transformation.Slime
                            Transformation = CharCollection.Transformation.Human
                            Move(0, 0)
                    End Select
                End Sub

                Public Sub UpdateEntitys() Implements ICharacter.UpdateEntitys

                End Sub

                Public ReadOnly Property TextureLocation As System.Drawing.Point Implements ICharacter.TextureLocation
                    Get
                        Return PositionIndex(CurrentTexture)
                    End Get
                End Property

                Public ReadOnly Property TextureSize As System.Drawing.Size Implements ICharacter.TextureSize
                    Get
                        Return SizeIndex(CurrentTexture)
                    End Get
                End Property

                Public ReadOnly Property MainTextureSize As System.Drawing.Size Implements ICharacter.MainTextureSize
                    Get
                        Return New Size(Config.<Character>.<Resources>.@Width, Config.<Character>.<Resources>.@Height)
                    End Get
                End Property

                Public ReadOnly Property Container1 As Container Implements ICharacter.Container
                    Get
                        Return Container
                    End Get
                End Property

                Public ReadOnly Property TextureID As String Implements ICharacter.TextureID
                    Get
                        Return Config.<Character>.@ID
                    End Get
                End Property

                Public Property Standing As Boolean Implements ICharacter.Standing
                    Get
                        If Direction = CharCollection.Direction.BottomStanding Or Direction = CharCollection.Direction.TopStanding Then
                            Return True
                        End If
                        Return False
                    End Get
                    Set(value As Boolean)
                        If value = True Then
                            If Direction = CharCollection.Direction.Top Or Direction = CharCollection.Direction.TopStanding Then
                                Direction = CharCollection.Direction.TopStanding
                            Else
                                Direction = CharCollection.Direction.BottomStanding
                            End If
                        End If
                        For Each Mode As String In Modes
                            If DirectionIndex.ContainsKey(Mode) And TransformationIndex.ContainsKey(Mode) Then
                                If DirectionIndex(Mode) = Direction And TransformationIndex(Mode) = Transformation Then
                                    CurrentMode = Mode
                                    Exit For
                                End If
                            End If
                        Next
                        If TextureChangingIndex.ContainsKey(CurrentMode) Then
                            TextureChanger.Interval = TextureChangingIndex(CurrentMode)
                            If Not CurrentTexture > StartIndexIndex(CurrentMode) Then
                                CurrentTexture = StartIndexIndex(CurrentMode)
                            End If
                            If Not TextureChanger.Enabled = True Then
                                TextureChanger.Enabled = True
                            End If
                        Else
                            TextureChanger.Enabled = False
                            CurrentTexture = StartIndexIndex(CurrentMode)
                        End If
                    End Set
                End Property

                Public Property _Direction As Direction Implements ICharacter.Direction
                    Get
                        Return Direction
                    End Get
                    Set(value As Direction)
                        Direction = value
                    End Set
                End Property

                Private Sub TextureChanger_Tick(sender As Object, e As System.EventArgs) Handles TextureChanger.Tick
                    If StartIndexIndex(CurrentMode) + CountIndex(CurrentMode) - 1 > CurrentTexture Then
                        CurrentTexture += 1
                    Else
                        CurrentTexture = StartIndexIndex(CurrentMode)
                    End If
                End Sub

                Public ReadOnly Property ShadowTexture As System.Drawing.Rectangle Implements ICharacter.ShadowTexture
                    Get
                        Select Case Transformation
                            Case CharCollection.Transformation.Human
                                Return New Rectangle(PositionIndex(StartIndexIndex("HumanShadow")), SizeIndex(StartIndexIndex("HumanShadow")))
                            Case CharCollection.Transformation.Slime
                                Return New Rectangle(PositionIndex(StartIndexIndex("SlimeShadow")), SizeIndex(StartIndexIndex("SlimeShadow")))
                        End Select
                    End Get
                End Property

                Public Property _AdvancedPunch As Weapon.IAdvancedPunch Implements ICharacter.AdvancedPunch
                    Get
                        Return AdvancedPunch
                    End Get
                    Set(value As Weapon.IAdvancedPunch)
                        Writer.Log.Write("Changed @" + ID + "'s AdvancedPunch to " + value.ToString)
                        AdvancedPunch = value
                    End Set
                End Property

                Public Property _Laser As Weapon.ILaser Implements ICharacter.Laser
                    Get
                        Return Laser
                    End Get
                    Set(value As Weapon.ILaser)
                        Writer.Log.Write("Changed @" + ID + "'s Laser to " + value.ToString)
                        Laser = value
                    End Set
                End Property

                Public Property _Punch As Weapon.IPunch Implements ICharacter.Punch
                    Get
                        Return Punch
                    End Get
                    Set(value As Weapon.IPunch)
                        Writer.Log.Write("Changed @" + ID + "'s Punch to " + value.ToString)
                        Punch = value
                    End Set
                End Property

                Public Property _SlimeBall As Weapon.ISlimeBall Implements ICharacter.SlimeBall
                    Get
                        Return SlimeBall
                    End Get
                    Set(value As Weapon.ISlimeBall)
                        Writer.Log.Write("Changed @" + ID + "'s Slimeball to " + value.ToString)
                        SlimeBall = value
                    End Set
                End Property
            End Class
        End Namespace
    End Namespace
End Namespace