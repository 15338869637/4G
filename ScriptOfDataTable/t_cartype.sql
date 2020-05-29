/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2019-03-07 16:53:46
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_cartype
-- ----------------------------
DROP TABLE IF EXISTS `t_cartype`;
CREATE TABLE `t_cartype` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `guid` varchar(32) NOT NULL,
  `carTypeName` varchar(255) NOT NULL,
  `carType` int(11) DEFAULT NULL COMMENT '0=时租车 1=月租车 2=储值车 3=贵宾车',
  `defaultType` int(11) DEFAULT '0' COMMENT '是否是默认车类，仅对临时车类有效',
  `enable` int(11) DEFAULT NULL,
  `idx` varchar(2) NOT NULL DEFAULT '' COMMENT '用于用户输入的2位长度，唯一标识该车类的字符串，主要是用于上传模板到线上平台',
  PRIMARY KEY (`guid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
