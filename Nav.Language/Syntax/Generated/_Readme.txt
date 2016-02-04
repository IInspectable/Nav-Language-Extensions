===============================

Die tt Dateien dienen zur manuellen Codegenerierung, und müssen bei Bedarf ausgeführt werden.
Das sollte nicht wirklich oft passieren, da der Syntaxbaum an sich sehr "stabil" sein dürfte.

Der Code kann nicht automatisch während des Kompilierens generiert werden, da die TT Files selbst vom 
resultierenden Assembly abhängig sind.

Das bedeuted aber auch, dass es in diesem Projekt keine Abhängigkeiten von nicht generierten Code zu
generierten Code geben darf.