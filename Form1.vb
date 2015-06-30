Public Class Form1
    Dim Main As New Game.Main(Me)
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Main.LoadConfigsFromFile("Config/StartConfig.xml")
        Main.Compile()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Main.Close()
    End Sub
End Class
