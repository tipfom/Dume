Namespace Global.Game
    Namespace Weapon
        Public Enum Direction
            Left
            Right
            Top '===== Laser Exklusiv =====
            Bottom
        End Enum

        Public Enum LaserPart
            Left
            Center
            Right
        End Enum

        Public Interface ISlimeBall
            ReadOnly Property Damage As Integer
            ReadOnly Property Name As String
            ReadOnly Property SnotIntervall As Integer
            ReadOnly Property SnotSpeed As Integer
            ReadOnly Property SnotsLeft As Integer

            Function Snot(ByVal Increase As Double, ByVal MouseLocation As Point) As SlimeBall.SlimeBall

            ReadOnly Property TextureRectangle As Rectangle 'Gibt das Rechteck des Overlays an

            ReadOnly Property Description As String
            ReadOnly Property Producer As String

            ReadOnly Property Container As Container
        End Interface


        Public Interface IPunch

        End Interface

        Public Interface ILaser
            ReadOnly Property Damage As Integer
            ReadOnly Property Name As String
            ReadOnly Property LaserIntervall As Integer
            ReadOnly Property LaserSpeed As Integer
            ReadOnly Property LaserLeft As Integer

            Function Laser(ByVal Direction As Direction, ByVal MouseLocation As Point) As Laser.Laser

            ReadOnly Property TextureRectangle(ByVal Part As LaserPart) As Rectangle 'Gibt das Rechteck des Overlays an

            ReadOnly Property Description As String
            ReadOnly Property Producer As String

            ReadOnly Property StandartSize As Size
            ReadOnly Property StackSize As Size
        End Interface

        Public Interface IAdvancedPunch

        End Interface
    End Namespace
End Namespace