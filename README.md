# EncodingConverter
A simple Unicode encoding converter written in C#.

I wrote this little utility because I needed something to convert UTF-16 files to UTF-8 so that git would treat them as text files instead of binary files.
It looks at the Byte Order Mark (BOM) of the file to determine which UTF encoding the file is using.

This is not intended to be a general purpose encoding converter or character set detector.
There are better utilities that already do those things, such as iconv and Mozilla's Universal Character Set Detector.

http://www.gnu.org/savannah-checkouts/gnu/libiconv/
http://mxr.mozilla.org/mozilla/source/extensions/universalchardet/src/
https://code.google.com/p/ude/

