Imports Game.Tile

Namespace Global.Game
    Structure KI
        Public Class Heap(Of Type As IHeapItem(Of Type))
            'https://www.youtube.com/watch?v=3Dw5d7PlcTM&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=4
            'Klasse zum beschleunigen der Berechnungsgeschwindigkeit
            '(Stammbäume)

            Dim Items As Type()
            Dim ItemCount As Integer

            Public ReadOnly Property Count As Integer
                Get
                    Return ItemCount
                End Get
            End Property

            Public Sub New(ByVal MaximumSize As Integer)
                ReDim Items(MaximumSize)
            End Sub

            Public Sub Add(ByVal Item As Type)
                Item.HeapIndex = ItemCount
                Items(ItemCount) = Item
                SortUp(Item)
                ItemCount += 1
            End Sub

            Public Function Contains(ByVal Item As Type)
                Return Equals(Items(Item.HeapIndex), Item)
            End Function

            Public Sub UpdateItem(ByVal Item As Type)
                SortUp(Item)
            End Sub

            Public Function RemoveFirst() As Type
                Dim FirstItem As Type = Items(0)
                ItemCount -= 1
                Items(0) = Items(ItemCount)
                Items(0).HeapIndex = 0
                SortDown(Items(0))
                Return FirstItem
            End Function

            Private Sub SortDown(ByVal Item As Type)
                While True
                    Dim ChildOnLeft As Integer = Item.HeapIndex * 2 + 1
                    Dim ChildOnRight As Integer = Item.HeapIndex * 2 + 2
                    Dim SwapIndex As Integer = 0

                    If ChildOnLeft < ItemCount Then
                        SwapIndex = ChildOnLeft

                        If ChildOnRight < ItemCount Then
                            If Items(ChildOnLeft).CompareTo(Items(ChildOnRight)) < 0 Then
                                SwapIndex = ChildOnRight
                            End If
                        End If

                        If Item.CompareTo(Items(SwapIndex)) < 0 Then
                            Swap(Item, Items(SwapIndex))
                        Else
                            Exit While
                        End If
                    Else
                        Exit While
                    End If
                End While
            End Sub

            Private Sub SortUp(ByVal Item As Type)
                Dim ParentIndex As Integer = (Item.HeapIndex - 1) / 2

                While True
                    Dim ParentItem As Type = Items(ParentIndex)
                    If Item.CompareTo(ParentItem) > 0 Then
                        Swap(Item, ParentItem)
                    Else
                        Exit While
                    End If
                    ParentIndex = (Item.HeapIndex - 1) / 2
                End While
            End Sub

            Private Sub Swap(ItemOne As Type, ItemTwo As Type)
                Items(ItemOne.HeapIndex) = ItemTwo
                Items(ItemTwo.HeapIndex) = ItemOne
                Dim TempIndex As Integer = ItemOne.HeapIndex
                ItemOne.HeapIndex = ItemTwo.HeapIndex
                ItemTwo.HeapIndex = TempIndex
            End Sub
        End Class

        Public Interface IHeapItem(Of HeapItem)
            Inherits IComparable(Of HeapItem)
            Property HeapIndex As Integer
        End Interface

        Public Class PathFinding
            Dim Map As Map

            Sub New(ByVal _Map As Map)
                Map = _Map
            End Sub

            Public Function GetNeighbours(ByVal Tile As _Tile) As List(Of Tile._Tile)
                Dim Neighbours As New List(Of _Tile)

                For x As Integer = -1 To 1
                    For y As Integer = -1 To 1
                        Dim xOnMap As Integer = Tile.MapLocation.X + x
                        Dim yOnMap As Integer = Tile.MapLocation.Y + y

                        If xOnMap >= 0 And yOnMap >= 0 And xOnMap <= Map.MapSize.Width And yOnMap <= Map.MapSize.Height Then
                            Neighbours.Add(Map.GetTileOf(xOnMap, yOnMap))
                        End If
                    Next
                Next
                Return Neighbours
            End Function

            Public Function GetDistance(ByVal TileOne As _Tile, ByVal TileTwo As _Tile) As Integer
                Dim Distance As New Size
                Distance.Width = Math.Abs(TileOne.MapLocation.X - TileTwo.MapLocation.X)
                Distance.Height = Math.Abs(TileOne.MapLocation.Y - TileTwo.MapLocation.Y)

                Return Distance.Width + Distance.Height
            End Function

            Private Function GetPath(ByVal StartTile As _Tile, ByVal EndTile As _Tile) As List(Of _Tile)
                Dim Path As New List(Of _Tile)
                Dim CurrentTile As _Tile = EndTile

                Path.Add(EndTile)

                While CurrentTile <> StartTile
                    Path.Add(Map.GetTileOf(CurrentTile.ParentLocation.X, CurrentTile.ParentLocation.Y))
                    CurrentTile = Map.GetTileOf(CurrentTile.ParentLocation.X, CurrentTile.ParentLocation.Y)
                End While

                Return Path
            End Function

            Public Function FindPath(ByVal StartLocation As Point, ByVal TargetLocation As Point) As List(Of _Tile)
                Dim StartTile As _Tile = Map.GetTileOf(StartLocation.X, StartLocation.Y)
                Dim EndTile As _Tile = Map.GetTileOf(TargetLocation.X, TargetLocation.Y)
                Dim CurrentTile As _Tile

                Dim OpenList As New Heap(Of _Tile)(Map.MapSize.Width * Map.MapSize.Height)
                Dim ClosedList As New HashSet(Of _Tile)

                OpenList.Add(StartTile)

                While OpenList.Count > 0
                    CurrentTile = OpenList.RemoveFirst

                    ClosedList.Add(CurrentTile)

                    If CurrentTile = EndTile Then
                        Return GetPath(StartTile, EndTile)
                    End If

                    For Each Neighbour As _Tile In GetNeighbours(CurrentTile)
                        If Neighbour.Visitable = False Or ClosedList.Contains(Neighbour) Then
                            Continue For
                        End If

                        Dim newMovementCostToNeighbour As Integer = CurrentTile.gCost + GetDistance(CurrentTile, Neighbour)
                        If newMovementCostToNeighbour < Neighbour.gCost Or Not OpenList.Contains(Neighbour) Then
                            Neighbour.gCost = newMovementCostToNeighbour
                            Neighbour.hCost = GetDistance(Neighbour, EndTile)
                            Neighbour.ParentLocation = CurrentTile.MapLocation

                            If Not OpenList.Contains(Neighbour) Then
                                OpenList.Add(Neighbour)
                                OpenList.UpdateItem(Neighbour)
                            End If
                        End If
                    Next
                End While
                Return New List(Of _Tile)
            End Function
        End Class
    End Structure

End Namespace
