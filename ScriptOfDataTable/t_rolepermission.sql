/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-27 09:12:04
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_rolepermission
-- ----------------------------
DROP TABLE IF EXISTS `t_rolepermission`;
CREATE TABLE `t_rolepermission` (
  `projectGuid` varchar(32) NOT NULL,
  `guid` varchar(32) NOT NULL,
  `roleName` varchar(255) DEFAULT NULL,
  `contentDetial` varchar(255) DEFAULT NULL COMMENT '权限详情',
  `parkingCodeList` text CHARACTER SET utf8 COLLATE utf8_general_ci COMMENT '授权停车场集合',
  `isAdmin` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否管理员',
  PRIMARY KEY (`guid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
