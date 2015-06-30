Namespace Global.Game
    Namespace Weapon

        Namespace SlimeBall
            Public Class SlimeBall
                Public ReadOnly Speed As Integer
                Public ReadOnly Name As String
                Public ReadOnly Texture As Rectangle

                Public Alive As Boolean

                Public Location As Point
                Dim Increase As Double
                Dim YCross As Integer
                Dim Direction As Direction

                Dim Source As ISlimeBall

                Public Sub New(ByVal _Source As ISlimeBall, ByVal StartLocation As Point, ByVal TextureRectangle As Rectangle, ByVal _Direction As Direction, ByVal _Increase As Double)
                    Source = _Source
                    Location = StartLocation
                    Speed = Source.SnotSpeed
                    Name = Source.Name
                    Texture = TextureRectangle
                    Alive = True
                    Increase = -_Increase
                    If Not Double.IsInfinity(Increase) Then
                        YCross = StartLocation.Y + _Increase * StartLocation.X
                    End If
                    Direction = _Direction
                End Sub

                Public Sub Move()
                    If Not Double.IsInfinity(Increase) Then
                        Select Case Direction
                            Case Weapon.Direction.Left
                                Location.X -= Math.Abs(Math.Cos(Math.Atan2(Increase, 1)) * Speed)
                            Case Weapon.Direction.Right
                                Location.X += Math.Abs(Math.Cos(Math.Atan2(Increase, 1)) * Speed)
                        End Select
                        Location.Y = Increase * Location.X + YCross
                        If Location.X >= 0 And Location.Y >= 0 And Location.X < Source.Container.Map.UnIndexedMapSize.Width * Source.Container.ImageSize And Location.Y < Source.Container.Map.UnIndexedMapSize.Height * Source.Container.ImageSize Then
                            If Source.Container.Map.GetTileOf(Location.X \ Source.Container.ImageSize, Location.Y \ Source.Container.ImageSize).Permeable = False Then
                                Alive = False
                            End If
                        Else
                            Alive = False
                        End If
                    Else

                    End If
                End Sub
            End Class

            Public Class FirstSlimeBall
                Implements ISlimeBall

                Dim SnotConfig As XDocument = XDocument.Load("Content/Weapon/NormalSnot.xml")
                Dim Character As Character.ICharacter

                Dim StartPointIndex As New Dictionary(Of Character.CharCollection.Direction, Point)

                Dim CanSnot As Boolean = True
                Dim WithEvents ReSlimer As New Timer

                Public Sub New(ByVal _Character As Game.Character.ICharacter)
                    Character = _Character

                    For Each StartPoint In SnotConfig.<SlimeBall>.<Data>.<StartPoint>.Elements
                        StartPointIndex.Add([Enum].Parse(GetType(Character.CharCollection.Direction), StartPoint.@Direction), New Point(StartPoint.@X, StartPoint.@Y))
                    Next

                    ReSlimer.Interval = Convert.ToInt32(SnotConfig.<SlimeBall>.<Data>.<ReSnot>.@SnotSpeed)
                End Sub

                Public ReadOnly Property Damage As Integer Implements ISlimeBall.Damage
                    Get
                        Return SnotConfig.<SlimeBall>.<Data>.@Damage
                    End Get
                End Property

                Public ReadOnly Property Description As String Implements ISlimeBall.Description
                    Get
                        Return SnotConfig.<SlimeBall>.<Description>.Value
                    End Get
                End Property

                Public ReadOnly Property Name As String Implements ISlimeBall.Name
                    Get
                        Return SnotConfig.<SlimeBall>.@Name
                    End Get
                End Property

                Public ReadOnly Property Producer As String Implements ISlimeBall.Producer
                    Get
                        Return SnotConfig.<SlimeBall>.@Producer
                    End Get
                End Property

                Public ReadOnly Property SnotIntervall As Integer Implements ISlimeBall.SnotIntervall
                    Get
                        Return SnotConfig.<SlimeBall>.<Data>.<Reload>.@FireSpeed
                    End Get
                End Property

                Public ReadOnly Property SnotsLeft As Integer Implements ISlimeBall.SnotsLeft
                    Get
                        Return Convert.ToInt32(Double.PositiveInfinity)
                    End Get
                End Property

                Public ReadOnly Property SnotSpeed As Integer Implements ISlimeBall.SnotSpeed
                    Get
                        Return SnotConfig.<SlimeBall>.<Data>.@SnotSpeed
                    End Get
                End Property

                Public ReadOnly Property TextureRectangle As System.Drawing.Rectangle Implements ISlimeBall.TextureRectangle
                    Get
                        Return New Rectangle(0, 0, 0, 0)
                    End Get
                End Property

                Public Function Snot(ByVal Increase As Double, ByVal MouseLocation As Point) As SlimeBall Implements ISlimeBall.Snot
                    If CanSnot Then
                        CanSnot = False
                        ReSlimer.Enabled = True
                        If TrackMath.IsLeft(Character.MiddlePointDrawedOnScreen.X, MouseLocation.X) = True Then
                            Return New SlimeBall(Me, StartPointIndex(Character.Direction) + Character.PointToDraw - New Point(Character.TextureSize.Width / 2, Character.TextureSize.Height / 2), New Rectangle(SnotConfig.<SlimeBall>.<Resource>.@X, SnotConfig.<SlimeBall>.<Resource>.@Y, SnotConfig.<SlimeBall>.<Resource>.@Width, SnotConfig.<SlimeBall>.<Resource>.@Height), Direction.Left, -TrackMath.GetIncrease(Character.MiddlePointDrawedOnScreen, MouseLocation))
                        Else
                            Return New SlimeBall(Me, StartPointIndex(Character.Direction) + Character.PointToDraw - New Point(Character.TextureSize.Width / 2, Character.TextureSize.Height / 2), New Rectangle(SnotConfig.<SlimeBall>.<Resource>.@X, SnotConfig.<SlimeBall>.<Resource>.@Y, SnotConfig.<SlimeBall>.<Resource>.@Width, SnotConfig.<SlimeBall>.<Resource>.@Height), Direction.Right, -TrackMath.GetIncrease(Character.MiddlePointDrawedOnScreen, MouseLocation))
                        End If
                    Else
                        Return Nothing
                    End If
                End Function

                Public ReadOnly Property Container As Container Implements ISlimeBall.Container
                    Get
                        Return Character.Container
                    End Get
                End Property

                Private Sub ReSlimer_Tick(sender As Object, e As System.EventArgs) Handles ReSlimer.Tick
                    CanSnot = True
                    ReSlimer.Enabled = False
                End Sub
            End Class
        End Namespace
    End Namespace
End Namespace