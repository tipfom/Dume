Imports System.Net
Imports System.IO
Namespace Global.Game
    Namespace Highscores
        Public Class HighscoreDatabase
            'Daten des Provider(dreamlo) > http://dreamlo.com/lb/uKUAEFGm_kqlj6namGyg-Afy3sd-qec0C6Zvyr9LL4QQ
            Private PrivateCode As String = "uKUAEFGm_kqlj6namGyg-Afy3sd-qec0C6Zvyr9LL4QQ"
            Private PublicCode As String = "555e089b6e51b61dbcc524c7"
            Private DomainURL As String = "http://dreamlo.com/lb/"

            Public Function UploadNewHighscore(ByVal Owner As String, ByVal Score As Integer, ByVal ScoreAdd As Integer, ByVal Comment As String) As Boolean
                Dim Uploader As New WebClient()
                'Löscht den Highscorewert,falls einer vorhanden ist
                Uploader.OpenRead(New Uri(DomainURL.ToString + PrivateCode.ToString + "/delete/" + Owner.ToString))
                'Erstellt einen StreamReader der den Stream des Webclients ausliest
                Dim Reader As New StreamReader(Uploader.OpenRead(New Uri(DomainURL.ToString + PrivateCode.ToString + "/add/" + Owner.ToString + "/" + Score.ToString + "/" + ScoreAdd.ToString + "/" + Comment)))
                If Reader.ReadToEnd() = "OK" Then
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Function DownloadHighscores() As String
                Dim Downloader As New WebClient()
                Dim Reader As New StreamReader(Downloader.OpenRead(New Uri(DomainURL.ToString + PublicCode.ToString + "/pipe/")))
                Return Reader.ReadToEnd()
            End Function

            Public Sub ClearAll()
                Dim Uploader As New WebClient()
                Uploader.OpenRead(New Uri(DomainURL.ToString + PrivateCode.ToString + "/clear/"))
            End Sub
        End Class

        Public Class HighscoreManager
            Inherits HighscoreDatabase

            Public Function FormatHighscores() As List(Of Highscore)
                Dim Returned As New List(Of Highscore)
                Dim SplittedHighscores() As String = Me.DownloadHighscores().Split(New Char() {"\n"}, System.StringSplitOptions.RemoveEmptyEntries)

                For i = 0 To SplittedHighscores.Length - 1
                    Dim HighscoreInfo() As String = SplittedHighscores(i).Split("|")
                    Returned.Add(New Highscore With {.Owner = HighscoreInfo(0), .Score = Integer.Parse(HighscoreInfo(1)), _
                                                     .CreatingDate = New DateTime(HighscoreInfo(4).Split("/")(2).Split(" ")(0), HighscoreInfo(4).Split("/")(0), HighscoreInfo(4).Split("/")(1), DateTime.Parse(HighscoreInfo(4).Split(" ")(1) + HighscoreInfo(4).Split(" ")(2)).ToLocalTime().Hour, HighscoreInfo(4).Split(" ")(1).Split(":")(1), HighscoreInfo(4).Split(" ")(1).Split(":")(2)), _
                                                     .Comment = HighscoreInfo(3), .ScoreAdd = HighscoreInfo(2)})
                Next

                Return Returned
            End Function

            Public Sub AddToConsole(ByVal DisplayColor As ConsoleColor)
                Dim FirstDisplayColor As ConsoleColor = Console.ForegroundColor
                Console.ForegroundColor = DisplayColor

                Console.WriteLine("")
                Console.WriteLine(" === Highscores ===")
                For Each Score As Highscore In Me.FormatHighscores
                    Console.WriteLine(Score.ToString)
                Next
                Console.WriteLine(" === Highscores ===")
                Console.WriteLine("")

                Console.ForegroundColor = FirstDisplayColor
            End Sub
        End Class

        Public Class Highscore
            Public Score As Integer
            Public Owner As String
            Public CreatingDate As DateTime
            Public Comment As String
            Public ScoreAdd As Integer

            Public Overrides Function ToString() As String
                Return Owner + " with " + Score.ToString + " / " + ScoreAdd.ToString + " reached on " + CreatingDate.ToString + " commented as '" + Comment + "'"
            End Function
        End Class
    End Namespace
End Namespace