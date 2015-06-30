Namespace Global.Game
    Namespace Weapon
        Namespace Laser
            Public Class Laser
                Dim Direction As Direction

                Private Location As Point

                Public Source As ILaser
                Dim Stack As Integer

                Public ReadOnly TextureRotation As Double = 0

                Dim Size(2) As Size

                Public Sub New(ByVal SummonDirection As Direction, ByVal _Source As ILaser, ByVal StartLocation As Point, ByVal _Stack As Integer)
                    Stack = _Stack
                    Source = _Source
                    Size(0) = New Size(Source.TextureRectangle(LaserPart.Left).Width, Stack * Source.StackSize.Height + Source.StandartSize.Height)
                    Size(2) = New Size(Source.TextureRectangle(LaserPart.Right).Width, Stack * Source.StackSize.Height + Source.StandartSize.Height)
                    Size(1) = New Size(Stack * Source.StackSize.Width + Source.StandartSize.Width, Stack * Source.StackSize.Height + Source.StandartSize.Height)
                    Direction = SummonDirection
                    If Direction = Weapon.Direction.Top Or Direction = Weapon.Direction.Bottom Then TextureRotation = 90 * (Math.PI / 180)
                    Location = StartLocation
                End Sub

                Public ReadOnly Property Position(ByVal Part As LaserPart) As Point
                    Get
                        Select Case Part
                            Case LaserPart.Left
                                If Direction = Weapon.Direction.Left Or Direction = Weapon.Direction.Right Then
                                    Return New Point(Location.X - Size(1).Width / 2 - Size(0).Width / 2, Location.Y)
                                Else
                                    Return New Point(Location.X, Location.Y - Size(1).Width / 2 - Size(0).Width / 2)
                                End If
                            Case LaserPart.Center
                                Return Location
                            Case LaserPart.Right
                                If Direction = Weapon.Direction.Left Or Direction = Weapon.Direction.Right Then
                                    Return New Point(Location.X + Size(1).Width / 2 + Size(2).Width / 2, Location.Y)
                                Else
                                    Return New Point(Location.X, Location.Y + Size(1).Width / 2 + Size(2).Width / 2)
                                End If
                        End Select
                    End Get
                End Property

                ReadOnly Property DrawnSize(ByVal Part As LaserPart) As Size
                    Get
                        Select Case Part
                            Case LaserPart.Left
                                Return Size(0)
                            Case LaserPart.Center
                                Return Size(1)
                            Case LaserPart.Right
                                Return Size(2)
                        End Select
                    End Get
                End Property

                Public Sub Move()
                    Select Case Direction
                        Case Weapon.Direction.Left
                            Location.X -= Source.LaserSpeed
                        Case Weapon.Direction.Right
                            Location.X += Source.LaserSpeed
                        Case Weapon.Direction.Top
                            Location.Y -= Source.LaserSpeed
                        Case Weapon.Direction.Bottom
                            Location.Y += Source.LaserSpeed
                    End Select
                End Sub
            End Class

            Public Class NormalLaser
                Implements ILaser

                Dim Config As XDocument = XDocument.Load("Content/Weapon/NormalLaser.xml")
                Dim Character As Character.ICharacter

                Public Sub New(ByVal _Character As Character.ICharacter)
                    Character = _Character
                End Sub

                Public ReadOnly Property Damage As Integer Implements ILaser.Damage
                    Get
                        Return Config.<Laser>.<Data>.@Damage
                    End Get
                End Property

                Public ReadOnly Property Description As String Implements ILaser.Description
                    Get
                        Return Config.<Laser>.<Description>.Value
                    End Get
                End Property

                Public ReadOnly Property LaserIntervall As Integer Implements ILaser.LaserIntervall
                    Get
                        Return Config.<Laser>.<Data>.<Reload>.@FireSpeed
                    End Get
                End Property

                Public ReadOnly Property LaserLeft As Integer Implements ILaser.LaserLeft
                    Get
                        Return Integer.MaxValue
                    End Get
                End Property

                Public ReadOnly Property LaserSpeed As Integer Implements ILaser.LaserSpeed
                    Get
                        Return Config.<Laser>.<Data>.@LaserSpeed
                    End Get
                End Property

                Public ReadOnly Property Name As String Implements ILaser.Name
                    Get
                        Return Config.<Laser>.@Name
                    End Get
                End Property

                Public ReadOnly Property Producer As String Implements ILaser.Producer
                    Get
                        Return Config.<Laser>.@Producer
                    End Get
                End Property

                Public ReadOnly Property TextureRectangle(ByVal Part As LaserPart) As System.Drawing.Rectangle Implements ILaser.TextureRectangle
                    Get
                        Select Case Part
                            Case LaserPart.Left
                                Return New Rectangle(Config.<Laser>.<Resource>.<Left>.@X, Config.<Laser>.<Resource>.<Left>.@Y, Config.<Laser>.<Resource>.<Left>.@Width, Config.<Laser>.<Resource>.<Left>.@Height)
                            Case LaserPart.Center
                                Return New Rectangle(Config.<Laser>.<Resource>.<Center>.@X, Config.<Laser>.<Resource>.<Center>.@Y, Config.<Laser>.<Resource>.<Center>.@Width, Config.<Laser>.<Resource>.<Center>.@Height)
                            Case LaserPart.Right
                                Return New Rectangle(Config.<Laser>.<Resource>.<Right>.@X, Config.<Laser>.<Resource>.<Right>.@Y, Config.<Laser>.<Resource>.<Right>.@Width, Config.<Laser>.<Resource>.<Right>.@Height)
                        End Select
                    End Get
                End Property

                Public Function Laser(Direction As Direction, MouseLocation As Point) As Laser Implements ILaser.Laser
                    Return New Laser(Direction, Me, Character.PointToDraw, 100)
                End Function

                Public ReadOnly Property StackSize As System.Drawing.Size Implements ILaser.StackSize
                    Get
                        Return New Size(Config.<Laser>.<Stack>.@WidthIncrease, Config.<Laser>.<Stack>.@HeightIncrease)
                    End Get
                End Property

                Public ReadOnly Property StandartSize As System.Drawing.Size Implements ILaser.StandartSize
                    Get
                        Return New Size(Config.<Laser>.<Stack>.@WidthStandart, Config.<Laser>.<Stack>.@HeightStandart)
                    End Get
                End Property
            End Class

        End Namespace
    End Namespace
End Namespace