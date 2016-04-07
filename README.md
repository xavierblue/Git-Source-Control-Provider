Git Source Control Provider 2015
================================

[![Join the chat at https://gitter.im/jzoss/Git-Source-Control-Provider](https://badges.gitter.im/jzoss/Git-Source-Control-Provider.svg)](https://gitter.im/jzoss/Git-Source-Control-Provider?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Introduction
------------
** This Visual Studio Extensions integrates Git with Visual Studio solution explorer. ** 



![solution explorer](http://gitscc.codeplex.com/Project/Download/FileDownload.aspx?DownloadId=123874)

Features
--------
* Display file status in solution explorer and solution navigator
* Multiple repository support.  
* Display repository status e.g. in the middle of merging, patching, rebase and bisecting
* Enable/disable plug-in through visual studio's source control plug-in selection
* No source code control information stored in solution or project file
* Initialize new git repository and generate .gitignore 
* Integrates with [Git for Windows](http://code.google.com/p/msysgit)
* Integrates with [Git Extensions](http://code.google.com/p/gitextensions)
* Integrates with [TortoiseGit](http://code.google.com/p/tortoisegit)
* [Git - Pending Changes Tool Window] (http://gitscc.codeplex.com/wikipage?title=Commit%20Changes)
* [Git - View History Tool Window] (http://gitscc.codeplex.com/wikipage?title=View%20History)
* Options page

How to use
----------
* Install [Git for Windows](http://code.google.com/p/msysgit), or [Git Extensions](http://code.google.com/p/gitextensions), or [TortoiseGit](http://code.google.com/p/tortoisegit).
* Run Visual Studio. 
* Go to Tools | Extension Manager, search online gallery for Git Source Control Provider and install. Or Install From [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/51e11ccc-6334-4873-912d-bf5025eb115d) 
* Go to Tools | Options, Select Source Control.
* Select Git Source Control Provider from the drop down list, and click OK.
* Open your solution controlled by Git to see the file's status.
* Right click within solution explorer and select "Git". If Git for Windows, Git Extensions or TortoiseGit are installed, their commands are listed in the menu.


How to contribute 
----------
If you like this plugin there is a few way you can help out.

* Review It! - I'm not asking for donations or anything, but if you do use this and like it.. Please head over to the [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/51e11ccc-6334-4873-912d-bf5025eb115d) and review it.  Good reviews refill my nerd powers that power all those late night programming sessions. They also make me feel all warm and fuzzy! 
* Suggest Features / Point out bugs. 
* Test It - Check out the [Releases](https://github.com/jzoss/Git-Source-Control-Provider/releases). If there is a early release try it out and let me know if you find any bugs!
* Code - Check out the code, play with it. maybe fix a bug while you are there.. It fun and educational. It also makes you super cool, better looking and possibly give you superpowers.


#### Building

All you need to build the code is Visual Sudio 2015 with Visual Studio Extensibility Tools installed. It's **THAT** easy!



## Current status

| Build | Status |
| --- | --- |
| **Release** | [![release][release-badge]][release] |
| **Dev** | [![dev][dev-badge]][dev] |


[release]: https://ci.appveyor.com/project/jzoss/git-source-control-provider
[release-badge]: https://ci.appveyor.com/api/projects/status/pxqohbd79ix57vw5?svg=true
[dev]: https://ci.appveyor.com/project/jzoss/git-source-control-provider-bfftg
[dev-badge]: https://ci.appveyor.com/api/projects/status/qr4hm0uqyr4wnnm9?svg=true

## Change Logs -- Visual Studio 2015

**V-Next** 

**V1.5**

#####---New Features---
*   Added ability to update your .gitingore file to latest version from github
*   Added Track Active Item In Pending Changes - This will help those who have multiple repositories in one solution. It switch the pending changes to the repo for the active file.  
*   Added ability Filter Pending Changes To Only Show Solution Files


#####---Fixes---
*   Fixed issue where plugin could periodically crash Visual Studio.
*   Theme now switches with the need to resart.
*   Double clicking on files in Pending Changes now opens/switches to the file. 
*   No longer slows down when used with Resharper
*   Fixed issue #15, #21 where sometimes the plugin caused Visual Studio to crash. 
*   Fixed issue #16, Solution Explorer glyphs not updating. 


**V1.4.2**

*   Fixed Issues #1, #7, #8, #9, #10, #11 and #12
*   Fixed Switch command
*   Added dark theme form diff window

**V1.4.1**

*   Fixed Settings Error
*   Fix bugs #5 + #6. -Thanks 
*   Thanks To  teebee76 (You Rock) for his help, mnadel, PureKrome + NightOwl888, you guys are also super cool. Keep finidng bugs and I will fix them as fast as I can.  

**V1.4**

*   Switch to using LibGit2Sharp, should see a large performace boost.
*   Fix Pending changes window.
*   Fixed reported bug 2 + 3
*   Refactored external diff tools.. Should be able to extend them in the future
*   Cleaned out lots of dead code. 
*   Fix lot's of bugs
*   Removed Git Diff Margin(use https://visualstudiogallery.msdn.microsoft.com/cf49cf30-2ca6-4ea0-b7cc-6a8e0dadc1a8 )
*   Fixed file missing exceptions.

## Notice : Initial Visual Studio 2015 Upgrade
This is a fork of the super awesome [git SCC plugin](https://visualstudiogallery.msdn.microsoft.com/63a7e40d-4d71-4fbb-a23b-d262124b8f4c) by [Yiyi Sun](https://visualstudiogallery.msdn.microsoft.com/site/search?f[0].Type=User&f[0].Value=yysun)


[Full Changelog](history.md)

Documentation
-----------------
[Source + Documentation](https://github.com/jzoss/Git-Source-Control-Provider)


