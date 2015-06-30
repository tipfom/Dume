Namespace Global.Game
    Public Enum Style
        Shadowed
    End Enum

    Public Class ChatBox

        Public ChatBox As New ListBox
        Public WithEvents TextBox As New TextBox
        Public Background As New Panel

        Dim ChatContainer As New List(Of String)

        Dim Form As GameForm

        Dim Font As New Font("TAHOMA", 10, FontStyle.Regular)

        Sub New(ByVal Size As Size, ByVal Style As Style, ByVal _Form As GameForm)
            Form = _Form

            Form.Controls.Add(ChatBox)
            Form.Controls.Add(TextBox)
            Form.Controls.Add(Background)

            Select Case Style
                Case Game.Style.Shadowed
                    ChatBox.BorderStyle = BorderStyle.None
                    TextBox.BorderStyle = BorderStyle.None
                    Background.BorderStyle = BorderStyle.None

                    ChatBox.BackColor = System.Drawing.SystemColors.MenuHighlight
                    TextBox.BackColor = System.Drawing.SystemColors.MenuHighlight
                    Background.BackColor = System.Drawing.SystemColors.WindowFrame
            End Select

            ChatBox.Font = Font
            TextBox.Font = Font

            Background.Size = Size
            Background.Location = New Point(Form.Width / 2 - Size.Width / 2, Form.Height / 2 - Size.Height / 2)

            TextBox.Size = New Size(Background.Width - 10, TextBox.PreferredSize.Height)
            TextBox.Location = New Point(Background.Location.X + 5, Background.Bottom - TextBox.Height - 5)

            ChatBox.Size = New Size(Background.Width - 10, Background.Height - 10 - TextBox.PreferredSize.Height)
            ChatBox.Location = New Point(Background.Location.X + 5, Background.Location.Y + 5)

            Background.Visible = False
            TextBox.Visible = False
            ChatBox.Visible = False
        End Sub

        Public Sub Show()
            Background.Visible = True
            TextBox.Visible = True
            ChatBox.Visible = True

            TextBox.Focus()
        End Sub

        Public Sub Disapear()
            Background.Visible = False
            TextBox.Visible = False
            ChatBox.Visible = False

            Form.Focus()
        End Sub

        Public Function Add(ByVal Message As String, ByVal sender As String)
            ChatContainer.Add(String.Format("[{0}] {1} > {2}", String.Format("{0}:{1}", DateTime.Now.Hour, DateTime.Now.Minute), sender, Message))
            Update()
            Return True
        End Function

        Private Sub Update()
            ChatBox.Items.Clear()
            For Each ChatEntry As String In ChatContainer
                ChatBox.Items.Add(ChatEntry)
            Next
        End Sub

        Private Sub TextBox_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TextBox.KeyDown
            Select Case e.KeyValue
                Case Keys.Enter
                    If Not TextBox.Text = "" Then
                        Form.ChatContainer.Add(TextBox.Text)
                        Me.Add(TextBox.Text, "You")
                        TextBox.Clear()
                    End If
                Case Keys.Escape
                    Me.Disapear()
                    Form.Focus()
            End Select
        End Sub
    End Class
End Namespace