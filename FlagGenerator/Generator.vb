' Created By, Mohammad Hamdy Ghanem, 
' Egypt, 2020

Imports System.IO
Imports System.Text
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text

<Generator>
Class FlagGenerator
    Implements ISourceGenerator


    Shared FlagTemplate As XCData = <![CDATA[
Class #FlagName#
#Flags#
    Const MaxValue As Uinteger = #MaxValue#
    Public Shared ReadOnly None As New #FlagName# ("None", 0)
    Public Shared ReadOnly All As New #FlagName#("All", MaxValue)

    Private ReadOnly Value As UInteger

    Private Sub New(value As UInteger)
        Me.Value = If(value > MaxValue, value And MaxValue, value)
    End Sub

    Private Sub New(name As String, value As UInteger)
        _Name = name
        Me.Value = If(value > MaxValue, value And MaxValue, value)
    End Sub

    Public Shared ReadOnly Property Flags As #FlagName#() = {#FlagList#}
    Public Shared ReadOnly Property FlagNames As String() = {#StrFlagList#}
    Public Shared ReadOnly Property FlagValues As UInteger() = {#ValueList#}
    Public ReadOnly Property Name As String

    Public Property OnFlags As List(Of #FlagName#)
        Get
            Dim lstFlags As New List(Of #FlagName#)
            For Each flag In Flags
                If (Value And flag.Value) > 0 Then lstFlags.Add(flag)
            Next
            Return lstFlags
        End Get
        Set()
            SetFlags(Value.ToArray())
        End Set
    End Property

    Public Property OffFlags As List(Of #FlagName#)
        Get
            Dim lstFlags As New List(Of #FlagName#)
            For Each flag In Flags
                If (Value And flag.Value) = 0 Then lstFlags.Add(flag)
            Next
            Return lstFlags
        End Get
        Set()
            UnsetFlags(Value.ToArray)
        End Set
    End Property

    Default Public ReadOnly Property IsSet(flag As #FlagName#) As Boolean
        Get
            Return (Value And flag.Value) > 0
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return ToString("+")
    End Function

    Public Overloads Function ToString(Separator As String) As String
        If Value = 0 Then Return None.Name
        If Value = MaxValue Then Return All.Name
        Dim sb As New Text.StringBuilder
        For Each flag In Flags
            If (Value And flag.Value) > 0 Then
                If sb.Length > 0 Then sb.Append(Separator)
                sb.Append(flag.Name)
            End If
        Next
        Return sb.ToString
    End Function

    Public Function ToInteger() As UInteger
        Return Value
    End Function

   Public Function SetFlag(flag As #FlagName#) As #FlagName#
        Return New #FlagName#(Value Or flag.Value)
    End Function

    Public Function SetFlags(ParamArray flags() As #FlagName#) As #FlagName#
        If flags Is Nothing OrElse flags.Length = 0 Then Return new #FlagName#(Value)

        Dim v = Value
        For Each flag In flags
            v = v Or flag.Value
        Next
        Return New #FlagName#(v)
    End Function

    Public Function SetAllExcxept(ParamArray flags() As #FlagName#) As #FlagName#
        If flags Is Nothing OrElse flags.Length = 0 Then Return new #FlagName#(Value)

        Dim v = MaxValue
        For Each flag In flags
            v -= flag.Value
        Next
        Return New #FlagName#(v)
    End Function

   Public Function UnsetFlag(flag As #FlagName#) As #FlagName#
        Return New #FlagName#(Value And Not flag.Value)
    End Function

    Public Function UnsetFlags(ParamArray flags() As #FlagName#) As #FlagName#
        If flags Is Nothing OrElse flags.Length = 0 Then Return new #FlagName#(Value)

        Dim v = Value
        For Each flag In flags
            v = v And Not flag.Value
        Next
        Return New #FlagName#(v)
    End Function

    Public Function UnsetAllExcxept(ParamArray flags() As #FlagName#) As #FlagName#
        If flags Is Nothing OrElse flags.Length = 0 Then Return new #FlagName#(Value)

        Dim v As UInteger = 0
        For Each flag In flags
            v += flag.Value
        Next
        Return New #FlagName#(v)
    End Function

   Public Function ToggleFlag(flag As #FlagName#) As #FlagName#
        Return New #FlagName#(Value Xor flag.Value)
    End Function

    Public Function ToggleFlags(ParamArray flags() As #FlagName#) As #FlagName#
        If flags Is Nothing OrElse flags.Length = 0 Then Return new #FlagName#(Value)

        Dim v = Value
        For Each flag In flags
            v = v Xor flag.Value
        Next
        Return New #FlagName#(v)
    End Function

    Public Function ToggleAllFlags() As #FlagName#
        Return New #FlagName#(Value Xor MaxValue)
    End Function

    Public Function AreAllSet(ParamArray flags() As #FlagName#) As Boolean
        If flags Is Nothing OrElse flags.Length = 0 Then Return Value = MaxValue

        For Each flag In flags
            If (Value And flag.Value) = 0 Then Return False
        Next
        Return True
    End Function

    Public Function AreAllUnset(ParamArray flags() As #FlagName#) As Boolean
        If flags Is Nothing OrElse flags.Length = 0 Then Return Value = 0

        For Each flag In flags
            If (Value And flag.Value) <> 0 Then Return False
        Next
        Return True
    End Function

    Public Function AreAnySet(ParamArray flags() As #FlagName#) As Boolean
        If flags Is Nothing OrElse flags.Length = 0 Then Return Value > 0

        For Each flag In flags
            If (Value And flag.Value) <> 0 Then Return True
        Next
        Return False
    End Function

    Public Function AreAnyUnset(ParamArray flags() As #FlagName#) As Boolean
        If flags Is Nothing OrElse flags.Length = 0 Then Return Value < MaxValue

        For Each flag In flags
            If (Value And flag.Value) = 0 Then Return True
        Next
        Return False
    End Function

#Region "Operators"

    Public Shared Widening Operator CType(value As UInteger) As #FlagName#
        Return New #FlagName#(value)
    End Operator

    Public Shared Widening Operator CType(flag As #FlagName#) As UInteger
        Return flag.Value
    End Operator


    Public Shared Operator +(flag As #FlagName#, value As Integer) As #FlagName#
        Return flag.SetFlags(new #FlagName#(value))
    End Operator

    Public Shared Operator -(flag As #FlagName#, value As Integer) As #FlagName#
        Return flag.UnsetFlags(new #FlagName#(value))
    End Operator

    Public Shared Operator Or(flag As #FlagName#, value As UInteger) As #FlagName#
        Return New #FlagName#(flag.Value Or value)
    End Operator

    Public Shared Operator And(flag As #FlagName#, value As UInteger) As #FlagName#
        Return New #FlagName#(flag.Value And value)
    End Operator

    Public Shared Operator Xor(flag As #FlagName#, value As UInteger) As #FlagName#
        Return New #FlagName#(flag.Value Xor value)
    End Operator

    Public Shared Operator Not(flag As #FlagName#) As #FlagName#
        Return New #FlagName#(Not flag.Value)
    End Operator

    Public Shared Operator IsTrue(flag As #FlagName#) As Boolean
        Return flag.Value > 0
    End Operator

    Public Shared Operator IsFalse(flag As #FlagName#) As Boolean
        Return flag.Value = 0
    End Operator

    Public Shared Operator =(flag As #FlagName#, value As UInteger) As Boolean
        Return flag.Value = value
    End Operator

    Public Shared Operator <>(flag As #FlagName#, value As UInteger) As Boolean
        Return flag.Value <> value
    End Operator

    Public Shared Operator >(flag As #FlagName#, value As UInteger) As Boolean
        Return flag.Value > value
    End Operator

    Public Shared Operator <(flag As #FlagName#, value As UInteger) As Boolean
        Return flag.Value < value
    End Operator

    Public Shared Operator >=(flag As #FlagName#, value As UInteger) As Boolean
        Return flag.Value >= value
    End Operator

    Public Shared Operator <=(flag As #FlagName#, value As UInteger) As Boolean
        Return flag.Value <= value
    End Operator

    Public Shared Operator +(flag1 As #FlagName#, flag2 As #FlagName#) As #FlagName#
        Return flag1.SetFlags(flag2)
    End Operator

    Public Shared Operator -(flag1 As #FlagName#, flag2 As #FlagName#) As #FlagName#
        Return flag1.UnsetFlags(flag2)
    End Operator
    Public Shared Operator Or(flag1 As #FlagName#, flag2 As #FlagName#) As #FlagName#
        Return New #FlagName#(flag1.Value Or flag2.Value)
    End Operator

    Public Shared Operator And(flag1 As #FlagName#, flag2 As #FlagName#) As #FlagName#
        Return New #FlagName#(flag1.Value And flag2.Value)
    End Operator

    Public Shared Operator Xor(flag1 As #FlagName#, flag2 As #FlagName#) As #FlagName#
        Return New #FlagName#(flag1.Value Xor flag2.Value)
    End Operator

    Public Shared Operator =(flag1 As #FlagName#, flag2 As #FlagName#) As Boolean
        Return flag1.Value = flag2.Value
    End Operator

    Public Shared Operator <>(flag1 As #FlagName#, flag2 As #FlagName#) As Boolean
        Return flag1.Value <> flag2.Value
    End Operator

    Public Shared Operator >(flag1 As #FlagName#, flag2 As #FlagName#) As Boolean
        Return flag1.Value > flag2.Value
    End Operator

    Public Shared Operator <(flag1 As #FlagName#, flag2 As #FlagName#) As Boolean
        Return flag1.Value < flag2.Value
    End Operator

    Public Shared Operator >=(flag1 As #FlagName#, flag2 As #FlagName#) As Boolean
        Return flag1.Value >= flag2.Value
    End Operator

    Public Shared Operator <=(flag1 As #FlagName#, flag2 As #FlagName#) As Boolean
        Return flag1.Value <= flag2.Value
    End Operator
#End Region
End Class
]]>

    Public Sub Initialize(context As GeneratorInitializationContext) Implements ISourceGenerator.Initialize

    End Sub

    Public Sub Execute(context As GeneratorExecutionContext) Implements ISourceGenerator.Execute
        Try
            Dim flagFiles = From file In context.AdditionalFiles
                            Where file.Path.ToLower().EndsWith(".txt")

            For Each flagFile In flagFiles
                Dim generatorDocuments = New List(Of (documentName As String, source As String))
                Dim text = flagFile.GetText().ToString()
                Dim lines = text.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                For Each line In lines
                    Dim result = ParseFlagSyntax(line)
                    If result.HasValue Then
                        Dim flagInfo = result.Value
                        Dim flagCode = GenerateFlag(flagInfo.Modifier, flagInfo.Name, flagInfo.Flags)
                        context.AddSource(flagInfo.Name & "Flag", SourceText.From(flagCode, Encoding.UTF8))
                    End If
                Next
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function ParseFlagSyntax(line As String) As (Modifier$, Name$, Flags As String())?
        Dim pos2 = line.IndexOf("(")
        If pos2 < 6 Then Return Nothing
        Dim flags = line.Substring(pos2 + 1).Trim(" "c, ")"c).Split({" "c, ","c}, StringSplitOptions.RemoveEmptyEntries)
        Dim pos1 = line.IndexOf("flag", StringComparison.OrdinalIgnoreCase)
        If pos1 = -1 Then Return Nothing
        Dim modifier = If(pos1 > 0, line.Substring(0, pos1).Trim(), "")
        Dim name = line.Substring(pos1 + 4, pos2 - pos1 - 4).Trim()
        Return (modifier, Name, flags)
    End Function

    Private Function GenerateFlag(modifier As String, flagName As String, flags() As String) As String
        Dim sbTemplate As New StringBuilder(modifier + " ")
        sbTemplate.Append(FlagTemplate.Value.TrimStart(vbLf))
        sbTemplate.Replace(vbCrLf, vbCr)
        sbTemplate.Replace(vbLf, vbCr)
        sbTemplate.Replace(vbCr, vbCrLf)

        sbTemplate.Replace("#FlagName#", flagName)

        Dim L = flags.Length
        Dim MaxValue = 2 ^ L - 1
        sbTemplate.Replace("#MaxValue#", MaxValue)

        Dim sbFlags As New StringBuilder
        Dim sbFlagList As New StringBuilder
        Dim sbStrFlagList As New StringBuilder
        Dim sbValueFlagList As New StringBuilder

        For i = 0 To L - 1
            Dim flag = ChrW(34) & flags(i) & ChrW(34)
            Dim value = 2 ^ i

            sbFlags.AppendLine($"    Public Shared ReadOnly {flags(i)} As new {flagName}({flag}, {value})")
            If sbFlagList.Length > 0 Then
                sbFlagList.Append(", ")
                sbStrFlagList.Append(", ")
                sbValueFlagList.Append(", ")
            End If
            sbFlagList.Append(flags(i))
            sbStrFlagList.Append(flag)
            sbValueFlagList.Append(value)
        Next
        sbTemplate.Replace("#Flags#", sbFlags.ToString)
        sbTemplate.Replace("#FlagList#", sbFlagList.ToString)
        sbTemplate.Replace("#StrFlagList#", sbStrFlagList.ToString)
        sbTemplate.Replace("#ValueList#", sbValueFlagList.ToString)

        Return sbTemplate.ToString
    End Function

End Class