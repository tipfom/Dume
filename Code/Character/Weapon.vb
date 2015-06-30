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
    Namespace Weapons
        Friend Class GunMath
            Public Shared Function GetProjectileStartPoint(ByVal Radius As Integer, ByVal MiddlePoint As Point, ByVal StartWinkel As Double) As Point
                Dim xWinkel As Double = 2 * Math.Atan(Math.PI / 2)
                ' siehe zeichnung und cas
                Dim gWinkel As Double = StartWinkel + xWinkel
                Dim returnedIncrease As Double
                Dim returnedPoint As Point
                Dim xMove As Double
                Select Case gWinkel Mod 360 * Math.PI / 180
                    Case Is >= 270 * (Math.PI / 180)
                        returnedIncrease = Math.Tan(gWinkel)
                        xMove = -Math.Abs(Math.Cos(Math.Atan2(returnedIncrease, 1)) * Radius)
                    Case Is >= 180 * (Math.PI / 180)
                        returnedIncrease = Math.Tan(gWinkel)
                        xMove = Math.Abs(Math.Cos(Math.Atan2(returnedIncrease, 1)) * Radius)
                    Case Is >= 90 * (Math.PI / 180)
                        returnedIncrease = Math.Tan(gWinkel)
                        xMove = Math.Abs(Math.Cos(Math.Atan2(returnedIncrease, 1)) * Radius)
                    Case Else
                        returnedIncrease = Math.Tan(gWinkel)
                        xMove = -Math.Abs(Math.Cos(Math.Atan2(returnedIncrease, 1)) * Radius)
                End Select

                returnedPoint = New Point(MiddlePoint.X + xMove, _
                                        (MiddlePoint.Y + xMove * returnedIncrease))

                Return returnedPoint
            End Function

            Public Shared Function CheckProjectileTile(ByVal Shot As Weapons.IShot, ByVal Map As Map, ByVal ImageSize As Integer)

                Return Map.GetTileOf((Shot.PointToDraw.X - ImageSize) \ ImageSize, (Shot.PointToDraw.Y - ImageSize) \ ImageSize)
                ' auch hier den nullpunkt 32,32 beachten

            End Function

            Public Shared Function ShotIsOutOfRange(ByVal Shot As Weapons.IShot, ByVal Map As Map, ByVal ImageSize As Integer) As Boolean
                If Shot.PointToDraw.X <= 0 Then Return True
                If Shot.PointToDraw.X >= Map.MapSize.Width * ImageSize Then Return True

                If Shot.PointToDraw.Y <= 0 Then Return True
                If Shot.PointToDraw.Y >= Map.MapSize.Height * ImageSize Then Return True
            End Function
        End Class

        Public Interface IGun
            ReadOnly Property Damage As Integer
            ReadOnly Property Name As String
            ReadOnly Property FireSpeed As Integer
            ReadOnly Property ReloadTime As Integer
            ReadOnly Property MaxShells As Integer
            Property Ammo As Integer
            ReadOnly Property GunType As GunType
            ReadOnly Property AmmoType As AmmoType
            ReadOnly Property ShellSpeed As Integer
            ReadOnly Property Description As String
            ReadOnly Property Producer As String
            ReadOnly Property _DirectXTexturePath As String
            ReadOnly Property _DirectXShotTexturePath As String
            Function SummonProjectile(ByVal CurrentFacingIncrease As Double) As Weapons.IShot
            ReadOnly Property TextureSize As Size
            ReadOnly Property HaveToReload As Boolean
            ReadOnly Property LeftAmmo As Integer
            Sub Reload()
        End Interface

        Public Interface IShot
            ReadOnly Property Damage As Integer
            ReadOnly Property AmmoType As AmmoType
            ReadOnly Property ProjectileIncrease As Double
            ReadOnly Property CrossWithY As Double
            ReadOnly Property PointToDraw As Point
            ReadOnly Property ShotDirection As Character.Facing
            ReadOnly Property ShotRotation As Double
            ReadOnly Property IsAlive As Boolean
            ReadOnly Property DeathTile As Point
            ReadOnly Property ImageSize As Size
            Sub Move()
        End Interface

        Public Enum GunType
            AssaultRifle
            Sniper
            Shotgun
            Granate
            AntiTankRifle
            Pistol
        End Enum

        Public Enum AmmoType
            Laser
            Rocket
            Shell
        End Enum

        Public Enum Direction
            Right
            Left
        End Enum

        Namespace GunCollection
            Class DuX_127_S
                Implements IGun

                Dim XMLDoc As XDocument = XDocument.Load("Content/Config/Main.xml")

                Dim CuChar As Character.ICharacter

                Dim Shot As Weapons.ShotCollection.DuX_127_S_Shot

                Dim CanFire As Boolean = True
                Dim Reload As Boolean = False
                Dim WithEvents FirePause As New System.Windows.Forms.Timer
                Dim WithEvents ReloadTimer As New System.Windows.Forms.Timer
                Dim Ammonution As Integer
                Dim AmmoLeft As Integer = XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<MaxAmmo>.Value - XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<Ammo>.Value

                Sub New(ByVal CurrentChar As Character.ICharacter)
                    Ammonution = XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<Ammo>.Value
                    CuChar = CurrentChar
                    FirePause.Interval = FireSpeed
                    FirePause.Enabled = False
                    ReloadTimer.Interval = ReloadTime
                    ReloadTimer.Enabled = False
                End Sub


                Public ReadOnly Property AmmoType As AmmoType Implements IGun.AmmoType
                    Get
                        Return Weapons.AmmoType.Shell
                    End Get
                End Property

                Public ReadOnly Property Damage As Integer Implements IGun.Damage
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<Damage>.Value
                    End Get
                End Property

                Public ReadOnly Property Description As String Implements IGun.Description
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<Description>.Value
                    End Get
                End Property

                Public ReadOnly Property FireSpeed As Integer Implements IGun.FireSpeed
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<FireSpeed>.Value
                    End Get
                End Property

                Public ReadOnly Property GunType As GunType Implements IGun.GunType
                    Get
                        Return GunType.Pistol
                    End Get
                End Property

                Public ReadOnly Property MaxShells As Integer Implements IGun.MaxShells
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<MaxAmmo>.Value
                    End Get
                End Property

                Public ReadOnly Property Name As String Implements IGun.Name
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<Name>.Value
                    End Get
                End Property

                Public ReadOnly Property Producer As String Implements IGun.Producer
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<Producer>.Value
                    End Get
                End Property

                Public ReadOnly Property ReloadTime As Integer Implements IGun.ReloadTime
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<ReloadTime>.Value
                    End Get
                End Property

                Public ReadOnly Property ShellSpeed As Integer Implements IGun.ShellSpeed
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<ShellSpeed>.Value
                    End Get
                End Property

                Public Property Ammo As Integer Implements IGun.Ammo
                    Get
                        Return Ammonution
                    End Get
                    Set(ByVal value As Integer)

                        Ammonution = value
                        If Ammonution < 1 Then
                            Reload = True
                        End If
                    End Set
                End Property


                Private Sub ToggleFire() Handles FirePause.Tick
                    CanFire = True
                    FirePause.Enabled = False
                End Sub

                Public Function SummonProjectile(ByVal CurrentFacingIncrease As Double) As IShot Implements IGun.SummonProjectile
                    If CanFire Then
                        If Reload = False And Ammo > 0 Then
                            Ammo = Ammo - 1
                            CanFire = False
                            FirePause.Enabled = True
                            Return New ShotCollection.DuX_127_S_Shot(CurrentFacingIncrease, _
                            GunMath.GetProjectileStartPoint(25, New Point(CuChar.PointToDraw.X, CuChar.PointToDraw.Y), CuChar.Rotation), _
                            TrackMath.IsLeft(GunMath.GetProjectileStartPoint(25, New Point(CuChar.MiddlePointDrawedOnScreen.X, CuChar.MiddlePointDrawedOnScreen.Y), CuChar.Rotation).X, _
                            Cursor.Position.X + CuChar.Container.Form.Location.X + 9), CuChar.Rotation, CuChar)
                        End If
                    Else
                        Return Nothing
                    End If
                End Function

                Public ReadOnly Property _DirectXTexturePath As String Implements IGun._DirectXTexturePath
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<GunResource>.Value
                    End Get
                End Property

                Public ReadOnly Property _DirectXShotTexturePath As String Implements IGun._DirectXShotTexturePath
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<ProjectileResource>.Value
                    End Get
                End Property

                Public ReadOnly Property TextureSize As Size Implements IGun.TextureSize
                    Get
                        Return New Size(64, 64)
                    End Get
                End Property

                Public ReadOnly Property HaveToReload As Boolean Implements IGun.HaveToReload
                    Get
                        Return Reload
                    End Get
                End Property

                Public Sub Reload1() Implements IGun.Reload
                    ReloadTimer.Enabled = True
                End Sub

                Private Sub ReloadTick() Handles ReloadTimer.Tick
                    ReloadTimer.Enabled = False
                    Reload = False
                    If AmmoLeft > 0 Then
                        Ammo = XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<Ammo>.Value
                        AmmoLeft -= Ammo
                    End If
                End Sub

                Public ReadOnly Property LeftAmmo As Integer Implements IGun.LeftAmmo
                    Get
                        Return AmmoLeft
                    End Get
                End Property
            End Class
        End Namespace

        Namespace ShotCollection

            Class DuX_127_S_Shot
                Implements IShot

                Dim ShotDir As Character.Facing
                Dim Direction As Direction = Direction.Right

                Dim XMLDoc As XDocument = XDocument.Load("Content/Config/Main.xml")
                Dim ProjecSpeed As Double = XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<ShellSpeed>.Value

                Dim Increase As Double
                Dim YCross As Integer
                Dim Rotation As Double
                Dim CurrentX As Double

                Dim StartX As Integer 'Infinity Clausel weil wegen error
                Dim DeathX, DeathY As Integer
                Dim DPoint As Point
                Dim Alive As Boolean = True

                Sub New(ByVal CFacingIncrease As Double, ByVal StartPosition As Point, ByVal IsLeftOfChar As Boolean, ByVal StartRotation As Double, ByVal CurrentChar As Character.ICharacter)
                    If Double.IsInfinity(CFacingIncrease) = False Then
                        Select Case IsLeftOfChar
                            Case True
                                Increase = -CFacingIncrease
                                Dim PX As Integer = StartPosition.X
                                Dim PY As Integer = StartPosition.Y
                                YCross = Convert.ToInt32(PY + CFacingIncrease * PX)
                                CurrentX = PX
                                Direction = Weapons.Direction.Left
                            Case False
                                Increase = -CFacingIncrease
                                Dim PX As Integer = StartPosition.X
                                Dim PY As Integer = StartPosition.Y
                                YCross = Convert.ToInt32(PY + CFacingIncrease * PX)
                                CurrentX = PX
                                Direction = Weapons.Direction.Right
                        End Select
                    ElseIf Double.IsNegativeInfinity(CFacingIncrease) = True Then
                        Direction = Weapons.Direction.Right
                        ShotDir = Character.Facing.South
                        Increase = Double.NegativeInfinity
                        CurrentX = StartPosition.Y
                        StartX = StartPosition.X
                    ElseIf Double.IsPositiveInfinity(CFacingIncrease) = True Then
                        Direction = Weapons.Direction.Left ' Damit an höhe addiert und auf y subtrahiert wird(0,0) = Koordinatenursprung links ober
                        ShotDir = Character.Facing.North
                        Increase = Double.PositiveInfinity
                        CurrentX = StartPosition.Y
                        StartX = StartPosition.X
                    End If
                    Rotation = StartRotation


                    'Bestimmt den "TodesPunkt" eines Projektils
                    Select Case Direction
                        Case Weapons.Direction.Left
                            If Not Double.IsInfinity(Increase) Then
                                Dim TempXTile As Integer
                                Dim TempYTile As Integer
                                If Increase < 0 Then 'Ist ja invertiert
                                    For x = CurrentX To 0 Step -1
                                        TempXTile = x \ CurrentChar.ImageSize
                                        For y As Integer = (x - 1) * Increase + YCross To x * Increase + YCross Step -1
                                            TempYTile = y \ CurrentChar.ImageSize
                                            If TempXTile <= CurrentChar.Container.Map.MapSize.Width And TempXTile >= 0 And TempYTile <= CurrentChar.Container.Map.MapSize.Height And TempYTile >= 0 Then
                                                If CurrentChar.Container.Map.GetTileOf(TempXTile, TempYTile).Permeable = False Then
                                                    DeathX = x
                                                    DeathY = y
                                                    DPoint = New Point(TempXTile, TempYTile)
                                                    GoTo MDoneL
                                                End If
                                            Else
                                                GoTo MDoneL
                                            End If
                                        Next
                                    Next
