/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-08 11:05:27
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_permanenttemplate
-- ----------------------------
DROP TABLE IF EXISTS `t_permanenttemplate`;
CREATE TABLE `t_permanenttemplate` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `carTypeGuid` varchar(32) NOT NULL COMMENT '卡类型(对应的车类型的guid,t_cartype表中的)',
  `months` int(255) DEFAULT '1' NOT NULL COMMENT '月数',
  `amount` decimal(6,0) DEFAULT NULL COMMENT '金额',
  `operateTime` varchar(255) NOT NULL COMMENT '最后一次操作时间 yyyy-MM-dd HH:mm:ss',
  `operateUser` varchar(255) NOT NULL COMMENT '最后一次操作人',
  PRIMARY KEY (`carTypeGuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
