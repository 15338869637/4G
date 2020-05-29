/*
Navicat MySQL Data Transfer

Source Server         : 4GCameraTest115
Source Server Version : 80015
Source Host           : 192.168.15.115:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 80015
File Encoding         : 65001

Date: 2019-07-24 15:16:07
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_monthcard
-- ----------------------------
DROP TABLE IF EXISTS `t_monthcard`;
CREATE TABLE `t_monthcard` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `projectGuid` varchar(32) NOT NULL,
  `identifying` varchar(100) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `carNo` varchar(200) NOT NULL COMMENT '唯一标识 车场编码+车牌base64',
  `carOwnerName` varchar(200) NOT NULL,
  `mobile` varchar(50) NOT NULL,
  `carTypeGuid` varchar(32) NOT NULL COMMENT '所属卡类的guid',
  `remark` text COMMENT '备注字段',
  `locked` int(255) DEFAULT NULL,
  `enable` int(255) DEFAULT '1' COMMENT '0=无效 1=有效',
  `startDate` varchar(20) DEFAULT NULL COMMENT '开始时间',
  `endDate` varchar(20) DEFAULT NULL COMMENT '结束时间',
  `pauseDate` varchar(20) DEFAULT NULL COMMENT '报停日期',
  `continueDate` varchar(20) DEFAULT NULL COMMENT '激活日期，报停时指定激活日期的',
  `drivewayListContent` text COMMENT '授权车道集合',
  `rechargeOperator` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL COMMENT '操作员',
  PRIMARY KEY (`id`),
  UNIQUE KEY `identifyinUnique` (`identifying`) USING BTREE,
  KEY `moreNormal` (`projectGuid`,`parkCode`,`carOwnerName`,`mobile`,`startDate`,`locked`,`carNo`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8;
