/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-19 13:30:46
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_voicecommand
-- ----------------------------
DROP TABLE IF EXISTS `t_voicecommand`;
CREATE TABLE `t_voicecommand` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `drivewayGuid` varchar(32) NOT NULL,
  `entityContent` text COMMENT '具体的实体内容',
  PRIMARY KEY (`drivewayGuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