MDoneL:
                                    If DeathX = Nothing Then
                                        DeathX = 0
                                        DeathY = YCross
                                    End If
                                ElseIf Increase >= 0 Then
                                    For x As Integer = CurrentX To 0 Step -1
                                        TempXTile = x \ CurrentChar.ImageSize
                                        For y = x * Increase + YCross To (x + 1) * Increase + YCross
                                            TempYTile = y \ CurrentChar.ImageSize
                                            If TempXTile <= CurrentChar.Container.Map.MapSize.Width And TempXTile >= 0 And TempYTile <= CurrentChar.Container.Map.MapSize.Height And TempYTile >= 0 Then
                                                If CurrentChar.Container.Map.GetTileOf(TempXTile, TempYTile).Permeable = False Then
                                                    DeathX = x
                                                    DeathY = y
                                                    DPoint = New Point(TempXTile, TempYTile)
                                                    GoTo PDoneL
                                                End If
                                            Else
                                                GoTo PDoneL
                                            End If
                                        Next
                                    Next
PDoneL:

                                    If DeathX = Nothing Then
                                        DeathX = 0
                                        DeathY = YCross
                                    End If
                                End If

                            Else

                            End If
                        Case Weapons.Direction.Right
                            If Not Double.IsInfinity(Increase) Then
                                Dim TempXTile As Integer
                                Dim TempYTile As Integer
                                If Increase > 0 Then 'Ist ja invertiert
                                    For x = CurrentX To CurrentChar.Container.Map.MapSize.Width * CurrentChar.ImageSize
                                        TempXTile = x \ CurrentChar.ImageSize
                                        For y = x * Increase + YCross To (x + 1) * Increase + YCross
                                            TempYTile = y \ CurrentChar.ImageSize
                                            If TempXTile <= CurrentChar.Container.Map.MapSize.Width And TempXTile >= 0 And TempYTile <= CurrentChar.Container.Map.MapSize.Height And TempYTile >= 0 Then
                                                If CurrentChar.Container.Map.GetTileOf(TempXTile, TempYTile).Permeable = False Then
                                                    DeathX = x
                                                    DeathY = y
                                                    DPoint = New Point(TempXTile, TempYTile)
                                                    GoTo MDone
                                                End If
                                            Else
                                                GoTo MDone
                                            End If
                                        Next
                                    Next
