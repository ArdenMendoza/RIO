Create table TblAudioDetails(id int, SongTitle nvarchar, SongArtist nvarchar, SongPath nvarchar, SongGrammar nvarchar,PRIMARY KEY (id));
Drop Table TblAudioDetails;

Create table TblCmdTypes(id int, CmdName nvarchar,PRIMARY KEY (id));
Drop table TblCmdTypes;

Create table TblSysCommands(id int, CmdType int,Cmd nvarchar,PRIMARY KEY (id));
Drop table TblSysCommands;

Create table TblUserCommands(id int, UsrCmds nvarchar, JrvsRspns nvarchar, ShellLoc nvarchar, PRIMARY KEY (id));
Drop table TblUserCommands;

Create table TblVidDetails(id int, VidPath nvarchar, VidName nvarchar, PRIMARY KEY (id));
Drop table TblVidDetails;


INSERT INTO TblAudioDetails("id", "SongTitle") VALUES(1,'Sample Song Title');, '1','mp3','song tag Error by 1');


