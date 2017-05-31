**Nav Language diagnostic errors**

|  Id    | Category | Severity |  Message |
|--------|----------|----------|----------| 
|Nav0003 | Semantic| Error| Source file needs to be saved before include directives can be processed|
|Nav0004 | Semantic| Error| File '{0}' not found|
|Nav0005 | Semantic| Warning| The file '{0}' has some errors|
|Nav0010 | Semantic| Error| Cannot resolve task '{0}'|
|Nav0011 | Semantic| Error| Cannot resolve node '{0}'|
|Nav0012 | Semantic| Error| Cannot resolve exit '{0}'|
|Nav0020 | Semantic| Error| A task with the name '{0}' is already declared|
|Nav0021 | Semantic| Error| A connection point with the name '{0}' is already declared|
|Nav0022 | Semantic| Error| A node with the name '{0}' is already declared|
|Nav0023 | Semantic| Error| An outgoing edge for Trigger '{0}' is already declared|
|Nav0024 | Semantic| Error| An outgoing edge for exit '{0}' is already declared|
|Nav0025 | Semantic| Error| No outgoing edge for exit '{0}' declared|
|Nav0026 | Semantic| Error| Trigger '{0}' is already declared|
|Nav0100 | Semantic| Error| The task '{0}' must not contain leaving edges|
|Nav0101 | Semantic| Error| An exit node must not contain leaving edges|
|Nav0102 | Semantic| Error| An end node must not contain leaving edges|
|Nav0103 | Semantic| Error| An init node must not contain incoming edges|
|Nav0104 | Semantic| Error| Choice node '{0}' must only be reached by -->|
|Nav0105 | Semantic| Error| Exit node '{0}' must only be reached by -->|
|Nav0106 | Semantic| Error| End node '{0}' must only be reached by -->|
|Nav0107 | Semantic| Warning| The exit node '{0}' has no incoming edges|
|Nav0108 | Semantic| Warning| The end node has no incoming edges|
|Nav0109 | Semantic| Warning| The init node '{0}' has no outgoing edges|
|Nav0110 | Semantic| Error| '{0}' edge not allowed here because '{1}' is reachable from init node '{2}'|
|Nav0111 | Semantic| Warning| The choice node '{0}' has no incoming edges|
|Nav0112 | Semantic| Warning| The choice node '{0}' has no outgoing edges|
|Nav0113 | Semantic| Warning| The task node '{0}' has no incoming edges|
|Nav0114 | Semantic| Warning| The dialog node '{0}' has no incoming edges|
|Nav0115 | Semantic| Warning| The dialog node '{0}' has no outgoing edges|
|Nav0116 | Semantic| Warning| The view node '{0}' has no incoming edges|
|Nav0117 | Semantic| Warning| The view node '{0}' has no outgoing edges|
|Nav0200 | Semantic| Error| Trigger not allowed after init|
|Nav0201 | Semantic| Error| Spontaneous not allowed in signal trigger|
|Nav0202 | Semantic| Error| 'spontaneous' only allowed after view and init nodes|
|Nav0203 | Semantic| Warning| Trigger not allowed after choice|
|Nav0220 | Semantic| Error| Conditions are only supported after Init and Choice nodes|
|Nav0221 | Semantic| Error| Only if conditions allowed in exit transitions|
|Nav2000 | Semantic| Error| Identifier expected|
