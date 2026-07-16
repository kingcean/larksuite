using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Text;

namespace LarkSuite.OapiModels;

public enum LarkContentBlockType
{
    Page = 1,
    Text = 2,
    Heading1 = 3,
    Heading2 = 4,
    Heading3 = 5,
    Heading4 = 6,
    Heading5 = 7,
    Heading6 = 8,
    Heading7 = 9,
    Heading8 = 10,
    Heading9 = 11,
    Bullet = 12,
    Ordered = 13,
    Code = 14,
    Quote = 15,
    Equation = 16,
    ToDo = 17,
    BaseTable = 18,
    Highlight = 19,
    Conversation = 20,
    Uml = 21,
    Separator = 22,
    File = 23,
    Columns = 24,
    Column = 25,
    WebPage = 26,
    Image = 27,
    Widget = 28,
    Mind = 29,
    SheetTable = 30,
    GridTable = 31,
    TableCell = 32,
    View = 33,
    ReferenceContainer = 34,
    Task = 35,
    OkrBlock = 36,
    OkrObjective = 37,
    OkrKeyResult = 38,
    OkrProgress = 39,
    DocsWidget = 40,
    JiraIssue = 41,
    WikiContents1 = 42,
    Whiteboard = 43,
    Agenda = 44,
    AgendaItem = 45,
    AgendaSubject = 46,
    AgendaContent = 47,
    LinkPreview = 48,
    SyncBlock = 49,
    ReferenceSyncBlock = 50,
    WikiContents2 = 51,
    AiTemplate = 52,
    Unsupported = 999,
}

public enum LarkContentTextAlign
{
    Left = 1,
    Center = 2,
    Right = 3,
}

public enum LarkContentTextIndentationLevel
{
    NoIndent = 0,
    OneLevelIndent = 1,
}
