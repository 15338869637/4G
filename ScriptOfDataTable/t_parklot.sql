/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-01 14:34:19
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_parklot
-- ----------------------------
DROP TABLE IF EXISTS `t_parklot`;
CREATE TABLE `t_parklot` (
  `projectGuid` varchar(32) NOT NULL,
  `parkCode` varchar(20) NOT NULL,
  `parkName` varchar(255) NOT NULL,
  `entityContent` text NOT NULL COMMENT '实体内容',
  `existence` int(11) DEFAULT '0' COMMENT '存续标志 0=不启用 1=启用',
  PRIMARY KEY (`parkCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
