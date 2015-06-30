Imports System.IO

Namespace Global.Game
    Namespace Overlay
        Public Class OverlayContainer
            Public ReadOnly Container As New List(Of Overlay)
            Public ReadOnly Overlays As New List(Of Overlay)

            Public Sub CheckForEvents(ByVal PressedButtons As List(Of Keys), ByVal MouseDown As Boolean)

            End Sub

            Public Sub Add(ByVal Name As String, Location As Point, ByVal Rotation As Double)
                For Each Overlay In Overlays
                    If Overlay.Name.ToUpper = Name.ToUpper Then
                        Container.Add(Overlay.Clone)
                        Container.Item(Container.Count - 1).Position = Location
                        Container.Item(Container.Count - 1).Rotation = Rotation

                        Writer.Log.Write("Spawned Overlay[" + Name + "] @" + Location.X.ToString + "|" + Location.Y.ToString + " with " + Rotation.ToString)
                        Exit For
                    End If
                Next
            End Sub

            Public Sub LoadFrom(ByVal Directory As String)
                For Each Overlay As DirectoryInfo In New DirectoryInfo(Directory).GetDirectories()
                    Overlays.Add(New Overlay(XDocument.Load(Overlay.FullName + "/Overlay.xml")))
                    Writer.Log.Write("Loaded Overlay from " + Overlay.FullName)
                Next
            End Sub
        End Class

        Public Class Overlay
            Implements ICloneable

            Dim OverlayConfig As XDocument
            Public Position As Point
            Public Mode As String

            Dim CurrentTexture As Integer = 0
            Dim Modes As New List(Of String)
            Dim StartIndex As New Dictionary(Of String, Integer)
            Dim CountIndex As New Dictionary(Of String, Integer)
            Dim SizeDictionary As New Dictionary(Of Integer, Size)
            Dim PositionDictionary As New Dictionary(Of Integer, Point)
            Dim IntervallDictionary As New Dictionary(Of String, Integer)

            Dim HitBoxes As New List(Of String)

            Dim WithEvents TextureChanger As New Timer

            Dim Rot As Double
            Dim DMG As Integer

            Sub New(ByVal Config As XDocument)
                OverlayConfig = Config
                Mode = OverlayConfig.<Overlay>.@StartMode
                For Each Texture In OverlayConfig.<Overlay>.<Resources>.Elements
                    Modes.Add(Texture.@Mode)
                    StartIndex.Add(Texture.@Mode, CurrentTexture)
                    Select Case Convert.ToBoolean(Texture.@Sprite)
                        Case True
                            CountIndex.Add(Texture.@Mode, OverlayConfig.<Overlay>.Elements(Texture.@SpritesheetElement).@Count)
                            IntervallDictionary.Add(Texture.@Mode, Texture.@TextureChangingIntervall)
                            For Each Sprite In OverlayConfig.<Overlay>.Elements(Texture.@SpritesheetElement).Elements
                                SizeDictionary.Add(CurrentTexture, New Size(OverlayConfig.<Overlay>.Elements(Texture.@SpritesheetElement).@Width, OverlayConfig.<Overlay>.Elements(Texture.@SpritesheetElement).@Height))
                                PositionDictionary.Add(CurrentTexture, New Point(Sprite.@X, Sprite.@Y))
                                CurrentTexture += 1
                            Next
                        Case False
                            CountIndex.Add(Texture.@Mode, 1)
                            SizeDictionary.Add(CurrentTexture, New Size(Texture.@Width, Texture.@Height))
                            PositionDictionary.Add(CurrentTexture, New Point(Texture.@X, Texture.@Y))
                            CurrentTexture += 1
                    End Select
                Next
                CurrentTexture = StartIndex(Mode)
                If CountIndex(Mode) > 1 Then
                    TextureChanger.Interval = IntervallDictionary(Mode)
                    TextureChanger.Start()
                End If
                For Each HitBox In OverlayConfig.<Overlay>.<HitBox>.Elements
                    Select Case HitBox.@Type.ToUpper
                        Case "RECTANGLE"
                            HitBoxes.Add(String.Format("{0};{1},{2};{3},{4}", HitBox.@Type, HitBox.@Width, HitBox.@Height, HitBox.@X, HitBox.@Y))
                        Case "CIRCLE"
                            HitBoxes.Add(String.Format("{0};{1};{3},{4}", HitBox.@Type, HitBox.@Radius, HitBox.@X, HitBox.@Y))
                    End Select
                Next
            End Sub

            ReadOnly Property Name As String
                Get
                    Return OverlayConfig.<Overlay>.@Name
                End Get
            End Property

            ReadOnly Property TexturePath As String
                Get
                    Return OverlayConfig.<Overlay>.<Resources>.@Image
                End Get
            End Property

            ReadOnly Property TextureSize As Size
                Get
                    Return SizeDictionary(CurrentTexture)
                End Get
            End Property

            ReadOnly Property TextureLocation As Point
                Get
                    Return PositionDictionary(CurrentTexture)
                End Get
            End Property

            ReadOnly Property ImageSize As Size
                Get
                    Return New Size(OverlayConfig.<Overlay>.<Resources>.@Width, OverlayConfig.<Overlay>.<Resources>.@Heigth)
                End Get
            End Property

            ReadOnly Property Visitable As Boolean
                Get
                    Return Convert.ToBoolean(OverlayConfig.<Overlay>.<Hitbox>.@Visitable)
                End Get
            End Property


            ReadOnly Property Permeability As Boolean
                Get
                    Return Convert.ToBoolean(OverlayConfig.<Overlay>.<Hitbox>.@ProjectilePermeable)
                End Get
            End Property

            Public ReadOnly Property ID
                Get
                    Return OverlayConfig.<Overlay>.@ID
                End Get
            End Property

            Public Function CheckCollusion(ByVal CollusionPoint As Point) As Boolean
                For Each HitBox As String In HitBoxes
                    Dim Meta() As String = HitBox.Split(";")
                    Dim LocalHitBox As Rectangle = New Rectangle(New Point(Meta(2).Split(",")(0) - Me.TextureSize.Width / 2 + Me.Position.X, Meta(2).Split(",")(1) - Me.TextureSize.Height / 2 + Me.Position.Y), New Size(Meta(1).Split(",")(0), Meta(1).Split(",")(1)))

                    Select Case Meta(0).ToUpper
                        Case "RECTANGLE"
                            If Math.Abs((CollusionPoint.X - LocalHitBox.Location.X - LocalHitBox.Width / 2) * Math.Cos(Me.Rot) + (CollusionPoint.Y - LocalHitBox.Location.Y - LocalHitBox.Height / 2) * Math.Sin(Me.Rot)) <= LocalHitBox.Width / 2 Then
                                If Math.Abs(-(CollusionPoint.X - LocalHitBox.Location.X - LocalHitBox.Width / 2) * Math.Sin(Me.Rot) + (CollusionPoint.Y - LocalHitBox.Location.Y - LocalHitBox.Height / 2) * Math.Cos(Me.Rot)) <= LocalHitBox.Height / 2 Then
                                    Return True
                                End If
                            End If
                        Case "CIRCLE"
                            If (Me.Position.X - CollusionPoint.X) ^ 2 + (Me.Position.Y - CollusionPoint.Y) ^ 2 <= Meta(1) ^ 2 Then
                                Return True
                            End If
                    End Select
                Next
                Return False
            End Function

            Public Function CheckInteraction(ByVal Interaction As EventArgs)

            End Function

            Private Sub TextureChanger_Tick(sender As Object, e As System.EventArgs) Handles TextureChanger.Tick
                CurrentTexture += 1
                If CurrentTexture >= StartIndex(Mode) + CountIndex(Mode) Then
                    CurrentTexture = StartIndex(Mode)
                End If
            End Sub

            Public Property Rotation As Double
                Set(value As Double)
                    Rot = value
                End Set
                Get
                    Return Rot
                End Get
            End Property

            Public Function Clone() As Object Implements System.ICloneable.Clone
                Dim Overlay As New Overlay(Me.OverlayConfig)
                Return Overlay
            End Function

            Public Property Damage As Integer
                Get
                    Return DMG
                End Get
                Set(value As Integer)
                    DMG = value
                End Set
            End Property
        End Class
    End Namespace
End Namespace