Namespace Global.Game
    Public Class Container
        Public ReadOnly MobContainer As MobContainer
        Public ReadOnly TileContainer As Tile.TileContainer
        Public ReadOnly OverlayContainer As Overlay.OverlayContainer
        Public ReadOnly CharacterContainer As CharacterContainer
        Public ReadOnly SnotContainer As New List(Of Weapon.SlimeBall.SlimeBall)
        Public ReadOnly LaserContainer As New List(Of Weapon.Laser.Laser)
        Dim SnotContainerRemove As New List(Of Weapon.SlimeBall.SlimeBall)
        Public Form As GameForm
        Public Tracker As Tracker
        Public Map As Map
        Public ImageSize As Integer
        Public ImageScale As Double

        Sub New(ByRef _MobContainer As MobContainer, ByRef _TileContainer As Tile.TileContainer, ByRef _OverlayContainer As Overlay.OverlayContainer, ByVal _CharacterContainer As CharacterContainer)
            MobContainer = _MobContainer
            TileContainer = _TileContainer
            OverlayContainer = _OverlayContainer
            CharacterContainer = _CharacterContainer
            Writer.Log.Write("Initialized Container")
        End Sub

        Public Sub Remove(ByVal Item As Weapon.SlimeBall.SlimeBall)
            SnotContainerRemove.Add(Item)
        End Sub

        Public Sub Flush()
            For Each Item In SnotContainerRemove
                SnotContainer.Remove(Item)
            Next
            SnotContainerRemove.Clear()
        End Sub
    End Class

End Namespace