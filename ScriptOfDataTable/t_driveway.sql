/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-05 15:51:16
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_driveway
-- ----------------------------
DROP TABLE IF EXISTS `t_driveway`;
CREATE TABLE `t_driveway` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `guid` varchar(32) NOT NULL,
  `drivewayName` varchar(255) NOT NULL,
  `type` int(11) DEFAULT '0' COMMENT '0=入口车道 1=出口车道',
  `deviceName` varchar(255) NOT NULL COMMENT '关联设备名',
  `deviceMacAddress` varchar(255) NOT NULL COMMENT '设备唯一地址 如MAC地址等',
  `deviceEntranceURI` varchar(500) DEFAULT '' COMMENT '设备管理登录地址',
  `deviceAccount` varchar(255) DEFAULT NULL COMMENT '设备管理登录账户',
  `deviceVerification` varchar(255) DEFAULT NULL COMMENT '设备管理登录口令',
  PRIMARY KEY (`guid`),
  UNIQUE KEY `macAddressUnique` (`deviceMacAddress`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
