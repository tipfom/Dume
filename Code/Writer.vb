Imports System.IO

Namespace Global.Game
    Public Class Writer

        Private Path As String
        Private WriteTime As Boolean

        Private Writer As StreamWriter

        Public Function Write(ByVal Content As String) As Boolean
            If WriteTime = True Then
                Writer.WriteLine(String.Format("{0:00}:{1:00}:{2:00} ", Date.Now.Hour, Date.Now.Minute, Date.Now.Second) + Content)
            Else
                Writer.WriteLine(Content)
            End If
            Writer.Flush()
            Return True
        End Function

        Private Sub Index(ByVal Index As Integer)
            Writer.Write(String.Format("{0:0000} ", Index))
            Writer.Flush()
        End Sub

        Private Sub Initialize()
            Writer = New StreamWriter(File.Create(Path))
        End Sub

        Public Shared ReadOnly Property Log As Writer
            Get
                Static Writes As Integer = -1
                Writes += 1
                If Not Directory.Exists("log") Then Directory.CreateDirectory("log")
                Static Writer As New Writer With {.Path = String.Format("log/{0:00}.{1:00}.{2:0000} {3:00}-{4:00}-{5:00}", Date.Now.Day, Date.Now.Month, Date.Now.Year, Date.Now.Hour, Date.Now.Minute, Date.Now.Second) + ".log", .WriteTime = True}
                If Writes = 0 Then
                    Writer.Initialize()
                    Writer.Index(Writes)
                    Writer.Write("Initialize LogWriter")
                    Writes += 1
                End If
                Writer.Index(Writes)
                Return Writer
            End Get
        End Property
    End Class
End Namespace
