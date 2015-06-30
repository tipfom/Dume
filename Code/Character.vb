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
    Namespace Character
        Friend Class MoveChecker
            Public Shared Function WouldGoOutOfMapByMovingLeft(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WGOOM As Boolean = False

                If CuChar.PointToDraw.X - CuChar.Speed - 2 * CuChar.ImageSize <= 0 Then WGOOM = True

                Return WGOOM
            End Function

            Public Shared Function WouldGoOutOfMapByMovingRight(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WGOOM As Boolean = False

                If CuChar.PointToDraw.X + CuChar.Speed + 2 * CuChar.ImageSize >= CuChar.ImageSize * CuChar.Map.MapSizeX Then WGOOM = True
               
                Return WGOOM
            End Function

            Public Shared Function WouldGoOutOfMapByMovingTop(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WGOOM As Boolean = False

                If CuChar.PointToDraw.Y - CuChar.Speed - CuChar.ImageSize <= 0 Then WGOOM = True

                Return WGOOM
            End Function

            Public Shared Function WouldGoOutOfMapByMovingBottom(ByVal CuChar As Character.ICharacter) As Boolean
                Dim WGOOM As Boolean = False

                Select Case CuChar.Facing
                    Case Facing.North, Facing.South
                        If CuChar.PointToDraw.Y + CuChar.Speed + CuChar.ImageSize / 2 >= CuChar.ImageSize * (CuChar.Map.MapSizeY - 0.5) Then WGOOM = True
                    Case Facing.East, Facing.West
                        If CuChar.PointToDraw.Y + CuChar.Speed + CuChar.ImageSize >= CuChar.ImageSize * (CuChar.Map.MapSizeY - 1) Then WGOOM = True
                    Case Else 'Da die anderen alle die selbe hitbox haben(selbe wie north auf x)
                        If CuChar.PointToDraw.Y + CuChar.Speed + CuChar.ImageSize >= CuChar.ImageSize * (CuChar.Map.MapSizeY - 1) Then WGOOM = True
                End Select

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
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(0)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(1)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
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
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(0)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(1)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(2)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
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
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(0)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(1)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
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
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(0)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(1)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX, CurrentCharTileY(2)).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
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
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(0), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(1), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
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
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(0), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(1), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(2), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
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
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(0), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(1), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
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
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(0), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(1), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                        If CuChar.Map.GetTileOf(CurrentCharTilesX(2), CurrentCharTileY).Visitable = Tile.TileAttendability.UnVisitable Then WC = True
                    Catch ex As Exception
                        WC = False
                    End Try
                End If

                Return WC
            End Function
        End Class

        Public Interface ICharacter
            ReadOnly Property Health As Integer
            Property Armor As Integer
            ReadOnly Property Name As String
            Function GetDamage(ByVal OriginalDamage As Integer)
            Property Facing As Facing
            ReadOnly Property LocationX As Integer
            ReadOnly Property LocationY As Integer
            Sub Move(ByVal MovingOnX As Integer, ByVal MovingOnY As Integer)
            Event HealthPointsChanged()
            Property CurrentWeapon As Weapons.IGun
            ReadOnly Property Gender As Gender
            Event PositionChanged()
            ReadOnly Property Speed As Integer
            ReadOnly Property PointToDraw As Point
            ReadOnly Property GunPointToDraw As Point
            Sub AddShot(ByVal ShotIncrease As Double)
            Sub UpdateShots()
            Sub MoveShots()
            ReadOnly Property CurrentShotList As List(Of Weapons.IShot)
            Property CurrentTracker As Tracker
            ReadOnly Property RectangleToDraw As Rectangle
            Property CurrentForm As Form
            ReadOnly Property ImageSize As Integer
            Property MiddlePointDrawedOnScreen As Point
            ReadOnly Property PointDrawedOnScreen As Point
            ReadOnly Property Map As Map
            ReadOnly Property _DirectXTexturePath As String
            Property Rotation As Double
            Property TakeScreen As Boolean
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
            Class Rick
                Implements ICharacter

                Dim XMLDoc As XDocument = XDocument.Load("Config/Main.xml")

                Dim StandartArmor As Integer = XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<StandartArmor>.Value
                Dim AddedArmor As Integer = 0

                Dim HealthPoints As Integer = XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<HealthPoints>.Value

                Dim IMGSize As Integer

                Dim CurrentFacing As Facing = Character.Facing.North

                Dim Location(2) As Integer '(1) = x Achse (2) = y Achse

                Public Event HealthPointsChanged() Implements ICharacter.HealthPointsChanged

                Dim Gun As Weapons.IGun = Nothing
                Dim CurrentShots As New List(Of Weapons.IShot)
                Dim ShotRemoveQueue As New List(Of Weapons.IShot)
                Dim ShotQueue As New List(Of Weapons.IShot)
                Dim TempShot As Weapons.IShot

                Dim CurrentTracker As Tracker

                Dim CurrentRectangle As Rectangle
                Dim CurrentFormForm As New Form

                Dim CurrentMap As Map

                Public Event PositionChanged() Implements ICharacter.PositionChanged

                Dim PointDrawed As Point
                Dim CurrentRotation As Double = 0

                Dim ScreenNeeded As Boolean = False

                Sub New(ByVal StartLocationX As Integer, ByVal StartLocationY As Integer, ByVal ImageSize As Integer, ByVal CRectangle As Rectangle, ByVal CMap As Map)
                    Location(1) = StartLocationX
                    Location(2) = StartLocationY
                    IMGSize = ImageSize
                    CurrentMap = CMap
                    CurrentRectangle = CRectangle
                    RaiseEvent PositionChanged()
                End Sub

                Public Property Armor As Integer Implements ICharacter.Armor
                    Get
                        Return StandartArmor + AddedArmor
                    End Get
                    Set(ByVal value As Integer)
                        AddedArmor = value
                    End Set
                End Property

                Public Property Facing As Facing Implements ICharacter.Facing
                    Get
                        Return CurrentFacing
                    End Get
                    Set(ByVal value As Facing)

                        CurrentFacing = value

                    End Set
                End Property

                Public Function GetDamage(ByVal OriginalDamage As Integer) As Object Implements ICharacter.GetDamage
                    Dim Damage2Get As Integer

                    Damage2Get = OriginalDamage * (Armor / 100)

                    HealthPoints -= Damage2Get

                    RaiseEvent HealthPointsChanged()

                    Return Damage2Get
                End Function

                Public ReadOnly Property Health As Integer Implements ICharacter.Health
                    Get
                        Return HealthPoints
                    End Get
                End Property

                Public ReadOnly Property LocationX As Integer Implements ICharacter.LocationX
                    Get
                        Return Location(1)
                    End Get
                End Property

                Public ReadOnly Property LocationY As Integer Implements ICharacter.LocationY
                    Get
                        Return Location(2)
                    End Get
                End Property

                Public Sub Move(ByVal MovingOnX As Integer, ByVal MovingOnY As Integer) Implements ICharacter.Move
                    'Aus irgendwelchen gründen ist der schar nullpunkt bei 32,32 :)
                    'Bitte beachten
                    If MovingOnX > 0 And MoveChecker.WouldGoOutOfMapByMovingRight(Me) = False And MoveChecker.WouldCollideOnRight(Me) = False Then
                        Location(1) += MovingOnX
                    ElseIf MovingOnX > 0 And MoveChecker.WouldCollideOnRight(Me) = True Then
                        Location(1) = (Math.Round((PointToDraw.X - ImageSize) / ImageSize + 1, 0)) * ImageSize
                    ElseIf MovingOnX > 0 And MoveChecker.WouldGoOutOfMapByMovingRight(Me) = True Then
                        Location(1) = IMGSize * (CurrentMap.MapSizeX - 1)
                    End If

                    If MovingOnX < 0 And MoveChecker.WouldGoOutOfMapByMovingLeft(Me) = False And MoveChecker.WouldCollideOnLeft(Me) = False Then
                        Location(1) += MovingOnX
                    ElseIf MovingOnX < 0 And MoveChecker.WouldCollideOnLeft(Me) = True Then
                        Location(1) = (Math.Round((PointToDraw.X - ImageSize) / ImageSize + 1, 0)) * ImageSize
                    ElseIf MovingOnX < 0 And MoveChecker.WouldGoOutOfMapByMovingLeft(Me) = True Then
                        Location(1) = IMGSize
                    End If

                    If MovingOnY < 0 And MoveChecker.WouldGoOutOfMapByMovingTop(Me) = False And MoveChecker.WouldCollideOnTop(Me) = False Then
                        Location(2) += MovingOnY
                    ElseIf MovingOnY < 0 And MoveChecker.WouldCollideOnTop(Me) = True Then
                        Location(2) = (Math.Round((PointToDraw.Y - ImageSize) / ImageSize + 1, 0)) * ImageSize
                    ElseIf MovingOnY < 0 And MoveChecker.WouldGoOutOfMapByMovingTop(Me) = True Then
                        Location(2) = ImageSize
                    End If

                    If MovingOnY > 0 And MoveChecker.WouldGoOutOfMapByMovingBottom(Me) = False And MoveChecker.WouldCollideOnBottom(Me) = False Then
                        Location(2) += MovingOnY
                    ElseIf MovingOnY > 0 And MoveChecker.WouldCollideOnBottom(Me) = True Then
                        Location(2) = (Math.Round((PointToDraw.Y - ImageSize) / ImageSize + 1, 0)) * ImageSize
                    ElseIf MovingOnY > 0 And MoveChecker.WouldGoOutOfMapByMovingBottom(Me) = True Then
                        Location(2) = IMGSize * (CurrentMap.MapSizeY - 1)
                    End If
                    RaiseEvent PositionChanged()
                End Sub

                Public ReadOnly Property Name As String Implements ICharacter.Name
                    Get
                        Return XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<Name>.Value.ToString
                    End Get
                End Property

                Public Property CurrentWeapon As Weapons.IGun Implements ICharacter.CurrentWeapon
                    Get
                        Return Gun
                    End Get
                    Set(ByVal value As Weapons.IGun)
                        Gun = value
                    End Set
                End Property

                Public ReadOnly Property Gender As Gender Implements ICharacter.Gender
                    Get
                        Return Character.Gender.Men
                    End Get
                End Property

                Public ReadOnly Property Speed As Integer Implements ICharacter.Speed
                    Get
                        Return XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<Speed>.Value
                    End Get
                End Property

                Public ReadOnly Property PointToDraw As Point Implements ICharacter.PointToDraw
                    Get
                        Return New Point(LocationX, LocationY)
                    End Get
                End Property

                Public ReadOnly Property GunPointToDraw As Point Implements ICharacter.GunPointToDraw
                    Get
                        Select Case CurrentFacing
                            Case Character.Facing.North
                                Return New Point(PointToDraw.X, PointToDraw.Y - IMGSize * 0.5)
                            Case Character.Facing.South
                                Return New Point(PointToDraw.X, PointToDraw.Y + IMGSize * 0.5)
                            Case Character.Facing.East
                                Return New Point(PointToDraw.X + IMGSize * 0.5, PointToDraw.Y)
                            Case Character.Facing.West
                                Return New Point(PointToDraw.X - IMGSize * 0.5, PointToDraw.Y)
                            Case Else 'Character.Facing.NorthEast
                                Return New Point(PointToDraw.X, PointToDraw.Y)
                        End Select
                    End Get
                End Property

                Public Sub UpdateShots() Implements ICharacter.UpdateShots
                    For Each Shot In ShotQueue
                        CurrentShots.Add(Shot)
                    Next
                    For Each Shot In ShotRemoveQueue
                        CurrentShots.Remove(Shot)
                    Next
                    ShotQueue.RemoveRange(0, ShotQueue.Count)
                    ShotRemoveQueue.RemoveRange(0, ShotRemoveQueue.Count)
                End Sub

                Public ReadOnly Property CurrentShotsList As List(Of Weapons.IShot) Implements ICharacter.CurrentShotList
                    Get
                        Return CurrentShots
                    End Get
                End Property

                Public Sub AddShot(ByVal ShotIncrease As Double) Implements ICharacter.AddShot
                    TempShot = Gun.SummonProjectile(ShotIncrease)
                    If Not TempShot Is Nothing Then
                        ShotQueue.Add(TempShot)
                    End If
                End Sub

                Public Sub MoveShots() Implements ICharacter.MoveShots
                    For Each Shot In CurrentShots
                        If Weapons.GunMath.ShotIsOutOfRange(Shot, CurrentMap, ImageSize) = False And Shot.IsAlive = True Then
                            Shot.Move()
                        ElseIf Shot.IsAlive = False And Weapons.GunMath.ShotIsOutOfRange(Shot, CurrentMap, ImageSize) = False Then
                            ShotRemoveQueue.Add(Shot)
                            CurrentMap.DoDamagaToTile(Shot.DeathTile.X, Shot.DeathTile.Y, Shot.Damage)
                        Else
                            ShotRemoveQueue.Add(Shot)
                        End If
                    Next
                End Sub

                Public Property CurrentTracker1 As Tracker Implements ICharacter.CurrentTracker
                    Get
                        Return CurrentTracker
                    End Get
                    Set(ByVal value As Tracker)
                        CurrentTracker = value
                    End Set
                End Property

                Public ReadOnly Property ImageSize As Integer Implements ICharacter.ImageSize
                    Get
                        Return IMGSize
                    End Get
                End Property

                Public ReadOnly Property RectangleToDraw As Rectangle Implements ICharacter.RectangleToDraw
                    Get
                        Return CurrentRectangle
                    End Get
                End Property

                Public Property MiddlePointDrawedOnScreen As Point Implements ICharacter.MiddlePointDrawedOnScreen
                    Get
                        Return PointDrawed
                    End Get
                    Set(ByVal value As Point)
                        PointDrawed = New Point(value.X + IMGSize, _
                                         value.Y + 0.5 * IMGSize)
                    End Set
                End Property

                Public ReadOnly Property PointDrawedOnScreen As Point Implements ICharacter.PointDrawedOnScreen
                    Get

                        Return New Point(MiddlePointDrawedOnScreen.X - IMGSize, _
                                MiddlePointDrawedOnScreen.Y - 0.5 * IMGSize)

                    End Get
                End Property

                Public Property CurrentForm As Form Implements ICharacter.CurrentForm
                    Get
                        Return CurrentFormForm
                    End Get
                    Set(ByVal value As Form)
                        CurrentFormForm = value
                    End Set
                End Property

                Public ReadOnly Property Map As Map Implements ICharacter.Map
                    Get
                        Return CurrentMap
                    End Get
                End Property

                Public ReadOnly Property _DirectXTexturePath As String Implements ICharacter._DirectXTexturePath
                    Get
                        Return XMLDoc.<Game>.<Information>.<CharData>.<Rick>.<Resource>.Value.ToString
                    End Get
                End Property

                Public Property Rotation As Double Implements ICharacter.Rotation
                    Get
                        Return CurrentRotation
                    End Get
                    Set(value As Double)
                        CurrentRotation = value
                    End Set
                End Property

                Public Property TakeScreen As Boolean Implements ICharacter.TakeScreen
                    Get
                        Return ScreenNeeded
                    End Get
                    Set(value As Boolean)
                        ScreenNeeded = value
                    End Set
                End Property
            End Class
        End Namespace
    End Namespace
End Namespace