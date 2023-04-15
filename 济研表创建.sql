
/*定义users数据表*/
CREATE TABLE users(
 [username] [varchar](20) NOT NULL primary key,
 [code] [char](8) not NULL,
 [email] [varchar](40) not NULL)

/*定义literature_website数据表*/
create table literature_website
(
  lname varchar(100) not null,
  username varchar(20) not null 
           FOREIGN KEY
	       REFERENCES users,
  llink varchar(100),
  category varchar(100),
  descreption varchar(100),
  icon image not null,
  clicks INT,
  PRIMARY KEY(lname,username)
)

/*定义enword数据表*/
create table enword
(en varchar(35) not null,
ch varchar(400),
primary key(en)
)

/*定义wordtypes数据表*/
create table wordtypes
(
en varchar(35) foreign key references enword(en),
entypes char(7)
primary key(en,entypes)
)

/*定义wordrecord数据表*/
create table wordrecord
(
en varchar(35) foreign key references enword(en),
entimes int,
username varchar(20) foreign key references users(username)
primary key(en,username)
)

/*定义history数据表*/
create table history
(historyfilename varchar(100),
historyfilepath varchar(200) not null,
historyfiletype varchar(20),
username varchar(20) foreign key references users(username),
historytime datetime
primary key(historyfilename,historytime,username))


/*定义编程软件保存数据表saveexe*/
 create table saveexe
(
username varchar(20) not null  FOREIGN KEY REFERENCES users,
name varchar(20) not null ,
spath varchar(255) not null ,
category varchar(20) ,
rename varchar(20),
pic image,
times int
primary key(username,spath)
)