/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-12-28 09:51:43
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_project
-- ----------------------------
DROP TABLE IF EXISTS `t_project`;
CREATE TABLE `t_project` (
  `projectGuid` varchar(32) NOT NULL,
  `projectName` varchar(255) NOT NULL,
  `projectAccount` varchar(50) NOT NULL,
  `createTime` varchar(20) NOT NULL,
  PRIMARY KEY (`projectGuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
