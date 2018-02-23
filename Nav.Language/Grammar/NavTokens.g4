lexer grammar NavLexer;

channels {
    TriviaChannel
}

TaskKeyword            : 'task';
TaskrefKeyword         : 'taskref';
InitKeyword            : 'init';
EndKeyword             : 'end';
ChoiceKeyword          : 'choice';
DialogKeyword          : 'dialog';
ViewKeyword            : 'view';
ExitKeyword            : 'exit';
OnKeyword              : 'on';
IfKeyword              : 'if';
ElseKeyword            : 'else';
SpontaneousKeyword     : 'spontaneous';
SpontKeyword           : 'spont';
DoKeyword              : 'do';
ResultKeyword          : 'result';
ParamsKeyword          : 'params';
BaseKeyword            : 'base';
NamespaceprefixKeyword : 'namespaceprefix';
UsingKeyword           : 'using';
CodeKeyword            : 'code';
GeneratetoKeyword      : 'generateto';
NotimplementedKeyword  : 'notimplemented';
AbstractmethodKeyword  : 'abstractmethod';
DonotinjectKeyword     : 'donotinject';
GoToEdgeKeyword        :   '-->';
ModalEdgeKeyword       :   '*->' | 'o->';
NonModalEdgeKeyword    :   '==>';

fragment
NL
    :   '\r\n' | '\n' | '\r'
    ;

Whitespace
    :
    (   ' '
    |   '\u0009' // horizontal tab character
    |   '\u000B' // vertical tab character
    |   '\u000C' // form feed character
    )+
    -> channel(TriviaChannel)
    ;

SingleLineComment
    :   '//' .*? (NL | EOF)
    -> channel(TriviaChannel)
    ;

MultiLineComment
    :   '/*' .*? '*/'
    -> channel(TriviaChannel)
    ;

NewLine
    : NL
    -> channel(TriviaChannel)
    ;

Identifier
    :   IdentifierCharacter+
    ;

fragment
IdentifierCharacter
    :   'a'..'z'
    |   'A'..'Z'
    |   '_'
    |   '0'..'9'
    |   'Ä'|'Ö'|'Ü'|'ä'|'ö'|'ü'|'ß'|'.'
    ;

OpenBrace
    :   '{'
    ;

CloseBrace
    :   '}'
    ;

OpenParen
    :  '('
    ;

CloseParen
    :   ')'
    ;

OpenBracket
    :   '['
    ;

CloseBracket
    :   ']'
    ;

LessThan
    :   '<'
    ;

GreaterThan
    :   '>'
    ;

Semicolon
    :   ';'
    ;

Comma
    :   ','
    ;

Colon
    :   ':'
    ;

StringLiteral
    :   '\"' (StringLiteralCharacter)* '\"'
    ;

fragment
StringLiteralCharacter
    :   ~( '\"' | '\u000D' | '\u000A' | '\u2028' | '\u2029')
    ;

Unknown
  :  .
  -> channel(TriviaChannel)
  ;
