/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-05 15:50:42
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_blacklist
-- ----------------------------
DROP TABLE IF EXISTS `t_blacklist`;
CREATE TABLE `t_blacklist` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `carNo` varchar(20) NOT NULL,
  `entityContent` text NOT NULL COMMENT '实体内容',
  PRIMARY KEY (`parkCode`,`carNo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
