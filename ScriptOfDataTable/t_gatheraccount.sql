/*
Navicat MySQL Data Transfer

Source Server         : ThisPc
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : 4gcamera

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2018-11-06 11:20:26
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_gatheraccount
-- ----------------------------
DROP TABLE IF EXISTS `t_gatheraccount`;
CREATE TABLE `t_gatheraccount` (
  `projectGuid` varchar(32) NOT NULL,
  `guid` varchar(32) NOT NULL,
  `accountName` varchar(255) NOT NULL,
  `aliEntityContent` text COMMENT '支付宝具体支付账号的实体内容',
  `wxEntityContent` text COMMENT '微信具体支付账号的实体内容',
  PRIMARY KEY (`guid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
