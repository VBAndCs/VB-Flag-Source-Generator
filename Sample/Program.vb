Imports System.IO
Imports System.Text

Module Program
    Sub Main(args As String())
        dim myFileState = FileState.ReadOnly + FileState.Hidden
        myFileState = myFileState.SetFlag(FileState.System)
        myFileState = myFileState.UnsetFlag(FileState.Hidden)
        Console.WriteLine(myFileState.ToString())

        dim myFeeling = Feeling.Hungery + Feeling.Sleepy
        Console.WriteLine (myFeeling.IsSet(Feeling.Happy))
        myFeeling = myFeeling.ToggleFlag(Feeling.Hungery) + Feeling.Angery
        Console.WriteLine(myFeeling.ToString())
    End Sub
End Module
