# Exiv2CmdNet
Library for add/modify EXIF data of image file. Written in C#. This library make use of [Exiv2 command line tool]( http://www.exiv2.org/) 
(use only some commands). Sample project that use Exiv2CmdNet is provided (Exiv2CmdNetSample) so you can have a look at how to use it in code.
This library was created from Visual Studio 2015.

## Features
- Add EXIF tags to image file.
- Edit EXIF tags that exist in image file.
- Get EXIF tags value as string, datetime.
- Geo tag image file.

**Note:** some of [Exif tags which type is byte data](http://www.exiv2.org/tags.html) can not be set 
because I don't know how to set it via command line. Sorry for that. 

## Dependencies
Make sure you have installed
- Microsoft Visual C++ 2015 Runtime Library x86
- Microsoft Visual C++ 2015 Runtime Library x64

## How to use
1. Download as zip.
1. Extract it.
1. In your current solution (In solution explorer), Right click solution -> Add -> Existing project...
1. Choose Exiv2CmdNet.csproj (In folder you extracted it)
1. In solution explorer, expand project that you want to use the library. 
1. Right click References -> Add References...
1. In Reference Manager left pane, choose Projects -> Solution
1. In right pane, check/tick Exiv2CmdNet
1. click ok.
1. Now you can start using Exiv2CmdNet library in your project.
