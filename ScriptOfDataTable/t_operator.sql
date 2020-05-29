/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-12-28 17:44:12
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_operator
-- ----------------------------
DROP TABLE IF EXISTS `t_operator`;
CREATE TABLE `t_operator` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) DEFAULT NULL,
  `guid` varchar(32) NOT NULL,
  `userName` varchar(255) NOT NULL,
  `userPswd` varchar(255) NOT NULL,
  `mobile` varchar(11) NOT NULL,
  `privilege` varchar(500) NOT NULL DEFAULT '0',
  `roleGuid` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`mobile`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
