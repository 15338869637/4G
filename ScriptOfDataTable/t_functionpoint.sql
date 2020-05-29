/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2019-01-16 14:29:20
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_functionpoint
-- ----------------------------
DROP TABLE IF EXISTS `t_functionpoint`;
CREATE TABLE `t_functionpoint` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `entityContent` text NOT NULL,
  PRIMARY KEY (`projectGuid`,`parkCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
