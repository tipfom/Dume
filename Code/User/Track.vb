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
    Public Class TrackMath

        Public Const Tan675 As Double = 2.414214
        Public Const Tan225 As Double = 0.414214

        Public Shared Function GetIncrease(ByVal Ursprung As Point, ByVal Point As Point) As Double
            Dim Anstieg As Double

            Anstieg = (Point.Y - Ursprung.Y) / (Point.X - Ursprung.X)

            Return Anstieg
        End Function

        Public Shared Function IsRight(ByVal X1 As Integer, ByVal X2 As Integer) As Boolean
            Dim Point1IsRightOfPoint2 As Boolean = False

            If X1 <= X2 Then Point1IsRightOfPoint2 = True

            Return Point1IsRightOfPoint2
        End Function

        Public Shared Function IsLeft(ByVal X1 As Integer, ByVal X2 As Integer) As Boolean
            Dim Point1IsRightOfPoint2 As Boolean = False

            If X1 >= X2 Then Point1IsRightOfPoint2 = True

            Return Point1IsRightOfPoint2
        End Function
    End Class

    Public Class Tracker
        Dim WithEvents Form As GameForm

        Dim WithEvents PChar As Character.ICharacter

        Dim ActivatedKeys As New List(Of Keys)

        Dim Increase As Double

        Dim DrawedImageSize As Integer

        'Um den unendlichrenn bug zu fixen D:

        Dim _Cursor(3) As Cursor

        Public Event UpdateScreen()

        Declare Function LoadCursor Lib "user32.dll" Alias "LoadCursorFromFileA" (ByVal Pfad As String) As IntPtr

        Dim LeftClick, RightClick As Boolean

        Dim MousePoint As Point

        Sub New(ByVal TForm As GameForm, ByVal CurrentChar As Character.ICharacter)
            Form = TForm
            PChar = CurrentChar

            _Cursor(0) = New Cursor(LoadCursor(XDocument.Load("Content/Config/Main.xml").<Game>.<Configuration>.<Cursor>.<Normal>.Value.ToString))
            _Cursor(1) = New Cursor(LoadCursor(XDocument.Load("Content/Config/Main.xml").<Game>.<Configuration>.<Cursor>.<Attack>.Value.ToString))
            _Cursor(2) = New Cursor(LoadCursor(XDocument.Load("Content/Config/Main.xml").<Game>.<Configuration>.<Cursor>.<Sneaking>.Value.ToString))
            _Cursor(3) = New Cursor(LoadCursor(XDocument.Load("Content/Config/Main.xml").<Game>.<Configuration>.<Cursor>.<SneakyAttacking>.Value.ToString))
        End Sub

        Private Sub MainKeyDown(ByVal sender As Object, ByVal Key As KeyEventArgs) Handles Form.KeyDown
            If Key.KeyValue = Keys.W And ActivatedKeys.Contains(Keys.W) = False Then ActivatedKeys.Add(Keys.W)
            If Key.KeyValue = Keys.A And ActivatedKeys.Contains(Keys.A) = False Then ActivatedKeys.Add(Keys.A)
            If Key.KeyValue = Keys.S And ActivatedKeys.Contains(Keys.S) = False Then ActivatedKeys.Add(Keys.S)
            If Key.KeyValue = Keys.D And ActivatedKeys.Contains(Keys.D) = False Then ActivatedKeys.Add(Keys.D)
            If Key.KeyValue = Keys.ShiftKey And ActivatedKeys.Contains(Keys.Shift) = False Then ActivatedKeys.Add(Keys.Shift)
            If Key.KeyValue = Keys.Escape Then Form.Close()
            If Key.KeyValue = Keys.F12 Then
                PChar.TakeScreen = True
            End If
            If Key.KeyValue = Keys.P Then
                PChar.Container.Form.CommandContainer.Add("add me on skype")
            End If
            If Key.KeyValue = Keys.O Then
                PChar.Container.Form.ChatContainer.Add(InputBox("ChatMessage"))
            End If
            If Key.KeyValue = Keys.T Then PChar.Transform()
            If Key.KeyValue = Keys.Enter Then
                Form.Chat.Show()
            End If
        End Sub

        Private Sub MainKeyUp(ByVal sender As Object, ByVal Key As KeyEventArgs) Handles Form.KeyUp
            If Key.KeyValue = Keys.W Then ActivatedKeys.Remove(Keys.W)
            If Key.KeyValue = Keys.A Then ActivatedKeys.Remove(Keys.A)
            If Key.KeyValue = Keys.S Then ActivatedKeys.Remove(Keys.S)
            If Key.KeyValue = Keys.D Then ActivatedKeys.Remove(Keys.D)
            If Key.KeyValue = Keys.ShiftKey Then ActivatedKeys.Remove(Keys.Shift)
        End Sub

        Private Sub Form_MouseClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Form.MouseClick
            Select Case e.Button
                Case MouseButtons.Middle
                    PChar.Transform()
            End Select
        End Sub

        Private Sub Form_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Form.MouseDown
            Select Case e.Button
                Case MouseButtons.Left
                    LeftClick = True
                Case MouseButtons.Right
                    RightClick = True
            End Select
        End Sub

        Private Sub Form_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Form.MouseMove
            MousePoint = e.Location
        End Sub


        Private Sub Form_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Form.MouseUp
            Select Case e.Button
                Case MouseButtons.Left
                    LeftClick = False
                Case MouseButtons.Right
                    RightClick = False
            End Select
        End Sub

        Public Sub DoActions()
            SetCharFacing()
            DoKeyInteraction()
            DoMouseInteraction()
            SetCursor()
            SetCharRotation()
        End Sub

        Private Sub SetCharRotation()
            Dim TempRotation As Double
            If Not Double.IsInfinity(Increase) Then
                If TrackMath.IsRight(PChar.MiddlePointDrawedOnScreen.X, Cursor.Position.X - Form.Location.X - 9) = True Then
                    If Increase > 0 Then
                        TempRotation = 180 * (Math.PI / 180) - Math.Atan(Increase)
                    Else
                        TempRotation = 180 * (Math.PI / 180) - Math.Atan(Increase)
                    End If
                End If
                If TrackMath.IsLeft(PChar.MiddlePointDrawedOnScreen.X, _
                          Cursor.Position.X - Form.Location.X - 9) _
                      = True Then
                    If Increase > 0 Then
                        TempRotation = 360 * (Math.PI / 180) - Math.Atan(Increase)
                    Else
                        TempRotation = Math.Abs(Math.Atan(Increase))

                    End If
                End If
                PChar.Rotation = 270 * (Math.PI / 180) + TempRotation
            ElseIf Double.IsPositiveInfinity(Increase) Then
                PChar.Rotation = (Math.PI / 180)
            ElseIf Double.IsNegativeInfinity(Increase) Then
                PChar.Rotation = 180 * (Math.PI / 180)
            End If
        End Sub

        Private Sub SetCharFacing()
            Increase = -TrackMath.GetIncrease(PChar.MiddlePointDrawedOnScreen, New Point(Cursor.Position.X - Form.Location.X - 9, Cursor.Position.Y - Form.Location.Y - 32))
        End Sub

        Private Sub DoKeyInteraction()
            If ActivatedKeys.Count > 0 Then
                Select Case ActivatedKeys(0)
                    Case Keys.W
                        PChar.Move(0, -PChar.Speed)
                    Case Keys.S
                        PChar.Move(0, PChar.Speed)
                    Case Keys.D
                        PChar.Move(PChar.Speed, 0)
                    Case Keys.A
                        PChar.Move(-PChar.Speed, 0)
                End Select
            Else
                PChar.Standing = True
            End If
Done:
        End Sub

        Private Sub DoMouseInteraction()
            If LeftClick = True Then
                PChar.AddShot(-TrackMath.GetIncrease(PChar.MiddlePointDrawedOnScreen, MousePoint), Character.WeaponSlot.Primary, MousePoint)
            End If
            If RightClick = True Then
                PChar.AddShot(-TrackMath.GetIncrease(PChar.MiddlePointDrawedOnScreen, MousePoint), Character.WeaponSlot.Secondary, MousePoint)
            End If
        End Sub

        Private Sub SetCursor()
            If LeftClick = True Or RightClick = True Then
                If ActivatedKeys.Contains(Keys.Shift) Then
                    Form.Cursor = _Cursor(3)
                    PChar.Sneaking = True
                Else
                    Form.Cursor = _Cursor(1)
                    PChar.Sneaking = False
                End If
            Else
                If ActivatedKeys.Contains(Keys.Shift) Then
                    Form.Cursor = _Cursor(2)
                    PChar.Sneaking = True
                Else
                    Form.Cursor = _Cursor(0)
                    PChar.Sneaking = False
                End If
            End If
        End Sub

        Public ReadOnly Property CurrentIncrease As Double
            Get
                Return Increase
            End Get
        End Property

    End Class
End Namespace