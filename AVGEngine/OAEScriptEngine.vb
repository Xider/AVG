﻿Imports System.IO
Public Class OAEScriptEngine
    Dim MainPath As String
    Dim ScriptPath As String
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Int32, ByVal lpFileName As String) As Int32
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Int32
    '定义读取配置文件函数
    Public Function GetINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As String
        Dim Str As String = ""
        Str = LSet(Str, 256)
        GetPrivateProfileString(Section, AppName, lpDefault, Str, Len(Str), FileName)
        Return Microsoft.VisualBasic.Left(Str, InStr(Str, Chr(0)) - 1)
    End Function
    '定义写入配置文件函数
    Public Function WriteINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As Long
        WriteINI = WritePrivateProfileString(Section, AppName, lpDefault, FileName)
    End Function
    Public Sub Preproccess() '预处理器
        Dim iniText_Main As String = "", iniText_Script As String = "", TextTrimed As String = ""
        Dim re As StreamReader, Deal() As String '←对获得的main.ini进行分割处理的数组
        Dim IncludedName() As String '获得名字
        Dim CountWAttr, i As Integer '储存windowAttr的个数,用来获得included的文件个数
        Dim WhetherFindWAT As Integer = 0 '是否找到第一个<include>的位置,获得此位置后会取得第一个<include>所在的数组
        Dim FinalResult As String = ""
        'Dim sw As System.IO.StreamWriter
        'sw = New System.IO.StreamWriter(MainPath, True)
        'sw.WriteLine(Form1.TextBox1.Text)
        'sw.Close()
        're = New StreamReader(MainPath, System.Text.Encoding.GetEncoding("GB2312"))
        'iniText_Main = re.ReadToEnd
        're.Close()
        re = New StreamReader(ScriptPath, System.Text.Encoding.GetEncoding("GB2312"))
        iniText_Script = re.ReadToEnd
        re.Close()
        iniText_Script.Trim()
        Deal = Split(iniText_Script, Chr(13) + Chr(10))
        For i = 0 To UBound(Deal)
            'WhetherFindWAT = InStr()
        Next
        IncludedName = Split(iniText_Script, "<include>")
        CountWAttr = UBound(IncludedName)
        TextTrimed = (iniText_Script + iniText_Main).TrimEnd()
    End Sub
    Sub New(ByVal path As String) '构造函数
        MainPath = path '脚本目录
    End Sub
    Public Function GetScene(ByVal WindowSectionName As String) As OAEScene '获得类型为window的各项属性
        GetScene.bgImage = GetINI("scene-" + WindowSectionName, "bgImage", "", MainPath)
        GetScene.bgMusic = GetINI("scene-" + WindowSectionName, "bgMusic", "", MainPath)
        GetScene.itemList = GetINI("scene-" + WindowSectionName, "itemList", "", MainPath)
        GetScene.name = GetINI("scene-" + WindowSectionName, "name", "", MainPath)
    End Function
    Public Function GetItem(ByVal ItemSectionName As String) As OAEItem '获得类型为item的各项属性
        GetItem.type = GetINI("item-" + ItemSectionName, "type", "", MainPath)
        GetItem.height = ItNull(GetINI("item-" + ItemSectionName, "height", "0", MainPath))
        GetItem.locX = ItNull(GetINI("item-" + ItemSectionName, "locX", "0", MainPath))
        GetItem.locY = ItNull(GetINI("item-" + ItemSectionName, "locY", "0", MainPath))
        GetItem.width = ItNull(GetINI("item-" + ItemSectionName, "width", "0", MainPath))
        GetItem.name = ItemSectionName
        GetItem.NormalImage = GetINI("item-" + ItemSectionName, "NormalImage", "", MainPath)
        GetItem.HoverImage = GetINI("item-" + ItemSectionName, "HoverImage", "", MainPath)
        GetItem.ClickImage = GetINI("item-" + ItemSectionName, "ClickImage", "", MainPath)
        GetItem.HoverAction = GetINI("item-" + ItemSectionName, "HoverAction", "", MainPath)
        GetItem.ClickAction = GetINI("item-" + ItemSectionName, "ClickAction", "", MainPath)
        GetItem.ClickFont = GetINI("item-" + ItemSectionName, "ClickFont", "", MainPath)
        GetItem.ClickText = GetINI("item-" + ItemSectionName, "ClickText", "", MainPath)
        GetItem.HoverFont = GetINI("item-" + ItemSectionName, "HoverFont", "", MainPath)
        GetItem.HoverText = GetINI("item-" + ItemSectionName, "HoverText", "", MainPath)
        GetItem.TextMaxHeight = ItNull(GetINI("item-" + ItemSectionName, "TextMaxHeight", "", MainPath))
        GetItem.TextMaxWidth = ItNull(GetINI("item-" + ItemSectionName, "TextMaxWidth", "", MainPath))
        GetItem.NormalText = GetINI("item-" + ItemSectionName, "NormalText", "", MainPath)
        GetItem.NormalFont = GetINI("item-" + ItemSectionName, "NormalFont", "", MainPath)
    End Function
    Public Function GetInitInfo() As OAEInitInfo
        GetInitInfo.height = ItNull(GetINI("init", "height", "", MainPath))
        GetInitInfo.width = ItNull(GetINI("init", "width", "", MainPath))
        GetInitInfo.author = GetINI("init", "author", "", MainPath)
        GetInitInfo.gameName = GetINI("init", "gameName", "", MainPath)
    End Function
    Function GetImageRes(ByVal res As String) As Image
        'image(bgImage)->image-bgImage
        Dim ImagePath As String, IfImage() As String
        res = Replace(res, "(", "-")
        res = Replace(res, ")", "")
        IfImage = Split(res, "-")
        If (IfImage(0) = "image") Then
            ImagePath = GetINI(res, "path", "", MainPath)
            Return Image.FromFile(Application.StartupPath + "//script//" + ImagePath)
        Else
            Throw New Exception("Image resources type unmatch:" + IfImage(0))
        End If
    End Function
    Function GetSoundPath(ByVal res As String) As String
        Dim IfSound() As String
        res = Replace(res, "(", "-")
        res = Replace(res, ")", "")
        IfSound = Split(res, "-")
        If IfSound(1) = "sound" Then
            Return GetINI(res, "path", "", MainPath)
        Else
            Throw New Exception("Image resources type unmatch:" + IfSound(1))
        End If
    End Function
    Function ItNull(ByVal Res As String) As Integer
        If Res = "" Then
            Return 0
        Else : Return Res
        End If
    End Function
End Class