MDone:
                                    If DeathX = Nothing Then
                                        DeathX = CurrentChar.Container.Map.UnIndexedMapSize.Width * CurrentChar.ImageSize
                                        DeathY = DeathX * CurrentChar.ImageSize + YCross
                                    End If
                                ElseIf Increase <= 0 Then
                                    For x As Integer = CurrentX To CurrentChar.Container.Map.MapSize.Width * CurrentChar.ImageSize
                                        TempXTile = x \ CurrentChar.ImageSize
                                        For y As Integer = (x - 1) * Increase + YCross To x * Increase + YCross Step -1
                                            TempYTile = y \ CurrentChar.ImageSize
                                            If TempXTile <= CurrentChar.Container.Map.MapSize.Width And TempXTile >= 0 And TempYTile <= CurrentChar.Container.Map.MapSize.Height And TempYTile >= 0 Then
                                                If CurrentChar.Container.Map.GetTileOf(TempXTile, TempYTile).Permeable = False Then
                                                    DeathX = x
                                                    DeathY = y
                                                    DPoint = New Point(TempXTile, TempYTile)
                                                    GoTo PDone
                                                End If
                                            Else
                                                GoTo PDone
                                            End If
                                        Next
                                    Next
PDone:
                                    If DeathX = Nothing Then
                                        DeathX = CurrentChar.Container.Map.UnIndexedMapSize.Width * CurrentChar.ImageSize
                                    End If
                                End If

                            Else

                            End If
                    End Select
                End Sub

                Public ReadOnly Property AmmoType As AmmoType Implements IShot.AmmoType
                    Get
                        Return Weapons.AmmoType.Shell
                    End Get
                End Property

                Public ReadOnly Property Damage As Integer Implements IShot.Damage
                    Get
                        Return XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<Damage>.Value
                    End Get
                End Property

                Public Sub Move() Implements IShot.Move
                    Select Case Direction ' siehe aufzeichnungen
                        Case Weapons.Direction.Left
                            If Double.IsInfinity(Increase) = False Then
                                CurrentX -= Math.Abs(Math.Cos(Math.Atan2(Increase, 1)) * ProjecSpeed)
                            Else
                                CurrentX -= ProjecSpeed
                            End If
                            'CurrentX -= ProjecSpeed
                        Case Weapons.Direction.Right
                            If Double.IsInfinity(Increase) = False Then
                                CurrentX += Math.Abs(Math.Cos(Math.Atan2(Increase, 1)) * ProjecSpeed)
                            Else
                                CurrentX += ProjecSpeed
                            End If
                    End Select
                    If Double.IsInfinity(Increase) = False Then
                        Select Case Direction
                            Case Weapons.Direction.Left
                                If Increase < 0 Then
                                    If CurrentX <= DeathX Then
                                        Alive = False
                                        CurrentX = DeathX
                                    End If
                                Else
                                    If CurrentX <= DeathX Then
                                        Alive = False
                                        CurrentX = DeathX
                                    End If
                                End If
                            Case Weapons.Direction.Right
                                If Increase > 0 Then
                                    If CurrentX >= DeathX Or CurrentX * Increase + YCross >= DeathY Then
                                        Alive = False
                                        CurrentX = DeathX
                                    End If
                                Else
                                    If CurrentX >= DeathX Or CurrentX * Increase + YCross <= DeathY Then
                                        Alive = False
                                        CurrentX = DeathX
                                    End If
                                End If

                        End Select
                    End If
                End Sub

                Public ReadOnly Property CrossWithY As Double Implements IShot.CrossWithY
                    Get
                        Return YCross
                    End Get
                End Property

                Public ReadOnly Property PointToDraw As Point Implements IShot.PointToDraw
                    Get
                        If Double.IsInfinity(Increase) = False Then
                            Return New Point(CurrentX, Increase * CurrentX + CrossWithY)
                        Else
                            Return New Point(StartX, CurrentX)
                        End If
                    End Get
                End Property

                Public ReadOnly Property ProjectileIncrease As Double Implements IShot.ProjectileIncrease
                    Get
                        Return Increase
                    End Get
                End Property

                Public ReadOnly Property ShotDirection As Character.Facing Implements IShot.ShotDirection
                    Get

                    End Get
                End Property

                Public ReadOnly Property ShotRotation As Double Implements IShot.ShotRotation
                    Get
                        Return Rotation
                    End Get
                End Property

                Public ReadOnly Property IsAlive As Boolean Implements IShot.IsAlive
                    Get
                        Return Alive
                    End Get
                End Property

                Public ReadOnly Property DeathTile As Point Implements IShot.DeathTile
                    Get
                        Return DPoint
                    End Get
                End Property

                Public ReadOnly Property ImageSize As System.Drawing.Size Implements IShot.ImageSize
                    Get
                        Return New Size(XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<ProjectileResource>.@Width, XMLDoc.<Game>.<Information>.<GunData>.<DuX127>.<ProjectileResource>.@Height)
                    End Get
                End Property
            End Class
        End Namespace
    End Namespace
End Namespace