Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Threading
Imports MySql.Data.MySqlClient
Imports Microsoft.DirectX.Direct3D
Imports Microsoft.DirectX
Imports Game

Namespace Global.Game
    Namespace Visuals
        Public Class DirectXDisplayer
            Dim DirectXDevice As Device
            Dim DirectXParams As New PresentParameters

            Dim Textures As TextureContainer

            Dim DirectXText As Direct3D.Font

            Dim TileTexture(42) As Texture
            Dim CharTexture As Texture
            Dim GunTexture(1) As Texture
            Dim UserInterface(2) As Texture
            Dim MobTexture(1) As Texture

            Dim InterfaceSize(1) As Size

            Dim DirectXSprite As Sprite 'Direct2D Kompilation

            Dim WithEvents Form2Draw As Form
            Dim Map As Map

            Dim RenderThread As New Thread(AddressOf Render)

            Dim ImageSize As Integer

            Dim MapBitMap As Bitmap

            Dim HitBox(7) As CustomVertex.TransformedColored
            Dim Middlepointer(2) As CustomVertex.TransformedColored

            Dim Full As Boolean = False

            ' benötigte API-Deklaration
            Public Declare Function GetTickCount Lib "kernel32" () As Long

            ' Ist eine Sekunde um?
            Public FPS_LastCheck As Long

            ' FPS werden hochgezählt
            Public FPS_Count As Long

            ' Enthält aktuelle FPS Zahl
            Public FPS_Current As Integer

            ' Höchste erreichte fps
            Public FPS_Highest As Integer

            Dim FontSize As Integer
            Dim ImageScale As Double

            Dim Container As Container

            Sub New(ByVal TargetForm As Form, ByRef CurrentMap As Map, ByVal IMGSIZE As Integer, ByVal _FontSize As Integer, ByRef _Container As Container, ByVal _ImageScale As Double)
                Form2Draw = TargetForm
                Map = CurrentMap
                ImageSize = IMGSIZE
                FontSize = _FontSize
                Container = _Container
                ImageScale = _ImageScale
            End Sub

            Public Sub ReInitialize()
                DirectXParams.SwapEffect = SwapEffect.Discard 'siehe tutorial

                DirectXParams.Windowed = True

                DirectXParams.BackBufferHeight = Form2Draw.Height
                DirectXParams.BackBufferWidth = Form2Draw.Width

                DirectXDevice.Reset(DirectXParams)

                InterfaceSize(1) = New Size(Form2Draw.Width, InterfaceSize(0).Height / InterfaceSize(0).Width * Form2Draw.Width)
            End Sub

            Public Sub Initialize()
                Form2Draw.Show()
                DirectXParams.SwapEffect = SwapEffect.Discard 'siehe tutorial

                DirectXParams.Windowed = True

                DirectXDevice = New Device(0, DeviceType.Hardware, Form2Draw.Handle, CreateFlags.HardwareVertexProcessing, DirectXParams)
                DirectXDevice.RenderState.Lighting = False
                'Deaktiviert Licht
                DirectXDevice.RenderState.CullMode = Cull.None
                DirectXDevice.RenderState.FillMode = FillMode.Solid


                DirectXSprite = New Sprite(DirectXDevice)

                DirectXText = New Direct3D.Font(DirectXDevice, New Drawing.Font("Calibri", FontSize))

                Textures = New TextureContainer(DirectXDevice, Container)

                LoadTextures()
            End Sub

            Private Sub LoadTextures()
                Textures.LoadTile(Container.TileContainer.Container)
                Textures.LoadOverlay(Container.OverlayContainer.Overlays)
                Textures.LoadChar(New Character.CharCollection.DrSlime(Container, ImageSize, "NOTHING"))
                Textures.LoadMob(New Mob.Slime.Mob(New Point(0, 0)))

                GunTexture(0) = TextureLoader.FromFile(DirectXDevice, XDocument.Load("content/config/main.xml").<Game>.<Information>.<GunData>.<DuX127>.<GunResource>.Value, 64, 64, 1, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.Linear, Filter.None, Nothing)
                GunTexture(1) = TextureLoader.FromFile(DirectXDevice, XDocument.Load("content/config/main.xml").<Game>.<Information>.<GunData>.<DuX127>.<ProjectileResource>.Value, XDocument.Load("content/config/main.xml").<Game>.<Information>.<GunData>.<DuX127>.<ProjectileResource>.@Width, XDocument.Load("content/config/main.xml").<Game>.<Information>.<GunData>.<DuX127>.<ProjectileResource>.@Height, 1, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.None, Filter.None, Nothing)

                UserInterface(0) = TextureLoader.FromFile(DirectXDevice, XDocument.Load("content/config/main.xml").<Game>.<Configuration>.<UserInterface>.@Main, XDocument.Load("content/config/main.xml").<Game>.<Configuration>.<UserInterface>.<Main>.@Width, XDocument.Load("content/config/main.xml").<Game>.<Configuration>.<UserInterface>.<Main>.@Height, 1, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.None, Filter.None, Nothing)
                UserInterface(1) = TextureLoader.FromFile(DirectXDevice, XDocument.Load("content/config/main.xml").<Game>.<Configuration>.<UserInterface>.@Reload, XDocument.Load("content/config/main.xml").<Game>.<Configuration>.<UserInterface>.<Reload>.@Width, XDocument.Load("content/config/main.xml").<Game>.<Configuration>.<UserInterface>.<Reload>.@Height, 1, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.None, Filter.None, Nothing)

                InterfaceSize(0) = New Size(XDocument.Load("content/config/main.xml").<Game>.<Configuration>.<UserInterface>.<Main>.@Width, XDocument.Load("content/config/main.xml").<Game>.<Configuration>.<UserInterface>.<Main>.@Height)
                InterfaceSize(1) = New Size(Form2Draw.Width, InterfaceSize(0).Height / InterfaceSize(0).Width * Form2Draw.Width)
            End Sub

            Public Sub TakeScreenshot()
                Direct3D.SurfaceLoader.Save(String.Format("Screenshots/{0}_{1}_{2}_{3};{4}.png", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, DateTime.Now.Day), ImageFileFormat.Png, DirectXDevice.GetBackBuffer(0, 0, BackBufferType.Mono))
                MessageBox.Show("Screenshot taken!", "Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
            End Sub

            Public Sub Render()
                DirectXDevice.Clear(ClearFlags.Target, Color.White, 1.0F, 0)

                DirectXDevice.BeginScene()
                DirectXSprite.Begin(SpriteFlags.AlphaBlend)

                Dim xS As Integer
                Select Case Container.CharacterContainer.MainCharacter.PointToDraw.X
                    Case Is <= Form2Draw.Width / 2
                        xS = 0
                    Case Is >= Map.UnIndexedMapSize.Width * ImageSize - Form2Draw.Width / 2
                        xS = Map.UnIndexedMapSize.Width * ImageSize - Form2Draw.Width
                    Case Else
                        xS = Container.CharacterContainer.MainCharacter.PointToDraw.X - Form2Draw.Width / 2
                End Select

                'Gets the YCoordinates of the seen Map
                Dim yS As Integer

                Select Case Container.CharacterContainer.MainCharacter.PointToDraw.Y
                    Case Is <= Form2Draw.Height / 2 - InterfaceSize(1).Height / 2
                        yS = 0
                    Case Is >= Map.UnIndexedMapSize.Height * ImageSize - Form2Draw.Height / 2 + InterfaceSize(1).Height / 2
                        yS = Map.UnIndexedMapSize.Height * ImageSize - Form2Draw.Height + InterfaceSize(1).Height
                    Case Else
                        yS = Container.CharacterContainer.MainCharacter.PointToDraw.Y - Form2Draw.Height / 2 + InterfaceSize(1).Height / 2
                End Select

                Dim CurrentTile As Tile._Tile

                For x As Integer = xS \ ImageSize To Form2Draw.Width \ ImageSize + xS \ ImageSize + 1
                    For y As Integer = yS \ ImageSize To Form2Draw.Height \ ImageSize + yS \ ImageSize + InterfaceSize(1).Height \ ImageSize + 1
                        If Not x >= Map.UnIndexedMapSize.Width And Not y >= Map.UnIndexedMapSize.Height Then
                            CurrentTile = Map.GetTileOf(x, y)
                            DirectXSprite.Draw2D(Textures.GetTexture(CurrentTile.ID), New Rectangle(CurrentTile.TexturePosition, CurrentTile.TextureSize), New SizeF(ImageSize, ImageSize), New PointF(x * ImageSize - xS, y * ImageSize - yS), Color.White.ToArgb)
                        End If
                    Next
                Next

                'Mobs und Spawns aufs SpielFeld Malen
                For Each Swarm As Mob.ISwarm In Container.MobContainer.List
                    For Each Mob As Mob.IMob In Swarm.MobList
                        DirectXSprite.Draw2D(Textures.GetTexture(Mob.SpecialInformation(Game.Mob.MobInformation.TextureID)), New Rectangle(Mob.TexturePosition, Mob.TextureSize), _
                                             New SizeF(Mob.TextureSize.Width * ImageScale * Mob.ImageScale, Mob.TextureSize.Height * ImageScale * Mob.ImageScale), New PointF(Mob.TextureSize.Width / 2, Mob.TextureSize.Height / 2), 0, _
                                             New PointF(Mob.PointToDraw.X - xS, Mob.PointToDraw.Y - yS), Color.White)
                    Next
                    'For Each Spawn As Mob.ISpawn In Swarm.SpawnList
                    '    DirectXSprite.Draw2D(Textures.GetTexture(""), New Rectangle(Spawn.TexturePosition, Spawn.TextureSize), Spawn.TextureSize, New PointF(32, 32), Spawn.Rotation, New PointF(Spawn.Location.X - xS, Spawn.Location.Y - yS), Color.White.ToArgb)
                    'Next
                Next

                'Gegenstände
                For Each Overlay As Overlay.Overlay In Container.OverlayContainer.Container
                    DirectXSprite.Draw2D(Textures.GetTexture(Overlay.ID), New Rectangle(Overlay.TextureLocation, Overlay.TextureSize), Overlay.TextureSize, New PointF(Overlay.TextureSize.Width / 2, Overlay.TextureSize.Height / 2), Overlay.Rotation, New PointF(Overlay.Position.X - xS, Overlay.Position.Y - yS), Color.White.ToArgb)
                Next

                '''''Chars Malen
                For Each Character As Character.ICharacter In Container.CharacterContainer.Container
                    'Schatten malen
                    DirectXSprite.Draw2D(Textures.GetTexture(Character.TextureID), New Rectangle(Character.TextureLocation, Character.TextureSize), New Size(Character.TextureSize.Width * ImageScale, Character.TextureSize.Height * ImageScale), New PointF(Character.TextureSize.Width / 2, Character.TextureSize.Height / 2), Character.Rotation, New Point(Character.PointToDraw.X - xS, Character.PointToDraw.Y - yS), Color.White) ' New Point(Character.PointToDraw.X - xS, Character.PointToDraw.Y - yS), Color.White.ToArgb)
                    Character.MiddlePointDrawedOnScreen = New Point((Character.PointToDraw.X - xS), (Character.PointToDraw.Y - yS))
                Next

                For Each SlimeBall In Container.SnotContainer
                    If SlimeBall.Location.X - xS >= 0 And SlimeBall.Location.Y - yS >= 0 And SlimeBall.Location.X - xS <= Form2Draw.Width And SlimeBall.Location.Y - yS <= Form2Draw.Height - InterfaceSize(1).Height Then
                        DirectXSprite.Draw2D(Textures.GetTexture(Container.CharacterContainer.MainCharacter.TextureID), SlimeBall.Texture, New SizeF(SlimeBall.Texture.Width * ImageScale, SlimeBall.Texture.Height * ImageScale), New PointF(SlimeBall.Texture.Width / 2, SlimeBall.Texture.Height / 2), 0, New PointF(SlimeBall.Location.X - xS, SlimeBall.Location.Y - yS), Color.White.ToArgb)

                    End If
                Next
                For Each Laser In Container.LaserContainer
                    If Laser.Position(Weapon.LaserPart.Right).X - xS >= 0 And Laser.Position(Weapon.LaserPart.Right).Y - yS >= 0 And Laser.Position(Weapon.LaserPart.Left).X - xS <= Form2Draw.Width And Laser.Position(Weapon.LaserPart.Left).Y - yS <= Form2Draw.Height - InterfaceSize(1).Height Then
                        DirectXSprite.Draw2D(Textures.GetTexture(Container.CharacterContainer.MainCharacter.TextureID), Laser.Source.TextureRectangle(Weapon.LaserPart.Left), Laser.DrawnSize(Weapon.LaserPart.Left), New PointF(Laser.Source.TextureRectangle(Weapon.LaserPart.Left).Width / 2, Laser.Source.TextureRectangle(Weapon.LaserPart.Left).Height / 2), Laser.TextureRotation, New PointF(Laser.Position(Weapon.LaserPart.Left).X - xS, Laser.Position(Weapon.LaserPart.Left).Y - yS), Color.White)
                        DirectXSprite.Draw2D(Textures.GetTexture(Container.CharacterContainer.MainCharacter.TextureID), Laser.Source.TextureRectangle(Weapon.LaserPart.Center), Laser.DrawnSize(Weapon.LaserPart.Center), New PointF(Laser.Source.TextureRectangle(Weapon.LaserPart.Center).Width / 2, Laser.Source.TextureRectangle(Weapon.LaserPart.Center).Height / 2), Laser.TextureRotation, New PointF(Laser.Position(Weapon.LaserPart.Center).X - xS, Laser.Position(Weapon.LaserPart.Center).Y - yS), Color.White)
                        DirectXSprite.Draw2D(Textures.GetTexture(Container.CharacterContainer.MainCharacter.TextureID), Laser.Source.TextureRectangle(Weapon.LaserPart.Right), Laser.DrawnSize(Weapon.LaserPart.Right), New PointF(Laser.Source.TextureRectangle(Weapon.LaserPart.Right).Width / 2, Laser.Source.TextureRectangle(Weapon.LaserPart.Right).Height / 2), Laser.TextureRotation, New PointF(Laser.Position(Weapon.LaserPart.Right).X - xS, Laser.Position(Weapon.LaserPart.Right).Y - yS), Color.White)
                    End If
                Next


                'Interface
                DirectXSprite.Draw2D(UserInterface(0), New Rectangle(New Point(0, 0), InterfaceSize(0)), _
                               InterfaceSize(1), _
                               New PointF(0, Form2Draw.Height - InterfaceSize(1).Height), _
                               Color.White.ToArgb)

                DirectXSprite.End()

                '640,30 Skalierung für pos dieser werte in 1886px version
                'Score
                DirectXText.DrawText(Nothing, "Score : " & Container.CharacterContainer.MainCharacter.Score, _
                                     New Point(640 * Form2Draw.Width / 1886, Form2Draw.Height - 137 / 1886 * Form2Draw.Width), _
                                     &HFF244F7C)

                'Waffendaten
                'DirectXText.DrawText(Nothing, String.Format("{0} {1} - {2}\{3}", "Munition :", Container.CharacterContainer.MainCharacter.CurrentWeapon.Ammo, Container.CharacterContainer.MainCharacter.CurrentWeapon.LeftAmmo, Container.CharacterContainer.MainCharacter.CurrentWeapon.MaxShells), _
                '                    New Point(1270 * Form2Draw.DisplayRectangle.Width / 1886, Form2Draw.DisplayRectangle.Height - 137 / 1886 * Form2Draw.DisplayRectangle.Width), _
                '                     &HFF244F7C)
                'DirectXText.DrawText(Nothing, String.Format("{0} {1}", "Schaden :", Container.CharacterContainer.MainCharacter.CurrentWeapon.Damage), _
                '                    New Point(1270 * Form2Draw.DisplayRectangle.Width / 1886, Form2Draw.DisplayRectangle.Height - (137 - 1.5 * FontSize) / 1886 * Form2Draw.DisplayRectangle.Width), _
                '                     &HFF244F7C)
                'DirectXText.DrawText(Nothing, String.Format("{0} {1} ", "Name :", Container.CharacterContainer.MainCharacter.CurrentWeapon.Name), _
                '                    New Point(1270 * Form2Draw.DisplayRectangle.Width / 1886, Form2Draw.DisplayRectangle.Height - (137 - 1.5 * FontSize * 2) / 1886 * Form2Draw.DisplayRectangle.Width), _
                '                     &HFF244F7C)
                DirectXText.DrawText(Nothing, Container.CharacterContainer.MainCharacter.PointToDraw.ToString, New Point(0, 0), Color.Black)
                DirectXText.DrawText(Nothing, New Point(Container.CharacterContainer.MainCharacter.PointToDraw.X - Container.CharacterContainer.MainCharacter.TextureSize.Width * Container.ImageScale / 2, Container.CharacterContainer.MainCharacter.PointToDraw.Y - Container.CharacterContainer.MainCharacter.TextureSize.Height * Container.ImageScale / 2).ToString, New Point(0, 15), Color.Black)
                DirectXDevice.EndScene()

                DirectXDevice.Present()

                If Container.CharacterContainer.MainCharacter.TakeScreen = True Then
                    TakeScreenshot()
                    Container.CharacterContainer.MainCharacter.TakeScreen = False
                End If
            End Sub

            Public Function GetFPS()
                If GetTickCount() - FPS_LastCheck >= 1000 Then
                    FPS_Current = FPS_Count
                    FPS_Count = 0 'setzt Counter zurück
                    FPS_LastCheck = GetTickCount()

                    ' Höchste erreichte FPS Zahl
                    If FPS_Current > FPS_Highest Then
                        FPS_Highest = FPS_Current
                    End If
                End If

                FPS_Count = FPS_Count + 1

                Form2Draw.Text = "fps > " & FPS_Current
            End Function
        End Class

        Public Class TextureContainer
            Dim TextureContainer As New Dictionary(Of String, Texture)
            Dim Container As Container
            Dim Scale As New Dictionary(Of String, Integer)
            Dim Device As Device

            Public Sub New(ByRef DirectXDevice As Device, ByVal _Container As Container)
                Device = DirectXDevice
                Container = _Container
            End Sub

            Public Sub LoadTile(ByVal TileList As List(Of Tile._Tile))
                For Each Tile As Tile._Tile In TileList
                    TextureContainer.Add(Tile.ID, TextureLoader.FromFile(Device, Tile.TexturePath, Tile.ImageSize.Width, Tile.ImageSize.Height, 0, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.None, Filter.None, Nothing))
                    Writer.Log.Write("Loaded Texture from " + Tile.ID)
                Next
            End Sub

            Public Sub LoadChar(ByVal Character As Character.ICharacter)
                TextureContainer.Add(Character.TextureID, TextureLoader.FromFile(Device, Character._DirectXTexturePath, Character.MainTextureSize.Width, Character.MainTextureSize.Height, 0, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.Triangle, Filter.None, Nothing))
                Writer.Log.Write("Loaded Texture from " + Character.TextureID)
            End Sub

            Public Sub LoadMob(ByVal Mob As Mob.IMob)
                TextureContainer.Add(Mob.SpecialInformation(Game.Mob.MobInformation.TextureID), TextureLoader.FromFile(Device, Mob.ImagePath, Mob.ImageSize.Width, Mob.ImageSize.Height, 0, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.Triangle, Filter.None, Nothing))
                Writer.Log.Write("Loaded Texture from " + Mob.SpecialInformation(Game.Mob.MobInformation.TextureID))
            End Sub

            Public Sub LoadInterface(ByVal Interfaces As Dictionary(Of String, String))

            End Sub

            Public Sub LoadOverlay(ByVal Overlays As List(Of Overlay.Overlay))
                For Each Overlay As Overlay.Overlay In Overlays
                    TextureContainer.Add(Overlay.ID, TextureLoader.FromFile(Device, Overlay.TexturePath, Overlay.ImageSize.Width, Overlay.ImageSize.Height, 0, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.Triangle, Filter.None, Nothing))
                    Writer.Log.Write("Loaded Texture from " + Overlay.ID)
                Next
            End Sub

            Public Function GetTexture(ByVal ID As String) As Texture
                Return TextureContainer(ID)
            End Function

            Public Function GetScale(ByVal ID As String) As Integer
                Return Scale(ID)
            End Function

            Private Function GetScaledTexture(ByVal Path As String, ByVal ScaleMultiplier As Integer)
                Dim Original As Bitmap = Bitmap.FromFile(Path)
                Writer.Log.Write("Scaled Texture from " + Path)
                Return Texture.FromBitmap(Device, New Bitmap(Original, New Size(Original.Width * ScaleMultiplier, Original.Height * ScaleMultiplier)), Usage.None, Pool.Managed)
            End Function
        End Class
    End Namespace

End Namespace