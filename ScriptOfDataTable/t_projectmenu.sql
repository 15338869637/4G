/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-26 10:26:06
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_projectmenu
-- ----------------------------
DROP TABLE IF EXISTS `t_projectmenu`;
CREATE TABLE `t_projectmenu` (
  `projectGuid` varchar(32) NOT NULL,
  `menuSerial` int(11) NOT NULL COMMENT '菜单序号',
  `menuName` varchar(255) NOT NULL COMMENT '菜单名称',
  `pageUrl` varchar(255) NOT NULL COMMENT '关联的页面地址',
  PRIMARY KEY (`projectGuid`,`menuSerial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
