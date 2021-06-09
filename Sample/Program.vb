Imports System.IO
Imports System.Text

Module Program
    Sub Main(args As String())
        dim myFileState = FileState.ReadOnly + FileState.Hidden
        myFileState = myFileState.SetFlag(FileState.System)
        myFileState = myFileState.UnsetFlag(FileState.Hidden)
        Console.WriteLine(myFileState.ToString())
    End Sub
End Module
