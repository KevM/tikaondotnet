Using Tika on .Net
==================

This project is a simple example showing how to use the Java [Tika][1] library within a .Net application (via [IKVM][2]) to do text extraction from rich documents.

##Building##

There is build automation provided via a Ruby rake file. You need to have [Ruby] [3] installed and in your path.

###Dependencies###
Note : There is a dependency on the albacore gem.

`gem install albacore`


###To Build and Run Tests###

`rake`

[1]: http://tika.apache.org/
[2]: http://www.ikvm.net/
[3]: http://rubyinstaller.org/
