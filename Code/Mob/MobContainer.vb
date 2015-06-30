Imports Game.Mob

Namespace Global.Game
    Public Class MobContainer
        Dim Container As New List(Of ISwarm)

        Public Sub Add(ByVal Swarm As ISwarm)
            Container.Add(Swarm)
        End Sub

        Public Function Contains(ByVal Swarm As ISwarm) As Boolean
            If Container.Contains(Swarm) Then
                Return True
            Else
                Return False
            End If
        End Function

        Public ReadOnly Property List As List(Of ISwarm)
            Get
                Return Container
            End Get
        End Property

        Public Sub MoveAll()
            For Each Swarm As ISwarm In Container
                Swarm.Move()
            Next
        End Sub

        Public Sub MoveSpecific(ByVal Swarm As ISwarm)
            If Container.Contains(Swarm) Then
                Container.Item(Container.IndexOf(Swarm)).Move()
            End If
        End Sub
    End Class
End Namespace