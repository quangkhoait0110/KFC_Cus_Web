create database KFC_APP
go
use KFC_APP
go
create table account(
	userName varchar(50) primary key,
	passWord varchar(50),
	name nvarchar(max),
	address nvarchar(max),
	phone varchar(10),
)
go
create table foodCategory(
	id INT IDENTITY(1,1) primary key,
	name nvarchar(max),
	image nvarchar(max)
)
go
create table food(
	id INT IDENTITY(1,1) primary key,
	idCategory int,
	name nvarchar(max),
	image nvarchar(max),
	price float,
	discount float,
	description nvarchar(max),
	timeSellStart date,
	timeSellEnd date,
	foreign key (idCategory) references foodCategory (id)
)
go
create table cart(
	id INT IDENTITY(1,1) primary key,
	idFood int,
	userName varchar(50),
	quantity int,
	totalPrice float,
	foreign key (idFood) references food (id),
	foreign key (userName) references account (userName)
)
go
create table discountCode(
	id INT IDENTITY(1,1) primary key,
	code varchar(50),
	discount float,
)
go
insert into account (username, passWord) values ('admin', '123');