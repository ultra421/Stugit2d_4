create database unityusers;
use unitysers;

create table users(
	id int primary key auto_increment,
    username varchar(45),
    password varchar(45)
);