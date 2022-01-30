# utf8parsepos

Small library that parses bytes with UTF-8 to characters.
Unique for this implementation is that the original byte-position of each character is retained.
This can be useful when searching, so a search-result can say where a string like Räksmörgås was in the orignal byte-data.

But if just "changing bytes to text" is the goal, this is the wrong library
